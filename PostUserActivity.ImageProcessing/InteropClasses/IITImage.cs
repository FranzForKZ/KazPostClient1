using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using IITFaceDetector;

namespace IITImage
{
    /// <summary>
    /// This class represents ImageObject object.
    /// </summary>
    public class ImageObject : IDisposable
    {
        /// <summary>
        /// Creates inner image object.
        /// </summary>
        /// <param name="Pixels">[In]Pointer to image data</param>
        /// <param name="Width">[In]Width of an input image in pixels</param>
        /// <param name="Height">[In]Height of an input image in pixels</param>
        /// <param name="Stride">[In]Stride in bytes</param>
        /// <param name="format">[In]Input image format</param>
        /// <param name="orientation">[In]Image orientation</param>
        /// <param name="Imageptr">[Out]Pointer to inner image object</param>
        /// <returns>Function returns standard error code(see FD_RESULT enum)</returns>

        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "Image_CreateImageFromData")]
        private static extern FACE_DETECTION_RESULT Image_CreateImageFromData(IntPtr Pixels, int Width, int Height, int Stride, IMAGE_FORMAT format, IMAGE_ORIENTATION orientation, ref IntPtr Imageptr);

        /// <summary>
        /// Releases inner image object
        /// </summary>
        /// <param name="Imageptr">Pointer to image object</param>
        /// <returns></returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "Image_Destroy")]
        private static extern FACE_DETECTION_RESULT Image_Destroy(IntPtr Imageptr);

        IntPtr IntObjPtr = IntPtr.Zero;

        /// <summary>
        /// Pointer to object on DLL side.(ImageObject handle)
        /// </summary>
        public IntPtr InnerPtr { get { return IntObjPtr; } }

        /// <summary>
        ///  Constructor. Creates object using ImageObject handle.
        /// </summary>
        /// <param name="IntObjPtr">ImageObject handle</param>
        public ImageObject(IntPtr IntObjPtr)
        {
            this.IntObjPtr = IntObjPtr;
        }

        /// <summary>
        /// Creates new ImageObject object from Image object
        /// </summary>
        /// <param name="Image">[In] Image object</param>
        /// <param name="Orientation">[In] Image orientation(see IMAGE_ORIENTATION enum)</param>
        /// <returns>Function returns standard error code(see FD_RESULT enum).</returns>
        public static ImageObject Create_ImageObject_fData(Bitmap Image, IMAGE_ORIENTATION Orientation = IMAGE_ORIENTATION.posLEFTTOP)
        {
            IntPtr tmpPtr = IntPtr.Zero;
            BitmapData BD = null;
            switch (Image.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    BD = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, Image.PixelFormat);
                    if (Image_CreateImageFromData(BD.Scan0, BD.Width, BD.Height, BD.Stride, IMAGE_FORMAT.fbmp8bppgrey, Orientation, ref tmpPtr) == FACE_DETECTION_RESULT.FD_RESULT_OK)
                    {
                        Image.UnlockBits(BD);
                        return new ImageObject(tmpPtr);
                    }
                    else
                    {
                        Image.UnlockBits(BD);
                        return null;
                    }
                
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:

                    BD = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, Image.PixelFormat);
                    if (Image_CreateImageFromData(BD.Scan0, BD.Width, BD.Height, BD.Stride, IMAGE_FORMAT.fbmp24bppRGB, Orientation, ref tmpPtr) == FACE_DETECTION_RESULT.FD_RESULT_OK)
                    {
                        Image.UnlockBits(BD);
                        return new ImageObject(tmpPtr);
                    }
                    else
                    {
                        Image.UnlockBits(BD);
                        return null;
                    }
                
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    BD = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, Image.PixelFormat);
                    if (Image_CreateImageFromData(BD.Scan0, BD.Width, BD.Height, BD.Stride, IMAGE_FORMAT.fbmp32bppARGB, Orientation, ref tmpPtr) == FACE_DETECTION_RESULT.FD_RESULT_OK)
                    {
                        Image.UnlockBits(BD);
                        return new ImageObject(tmpPtr);
                    }
                    else
                    {
                        Image.UnlockBits(BD);
                        return null;
                    }
                
                default:
                    return null;
            }
        }

        /// <summary>
        /// Releases mirror object
        /// </summary>
        public void Dispose()
        {
            if (IntObjPtr != IntPtr.Zero)
            {
                Image_Destroy(IntObjPtr);
                IntObjPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ImageObject()
        {
            if (IntObjPtr != IntPtr.Zero)
                Image_Destroy(IntObjPtr);
        }
    }

}
