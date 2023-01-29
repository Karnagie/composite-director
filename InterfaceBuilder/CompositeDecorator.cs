using System;

public class CompositeDecorator<T> : IItemsHandler<T>
{
    public Action<T[]> Process;
    public T[] Items { get; private set; }

    public CompositeDecorator(Action<T[]> process,  T[] items)
    {
        Items = items;
        Process = process;
    }
    
    public void Perform()
    {
        Process.Invoke(Items);
    }

    public void Apply(ICommand command)
    {
        Items = command.Do(Items);
    }
}