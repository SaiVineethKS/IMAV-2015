using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;

namespace HighFlyers.UserControls
{
    /// <summary>
    /// Interaction logic for BatteryVoltage.xaml
    /// </summary>
    public partial class BatteryVoltage
    {

        private float voltage;
        private float maxvalue, minvalue;

        private readonly Color red1 = Color.FromRgb(147, 0, 0);
        private readonly Color red2 = Color.FromRgb(255, 78, 78);
        private readonly Color green1 = Color.FromRgb(50, 179, 0);
        private readonly Color green2 = Color.FromRgb(169, 255, 121);



        public BatteryVoltage()
        {
            InitializeComponent();
            maxvalue = 12.6F;
            minvalue = 10.5F;
            voltage = minvalue;
        }


        public float Value
        {
            set
            {
                if (value > maxvalue)
                    voltage = maxvalue;
                else
                    if (value < minvalue)
                        voltage = minvalue;
                    else
                        voltage = value;

                valueTextBlock.Text = voltage.ToString("0.00") + "V";
                levelDockPanel.Height = (voltage-minvalue) * 80 / (maxvalue - minvalue);

                if (levelDockPanel.Height >= 20)
                    levelDockPanel.Background = new LinearGradientBrush(green1, green2, new Point(0, 0.5), new Point(1, 0.5));
                else
                    levelDockPanel.Background = new LinearGradientBrush(red1, red2, new Point(0, 0.5), new Point(1, 0.5));

            }

            get
            {
                return voltage;
            }
        }

        public float MaxValue
        {
            set
            {
                maxvalue = value;
                maxValueLabel.Content = maxvalue;
            }

            get
            {
                return maxvalue;
            }
        }


        public float MinValue
        {
            set
            {
                minvalue = value;
                minvalueLabel.Content = minvalue;
            }

            get
            {
                return minvalue;
            }
        }
    }
}
