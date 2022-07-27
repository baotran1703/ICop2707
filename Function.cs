using ICOP_3.ModelLogic;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;


using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using Point = OpenCvSharp.Point;


namespace ICOP_3
{
    public enum Functions
    {
        MatchTemplate,
        Polarized_Capacitors,
        Ceramic_Capacitors,
        ReadQR
    }

    public class Function
    {
        public static BitmapSource Polarized_Capacitors(Step step, BitmapSource source, int position_Polarized)
        {
            Mat ImageMat = new Mat();
            Mat ImageTemplate = new Mat();
            ImageMat = source.ToMat();
            ImageTemplate = step.DetectedImage != null ? step.DetectedImage.ToMat() : new Mat();
            double score = 0;
            using (Mat scoreMat = ImageMat.MatchTemplate(ImageTemplate, TemplateMatchModes.CCoeffNormed))
            {
                scoreMat.MinMaxLoc(out _, out score, out _, out OpenCvSharp.Point maxLocPoint);
                int ImageWidth = (int)(source.Width / 3);
                int ImageHeight = (int)(source.Height / 3);
                var positionsRect_Capacitor = ((maxLocPoint.X + ImageTemplate.Width / 2) / ImageWidth) + ((maxLocPoint.Y + ImageTemplate.Height / 2) / (int)ImageHeight) * 3 + 1;
                step.Set1 = position_Polarized.ToString();
                step.Get1 = positionsRect_Capacitor.ToString();
                step.Set2 = "";
                step.Set3 = "";
                step.Get3 = ""; 
                step.Set4 = "";
                step.Get2 = "";
                if (position_Polarized == positionsRect_Capacitor)
                {
                    step.Result = "Pass"; step.ResutlTest = true;   
                    ImageMat.Rectangle(
                        maxLocPoint,
                        new OpenCvSharp.Point()
                        {
                            X = maxLocPoint.X + ImageTemplate.Width,
                            Y = maxLocPoint.Y + ImageTemplate.Height,
                        },
                        new Scalar(0, 255, 0, 255),
                        1
                        );
                }
                else
                {
                    step.Result = "Fail"; step.ResutlTest = false;                 
                    ImageMat.Rectangle(
                            maxLocPoint,
                            new OpenCvSharp.Point()
                            {
                                X = maxLocPoint.X + ImageTemplate.Width,
                                Y = maxLocPoint.Y + ImageTemplate.Height,
                            },
                            new Scalar(255, 0, 0, 255),
                            1
                            );
                }
            }
         
            return ImageMat.ToBitmapSource();     
        }

