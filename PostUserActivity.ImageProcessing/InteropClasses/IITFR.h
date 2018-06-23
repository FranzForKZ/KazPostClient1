#ifndef __IIT_FACE_RECOGNITION_H__
#define __IIT_FACE_RECOGNITION_H__

/*
// MSVC DLL Import/Export support
*/
#if defined(_MSC_VER)
#ifdef _FACE_REC_EXPORTS
#define FACE_REC_EXPORT extern "C" __declspec(dllexport)
#else
#define FACE_REC_EXPORT extern "C" __declspec(dllimport)
#endif 
#else
#define FACE_REC_EXPORT
#endif

/*
// MSVC call type feature
*/
#if defined(_MSC_VER)
#define FACE_REC_CALLTYPE __stdcall
#else
#define FACE_REC_CALLTYPE
#endif

#include "IITImage.h"

namespace IITFR
{

/// @defgroup ExportFunctions List of exported functions
/// @{

//////////////////////////////////////////Common data types/////////////////////////////////////////////

/// face recognition SDK object handle
typedef void* FR_OBJ_HANDLE;
/// Unsigned int short defenition
typedef unsigned int UINT32;

/// This enum defines result codes, returned by identification SDK
enum FR_RESULT
{
	///All Ok
    FR_RESULT_OK,     
	///Generic errors
    FR_RESULT_ERROR,
	///Out of memory
    FR_RESULT_OUTOFMEMORY,
	///Method is not implemented, reserved
    FR_RESULT_NOTIMPL,
	///Zero pointer was passed
    FR_RESULT_POINTER,
	///Licensing error
    FR_RESULT_NOLICENSE,
	///Operation aborted
    FR_RESULT_ABORT,
	///Recognition failed on specified image
    FR_RESULT_RECOGFAILED,
	///Invalid parameter or parameters combination
    FR_RESULT_INVALIDPARAM,
	///Invalid template
    FR_RESULT_INVALIDKEY,
};


/// This struct describes eye points on user image.
/// Y=0 for bottom image line, and grows in the up direction.
/// All values are in pixels.
/// Note, that a left eye means here actually a right human eye, which will seen as a left on the image.
/// The simple rule to be sure that everything  is correct is check whether x-coordinate of the left eye
/// is less than x-coordinate of the right eye.

struct FACE_POINTS
{
	/// Approximate eyes coordinates:Left eye X value.
    int lEyeX;       
	/// Approximate eyes coordinates:Left eye Y value.
    int lEyeY;        
	/// Approximate eyes coordinates:Right eye X value.
    int rEyeX;
	/// Approximate eyes coordinates:Right eye Y value.
    int rEyeY;
};


/// This struct defines identification result
struct IDENTIFICATION_RESULT
{
	///  Unique ID
	UINT32 UniqueID;
	/// Similarity score
	float score;
};


/// <summary>
/// Enable debug mode.
/// </summary>
/// <param name="Path2Save">[In]Path for logs and images.</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE Set_Debugmode(char* Path2Save);

/// <summary>
/// Library initialization
/// </summary>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE InitLib();
 
/// <summary>
/// Releases library resources
/// </summary>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  FreeLib();

