using System;
using System.Runtime.InteropServices;

namespace IITFaceRec
{
    /// <summary>
    /// This enum defines result codes, returned by face recognition SDK.
    /// </summary>

    public enum FR_RESULT
    {
        /// <summary>
        ///  All ok                                    
        /// </summary>
        FR_RESULT_OK,
        /// <summary>
        ///  Generic errors                               
        /// </summary>
        FR_RESULT_ERROR,
        /// <summary>
        ///  Out of memory                                
        /// </summary>
        FR_RESULT_OUTOFMEMORY,
        /// <summary>
        ///  Method is not implemented, reserved  
        /// </summary>
        FR_RESULT_NOTIMPL,
        /// <summary>
        ///  Zero pointer was passed                      
        /// </summary>
        FR_RESULT_POINTER,
        /// <summary>
        ///  Licensing error                              
        /// </summary>
        FR_RESULT_NOLICENSE,    
        /// <summary>
        ///  Operation aborted  
        /// </summary>
        FR_RESULT_ABORT,
        /// <summary>
        ///  Recognition failed on specified image        
        /// </summary>
        FR_RESULT_RECOGFAILED,
        /// <summary>
        ///  Invalid parameter or parameters combination  
        /// </summary>
        FR_RESULT_INVALIDPARAM,
        /// <summary>
        /// Invalid template                                  
        /// </summary>
        FR_RESULT_INVALIDKEY,
    };

    /// <summary>
    /// This struct defines identification result.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IDENTIFICATION_RESULT
    {
        /// <summary>
        ///  Unique ID
        /// </summary>
        public UInt32 UniqueID;
        /// <summary>
        /// Similarity score
        /// </summary>
        public float Score;
    }

 
    /// <summary>
    /// This struct describes eye points on user image.
    /// Y=0 for bottom image line, and grows in the up direction.
    /// All values are in pixels.
    /// Note, that a left eye means here actually a right human eye, which will seen as a left on the image.
    /// The simple rule to be sure that everything  is correct is check whether x-coordinate of the left eye
    /// is less than x-coordinate of the right eye.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FACE_POINTS
    {
        /// <summary>
        /// Approximate eyes coordinates:Left eye X value.
        /// </summary>
        public int lEyeX;

        /// <summary>
        /// Approximate eyes coordinates:Left eye Y value.
        /// </summary>
        public int lEyeY;

        /// <summary>
        /// Approximate eyes coordinates:Right eye X value.
        /// </summary>
        public int rEyeX;

        /// <summary>
        /// Approximate eyes coordinates:Right eye Y value.
        /// </summary>
        public int rEyeY;        
    };
}
