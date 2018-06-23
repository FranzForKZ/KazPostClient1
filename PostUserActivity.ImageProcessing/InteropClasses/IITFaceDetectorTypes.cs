using System;
using System.Runtime.InteropServices;


namespace IITFaceDetector
{
    /// <summary>
    /// This struct defines detector initial settings.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FACE_DETECTOR_SETTINGS
    {
        /// <summary>
        /// Input image width (in pixels)
        /// </summary>
        public int nWidth;

        /// <summary>
        /// Input image height (in pixels)
        /// </summary>

        public int nHeight;

        /// <summary>
        /// Minimum face size to search (in pixels)        
        /// </summary>
        public int nMinFaceSize;
    }

    /// <summary>
    /// This struct defines detected face area parameters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DETECTED_ITEM
    {
        /// <summary>
        /// Face rectangle:left-bottom corner X value.
        /// </summary>
        public int nX;

        /// <summary>
        /// Face rectangle:left-bottom corner Y value.
        /// </summary>
        public int nY;

        /// <summary>
        ///Face rectangle:Width.
        /// </summary>
        public int nW;

        /// <summary>
        ///Face rectangle:Height.
        /// </summary>
        public int nH;

        /// <summary>
        ///Approximate eyes coordinates:Left eye X value.
        /// </summary>
        public int nLEyeX;

        /// <summary>
        ///Approximate eyes coordinates:Left eye Y value. 
        /// </summary>
        public int nLEyeY;

        /// <summary>
        ///Approximate eyes coordinates:Right eye X value.
        /// </summary>
        public int nREyeX;

        /// <summary>
        ///Approximate eyes coordinates:Right eye Y value.
        /// </summary>
        public int nREyeY;

        /// <summary>
        ///Tilt face angle (in radians).
        /// </summary>
        public float fTiltAngle;

        /// <summary>
        /// Yaw face angle (in radians)
        /// </summary>
        public float fYawAngle;

        /// <summary>
        /// Detection probability, from 0.5 to 1.0.\n Faces with probability lesser than 0.5 not returned.
        /// </summary>
        public float fProbability;
    }

    /// <summary>
    /// This enum defines result codes, returned by face detection SDK.
    /// </summary>
    enum FACE_DETECTION_RESULT
    {
        /// <summary>
        /// No errors                                     
        /// </summary>
        FD_RESULT_OK = 0,
        /// <summary>
        /// Internal error                               
        /// </summary>
        FD_RESULT_ERROR = 1,
        /// <summary>
        /// Invalid parameter or parameters combination   
        /// </summary>
        FD_RESULT_PARAM = 2,
        /// <summary>
        /// Licensing error                               
        /// </summary>
        FD_RESULT_LICENSE_ERROR = 3,
        /// <summary>
        /// Licensing error                               
        /// </summary>
        FD_INVALID_POINTER = 4

    }

    /// <summary>
    /// Settings for simple tracker
    /// </summary>
    public struct FACE_TRACKER_SETTINGS
    {
        public int cnt;			 // Number of frames to forget a track
        public int nWidth;         // Input image width (in pixels) 
        public int nHeight;        // Input image height (in pixels)
        public int nMinFaceSize;   // Minimum face size to search (in pixels)
        public int nMaxFaceSize;   // Maximum face size to search (in pixels)
    }

    /// <summary>
    /// Struct containing Face information
    /// </summary>
    public struct TRACK_ITEM
    {
        public DETECTED_ITEM rect; // Standart geometric information.
        public int id;               // Id of track this face belongs to.
    };
}
