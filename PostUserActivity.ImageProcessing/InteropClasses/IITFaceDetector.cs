using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using IITImage;

namespace IITFaceDetector
{
 
  /// <summary>
  /// FaceDetector class.Provides easy access to DLL functions.Each object have "mirror" object on DLL side.
  /// </summary>
    public class FaceDetector : IDisposable
    {
        /// <summary>
        /// Creates face detector object using specified settings.
        /// </summary>
        /// <param name="Settings">[In]Pointer to FACE_DETECTOR_SETTINGS structure.</param>
        /// <param name="Detector">[Out] Created detector handle.</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum)</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceDetector_Create")]
        private static extern FACE_DETECTION_RESULT FaceDetector_Create(ref FACE_DETECTOR_SETTINGS Settings, ref IntPtr Detector);

        /// <summary>
        /// Releases face detector object.
        /// </summary>
        /// <param name="Detector">[In]Face Detector object handle.</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceDetector_Destroy")]
        private static extern FACE_DETECTION_RESULT FaceDetector_Destroy(IntPtr Detector);

        /// <summary>
        ///  Release detector results.This function must be called after each detection.
        /// </summary>
        /// <param name="Detector">[In]Face Detector object handle.</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceDetector_DetectReset")]
        private static extern FACE_DETECTION_RESULT FaceDetector_DetectReset(IntPtr Detector);

        /// <summary>
        /// Detects faces on image.
        /// </summary>
        /// <param name="Detector">[In]  Face Detector object handle.</param>
        /// <param name="Image">[In] ImageObject object handle</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceDetector_DetectFaces")]
        private static extern FACE_DETECTION_RESULT FaceDetector_DetectFaces(IntPtr Detector, IntPtr Image);

        /// <summary>
        /// Returns count of faces detected on image.
        /// </summary>
        /// <param name="Detector">[In]  Face Detector object handle.</param>
        /// <returns>Function returns count of faces detected on image.</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceDetector_GetDetectedCount")]
        private static extern int FaceDetector_GetDetectedCount(IntPtr Detector);

        /// <summary>
        /// Returns detected face description.
        /// </summary>
        /// <param name="Detector">[In]  Face Detector object handle.</param>
        /// <param name="Index">[In]  Index of detected face.</param>
        /// <param name="Item">[Out]  Pointer to DETECTED_ITEM structure.</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceDetector_GetDetectedItem")]
        private static extern FACE_DETECTION_RESULT FaceDetector_GetDetectedItem(IntPtr Detector, int Index, ref DETECTED_ITEM Item);

        /// <summary>
        /// Pointer to object on DLL side.        
        /// </summary>
        private IntPtr IntObjPtr = IntPtr.Zero;

        /// <summary>
        /// Constructor. Creates object using specified settings.
        /// </summary>
        /// <param name="Settings">[In]Face detector settings</param>
        public FaceDetector(FACE_DETECTOR_SETTINGS Settings)
        {
            FaceDetector_Create(ref Settings, ref IntObjPtr);
        }

        /// <summary>
        /// Class constructor. Creates object for image.
        /// </summary>
        /// <param name="Image">[In]Pointer to Bitmap object.</param>
        /// <param name="MinFaceSize">[In]Minimum face size in pixels default value is 20% of image size.</param>
        public FaceDetector(Image Image, int MinFaceSize = -1)
        {
            FACE_DETECTOR_SETTINGS Settings = new FACE_DETECTOR_SETTINGS();
            Settings.nHeight = Image.Height;
            Settings.nWidth = Image.Width;
            if (MinFaceSize < 0) MinFaceSize = (int)(Math.Min(Image.Width, Image.Height) * 0.2);
            Settings.nMinFaceSize = MinFaceSize;
            FaceDetector_Create(ref Settings, ref IntObjPtr);
        }

        /// <summary>
        /// Static method that detects faces on image.
        /// </summary>
        /// <param name="Image">[In]Pointer to Bitmap object.</param>
        /// <param name="MinFaceSize">[In]Minimum face size in pixels default value is 20% of image size.</param>
        /// <returns>Method returns list with detected faces or empty list when no faces found. </returns>
        public static List<DETECTED_ITEM> DetectFacesfromImage(Bitmap Image, int MinFaceSize = -1)
        {
            if (MinFaceSize < 0) MinFaceSize = (int)(Math.Min(Image.Width, Image.Height) * 0.2);
            using (FaceDetector TempDetector = new FaceDetector(Image, MinFaceSize))
            {
                return TempDetector.DetectFaces(Image);
            }
        }