 #ifndef __IIT_FACE_DETECTOR_H__

//Only if FaceDetector library not defined else use same function in Face Detector library
//////////////////////////////////////////IMAGE_OBJECT////////////////////////////////////////////

/// <summary>
/// Creates inner image object.
/// </summary>
/// <param name="Pixels">[In]Pointer to image data</param>
/// <param name="Width">[In]Width of an input image in pixels</param>
/// <param name="Height">[In]Height of an input image in pixels</param>
/// <param name="Stride">[In]Stride in bytes</param>
/// <param name="img_format">[In]Input image format</param>
/// <param name="orientation">[In]Image orientation</param>
/// <param name="outImage">[Out]Image object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>

FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE 
Image_CreateImageFromData(void* Pixels, int Width, int Height, int Stride, IITImage::IMAGE_FORMAT img_format, IITImage::IMAGE_ORIENTATION orientation, IITImage::IMG_OBJECT_HANDLE* outImage);

/// <summary>
/// Releases inner image object
/// </summary>
/// <param name="Image">Image object handle</param>
/// <returns></returns>

FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE 
Image_Destroy(IITImage::IMG_OBJECT_HANDLE Image);

#endif // __IIT_FACE_DETECTOR_H__

//////////////////////////////////////////Verifier/////////////////////////////////////////////

/// <summary>
/// Creates biometric vector Verifier object
/// </summary>
/// <param name="Verifier">[Out] Template Verifier object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Verifier_Create(FR_OBJ_HANDLE* Verifier);
 
/// <summary>
/// Releases biometric vector Verifier object
/// </summary>
/// <param name="Verifier"> [In] Template Verifier object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Verifier_Destroy(FR_OBJ_HANDLE Verifier);

/// <summary>
/// Returns similarity score of two biometric Template objects.
/// </summary>
/// <param name="Verifier">[In] Template Verifier object handle</param>
/// <param name="TemplateA">[In] Biometric "A" Template object handle</param>
/// <param name="TemplateB">[In] Biometric "B" Template object handle</param>
/// <param name="pfScore">[Out] Similarity score between "A" and "B" vectors.</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Verifier_Compare (FR_OBJ_HANDLE Verifier,FR_OBJ_HANDLE TemplateA,FR_OBJ_HANDLE TemplateB,float* pfScore);

//////////////////////////////////////////StreamVerifier///////////////////////////////////////////// 

/// <summary>
/// Creates biometric vector StreamVerifier object
/// </summary>
/// <param name="Verifier">[Out] Template Verifier object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE StreamVerifier_Create(FR_OBJ_HANDLE* Verifier);

/// <summary>
/// Releases biometric vector Verifier object
/// </summary>
/// <param name="Verifier"> [In] Template Verifier object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE StreamVerifier_Destroy(FR_OBJ_HANDLE Verifier);

/// <summary>
/// Returns similarity score of two biometric Template objects.
/// </summary>
/// <param name="Verifier">[In] Template Verifier object handle</param>
/// <param name="TemplateA">[In] Biometric "A" Template object handle</param>
/// <param name="TemplateB">[In] Biometric "B" Template object handle</param>
/// <param name="score">[Out] Similarity score between "A" and "B" vectors.</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE StreamVerifier_Compare (FR_OBJ_HANDLE Verifier,FR_OBJ_HANDLE TemplateA,FR_OBJ_HANDLE TemplateB,float* score);

/// <summary>
/// Returns similarity score between inner multy Template and Template A.
/// </summary>
/// <param name="Verifier">[In] Template Verifier object handle</param>
/// <param name="TemplateA">[In] Biometric "A" Template object handle</param>
/// <param name="score">[Out] Similarity score between between inner multy Template and Template A.</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE StreamVerifier_CompareNext (FR_OBJ_HANDLE Verifier,FR_OBJ_HANDLE TemplateA,float* score);


/// <summary>
/// Creates inner multybiometric template for matching from Templates object set.
/// Each Verifier can have only one inner template.
/// </summary>
/// <param name="Verifier">[In] Template Verifier object handle</param>
/// <param name="Keys">[In] Pointer to Templates array</param>
/// <param name="KeyCount">[In] Keys count</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE StreamVerifier_SetPerson (FR_OBJ_HANDLE Verifier,FR_OBJ_HANDLE* Keys,int KeyCount);


//////////////////////////////////////////Extractor/////////////////////////////////////////////

/// <summary>
/// Creates biometric vector extractor object
/// </summary>
/// <param name="Extractor">[Out] Face Extractor object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Extractor_Create(FR_OBJ_HANDLE*  Extractor);

/// <summary>
/// Releases biometric vector extractor object
/// </summary>
/// <param name="Extractor">[In] Face Extractor object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Extractor_Destroy(FR_OBJ_HANDLE  Extractor);

/// <summary>
/// Extracts biometric template from user image with specified eyes coordinates.
/// </summary>
/// <param name="Extractor">[In] Face Extractor object handle</param>
/// <param name="Image">[In] Image object handle</param>
/// <param name="Eyes">[In] Eyes coordinates(see FACE_POINTS description)</param>
/// <param name="Template">[Out] Biometric template object handle</param>
/// <param name="UniqueID">[In] Unique identifier for created template</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Extractor_MakeTemplate_Custom(FR_OBJ_HANDLE Extractor,IITImage::IMG_OBJECT_HANDLE Image,FACE_POINTS* Eyes,FR_OBJ_HANDLE* Template,UINT32 UniqueID);
       

/// <summary>
/// Creates Multitemplate object
/// </summary>
/// <param name="Extractor">[In] Face Extractor object handle</param>
/// <param name="MultyTemplate">[Out] Multitemplate object handle</param>
/// <param name="Templates">[In] Template objects handles for multitemplate</param>
/// <param name="TemplateCnt">[In] Template objects handles count.</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE Extractor_MakeMultyTemplate(FR_OBJ_HANDLE Extractor,FR_OBJ_HANDLE* MultyTemplate,FR_OBJ_HANDLE* Templates,int TemplateCnt);


#ifdef __IIT_FACE_DETECTOR_H__

/// <summary>
/// Extracts biometric template from user image with face detector returned eyes coordinates.
/// </summary>
/// <param name="Extractor">[In] Face Extractor object handle</param>
/// <param name="Image">[In] Image object handle</param>
/// <param name="Face">[In] Face detected by Face Detector(see face detection SDK manual)</param>
/// <param name="Template">[Out] Biometric template object handle</param>
/// <param name="UniqueID">[In] Unique identifier for created template</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Extractor_MakeTemplate(FR_OBJ_HANDLE Extractor,IITImage::IMG_OBJECT_HANDLE Image,IITFD::DETECTED_ITEM* Face,FR_OBJ_HANDLE* Template,UINT32 UniqueID);
 
#endif // __IIT_FACE_DETECTOR_H__

//////////////////////////////////////////Identifier/////////////////////////////////////////////

/// <summary>
/// Creates biometric template search module object
/// </summary>
/// <param name="Identifier"> [Out] Face Identifier object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Identifier_Create(FR_OBJ_HANDLE*  Identifier);
 
/// <summary>
/// Releases biometric template extractor object
/// </summary>
/// <param name="Identifier"> [In] Face Identifier object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>

 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Identifier_Destroy(FR_OBJ_HANDLE  Identifier);

/// <summary>
/// Do the search in the Template Storage for specified biometric Template.
/// </summary>
/// <param name="Identifier">[In] Face Identifier object handle</param>
/// <param name="FaceTemplateStorage">[In] FaceTemplate Storage object handle</param>
/// <param name="Template">[In] Reference Template object handle</param>
/// <param name="ResultSize">[In/Out] Maximum result list size/real result list size</param>
/// <param name="ResultList">[Out] Result List object handle that contains identification results</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Identifier_Identify(FR_OBJ_HANDLE Identifier,FR_OBJ_HANDLE FaceTemplateStorage,FR_OBJ_HANDLE Template,int* ResultSize,IDENTIFICATION_RESULT** ResultList);

 
/// <summary>
/// Return Result List item.Provide access to results of identification.
/// </summary>
/// <param name="ResultList">[In] Result List object handle</param>
/// <param name="Index">[In] index of item</param>
/// <param name="Item">[Out] Result List item</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Identifier_GetListItemAt(IDENTIFICATION_RESULT* ResultList,int Index,IDENTIFICATION_RESULT* Item);

/// <summary>
/// Releses Result List object
/// </summary>
/// <param name="ResultList">[In] Result List object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  Identifier_FreeList(IDENTIFICATION_RESULT* ResultList);

