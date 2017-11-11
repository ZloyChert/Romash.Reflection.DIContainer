using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public static class ContainerHelper
    {
        public static IEnumerable<ConstructorInfo> GetResolvableConstuctors(this IEnumerable<ConstructorInfo> ctors, IEnumerable<Type> possibleTypes)
        {
            return ctors.Where(ctor => ctor.IsConstructorResolvable(possibleTypes));
        }

        public static bool IsConstructorResolvable(this ConstructorInfo ctor, IEnumerable<Type> possibleTypes)
        {
            var ctorParams = ctor.GetParameters();
            return ctorParams.All(param => possibleTypes.Any(n => n != param.GetType()));
        }
    }
}
