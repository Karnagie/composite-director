using System;

public interface IFooable
{
    Result Foo();
    Result FooWithArgs(int name);
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
        Console.WriteLine(name);
        return default;
    }

    public Result Save()
    {
        Console.WriteLine("Mock saved");
        return default;
    }
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
}

public interface ISavable
{
    Result Save();
}