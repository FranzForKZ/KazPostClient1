#ifndef __IIT_FACE_DETECTOR_H__
#define __IIT_FACE_DETECTOR_H__

/*
// MSVC DLL Import/Export support
*/
#if defined(_MSC_VER)
#ifdef _FACE_DET_EXPORTS
#define FACE_DET_EXPORT extern "C" __declspec(dllexport)
#else
#define FACE_DET_EXPORT extern "C" __declspec(dllimport)
#endif 
#else
#define FACE_DET_EXPORT
#endif

/*
// MSVC call type feature
*/
#if defined(_MSC_VER)
#define FACE_DET_CALLTYPE __stdcall
#else
#define FACE_DET_CALLTYPE
#endif

#include "IITImage.h"

namespace IITFD
{

/// @defgroup ExportFunctions List of exported functions
/// @{


/// Face detector object handle
typedef void* FD_OBJ_HANDLE;

/// This enum defines result codes, returned by face detection SDK.
typedef enum FACE_DETECTION_RESULT
{
	///No errors.
	FD_RESULT_OK  = 0, 
	///Internal error.
	FD_RESULT_ERROR = 1, 
	///Invalid parameter value, or combination of parameters. 
	FD_RESULT_PARAM  = 2, 
	///Licensing error. 
	FD_RESULT_LICENSE_ERROR  = 3 
} FACE_DETECTION_RESULT;

/// This struct defines detector initial settings. 
typedef struct FACE_DETECTOR_SETTINGS
{
	///Input image width, in pixels.
	int nWidth;  
	///Input image height, in pixels.
	int nHeight; 
	/// Minimum face size to search, in pixels.
	int	nMinFaceSize; 
} FACE_DETECTOR_SETTINGS;

/// This struct defines detected face area parameters.
typedef struct DETECTED_ITEM
{
	///Face rectangle:left-bottom corner X value.
	int nX,
	///Face rectangle:left-bottom corner Y value.
		nY,
	///Face rectangle:Width.
		nW, 
	///Face rectangle:Height.
		nH;

	///Approximate eyes coordinates:Left eye X value.
	int	nLEyeX, 
	///Approximate eyes coordinates:Left eye Y value.
		nLEyeY, 
	///Approximate eyes coordinates:Right eye X value.
		nREyeX, 
	///Approximate eyes coordinates:Right eye Y value.
		nREyeY; 

	///Tilt face angle, in radians.
	float fTiltAngle; 

	///Yaw face angle, in radians
	float fYawAngle; 

	///Detection probability, from 0.5 to 1.0. Faces with probability less than 0.5 not returned.
	float fProbability; 

} DETECTED_ITEM;




/// This structure contains track item description. 

typedef struct TRACK_ITEM{
	/// Face description
	DETECTED_ITEM Face;
	/// Track ID
	int id;		
}TRACK_ITEM;

/// This structure defines tracker initial settings.
typedef struct FACE_TRACKER_SETTINGS{
	///Track memory size in frames
	int FrameMemCnt;

	///Input image width, in pixels.
	int nWidth;       

	///Input image height, in pixels.
    int nHeight;

	///Minimum face size to search (in pixels).
    int nMinFaceSize; 

	///Maximum face size to search (in pixels).
	int nMaxFaceSize;
}FACE_TRACKER_SETTINGS;

/// <summary>
/// Creates inner image object.
/// </summary>
/// <param name="Pixels">[In]Pointer to image data</param>
/// <param name="Width">[In]Width of an input image in pixels</param>
/// <param name="Height">[In]Height of an input image in pixels</param>
/// <param name="Stride">[In]Stride in bytes</param>
/// <param name="img_format">[In]Input image format</param>
/// <param name="orientation">[In]Image orientation</param>
/// <param name="outImage">[Out]ImageObject handle</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum)</returns>

FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE 
Image_CreateImageFromData(
void* Pixels,
int Width,
int Height,
int Stride,
IITImage::IMAGE_FORMAT img_format,
IITImage::IMAGE_ORIENTATION orientation,
IITImage::IMG_OBJECT_HANDLE* outImage
);

/// <summary>
/// Releases inner image object
/// </summary>
/// <param name="Image">ImageObject object handle</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum)</returns>

FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE 
Image_Destroy
(
IITImage::IMG_OBJECT_HANDLE Image
);


/// <summary>
/// Creates face detector object using specified settings.
/// </summary>
/// <param name="pSettings">Pointer to FACE_DETECTOR_SETTINGS structure.</param>
/// <param name="phDetector">[Out] Created detector handle.</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum)</returns>
FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceDetector_Create(FACE_DETECTOR_SETTINGS*    pSettings,FD_OBJ_HANDLE* phDetector);

/// <summary>
/// Release face detector object.
/// </summary>
/// <param name="hDetector">Face Detector object handle.</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>

FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceDetector_Destroy(FD_OBJ_HANDLE hDetector);

/// <summary>
/// Detects faces on grayscale image. Image given by pixels pointer and image stride (row size), in bytes.
/// Function supports only grayscale bitmap images.
/// </summary>
/// <param name="hDetector">[In]  Face Detector object handle.</param>
/// <param name="Image">[In] ImageObject object handle.</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>
FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceDetector_DetectFaces(FD_OBJ_HANDLE hDetector,IITImage::IMG_OBJECT_HANDLE Image);

/// <summary>
/// Returns count of faces detected on image.
/// </summary>
/// <param name="hDetector">[In]  Face Detector object handle.</param>
/// <returns>Function returns count of faces detected on image.</returns>
FACE_DET_EXPORT int FACE_DET_CALLTYPE             
FaceDetector_GetDetectedCount(FD_OBJ_HANDLE hDetector);
 
/// <summary>
/// Returns detected face description.
/// </summary>
/// <param name="hDetector">[In]  Face Detector object handle.</param>
/// <param name="Index">[In]  Index of detected face.</param>
/// <param name="pItem">[Out]  Pointer to DETECTED_ITEM structure.</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>

FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceDetector_GetDetectedItem(FD_OBJ_HANDLE hDetector,int Index,DETECTED_ITEM* pItem);

/// <summary>
///  Release detector results.This function must be called after each detection.
/// </summary>
/// <param name="hDetector">Face Detector object handle.</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>

FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceDetector_DetectReset(FD_OBJ_HANDLE hDetector);


/// <summary>
/// Creates face tracker object using specified settings.
/// </summary>
/// <param name="pSettings">[In]Pointer to FACE_TRACKER_SETTINGS structure.</param>
/// <param name="phTracker">[Out] Created Face Tracker object handle.</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum)</returns>
FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceTracker_Create(FACE_TRACKER_SETTINGS*    pSettings,FD_OBJ_HANDLE* phTracker);

/// <summary>
/// Release face tracker object.
/// </summary>
/// <param name="hTracker">Face Tracker object handle.</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>

FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceTracker_Destroy(FD_OBJ_HANDLE hTracker);

/// <summary>
/// Detects and tracks faces on image.
/// </summary>
/// <param name="Handler">[In] Face Tracker object handle.</param>
/// <param name="Image">[In] ImageObject object handle</param>
/// <returns>Function returns standard error code(see FACE_DETECTION_RESULT enum).</returns>

FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceTracker_DetectFaces(FD_OBJ_HANDLE Handler, FD_OBJ_HANDLE Image);

/// <summary>
/// Returns track item(face description + track ID).
/// </summary>
/// <param name="Handler">[In] Face Tracker object handle.</param>
/// <param name="TrackInd">[In] Track index.</param>
/// <param name="pItem">[Out] Pointer to TRACK_ITEM structure.</param>
/// <returns>Function returns standard error code (see FACE_DETECTION_RESULT enum)</returns>

FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceTracker_GetDetectedItem(FD_OBJ_HANDLE Handler,int TrackInd,TRACK_ITEM* pItem );

/// <summary>
/// Returns count of tracks detected on image.
/// </summary>
/// <param name="hDetector">[In] Face Tracker object handle.</param>
/// <returns>Function returns count of tracks detected on image.</returns>
FACE_DET_EXPORT int FACE_DET_CALLTYPE
FaceTracker_GetDetectedCount(FD_OBJ_HANDLE hDetector);

/// <summary>
/// Release detector results.This function must be called after each detection.
/// </summary>
/// <param name="hDetector">[In]Face Detector object handle.</param>
/// <returns>Function returns standard error code(see FD_RESULT enum).</returns>
FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceTracker_DetectReset(FD_OBJ_HANDLE hDetector);
       
/// <summary>
/// Resets track data(all indexes and paths).
/// </summary>
/// <param name="Handler">[In] Face Tracker object handle.</param>
/// <returns>Function returns standard error code (see FACE_DETECTION_RESULT enum)</returns>
FACE_DET_EXPORT FACE_DETECTION_RESULT FACE_DET_CALLTYPE
FaceTracker_ResetTracks(FD_OBJ_HANDLE Handler);

/// }@
}

#endif /*__IIT_FACE_DETECTOR_H__*/
