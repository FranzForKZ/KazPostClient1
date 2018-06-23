using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using IITFaceDetector;
using IITImage;

namespace IITFaceRec
{
    public class FaceRec
    {
        /// <summary>
        /// Library initialization
        /// </summary>
        /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
        [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "InitLib")]
        public static extern FR_RESULT InitLib();

        /// <summary>
        /// Releases library resources
        /// </summary>
        /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
        [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "FreeLib")]
        public static extern FR_RESULT FreeLib();

    }

    /// <summary>
    /// FaceTemplateExtractor class extracts biometric templates from images.
    /// </summary>
        public class FaceTemplateExtractor
        {
            /// <summary>
            /// Creates biometric vector extractor object
            /// </summary>
            /// <param name="Extractor">[Out] Face Extractor object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Extractor_Create")]
            private static extern FR_RESULT Extractor_Create(ref IntPtr Extractor);

            /// <summary>
            /// Releases biometric vector extractor object
            /// </summary>
            /// <param name="Extractor">[In] Face Extractor object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Extractor_Destroy")]
            private static extern FR_RESULT Extractor_Destroy(IntPtr Extractor);

            /// <summary>
            /// Extracts biometric template from user image with specified eyes coordinates.
            /// </summary>
            /// <param name="Extractor">[In] Face Extractor object handle</param>
            /// <param name="Image">[In] ImageObject object handle</param>
            /// <param name="Eyes">[In] Eyes coordinates(see FACE_POINTS description)</param>
            /// <param name="Template">[Out] Biometric template object handle</param>
            /// <param name="UniqueID">[In] Unique identifier for created template</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Extractor_MakeTemplate_Custom")]
            private static extern FR_RESULT Extractor_MakeTemplate_Custom(IntPtr Extractor, IntPtr Image, ref FACE_POINTS Eyes, ref IntPtr Template, UInt32 UniqueID);

            /// <summary>
            /// Extracts biometric template from user image with face detector returned eyes coordinates.
            /// </summary>
            /// <param name="Extractor">[In] Face Extractor object handle</param>
            /// <param name="Image">[In] ImageObject object handle</param>
            /// <param name="Face">[In] Face detected by Face Detector(see face detection SDK manual)</param>
            /// <param name="Template">[Out] Biometric template object handle</param>
            /// <param name="UniqueID">[In] Unique identifier for created template</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Extractor_MakeTemplate")]
            private static extern FR_RESULT Extractor_MakeTemplate(IntPtr Extractor, IntPtr Image, ref DETECTED_ITEM Face, ref IntPtr Template,UInt32 UniqueID);

            /// <summary>
            /// Extracts Multitemplate object
            /// </summary>
            /// <param name="Extractor">[In] Face Extractor object handle</param>
            /// <param name="Template">[Out] Multitemplate object handle</param>
            /// <param name="Keys">[In] Template objects handles for multitemplate</param>
            /// <param name="KeysCnt">[In] Template objects handles count.</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Extractor_MakeMultyTemplate")]
            private static extern FR_RESULT Extractor_MakeMultyTemplate(IntPtr Extractor, ref IntPtr Template, IntPtr[] Keys, int KeysCnt);
            

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>       
            IntPtr IntObjPtr = IntPtr.Zero;


            /// <summary>
            /// Constructor.
            /// </summary>
            public FaceTemplateExtractor()
            {
                Extractor_Create(ref IntObjPtr);
            }

            /// <summary>
            /// Extracts biometric template from user image with face detector returned eyes coordinates.
            /// </summary>
            /// <param name="Image">[In] Bitmap object</param>
            /// <param name="Face">[In] Face detected by Face Detector(see face detection SDK manual)</param>
            /// <param name="UniqueID">[In] Unique identifier for created template</param>
            /// <returns>Returns Template object if success or null otherwise</returns>
            public Template Extract(Bitmap Image, DETECTED_ITEM Face,UInt32 UniqueID)
            {
                using (ImageObject ImageObject = ImageObject.Create_ImageObject_fData(Image))
                {
                    return Extract(ImageObject, Face, UniqueID);
                }
            }

            /// <summary>
            /// Extracts biometric template from user image in byte array representation with face detector returned eyes coordinates.
            /// </summary>
            /// <param name="Image">[In]ImageObject object</param>
            /// <param name="Face">[In] Face detected by Face Detector(see face detection SDK manual)</param>
            /// <param name="UniqueID">[In] Unique identifier for created template</param>
            /// <returns>Returns Template object if success or null otherwise</returns>
            public Template Extract(ImageObject ImageObject, DETECTED_ITEM Face, UInt32 UniqueID)
            {
                if (Math.Abs(Face.fYawAngle) > 0.6)
                    return null;
                IntPtr TemplatePtr = IntPtr.Zero;

                if (Extractor_MakeTemplate(IntObjPtr, ImageObject.InnerPtr, ref Face, ref TemplatePtr, UniqueID) == FR_RESULT.FR_RESULT_OK)
                {
                    if (TemplatePtr != IntPtr.Zero)
                    {
                        return new Template(TemplatePtr);
                    }
                }
                return null;
            }

            /// <summary>
            ///  Extracts biometric template from user image with specified eyes coordinates.
            /// </summary>
            /// <param name="Image">[In] Bitmap object</param>
            /// <param name="Eyes">[In] Eyes coordinates(see FACE_POINTS description)</param>
            /// <param name="UniqueID">[In] Unique identifier for created template</param>
            /// <returns>Returns Template object if success or null otherwise</returns>
            public Template Extract(Bitmap Image, FACE_POINTS Eyes, UInt32 UniqueID)
            {
                using (ImageObject ImageObject = ImageObject.Create_ImageObject_fData(Image))
                {
                    return Extract(ImageObject, Eyes, UniqueID);
                }
            }

            /// <summary>
            /// Extracts biometric template from user image in byte array representation with  specified eyes coordinates.
            /// </summary>
            /// <param name="Image">[In]ImageObject object</param>
            /// <param name="Eyes">[In] Eyes coordinates(see FACE_POINTS description)</param>
            /// <param name="UniqueID">[In] Unique identifier for created template</param>
            /// <returns>Returns Template object if success or null otherwise</returns>
            public Template Extract(ImageObject ImageObject, FACE_POINTS Eyes, UInt32 UniqueID)
            {               
                IntPtr TemplatePtr = IntPtr.Zero;

                if (Extractor_MakeTemplate_Custom(IntObjPtr, ImageObject.InnerPtr, ref Eyes, ref TemplatePtr, UniqueID) == FR_RESULT.FR_RESULT_OK)
                {
                    if (TemplatePtr != IntPtr.Zero)
                    {
                        return new Template(TemplatePtr);
                    }
                }
                return null;
            }

            /// <summary>
            /// Extracts biometric Multy Template, using array of Templates.
            /// </summary>
            /// <param name="IntObjPtrObjs">[IN] Array o templates.</param>
            /// <returns>Returns MultyTemplate class.</returns>
            public MultiTemplate ExtractMultiTemplate(IntPtr[] IntObjPtrObjs)
            {
                IntPtr MTemplatePtr = IntPtr.Zero;
                if (Extractor_MakeMultyTemplate(IntObjPtr, ref MTemplatePtr, IntObjPtrObjs, IntObjPtrObjs.Count()) == FR_RESULT.FR_RESULT_OK)
                {
                    if (MTemplatePtr != IntPtr.Zero)
                    {
                        return new MultiTemplate(MTemplatePtr);
                    }
                }
                return null;
            }

            /// <summary>
            /// Destructor.
            /// </summary>
            ~FaceTemplateExtractor()
            {
                if (IntObjPtr != IntPtr.Zero)
                    Extractor_Destroy(IntObjPtr);
            }

        }

    /// <summary>
    /// FaceIdentifier class needed to do seach in the Template Storage.
    /// </summary>
        public class FaceIdentifier
        {
            /// <summary>
            /// Creates biometric vector search module object
            /// </summary>
            /// <param name="Identifier"> [Out] Face Identifier object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Identifier_Create")]
            private static extern FR_RESULT Identifier_Create(ref IntPtr Identifier);

            /// <summary>
            /// Releases FaceIdentifier object
            /// </summary>
            /// <param name="Identifier"> [In] Face Identifier object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Identifier_Destroy")]
            private static extern FR_RESULT Identifier_Destroy(IntPtr Identifier);

            /// <summary>
            /// Do the search in the Template Storage for specified biometric Template.
            /// </summary>
            /// <param name="Identifier">[In] Face Identifier object handle</param>
            /// <param name="FaceTemplateStorage">[In] FaceTemplate Storage object handle</param>
            /// <param name="Template">[In] Reference Template object handle</param>
            /// <param name="ResultSize">[In/Out] Maximum result list size/real result list size</param>
            /// <param name="ResultList">[Out] Result List object handle that contains identification results</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Identifier_Identify")]
            private static extern FR_RESULT Identifier_Identify(IntPtr Identifier, IntPtr FaceTemplateStorage, IntPtr Template, ref int ResultSize, ref IntPtr ResultList);

            /// <summary>
            /// /// Do the search in the Template Storage for specified biometric Multitemplate.
            /// </summary>
            /// <param name="Identifier">[In] Face Identifier object handle</param>
            /// <param name="FaceTemplateStorage">[In] FaceTemplate Storage object handle</param>
            /// <param name="MultiTemplate">[In] Reference Multitemplate object handle</param>
            /// <param name="UniqueID">[Out] Best match UniqueID</param>
            /// <param name="score">[Out] Best match score</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Identifier_IdentifyMultiTemplate")]
            private static extern FR_RESULT Identifier_IdentifyMultiTemplate(IntPtr Identifier, IntPtr FaceTemplateStorage, IntPtr MultiTemplate, ref UInt32 UniqueID, ref float score);


            /// <summary>
            /// Return Result List item.Provide access to results of identification.
            /// </summary>
            /// <param name="ResultList">[In] Result List object handle</param>
            /// <param name="Index">[In] index of item</param>
            /// <param name="Item">[Out] Result List item</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Identifier_GetListItemAt")]
            private static extern FR_RESULT Identifier_GetListItemAt(IntPtr ResultList, int Index, ref IDENTIFICATION_RESULT Item);

            /// <summary>
            /// Releses Result List object
            /// </summary>
            /// <param name="ResultList">[In] Result List object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Identifier_FreeList")]
            private static extern FR_RESULT Identifier_FreeList(IntPtr ResultList);

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>       
            IntPtr IntObjPtr = IntPtr.Zero;

            /// <summary>
            /// Pointer to Result List object on DLL side.        
            /// </summary>
            IntPtr ResultList = IntPtr.Zero;

            /// <summary>
            /// Constructor.
            /// </summary>
            public FaceIdentifier()
            {
                Identifier_Create(ref IntObjPtr);
            }

            /// <summary>
            ///  Do the search in the Template Storage for specified biometric Template.
            /// </summary>
            /// <param name="TemplateStorage">[In] FaceTemplate Storage object</param>
            /// <param name="Template">[In] Reference Template object</param>
            /// <param name="ResultSize">[In] Maximum result list size.Default value 100</param>
            /// <returns>Method return list of mathing results.In case of errors method returns empty list</returns>
            public List<IDENTIFICATION_RESULT> Identify(TemplateStorage TemplateStorage, Template Template, int ResultSize = 100)
            {
                List<IDENTIFICATION_RESULT> Result = new List<IDENTIFICATION_RESULT>();

                if (Template == null) return Result;
                if (Identifier_Identify(IntObjPtr, TemplateStorage.InnerPtr, Template.InnerPtr, ref ResultSize, ref ResultList) == FR_RESULT.FR_RESULT_OK)
                {
                    if (ResultSize != 0)
                    {
                        for (int i = 0; i < ResultSize; i++)
                        {
                            IDENTIFICATION_RESULT temp = new IDENTIFICATION_RESULT();
                            Identifier_GetListItemAt(ResultList, i, ref temp);
                            Result.Add(temp);
                        }
                        Identifier_FreeList(ResultList);
                    }
                    ResultList = IntPtr.Zero;
                }
                return Result;
            }


            public FR_RESULT Identify(TemplateStorage TemplateStorage, MultiTemplate Template,out UInt32 UniqueID,out float score)
            {
                UniqueID = 0;
                score = -1;
                return Identifier_IdentifyMultiTemplate(IntObjPtr, TemplateStorage.InnerPtr, Template.InnerPtr, ref UniqueID, ref score);
            }
            /// <summary>
            /// Destructor.
            /// </summary>
            ~FaceIdentifier()
            {
                if (IntObjPtr != IntPtr.Zero)
                    Identifier_Destroy(IntObjPtr);
            }
        }

 
    /// <summary>
    /// TemplateStorage class represents inner storage object
    /// </summary>
        public class TemplateStorage
        {
            /// <summary>
            /// Creates biometric Template Storage object
            /// </summary>
            /// <param name="TemplateStorage">[Out] Template Storage object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "TemplateStorage_Create")]
            private static extern FR_RESULT TemplateStorage_Create(ref IntPtr TemplateStorage);

            /// <summary>
            /// Releases biometric Template Storage object
            /// </summary>
            /// <param name="TemplateStorage"> [In] Template Storage object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "TemplateStorage_Destroy")]
            private static extern FR_RESULT TemplateStorage_Destroy(IntPtr TemplateStorage);

            /// <summary>
            /// Inserts biometric template into Template Storage object
            /// </summary>
            /// <param name="TemplateStorage">[In] Template Storage object handle</param>
            /// <param name="Template">[In] biometric Template object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "TemplateStorage_AddTemplate")]
            private static extern FR_RESULT TemplateStorage_AddTemplate(IntPtr TemplateStorage, IntPtr Template);

            /// <summary>
            /// Removes biometric template with specified UniqueID.
            /// </summary>
            /// <param name="TemplateStorage">[In] Template Storage object handle</param>
            /// <param name="UniqueID">[In]UniqueID</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>

            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "TemplateStorage_RemoveTemplate")]
            private static extern FR_RESULT TemplateStorage_RemoveTemplate(IntPtr TemplateStorage, UInt32 UniqueID);

            /// <summary>
            /// Returns a copy of biometric template object with specified UniqueID.Biometric template must be released by user.
            /// </summary>
            /// <param name="TemplateStorage">[In] Template Storage object handle</param>
            /// <param name="Template">[Out] Biometric Template object handle</param>
            /// <param name="UniqueID">[In]UniqueID</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>

            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "TemplateStorage_ExtractTemplate")]
            private static extern FR_RESULT TemplateStorage_ExtractTemplate(IntPtr TemplateStorage, ref IntPtr Template, UInt32 UniqueID);

            /// <summary>
            /// Returns number of biometric templates in Template Storage object.
            /// </summary>
            /// <param name="TemplateStorage">[In] Template Storage object handle</param>
            /// <returns>Number of biometric templates in Template Storage object</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "TemplateStorage_GetCount")]
            private static extern int TemplateStorage_GetCount(IntPtr TemplateStorage);

            /// <summary>
            /// Removes all templates from Template Storage.
            /// </summary>
            /// <param name="TemplateStorage">[In] Template Storage object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "TemplateStorage_Clear")]
            private static extern FR_RESULT TemplateStorage_Clear(IntPtr TemplateStorage);

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>       
            IntPtr IntObjPtr = IntPtr.Zero;

            /// <summary>
            /// Template count
            /// </summary>
            public int TemplateCount { get { if (IntObjPtr != IntPtr.Zero)return TemplateStorage_GetCount(IntObjPtr); else return -1; } }

            /// <summary>
            /// Pointer to object on DLL side.
            /// </summary>
            public IntPtr InnerPtr { get { return IntObjPtr; } }

            /// <summary>
            /// Constructor.
            /// </summary>
            public TemplateStorage()
            {
                TemplateStorage_Create(ref IntObjPtr);
            }

            /// <summary>
            /// Adds biometric Template object to storage
            /// </summary>
            /// <param name="Template">[In] Template object</param>
            /// <returns>Returns standard error code(see FR_RESULT enum)</returns>
            public FR_RESULT AddTemplate(Template Template)
            {
                return TemplateStorage_AddTemplate(IntObjPtr, Template.InnerPtr);
            }

            /// <summary>
            /// Removes template with specified UniqueID from storage
            /// </summary>
            /// <param name="UniqueID">[In] UniqueID</param>
            /// <returns>Returns standard error code(see FR_RESULT enum)</returns>
            public FR_RESULT RemoveTemplate(UInt32 UniqueID)
            {
                return TemplateStorage_RemoveTemplate(IntObjPtr, UniqueID);
            }

            /// <summary>
            /// Removes all templates from storage.
            /// </summary>
            /// <returns>Returns standard error code(see FR_RESULT enum)</returns>
            public FR_RESULT Clear()
            {
                if (IntObjPtr != null)
                    return TemplateStorage_Clear(IntObjPtr);
                else
                    return FR_RESULT.FR_RESULT_POINTER;
            }

            /// <summary>
            /// Returns a template with specified UniqID from storage.
            /// </summary>
            /// <param name="UniqueID">[In] UniqueID</param>
            /// <returns>Returns Template object or null if template not found</returns>
            public Template ExtractTemplate(UInt32 UniqueID)
            {
                IntPtr TemplatePtr = IntPtr.Zero;
                FR_RESULT Result = TemplateStorage_ExtractTemplate(IntObjPtr, ref TemplatePtr, UniqueID);
                if (Result == FR_RESULT.FR_RESULT_OK)
                {
                    if (TemplatePtr != IntPtr.Zero)
                        return new Template(TemplatePtr);
                }                
                return null;
            }

            /// <summary>
            /// Destructor
            /// </summary>
            ~TemplateStorage()
            {
                if (IntObjPtr != IntPtr.Zero)
                    TemplateStorage_Destroy(IntObjPtr);
            }

        }

    /// <summary>
    /// Class represents inner biometric template object
    /// </summary>
        public class Template : IDisposable
        {
            /// <summary>
            /// Imports biometric template from byte array.
            /// </summary>
            /// <param name="Template">[Out] Biometric Template object handle</param>
            /// <param name="Buffer">[In] Byte array buffer</param>
            /// <param name="TemplateSize">[In] Template size in bytes</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTemplate_Import")]
            private static extern FR_RESULT FaceTemplate_Import(ref IntPtr Template, byte[] Buffer, int TemplateSize);

            /// <summary>
            /// Returns biometric Template quality.
            /// </summary>
            /// <param name="Template">[In] Biometric Template object handle</param>
            /// <returns>Function Returns biometric Template quality.</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTemplate_GetQuality")]
            private static extern double FaceTemplate_GetQuality(IntPtr Template);

            /// <summary>
            /// Return UniqueID of biometric template.
            /// </summary>
            /// <param name="Template">[Out] Biometric Template object handle</param>
            /// <returns>Function returns UniqueID of template</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTemplate_GetID")]
            private static extern UInt32 FaceTemplate_GetID(IntPtr Template);

            /// <summary>
            /// Set UniqueID for biometric template.
            /// </summary>
            /// <param name="Template">[Out] Biometric Template object handle</param>
            /// <param name="UniqueID">[In]UniqueID</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTemplate_SetID")]
            private static extern FR_RESULT FaceTemplate_SetID(IntPtr Template, UInt32 ID);

            /// <summary>
            /// Save biometric Template data to memory buffer.
            /// </summary>
            /// <param name="Template">[In] Biometric Template object handle</param>
            /// <param name="Buffer">[In] Byte array buffer.Must be allocated on user side</param>
            /// <param name="TemplateSize">[Out] Actual Template size in bytes</param>
            /// <returns></returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "FaceTemplate_Export")]
            private static extern FR_RESULT FaceTemplate_Export(IntPtr Template, byte[] Buffer, ref int TemplateSize);

            /// <summary>
            /// Returns size in bytes of biometric Template object.
            /// </summary>
            /// <param name="Template">[In] Biometric Template object handle.If this parameter is NULL function returns actual size for this SDK.</param>
            /// <param name="TemplateSize">[Out]Biometric Template object size in bytes</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "GetTemplateSize")]
            private static extern FR_RESULT GetTemplateSize(IntPtr Template, ref int TemplateSize);

            /// <summary>
            /// Releases Template object
            /// </summary>
            /// <param name="Template">[In] Biometric Template object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "FreeTemplate")]
            private static extern FR_RESULT FreeTemplate(IntPtr Template);

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>   
            IntPtr IntObjPtr = IntPtr.Zero;

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>               
            public IntPtr InnerPtr { get { return IntObjPtr; } }

            /// <summary>
            /// Constructor.Creates object from biometric Template object handle.
            /// </summary>      
            /// <param name="IntObjPtr">[In] Biometric Template object handle</param>
            public Template(IntPtr IntObjPtr)
            {
                this.IntObjPtr = IntObjPtr;
            }

            /// <summary>
            /// Constructor.Creates object from byte array
            /// </summary>      
            /// <param name="Data">[In] Byte array</param>
            public Template(byte[] Data)
            {
                if (FaceTemplate_Import(ref IntObjPtr, Data, Data.Count()) != FR_RESULT.FR_RESULT_OK)
                    IntObjPtr = IntPtr.Zero;
            }

            /// <summary>
            /// Constructor. Creates object from Image and FaceRect by Extractor
            /// </summary>
            public Template(Bitmap Image, DETECTED_ITEM DE, uint UID)
            {
                FaceTemplateExtractor FTE = new FaceTemplateExtractor();
                this.IntObjPtr = FTE.Extract(Image, DE, UID).InnerPtr;                
                
            }
            
            
            /// <summary>
            /// Releases mirror object
            /// </summary>


            public void Dispose()
            {
                if (IntObjPtr != IntPtr.Zero)
                {
                    FreeTemplate(IntObjPtr);
                    IntObjPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Destructor
            /// </summary>
            ~Template()
            {
                if (IntObjPtr != IntPtr.Zero)
                    FreeTemplate(IntObjPtr);
            }

            /// <summary>
            /// Returns true if template has real object on dll side
            /// </summary>
            /// <returns>Returns true if template has real object on dll side</returns>
            public bool IsValid() { return IntObjPtr == IntPtr.Zero; }

            /// <summary>
            /// Save Template data as byte array
            /// </summary>
            /// <returns>Byte array with biometric data</returns>
            public byte[] Export()
            {
                int TemplateSize = 0;
                GetTemplateSize(IntObjPtr, ref TemplateSize);
                byte[] Buffer = new byte[TemplateSize];
                if (FaceTemplate_Export(IntObjPtr, Buffer, ref TemplateSize) == FR_RESULT.FR_RESULT_OK)
                {
                    return Buffer;
                }
                else
                    return new byte[0];
            }

            /// <summary>
            /// Returns Template size in bytes.
            /// </summary>
            /// <returns>Returns Template size in bytes.</returns>
            public static int GetTemplateSize()
            {
                int TemplateSize = 0;
                GetTemplateSize(IntPtr.Zero, ref TemplateSize);
                return TemplateSize;
            }


            /// <summary>
            /// Return UniqueID of biometric template.
            /// </summary>
            public uint GetID()
            {
                if (IntObjPtr != IntPtr.Zero)
                    return FaceTemplate_GetID(IntObjPtr);
                else
                    return 0;
            }

            /// <summary>
            /// Set UniqueID for biometric template.
            /// </summary>
            /// <param name="ID">[In]UniqueID</param>          
            public void SetID(uint ID)
            {
                if (IntObjPtr != IntPtr.Zero)
                    FaceTemplate_SetID(IntObjPtr, ID);
            }

            /// <summary>
            /// Returns biometric Template quality.
            /// </summary>
            /// <returns>Function Returns biometric Template quality.</returns>
            public double Quality()
            {
                if (IntObjPtr != IntPtr.Zero)
                    return FaceTemplate_GetQuality(IntObjPtr);
                else
                    return -1;
            }

        }


        /// <summary>
        /// Class represents inner biometric multitemplate object
        /// </summary>
        public class MultiTemplate : IDisposable
        {
            /// <summary>
            /// Imports biometric template from byte array.
            /// </summary>
            /// <param name="Template">[Out] Biometric Template object handle</param>
            /// <param name="Buffer">[In] Byte array buffer</param>
            /// <param name="TemplateSize">[In] Template size in bytes</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "MultyTemplate_Import")]
            private static extern FR_RESULT MultyTemplate_Import(ref IntPtr Template, byte[] Buffer, int TemplateSize);

            /// <summary>
            /// Save biometric Template data to memory buffer.
            /// </summary>
            /// <param name="Template">[In] Biometric Template object handle</param>
            /// <param name="Buffer">[In] Byte array buffer.Must be allocated on user side</param>
            /// <param name="TemplateSize">[Out] Actual Template size in bytes</param>
            /// <returns></returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "MultyTemplate_Export")]
            private static extern FR_RESULT MultyTemplate_Export(IntPtr Template, byte[] Buffer, ref int TemplateSize);

            /// <summary>
            /// Returns size in bytes of biometric Multitemplate object.
            /// </summary>
            /// <param name="Template">[In] Biometric Template object handle.If this parameter is NULL function returns actual size for this SDK.</param>
            /// <param name="TemplateSize">[Out]Biometric Template object size in bytes</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "MultyTemplate_GetTemplateSize")]
            private static extern FR_RESULT MultyTemplate_GetTemplateSize(IntPtr Template, ref int TemplateSize);

            /// <summary>
            /// Creates Multitemplate object
            /// </summary>
            /// <param name="Template">[Out] Multitemplate object handle</param>
            /// <param name="Keys">[In] Template objects handles for multitemplate</param>
            /// <param name="KeysCnt">[In] Template objects handles count.</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "MultyTemplate_Create")]
            private static extern FR_RESULT MultyTemplate_Create(ref IntPtr Template,IntPtr[] Keys, int KeysCnt);

            /// <summary>
            /// Releases Multitemplate object
            /// </summary>
            /// <param name="Template">[In] Biometric Template object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "MultyTemplate_Free")]
            private static extern FR_RESULT MultyTemplate_Free(IntPtr Template);

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>   
            IntPtr IntObjPtr = IntPtr.Zero;

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>               
            public IntPtr InnerPtr { get { return IntObjPtr; } }

            /// <summary>
            /// Constructor.Creates object from biometric MutliTemplate object handle.
            /// </summary>      
            /// <param name="IntObjPtr">[In] Biometric MultiTemplate object handle</param>
            public MultiTemplate(IntPtr IntObjPtr)
            {
                this.IntObjPtr = IntObjPtr;
            }

            /// <summary>
            /// Constructor.Creates object from biometric Template object handle.
            /// </summary>      
            /// <param name="IntObjPtr">[In] Biometric Template object handle</param>
            public MultiTemplate(IntPtr[] IntObjPtrObjs)
            {
                MultyTemplate_Create(ref IntObjPtr, IntObjPtrObjs, IntObjPtrObjs.Count());
                //this.IntObjPtr = IntObjPtr;
            }

            /// <summary>
            /// Constructor.Creates object from byte array
            /// </summary>      
            /// <param name="Data">[In] Byte array</param>
            public MultiTemplate(byte[] Data)
            {
                if (MultyTemplate_Import(ref IntObjPtr, Data, Data.Count()) != FR_RESULT.FR_RESULT_OK)
                    IntObjPtr = IntPtr.Zero;
            }

            /// <summary>
            /// Releases mirror object
            /// </summary>


            public void Dispose()
            {
                if (IntObjPtr != IntPtr.Zero)
                {
                    MultyTemplate_Free(IntObjPtr);
                    IntObjPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Destructor
            /// </summary>
            ~MultiTemplate()
            {
                if (IntObjPtr != IntPtr.Zero)
                    MultyTemplate_Free(IntObjPtr);
            }

            /// <summary>
            /// Returns true if template has real object on dll side
            /// </summary>
            /// <returns>Returns true if template has real object on dll side</returns>
            public bool IsValid() { return IntObjPtr == IntPtr.Zero; }

            /// <summary>
            /// Save Template data as byte array
            /// </summary>
            /// <returns>Byte array with biometric data</returns>
            public byte[] Export()
            {
                int TemplateSize = 0;
                MultyTemplate_GetTemplateSize(IntObjPtr, ref TemplateSize);
                byte[] Buffer = new byte[TemplateSize];
                if (MultyTemplate_Export(IntObjPtr, Buffer, ref TemplateSize) == FR_RESULT.FR_RESULT_OK)
                {
                    return Buffer;
                }
                else
                    return new byte[0];
            }

            /// <summary>
            /// Returns Template size in bytes.
            /// </summary>
            /// <returns>Returns Template size in bytes.</returns>
            public int GetTemplateSize()
            {
                int TemplateSize = 0;
                if (IntObjPtr!=null)
                    MultyTemplate_GetTemplateSize(IntObjPtr, ref TemplateSize);
                return TemplateSize;
            }
        }


    /// <summary>
    /// Class provide verification functionality
    /// </summary>
        public class FaceVerifier
        {
            /// <summary>
            /// Creates biometric vector Verifier object
            /// </summary>
            /// <param name="Verifier">[Out] Template Verifier object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Verifier_Create")]
            private static extern FR_RESULT Verifier_Create(ref IntPtr Verifier);

            /// <summary>
            /// Releases biometric vector Verifier object
            /// </summary>
            /// <param name="Verifier"> [In] Template Verifier object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Verifier_Destroy")]
            private static extern FR_RESULT Verifier_Destroy(IntPtr Verifier);

            /// <summary>
            /// Returns similarity score of two biometric Template objects.
            /// </summary>
            /// <param name="Verifier">[In] Template Verifier object handle</param>
            /// <param name="TemplateA">[In] Biometric "A" Template object handle</param>
            /// <param name="TemplateB">[In] Biometric "B" Template object handle</param>
            /// <param name="score">[Out] Similarity score between "A" and "B" vectors.</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "Verifier_Compare")]
            private static extern FR_RESULT Verifier_Compare(IntPtr Verifier, IntPtr TemplateA, IntPtr TemplateB, ref float score);

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>   
            IntPtr IntObjPtr = IntPtr.Zero;

            /// <summary>
            /// Constructor
            /// </summary>
            public FaceVerifier()
            {
                Verifier_Create(ref IntObjPtr);
            }

            /// <summary>
            /// Returns similarity score of two biometric Template objects.
            /// </summary>
            /// <param name="Template1">Template1 object</param>
            /// <param name="Template2">Template2 object</param>
            /// <returns>Returns similarity score between Template1 and Template2.</returns>
            public float Compare(Template Template1, Template Template2)
            {
                float score = -1;
                if (Verifier_Compare(IntObjPtr, Template1.InnerPtr, Template2.InnerPtr, ref score) != FR_RESULT.FR_RESULT_OK) score = -1;
                return score;
            }

            /// <summary>
            /// Destructor.
            /// </summary>
            ~FaceVerifier()
            {
                if (IntObjPtr != IntPtr.Zero)
                    Verifier_Destroy(IntObjPtr);
            }
        }


        /// <summary>
        /// Class provide verification functionality for real-time applications
        /// Must be used only for working with video or image sequnces.
        /// </summary>
        public class StreamFaceVerifier
        {
            /// <summary>
            /// Creates biometric vector StreamVerifier object
            /// </summary>
            /// <param name="Verifier">[Out] Template Verifier object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamVerifier_Create")]
            private static extern FR_RESULT StreamVerifier_Create(ref IntPtr Verifier);

            /// <summary>
            /// Releases biometric vector Verifier object
            /// </summary>
            /// <param name="Verifier"> [In] Template Verifier object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamVerifier_Destroy")]
            private static extern FR_RESULT StreamVerifier_Destroy(IntPtr Verifier);

            /// <summary>
            /// Returns similarity score of two biometric Template objects.
            /// </summary>
            /// <param name="Verifier">[In] Template Verifier object handle</param>
            /// <param name="TemplateA">[In] Biometric "A" Template object handle</param>
            /// <param name="TemplateB">[In] Biometric "B" Template object handle</param>
            /// <param name="score">[Out] Similarity score between "A" and "B" vectors.</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamVerifier_Compare")]
            private static extern FR_RESULT StreamVerifier_Compare(IntPtr Verifier, IntPtr TemplateA, IntPtr TemplateB, ref float score);

            /// <summary>
            /// Returns similarity score between inner multy Template and Template A.
            /// </summary>
            /// <param name="Verifier">[In] Template Verifier object handle</param>
            /// <param name="TemplateA">[In] Biometric "A" Template object handle</param>
            /// <param name="score">[Out] Similarity score between between inner multy Template and Template A.</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamVerifier_CompareNext")]
            private static extern FR_RESULT StreamVerifier_CompareNext(IntPtr Verifier, IntPtr TemplateA, ref float score);

            /// <summary>
            /// Creates inner multybiometric template for matching from Templates object set.
            /// Each Verifier can have only one inner template.
            /// </summary>
            /// <param name="Verifier">[In] Template Verifier object handle</param>
            /// <param name="Keys">[In] Pointer to Templates array</param>
            /// <param name="KeyCount">[In] Keys count</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamVerifier_SetPerson")]
            private static extern FR_RESULT StreamVerifier_SetPerson(IntPtr Verifier, IntPtr[] Keys, int KeyCount);

            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>   
            IntPtr IntObjPtr = IntPtr.Zero;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="Templates"> Array of templates to create inner multy-template </param>
            public StreamFaceVerifier(Template[] Templates)
            {
                StreamVerifier_Create(ref IntObjPtr);
                IntPtr[] Keys = new IntPtr[Templates.Count()];
                for (int i = 0; i < Keys.Count(); i++)
                {
                    Keys[i] = Templates[i].InnerPtr;
                }
                StreamVerifier_SetPerson(IntObjPtr, Keys, Keys.Count());
            }

            /// <summary>
            /// Returns similarity score of two biometric Template objects.
            /// </summary>
            /// <param name="Template1">Template1 object</param>
            /// <param name="Template2">Template2 object</param>
            /// <returns>Returns similarity score between Template1 and Template2.</returns>
            public float Compare(Template Template1, Template Template2)
            {
                float score = -1;
                if (StreamVerifier_Compare(IntObjPtr, Template1.InnerPtr, Template2.InnerPtr, ref score) != FR_RESULT.FR_RESULT_OK) score = -1;
                return score;
            }

            /// <summary>
            /// Returns similarity score between inner Template and TemplateA.
            /// </summary>
            /// <param name="Template">TemplateA object</param>
            /// <returns>Returns similarity score between inner Template and TemplateA.</returns>
            public float CompareNext(Template TemplateA)
            {
                float score = -1;
                if (StreamVerifier_CompareNext(IntObjPtr, TemplateA.InnerPtr, ref score) != FR_RESULT.FR_RESULT_OK) score = -1;
                return score;
            }

            /// <summary>
            /// Destructor.
            /// </summary>
            ~StreamFaceVerifier()
            {
                if (IntObjPtr != IntPtr.Zero)
                    StreamVerifier_Destroy(IntObjPtr);
            }
        }



        /// <summary>
        /// Class provide identification functionality for real-time applications
        /// Must be used only for working with video or image sequnces.
        /// </summary>
        public class StreamFaceIdentifier
        {

            /// <summary>
            /// Creates biometric StreamIdentifier object
            /// </summary>
            /// <param name="StreamIdentifier">[Out] StreamIdentifier object handle</param>
            /// <param name="Storage">[In] FaceTemplate Storage object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamIndentifier_Create")]
            private static extern FR_RESULT StreamIndentifier_Create(ref IntPtr StreamIdentifier, IntPtr Storage);

            /// <summary>
            /// Releases biometric StreamIdentifier object
            /// </summary>
            /// <param name="StreamIdentifier"> [In] StreamIdentifier object handle</param>
            /// <returns>Function returns standard error code(see FR_RESULT enum)</returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamIndentifier_Destroy")]
            private static extern FR_RESULT StreamIndentifier_Destroy(IntPtr StreamIdentifier);

            /// <summary>
            /// Returns results for current state
            /// </summary>
            /// <param name="StreamIdentifier">[In] StreamIdentifier object handle</param>
            /// <param name="UniqueID">[Out] Best match UniqueID</param>
            /// <param name="score">[Out] Best match score</param>
            /// <returns></returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamIndentifier_GetResults")]
            private static extern FR_RESULT StreamIndentifier_GetResults(IntPtr StreamIdentifier, ref int UniqueID, ref float score);

            /// <summary>
            /// Processes next Template from stream.
            /// </summary>
            /// <param name="StreamIdentifier">[In] StreamIdentifier object handle</param>
            /// <param name="Template">[In] Current Template object handle</param>
            /// <param name="UniqueID">[Out] Best match UniqueID</param>
            /// <param name="score">[Out] Best match score</param>
            /// <returns></returns>
            [DllImport("IITFR.dll", CharSet = CharSet.Unicode, EntryPoint = "StreamIndentifier_IdentifyNext")]
            private static extern FR_RESULT StreamIndentifier_IdentifyNext(IntPtr StreamIdentifier, IntPtr Template, ref int UniqueID, ref float score);


            /// <summary>
            /// Pointer to object on DLL side.        
            /// </summary>   
            IntPtr IntObjPtr = IntPtr.Zero;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="Templates"> Array of templates to create inner multy-template </param>
            public StreamFaceIdentifier(TemplateStorage TS)
            {
                StreamIndentifier_Create(ref IntObjPtr, TS.InnerPtr);
            }

            /// <summary>
            /// Returns similarity score of two biometric Template objects.
            /// </summary>
            /// <param name="Template1">Template1 object</param>
            /// <param name="Template2">Template2 object</param>
            /// <returns>Returns similarity score between Template1 and Template2.</returns>
            public void StreamIndentifier_GetResults(out float score, out int IParam)
            {
                score = -1;
                IParam = -1;

                if (StreamIndentifier_GetResults(IntObjPtr, ref IParam, ref score) != FR_RESULT.FR_RESULT_OK)
                {
                    score = -1;
                    IParam = -1;
                }
            }

            /// <summary>
            /// Returns similarity score between inner Template and TemplateA.
            /// </summary>
            /// <param name="Template">TemplateA object</param>
            /// <returns>Returns similarity score between inner Template and TemplateA.</returns>
            public void IdentifyNext(Template TemplateA, out float score, out int IParam)
            {
                score = -1;
                IParam = -1;

                if (StreamIndentifier_IdentifyNext(IntObjPtr, TemplateA.InnerPtr, ref IParam, ref score) != FR_RESULT.FR_RESULT_OK)
                {
                    score = -1;
                    IParam = -1;
                }
            }

            /// <summary>
            /// Destructor.
            /// </summary>
            ~StreamFaceIdentifier()
            {
                if (IntObjPtr != IntPtr.Zero)
                    StreamIndentifier_Destroy(IntObjPtr);
            }
        }
    

}
