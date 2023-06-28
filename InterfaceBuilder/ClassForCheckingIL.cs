using System;
using System.Collections.Generic;
using InterfaceBuilder.CompositeGeneration;

namespace InterfaceBuilder
{
    public class TestComposite : IPool<ITest>, ITest
    {
        private Dictionary<ITest, Action> _removeActions = new();

        public List<ITest> Items { get; } = new ();

        public void Add(ITest item)
        {
            Items.Add(item);
            
            Action removing = Removing(item);
            _removeActions.Add(item, removing);
            item.Disposed += removing;
        }

        public void Remove(ITest item)
        {
            Items.Remove(item);

            item.Disposed -= _removeActions[item];
            _removeActions.Remove(item);
        }

        private Action Removing(ITest item)
        {
            return () => Remove(item);
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

        public void Dispose()
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                item.Dispose();
            }
            Items.Clear();
            
            Disposed?.Invoke();
        }

        public event Action Disposed;
    }

    public interface ITest : IPoolItem
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

        public void FooWithTwoArgs(ITest[] tests, string value1, int i, string value, int val, int val1)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose");
            Disposed?.Invoke();
        }

        public event Action Disposed;
    }
}