using System.Collections.Generic;
using CompositeDirectorOld.CD;
using CompositeDirectorOld.CD.Composites;
using CompositeDirectorOld.Mocks;

namespace CompositeDirectorOld
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var composites = new List<IProcessComposite>()
            {
                new MockComposite(),
            };
            
            var executors = new List<IOldFooable>()
            {
                new Mock(),
                new NotSavableMock(),
                new Mock(),
                new NotSavableMock(),
                new Mock(),
            };
            
            var director = new CompositeDirector(composites);
            foreach (var executor in executors)
            {
                director.TryAddExecutor(executor);
            }

            director.SelectComposite<IOldFooable>().ForAll().Do().Foo();
            director.SelectComposite<IOldFooable>()
                .ForAll()
                .Except<NotSavableMock>()
                .Do().FooWithArgs("Execute only for all except savable mocks");

        }
    }
}