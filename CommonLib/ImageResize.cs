using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CommonLib
{
    /// <summary>
    /// вспомогательный класс, для ресайза картинок
    /// </summary>
    public class ImageResize
    {
        /// <summary>
        /// resize image to show in smaller windows
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap Resize(Image bitmap, int width, int height)
        {
            if (bitmap == null)
                return null;
            if (width == 0 || height == 0)
                return new Bitmap(bitmap);

            var bmp = new Bitmap(width, height);
            using (var graph = Graphics.FromImage(bmp))
            {
                var scale = Math.Min((float)width / bitmap.Width, (float)height / bitmap.Height);
                var brush = new SolidBrush(Color.Black);
                var scaleWidth = Convert.ToInt32(bitmap.Width * scale);
                var scaleHeight = Convert.ToInt32(bitmap.Height  * scale);

                graph.FillRectangle(brush, new RectangleF(0.0f, 0.0f, width, height));
                graph.DrawImage(bitmap, new Rectangle(((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight));
            }
            
            return bmp;
        }
        
    }
}
