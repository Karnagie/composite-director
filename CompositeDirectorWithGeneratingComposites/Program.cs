using System.Collections.Generic;
using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.Mocks;

namespace CompositeDirectorWithGeneratingComposites
{
    class Program
    {
        static void Main(string[] args)
        {
            //Entities
            var mocks = new List<IFooable>
            {
                new Mock(),
                new NotSavableMock(),
                new Mock(),
                new NotSavableMock(),
                new Mock(),
            };
            var anotherMocks = new List<IAnotherFooable>
            {
                new AnotherMock(),
                new AnotherMock(),
                new AnotherMock(),
            };
            
            //Director's initializing
            var director = new CompositeDirector.CompositeDirector();
            director.SetupComposite<IFooable>();
            director.SetupComposite<IAnotherFooable>();

            //Add mock to director
            foreach (var mock in mocks)
            {
                director.Add(mock);
            }
            foreach (var mock in anotherMocks)
            {
                director.Add(mock);
            }

            //Getting composites
            IFooable fooable = director.GetComposite<IFooable>();
            IAnotherFooable anotherFooable = director.GetComposite<IAnotherFooable>();
            
            //Using composites
            fooable!.Foo();
            fooable.FooWithArgs("'passed parameter'");
            anotherFooable.AnotherFoo();
            
            //Performing all last commands that cached and not performed
            CompositeHelper.Perform();
            
            //Disposing all composites
            director.Dispose();
        }
    }
}