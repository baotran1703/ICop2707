using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ICOP_3.ModelLogic
{
    public class IcopModel
    {
        // public string Result { get; set; }

        //public const string Result_OK = "True";

        //public const string Result_NG = "False";

        //public const string IsProject = "P";

        //public const string IsModel = "M";
        private History History_ = new History();
        private string name { get; set; }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string Path { get; set; }
        public string ImageResultName { get; set; }
        public DateTime TestTime { get; set; }
        public string ResutlTest { get; set; }
        public bool isTrue { get; set; }
        public bool ResultTest { get; private set; }
        private int numberPBA;
        public int Number_PBA
        {
            get { return numberPBA; }
            set
            {
                numberPBA = value;
            }
        }
        public string QR { get; set; }
        public string QR1 { get; set; }
        public string QR2 { get; set; }
        public string QR3 { get; set; }
        public string QR4 { get; set; }
        public int QR_Lenght { get; set; } = 23;
        public ObservableCollection<Step> QRs { get; set; }
        public BitmapSource ImageResultHistory { get; set; }
        public System.Drawing.Bitmap Image1;
        public System.Drawing.Bitmap ImageSource;
        public string PathImage1 { get; set; }
        public System.Drawing.Bitmap Image2;
        public string PathImage2 { get; set; }
        public System.Drawing.Bitmap Image3;
        public string PathImage3 { get; set; }
        public System.Drawing.Bitmap Image4;
        public string PathImage4 { get; set; }
        public BitmapSource MergeSource;
        public ObservableCollection<Step> Steps { get; set; }
        public System.Drawing.Bitmap ImageTest1;
        public System.Drawing.Bitmap ImageTest2;
        public System.Drawing.Bitmap ImageTest3;
        public System.Drawing.Bitmap ImageTest4;
        public BitmapSource ImageSourceTest1;
        public BitmapSource ImageSourceTest2;
        public BitmapSource ImageSourceTest3;
        public BitmapSource ImageSourceTest4;
        public enum RunTest_
        {
            WaitTest,
            Testting_,
            ResultOK,
            ResultNG
        }
        public IcopModel()
        {
            Name = "New model";
            Steps = new ObservableCollection<Step>();
        }
        #region Runtest

        public bool LoadImageTestAuto(BitmapSource source1, BitmapSource source2, BitmapSource source3, BitmapSource source4)
        {

            bool result = true;
            try
            {
                if (source1 != null) ImageTest1 = Function.BitmapFromSource(source1.Clone());
                else result = false;

                if (source2 != null) ImageTest2 = Function.BitmapFromSource(source2.Clone());
                else result = false;

                if (source3 != null) ImageTest3 = Function.BitmapFromSource(source3.Clone());
                else result = false;

                if (source4 != null) ImageTest4 = Function.BitmapFromSource(source4.Clone());
                else result = false;
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public Statitis.Statitis statitis = new Statitis.Statitis();

        public double statustest { get; set; }

        public event EventHandler StatusTest_Loadd;

        public double RunTest(int cam1, int cam2, int cam3, int cam4, BitmapSource Image1, BitmapSource Image2, BitmapSource Image3, BitmapSource Image4)
        {
            this.MergedBitmaps();
            STATE = RunTest_.Testting_.ToString();
            var start = DateTime.Now;
            var ImageTestSource = StitchBitmaps(cam1, cam2, cam3, cam4, Image1, Image2, Image3, Image4);
            ImageTestSource.Freeze();
            Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
            for (int i = 0; i < Steps.Count; i++)
            {
                var Step = Steps[i];

                statustest = ((Steps.Count * 100 / Steps.Count));

                if (statustest != 0)
                {
                    StatusTest_Loadd?.Invoke(null, null);
                }
                var startStep = DateTime.Now;
                if (Step.Skip)
                {
                    Step.ResutlTest = true;
                }
                else
                {
                    if (MergeSource != null)
                    {
                        if (Step != new ModelLogic.Step())
                        {
                            var Imagee = Step.GetImage(MergeSource); // big image source 
                            var ImageeTest = Step.GetImage(ImageTestSource); //big image for test from camera
                            if (Imagee != null)
                            {
                                Console.WriteLine(Step.Name);
                                switch (Step.FUNC)
                                {
                                    ///////////////////Read_QR////////////
                                    case ModelLogic.Step.Functions.Read_QR:
                                        if (ImageeTest != null)
                                            Step.Get1 = Function.ReadQR(ImageeTest, Step);

                                        QR = Step.Get1;

                                        if (Step.PCB == 1)
                                        {
                                            QR1 = Step.Get1;
                                        }

                                        if (Step.PCB == 2)
                                        {
                                            QR2 = Step.Get1;
                                        }

                                        if (Step.PCB == 3)
                                        {
                                            QR3 = Step.Get1;
                                        }
                                        if (Step.PCB == 4)
                                        {
                                            QR4 = Step.Get1;
                                        }
                                        Step.ImageTest = ImageeTest;
                                        Step.ImageSource = Imagee;

                                        break;
                                    ////////////////////Ceramic_Capacitors////////////
                                    case ModelLogic.Step.Functions.Ceramic_Capacitors:
                                        if ((ImageeTest != null) && (Imagee != null))
                                        {
                                            Step.ImageTest = ImageeTest;
                                            Step.ImageSource = Imagee;
                                            Function.Ceramic_Capacitors(Step, ImageeTest);
                                        }
                                        break;
                                    /////////////////////Matchtemplate/////////////
                                    case ModelLogic.Step.Functions.Matchtemplate:
                                        if ((ImageeTest != null) && (Imagee != null))
                                        {
                                            Step.ImageTest = null;
                                            Step.ImageSource = null;
                                            Step.ImageTest = ImageeTest;
                                            Step.ImageSource = Imagee;
                                            try
                                            {
                                                var template = new CroppedBitmap(Imagee, Step.funcRect);
                                                Mat Template = template.ToMat();
                                                Template.CvtColor(ColorConversionCodes.RGBA2RGB);
                                                Template.ConvertTo(Template, MatType.CV_8UC3);
                                                Function.Matchtemplate(Step, ImageeTest, template);
                                            }
                                            catch (OpenCvSharp.OpenCVException err)
                                            {
                                                Console.WriteLine(err.Message);
                                                Console.WriteLine(err.Data);
                                                Console.WriteLine(err.StackTrace);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.StackTrace);
                                                Step.ResutlTest = false;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("");
                                        }
                                        break;

                                    default:
                                        break;
                                    ///////////////////Polarized_Capacitors//////////////////
                                    case ModelLogic.Step.Functions.Polarized_Capacitors:
                                        if ((ImageeTest != null) && (Imagee != null))
                                        {
                                            Step.ImageTest = ImageeTest;  //ImageeTest = Step.GetImage(ImageTestSource); //big image for test from camera
                                            Step.ImageSource = Imagee;
                                            try
                                            {
                                                var template = new CroppedBitmap(Imagee, Step.funcRect); // template image crop from model curent
                                                Function.Polarized_Capacitors(Step, template);
                                            }
                                            catch (Exception e)
                                            {
                                                if (Step.funcRect == null)
                                                {
                                                    Step.Get1 = "Chua chon vung anh";
                                                }
                                                Console.WriteLine(e.StackTrace);
                                                Step.ResutlTest = false;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            Image1.Freeze();
            Image2.Freeze();
            Image3.Freeze();
            Image4.Freeze();
            STATE = RunTest_.WaitTest.ToString();
            ImageTestSource.Freeze();
            return DateTime.Now.Subtract(start).TotalSeconds;
        }

        public event EventHandler StateRunTest;

        private static string state;

        public string STATE
        {
            get { return state; }
            set
            {
                if (value != state)
                {
                    state = value;
                    StateRunTest?.Invoke(state, null);
                }
            }
        }

        ////////////////
        public void LoadImageTestManual()
        {
            OpenFileDialog openImage = new OpenFileDialog();
            openImage.Multiselect = true;
            openImage.Title = "Select a picture";

            if (openImage.ShowDialog() == true)
            {
                foreach (var item in openImage.FileNames)
                {
                    if (item.Contains("cam1"))
                    {
                        ImageTest1 = new Bitmap(item);
                    }
                    if (item.Contains("cam2"))
                    {
                        ImageTest2 = new Bitmap(item);
                    }
                    if (item.Contains("cam3"))
                    {
                        ImageTest3 = new Bitmap(item);
                    }
                    if (item.Contains("cam4"))
                    {
                        ImageTest4 = new Bitmap(item);
                    }
                }
            }
        }         //không dùng

        #endregion

        //////////History///////////
        bool isTrue_NoQR;
        //  bool isPCB1;
        public string resultString1;
        public string resultString2;
        public string resultString3;
        public string resultString4;

        History history = new History();
    

        public void SaveResultImage(BitmapSource ImageTest, int Total, int NG_Number, int OK_Number, string LineName)
        {
            var now = DateTime.Now;
            string nowyear = DateTime.Now.ToString("yyyy");
            string nowmonth = DateTime.Now.ToString("MM");
            string nowday = DateTime.Now.ToString("dd");

        




        var Folder_Result = DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + "_image";
            Console.WriteLine("Start save: " + Folder_Result);
            if (!Directory.Exists(Folder_Result))
            {
                Directory.CreateDirectory(Folder_Result);
            }

            try
            {

                BitmapSource ImageResultHistory = ImageTest;
                ImageTest.Freeze();


            }
            catch { }

            Mat ImageResultHistory_Mat = ImageTest.ToMat();

            ImageTest.Freeze();

            ImageResultHistory_Mat = ImageResultHistory_Mat.CvtColor(ColorConversionCodes.RGBA2RGB);

            foreach (var item in Steps)
            {
                if (item.ResutlTest == false)
                {
                    Cv2.Rectangle(ImageResultHistory_Mat, new OpenCvSharp.Point(item.Area.X, item.Area.Y), new OpenCvSharp.Point(item.Area.X + item.Area.Width, item.Area.Y + item.Area.Height), Scalar.Red, 3);
                    Cv2.PutText(ImageResultHistory_Mat, item.Name, new OpenCvSharp.Point(item.Area.X, item.Area.Y), fontFace: HersheyFonts.HersheyPlain, fontScale: 2, Scalar.Red, 2);
                }
                else
                {
                    Cv2.Rectangle(ImageResultHistory_Mat, new OpenCvSharp.Point(item.Area.X, item.Area.Y), new OpenCvSharp.Point(item.Area.X + item.Area.Width, item.Area.Y + item.Area.Height), Scalar.Green, 3);
                    Cv2.PutText(ImageResultHistory_Mat, item.Name, new OpenCvSharp.Point(item.Area.X, item.Area.Y), fontFace: HersheyFonts.HersheyPlain, fontScale: 2, Scalar.Green, 2);
                }
            };

            using (var fileStream = new FileStream(Folder_Result + "\\" + now.ToString("yyyyMMddHHmmss") + ".png", FileMode.Create, FileAccess.Write))
            {

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(ImageResultHistory_Mat.ToBitmapSource()));
                encoder.Save(fileStream);
            }
            History history = new History()
            {
                TestTime = now,
                ImageResultName = now.ToString("yyyyMMddHHmmss") + ".png",
                Result = "Pass",
                Name = QR,
                PCB = numberPBA

            };
            history.HistoryItems = new List<HistoryItem>();
            foreach (var item in Steps)
            {
                    if (item.FUNC == Step.Functions.Read_QR)
                    {
                        item.Name = item.Name;
                    }
                    else
                    {
                        history.HistoryItems.Add(new HistoryItem()
                        {

                            Name = item.Name,
                            PM = item.PM,
                            PCB = item.PCB,
                            FUNC = (Functions)item.FUNC,
                            Set1 = item.Set1,
                            Set2 = item.Set2,
                            Set3 = item.Set3,
                            Set4 = item.Set4,
                            Get1 = item.Get1,
                            Get2 = item.Get2,
                            Get3 = item.Get3,
                            Get4 = item.Get4,
                            Result = item.Result,
                            Skip = item.Skip
                        }); ;
                    }
                if (item.FUNC != Step.Functions.Read_QR)
                {
                    if (item.ResutlTest == false)
                    {
                        isTrue_NoQR = false;
                    }
                }
            }

            var resultList_ = Steps.Select(o => o.ResutlTest).ToList();
             
            if (resultList_.Contains(false))
            {
                if (isTrue_NoQR == false)
                {
                    history.Result = "Fail";

                }
                else
                {
                    history.Result = "Pass";

                }
            }
            else
            {
                history.Result = "Pass";

            }

            //string resultString = JsonSerializer.Serialize(history);
            //File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString + "\r\n");


            

            History history_1 = new History()
            {
                TestTime = now,
                ImageResultName = now.ToString("yyyyMMddHHmmss") + ".png",
                Result = "Pass",
                Name = QR1,
                PCB = 1,
                NumberNG1 = 0,
                NumberOK1 = 0,
            };
            history_1.HistoryItems = new List<HistoryItem>();           
            foreach (var item in Steps)
            {
                if (item.FUNC == Step.Functions.Read_QR)
                {
                    item.Name = item.Name;
                }
                else
                {
                    if (item.PCB == 1 )
                    {
                        history_1.HistoryItems.Add(new HistoryItem()
                        {
                            Name = item.Name,
                            PM = item.PM,
                            PCB = item.PCB,
                            FUNC = (Functions)item.FUNC,
                            Set1 = item.Set1,
                            Set2 = item.Set2,
                            Set3 = item.Set3,
                            Set4 = item.Set4,
                            Get1 = item.Get1,
                            Get2 = item.Get2,
                            Get3 = item.Get3,
                            Get4 = item.Get4,
                            Result = item.Result,
                            Skip = item.Skip
                        }); ;
                        if (item.ResutlTest == false)
                        {
                            history_1.Result = "Fail";


                        }
                    }

                   

                }
            }


            if (history_1.Result == "Fail")
            {
                history_1.NumberNG1 = 1;
            }
            else if (history_1.Result == "Pass")
            {
                history_1.NumberOK1 = 1;
            }



            

            resultString1 = JsonSerializer.Serialize(history_1);
           

            History history_2 = new History()
            {
                TestTime = now,
                ImageResultName = now.ToString("yyyyMMddHHmmss") + ".png",
                Result = "Pass",
                Name = QR2,
                PCB = 2,
                NumberNG2 =0,
                NumberOK2 = 0,


            };
            history_2.HistoryItems = new List<HistoryItem>();
            foreach (var item in Steps)
            {
                if (item.FUNC == Step.Functions.Read_QR)
                {
                    item.Name = item.Name;
                }
                else
                {
                    if (item.PCB == 2 )
                    {
                        history_2.HistoryItems.Add(new HistoryItem()
                        {
                            Name = item.Name,
                            PM = item.PM,
                            PCB = item.PCB,
                            FUNC = (Functions)item.FUNC,
                            Set1 = item.Set1,
                            Set2 = item.Set2,
                            Set3 = item.Set3,
                            Set4 = item.Set4,
                            Get1 = item.Get1,
                            Get2 = item.Get2,
                            Get3 = item.Get3,
                            Get4 = item.Get4,
                            Result = item.Result,
                            Skip = item.Skip
                        }); ;
                        if (item.ResutlTest == false)
                        {
                            history_2.Result = "Fail";


                        }
                    }

                }


            }

            if (history_2.Result == "Fail")
            {
                history_2. NumberNG2 = 1;
            }
            else if (history_2.Result == "Pass")
            {
                history_2.NumberOK2 = 1;
            }
            resultString2 = JsonSerializer.Serialize(history_2);
        

            History history_3 = new History()
            {
                TestTime = now,
                ImageResultName = now.ToString("yyyyMMddHHmmss") + ".png",
                Result = "Pass",
                Name = QR3,
                PCB = 3,
                NumberNG3 = 0,
                NumberOK3 = 0,

            };
            history_3.HistoryItems = new List<HistoryItem>();
            foreach (var item in Steps)
            {
                if (item.FUNC == Step.Functions.Read_QR)
                {
                    item.Name = item.Name;
                }
                else
                {
                    if (item.PCB == 3 )
                    {
                        history_3.HistoryItems.Add(new HistoryItem()
                        {
                            Name = item.Name,
                            PM = item.PM,
                            PCB = item.PCB,
                            FUNC = (Functions)item.FUNC,
                            Set1 = item.Set1,
                            Set2 = item.Set2,
                            Set3 = item.Set3,
                            Set4 = item.Set4,
                            Get1 = item.Get1,
                            Get2 = item.Get2,
                            Get3 = item.Get3,
                            Get4 = item.Get4,
                            Result = item.Result,
                            Skip = item.Skip
                        }); ;
                        if (item.ResutlTest == false)
                        {
                            history_3.Result = "Fail";
                        }
                    }

                }


            }

            if (history_3.Result == "Fail")
            {
                history_3.NumberNG3 = 1;
            }
            else if (history_3.Result == "Pass")
            {
                history_3.NumberOK3 = 1;
            }

            resultString3 = JsonSerializer.Serialize(history_3);      
            History history_4 = new History()
            {
                TestTime = now,
                ImageResultName = now.ToString("yyyyMMddHHmmss") + ".png",
                Result = "Pass",
                Name = QR4,
                PCB = 4,
                NumberNG4 = 0,
                NumberOK4 = 0,

            };

            history_4.HistoryItems = new List<HistoryItem>();
            foreach (var item in Steps)
            {
                if (item.FUNC == Step.Functions.Read_QR)
                {
                    item.Name = item.Name;
                }
                else
                {
                    if (item.PCB == 4)
                    {
                        history_4.HistoryItems.Add(new HistoryItem()
                        {
                            Name = item.Name,
                            PM = item.PM,
                            PCB = item.PCB,
                            FUNC = (Functions)item.FUNC,
                            Set1 = item.Set1,
                            Set2 = item.Set2,
                            Set3 = item.Set3,
                            Set4 = item.Set4,
                            Get1 = item.Get1,
                            Get2 = item.Get2,
                            Get3 = item.Get3,
                            Get4 = item.Get4,
                            Result = item.Result,
                            Skip = item.Skip
                        }); ;
                        if (item.ResutlTest == false)
                        {
                            history_4.Result = "Fail";
                        }
                    }

                }


            }
            if (history_4.Result == "Fail")
            {
                history_4.NumberNG4 = 1;
            }
            else if (history_4.Result == "Pass")
            {
                history_4.NumberOK4 = 1;
            }
            resultString4 = JsonSerializer.Serialize(history_4);
           

            if (Number_PBA == 1)
            {
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString1 + "\r\n");

            }


            if (Number_PBA == 2)
            {
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString1 + "\r\n");
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString2 + "\r\n");
               
            }

            if (Number_PBA == 3)
            {
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString1 + "\r\n");
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString2 + "\r\n");
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString3 + "\r\n");
            }


            if (Number_PBA == 4)
            {
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString1 + "\r\n");
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString2 + "\r\n");
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString3 + "\r\n");
                File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString4 + "\r\n");
            }



            isTrue_NoQR = true;
            foreach (var step in Steps)
            {

                if (step.FUNC != Step.Functions.Read_QR)
                {
                    //  isQRcode = false;
                    if (step.ResutlTest == false)
                    {
                        isTrue_NoQR = false;
                    }

                }

            }
            statitis.NG_Number = NG_Number;
            statitis.OK_Number = OK_Number;
            statitis.Total = Total;

            var resultList = Steps.Select(o => o.ResutlTest).ToList();
           
            if (resultList.Contains(false))
            {
                if (isTrue_NoQR == false)
                {
                    //  history.Result = "Fail";
                    statitis.NG_Number = statitis.NG_Number + history_1.NumberNG1 + history_2.NumberNG2 + history_3.NumberNG3 + history_4.NumberNG4;
                    statitis.OK_Number = statitis.OK_Number + history_1.NumberOK1 + history_2.NumberOK2 + history_3.NumberOK3 + history_4.NumberOK4;

                }
                else
                {
                    //  history.Result = "Pass";
                    statitis.OK_Number = statitis.OK_Number + Number_PBA; 
                }
            }
            else
            {
                // history.Result = "Pass";
                statitis.OK_Number = statitis.OK_Number + Number_PBA;
            }
            statitis.Total = statitis.OK_Number + statitis.NG_Number;
            string NumberNG = JsonSerializer.Serialize(statitis.NG_Number);
            string NumberOK = JsonSerializer.Serialize(statitis.OK_Number);
            string NumberTotal = JsonSerializer.Serialize(statitis.Total);
            string LineName_ = JsonSerializer.Serialize(LineName);
            string[] Number = { NumberNG, NumberOK, NumberTotal, LineName_ };
            var path = DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\NumberResult.txt";
            File.WriteAllLines(path, Number);
            //string resultString = JsonSerializer.Serialize(history);
            //File.AppendAllText((DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\" + now.ToString("yyyyMMdd") + ".icop"), resultString + "\r\n");
            SaveHistory_Complete?.Invoke("OK", null);
            Console.WriteLine("End save: " + Folder_Result);
        }

        public event EventHandler SaveHistory_Complete;

        public WriteableBitmap result;

        public BitmapSource StitchBitmaps(int cam1, int cam2, int cam3, int cam4, BitmapSource Image1, BitmapSource Image2, BitmapSource Image3, BitmapSource Image4)
        {
            var width = Image1.PixelWidth + Image2.PixelWidth;
            var height = Image1.PixelHeight + Image3.PixelHeight;
            if (Image1 != null && Image2 != null && Image3 != null && Image4 != null)
            {
                result = new WriteableBitmap(width, height, 96, 96, Image1.Format, null);
                var stride1 = (Image1.PixelWidth * Image1.Format.BitsPerPixel + 7) / 8;
                var stride2 = (Image2.PixelWidth * Image2.Format.BitsPerPixel + 7) / 8;
                var stride3 = (Image3.PixelWidth * Image3.Format.BitsPerPixel + 7) / 8;
                var stride4 = (Image4.PixelWidth * Image4.Format.BitsPerPixel + 7) / 8;
                var size = Image1.PixelHeight * stride1;
                size = Math.Max(size, Image2.PixelHeight * stride2);
                size = Math.Max(size, Image3.PixelHeight * stride3);
                size = Math.Max(size, Image4.PixelHeight * stride4);
                var buffer = new byte[size];
                if (cam1 == 1 && cam2 == 2 && cam3 == 3 && cam4 == 4) // đúng
                {

                    Image1.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image2.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image3.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image4.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 1 && cam2 == 2 && cam3 == 4 && cam4 == 3)
                {

                    Image1.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image2.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image4.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image3.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 1 && cam2 == 3 && cam3 == 2 && cam4 == 4)
                {
                    Image1.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image3.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image2.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image4.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 1 && cam2 == 3 && cam3 == 4 && cam4 == 2)     // vị tri  1 2 3 4 tương tương ứng   1 = image 1, 2 = image 4,  3 = image 2, 4 = image 3
                {
                    Image1.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);            // ví trị  1

                    Image4.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);  // ví trị  2

                    Image2.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);  // ví trị  3

                    Image3.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);  // ví trị  4
                }

                if (cam1 == 1 && cam2 == 4 && cam3 == 2 && cam4 == 3) ////
                {
                    Image1.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image3.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image4.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image2.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 1 && cam2 == 4 && cam3 == 3 && cam4 == 2)  ///// đúng
                {
                    Image1.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image4.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image3.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image2.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image3.PixelWidth, Image2.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                /////////////////////

                if (cam1 == 2 && cam2 == 1 && cam3 == 3 && cam4 == 4)
                {
                    Image2.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image1.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image3.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image4.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 2 && cam2 == 1 && cam3 == 4 && cam4 == 3)
                {
                    Image2.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image1.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image4.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image3.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 2 && cam2 == 3 && cam3 == 4 && cam4 == 1)
                {
                    Image4.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image1.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image2.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image3.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 2 && cam2 == 3 && cam3 == 1 && cam4 == 4)
                {
                    Image3.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image1.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image2.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image4.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 2 && cam2 == 4 && cam3 == 1 && cam4 == 3)
                {
                    Image3.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image1.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image4.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image2.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 2 && cam2 == 4 && cam3 == 3 && cam4 == 1)
                {
                    Image4.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image1.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image3.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image2.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image3.PixelWidth, Image2.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                //////////

                if (cam1 == 3 && cam2 == 1 && cam3 == 2 && cam4 == 4)
                {
                    Image2.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image3.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image1.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image4.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image3.PixelWidth, Image2.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 3 && cam2 == 1 && cam3 == 4 && cam4 == 2)
                {
                    Image2.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image4.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image1.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image3.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 3 && cam2 == 2 && cam3 == 4 && cam4 == 1)
                {
                    Image4.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image2.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image1.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image3.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 3 && cam2 == 2 && cam3 == 1 && cam4 == 4)
                {
                    Image3.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image2.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image1.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image4.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 3 && cam2 == 4 && cam3 == 1 && cam4 == 2)
                {
                    Image3.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image4.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image1.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image2.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 3 && cam2 == 4 && cam3 == 2 && cam4 == 1)
                {
                    Image4.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image3.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image1.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image2.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }
                ////////

                if (cam1 == 4 && cam2 == 1 && cam3 == 2 && cam4 == 3) ////////////////////////////////////////////
                {
                    Image2.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image3.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image4.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);  ///cam 1

                    Image1.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 4 && cam2 == 1 && cam3 == 3 && cam4 == 2)
                {
                    Image2.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image4.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image3.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image1.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 4 && cam2 == 2 && cam3 == 1 && cam4 == 3)
                {
                    Image3.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image2.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image4.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image1.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 4 && cam2 == 2 && cam3 == 3 && cam4 == 1)
                {
                    Image4.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image2.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image3.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image1.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 4 && cam2 == 3 && cam3 == 2 && cam4 == 1) ////// đnúng 
                {
                    Image4.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image3.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image2.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image1.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                if (cam1 == 4 && cam2 == 3 && cam3 == 1 && cam4 == 2)
                {
                    Image3.CopyPixels(buffer, stride1, 0);
                    result.WritePixels(new Int32Rect(0, 0, Image1.PixelWidth, Image1.PixelHeight), buffer, stride1, 0);

                    Image4.CopyPixels(buffer, stride2, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, 0, Image2.PixelWidth, Image2.PixelHeight), buffer, stride2, 0);

                    Image2.CopyPixels(buffer, stride3, 0);
                    result.WritePixels(new Int32Rect(0, Image1.PixelHeight, Image3.PixelWidth, Image3.PixelHeight), buffer, stride3, 0);

                    Image1.CopyPixels(buffer, stride4, 0);
                    result.WritePixels(new Int32Rect(Image1.PixelWidth, Image1.PixelHeight, Image4.PixelWidth, Image4.PixelHeight), buffer, stride4, 0);
                }

                Image1.Freeze();
                Image2.Freeze();
                Image3.Freeze();
                Image4.Freeze();
            }

            return result;
        }
        #region Bitmap

        private Bitmap MergedBitmaps(Bitmap bmp1, Bitmap bmp2, Bitmap bmp3, Bitmap bmp4)
        {


            if (bmp1 != null && bmp2 != null && bmp3 != null && bmp4 != null)
            {
                Bitmap result = new Bitmap(bmp1.Width + bmp2.Width,      ////////////bug
                                      bmp1.Height + bmp3.Height);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(bmp1, new PointF(0, 0));
                    g.DrawImage(bmp2, new PointF(bmp1.Width, 0));
                    g.DrawImage(bmp3, new PointF(0, bmp1.Height));
                    g.DrawImage(bmp4, new PointF(bmp1.Width, bmp1.Height));
                }
                Function.ShowImage(result.ToBitmapSource(), "Source from camera");
                result.Save("D:\\mergerImage.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                return result;
            }
            return null;
        }

        public Bitmap MergedBitmapss(Bitmap bmp1, Bitmap bmp2, Bitmap bmp3, Bitmap bmp4)
        {
            if (bmp1 != null && bmp2 != null && bmp3 != null && bmp4 != null)
            {
                Bitmap result = new Bitmap((bmp1.Width + bmp2.Width),
                                      (bmp1.Height + bmp3.Height));
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(bmp1, new PointF(0, 0));
                    g.DrawImage(bmp2, new PointF(bmp1.Width, 0));
                    g.DrawImage(bmp3, new PointF(0, bmp1.Height));
                    g.DrawImage(bmp4, new PointF(bmp1.Width, bmp1.Height));
                }

                return result;
            }
            return null;
        }

        public void MergedBitmaps()
        {
            if (Image1 != null && Image2 != null && Image3 != null && Image4 != null)
            {
                Bitmap result = new Bitmap((Image1.Width + Image2.Width),
                                      (Image1.Height + Image3.Height));
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(Image1, new PointF(0, 0));
                    g.DrawImage(Image2, new PointF(Image1.Width, 0));
                    g.DrawImage(Image3, new PointF(0, Image1.Height));
                    g.DrawImage(Image4, new PointF(Image1.Width, Image1.Height));
                }

                MergeSource = Function.Convert(result);
                Function.ShowImage(MergeSource, "Source from program");
                result.Dispose();
            }
        }
        #endregion
    }



}
