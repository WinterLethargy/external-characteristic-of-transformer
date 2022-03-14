using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MyClass m1 = new MyClass { i = 1 };
            List<MyClass> list = new List<MyClass>();
            list.Add(m1);
            list.Add(m1);
            Console.WriteLine(list.Count);
            Console.WriteLine(list[0] == list[1]);
            Console.WriteLine();

            list.Remove(m1);
            Console.WriteLine(list.Count);
            Console.WriteLine();
        }
    }

    public class MyClass
    {
        public int i;
    }
}
