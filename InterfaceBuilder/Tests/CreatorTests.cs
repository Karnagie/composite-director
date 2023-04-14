using System;
using System.Collections.Generic;
using InterfaceBuilder.CompositeGeneration;
using InterfaceBuilder.Mocks;
using Moq;
using NUnit.Framework;

namespace InterfaceBuilder.Tests
{
    public class CreatorTests
    {
        [Test]
        public void CallFooOfAllObjects_WhenUseComposite_AndCallFoo()
        {
            // Arrange.
            int count = 0;
            var mocks = new List<Mock<IFooable>>();
            for (int i = 0; i < 10; i++)
            {
                mocks.Add(new Mock<IFooable>());
            }
            
            for (int i = 0; i < 10; i++)
            {
                mocks[i].Setup((fooable => fooable.Foo())).Callback((() => count++));
            }

            var creator = new CompositeCreator();
            var fooablePool = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                fooablePool.Add(mock.Object);
            }
            var fooableComposite = fooablePool as IFooable;
            
            // Act.
            fooableComposite!.Foo().Now();

            // Assert.
            Assert.AreEqual(10, count);
        }
        
        [Test]
        public void Create_CompositeFooable_AndCallFoo()
        {
            // Arrange.
            int count = 0;
            var mocks = new List<Mock<IFooable>>();
            for (int i = 0; i < 10; i++)
            {
                mocks.Add(new Mock<IFooable>());
            }
            
            for (int i = 0; i < 10; i++)
            {
                mocks[i].Setup((fooable => fooable.Foo())).Callback((() => count++));
            }

            var creator = new CompositeCreator();
            var fooablePool = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                fooablePool.Add(mock.Object);
            }
            var fooableComposite = fooablePool as IFooable;
            
            // Act.
            fooableComposite!.Foo().Now();

            // Assert.
            Assert.AreEqual(10, count);
        }
        
        [Test]
        public void Create_CompositeFooable_AndCallFooWithArgs()
        {
            // Arrange.
            List<char> answers = new (10);
            var mocks = new List<Mock<IFooable>>();
            for (int i = 0; i < 10; i++)
            {
                mocks.Add(new Mock<IFooable>());
            }
            Action addingAnswer = () => answers.Add('1');

            for (int i = 0; i < 10; i++)
            {
                mocks[i].Setup((fooable => fooable.FooWithArgs(It.IsAny<string>())))
                    .Callback<string>(_ => addingAnswer.Invoke())
                    .Returns(Result.Ok);
            }

            var creator = new CompositeCreator();
            var fooablePool = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                var obj = mock.Object;
                obj.FooWithArgs("1");
                fooablePool.Add(obj);
            }
            var fooableComposite = fooablePool as IFooable;
            
            // Act.
            fooableComposite!.FooWithArgs('1').Now();   
            
            // Assert.
            List<char> expectedAnswer = new ();
            for (int i = 0; i < 10; i++)
            {
                expectedAnswer.Add('1');
            }
            Assert.AreEqual(expectedAnswer, answers);
        }
    }
}