using System;
using System.Collections.Generic;
using System.Linq;
using InterfaceBuilder;

public static class CompositeHelper
{
    private static readonly HashSet<ILayer> Stash = new();
    public static ILayer Last => Stash.Last();

    public static void Group<T>(Action<T[]> process, IPool<T> composite)
    {
        Stash.Add(new CompositeDecorator<T>(process, composite.Items.ToArray()));
    }

    public static void Perform()
    {
        foreach (var layer in Stash)
        {
            layer.Perform();
        }
        Stash.Clear();
    }
}


public interface ILayer
{
    void Perform(); 
    void Apply(ICommand command);
}

public interface IItemsHandler<T> : ILayer
{
    T[] Items { get; }
}

public interface ICommand
{
    T[] Do<T>(T[] items);
}