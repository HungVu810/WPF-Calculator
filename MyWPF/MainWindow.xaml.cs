using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MyWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    // Turn this class into a struct?
    public class ObservableObject<T> : INotifyPropertyChanged
    {
        private T _value;
        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }

        public T? Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!_value.Equals(value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
        // Need to be public to implement the derived interface member
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public partial class MainWindow : Window
    {
        private enum Op { Add, Subtract, Multiply, Divide, None };
        private Op CurrentOp = Op.None;
        private float MainResult = 0.0f;
        private float ChildResult = 0.0f;

        // XAML bindings, need to be public so they can be resolved at runtime?
        ObservableObject<string> MainViewer = new ObservableObject<string>();
        ObservableObject<string> ChildViewer = new ObservableObject<string>();
        // TODO: public uint NumPressed { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Init Value
            MainViewer.Value = $"{MainResult}";
            ChildViewer.Value = $"{ChildResult}";

            // Data Context
            MainViewerBlock.DataContext = MainViewer;
            ChildViewerBlock.DataContext = ChildViewer;
        }

        private float Eval(Op op = Op.None, float opLeft = 0.0f, float opRight = 0.0f)
        {
            switch (op)
            {
                case Op.Add: return opLeft + opRight;
                case Op.Subtract: return opLeft - opRight;
                case Op.Divide: return opLeft / opRight;
                case Op.Multiply: return opLeft * opRight;
                default: return 0.0f;
            }
        }

        private float IncrementDecimalPlace(float target = 0.0f, float value = 0.0f)
        {
            if (target == 0.0f) return value;
            else return target * 10 + value;
        }

        private string CreateViewer(float value = 0.0f, Op op = Op.None)
        {
            string viewer = "";
            int quotient = (int)(value / 1000);
            int remainder = (int)(value % 1000);
            if (quotient == 0)
            { // TODO: decimal
                viewer = $"{value}";
            }
            else
            {
                // 12345678 -> 12,345,678
                // 12345678/1000 -> 12345, 678
                // 12345/1000 -> 12, 345
                // 12/1000 -> 0, ...
                while(quotient > 0)
                {
                    viewer = "," + remainder + viewer;
                    remainder = (int)(quotient % 1000);
                    quotient = (int)(quotient / 1000);
                }
                viewer = quotient + viewer;
            }
            switch (op)
            {
                case Op.Add: viewer += " +"; break;
                case Op.Subtract: viewer += " -"; break;
                case Op.Multiply: viewer += " *"; break;
                case Op.Divide: viewer += " /"; break;
                default: break;
            }
            return viewer;
        }

        private void Nine_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 9.0f);
            MainViewer.Value = CreateViewer(MainResult);
        }

        private void Eight_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 8.0f);
        }

        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 7.0f);

        }
        private void Six_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 6.0f);

        }

        private void Five_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 5.0f);

        }
        private void Four_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 4.0f);

        }

        private void Three_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 3.0f);

        }
        private void Two_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 2.0f);

        }

        private void One_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult, 1.0f);

        }

        private void Zero_Click(object sender, RoutedEventArgs e)
        {
            MainResult = IncrementDecimalPlace(MainResult);
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            ChildViewer.Value = MainResult;

        }

        private void Subtract_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}