        public static void Polarized_Capacitors(Step step, BitmapSource template) // runtest     // template image of model curent                    
        {
            Mat ImageMat = new Mat();
            Mat ImageMat_ = new Mat();
            ImageMat_ = step.ImageTest.ToMat();
            ImageMat = ImageMat_.CvtColor(ColorConversionCodes.RGB2RGBA);
            Mat ImageTemplate = template.ToMat();  // CV_8UC4
            Mat ImageTemplat_ = new Mat();
            ImageTemplate.ConvertTo(ImageTemplat_, MatType.CV_8UC4);
            double score = 0;
            var position_Polarized = double.Parse(step.Set1.ToString());
            using (Mat scoreMat = ImageMat.MatchTemplate(ImageTemplat_, TemplateMatchModes.CCoeffNormed))
            {
                scoreMat.MinMaxLoc(out _, out score, out _, out OpenCvSharp.Point maxLocPoint);
                int ImageWidth = (int)(ImageMat.Width / 3);
                int ImageHeight = (int)(ImageMat.Height / 3);
                var positionsRect_Capacitor = ((maxLocPoint.X + ImageTemplate.Width / 2) / ImageWidth) + ((maxLocPoint.Y + ImageTemplate.Height / 2) / (int)ImageHeight) * 3 + 1;
                if (score < 0.65)
                {
                    positionsRect_Capacitor = 10;
                }
                step.Get1 = positionsRect_Capacitor.ToString();
                if (position_Polarized == positionsRect_Capacitor)
                {
                    step.Result = "Pass";
                    step.ResutlTest = true;
                    ImageMat.Rectangle(
                        maxLocPoint,
                        new OpenCvSharp.Point()
                        {
                            X = maxLocPoint.X + ImageTemplate.Width,
                            Y = maxLocPoint.Y + ImageTemplate.Height,
                        },
                       new Scalar(0, 255, 0, 255),      //////green
                       // new Scalar(0,0, 255, 255),    //red
                        1
                        );
                }
                else
                {
                    step.Result = "Fail";
                    step.ResutlTest = false;
                    ImageMat.Rectangle(
                            maxLocPoint,
                            new OpenCvSharp.Point()
                            {
                                X = maxLocPoint.X + ImageTemplate.Width,
                                Y = maxLocPoint.Y + ImageTemplate.Height,
                            },
                            new Scalar(0, 0, 255, 255),    //
                            1
                            );
                    step.Set2 = "False";
                }
            }
          //  step.ImageTestReuslt = ImageMat.ToBitmapSource();
            step.ImageTestReuslt = ImageMat.CvtColor(ColorConversionCodes.RGBA2RGB).ToBitmapSource();
            ImageMat.Dispose();
            ImageTemplate.Dispose();
        }

        public static BitmapSource Matchtemplate(Step step, BitmapSource source, string Threasold)       
        {
            Mat ImageMat = new Mat();
            Mat ImageTemplate = new Mat();
            ImageMat = source.ToMat();
            ImageTemplate = step.DetectedImage != null ? step.DetectedImage.ToMat() : new Mat();
            step.Set1 = Threasold;
            step.Set2 = "";
            step.Get2 = "";
            step.Set3 = "";
            step.Get3 = "";
            step.Set4 = "";
            double ThreasoldDouble = 0;
            bool isNumeric = double.TryParse(Threasold.ToString(), out ThreasoldDouble);
            double score = 0;
            using (Mat scoreMat = ImageMat.MatchTemplate(ImageTemplate, TemplateMatchModes.CCoeffNormed))
            {
                scoreMat.MinMaxLoc(out _, out score, out _, out OpenCvSharp.Point maxLocPoint);
                step.Get1 = score.ToString("F2");

                if (ThreasoldDouble <= score)
                {
                    step.Result = "Pass";
                    step.ResutlTest = true;
                    ImageMat.Rectangle(
                    maxLocPoint,
                    new OpenCvSharp.Point()
                    {
                        X = maxLocPoint.X + ImageTemplate.Width,
                        Y = maxLocPoint.Y + ImageTemplate.Height,
                    },
                    new Scalar(0, 255, 0, 255),
                    1
                    );
                }
                else
                {
                    step.Result = "Fail";
                    step.ResutlTest = false;
                    ImageMat.Rectangle(
                    maxLocPoint,
                    new OpenCvSharp.Point()
                    {
                        X = maxLocPoint.X + ImageTemplate.Width,
                        Y = maxLocPoint.Y + ImageTemplate.Height,
                    },
                    new Scalar(0, 0, 255, 255),
                    1
                    );
                }
                scoreMat.Dispose();
            }
            return ImageMat.ToBitmapSource();    
        }

