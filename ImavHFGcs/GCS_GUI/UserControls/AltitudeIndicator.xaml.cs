using System.Globalization;

namespace HighFlyers.UserControls
{
    /// <summary>
    /// Interaction logic for AltitudeIndicator.xaml
    /// </summary>
    public partial class AltitudeIndicator
    {
        private float altitude;
        private float dAltitude;

        public AltitudeIndicator()
        {
            InitializeComponent();
            altitude = 0;
            dAltitude = 0;
        }

        public float Value
        {
            set
            {
                if (value < 0)
                    altitude = 0;
                else
                    altitude = value;

                valueTextBlock.Text = altitude.ToString("0.00") + "m";
            }

            get
            {
                return altitude;
            }
        }

        public float DValue
        {
            set
            {
                if (value < 0)
                    dAltitude = 0;
                else
                    dAltitude = value;

                DValueTextBlock.Text = dAltitude.ToString("0.00") + "m";
            }

            get
            {
                return dAltitude;
            }
        }


    }
}
