using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;
using WIA;

namespace PostUserActivity.HW.WIALib
{
    public static class WiaExtensions
    {

        // copies of constants that are found in WiaDef.h 

        public const int FACILITY_WIA = 33;

        public const int WIA_ERROR_GENERAL_ERROR = 1;
        public const int WIA_ERROR_PAPER_JAM = 2;
        public const int WIA_ERROR_PAPER_EMPTY = 3;
        public const int WIA_ERROR_PAPER_PROBLEM = 4;
        public const int WIA_ERROR_OFFLINE = 5;
        public const int WIA_ERROR_BUSY = 6;
        public const int WIA_ERROR_WARMING_UP = 7;
        public const int WIA_ERROR_USER_INTERVENTION = 8;
        public const int WIA_ERROR_ITEM_DELETED = 9;
        public const int WIA_ERROR_DEVICE_COMMUNICATION = 10;
        public const int WIA_ERROR_INVALID_COMMAND = 11;
        public const int WIA_ERROR_INCORRECT_HARDWARE_SETTING = 12;
        public const int WIA_ERROR_DEVICE_LOCKED = 13;
        public const int WIA_ERROR_EXCEPTION_IN_DRIVER = 14;
        public const int WIA_ERROR_INVALID_DRIVER_RESPONSE = 15;
        public const int WIA_ERROR_COVER_OPEN = 16;
        public const int WIA_ERROR_LAMP_OFF = 17;
        public const int WIA_ERROR_DESTINATION = 18;
        public const int WIA_ERROR_NETWORK_RESERVATION_FAILED = 19;
        public const int WIA_STATUS_END_OF_MEDIA = 1;

        // Definitions for errors and status codes passed to IWiaDataTransfer::BandedDataCallback as the lReason parameter.
        // These codes are in addition to the errors defined above; in some cases the SEVERITY_SUCCESS version of
        // an error is meant to replace the SEVERITY_ERROR version listed above.
        public const int WIA_STATUS_WARMING_UP = 2;
        public const int WIA_STATUS_CALIBRATING = 3;
        public const int WIA_STATUS_RESERVING_NETWORK_DEVICE = 6;
        public const int WIA_STATUS_NETWORK_DEVICE_RESERVED = 7;
        public const int WIA_STATUS_CLEAR = 8;
        public const int WIA_STATUS_SKIP_ITEM = 9;
        public const int WIA_STATUS_NOT_HANDLED = 10;


        // The value is returned by Scansetting.dll when the user chooses to change the scanner in scandialog
        public const int WIA_S_CHANGE_DEVICE = 11;

        // SelectDeviceDlg and SelectDeviceDlgID status code when there are no devices available
        public const int WIA_S_NO_DEVICE_AVAILABLE = 21;


        public const int WIA_COMPRESSION_JPEG = 5;

        public const int WIA_PROPERTY_CurrentIntent = 6146;
        public const int WIA_PROPERTY_HorizontalResolution = 6147;
        public const int WIA_PROPERTY_VerticalResolution = 6148;
        public const int WIA_PROPERTY_HorizontalExtent = 6151;
        public const int WIA_PROPERTY_VerticalExtent = 6152;
        public const int WIA_PROPERTY_DocumentHandlingSelect = 3088;
        public const int WIA_PROPERTY_DocumentHandlingStatus = 3087;

        public const int WIA_PROPERTY_BitsPerPixel = 4104;
        public const int WIA_PROPERTY_Format = 4106;
        public const int WIA_PROPERTY_Compression = 4107;

