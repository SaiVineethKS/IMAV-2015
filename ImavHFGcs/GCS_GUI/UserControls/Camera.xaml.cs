using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using HighFlyers.Mission;
using Point = System.Windows.Point;


namespace HighFlyers.UserControls
{
    /// <summary>
    /// Interaction logic for Camera.xaml
    /// </summary>
    public partial class Camera
    {
        private Capture cap; //main capture object
        private byte[] imgBuffer;  //raw bytes
        private VideoWriter writer;
        private string fileName = String.Empty;
        private DispatcherTimer timer;
        private System.Windows.Forms.PictureBox picBox;
        private readonly Stopwatch missionSw = new Stopwatch();
        private bool isCrosshair;
        public event MissionEventHandler AlgorithmFinished;
        public Mission.Mission CurrentMission { get; private set; }

        public bool IsCrosshair
        {
            get { return isCrosshair; }
            set
            {
                if (value)
                {
                    isCrosshair = true;
                    DrawCrosshair(null, null);
                }
                else
                {
                    isCrosshair = false;
                    canvas2.Children.Clear();
                }
            }
        }

        public TimeSpan CurrentProcessingTime
        { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        public Camera()
        {
            InitializeComponent();
            viewImage.SizeChanged += DrawCrosshair;
        }

        public void SetPictureBox(System.Windows.Forms.PictureBox pb)
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

        public static System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        /// <summary>
        /// convert bitmap to byte array
        /// </summary>
        /// <param name="imageIn">image to convert</param>
        /// <returns>byte array, represents image</returns>
        public byte[] ImageToByteArray(System.Drawing.Bitmap imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        public void DrawPoints(List<Point> driftPoints)
        {
            var stroke = Brushes.Aqua;

            if (driftPoints.Count < 2)
                return;

            var middle = new Point(320, 240);
            Point temp = middle;

            if (driftPoints.Count > 100)
                driftPoints.RemoveAt(0);

            canvas.Children.Clear();

            for (int i = driftPoints.Count - 1; i >= 1; --i)
            {
                var tempLine = new System.Windows.Shapes.Line
                    {
                    StrokeThickness = 4,
                    Stroke = stroke,
                    X1 = temp.X,
                    Y1 = temp.Y,
                    X2 = temp.X + driftPoints[i].X - driftPoints[i-1].X,
                    Y2 = temp.Y + driftPoints[i].Y - driftPoints[i-1].Y,

                };
                canvas.Children.Add(tempLine);
                temp = new Point(tempLine.X2, tempLine.Y2);

            }
            canvas.InvalidateVisual();
        }

        public void ClearPoints()
        {
            try
            {
                canvas.Children.Clear();
                canvas.InvalidateVisual();
            }
            catch (Exception)
            {
                
            }
        }


        /// <summary>
        /// event handler for grab image from camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrabFromCamera(object sender, EventArgs e)
        {
            //unsafe
            {
                Image<Bgr, byte> frame = cap.RetrieveBgrFrame();
                var tt = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_PLAIN, 1, 1);
                frame.Draw(DateTime.Now.ToString("G", CultureInfo.InvariantCulture), ref tt, new System.Drawing.Point(0, cap.Height - 20), new Bgr(System.Drawing.Color.White));

                if (IsRecording && writer != null)
                    frame.Draw("Rozmiar pliku: " + new FileInfo(fileName).Length / 1000000, ref tt, new System.Drawing.Point(3, 12), new Bgr(System.Drawing.Color.White));

                
                viewImage.Source = ToBitmapSource(frame);
                imgBuffer = ImageToByteArray(frame.ToBitmap());

                if (CurrentMission != null)
                {
                    missionSw.Reset();
                    missionSw.Start();
                    MakeCurrentMission(frame.Width, frame.Height);
                    missionSw.Stop();
                    CurrentProcessingTime = missionSw.Elapsed;
                }

                if (IsRecording && writer != null)
                    writer.WriteFrame(frame);
            }
        }

        private void MakeCurrentMission(int width, int height)
        {
            unsafe
            {
                fixed (byte* arr = imgBuffer)
                {
                    bool methodFinished = CurrentMission.RunMethod(arr, width, height);
                    

                    if (picBox != null)
                    {
                        picBox.Image = ByteArrayToImage(imgBuffer);
                    }

                    if (methodFinished)
                    {
                        OnAlgorithmFinished(new MissionEventArgs(CurrentMission));
                        SetMission(null);
                        CurrentProcessingTime = TimeSpan.Zero;
                    }
                }
            }
        }

        public void SetMission(Mission.Mission mission)
        {
            CurrentMission = mission;
            if (mission != null)
                mission.Reset();
        }

        protected virtual void OnAlgorithmFinished(MissionEventArgs e)
        {
            MissionEventHandler handler = AlgorithmFinished;
            if (handler != null) handler(this, e);
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

                cap = new Capture(camNumber);
                timer = new DispatcherTimer();
                timer.Tick += GrabFromCamera;
                timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                timer.Start();
                ChangeSize();
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
                timer.Stop();
                cap.Stop();
                cap.Dispose();
                cap = null;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot stop camera: " + ex.Message);
            }
        }

        /// <summary>
        /// Change size of the control
        /// </summary>
        private void ChangeSize()
        {
            if (cap == null)
                return;

            if (cap.Width / (float)Width > cap.Height / (float)Height)
            {
                viewImage.Width = Width;
                viewImage.Height = (int)((float)cap.Height / cap.Width * Width);
            }
            else
            {
                viewImage.Height = Height;
                viewImage.Width = (double) cap.Width/cap.Height*Height;
            }
        }


        // Taken from Emgu Wiki Tutorial - to convert frames to source for image control
        // http://www.emgu.com/wiki/index.php/WPF_in_CSharp
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        public void LandingOccured()
        {
            var mission = CurrentMission as LandAndStartMission;
            if (mission != null)
                mission.SetReadyForTakeOff();
        }

        public void DrawCrosshair(object sender, EventArgs e)
        {
            if (!IsCrosshair)
                return;
            Color color = Colors.Purple;
            var brush = new SolidColorBrush(color);
            canvas2.Children.Clear();
            double width = viewImage.ActualWidth;
            double height = viewImage.ActualHeight;
            canvas2.Children.Add(new System.Windows.Shapes.Rectangle
            {
                Width = width * 0.5,
                Height = height * 0.5,
                StrokeThickness = 2,
                Stroke = brush,
                
                Margin = new Thickness(-85, 85, 0, 0)
            });
            canvas2.Children.Add(new System.Windows.Shapes.Rectangle
            {
                Width = width * 0.30,
                Height = height * 0.30,
                StrokeThickness = 2,
                Stroke = brush,
                Margin = new Thickness(-85, 85, 0, 0)
            });

            canvas2.Children.Add(new System.Windows.Shapes.Rectangle
            {
                Width = 5,
                Height = 5,
                StrokeThickness = 3,
                Stroke = brush,
                Margin = new Thickness(-85, 85, 0, 0)
            });

            canvas2.InvalidateVisual();
            
        }
    }
}
