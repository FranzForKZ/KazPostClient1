using System;
using System.Runtime.InteropServices;

namespace IITImage
{
    /// <summary>
    /// This enum defines supported first bytes positions
    /// </summary>
    public enum IMAGE_ORIENTATION
    {
        /// <summary>
        /// First byte in left top corner of image
        /// </summary>
        posLEFTTOP,
        /// <summary>
        /// First byte in left bottom corner of image
        /// </summary>
        posLEFTBOTTOM
    };

    /// <summary>
    /// This enum defines supported bitmap formats
    /// </summary>
    public enum IMAGE_FORMAT
    {
        /// <summary>
        /// 8bit grayscale image
        /// </summary>
        fbmp8bppgrey,
        /// <summary>
        /// 24bit RGB image
        /// </summary>
        fbmp24bppRGB,
        /// <summary>
        /// 24bit BGR image
        /// </summary>
        fbmp24bppBGR,
        /// <summary>
        /// 32bit ARGB image
        /// </summary>
        fbmp32bppARGB
    };


    /// <summary>
    /// This struct defines bitmap image representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IMAGE_OBJECT
    {
        /// <summary>
        /// Width of an input image in pixels
        /// </summary>
        public int nWidth;

        /// <summary>
        /// Height of an input image in pixels
        /// </summary>
        public int nHeight;

        /// <summary>
        /// Stride in bytes
        /// </summary>
        public int nStride;

        /// <summary>
        /// Pixels pointer of an input image, bottom line first (for positive strides)
        /// </summary>
        public IntPtr pPixels;
    };


}
