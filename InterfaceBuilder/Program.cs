using System;
using System.Collections.Generic;
using InterfaceBuilder.CompositeGeneration;

namespace InterfaceBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var mocks = new List<IFooable>
            {
                new Mock(),
                new NotSavableMock(),
                new Mock(),
                new NotSavableMock(),
                new Mock(),
            };
            
            var creator = new CompositeCreator();
            
            var instance = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                instance.Add(mock);
            }
            
            IFooable fooable = instance as IFooable;

            fooable!.Foo();
            
            CompositeHelper.Perform();
            Console.Read();
        }
    }


    public interface Test<out T>
    {
        void Group(Action<T> s);
    }
}