using System.Windows.Forms;

namespace UglyGCS.Mission.Controls
{
    public partial class QrCodeControl : UserControl
    {
        public QrCodeControl()
        {
            InitializeComponent();
        }

        public void SetReturned(bool ok, string text)
        {
            returnedValueLabel.Text = ok ? text : "none";
        }
    }
}
