using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace PostUserActivity.HW.WIALib
{

    /// <summary>
    /// Contains TIFF image data.
    /// </summary>
    public class WiaResult
    {

        #region Private Members

        private int _currentFrameNumber;
        private string _fileName;
        private ObservableCollection<BitmapFrame> _bitmapFrames;

        #endregion

        #region Properties

        /// <summary>
        /// A list of binary arrays that make up the raw data used to load up the BitmapFrames.
        /// </summary>
        public List<byte[]> BinaryFrames { get; private set; }

        /// <summary>
        /// Gets the current bitmap frame. The return value may be null.
        /// </summary>
        public BitmapFrame CurrentFrame
        {
            get
            {
                if (_bitmapFrames != null
                    && _currentFrameNumber < _bitmapFrames.Count)
                {
                    return _bitmapFrames[_currentFrameNumber];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the current frame number.
        /// </summary>
        public int CurrentFrameNumber
        {
            get { return _currentFrameNumber; }
            private set
            {
                if (_bitmapFrames != null && value >= 0 && value < _bitmapFrames.Count)
                {
                    _currentFrameNumber = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName
        {
            get { return _fileName ?? (_fileName = String.Empty); }
            set { _fileName = value; }
        }

        /// <summary>
        /// Gets the frames count.
        /// </summary>
        public int FrameCount
        {
            get { return (_bitmapFrames != null) ? _bitmapFrames.Count : 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<BitmapFrame> Frames
        {
            get { return _bitmapFrames; }
            private set { _bitmapFrames = value; }
        }

        /// <summary>
        /// Gets the image page number.
        /// </summary>
        public int ImagePage
        {
            get { return CurrentFrameNumber + 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public BitmapFrame this[int i]
        {
            get
            {
                if (i >= 0 && i < _bitmapFrames.Count)
                    return _bitmapFrames[i];

                return null;
            }
        }

        public string LastError { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public WiaResult()
        {
            _currentFrameNumber = 0;
            this.Frames = null;
            this.BinaryFrames = new List<byte[]>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frames"></param>
        public WiaResult(List<byte[]> frames)
        {
            _currentFrameNumber = 0;
            this.Frames = new ObservableCollection<BitmapFrame>();
            this.BinaryFrames = frames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frames"></param>
        public WiaResult(ObservableCollection<BitmapFrame> frames)
        {
            _currentFrameNumber = 0;
            this.Frames = frames;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Moves to the next frame.
        /// </summary>
        /// <returns>Returns true if the operation was successful, othewise returns false.</returns>
        public bool MoveNext()
        {
            if (_bitmapFrames != null && _currentFrameNumber < (_bitmapFrames.Count - 1))
            {
                CurrentFrameNumber = _currentFrameNumber + 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves to the previous frame.
        /// </summary>
        /// <returns>Returns true if the operation was successful, othewise returns false.</returns>
        public bool MoveBack()
        {
            if (_bitmapFrames != null && _currentFrameNumber > 0)
            {
                CurrentFrameNumber = _currentFrameNumber - 1;
                return true;
            }
            return false;
        }

        #region LoadBitmapFrames

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useBlankDetection"></param>
        /// <returns>A boolean indicating whether or not the file was loaded properly.</returns>
        public bool LoadBitmapFrames(bool useBlankDetection = false)
        {
            foreach (byte[] bytes in this.BinaryFrames.Where(bytes => bytes != null))
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var frame = BitmapFrame.Create(ms);
                    if (useBlankDetection && frame.ToBitmap().IsBlank())
                        continue;

                    _bitmapFrames.Add(frame);
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Opens image file.
        /// </summary>
        /// <param name="fileName">Specifies the image file.</param>
        /// <returns>True, if succeeded; otherwise - false.</returns>
        public bool Open(string fileName)
        {

            bool result = false;
            FileName = fileName;

            try
            {
                using (Stream imageStreamSource = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    result = Open(imageStreamSource);
            }
            catch (FileNotFoundException)
            {
                LastError = "Can't open file " + fileName;
            }
            catch (FileFormatException excp)
            {
                LastError = String.Format("Can't open file {0}: {1}", fileName, excp.Message);
            }

            return result;
        }

        /// <summary>
        /// Opens image file.
        /// </summary>
        /// <param name="stream">Specifies the image file.</param>
        /// <returns>True, if succeeded; otherwise - false.</returns>
        public bool Open(Stream stream)
        {
            try
            {
                using (stream)
                {
                    TiffBitmapDecoder tiffDecoder = new TiffBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    //we need to break out the frames now because they can only be accessed on the same thread we do the decode on.
                    _bitmapFrames = new ObservableCollection<BitmapFrame>(tiffDecoder.Frames);

                    //_currentFrameNumber = 0;
                    CurrentFrameNumber = 0;
                    LastError = null;
                    return true;
                }
            }

            catch (Exception ex)
            {
                LastError = String.Format("Can't open stream: {0}", ex.Message);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compression"></param>
        public void Save(TiffCompressOption compression)
        {
            using (FileStream fs = new FileStream(this.FileName, FileMode.Create))
                this.Save(fs, compression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compression"></param>
        public void Save(Stream fs, TiffCompressOption compression)
        {
            TiffBitmapEncoder encoder = new TiffBitmapEncoder { Compression = compression };
            foreach (var bitmap in _bitmapFrames)
                encoder.Frames.Add(bitmap);
            encoder.Save(fs);
        }


        /// <summary>
        /// Sets the 1st frame as current frame.
        /// </summary>
        /// <returns>True, if succeeded; otherwise - false.</returns>
        public bool SetFirstFrame()
        {
            if (FrameCount > 0)
            {
                CurrentFrameNumber = 0;
                return true;
            }

            return false;
        }

        #endregion Methods

    }
}