        public static void Matchtemplate(Step step, BitmapSource testImage, BitmapSource Template) //runtest
        {       
            Mat source = testImage.ToMat();       // CV_8UC3
            Mat ImageMat = new Mat();
            ImageMat = source.CvtColor(ColorConversionCodes.RGB2RGBA);
            Mat ImageTemplate = Template.ToMat();  // CV_8UC4
            Mat ImageMatt = new Mat();
            ImageTemplate.ConvertTo(ImageMatt, MatType.CV_8UC4);
            double Threasold = double.Parse(step.Set1);
            double score;
            using (Mat scoreMat = ImageMat.MatchTemplate(ImageMatt, TemplateMatchModes.CCoeffNormed))
            {
                scoreMat.MinMaxLoc(out _, out score, out _, out OpenCvSharp.Point maxLocPoint);
                step.Get1 = score.ToString("F2");
                if (Threasold <= score)
                {
                    step.Result = "Pass";
                    step.ResutlTest = true;
                   
                    ImageMat.Rectangle(maxLocPoint, new OpenCvSharp.Point()
                    {
                        X = maxLocPoint.X + ImageTemplate.Width,
                        Y = maxLocPoint.Y + ImageTemplate.Height,
                    },
                          new Scalar(0, 255, 0, 255), 1
                    );
                    // AGRB
                }
                else
                {
                    step.Result = "Fail";
                    step.ResutlTest = false;
                    
                    ImageMat.Rectangle(
                        maxLocPoint,
                        new OpenCvSharp.Point()
                        {
                            X = maxLocPoint.X + ImageTemplate.Width,
                            Y = maxLocPoint.Y + ImageTemplate.Height,
                        },
                       new Scalar(0, 0, 255, 0), //(0, 0, 255, 0)
                        1
                        );
                }

                Mat Ima = ImageMat.CvtColor(ColorConversionCodes.RGBA2RGB);
                step.ImageTestReuslt = Ima.ToBitmapSource();
              
            }
            step.ImageTestReuslt = ImageMat.CvtColor(ColorConversionCodes.RGBA2RGB).ToBitmapSource();
            ImageMat.Dispose();
            ImageTemplate.Dispose();
        }

        public static BitmapSource Ceramic_Capacitors(Step step, BitmapSource source, double ThreasoldTop, double ThreasoldBottom, int X, int Y)
        {
            step.Get1 = "";
            Mat ImageMat = source.ToMat();
            Mat ImageMatHSV = ImageMat.CvtColor(ColorConversionCodes.BGR2HSV);
            Scalar HIGHLimit = new Scalar(ThreasoldTop, 255, 255);
            Scalar LOWLimit = new Scalar(ThreasoldBottom, 0, 0);
            Mat mat = ImageMatHSV.InRange(LOWLimit, HIGHLimit);
            Point[][] points;
            Cv2.FindContours(mat, out points, out _, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
            Mat mask = ImageMatHSV.InRange(new Scalar(255, 255, 255), new Scalar(0, 0, 0));
            if (points.Length > 0)
            {
                double area = 0;
                int maxpointIndex = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    if (Cv2.ContourArea(points[i]) > area)
                    {
                        area = Cv2.ContourArea(points[i]);
                        maxpointIndex = i;
                    }
                }
                Cv2.DrawContours(mask, points, maxpointIndex, new Scalar(255, 255, 255, 255), thickness: Cv2.FILLED);
                step.Get2 = (area / (mat.Width * mat.Height) * 100).ToString("F2");
                var ThreasoldColor = double.Parse(step.Get2);
                if (step.Set4 != null)
                {
                    var ThreasoldColorTop = double.Parse(step.Set4);
                    var ThreasoldColorBotttom = double.Parse(step.Set3);

                    if (ThreasoldColorBotttom < ThreasoldColor && ThreasoldColor < ThreasoldColorTop)
                    {
                        step.Result = "Pass";
                        step.ResutlTest = true;
                    }
                    else
                    {
                        step.Result = "Fail";
                        step.ResutlTest = false;
                    }
                       
                }
            }
            mask = mask.CvtColor(ColorConversionCodes.GRAY2RGBA);
            ImageMat = ImageMat.BitwiseAnd(mask);
            mask.Dispose();
            return ImageMat.ToBitmapSource();
        }