 //////////////////////////////////////////TemplateStorage/////////////////////////////////////////////

/// <summary>
/// Creates biometric Template Storage object
/// </summary>
/// <param name="TemplateStorage">[Out] Template Storage object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  TemplateStorage_Create(FR_OBJ_HANDLE* TemplateStorage);
             
/// <summary>
/// Releases biometric Template Storage object
/// </summary>
/// <param name="TemplateStorage"> [In] Template Storage object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  TemplateStorage_Destroy(FR_OBJ_HANDLE TemplateStorage);

/// <summary>
/// Returns number of biometric templates in Template Storage object.
/// </summary>
/// <param name="TemplateStorage">[In] Template Storage object handle</param>
/// <returns>Number of biometric templates in Template Storage object</returns>
 FACE_REC_EXPORT int FACE_REC_CALLTYPE TemplateStorage_GetCount(FR_OBJ_HANDLE TemplateStorage);

/// <summary>
/// Inserts biometric template into Template Storage object
/// </summary>
/// <param name="TemplateStorage">[In] Template Storage object handle</param>
/// <param name="Template">[In] biometric Template object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE TemplateStorage_AddTemplate(FR_OBJ_HANDLE TemplateStorage,FR_OBJ_HANDLE Template);

 
/// <summary>
/// Removes biometric template with specified UniqueID.
/// </summary>
/// <param name="TemplateStorage">[In] Template Storage object handle</param>
/// <param name="UniqueID">[In]UniqueID</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE TemplateStorage_RemoveTemplate(FR_OBJ_HANDLE TemplateStorage,UINT32 UniqueID);

/// <summary>
/// Removes all templates from Template Storage.
/// </summary>
/// <param name="TemplateStorage">[In] Template Storage object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE TemplateStorage_Clear(FR_OBJ_HANDLE TemplateStorage);

 
/// <summary>
/// Returns a copy of biometric template object with specified UniqueID.Biometric template must be released by user.
/// </summary>
/// <param name="TemplateStorage">[In] Template Storage object handle</param>
/// <param name="Template">[Out] Biometric Template object handle</param>
/// <param name="UniqueID">[In]UniqueID</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE TemplateStorage_ExtractTemplate(FR_OBJ_HANDLE TemplateStorage,FR_OBJ_HANDLE* Template,UINT32 UniqueID);


//////////////////////////////////////////FaceTemplate/////////////////////////////////////////////

/// <summary>
/// Return UniqueID of biometric template.
/// </summary>
/// <param name="Template">[Out] Biometric Template object handle</param>
/// <returns>Function returns UniqueID of template</returns>
 FACE_REC_EXPORT unsigned int FACE_REC_CALLTYPE FaceTemplate_GetID(FR_OBJ_HANDLE Template);

/// <summary>
/// Set UniqueID for biometric template.
/// </summary>
/// <param name="Template">[Out] Biometric Template object handle</param>
/// <param name="UniqueID">[In]UniqueID</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE FaceTemplate_SetID(FR_OBJ_HANDLE Template, UINT32 UniqueID);

/// <summary>
/// Save biometric Template data to memory buffer.
/// </summary>
/// <param name="Template">[In] Biometric Template object handle</param>
/// <param name="Buffer">[In] Byte array buffer.Must be allocated on user side</param>
/// <param name="TemplateSize">[Out] Actual Template size in bytes</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE FaceTemplate_Export(FR_OBJ_HANDLE Template,unsigned char* Buffer,int* TemplateSize);

/// <summary>
/// Returns biometric Template quality.
/// </summary>
/// <param name="Template">[In] Biometric Template object handle</param>
 /// <returns>Function Returns biometric Template quality.</returns>

