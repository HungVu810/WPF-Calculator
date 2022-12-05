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
        // Need to be public to implement the derived interface member
        public event PropertyChangedEventHandler? PropertyChanged;

        public T? Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == null || !_value.Equals(value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }

    }

    public partial class MainWindow : Window
    {
        private static double MAX_DISPLAYABLE = 1e15;
        private enum Op { Add, Subtract, Multiply, Divide, Equal, None };
        private Op CurrentOp = Op.None;
        private double[] Expr = new double[3] { 0, 0, 0 };  // LeftOp, RightOp, Result
        private int CurrentExprElement = 0;
        private bool IsCurrentExprElementModifiable = true;

        // XAML bindings, need to be public so they can be resolved at runtime?
        ObservableObject<string> CurrentExprElementDisplay = new ObservableObject<string>();
        ObservableObject<string> CurrentOpDisplay = new ObservableObject<string>();

        // TODO: public uint NumPressed { get; set; }
        // TODO: automatically call setdisplays() if the current op is modified, or one of the entry in expr is modified
        public MainWindow()
        {
            InitializeComponent();

            // Init Value
            CurrentExprElementDisplay.Value = "0";
            CurrentOpDisplay.Value = "";

            // Data Context
            MainDisplay.DataContext = CurrentExprElementDisplay;
            ChildDisplay.DataContext = CurrentOpDisplay;
        }

        private void Eval()
        {
            if (CurrentExprElement == 2)
            {
                Expr[0] = Expr[2];
            }
            try
            {
                checked
                {
                    switch (CurrentOp)
                    {
                        case Op.Add: Expr[2] = Expr[0] + Expr[1]; break;
                        case Op.Subtract: Expr[2] = Expr[0] - Expr[1]; break;
                        case Op.Divide: Expr[2] = Expr[0] / Expr[1]; break;
                        case Op.Multiply: Expr[2] = Expr[0] * Expr[1]; break;
                        default: Expr[2] = 0; return;
                    }
                    CurrentExprElement = 2;
                }
            }
            catch (OverflowException Except)
            {
                Expr[2] = 0;
            }
        }

        // Generic functions
        private void AppendCurrentExprElement(double Value = 0)
        {
            if (IsCurrentExprElementModifiable)
            {
                IsCurrentExprElementModifiable = false;
                Expr[CurrentExprElement] = Value;
                return;
            }
            try
            {
                checked
                {
                    double Temp = Expr[CurrentExprElement] * 10 + Value;
                    Expr[CurrentExprElement] = Temp < MAX_DISPLAYABLE ? Temp : Expr[CurrentExprElement];
                }
            }
            catch(OverflowException e)
            {
                Expr[CurrentExprElement] = 0;
            }
        }

        private string CurrentOpToStr()
        {
            switch (CurrentOp)
            {
                case Op.Add: return " + ";
                case Op.Subtract: return " - ";
                case Op.Divide: return " / ";
                case Op.Multiply: return " * ";
                case Op.Equal: return " = ";
                default: return "";
            }
        }

        private string ExprElementToStr(int index)
        {
            return Expr[index].ToString(Expr[index] < MAX_DISPLAYABLE ? "N0" : "E");
        }

        private void SetDisplays()
        {
            CurrentExprElementDisplay.Value = $"{ExprElementToStr(CurrentExprElement)}";
            string OpStr = CurrentOpToStr();
            switch (CurrentExprElement)
            {
                case 0: CurrentOpDisplay.Value = ""; break;
                case 1: CurrentOpDisplay.Value = $"{ExprElementToStr(0)}" + OpStr; break;
                case 2: CurrentOpDisplay.Value = $"{ExprElementToStr(0)}" + OpStr + $"{ExprElementToStr(1)} = "; break;
                default: break;
            }
        }

        // Events
        private void Nine_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(9);
            SetDisplays();
        }

        private void Eight_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(8);
            SetDisplays();
        }

        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(7);
            SetDisplays();
        }

        private void Six_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(6);
            SetDisplays();
        }

        private void Five_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(5);
            SetDisplays();
        }

        private void Four_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(4);
            SetDisplays();
        }

        private void Three_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(3);
            SetDisplays();
        }

        private void Two_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(2);
            SetDisplays();
        }

        private void One_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(1);
            SetDisplays();
        }

        private void Zero_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(0);
            SetDisplays();
        }

        private void UpdateExprPostCurrentOpUpdated()
        {
            switch (CurrentExprElement)
            {
                case 0: Expr[1] = Expr[0]; break;
                case 1: Expr[0] = Expr[1]; break;
                case 2:
                    {
                        Expr[0] = Expr[2];
                        Expr[1] = Expr[2];
                        Expr[2] = 0;
                    } break;
                default: break;
            }
            CurrentExprElement = 1;
            IsCurrentExprElementModifiable = true;
        }

        private void OnCurrentOpModified()
        {
            UpdateExprPostCurrentOpUpdated();
            SetDisplays();
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            CurrentOp = Op.Multiply;
            OnCurrentOpModified();
        }

        private void Subtract_Click(object sender, RoutedEventArgs e)
        {
            CurrentOp = Op.Subtract;
            OnCurrentOpModified();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CurrentOp = Op.Add;
            OnCurrentOpModified();
        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {
            CurrentOp = Op.Divide;
            OnCurrentOpModified();
        }

        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentOp != Op.Equal && CurrentOp != Op.None)
            {
                Eval();
                SetDisplays();
            }
            else
            {
                CurrentOp = Op.Equal;
                OnCurrentOpModified();
            }
        }

        private void ClearExpr()
        {
            Expr[0] = 0;
            Expr[1] = 0;
            Expr[2] = 0;
            CurrentExprElement = 0;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearExpr();
            SetDisplays();
        }

        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentExprElement == 2)
            {
                ClearExpr();
            }
            else
            {
                Expr[CurrentExprElement] = 0;
            }
            SetDisplays();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //delegate ProcessNumber Delegates[] = { One_Click, Two_Click, Three_Click, Four_Click, ... };
            //if (e.Key >= Key.D0 && e.Key <= Key.D9)
            //{
            //    Delegates[e.Key - Key.D0].Invoke(this, e);
            //}
        }
    }
}






 