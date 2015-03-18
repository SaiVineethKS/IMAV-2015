using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace HighFlyers.UserControls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown
    {
        private double numValue;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private static readonly TimeSpan StartTimespan = new TimeSpan(0, 0, 0, 1);
        private static readonly TimeSpan FastTickTimespan = new TimeSpan(0, 0, 0, 0, 100);
        private static readonly TimeSpan SecondStageFastTickTimespan = new TimeSpan(0, 0, 0, 0, 50);
        private int fastTimer;

        public NumericUpDown()
        {
            InitializeComponent();
            AddHandler(MouseDownEvent, new MouseButtonEventHandler(Button_MouseDown), true);
            AddHandler(MouseUpEvent, new MouseButtonEventHandler(BtnUp), true);
            txtNum.Text = numValue.ToString(CultureInfo.InvariantCulture);
            Increment = 0.25;
        }

        public double Value
        {
            get { return numValue; }
            set
            {
                numValue = value;
                txtNum.Text = numValue.ToString();
            }
        }

        public double Increment
        {
            get; set;
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            if (timer.Interval == StartTimespan)
                Value += Increment;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            if (timer.Interval == StartTimespan)
                Value -= Increment;
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!double.TryParse(txtNum.Text, out numValue))
                txtNum.Text = numValue.ToString(CultureInfo.InvariantCulture);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            fastTimer = 0;
            timer.Interval = StartTimespan;
            timer.Tick += IncrementTick;
            timer.Start();
        }

        private void BtnUp(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            timer.Tick -= IncrementTick;
            timer.Tick -= DecrementTick;
            fastTimer = 0;
        }

        private void DecrementTick(object sender, EventArgs e)
        {
            Value -= Increment;
            timer.Interval = FastTickTimespan;
            UpdateTimer();
        }

        private void IncrementTick(object o, EventArgs e)
        {
            Value += Increment;
            timer.Interval = FastTickTimespan;
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            fastTimer++;
            if (fastTimer > 10)
                timer.Interval = SecondStageFastTickTimespan;
        }
    }
}
