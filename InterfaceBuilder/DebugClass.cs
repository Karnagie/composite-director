 using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using InterfaceBuilder;

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
        CompositeHelper.Group(FooInternal, this);

        return default;
    }
    
    private void FooInternal(ITest[] tests)
    {
        for (int i = 0; i < tests.Length; i++)
        {
            Items[i].Foo();
        }
    }
    
    public Result FooWithArgs(string value)
    {
        CompositeHelper.Group(tests =>
        {
            Foo1Internal(tests, value);
        }, this);

        return default;
    }
    
    private void Foo1Internal(ITest[] tests, string value)
    {
        for (int i = 0; i < tests.Length; i++)
        {
            tests[i].FooWithArgs(value);
        }
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
}

public class Test1 : ITest, ILayer
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

    public void Perform()
    {
        throw new NotImplementedException();
    }

    public void Apply(ICommand command)
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