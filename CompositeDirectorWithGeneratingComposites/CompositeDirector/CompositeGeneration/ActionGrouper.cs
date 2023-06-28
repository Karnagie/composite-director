using System;
using System.Collections.Generic;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration
{
    public static class ActionGrouper
    {
        public static Result Now(this Result result)
        {
            CompositeHelper.PerformLast();
            return default;
        }
        
        public static Result For<T>(this Result result)
        {
            var last = CompositeHelper.Last;
            last.Apply(new CommandFor<T>());
            
            return default;
        }
        
        public static Result For<T>(this Result result, Func<T, bool> condition) where T : class
        {
            var last = CompositeHelper.Last;
            last.Apply(new CommandForWithCondition<T>(condition));
            
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
    
    public class CommandForWithCondition<TU> : ICommand where TU : class
    {
        private Func<TU, bool> _condition;

        public CommandForWithCondition(Func<TU, bool> condition)
        {
            _condition = condition;
        }
        
        public T[] Do<T>(T[] items)
        {
            List<T> stash = new List<T>();
            foreach (var item in items)
            {
                if (item is TU && _condition(item as TU))
                {
                    stash.Add(item);
                }
            }
            return stash.ToArray();
        }
    }
}