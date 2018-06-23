using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.HWContracts;
using Emgu.CV;
using Emgu.CV.Structure;
using System.ComponentModel;

namespace PostUserActivity.Python
{
    /// <summary>
    /// предположим, что тут будет анализатор картинок
    /// </summary>
    public class ImageAnalyzerO : IImageAnalyzer
    {

        Image<Bgr, Byte> currentFrame; //current image aquired from webcam for display
        Image<Gray, byte> result, TrainedFace = null; //used to store the result image and trained face
        Image<Gray, byte> gray_frame = null; //grayscale current image aquired from webcam for processing

        int _counter = 0;
        //bool _isStopped = false;
        System.ComponentModel.BackgroundWorker bwImageAnalizer;
        bool isFinalImageFound = false;

        public ImageAnalyzerO()
        {
            bwImageAnalizer = new BackgroundWorker();
            bwImageAnalizer.DoWork += bwImageAnalizer_DoWork;
            bwImageAnalizer.RunWorkerCompleted += bwImageAnalizer_RunWorkerCompleted;
            bwImageAnalizer.ProgressChanged += bwImageAnalizer_ProgressChanged; 
        }

        void bwImageAnalizer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void bwImageAnalizer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            ImageProcessResult pythonResult = (ImageProcessResult)e.Result;
            List<AnalyzeImageResultType> errors = new List<AnalyzeImageResultType>();
           
            if (pythonResult.isGood)
            {
                Console.WriteLine("*************************** FOUND!");
                isFinalImageFound = true;
            }
            else
            {
                Console.WriteLine("VERY BAD FACE DETECTED !");

                for (int i = 0; i < pythonResult.ProblemList.Count(); i++)
                {
                    errors.Add((AnalyzeImageResultType)pythonResult.ProblemList[i]);

                    Console.WriteLine("Problems: " + pythonResult.ProblemList[i].ToString());
                }
            }

            if (pythonResult.isGood==true)
                AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(DeviceType.WebCam, errors, pythonResult.RGBImage, pythonResult.croppedImage));
            else
                AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(DeviceType.WebCam, errors, null, null));
        }

        void bwImageAnalizer_DoWork(object sender, DoWorkEventArgs e)
        {
            Image<Bgr, Byte> originalFrame = (Image<Bgr, Byte>)e.Argument;
            ImageAnalyzer ia = new ImageAnalyzer();
            ImageProcessResult pythonResult = ia.Analyze((Image)originalFrame.Bitmap);
            e.Result = pythonResult;
        }

        private void FrameGrabber_Parrellel(Image inputImage)
        {
            if (isFinalImageFound == true)
            {
                Console.WriteLine("JOB DONE");
                return;
            }
            
            //Get the current frame form capture device
            _counter++;

            Image<Bgr, Byte> originalFrame = new Image<Bgr, Byte>(new Bitmap(inputImage));

            currentFrame = originalFrame;

            // See if we have face, process is fast, sync!
            FaceDetectorProcessor fdp = new FaceDetectorProcessor(currentFrame);
            bool isFaceFound = fdp.isFaceDetected();
            Console.WriteLine("Face detected: " + isFaceFound.ToString() + "   Python is busy: " + bwImageAnalizer.IsBusy.ToString());

            if (isFaceFound == true)
            {
                if (bwImageAnalizer.IsBusy == false)
                    bwImageAnalizer.RunWorkerAsync(originalFrame);
            }


            /*
            //Convert it to Grayscale
            //Clear_Faces_Found();

            if (currentFrame != null)
            {
                gray_frame = currentFrame.Convert<Gray, Byte>();
                //Face Detector
                Rectangle[] facesDetected = Face.DetectMultiScale(gray_frame, 1.2, 10, new Size(50, 50), Size.Empty);

                //Action for each element detected
                for (int i = 0; i < facesDetected.Length; i++)
                {
                    try
                    {
                        facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                        facesDetected[i].Y += (int)(facesDetected[i].Width * 0.22);
                        facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.3);
                        facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.35);

                        result = currentFrame.Copy(facesDetected[i]).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                        result._EqualizeHist();
                        //draw the face detected in the 0th (gray) channel with blue color
                        currentFrame.Draw(facesDetected[i], new Bgr(Color.Red), 2);

                        if (Eigen_Recog.IsTrained)
                        {
                            string name = Eigen_Recog.Recognise(result);
                            int match_value = (int)Eigen_Recog.Get_Eigen_Distance;

                            //Draw the label for each face detected and recognized
                            currentFrame.Draw(name + " ", ref font, new Point(facesDetected[i].X - 2, facesDetected[i].Y - 2), new Bgr(Color.LightGreen));
                            ADD_Face_Found(result, name, match_value);
                        }

                    }
                    catch
                    {
                        //do nothing as parrellel loop buggy
                        //No action as the error is useless, it is simply an error in 
                        //no data being there to process and this occurss sporadically 
                    }
                };
                //Show the faces procesed and recognized
                image_PICBX.Image = currentFrame.ToBitmap();             
            }*/
        }

        /****************************************************************************************************/
    
        public void Analyze(Image[] img)
        {
        }

        private static readonly Random rnd = new Random();

        #region Implementation of IImageAnalyzer

        public void Analyze(ImageChangedEventArgs imgArgs, DeviceType device)
        {
            //  если изображение подходит, то вызываем             
            //AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(device, null, null, null));

            Image inputImage = imgArgs.Img;
            FrameGrabber_Parrellel(inputImage);


        }

        public event EventHandler<AnalyzeCompletedEventArgs> AnalyzeCompleted = (sender, e) => { };

        #endregion
    }
}
