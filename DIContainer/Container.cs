using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class Container
    {
        private Assembly AssemblyInfo { get; set; }
        private Dictionary<Type, Type> ResovingDictionary { get; } = new Dictionary<Type, Type>();

        public void AddAssembly(Assembly assembly)
        {
            AssemblyInfo = assembly;
            var typesToResolve = AssemblyInfo.GetTypes().Where(n => Attribute.IsDefined(n, typeof(ExportAttribute)));
            foreach (var type in typesToResolve)
            {
                var attribute = (ExportAttribute) type.GetCustomAttribute(typeof(ExportAttribute));
                if (attribute.Contract != null)
                {
                    AddType(type, attribute.Contract);
                }
                else
                {
                    AddType(type);
                }
            }
        }

        public void AddType(Type type)
        {
            AddType(type, type);
        }

        public void AddType(Type type, Type baseType)
        {
            if (ResovingDictionary.ContainsKey(type))
            {
                throw new ArgumentException("This type has been alredy added");
            }
            if (type.IsAbstract || type.IsInterface)
            {
                throw new ArgumentException("Can't resolve type on abstract class or interface");
            }
            ResovingDictionary.Add(baseType, type);
        }

        private bool IsCircularDependencySafe(Type type, List<Type> listOfTypes = null)
        {
            //if (listOfTypes == null)
            //{
            //    listOfTypes = new List<Type>();
            //}
            //if (listOfTypes.Contains(type))
            //{
            //    return false;
            //}
            //listOfTypes.Add(type);
            //if ((type.IsAbstract || type.IsInterface) && ResovingDictionary.ContainsKey(type))
            //{
            //    return IsCircularDependencySafe(ResovingDictionary[type], listOfTypes);
            //}
            //if (Attribute.IsDefined(type, typeof(ImportConstructorAttribute)))
            //{
            //    var ctorParams = GetSinglePossibleCtor(type).GetParameters();
            //    if (ctorParams.Length == 0)
            //    {
            //        return true;
            //    }
            //    return ctorParams.Select(n => n.ParameterType).All(n => IsCircularDependencySafe(n, listOfTypes));
            //}
            //var properties = type.GetProperties().Where(n => n.IsDefined(typeof(ImportAttribute))).Select(n => n.PropertyType);
            //var fields = type.GetFields().Where(n => n.IsDefined(typeof(ImportAttribute))).Select(n => n.FieldType);
            //return properties.All(n => IsCircularDependencySafe(n, listOfTypes)) && fields.All(n => IsCircularDependencySafe(n, listOfTypes));
            return true;
        }

        public object CreateInstance(Type type)
        {
            if (!IsCircularDependencySafe(type))
            {
                throw new ArgumentException("Circular dependencies was found");
            }
            if (type.IsAbstract || type.IsInterface)
            {
                if (!ResovingDictionary.ContainsKey(type))
                {
                    throw new ArgumentException($"Can't resolve type {type}");
                }
                return CreateInstance(ResovingDictionary[type]);
            }
            if (Attribute.IsDefined(type, typeof(ImportConstructorAttribute)))
            {
                return CreateInstanceWithConstructor(type);
            }
            return CreateInstanceWithProperties(type);
        }

        private object CreateInstanceWithConstructor(Type type)
        {
            ConstructorInfo ctor = GetSinglePossibleCtor(type);
            List<object> ctorParamInstances = new List<object>();
            foreach (var param in ctor.GetParameters())
            {
                ctorParamInstances.Add(CreateInstance(ResovingDictionary[param.ParameterType]));
            }
            return ctor.Invoke(ctorParamInstances.ToArray());
        }

        private ConstructorInfo GetSinglePossibleCtor(Type type)
        {
            var possibleCtors = type.GetConstructors().GetResolvableConstuctors(ResovingDictionary.Values);
            if (possibleCtors.Count() != 1)
            {
                var markedCtors = type.GetConstructors().Where(n => n.IsDefined(typeof(InjectedConstructorAttribute)));
                if (markedCtors.Count() != 1)
                {
                    throw new ArgumentException("Target type contains unresolvable ctors");
                }
                return markedCtors.First();
            }
            return possibleCtors.First();
        }

        private object CreateInstanceWithProperties(Type type)
        {
            var ctorWithoutPaarams = type.GetConstructors().FirstOrDefault(n => n.GetParameters().Length == 0);
            if (ctorWithoutPaarams == null)
            {
                throw new ArgumentException("There no ctor wiithout params in target type");
            }
            var instance = ctorWithoutPaarams.Invoke(null);

            var properties = type.GetProperties().Where(n => n.IsDefined(typeof(ImportAttribute)));
            foreach (var property in properties)
            {
                property.SetValue(instance, CreateInstance(ResovingDictionary[property.PropertyType]));
            }

            var fields = type.GetFields().Where(n => n.IsDefined(typeof(ImportAttribute)));
            foreach (var field in fields)
            {
                field.SetValue(instance, CreateInstance(ResovingDictionary[field.FieldType]));
            }

            return instance;
        }

        public T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }
    }
}