        /// <summary>
        /// Static method that detects faces on image provided as byte array.First byte corresponds to lower left corner on image.
        /// </summary>
        /// <param name="Image">[In]ImageObject object</param>
        /// <param name="Width">[In]Image width</param>
        /// <param name="Height">[In]Image height</param>
        /// <param name="MinFaceSize">[In]Minimum face size in pixels default value is 20% of image size.</param>
        /// <returns>Method returns list with detected faces or empty list when no faces found.</returns>
        public static List<DETECTED_ITEM> DetectFacesfromImage(ImageObject ImageObject,int Width,int Height,int MinFaceSize = -1)
        {
            if (MinFaceSize < 0) MinFaceSize =  (int)(Math.Min(Width, Height) * 0.2);
            FACE_DETECTOR_SETTINGS Settings = new FACE_DETECTOR_SETTINGS();
            Settings.nHeight = Height;
            Settings.nWidth = Width;
            Settings.nMinFaceSize = MinFaceSize;
            FaceDetector TempDetector = new FaceDetector(Settings);
            return TempDetector.DetectFaces(ImageObject);
        }

        /// <summary>
        /// Method detects face on Bitmap object.
        /// </summary>
        /// <param name="Image">[In]Pointer to Bitmap object.</param>
        /// <returns>Method returns list with detected faces or empty list when no faces found.</returns>
        public List<DETECTED_ITEM> DetectFaces(Bitmap Image)
        {
            using (ImageObject ImageObject = ImageObject.Create_ImageObject_fData(Image))
            {
                return DetectFaces(ImageObject);
            }

        }

        /// <summary>
        /// Method detects face on image provided as byte array.
        /// </summary>
        /// <param name="Image">[In]ImageObject object</param>
        /// <returns>Method returns list with detected faces or empty list when no faces found.</returns>
        public List<DETECTED_ITEM> DetectFaces(ImageObject ImageObject)
        {
            List<DETECTED_ITEM> DetectedFaces = new List<DETECTED_ITEM>();
            if ((ImageObject != null) && (FaceDetector_DetectFaces(IntObjPtr, ImageObject.InnerPtr) == FACE_DETECTION_RESULT.FD_RESULT_OK))
            {
                int DetectedCount = FaceDetector_GetDetectedCount(IntObjPtr);
                if (DetectedCount > 0)
                {
                    for (int i = 0; i < DetectedCount; i++)
                    {
                        DETECTED_ITEM Item = new DETECTED_ITEM();
                        FaceDetector_GetDetectedItem(IntObjPtr, i, ref Item);
                        DetectedFaces.Add(Item);
                    }
                    FaceDetector_DetectReset(IntObjPtr);
                }

            }
            return DetectedFaces;

        }

        /// <summary>
        /// Releases mirror object
        /// </summary>


