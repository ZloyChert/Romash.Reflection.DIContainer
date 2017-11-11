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
            if (ResovingDictionary.ContainsKey(type))
            {
                throw new ArgumentException("This type has been alredy added");
            }
            ResovingDictionary.Add(type, type);
        }

        public void AddType(Type type, Type baseType)
        {
            ResovingDictionary.Add(baseType, type);
        }

        public object CreateInstance(Type type)
        {
            if (Attribute.IsDefined(type, typeof(ImportConstructorAttribute)))
            {
                return CreateInstanceWithConstructor(type);
            }
            return CreateInstanceWithProperties(type);
        }

        private object CreateInstanceWithConstructor(Type type)
        {
            var possibleCtors = type.GetConstructors().GetResolvableConstuctors(ResovingDictionary.Values);
            if (possibleCtors.Count() != 1)
            {
                throw new ArgumentException("Target type contains unresolvable ctors");
            }
            ConstructorInfo ctor = possibleCtors.First();
            List<object> ctorParamInstances = new List<object>();
            foreach (var param in ctor.GetParameters())
            {
                ctorParamInstances.Add(CreateInstance(ResovingDictionary[param.ParameterType]));
            }
            return ctor.Invoke(ctorParamInstances.ToArray());
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
                property.SetValue(instance, CreateInstance(property.PropertyType));
            }

            var fields = type.GetFields().Where(n => n.IsDefined(typeof(ImportAttribute)));
            foreach (var field in fields)
            {
                field.SetValue(instance, CreateInstance(field.FieldType));
            }

            return instance;
        }

        public T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }
    }
}
