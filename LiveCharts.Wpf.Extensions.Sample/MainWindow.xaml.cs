using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LiveCharts.Wpf.Extensions.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer _timer;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Data = new ObservableCollection<ObservableCollection<SampleData>>();

            var rnd = new Random();

            for (int i = 0; i < 3; i++)
            {
                var d = new ObservableCollection<SampleData>();

                for (int j = 0; j < 100; j++)
                {
                    d.Add(new SampleData(DateTime.Now, rnd.NextDouble()));
                    Thread.Sleep(20);
                }

                Data.Add(d);
            }

            _timer = new Timer((s) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Data.First().Add(new SampleData(DateTime.Now, rnd.NextDouble()));
                });
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        public ObservableCollection<ObservableCollection<SampleData>> Data { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();

            for (int i = 0; i < 1; i++)
            {
                var d = new ObservableCollection<SampleData>();

                for (int j = 0; j < 20; j++)
                {
                    d.Add(new SampleData(DateTime.Now, rnd.NextDouble()));
                    Thread.Sleep(10);
                }

                Data.Add(d);
            }
        }
    }

    public class SampleData : INotifyPropertyChanged
    {
        public double Value { get; set; }

        public DateTime TimeStamp { get; set; }

        public SampleData(DateTime timeStamp, double value)
        {
            TimeStamp = timeStamp;
            Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
