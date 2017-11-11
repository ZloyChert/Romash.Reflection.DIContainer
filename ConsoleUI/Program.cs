using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DIContainer;
using InjectedAssembly;

namespace ConsoleUI
{
    class Creating
    {
        public Creating(Stub s)
        { }
    }

    class Stub
    { }

    class Program
    {
        static void Main(string[] args)
        {
            Container di = new Container();
            di.AddAssembly(Assembly.GetAssembly(typeof(ClassWithNoParamsCtor)));
            ClassWithNoParamsCtor one = di.CreateInstance<ClassWithNoParamsCtor>();
            ClassWithDifficultCtor two = di.CreateInstance<ClassWithDifficultCtor>();
            ClassWithSimpleCtor three = di.CreateInstance<ClassWithSimpleCtor>();
            ClassWithDifficultFields four = di.CreateInstance<ClassWithDifficultFields>();




            Console.Read();
        }
    }
}
