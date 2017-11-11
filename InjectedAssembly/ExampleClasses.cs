using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIContainer;

namespace InjectedAssembly
{
    [ImportConstructor]
    public class ClassWithNoParamsCtor
    {
        public ClassWithNoParamsCtor()
        { }
    }

    [ImportConstructor]
    public class ClassWithDifficultCtor
    {
        public ClassWithDifficultCtor(BaseClassToInject baseClass, IInterface inter, StubClassToInject stubClass)
        { }
    }

    [ImportConstructor]
    public class ClassWithSimpleCtor
    {
        public ClassWithSimpleCtor(IInterface inter)
        { }
    }

    public class BaseClassToInject
    {
        public BaseClassToInject()
        { }
    }

    [Export(typeof(BaseClassToInject))]
    [ImportConstructor]
    public class DerivedClassToInject : BaseClassToInject
    {
        public DerivedClassToInject(StubClassToInject a)
        { }
    }

    public interface IInterface
    {
    }

    [Export(typeof(IInterface))]
    [ImportConstructor]
    public class StubClassToInjectWithInterface : IInterface
    {
        public StubClassToInjectWithInterface(BaseClassToInject baseClass)
        { }
    }

    [Export]
    public class StubClassToInject
    {
        public StubClassToInject()
        { }
    }
}
