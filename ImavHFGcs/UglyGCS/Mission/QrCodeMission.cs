using HighFlyers.Algorithms;

namespace UglyGCS.Mission
{
    internal class QrCodeMission : Mission
    {
        private QRCode qrcodeAlgorithm = new QRCode();
        public QrCodeMission(string title)
            : base(title)
        {
            MissionControl = new Controls.QrCodeControl();
        }

        public override unsafe bool RunMethod(byte* imgBuffer, int width, int height)
        {
            string returnedText = qrcodeAlgorithm.RecognizeQrCode(imgBuffer, width, height);
            bool qrFound = returnedText != null;
            (MissionControl as Controls.QrCodeControl).SetReturned(qrFound, returnedText);

            return qrFound;
        }
    }
}