        public void Dispose()
        {
            if (IntObjPtr != IntPtr.Zero)
            {
                FaceDetector_Destroy(IntObjPtr);
                IntObjPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Object destructor.
        /// </summary>

        ~FaceDetector()
        {
            Dispose();
        }

    }

    /// <summary>
    /// FaceTracker class.Provides easy access to DLL functions.Each object have "mirror" object on DLL side.
    /// </summary>
    public class FaceTracker
    {

        /// <summary>
        /// Creates face tracker object using specified settings.
        /// </summary>
        /// <param name="Settings">[In]Pointer to FACE_TRACKER_SETTINGS structure.</param>
        /// <param name="Tracker">[Out] Created Face Tracker object handle.</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum)</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTracker_Create")]
        private static extern FACE_DETECTION_RESULT FaceTracker_Create(ref FACE_TRACKER_SETTINGS Settings, ref IntPtr Tracker);


        /// <summary>
        /// Releases face tracker object.
        /// </summary>
        /// <param name="Tracker">[In] Face Tracker object handle.</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTracker_Destroy")]
        private static extern FACE_DETECTION_RESULT FaceTracker_Destroy(IntPtr Tracker);

        /// <summary>
        /// Detects and tracks faces on image.
        /// </summary>
        /// <param name="Tracker">[In] Face Tracker object handle.</param>
        /// <param name="Image">[In] ImageObject object handle</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTracker_DetectFaces")]
        private static extern FACE_DETECTION_RESULT FaceTracker_DetectFaces(IntPtr Tracker, IntPtr Image);

        /// <summary>
        /// Returns track item(face description + track ID).
        /// </summary>
        /// <param name="Tracker">[In] Face Tracker object handle.</param>
        /// <param name="TrackInd">[In] Track index.</param>
        /// <param name="rect">[Out] Pointer to TRACK_ITEM structure.</param>
        /// <returns>Function returns standard error code (see FACE_DETECTION_RESULT enum)</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTracker_GetDetectedItem")]
        private static extern FACE_DETECTION_RESULT FaceTracker_GetDetectedItem(IntPtr Tracker, int TrackInd, ref TRACK_ITEM rect);

        /// <summary>
        /// Resets track data(all indexes and paths).
        /// </summary>
        /// <param name="Handler">[In] Face Tracker object handle.</param>
        /// <returns>Function returns standard error code (see FACE_DETECTION_RESULT enum)</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTracker_ResetTracks")]
        private static extern FACE_DETECTION_RESULT FaceTracker_ResetTracks(IntPtr Handler);


        /// <summary>
        /// Returns count of tracks detected on image.
        /// </summary>
        /// <param name="Detector">[In] Face Tracker object handle.</param>
        /// <returns>Function returns count of tracks detected on image.</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTracker_GetDetectedCount")]
        private static extern int FaceTracker_GetDetectedCount(IntPtr Detector);


        /// <summary>
        ///  Release detector results.This function must be called after each detection.
        /// </summary>
        /// <param name="Detector">[In]Face Detector object handle.</param>
        /// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>
        [DllImport("IITFD.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTracker_DetectReset")]
        private static extern FACE_DETECTION_RESULT FaceTracker_DetectReset(IntPtr Detector);



        /// <summary>
        /// Pointer to object on DLL side.
        /// </summary>
        private IntPtr IntObjPtr = IntPtr.Zero;

        /// <summary>
        /// Tracker object constructor.
        /// </summary>
        /// <param name="settings">[In] Structure with Tracker settings.</param>
        public FaceTracker(FACE_TRACKER_SETTINGS settings)
        {
            FaceTracker_Create(ref settings, ref IntObjPtr);
        }

        /// <summary>
        /// Detects and tracks faces on image.
        /// </summary>
        /// <param name="Image">[In]Pointer to Bitmap object.</param>
        /// <returns>Method returns list with tracking data or empty list when no faces found.</returns>
        public List<TRACK_ITEM> ProcessFrame(Bitmap Image)
        {
            using (ImageObject IO = ImageObject.Create_ImageObject_fData(Image))
            {
                return ProcessFrame(IO);
            }
        }

        /// <summary>
        /// Detects and tracks faces on ImageObject object.
        /// </summary>
        /// <param name="Image">[In]Pointer to Bitmap object.</param>
        /// <returns>Method returns list with tracking data or empty list when no faces found.</returns>
        public List<TRACK_ITEM> ProcessFrame(ImageObject IO)
        {
            List<TRACK_ITEM> Faces = new List<TRACK_ITEM>();
          
            FaceTracker_DetectFaces(IntObjPtr, IO.InnerPtr);
            int size = FaceTracker_GetDetectedCount(IntObjPtr);
            for (int i = 0; i < size; i++)
            {
                TRACK_ITEM temp = new TRACK_ITEM();
                FaceTracker_GetDetectedItem(IntObjPtr, i, ref temp);
                Faces.Add(temp);
            }
            FaceTracker_DetectReset(IntObjPtr);
            return Faces;
        }

        /// <summary>
        /// Reset Tracker to default state.
        /// </summary>
        public void ResetTracks()
        {
            FaceTracker_ResetTracks(IntObjPtr);
        }


        public void Dispose()
        {
            if (IntObjPtr != IntPtr.Zero)
            {
                FaceTracker_Destroy(IntObjPtr);
                IntObjPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }
}
