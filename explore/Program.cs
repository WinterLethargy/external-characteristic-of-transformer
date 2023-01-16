using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace explore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var v = new MyClass();
            Console.WriteLine(v.Act == v.Act1);

            v.Act = v.Act2;
            Console.WriteLine(v.Act == v.Act1);
            Console.WriteLine(v.Act == v.Act2);
        }
    }

    public class MyClass
    {

        public Action Act;

        public void Act1() { }
        public void Act2() { }

        public MyClass()
        {
            Act = Act1;
        }
    }
}
