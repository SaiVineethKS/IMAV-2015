using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace UglyGCS
{
    class Algorithms
    {

        private const string DllPath = @"..\..\..\" +
#if DEBUG
                                       "Debug" +
#else
            "Release" +
#endif
                                       @"\Algorithms.dll";

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TestAdd(int x1, int x2);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void TestImage(byte* imgBuffer, int width, int height);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        unsafe public static extern string RecognizeQrCode(byte* imgBuffer, int width, int height);

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
