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
        
        IPool<ITest> comp = this;
        Action<ITest[]> func = tests =>
        {
            Foo1Internal(tests, value);
        };
        
        CompositeHelper.Group(func, comp);

        return default;
    }

    private void Foo1Internal(ITest[] tests, string value)
    {
        for (int i = 0; i < tests.Length; i++)
        {
            tests[i].FooWithArgs(value);
        }
    }
    
    public Result FooWithTwoArgs(string value, int value1)
    {
        CompositeHelper.Group(tests =>
        {
            Foo1Internal(tests, value, value1);
        }, this);

        return default;
    }

    public void FooWithTwoArgs(ITest[] tests, string value1, int i, string valu, int val, int val1)
    {
        throw new NotImplementedException();
    }

    public Result FooWithTwoArgs1(string value, int value1, string valu, int val, int val1)
    {
        CompositeHelper.Group(tests =>
        {
            Foo1Internal(tests, value, value1, valu, val, val1);
        }, this);

        return default;
    }

    private void Foo1Internal(ITest[] tests, string value, int value1, string valu, int val, int val1)
    {
        for (int i = 0; i < tests.Length; i++)
        {
            tests[i].FooWithTwoArgs(tests, value, value1, valu, val, val1);
        }
    }

    private void Foo1Internal(ITest[] tests, string value, int value1)
    {
        for (int i = 0; i < tests.Length; i++)
        {
            tests[i].FooWithTwoArgs(value, value1);
        }
    }
}

public interface ITest
{
    Result Foo();
    Result FooWithArgs(string value);
    Result FooWithTwoArgs(string value, int value1);
    void FooWithTwoArgs(ITest[] tests, string value1, int i, string valu, int val, int val1);
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

    public Result FooWithTwoArgs(string value, int value1)
    {
        Console.WriteLine($"1//{value}+{value1}");

        return default;
    }

    public void FooWithTwoArgs(ITest[] tests, string value1, int i, string valu, int val, int val1)
    {
        throw new NotImplementedException();
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