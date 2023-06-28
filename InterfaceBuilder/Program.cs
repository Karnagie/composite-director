using System.Collections.Generic;
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
            var speakables = new List<ISpeakable>
            {
                new SpeakableEntity(),
                new SpeakableEntity(),
                new SpeakableEntity(),
                new SpeakableEntity(),
                new SpeakableEntity(),
            };
            
            var creator = new CompositeCreator();
            
            var instance = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                instance.Add(mock);
            }
            IFooable fooable = instance as IFooable;
            
            var instanceSpeakable = creator.Create<ISpeakable>();
            foreach (var mock in speakables)
            {
                instanceSpeakable.Add(mock);
            }
            ISpeakable speakable = instanceSpeakable as ISpeakable;
            
            speakable!.Say("bark").Now();
            speakables[0].Dispose();
            speakable!.Say("bark").Now();
            
            fooable!.Foo().Now();
            mocks[0].Dispose();
            mocks[1].Dispose();
            fooable.FooWithArgs("'passed parameter'").Now();
            fooable.Dispose();
        }
    }
}