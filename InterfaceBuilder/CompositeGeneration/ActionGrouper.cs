using System;
using System.Collections.Generic;
using System.Linq;

namespace InterfaceBuilder
{
    public static class ActionGrouper
    {
        public static Result ForAll(this Result result)
        {
            // TODO remove
            return default;
        }
        
        public static Result For<T>(this Result result)
        {
            var last = CompositeHelper.Last;
            last.Apply(new CommandFor<T>());
            
            return default;
        }
    }

    public class CommandFor<TExcept> : ICommand
    {
        public T[] Do<T>(T[] items)
        {
            List<T> stash = new List<T>();
            foreach (var item in items)
            {
                if (item is TExcept)
                {
                    stash.Add(item);
                }
            }
            return stash.ToArray();
        }
    }
}