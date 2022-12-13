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
        // Need to be public to implement the derived interface member
        public event PropertyChangedEventHandler? PropertyChanged;
        public Action? OnPropertyChangedAction { get; set; }

        private T? _value;
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
                    OnPropertyChangedAction?.Invoke();
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
        private static string[] OpToStr = { " + ", " - ", " * ", " / ", " = ", "" };
        private enum Op { Add, Subtract, Multiply, Divide, Equal, None };
        private Op CurrentOp = Op.None;
        private ObservableObject<double>[] Expr = new ObservableObject<double>[3];  // LeftOp, RightOp, Result
        private ObservableObject<int> CurrentExprElement = new ObservableObject<int>();
        private bool IsCurrentExprElementModifiable = true;

        // XAML bindings, need to be public so they can be resolved at runtime?
        ObservableObject<string> CurrentExprElementDisplay = new ObservableObject<string>();
        ObservableObject<string> CurrentOpDisplay = new ObservableObject<string>();

        // TODO: public uint NumPressed { get; set; }
        // TODO: automatically call setdisplays() if the current op is modified, or one of the entry in expr is modified
        // TODO: automatically set iscurrentmodifiable when currentexprelement changed and set it to false when the expr[currentexprelement] is modifed after copying from the preivuos index
        public MainWindow()
        {
            InitializeComponent();

            // Init Value
            for (int i = 0; i < 3; i++)
            {
                Expr[i] = new ObservableObject<double>();
                Expr[i].Value = 0;
                Expr[i].OnPropertyChangedAction += SetDisplays;
            }
            CurrentExprElement.Value = 0;
            CurrentExprElement.OnPropertyChangedAction += () => { IsCurrentExprElementModifiable = true; SetDisplays(); };
            CurrentExprElementDisplay.Value = "0";
            CurrentOpDisplay.Value = "";

            // Data Context
            MainDisplay.DataContext = CurrentExprElementDisplay;
            ChildDisplay.DataContext = CurrentOpDisplay;
        }

        // Generic functions
        private void AppendCurrentExprElement(uint Value = 0)
        {
            if (Value > 9) return;
            if (IsCurrentExprElementModifiable)
            {
                Expr[CurrentExprElement.Value].Value = Value;
                IsCurrentExprElementModifiable = false;
                return;
            }
            try
            {
                checked
                {
                    if (CurrentExprElementDisplay.Value == null) return;
                    int? DecPointIndex = CurrentExprElementDisplay.Value.IndexOf('.');
                    if (DecPointIndex != null && DecPointIndex != -1) // Floating point
                    {
                        string Temp = CurrentExprElementDisplay.Value;
                        for (char i = '1'; i < '9'; i++)
                        {
                            Temp = Temp.Replace(i, '0');
                        }
                        Temp += "1"; // turn the elem into 0--0.00--01
                        double NormDecimal = double.Parse(Temp); // 0.00--01
                        Expr[CurrentExprElement.Value].Value += NormDecimal * Value;
                    }
                    else // Normal integer
                    {
                        double Temp = Expr[CurrentExprElement.Value].Value * 10 + Value;
                        if (Temp < MAX_DISPLAYABLE)
                        {
                            Expr[CurrentExprElement.Value].Value = Temp;
                        }
                    }
                }
            }
            catch(Exception Ex)
            {
                Expr[CurrentExprElement.Value].Value = 0;
                MessageBox.Show(Ex.Message);
            }
        }

        private string ExprElementToStr(int index)
        {
            string ElemStr = Expr[index].Value.ToString();
            int SeperatorIndex = ElemStr.IndexOf(".");
            int NumDecimals = 0;
            // In the case "." is pressed, this "if" should fail because the
            // seperator is only shown on the display but not in the
            // Expr[index]
            if (SeperatorIndex != -1)
            {
                NumDecimals = ElemStr.Substring(SeperatorIndex + 1).Length;
            }
            // if (DecPointIndex + 1 >= CurrentExprElementDisplay.Value.Length) return;
            return Expr[index].Value.ToString(Expr[index].Value < MAX_DISPLAYABLE ? "N" + $"{NumDecimals}" : "E");
        }

        private void SetDisplays()
        {
            CurrentExprElementDisplay.Value = $"{ExprElementToStr(CurrentExprElement.Value)}";
            string OpStr = OpToStr[(int)CurrentOp];
            switch (CurrentExprElement.Value)
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
        }

        private void Eight_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(8);
        }

        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(7);
        }

        private void Six_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(6);
        }

        private void Five_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(5);
        }

        private void Four_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(4);
        }

        private void Three_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(3);
        }

        private void Two_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(2);
        }

        private void One_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(1);
        }

        private void Zero_Click(object sender, RoutedEventArgs e)
        {
            AppendCurrentExprElement(0);
        }

        private void UpdateExprPostCurrentOpUpdated()
        {
            switch (CurrentExprElement.Value)
            {
                case 0: Expr[1].Value = Expr[0].Value; break;
                case 1: Expr[0].Value = Expr[1].Value; break;
                case 2:
                    {
                        Expr[0].Value = Expr[2].Value;
                        Expr[1].Value = Expr[2].Value;
                        Expr[2].Value = 0;
                    } break;
                default: break;
            }
            CurrentExprElement.Value = 1;
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentOp != Op.None)
            {
                Eval();
            }
            CurrentOp = Op.Multiply;
            UpdateExprPostCurrentOpUpdated();
        }

        private void Subtract_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentOp != Op.None)
            {
                Eval();
            }
            CurrentOp = Op.Subtract;
            UpdateExprPostCurrentOpUpdated();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentOp != Op.None)
            {
                Eval();
            }
            CurrentOp = Op.Add;
            UpdateExprPostCurrentOpUpdated();
        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentOp != Op.None)
            {
                Eval();
            }
            CurrentOp = Op.Divide;
            UpdateExprPostCurrentOpUpdated();
        }
        private void Eval()
        {
            if (CurrentExprElement.Value == 2)
            {
                Expr[0].Value = Expr[2].Value;
            }
            try
            {
                checked
                {
                    switch (CurrentOp)
                    {
                        case Op.Add: Expr[2].Value = Expr[0].Value + Expr[1].Value; break;
                        case Op.Subtract: Expr[2].Value = Expr[0].Value - Expr[1].Value; break;
                        case Op.Divide: Expr[2].Value = Expr[0].Value / Expr[1].Value; break;
                        case Op.Multiply: Expr[2].Value = Expr[0].Value * Expr[1].Value; break;
                        default: Expr[2].Value = 0; return;
                    }
                    CurrentExprElement.Value = 2;
                }
            }
            catch (Exception Ex)
            {
                Expr[2].Value = 0;
                MessageBox.Show(Ex.Message);
            }
        }

        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentOp != Op.Equal && CurrentOp != Op.None)
            {
                Eval();
            }
            else
            {
                CurrentOp = Op.Equal;
                UpdateExprPostCurrentOpUpdated();
            }
        }

        private void ClearExpr()
        {
            CurrentOp = Op.None;
            Expr[0].Value = 0;
            Expr[1].Value = 0;
            Expr[2].Value = 0;
            CurrentExprElement.Value = 0;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearExpr();
        }

        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentExprElement.Value == 2)
            {
                ClearExpr();
            }
            else
            {
                Expr[CurrentExprElement.Value].Value = 0;
            }
        }

        private void Negate_Click(object sender, RoutedEventArgs e)
        {
            Expr[CurrentExprElement.Value].Value *= -1;
        }

        private void Seperator_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentExprElementDisplay.Value.IndexOf('.') == -1)
            {
                CurrentExprElementDisplay.Value += ".";
                IsCurrentExprElementModifiable = false;
            }
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






 