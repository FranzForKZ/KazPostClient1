using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using NLog;
namespace CommonLib
{

    public static class FileHelper
    {
        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;
         static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private static bool IsFileLocked(Exception exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION;
        }

        internal static bool CanReadFile(string filePath)
        {
            //Try-Catch so we dont crash the program and can check the exception
            try
            {
                //The "using" is important because FileStream implements IDisposable and
                //"using" will avoid a heap exhaustion situation when too many handles  
                //are left undisposed.
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    if (fileStream != null) fileStream.Close(); //This line is me being overly cautious, fileStream will never be null unless an exception occurs... and I know the "using" does it but its helpful to be explicit - especially when we encounter errors - at least for me anyway!
                }
            }
            catch (IOException ex)
            {
                //THE FUNKY MAGIC - TO SEE IF THIS FILE REALLY IS LOCKED!!!
                if (IsFileLocked(ex))
                {
                    // do something, eg File.Copy or present the user with a MsgBox - I do not recommend Killing the process that is locking the file
                    return false;
                }
            }
            finally
            {
            }
            return true;
        }

        public static bool IsFileReady(string sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    
                    //if (inputStream.Length > 0)
                    //{
                        return true;
                    //}
                    //else
                    //{
                    //    return false;
                    //}
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool ReadFile(string fileName, out string fileContent)
                //check here why did it fail and ask user to retry if the file is in use.
        {
            fileContent = "";
            try
            {
                while(!IsFileReady(fileName))
	            {
                    System.Windows.Forms.Application.DoEvents();
	            }
                fileContent = File.ReadAllText(fileName);
                //File.Delete(fileName);
                return true;     
            }
            catch(Exception ex)
            {
                logger.Error("Cant read file, because of  : " + ex);                
                return false;
            }
        }
        /// <summary>
        /// Создать архив
        /// </summary>
        /// <param name="InputFilePath">Входной файл</param>
        /// <param name="OutPutFilePath">Выходной архив с одним файлом</param>
        public static void CreateZip(string InputFilePath, string OutPutFilePath)
        {
            FileInfo outFileInfo = new FileInfo(OutPutFilePath);
            FileInfo inFileInfo = new FileInfo(InputFilePath);

            // Create the output directory if it does not exist
            if (!Directory.Exists(outFileInfo.Directory.FullName))
            {
                Directory.CreateDirectory(outFileInfo.Directory.FullName);
            }

            // Compress
            using (FileStream fsOut = File.Create(OutPutFilePath))
            {
                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fsOut))
                {
                    zipStream.UseZip64 = UseZip64.Off;
                    zipStream.SetLevel(9);

                    ICSharpCode.SharpZipLib.Zip.ZipEntry newEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(inFileInfo.Name);
                    newEntry.DateTime = DateTime.UtcNow;
                    zipStream.PutNextEntry(newEntry);

                    byte[] buffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(InputFilePath))
                    {
                        ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(streamReader, zipStream, buffer);
                    }

                    zipStream.CloseEntry();
                    zipStream.IsStreamOwner = true;
                    zipStream.Close();
                }
            }
        }
    }

}
