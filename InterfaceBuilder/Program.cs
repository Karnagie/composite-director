﻿using System.Collections.Generic;
using InterfaceBuilder.CompositeGeneration;
using InterfaceBuilder.Mocks;

namespace InterfaceBuilder
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var mocks = new List<IFooable>
            {
                new Mock(),
                new NotSavableMock(),
                new Mock(),
                new NotSavableMock(),
                new Mock(),
            };
            
            var creator = new CompositeCreator();
            
            var instance = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                instance.Add(mock);
            }
            
            IFooable fooable = instance as IFooable;

            fooable!.Foo().Now();
            mocks[0].Dispose();
            mocks[1].Dispose();
            fooable.FooWithArgs("'passed parameter'").Now();
            fooable.Dispose();

            //CompositeHelper.Perform();
        }
    }
}