 FACE_REC_EXPORT double FACE_REC_CALLTYPE FaceTemplate_GetQuality(FR_OBJ_HANDLE Template);

/// <summary>
/// Imports biometric template from byte array.
/// </summary>
/// <param name="Template">[Out] Biometric Template object handle</param>
/// <param name="Buffer">[In] Byte array buffer</param>
/// <param name="TemplateSize">[In] Template size in bytes</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE FaceTemplate_Import(FR_OBJ_HANDLE* Template,unsigned char* Buffer,int TemplateSize);

/// <summary>
/// Releases Template object
/// </summary>
/// <param name="Template">[In] Biometric Template object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE FreeTemplate(FR_OBJ_HANDLE Template);


/// <summary>
/// Returns size in bytes of biometric Template object.
/// </summary>
/// <param name="Template">[In] Biometric Template object handle.If this parameter is NULL function returns actual size for this SDK.</param>
/// <param name="TemplateSize">[Out]Biometric Template object size in bytes</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
 FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE GetTemplateSize(FR_OBJ_HANDLE Template,int* TemplateSize);

 //////////////////////////////////////////StreamIdentifier/////////////////////////////////////////////

/// <summary>
/// Creates biometric StreamIdentifier object
/// </summary>
/// <param name="StreamIdentifier">[Out] StreamIdentifier object handle</param>
/// <param name="Storage">[In] FaceTemplate Storage object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>

FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  StreamIndentifier_Create(FR_OBJ_HANDLE* StreamIdentifier, FR_OBJ_HANDLE Storage);

/// <summary>
/// Releases biometric StreamIdentifier object
/// </summary>
/// <param name="StreamIdentifier"> [In] StreamIdentifier object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>

FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE   StreamIndentifier_Destroy(FR_OBJ_HANDLE StreamIdentifier);

/// <summary>
/// Returns results for current state
/// </summary>
/// <param name="StreamIdentifier">[In] StreamIdentifier object handle</param>
/// <param name="UniqueID">[Out] Best match UniqueID</param>
/// <param name="score">[Out] Best match score</param>
/// <returns></returns>

FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE   StreamIndentifier_GetResults(FR_OBJ_HANDLE StreamIdentifier, int* UniqueID, float* score);

/// <summary>
/// Processes next Template from stream.
/// </summary>
/// <param name="StreamIdentifier">[In] StreamIdentifier object handle</param>
/// <param name="Template">[In] Current Template object handle</param>
/// <param name="UniqueID">[Out] Best match UniqueID</param>
/// <param name="score">[Out] Best match score</param>
/// <returns></returns>

FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE  StreamIndentifier_IdentifyNext(FR_OBJ_HANDLE StreamIdentifier, FR_OBJ_HANDLE Template, int* UniqueID, float* score);

//////////////////////////////////////////FaceMultiemplate /////////////////////////////////////////////



/// <summary>
/// Imports biometric Multiemplate from byte array.
/// </summary>
/// <param name="Template">[Out] Biometric Template object handle</param>
/// <param name="Buffer">[In] Byte array buffer</param>
/// <param name="TemplateSize">[In] Template size in bytes</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE MultyTemplate_Import(FR_OBJ_HANDLE  Template, unsigned char* Buffer, int TemplateSize);

/// <summary>
/// Save biometric Multiemplate data to memory buffer.
/// </summary>
/// <param name="Template">[In] Biometric Template object handle</param>
/// <param name="Buffer">[In] Byte array buffer.Must be allocated on user side</param>
/// <param name="TemplateSize">[Out] Actual Template size in bytes</param>
/// <returns></returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE MultyTemplate_Export(FR_OBJ_HANDLE   Template,unsigned char* Buffer, int* TemplateSize);

/// <summary>
/// Returns size in bytes of biometric Multitemplate object.
/// </summary>
/// <param name="Template">[In] Biometric Template object handle.</param>
/// <param name="TemplateSize">[Out]Biometric Template object size in bytes</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE MultyTemplate_GetTemplateSize(FR_OBJ_HANDLE Template, int* TemplateSize);

/// <summary>
/// Releases Multitemplate object
/// </summary>
/// <param name="Template">[In] Biometric Template object handle</param>
/// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
FACE_REC_EXPORT FR_RESULT FACE_REC_CALLTYPE MultyTemplate_Free(FR_OBJ_HANDLE Template);


/// @} 
}
#endif // __IIT_FACE_RECOGNITION_H__
