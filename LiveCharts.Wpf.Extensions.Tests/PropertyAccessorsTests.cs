using System;
using LiveCharts.Wpf.Extensions.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveCharts.Wpf.Extensions.Tests
{
    [TestClass]
    public class PropertyAccessorsTests
    {
        #region Weakly typed

        [TestMethod]
        public void CreateGetter_ReferenceTypeWithValidPropertyName_ReturnsGetter()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.CreateGetter("Name", data);

            Assert.IsNotNull(getter);
        }

        [TestMethod]
        public void CreateGetter_ValueTypeWithValidPropertyName_ReturnsGetter()
        {
            var data = TimeSpan.FromDays(1);
            var getter = PropertyAccessors.CreateGetter(nameof(TimeSpan.TotalMilliseconds), data);

            Assert.IsNotNull(getter);
        }

        [TestMethod]
        public void CreateGetter_GetterCalled_ReturnsValueType()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.CreateGetter("Value", data);

            Assert.AreEqual(5, getter(data));
        }

        [TestMethod]
        public void CreateGetter_GetterCalled_ReturnsRefferenceType()
        {
            var data = new TestData<object>("Test", null);
            var getter = PropertyAccessors.CreateGetter("Name", data);

            Assert.AreSame(data.Name, getter(data));
        }

        [TestMethod]
        public void CreateGetter_InvalidProperty_ReturnsNull()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.CreateGetter("Invalid", data);

            Assert.IsNull(getter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateGetter_GetterCalledWithInvalidObjectType_ThrowsArgumentException()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.CreateGetter("Value", data);

            getter(new object());
        }

        #endregion
        #region Strongly typed

        [TestMethod]
        public void CreateGetterT_ValidReturnType_ReturnsGetter()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.CreateGetter<int>("Value", data);

            Assert.IsNotNull(getter);
        }

        [TestMethod]
        public void CreateGetterT_InvalidReturnType_ReturnsNull()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.CreateGetter<string>("Value", data);

            Assert.IsNull(getter);
        }

        [TestMethod]
        public void CreateGetterT_GetterCalled_ReturnsValueType()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.CreateGetter<int>("Value", data);

            Assert.AreEqual(5, getter(data));
        }

        [TestMethod]
        public void CreateGetterT_GetterCalled_ReturnsReferenceType()
        {
            var data = new TestData<int>("Test", 5);
            var getter = PropertyAccessors.CreateGetter<string>("Name", data);

            Assert.AreSame(data.Name, getter(data));
        }

        #endregion
    }
}
