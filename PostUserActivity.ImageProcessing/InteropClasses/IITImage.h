#ifndef __IIT_IMAGE_H__
#define __IIT_IMAGE_H__

namespace IITImage
{
    ///This enum defines supported bitmap formats
    enum IMAGE_FORMAT
    {
	    fbmp8bppgrey,	/// 8bit grayscale image
	    fbmp24bppRGB,	/// 24bit RGB image
	    fbmp24bppBGR,	/// 24bit BGR image
	    fbmp32bppARGB	/// 24bit ARGB image
    };

    ///This enum defines supported first bytes positions
    enum IMAGE_ORIENTATION
    {
	    posLEFTTOP,	/// First byte in left top corner of image
	    posLEFTBOTTOM 	/// First byte in left bottom corner of image
    };

    ///ImageObject object handle
    typedef void* IMG_OBJECT_HANDLE;
}

#endif /*__IIT_IMAGE_H__*/
