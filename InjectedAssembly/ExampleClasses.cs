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
        public string CheckField { get; }
        public ClassWithNoParamsCtor()
        {
            CheckField = "ClassWithNoParamsCtor";
        }
    }

    [ImportConstructor]
    public class ClassWithDifficultCtor
    {
        public BaseClassToInject ChechBaseClass { get; }
        public IInterface CheckInterface { get; }
        public StubClassToInject CheckStubClass { get; }
        public ClassWithDifficultCtor(BaseClassToInject baseClass, IInterface inter, StubClassToInject stubClass)
        {
            ChechBaseClass = baseClass;
            CheckInterface = inter;
            CheckStubClass = stubClass;
        }
    }

    public class ClassWithDifficultFields
    {
        [Import]
        public BaseClassToInject ChechBaseClass { get; set; }
        [Import]
        public IInterface CheckInterface { get; set; }
        [Import]
        public StubClassToInject CheckStubClass { get; set; }
    }

    [ImportConstructor]
    public class ClassWithSimpleCtor
    {
        public IInterface CheckInterface { get; }
        public ClassWithSimpleCtor(IInterface inter)
        {
            CheckInterface = inter;
        }
    }

    public class BaseClassToInject
    {
        public string BaseCheckField { get; }
        public BaseClassToInject()
        {
            BaseCheckField = "BaseClassToInject";
        }
    }

    [Export(typeof(BaseClassToInject))]
    [ImportConstructor]
    public class DerivedClassToInject : BaseClassToInject
    {
        public StubClassToInject CheckStubClass { get; }
        public DerivedClassToInject(StubClassToInject stub)
        {
            CheckStubClass = stub;
        }
    }

    public interface IInterface
    {
    }

    [Export(typeof(IInterface))]
    [ImportConstructor]
    public class StubClassToInjectWithInterface : IInterface
    {
        public BaseClassToInject ChechBaseClass { get; }
        public StubClassToInjectWithInterface(BaseClassToInject baseClass)
        {
            ChechBaseClass = baseClass;
        }
    }

    [Export]
    public class StubClassToInject
    {
        public string CheckField { get; }
        public StubClassToInject()
        {
            CheckField = "StubClassToInject";
        }
    }
}
