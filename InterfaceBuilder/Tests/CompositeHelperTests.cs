using System;
using System.Collections.Generic;
using System.Reflection;
using InterfaceBuilder.CompositeGeneration;
using InterfaceBuilder.Mocks;
using NUnit.Framework;
using Moq;
using Mock = Moq.Mock;

namespace InterfaceBuilder.Tests
{
    public class CompositeHelperTests
    {
        [Test]
        public void PerformAction_WhenInvokePerform()
        {
            // Arrange.
            CompositeHelper.Reset();
            string answer = "not done";
            IPool<IFooable> pool = Mock.Of<IPool<IFooable>>(
                (pool1 => pool1.Items == new List<IFooable>())
                );

            // Act.
            CompositeHelper.Group(_ =>
            {
                answer = "done";
            }, pool);
            CompositeHelper.Perform();
            
            // Assert.
            Assert.AreEqual("done", answer);
        }

        [Test]
        public void PerformOnlyLastAction_WhenPerformLast()
        {
            // Arrange.
            CompositeHelper.Reset();
            string answer = "not done";
            IPool<IFooable> pool = Mock.Of<IPool<IFooable>>(
                (pool1 => pool1.Items == new List<IFooable>())
            );

            // Act.
            CompositeHelper.Group(_ => throw new Exception("Invoked wrong action"), pool);
            CompositeHelper.Group(_ =>
            {
                answer = "done";
            }, pool);
            CompositeHelper.PerformLast();
            
            // Assert.
            Assert.AreEqual("done", answer);
        }
        
        [Test]
        public void DeleteLastActionFromStash_WhenPerformLast()
        {
            // Arrange.
            CompositeHelper.Reset();
            string answer = "not done";
            IPool<IFooable> pool = Mock.Of<IPool<IFooable>>(
                (pool1 => pool1.Items == new List<IFooable>())
            );

            // Act.
            CompositeHelper.Group(_ => throw new Exception("Invoked wrong action"), pool);
            CompositeHelper.Group(_ =>
            {
                answer = "done";
            }, pool);
            CompositeHelper.PerformLast();
            
            // Assert.
            Assert.AreEqual("done", answer);
        }
    }
}