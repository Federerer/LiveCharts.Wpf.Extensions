using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Wpf.Extensions.Utils;

namespace LiveCharts.Wpf.Extensions.Behaviors
{
    public class ChartExtensionsBehavior : Behavior<Chart>
    {
        public static readonly DependencyProperty SeriesSourceProperty =
            DependencyProperty.Register("SeriesSource", typeof(IEnumerable), typeof(ChartExtensionsBehavior),
                new PropertyMetadata(null,
                    (o, args) =>
                        (o as ChartExtensionsBehavior)?.OnSeriesSourceChanged((IEnumerable) args.OldValue,
                            (IEnumerable) args.NewValue)));

        public static readonly DependencyProperty SeriesTemplateProperty =
            DependencyProperty.Register("SeriesTemplate", typeof(DataTemplate), typeof(ChartExtensionsBehavior),
                new PropertyMetadata(null,
                    (o, args) => (o as ChartExtensionsBehavior)?.OnSeriesTemplateChanged((DataTemplate) args.NewValue)));

        public IEnumerable SeriesSource
        {
            get { return (IEnumerable) GetValue(SeriesSourceProperty); }
            set { SetValue(SeriesSourceProperty, value); }
        }


        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate) GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }


        private void OnSeriesSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            var oldCollection = oldValue as INotifyCollectionChanged;
            if (oldCollection != null) oldCollection.CollectionChanged -= OnSeriesChanged;

            var newCollection = newValue as INotifyCollectionChanged;
            if (newCollection != null) newCollection.CollectionChanged += OnSeriesChanged;

            AddSeries(newValue);
        }

        private void OnSeriesTemplateChanged(DataTemplate value)
        {
            //TODO?
        }

        private void OnSeriesChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddSeries(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveSeries(notifyCollectionChangedEventArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Reset:
                    AssociatedObject.Series.Clear();
                    AddSeries(SeriesSource);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RemoveSeries(IEnumerable items)
        {
            foreach (var item in items)
            {
                AssociatedObject.Series.RemoveAll(i => (i as FrameworkElement)?.DataContext == item);
            }
        }

        private void AddSeries(IEnumerable items)
        {
            if (SeriesTemplate == null)
            {
                return;
            }

            foreach (var item in items)
            {
                var series = LoadDataTemplate<Series>(SeriesTemplate, item);
                AssociatedObject.Series.Add(series);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Series = new SeriesCollection();
        }

        private T LoadDataTemplate<T>(DataTemplate template, object dataContext) where T : FrameworkElement
        {
            var element = template.LoadContent();
            var view = element as T;
            view.DataContext = dataContext;

            var enumerator = element.GetLocalValueEnumerator();
            while (enumerator.MoveNext())
            {
                var bind = enumerator.Current;

                if (bind.Value is BindingExpression)
                {
                    view.SetBinding(bind.Property, ((BindingExpression) bind.Value).ParentBinding);
                }
            }

            return view;
        }
    }
}