        public const int WIA_PROPERTY_VALUE_Color = 1;
        public const int WIA_PROPERTY_VALUE_Gray = 2;
        public const int WIA_PROPERTY_VALUE_BlackAndWhite = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static string GetErrorCodeDescription(int errorCode)
        {
            string desc = null;

            switch (errorCode)
            {
                case (WIA_ERROR_GENERAL_ERROR):
                    desc = "A general error occurred";
                    break;
                case (WIA_ERROR_PAPER_JAM):
                    desc = "There is a paper jam";
                    break;
                case (WIA_ERROR_PAPER_EMPTY):
                    desc = "The feeder tray is empty";
                    break;
                case (WIA_ERROR_PAPER_PROBLEM):
                    desc = "There is a problem with the paper";
                    break;
                case (WIA_ERROR_OFFLINE):
                    desc = "The scanner is offline";
                    break;
                case (WIA_ERROR_BUSY):
                    desc = "The scanner is busy";
                    break;
                case (WIA_ERROR_WARMING_UP):
                    desc = "The scanner is warming up";
                    break;
                case (WIA_ERROR_USER_INTERVENTION):
                    desc = "The scanner requires user intervention";
                    break;
                case (WIA_ERROR_ITEM_DELETED):
                    desc = "An unknown error occurred";
                    break;
                case (WIA_ERROR_DEVICE_COMMUNICATION):
                    desc = "An error occurred attempting to communicate with the scanner";
                    break;
                case (WIA_ERROR_INVALID_COMMAND):
                    desc = "The scanner does not understand this command";
                    break;
                case (WIA_ERROR_INCORRECT_HARDWARE_SETTING):
                    desc = "The scanner has an incorrect hardware setting";
                    break;
                case (WIA_ERROR_DEVICE_LOCKED):
                    desc = "The scanner device is in use by another application";
                    break;
                case (WIA_ERROR_EXCEPTION_IN_DRIVER):
                    desc = "The scanner driver reported an error";
                    break;
                case (WIA_ERROR_INVALID_DRIVER_RESPONSE):
                    desc = "The scanner driver gave an invalid response";
                    break;
                default:
                    desc = "An unknown error occurred";
                    break;
            }

            return desc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cx"></param>
        /// <returns></returns>
        public static int GetWiaErrorCode(this COMException cx)
        {
            int origErrorMsg = cx.ErrorCode;
            int errorCode = origErrorMsg & 0xFFFF;
            int errorFacility = ((origErrorMsg) >> 16) & 0x1fff;
            if (errorFacility == FACILITY_WIA)
                return errorCode;
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBag"></param>
        /// <param name="propID"></param>
        /// <param name="propValue"></param>
        public static void SetProperty(this WIA.Properties searchBag, int propID, object propValue)
        {
            foreach (Property prop in searchBag.Cast<Property>().Where(prop => prop.PropertyID == propID))
            {
                prop.set_Value(ref propValue);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBag"></param>
        /// <param name="propID"></param>
        /// <returns></returns>
        public static object GetProperty(this WIA.Properties searchBag, int propID)
        {
            return (from Property prop in searchBag where prop.PropertyID == propID select prop.get_Value()).FirstOrDefault<object>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        public static byte[] ToByte(this ImageFile imageFile)
        {
            if (imageFile == null) return null;

            byte[] bytes = null;
            Vector vector = imageFile.FileData;
            if (vector != null)
                bytes = vector.get_BinaryData() as byte[];

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="inMemory">A boolean specifying whether to do the conversion in-memory. For larger files, specify False.</param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource(this ImageFile imageFile, bool inMemory = true)
        {
            if (imageFile == null) return null;
            BitmapSource result = null;

            if (inMemory)
            {
                Vector vector = imageFile.FileData;
                if (vector != null)
                {
                    var bytes = vector.get_BinaryData() as byte[];

                    if (bytes != null)
                    {
                        var ms = new MemoryStream(bytes);
                        result = BitmapFrame.Create(ms);
                    }
                }
            }
            else
            {
                string fileName = Path.GetTempFileName();
                //RWM: Delete the file if it already exists (not guaranteed to be unique).
                File.Delete(fileName);
                imageFile.SaveFile(fileName);

                // load the file back in to a WPF type, this is just 
                // to get around size issues with large scans
                using (FileStream stream = File.OpenRead(fileName))
                {
                    result = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    //stream.Close();
                }

                // clean up
                File.Delete(fileName);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap ToBitmap(this ImageFile imageFile)
        {
            System.Drawing.Bitmap bitmap = null;

            Vector vector = imageFile.FileData;
            if (vector != null)
            {
                var bytes = vector.get_BinaryData() as byte[];

                if (bytes != null)
                {
                    using (var ms = new MemoryStream(bytes))
                    {
                        var encoder = new BmpBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(ms));
                        using (MemoryStream outStream = new MemoryStream())
                        {
                            encoder.Save(outStream);
                            bitmap = new System.Drawing.Bitmap(outStream);
                        }
                    }
                }

            }
            return bitmap;
        }

    }
}
