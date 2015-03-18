using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using UglyGCS.Mission;
using System.Diagnostics;

namespace UglyGCS
{
    public partial class CameraControl : UserControl
    {
        Capture cap; //main capture object
        byte[] imgBuffer;  //raw bytes
        private VideoWriter writer;
        string fileName = String.Empty;
        private PictureBox picBox;
        private Mission.Mission currentMission;
        private Stopwatch missionSW = new Stopwatch();

        public event MissionEventHandler AlgorithmFinished;

        public TimeSpan CurrentProcessingTime
        { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        public CameraControl()
        {
            InitializeComponent();
        }

        public void SetPictureBox(PictureBox pb)
        {
            picBox = pb;
        }

        /// <summary>
        /// is camera started?
        /// </summary>
        public bool IsStarted
        { private set; get; }

        /// <summary>
        /// is stream recorded?
        /// </summary>
        public bool IsRecording
        { set; get; }

        /// <summary>
        /// returns image buffer
        /// </summary>
        public byte[] ImageBuffer
        {
            get { return imgBuffer; }
        }


        /// <summary>
        /// convert bitmap to byte array
        /// </summary>
        /// <param name="imageIn">image to convert</param>
        /// <returns>byte array, represents image</returns>
        public byte[] ImageToByteArray(Bitmap imageIn)
        {
            var ms = new System.IO.MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }


        /// <summary>
        /// event handler for grab image from camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrabFromCamera(object sender, EventArgs e)
        {
            Image<Bgr, byte> frame = cap.RetrieveBgrFrame();
            var tt = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_PLAIN, 1, 1);
            frame.Draw(DateTime.Now.ToString(CultureInfo.InvariantCulture), ref tt, new Point(0, cap.Height - 20),
                       new Bgr(Color.White));

            if (IsRecording && writer != null)
                frame.Draw("Rozmiar pliku: " + new System.IO.FileInfo(fileName).Length/1000000, ref tt, new Point(3, 12),
                           new Bgr(Color.White));

            Bitmap bmp = frame.ToBitmap();
            mainPictureBox.Image = bmp;
            imgBuffer = ImageToByteArray(bmp);

            if (currentMission != null)
            {
                missionSW.Reset();
                missionSW.Start();
                MakeCurrentMission(frame.Width, frame.Height);
                missionSW.Stop();
                CurrentProcessingTime = missionSW.Elapsed;
            }

            if (IsRecording && writer != null)
                writer.WriteFrame(frame);
        }

        private void MakeCurrentMission(int width, int height)
        {
            unsafe
            {
                fixed (byte* arr = imgBuffer)
                {
                    bool methodFinished = currentMission.RunMethod(arr, width, height);

                    if (picBox != null)
                        picBox.Image = Algorithms.ByteArrayToImage(imgBuffer);

                    if (methodFinished)
                    {
                        OnAlgorithmFinished(new MissionEventArgs(currentMission));
                        SetMission(null);
                        CurrentProcessingTime = TimeSpan.Zero;
                    }

                }
            }
        }


        public void StartRecording(string filename)
        {
            try
            {
                fileName = @String.Format("D:\\filename-{0:dd-M-yyyy_hh-mm-ss}", DateTime.Now) + ".avi";
                writer = new VideoWriter(fileName, -1, 60, cap.Width, cap.Height, true);
                IsRecording = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot record stream: " + ex.Message);
            }
        }


        public void StopRecording()
        {
            writer = null;
            IsRecording = false;
        }

        public void Start(int camNumber)
        {
            try
            {
                if (IsStarted)
                    return;

                IsStarted = true;

                if (cap == null)
                {
                    cap = new Capture(camNumber);
                    Application.Idle += GrabFromCamera;
                    CameraControl_SizeChanged(null, null);
                }
                else
                {
                    Application.Idle += GrabFromCamera;
                    cap.Grab();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Cannot start camera: " + ex.Message);
            }
        }

        /// <summary>
        /// stop current acquisition
        /// </summary>
        public void Stop()
        {
            try
            {
                StopRecording();
                IsStarted = false;
                cap.Stop();
                Application.Idle -= GrabFromCamera;

            }
            catch (Exception ex)
            {
                throw new Exception("Cannot stop camera: " + ex.Message);
            }
        }


        private void CameraControl_SizeChanged(object sender, EventArgs e)
        {
            if (cap == null)
                return;

            if (cap.Width / (float)Width > cap.Height / (float)Height)
            {
                mainPictureBox.Left = 0;
                mainPictureBox.Width = Width;
                mainPictureBox.Height = (int)((float)cap.Height / cap.Width * Width);
                mainPictureBox.Top = (Height - mainPictureBox.Height) / 2;
            }
            else
            {
                mainPictureBox.Top = 0;
                mainPictureBox.Height = Height;
                mainPictureBox.Width = cap.Width / cap.Height * Height;
                mainPictureBox.Left = (Width - mainPictureBox.Width) / 2;
            }
        }

        public void SetMission(Mission.Mission mission)
        {
            currentMission = mission;
        }

        protected virtual void OnAlgorithmFinished(MissionEventArgs e)
        {
            MissionEventHandler handler = AlgorithmFinished;
            if (handler != null) handler(this, e);
        }
    }
}
