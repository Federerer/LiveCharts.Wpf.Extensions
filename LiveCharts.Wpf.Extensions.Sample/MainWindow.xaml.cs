using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Data = new ObservableCollection<ObservableCollection<double>>();

            var rnd = new Random();

            for (int i = 0; i < 3; i++)
            {
                var d = new ObservableCollection<double>();

                for (int j = 0; j < 100; j++)
                {
                    d.Add(rnd.NextDouble());
                }

                Data.Add(d);
            }
        }

        public ObservableCollection<ObservableCollection<double>> Data { get; set; }
    }
}
