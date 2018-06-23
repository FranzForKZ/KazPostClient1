using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EfesSDKN
{

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct TEfesResult
    {
        public int Fim;
        public int FD;
        //        public TRect RD;
        public int EyeLx;
        public int EyeLy;
        public int EyeRx;
        public int EyeRy;
        public int SF;
        public int Pos;
        public int Sl;
        public int R;
        public int Bit;
        public int H;
        public int L;
        public int F;
        public int C;
        public IntPtr SIm;
        public int SW;
        public int SH;
        public IntPtr PIm;
        public int PW;
        public int PH;

    }



    class EfesSDK
    {

        /*      static ISOResult CopyToISOResult(TEfesResult isoResult)
              {
                  ISOResult IR = new ISOResult();
                  IR.ImageFormat = isoResult.Fim;
                  IR.FD = isoResult.FD;
                  IR.RD = new Rectangle(0, 0,isoResult.SW, isoResult.SH);
                  IR.EyeLeft = new Point(isoResult.EyeLx, isoResult.EyeLy);
                  IR.EyeRight = new Point(isoResult.EyeRx, isoResult.EyeRy);
                  IR.SF = isoResult.SF;
                  IR.Pos = isoResult.Pos;
                  IR.Sl = isoResult.Sl;
                  IR.R = isoResult.R;
                  IR.Bit = isoResult.Bit;
                  IR.H = isoResult.H;
                  IR.L = isoResult.L;
                  IR.F = isoResult.F;
                  IR.C = isoResult.C;
                  return IR;
              }*/

        [DllImport("EfesSDK.dll", CharSet = CharSet.Ansi, EntryPoint = "CreateEfes")]
        public static extern int InitISOChecker(string path);

        [DllImport("EfesSDK.dll", CharSet = CharSet.Ansi, EntryPoint = "DestoyEfes")]
        public static extern int DestroyISOChecker();

        [DllImport("EfesSDK.dll", CharSet = CharSet.Ansi, EntryPoint = "GetEfesResult")]
        private static extern int CheckFaceISO(byte[] imgData, int width, int height, int rowSize, ref TEfesResult isoResult);

        public static int CheckFaceISO(Bitmap image, ref TEfesResult isoResult, out Bitmap FrontalBmp)
        {
            //      image = Convert_to24bpp(image);
            if (image.PixelFormat != PixelFormat.Format24bppRgb) throw new ArgumentException("Wrong image pixel format. Please use 24bpp image.");

            int retval;
            var isoResultUnm = new TEfesResult();

            /*  isoResultUnm.EyeLx = isoResult.EyeLeft.X;
              isoResultUnm.EyeLy = isoResult.EyeLeft.Y;

              isoResultUnm.EyeRx = isoResult.EyeRight.X;
              isoResultUnm.EyeRy = isoResult.EyeRight.Y;
              */
            int stride = (image.Width * 3 + 3) / 4 * 4;

            using (var stream = new System.IO.MemoryStream())
            {
                image.Save(stream, ImageFormat.Bmp);
                var buffer = stream.ToArray();
                var buffer2 = new byte[stream.Length - 54];
                Array.Copy(buffer, 54, buffer2, 0, stream.Length - 54);

                retval = CheckFaceISO(buffer2, image.Width, image.Height, stride, ref isoResultUnm);
            }


            isoResult = isoResultUnm;
            //            var ResIm = new byte[isoResultUnm.SW * isoResultUnm.SH*3];
            //            GCHandle handle = GCHandle.Alloc(ResIm, GCHandleType.Pinned); 
            //            int scan0 = (int)handle.AddrOfPinnedObject();
            //            scan0 += (Height - 1) * Stride;

            FrontalBmp = null;

            if (isoResultUnm.SW > 0)
            {
                int scan0 = (int)isoResultUnm.SIm;
                scan0 += (isoResultUnm.SH - 1) * isoResultUnm.SW * 3;
                FrontalBmp = new Bitmap(isoResultUnm.SW, isoResultUnm.SH, -isoResultUnm.SW * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, (IntPtr)scan0);
            }

            if (retval == 1)
            {
                return 0;
            }
            if (retval == 0) return -1;

            return -1;
        }
    }
}

