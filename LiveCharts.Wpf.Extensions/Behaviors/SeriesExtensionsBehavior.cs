using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;
using LiveCharts.Configurations;
using LiveCharts.Wpf.Extensions.Utils;

namespace LiveCharts.Wpf.Extensions.Behaviors
{
    public class SeriesExtensionsBehavior : Behavior<Series>
    {
        public static readonly DependencyProperty XMemberProperty =
            DependencyProperty.Register("XMember", typeof(string), typeof(SeriesExtensionsBehavior),
                new PropertyMetadata(null));

        public static readonly DependencyProperty YMemberProperty =
            DependencyProperty.Register("YMember", typeof(string), typeof(SeriesExtensionsBehavior),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ValuesSourceProperty =
            DependencyProperty.Register("ValuesSource", typeof(IEnumerable), typeof(SeriesExtensionsBehavior),
                new PropertyMetadata(null,
                    (o, args) =>
                        (o as SeriesExtensionsBehavior)?.OnValuesSourceChanged((IEnumerable) args.OldValue,
                            (IEnumerable) args.NewValue)));

        private Type _dataType;
        private Type _pointType;

        private Func<object, object> _xGetter;

        private Func<object, object> _yGetter;

        public string XMember
        {
            get { return (string) GetValue(XMemberProperty); }
            set { SetValue(XMemberProperty, value); }
        }

        public string YMember
        {
            get { return (string) GetValue(YMemberProperty); }
            set { SetValue(YMemberProperty, value); }
        }

        public IEnumerable ValuesSource
        {
            get { return (IEnumerable) GetValue(ValuesSourceProperty); }
            set { SetValue(ValuesSourceProperty, value); }
        }

        private void OnValuesSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            var oldCollection = oldValue as INotifyCollectionChanged;
            if (oldCollection != null) oldCollection.CollectionChanged -= OnValuesChanged;

            var newCollection = newValue as INotifyCollectionChanged;
            if (newCollection != null) newCollection.CollectionChanged += OnValuesChanged;

            _dataType = newValue.GetType().GetGenericArguments()[0];
            _pointType = typeof(ObservableChartPoint<>).MakeGenericType(_dataType);

            CreateValuesCollection();
            AddValues(newValue);
        }

        private void CreateValuesCollection()
        {
            var type = typeof(ChartValues<>);
            var chartValuesType = type.MakeGenericType(_pointType);
            AssociatedObject.Values = (IChartValues)Activator.CreateInstance(chartValuesType);

            var m =
                typeof(SeriesExtensionsBehavior).GetMethod(nameof(CreateMapper),
                    BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(_pointType, _dataType);
            m.Invoke(this, null);
        }

        private void OnValuesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddValues(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveValues(e.OldStartingIndex, e.OldItems.Count);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddValues(IEnumerable items)
        {
            //TODO: OnValuesSourceChanged
            var propInfo = _pointType.GetProperty("Value");

            foreach (var item in items)
            {
                var point = Activator.CreateInstance(_pointType);
                propInfo.SetValue(point, item, null);
                AssociatedObject.Values.Add(point);
            }
        }

        private void RemoveValues(int index, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var point = AssociatedObject.Values[index];
                AssociatedObject.Values.Remove(point);
                ((IDisposable) point)?.Dispose();
            }
        }

        private void CreateMapper<T, T2>() where T : ObservableChartPoint<T2>
        {
            var mapper = new CartesianMapper<T>();

            mapper.X(
                (val, index) =>
                {
                    if (XMember != null)
                    {
                        if (_xGetter == null)
                        {
                            _xGetter = PropertyAccessors.CreateGetter<object>(XMember, val.Value);

                            if (_xGetter == null)
                            {
                                throw new Exception();
                            }
                        }

                        var res = _xGetter(val.Value);

                        if (res is DateTime)
                        {
                            return ((DateTime)res).ToUniversalTime()
                            .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                            .TotalMilliseconds;
                        }

                        return Convert.ToDouble(res);
                    }
                    return index;
                });

            mapper.Y(
                (val, index) =>
                {
                    if (YMember != null)
                    {
                        if (_yGetter == null)
                        {
                            _yGetter = PropertyAccessors.CreateGetter<object>(YMember, val.Value);

                            if (_yGetter == null)
                            {
                                throw new Exception();
                            }
                        }
                        return Convert.ToDouble(_yGetter(val.Value));
                    }
                    return Convert.ToDouble(val.Value);
                });

            AssociatedObject.Configuration = mapper;
        }
    }
}