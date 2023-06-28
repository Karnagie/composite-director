using System;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector.Mocks
{
    public interface IFooable : IPoolItem
    {
        Result Foo();
        Result FooWithArgs(int name);
        Result FooWithArgs(int index, string name);
        Result FooWithArgs(string name);
    }
    
    public interface IAnotherFooable : IPoolItem
    {
        Result AnotherFoo();
    }

    public interface ISpeakable : InterfaceBuilder.IPoolItem
    {
        Result Say(string text);
    }

    public class SpeakableEntity : InterfaceBuilder.Entity, ISpeakable
    {
        public Result Say(string text)
        {
            Console.WriteLine($"says: {text}");
            
            return default;
        }
    }

    public class Mock : IFooable, ISavable
    {
        public Result Foo()
        {
            Console.WriteLine("Foo");
            return default;
        }
    
        public Result FooWithArgs(int name)
        {
            Console.WriteLine($"savable {name}");
            return default;
        }

        public Result FooWithArgs(int index, string name)
        {
            Console.WriteLine($"savable {index} {name}");
            return default;
        }

        public Result FooWithArgs(string name)
        {
            Console.WriteLine($"savable overloaded {name}");
            return default;
        }

        public Result Save()
        {
            Console.WriteLine("Mock saved");
            return default;
        }

        public void Dispose()
        {
            Disposed?.Invoke();
        }

        public event Action Disposed;
    }

    public class NotSavableMock : IFooable
    {
        public Result Foo()
        {
            Console.WriteLine("not save foo");
            return default;
        }
    
        public Result FooWithArgs(int name)
        {
            Console.WriteLine($"not save {name}");
            return default;
        }

        public Result FooWithArgs(int index, string name)
        {
            Console.WriteLine($"not save {index} {name}");
            return default;
        }

        public Result FooWithArgs(string name)
        {
            Console.WriteLine($"not save overloaded {name}");
            return default;
        }

        public void Dispose()
        {
            Disposed?.Invoke();
        }

        public event Action Disposed;
    }

    public class AnotherMock : Entity, IAnotherFooable
    {
        public Result AnotherFoo()
        {
            Console.WriteLine("Another Foo");
            return default;
        }
    }

    public interface ISavable
    {
        Result Save();
    }
}