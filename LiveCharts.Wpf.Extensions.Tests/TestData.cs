using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCharts.Wpf.Extensions.Tests
{
    internal class TestData<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public TestData(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
