using System;
using System.Collections.Generic;
using InterfaceBuilder.CompositeGeneration;

namespace InterfaceBuilder
{
    public class TestComposite : IPool<ITest>, ITest
    {
        public List<ITest> Items { get; } = new ();

        public void Add(ITest item)
        {
            Items.Add(item);
        }
    
        public void Remove(ITest item)
        {
            Items.Remove(item);
        }

        public Result Foo()
        {
            IPool<ITest> comp = this;
            Action<ITest[]> func = new Action<ITest[]>(FooInternal);
        
            CompositeHelper.Group(func, comp);

            return default;
        }
    
        private void FooInternal(ITest[] tests)
        {
            for (int i = 0; i < tests.Length; i++)
            {
                tests[i].Foo();
            }
        }
    
        public Result FooWithArgs(string value)
        {
            var test = new Test();
            IPool<ITest> comp = this;
            Action<ITest[]> func = tests =>
            {
                FooWithArgsInternal(tests, value);
            };
            CompositeHelper.Group(func, comp);

            return default;
        }

        private void FooWithArgsInternal(ITest[] tests, string value)
        {
            for (int i = 0; i < tests.Length; i++)
            {
                tests[i].FooWithArgs(value);
            }
        }
    
        public class Test
        {
            private TestComposite _composite;
        }
    }

    public interface ITest
    {
        Result Foo();
        Result FooWithArgs(string value);
    }

    public class Test : ITest
    {
        public Result Foo()
        {
            Console.WriteLine("Foo");

            return default;
        }

        public Result FooWithArgs(string value)
        {
            Console.WriteLine($"Foo+{value}");

            return default;
        }

        public Result FooWithTwoArgs(string value, int value1)
        {
            Console.WriteLine($"{value}+{value1}");

            return default;
        }

        public void FooWithTwoArgs(ITest[] tests, string value1, int i, string valu, int val, int val1)
        {
            throw new NotImplementedException();
        }
    }

    public interface IPool<T> 
    {
        void Add(T item);
        void Remove(T item);
    
        List<T> Items { get; }
    }
}