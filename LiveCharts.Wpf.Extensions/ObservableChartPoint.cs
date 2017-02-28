using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCharts.Wpf.Extensions
{
    //TODO: INPC implementation
    internal class ObservableChartPoint<T> : IObservableChartPoint
    {
        public event Action PointChanged;

        public T Value { get; set; }
    }
}