        public static void Ceramic_Capacitors(Step step, BitmapSource source)   // runtest
        {
            Mat ImageMat = new Mat();
            ImageMat = source.ToMat();
            double ThreasoldTop = double.Parse(step.Set2);
            double ThreasoldBottom = double.Parse(step.Set1);
            var ThreasoldColorTop = double.Parse(step.Set4);
            var ThreasoldColorBotttom = double.Parse(step.Set3);
            Mat ImageMatHSV = ImageMat.CvtColor(ColorConversionCodes.BGR2HSV);
            Scalar HIGHLimit = new Scalar(ThreasoldTop, 255, 255);
            Scalar LOWLimit = new Scalar(ThreasoldBottom, 0, 0);
            Mat mat = ImageMatHSV.InRange(LOWLimit, HIGHLimit);    
            Point[][] points;
            Cv2.FindContours(mat, out points, out _, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
            Mat mask = ImageMatHSV.InRange(new Scalar(255, 255, 255), new Scalar(0, 0, 0));
            if (points.Length > 0)
            {
                double area = 0;
                int maxpointIndex = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    if (Cv2.ContourArea(points[i]) > area)
                    {
                        area = Cv2.ContourArea(points[i]);
                        maxpointIndex = i;
                    }
                }
                Cv2.DrawContours(mask, points, maxpointIndex, new Scalar(255, 255, 255, 255), thickness: Cv2.FILLED);
                step.Get2 = (area / (mat.Width * mat.Height) * 100).ToString("F2");
                var ThreasoldColor = double.Parse(step.Get2);
                if (ThreasoldColorBotttom < ThreasoldColor && ThreasoldColor < ThreasoldColorTop)
                {
                    step.Result = "Pass";
                    step.ResutlTest = true;
                }
                else
                {
                    step.Result = "Fail";
                    step.ResutlTest = false;
                }
     
            }
            else
            {
                //step.ResutlTest = false;
                //step.Result = "Fail";

            }
            mask = mask.CvtColor(ColorConversionCodes.GRAY2RGBA);
            step.ImageTestReuslt = ImageMat.ToBitmapSource();
            ImageMat.Dispose();
            mask.Dispose();

        }

        public static string ReadQR(BitmapSource source, Step step)
        {
            var MatSource = source.ToMat();
            Mat MatSourceGray = MatSource.CvtColor(ColorConversionCodes.RGBA2GRAY);
            for (int i = 3; i < 21; i = i + 2)
            {
                for (int j = 0; j < 11; j++)
                {
                    Mat Imageee = MatSourceGray.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, i, j);
                    QRCodeDetector qrDecoder = new QRCodeDetector();

                    if ( Imageee != null && qrDecoder != null)
                    {
                        try
                        {
                            string result = qrDecoder.DetectAndDecode(Imageee, out _, null);
                            if (result != null && result != "")
                            {
                                var sourcee = Imageee.ToBitmapSource();
                                step.ImageTestReuslt = sourcee;
                                step.Result = "Pass";
                                step.ResutlTest = true;
                                return result;
                            }

                        }
                        catch
                        {
                        }
                      
                    }
                }
            }
            step.ResutlTest = false;
            step.Result = "Fail";
            step.ImageTestReuslt = source;
            return "NO QR";
        }

        public static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var bitmapSource = BitmapSource.Create(bitmapData.Width, bitmapData.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution, PixelFormats.Bgr32, null,
                                                   bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        public static BitmapSource Convert(System.Drawing.Bitmap bitmap, int Channel)
        {
            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var bitmapSource = BitmapSource.Create(bitmapData.Width, bitmapData.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution, PixelFormats.Bgr24, null,
                                                   bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource) /////////// bug
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));///
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
                bitmap.Dispose();
            }
            return bitmap;
        }

        public static Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;

        }

        public static void ShowImage(BitmapSource source, string WindownName)
        {
          //  Mat image = source.ToMat();
          //  Cv2.ImShow(WindownName, image);
        }
    }
}




















