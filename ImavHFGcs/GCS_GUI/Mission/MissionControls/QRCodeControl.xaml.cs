namespace HighFlyers.Mission.MissionControls
{
    /// <summary>
    /// Interaction logic for QRCodeControl.xaml
    /// </summary>
    public partial class QRCodeControl
    {
        public QRCodeControl()
        {
            InitializeComponent();
        }

        public void SetReturned(bool ok, string text)
        {
            returnedValueLabel.Content = ok ? text : "none";
        }
    }
}
