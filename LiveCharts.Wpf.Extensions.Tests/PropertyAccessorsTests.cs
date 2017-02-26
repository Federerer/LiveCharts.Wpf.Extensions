using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveCharts.Wpf.Extensions.Tests
{
    [TestClass]
    public class PropertyAccessorsTests
    {
        [TestMethod]
        public void CreateGetter_ValidProperty_ReturnsGetter()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.PropertyAccessors.CreateGetter<TestData<int>>("Name");

            Assert.IsNotNull(getter);
        }

        [TestMethod]
        public void CreateGetter_ValidProperty_ReturnsValueType()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.PropertyAccessors.CreateGetter<TestData<int>>("Value");

            Assert.AreEqual(5, getter(data));
        }

        [TestMethod]
        public void CreateGetter_ValidProperty_ReturnsRefferenceType()
        {
            var refference = new object();
            var data = new TestData<object>("Test", refference);
            var getter = PropertyAccessors.PropertyAccessors.CreateGetter<TestData<object>>("Value");

            Assert.AreSame(refference, getter(data));
        }

        [TestMethod]
        public void CreateGetter_InvalidProperty_ReturnsNull()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.PropertyAccessors.CreateGetter<TestData<int>>("Invalid");

            Assert.IsNull(getter);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void CreateGetter_InvalidObject_ThrowsException()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.PropertyAccessors.CreateGetter<TestData<int>>("Value");

            getter(new object());
        }
    }
}
