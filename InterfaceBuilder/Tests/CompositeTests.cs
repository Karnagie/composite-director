using System;
using System.Collections.Generic;
using InterfaceBuilder.CompositeGeneration;
using InterfaceBuilder.Mocks;
using Moq;
using NUnit.Framework;

namespace InterfaceBuilder.Tests
{
    public class CompositeTests
    {
        [Test]
        public void RemovingEntityInComposite_ByDisposing()
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
                mocks[i].Setup((fooable => fooable.Dispose()))
                    .Raises(fooable => fooable.Disposed += null);
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
            mocks[0].Object.Dispose();
            fooableComposite!.Foo().Now();

            // Assert.
            Assert.AreEqual(19, count);
        }
        
        [Test]
        public void RemovingEntityInComposite_ByRemoving()
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
            fooablePool.Remove(mocks[0].Object);
            fooableComposite!.Foo().Now();

            // Assert.
            Assert.AreEqual(19, count);
        }
        
        [Test]
        public void WhenDisposingComposite_DisposeAllEntities()
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
                mocks[i].Setup((fooable => fooable.Dispose())).Callback((() => count++));
            }

            var creator = new CompositeCreator();
            var fooablePool = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                fooablePool.Add(mock.Object);
            }
            var fooableComposite = fooablePool as IFooable;
            
            // Act.
            fooableComposite!.Dispose();

            // Assert.
            Assert.AreEqual(10, count);
        }
        
        [Test]
        public void WhenDisposingComposite_RemoveAllEntities()
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
                mocks[i].Setup((fooable => fooable.Dispose())).Callback((() => count++));
            }

            var creator = new CompositeCreator();
            var fooablePool = creator.Create<IFooable>();
            foreach (var mock in mocks)
            {
                fooablePool.Add(mock.Object);
            }
            var fooableComposite = fooablePool as IFooable;
            
            // Act.
            fooableComposite!.Dispose();
            fooableComposite!.Dispose();

            // Assert.
            Assert.AreEqual(10, count);
        }
    }
}