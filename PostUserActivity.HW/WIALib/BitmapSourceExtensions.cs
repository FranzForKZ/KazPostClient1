using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace PostUserActivity.HW.WIALib
{
    /// <summary>
    /// Extensions to the System.Windows.Media.Imaging.BitmapSource class.
    /// </summary>
    public static class BitmapSourceExtensions
    {
        /// <summary>
        /// Converts the BitmapSource to a Bitmap.
        /// </summary>
        /// <param name="bitmapSource">The instance of the BitmapSource for this method extension.</param>
        /// <returns>A Bitmap of the given BitmapSource.</returns>
        public static System.Drawing.Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            System.Drawing.Bitmap bitmap;
            using (var output = new MemoryStream())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(output);
                bitmap = new System.Drawing.Bitmap(output);
            }

            return bitmap;
        }
    }
}
