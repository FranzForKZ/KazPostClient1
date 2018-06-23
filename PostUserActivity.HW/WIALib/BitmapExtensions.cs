using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PostUserActivity.HW.WIALib
{
    /// <summary>
    /// Extensions to the System.Drawing.Bitmap class.
    /// </summary>
    /// <remarks>
    /// This code originated from http://www.chinhdo.com/20080910/detect-blank-images
    /// </remarks>
    public static class BitmapExtensions
    {

        /// <summary>
        /// Determines whether or not a given Bitmap is blank.
        /// </summary>
        /// <param name="bitmap">The instance of the Bitmap for this method extension.</param>
        /// <returns>Returns trueif the given Bitmap is blank; otherwise returns false.</returns>
        public static bool IsBlank(this Bitmap bitmap)
        {
            return bitmap.IsBlank(BlankDetectionLevels.Medium);
        }

        /// <summary>
        /// Determines whether or not a given Bitmap is blank.
        /// </summary>
        /// <param name="bitmap">The instance of the Bitmap for this method extension.</param>
        /// <param name="level"></param>
        /// <returns>Returns trueif the given Bitmap is blank; otherwise returns false.</returns>
        public static bool IsBlank(this Bitmap bitmap, BlankDetectionLevels level)
        {
            int tolerance;
            double stdDev = GetStdDev(bitmap);
            switch (level)
            {
                case BlankDetectionLevels.VeryLow:
                    tolerance = 250000;
                    break;
                case BlankDetectionLevels.Low:
                    tolerance = 500000;
                    break;
                case BlankDetectionLevels.High:
                    tolerance = 1000000;
                    break;
                case BlankDetectionLevels.VeryHigh:
                    tolerance = 1250000;
                    break;
                default:
                    tolerance = 750000;
                    break;
            }
            return stdDev < tolerance;
        }

        /// <summary>
        /// Determines whether or not a given Bitmap is blank.
        /// </summary>
        /// <param name="bitmap">The instance of the Bitmap for this method extension.</param>
        /// <param name="tolerance">
        /// A number from 1 to 5000 that represents the sensitivity for the detection. 
        /// The default is 750, which is the equivalent of BlankDetectionLevels.Medium.
        /// </param>
        /// <returns>Returns true if the given Bitmap is blank; otherwise returns false.</returns>
        public static bool IsBlank(this Bitmap bitmap, int tolerance = 750)
        {
            double stdDev = GetStdDev(bitmap);
            return stdDev < (tolerance * 1000);
        }


        /// <summary>
        /// Gets the bits per pixel (bpp) for the given .
        /// </summary>
        /// <param name="bitmap">The instance of the Bitmap for this method extension.</param>
        /// <returns>Returns a representing the bpp for the .</returns>
        internal static byte GetBitsPerPixel(this Bitmap bitmap)
        {
            byte bpp = 0x1;

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    bpp = 0x1;
                    break;
                case PixelFormat.Format4bppIndexed:
                    bpp = 0x4;
                    break;
                case PixelFormat.Format8bppIndexed:
                    bpp = 0x8;
                    break;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                    bpp = 0x16;
                    break;
                case PixelFormat.Format24bppRgb:
                    bpp = 0x24;
                    break;
                case PixelFormat.Canonical:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    bpp = 0x32;
                    break;
                case PixelFormat.Format48bppRgb:
                    bpp = 0x48;
                    break;
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    bpp = 0x64;
                    break;
            }
            return bpp;
        }

        /// <summary>
        /// Get the standard deviation of pixel values.
        /// </summary>
        /// <param name="bitmap">The instance of the for this method extension.</param>
        /// <returns>Returns the standard deviation of pixel population of the Bitmap.</returns>
        public static double GetStdDev(this Bitmap bitmap)
        {
            double total = 0;
            double totalVariance = 0;
            int count = 0;
            double stdDev = 0;

            // First get all the bytes
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int stride = bmData.Stride;
            IntPtr scan0 = bmData.Scan0;

            byte bitsPerPixel = GetBitsPerPixel(bitmap);
            byte bytesPerPixel = (byte)(bitsPerPixel / 8);

            unsafe
            {
                var p = (byte*)(void*)scan0;
                var nOffset = stride - bitmap.Width * bytesPerPixel;
                for (var y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        count++;

                        byte blue = p[0];
                        byte green = p[1];
                        byte red = p[2];

                        int pixelValue = Color.FromArgb(0, red, green, blue).ToArgb();
                        total += pixelValue;
                        double avg = total / count;
                        totalVariance += Math.Pow(pixelValue - avg, 2);
                        stdDev = Math.Sqrt(totalVariance / count);
                        p += bytesPerPixel;
                    }
                    p += nOffset;
                }
            }
            bitmap.UnlockBits(bmData);

            return stdDev;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            var sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            var destination = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            destination.Freeze();
            return destination;
        }

    }
}
