﻿using System;
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
            
            var fooable = instance as IFooable;

            //fooable.Foo();
            fooable!.FooWithArgs(10).For<ISavable>();
            //fooable.Foo().For<ILayer>();
            //fooable.FooWithArgs("Testing");
            CompositeHelper.Perform();
            
            
            //
            //var creator = new CompositeCreator();
            // IPool<ITest> instance = new TestComposite();//creator.Create<IFooable>();
            // foreach (var mock in mocks)
            // {
            //     instance.Add(mock);
            // }

            //var fooable = instance as ITest;
            // var savable = creator.Create<ISavable>();
            // foreach (var mock in mocks)
            // {
            //     if(mock is ISavable savableItem)savable.Add(savableItem);
            // }
            //

            // fooable.Foo();
            // fooable.Foo().For<ILayer>();
            // fooable.FooWithArgs("Testing");
            // CompositeHelper.Perform();

            // var composite = instance as IFooable;
            // var savableComposite = savable as ISavable;
            // savableComposite.Save().ForAll();

            //add this later for multi thread
            // lock (savableComposite)
            // {
            //     savableComposite.Save().ForAll();
            // }
        }
    }


    public interface Test<out T>
    {
        void Group(Action<T> s);
    }
}