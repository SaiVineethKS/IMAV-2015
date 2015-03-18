using HighFlyers.Mission.MissionControls;
using HighFlyers.UserControls;

namespace HighFlyers.Mission
{
    internal class QrCodeMission : Mission
    {
        private readonly Algorithms.QRCode qrcodeAlgorithm = new Algorithms.QRCode();
        public QrCodeMission(string title)
            : base(title)
        {
            MissionControl = new QRCodeControl();
        }

        protected override unsafe bool RunMethodInternal(byte* imgBuffer, int width, int height)
        {
            string returnedText = qrcodeAlgorithm.RecognizeQrCode(imgBuffer, width, height);
            bool qrFound = returnedText != null;
            ((QRCodeControl) MissionControl).SetReturned(qrFound, returnedText);

            return qrFound;
        }
    }
}
