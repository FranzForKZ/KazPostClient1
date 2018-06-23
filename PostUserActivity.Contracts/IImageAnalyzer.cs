using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Contracts
{
    /// <summary>
    /// интерфейс питоновского анализатора изображений
    /// </summary>
    public interface IImageAnalyzer
    {
        void Analyze(ImageChangedEventArgs imgArgs, DeviceType device);
        event EventHandler<AnalyzeCompletedEventArgs> AnalyzeCompleted;
        event EventHandler<AnalyzeCompletedEventArgs> AnalyzeImage;
        
    }

    /// <summary>
    /// аргументы которые передаются анализатором, после обработки изображения
    /// </summary>
    public class AnalyzeCompletedEventArgs: EventArgs
    {
        public AnalyzeCompletedEventArgs(DeviceType device, List<AnalyzeImageResultType> errors)
        {
            this.SourceDevice = device;
            ErrorsList = errors;
        }

        public AnalyzeCompletedEventArgs(DeviceType device, List<AnalyzeImageResultType> errors, Image fullImg, Image previewImg)
        {
            ErrorsList = (errors == null ? new List<AnalyzeImageResultType>() : errors.Distinct().ToList());
            FullImage = fullImg;
            PreviewImage = previewImg;
            this.SourceDevice = device;
        }

        public DeviceType SourceDevice { get; private set; }

        public List<AnalyzeImageResultType> ErrorsList { get; private set; }

        public Image FullImage { get; private set; }
        public Image PreviewImage { get; private set; }

        public bool IsSuccess()
        {
            //if (ErrorsList == null || !ErrorsList.Any() || (FullImage!= null && PreviewImage!=null))
            if (FullImage != null && PreviewImage != null)
            {
                return true;
            }
            return false;
        }


        public string Message { get; set; }
    }

}

