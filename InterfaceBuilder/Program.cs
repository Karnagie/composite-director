using System;
using System.Collections.Generic;

namespace InterfaceBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var mocks = new List<Mock>
            {
                new (),
                new (),
                new (),
                new (),
                new (),
            };
            
            var creator = new CompositeCreator();
            var instance = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                instance.Add(mock);
            }
            
            var fooable = instance as IFooable;
            
            fooable!.Foo();
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