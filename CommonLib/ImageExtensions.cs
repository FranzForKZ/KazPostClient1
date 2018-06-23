using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public static class ImageExtensions
    {
        public static string ToBase64(this Image img)
        {
            ImageCodecInfo codecInfo = GetEncoderInfo(ImageFormat.Jpeg);
            //  Set the quality

            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            
            MemoryStream ms = new MemoryStream();
            img.Save(ms, codecInfo, parameters);
            //img.Save(ms, ImageFormat.Jpeg);
            return "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
        }

        public static Image ToImage(this string base64)
        {
            if (base64.Contains("data:image/jpeg;base64,"))
            {
                base64 = base64.Replace("data:image/jpeg;base64,", "");
            }
            var byteArray = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(byteArray);
            var img = Image.FromStream(ms);
            return img;
        }

        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().ToList().Find(delegate (ImageCodecInfo codec)
            {
                return codec.FormatID == format.Guid;
            });
        }
        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            using(Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            }
            return bmp;
        }
    }
}
