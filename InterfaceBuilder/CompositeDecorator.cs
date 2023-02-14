using System;
using InterfaceBuilder.Mocks;

namespace InterfaceBuilder
{
    public class CompositeDecorator<T> : IItemsHandler<T>
    {
        private readonly Action<T[]> _process;
        public T[] Items { get; private set; }

        public CompositeDecorator(Action<T[]> process,  T[] items)
        {
            Items = items;
            _process = process;
        }
    
        public void Perform()
        {
            _process.Invoke(Items);
        }

        public void Apply(ICommand command)
        {
            Items = command.Do(Items);
        }
    }
}