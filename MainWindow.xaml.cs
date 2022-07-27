using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
using HVT.VTM.Base;
using System.Windows.Media.Animation;
using System.IO.Ports;
using System.Windows.Threading;
using Image = System.Drawing.Image;
using ICOP_3.ModelLogic;
using ICOP_3;
using System.ComponentModel;
using ICOP_3.Statitis;
using OpenCvSharp;
using System.Diagnostics;

namespace ICOP_3
{
    public partial class MainWindow : System.Windows.Window
    {
        Model modelWindow = new Model();
        string Cam1;
        string Cam2;
        string Cam3;
        string Cam4;
        public bool Bybassing;
        bool testting;
        bool isTesting
        {
            get { return testting; }
            set
            {
                if (value == false)
                {
                    Show_app_status(APP_STATUS.READY);
                }
                testting = value;
            }
        }
        bool isAutoTest = false;
        bool GR_CAM_SHOW = false;
        bool GR_COM_SHOW = false;
        bool GR_PROGRAM_SHOW = false;
        bool GR_SETTING_SHOW = false;
        private bool BlinkOn = false;
        public Statitis.Statitis Statitis { get; set; }
        ICOP_3.ModelLogic.IcopModel IcopModel = new ICOP_3.ModelLogic.IcopModel();
        ICOP_3.ModelLogic.IcopModel IcopModell = new ICOP_3.ModelLogic.IcopModel();
        ICOP_3.ModelLogic.IcopModel IcopProject = new ICOP_3.ModelLogic.IcopModel();
        ICOP_3.ModelLogic.IcopModel IcopProjectt = new ICOP_3.ModelLogic.IcopModel();
        ModelLogic.Step CurrentStep = new ModelLogic.Step();
        //public event EventHandler Camstopped;
        public CameraStreaming cameraStreaming1;
        public CameraStreaming cameraStreaming2;
        public CameraStreaming cameraStreaming3;
        public CameraStreaming cameraStreaming4;
        DispatcherTimer SetCamPropertiesTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(2000)
        };

        bool CameraSetupDone = false;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            modelWindow.ModelClose += ModelWindow_ModelClose;
            cbb_COM_PORT.ItemsSource = SerialPort.GetPortNames();
            Serial_Init();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Tick += timer_Tick1;
            timer.Start();

            //DispatcherTimer timer_test = new DispatcherTimer();
            //timer_test.Interval = TimeSpan.FromSeconds(5);
            //timer_test.Tick += timer_Runtest;
            //timer_test.Start();

            SetCamPropertiesTimer.IsEnabled = true;
            DispatcherTimer timerCamera = new DispatcherTimer();
            timerCamera.Interval = TimeSpan.FromSeconds(1.5);
            timerCamera.Tick += SetCamPropertiesTimer_Tick;
            timerCamera.Start();

            //  SetCamPropertiesTimer.Start();
            var gcTimer = new DispatcherTimer();
            gcTimer.Tick += (sender, e) => { GC.Collect(); };
            gcTimer.Interval = TimeSpan.FromSeconds(1);
            gcTimer.Start();
        }

        private void timer_Runtest(object sender, EventArgs e)
        {
            // Show_app_status(APP_STATUS.Testting);
            isTesting = true;
            isAutoTest = true;
            if (btAuto.IsChecked == true)
            {
                string[] Set_CPositionCam = File.ReadAllLines(DirectoryF.Setting + "\\SetPositionCam.txt");
                if (Int32.TryParse(Set_CPositionCam[0], out int cam0ps)) cbbCam1.SelectedIndex = cam0ps - 1;
                if (Int32.TryParse(Set_CPositionCam[1], out int cam1ps)) cbbCam2.SelectedIndex = cam1ps - 1;
                if (Int32.TryParse(Set_CPositionCam[2], out int cam2ps)) cbbCam3.SelectedIndex = cam2ps - 1;
                if (Int32.TryParse(Set_CPositionCam[3], out int cam3ps)) cbbCam4.SelectedIndex = cam3ps - 1;
                int Position_Camera1 = int.Parse(cbbCam1.Text.ToString());
                int Position_Camera2 = int.Parse(cbbCam2.Text.ToString());
                int Position_Camera3 = int.Parse(cbbCam3.Text.ToString());
                int Position_Camera4 = int.Parse(cbbCam4.Text.ToString());
                if (isTesting == true)
                {
                    try
                    {
                        BitmapSource Image1 = cameraStreaming1?.LastFrame;
                        BitmapSource Image2 = cameraStreaming2?.LastFrame;
                        BitmapSource Image3 = cameraStreaming3?.LastFrame;
                        BitmapSource Image4 = cameraStreaming4?.LastFrame;
                        if (IcopModel.LoadImageTestAuto(Image1, Image2, Image3, Image4))
                        {
                            tgbtUpCylinder.IsChecked = true;
                            el_UpCylinder.Fill = new SolidColorBrush(Colors.Yellow);
                            var ImageTest = IcopModel.StitchBitmaps(Position_Camera1, Position_Camera2, Position_Camera3, Position_Camera4, Image1, Image2, Image3, Image4);      //khâu ảnh
                            lbTestTimes.Content = IcopModel.RunTest(Position_Camera1, Position_Camera2, Position_Camera3, Position_Camera4, Image1, Image2, Image3, Image4).ToString("F2") + "s";
                            IcopModel.StateRunTest -= IcopModel_StateRunTest;
                            isTrue_NoQR = true;
                            foreach (var step in IcopModel.Steps)
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
                            if (isAutoTest && btAuto.IsChecked == true)
                            {
                                var resultList = IcopModel.Steps.Select(o => o.ResutlTest).ToList();
                                if (resultList.Contains(false))
                                {
                                    if (isTrue_NoQR == false)
                                    {
                                        Show_app_status(APP_STATUS.NG);
                                        //int _ModeNG = (int)Conveyor.StatusCommand.ModeNG;
                                        //SerialSend((char)_ModeNG + "");
                                        Console.WriteLine("NG");
                                        if (GR_PROGRAM.Visibility == Visibility.Collapsed)
                                        {
                                            GR_PROGRAM.Visibility = Visibility.Visible;
                                        }
                                    }
                                    else
                                    {
                                        Show_app_status(APP_STATUS.OK);
                                        //int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
                                        //SerialSend((char)_ModeOut + "");
                                        Console.WriteLine("OK");
                                        if (GR_PROGRAM.Visibility == Visibility.Visible)
                                        {
                                            GR_PROGRAM.Visibility = Visibility.Collapsed;
                                        }
                                    }

                                }
                                else
                                {
                                    Show_app_status(APP_STATUS.OK);
                                    //int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
                                    //SerialSend((char)_ModeOut + "");
                                    Console.WriteLine("OK");
                                    if (GR_PROGRAM.Visibility == Visibility.Visible)
                                    {
                                        GR_PROGRAM.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }
                            IcopModell.Steps.Clear();
                            dgrModelSteps.ItemsSource = IcopModell.Steps;
                            dgrModelSteps.SelectedIndex = 0;
                            dgrModelSteps.Focus();
                            foreach (var step in IcopModel.Steps)
                            {
                                if (step.ResutlTest == false)
                                {
                                    IcopModell.Steps.Add(step);
                                }
                            }

                            dgrModelSteps.ItemsSource = IcopModell.Steps;

                            if (modelWindow.WindowState == WindowState.Normal)
                            {
                                modelWindow.WindowState = WindowState.Minimized;

                                if (modelWindow.WindowState == WindowState.Minimized)
                                {
                                    modelWindow.WindowState = WindowState.Normal;
                                }
                            }
                            // Show_app_status(APP_STATUS.READY);
                            int Total = Convert.ToInt32(lbTotal_value.Content);
                            int NG_Number = Convert.ToInt32(lbNG_value.Content);
                            int OK_Number = Convert.ToInt32(lbOK_value.Content);
                            string LineName = lbLineName.Content.ToString();
                            IcopModel.SaveResultImage(ImageTest, Total, NG_Number, OK_Number, LineName);
                            IcopModel.SaveHistory_Complete += ModelProgram_SaveHistory_Complete;
                        }
                        Image1.Freeze();
                        Image2.Freeze();
                        Image3.Freeze();
                        Image4.Freeze();
                    }
                    catch
                    {

                    }
                }

            }


        }

        private void SetCamPropertiesTimer_Tick(object sender, EventArgs e)
        {
            SetCamPropertiesTimer.Stop();
            bool CamStarted = true;
            if (cameraStreaming1 != null && cameraStreaming2 != null && cameraStreaming3 != null && cameraStreaming4 != null)
            {
                CamStarted = cameraStreaming1.IsStarted && cameraStreaming2.IsStarted && cameraStreaming3.IsStarted && cameraStreaming4.IsStarted;
                if (CamStarted && !CameraSetupDone)
                //     if ( !CameraSetupDone)
                {
                    if (File.Exists(DirectoryF.Setting + "\\CamSetting.txt"))
                    {
                        string[] SetCamera = File.ReadAllLines(DirectoryF.Setting + "\\CamSetting.txt");
                        string SetCamera1 = SetCamera[0];
                        string SetCamera2 = SetCamera[1];
                        string SetCamera3 = SetCamera[2];
                        string SetCamera4 = SetCamera[3];
                        cameraStreaming1.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera1);
                        cameraStreaming1.SetParammeter(cameraStreaming1.cameraSetting);
                        sl_Exposure.Value = cameraStreaming1.cameraSetting.Exposure;
                        sl_Focus.Value = cameraStreaming1.cameraSetting.Focus;
                        sl_Contrast.Value = cameraStreaming1.cameraSetting.Contrast;
                        sl_Sharpness.Value = cameraStreaming1.cameraSetting.Sharpness;
                        sl_Zoom.Value = cameraStreaming1.cameraSetting.Zoom;
                        sl_Brightness.Value = cameraStreaming1.cameraSetting.Brightness;
                        cameraStreaming2.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera2);
                        cameraStreaming2.SetParammeter(cameraStreaming2.cameraSetting);
                        cameraStreaming3.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera3);
                        cameraStreaming3.SetParammeter(cameraStreaming3.cameraSetting);
                        cameraStreaming4.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera4);
                        cameraStreaming4.SetParammeter(cameraStreaming4.cameraSetting);
                    }
                    CameraSetupDone = true;
                    SetCamPropertiesTimer.Tick -= SetCamPropertiesTimer_Tick;
                    SetCamPropertiesTimer.Stop();
                }
                else
                {
                    SetCamPropertiesTimer.Start();
                }
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GR_CAMERA.Visibility = Visibility.Collapsed;
            GR_COM.Visibility = Visibility.Collapsed;
            GR_PROGRAM.Visibility = Visibility.Collapsed;
            GR_SETTING.Visibility = Visibility.Collapsed;
            cameraStreaming1 = new CameraStreaming(Camera1_Image, 1920, 1080, 0);
            cameraStreaming2 = new CameraStreaming(Camera2_Image, 1920, 1080, 1);
            cameraStreaming3 = new CameraStreaming(Camera3_Image, 1920, 1080, 2);
            cameraStreaming4 = new CameraStreaming(Camera4_Image, 1920, 1080, 3);
            SetCamera(cameraStreaming1, Camera1_Image, 0);
            SetCamera(cameraStreaming2, Camera2_Image, 1);
            SetCamera(cameraStreaming3, Camera3_Image, 2);
            SetCamera(cameraStreaming4, Camera4_Image, 3);
            if (File.Exists(DirectoryF.Setting + "\\Comsetting.txt"))
            {
                string[] Set_ComPort = File.ReadAllLines(DirectoryF.Setting + "\\Comsetting.txt");

                if (Set_ComPort[0].Replace("\"", "") == "COM1")
                {
                    MessageBox.Show("Choose COM Port, then save as new parameter. ");

                    SerialPort.Close();
                }
                else
                {
                    cbb_COM_PORT.Text = Set_ComPort[0].Replace("\"", "");
                    cbb_BAUD_RATE.Text = Set_ComPort[1];
                    if (Set_ComPort[2] == "0")
                    {
                        cbb_PARITY.Text = "None";
                    }
                    if (Set_ComPort[2] == "1")
                    {
                        cbb_PARITY.Text = "Odd";
                    }
                    if (Set_ComPort[2] == "2")
                    {
                        cbb_PARITY.Text = "Even";
                    }
                    if (Set_ComPort[2] == "3")
                    {
                        cbb_PARITY.Text = "Mark";
                    }
                    if (Set_ComPort[2] == "4")
                    {
                        cbb_PARITY.Text = "Space";
                    }
                    // cbb_PARITY.Text = Set_ComPort[2].ToString();
                    cbb_STOPBIT.Text = Set_ComPort[3];
                    tb_ReadTimeout.Text = Set_ComPort[4];
                    tb_WriteTimeout.Text = Set_ComPort[5];

                    if (cbb_COM_PORT.Text.Length > 1)
                    {
                        if (SerialPort.IsOpen)
                        {
                            el_COM.Fill = System.Windows.Media.Brushes.Green;
                        }
                        else
                        {
                            SerialPort.PortName = cbb_COM_PORT.Text;
                            SerialPort.BaudRate = Convert.ToInt32(cbb_BAUD_RATE.Text);
                            SerialPort.Parity = (Parity)Enum.Parse(typeof(Parity), cbb_PARITY.Text);
                            SerialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbb_STOPBIT.Text);
                            SerialPort.ReadTimeout = Convert.ToInt32(tb_ReadTimeout.Text);
                            SerialPort.WriteTimeout = Convert.ToInt32(tb_WriteTimeout.Text);
                            try
                            {
                                SerialPort.Open();
                                if (SerialPort.IsOpen)
                                {
                                    el_COM.Fill = System.Windows.Media.Brushes.Green;
                                }
                            }
                            catch (Exception err)
                            {
                                el_COM.Fill = System.Windows.Media.Brushes.Gray;
                                Console.WriteLine(err.Message);
                            }
                            SerialPort.DataReceived += new SerialDataReceivedEventHandler(RecievedData_);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(" Set the parameter for the COM.");
            }

            if (File.Exists(DirectoryF.Setting + "\\SetPositionCam.txt"))
            {
                {
                    string[] Set_CPositionCam = File.ReadAllLines(DirectoryF.Setting + "\\SetPositionCam.txt");
                    if (Int32.TryParse(Set_CPositionCam[0], out int cam0ps)) cbbCam1.SelectedIndex = cam0ps - 1;
                    if (Int32.TryParse(Set_CPositionCam[1], out int cam1ps)) cbbCam2.SelectedIndex = cam1ps - 1;
                    if (Int32.TryParse(Set_CPositionCam[2], out int cam2ps)) cbbCam3.SelectedIndex = cam2ps - 1;
                    if (Int32.TryParse(Set_CPositionCam[3], out int cam3ps)) cbbCam4.SelectedIndex = cam3ps - 1;
                    if (Camera1_Image != null && Camera2_Image != null && Camera3_Image != null && Camera4_Image != null)
                    {
                        int Position_Camera1 = int.Parse(cbbCam1.Text.ToString());
                        int Position_Camera2 = int.Parse(cbbCam2.Text.ToString());
                        int Position_Camera3 = int.Parse(cbbCam3.Text.ToString());
                        int Position_Camera4 = int.Parse(cbbCam4.Text.ToString());
                        if (cbbCam1.Text == null || cbbCam2.Text == null || cbbCam3.Text == null || cbbCam4.Text == null)
                        {
                            MessageBox.Show(" Set the position for the camera ");
                        }
                        else
                        {
                            if (Position_Camera1 != Position_Camera2 && Position_Camera1 != Position_Camera3 && Position_Camera1 != Position_Camera4
                                                                     && Position_Camera2 != Position_Camera3 && Position_Camera2 != Position_Camera4
                                                                                                             && Position_Camera3 != Position_Camera4)
                            {
                                SetCamPosition(cameraStreaming1, Position_Camera1);

                                // lbCam1.Content = "CAM " + Position_Camera1;

                                SetCamPosition(cameraStreaming2, Position_Camera2);
                                //   lbCam2.Content = "CAM " + Position_Camera2;

                                SetCamPosition(cameraStreaming3, Position_Camera3);
                                //  lbCam3.Content = "CAM " + Position_Camera3;

                                SetCamPosition(cameraStreaming4, Position_Camera4);
                                //  lbCam4.Content = "CAM " + Position_Camera4;

                                lb_Position.Content = "";
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(" Set the parameter for the camera.");
            }
            string nowyear = DateTime.Now.ToString("yyyy");
            string nowmonth = DateTime.Now.ToString("MM");
            string nowday = DateTime.Now.ToString("dd");
            if (File.Exists(DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\NumberResult.txt"))
            {
                string[] Number = File.ReadAllLines(DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\NumberResult.txt");
                string NumberNG = Number[0];
                string NumberOK = Number[1];
                string NumberTotal = Number[2];
                string LineName = Number[3];
                lbTotal_value.Content = NumberTotal;
                lbNG_value.Content = NumberNG;
                lbOK_value.Content = NumberOK;
                lbLineName.Content = LineName.ToString().Replace('"', ' ');
                IcopModel.statitis.NG_Number = Int32.Parse(NumberNG);
                IcopModel.statitis.OK_Number = Int32.Parse(NumberOK);
                IcopModel.statitis.Total = Int32.Parse(NumberTotal);

            }
            Show_app_status(APP_STATUS.READY);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lbTime.Content = DateTime.Now.ToString("yyyy/MM/dd  hh:mm:ss");
        }

        private void timer_Tick1(object sender, EventArgs e)
        {
            if (BlinkOn)
            {
                lbHelp.Foreground = new SolidColorBrush(Colors.Black);
                lbHelp.Background = new SolidColorBrush(Colors.Yellow);
                lbBassing.Foreground = new SolidColorBrush(Colors.Black);
                lbBassing.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                lbHelp.Foreground = new SolidColorBrush(Colors.Black);
                lbHelp.Background = new SolidColorBrush(Colors.Red);
                lbBassing.Foreground = new SolidColorBrush(Colors.Black);
                lbBassing.Background = new SolidColorBrush(Colors.GreenYellow);
            }
            BlinkOn = !BlinkOn;
        }

        private void ModelWindow_ModelClose(object sender, EventArgs e)
        {

        }

        public async void SetCam(object sender, EventArgs e)
        {

            await Task.Delay(1500);
            if (File.Exists(DirectoryF.Setting + "\\CamSetting.txt"))
            {
                string[] SetCamera = File.ReadAllLines(DirectoryF.Setting + "\\CamSetting.txt");
                string SetCamera1 = SetCamera[0];
                string SetCamera2 = SetCamera[1];
                string SetCamera3 = SetCamera[2];
                string SetCamera4 = SetCamera[3];
                cameraStreaming1.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera1);
                cameraStreaming1.SetParammeter(cameraStreaming1.cameraSetting);
                sl_Exposure.Value = cameraStreaming1.cameraSetting.Exposure;
                sl_Focus.Value = cameraStreaming1.cameraSetting.Focus;
                //  sl_Saturation.Value = cameraStreaming1.cameraSetting.Saturation;
                sl_Contrast.Value = cameraStreaming1.cameraSetting.Contrast;
                sl_Sharpness.Value = cameraStreaming1.cameraSetting.Sharpness;
                sl_Zoom.Value = cameraStreaming1.cameraSetting.Zoom;
                sl_Brightness.Value = cameraStreaming1.cameraSetting.Brightness;

                cameraStreaming2.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera2);
                cameraStreaming2.SetParammeter(cameraStreaming2.cameraSetting);
                sl_Exposure.Value = cameraStreaming2.cameraSetting.Exposure;
                sl_Focus.Value = cameraStreaming2.cameraSetting.Focus;
                //  sl_Saturation.Value = cameraStreaming2.cameraSetting.Saturation;
                sl_Contrast.Value = cameraStreaming2.cameraSetting.Contrast;
                sl_Sharpness.Value = cameraStreaming2.cameraSetting.Sharpness;
                sl_Zoom.Value = cameraStreaming2.cameraSetting.Zoom;
                sl_Brightness.Value = cameraStreaming2.cameraSetting.Brightness;

                cameraStreaming3.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera3);
                cameraStreaming3.SetParammeter(cameraStreaming3.cameraSetting);
                sl_Exposure.Value = cameraStreaming3.cameraSetting.Exposure;
                sl_Focus.Value = cameraStreaming3.cameraSetting.Focus;
                //sl_Saturation.Value = cameraStreaming3.cameraSetting.Saturation;
                sl_Contrast.Value = cameraStreaming3.cameraSetting.Contrast;
                sl_Sharpness.Value = cameraStreaming3.cameraSetting.Sharpness;
                sl_Zoom.Value = cameraStreaming3.cameraSetting.Zoom;
                sl_Brightness.Value = cameraStreaming3.cameraSetting.Brightness;

                cameraStreaming4.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera4);
                cameraStreaming4.SetParammeter(cameraStreaming4.cameraSetting);
                sl_Exposure.Value = cameraStreaming4.cameraSetting.Exposure;
                sl_Focus.Value = cameraStreaming4.cameraSetting.Focus;
                // sl_Saturation.Value = cameraStreaming4.cameraSetting.Saturation;
                sl_Contrast.Value = cameraStreaming4.cameraSetting.Contrast;
                sl_Sharpness.Value = cameraStreaming4.cameraSetting.Sharpness;
                sl_Zoom.Value = cameraStreaming4.cameraSetting.Zoom;
                sl_Brightness.Value = cameraStreaming4.cameraSetting.Brightness;

            }

        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SerialPort.IsOpen) SerialPort.Close();
            await cameraStreaming1?.Stop();
            cameraStreaming2?.Stop();
            cameraStreaming3?.Stop();
            cameraStreaming4?.Stop();
            Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DirectoryF.TryCreateFolders())
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    if (dialog.SelectedPath != null)
                    {
                        DirectoryF.MainFolderName = dialog.SelectedPath;
                        DirectoryF.SAVE();
                        DirectoryF.TryCreateFolders();
                    }
                }
            }
            if (File.Exists("model.txt"))
            {
                string JsonStr = File.ReadAllText("model.txt");
                IcopModel = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr);
                LoadModel();
            }

            var AllModelFolder = Directory.GetDirectories(DirectoryF.Model);
            for (int i = 0; i < AllModelFolder.Count(); i++)
            {
                var foldername = AllModelFolder[i].Split('\\');
                AllModelFolder[i] = foldername[foldername.Count() - 1];
            }
            cbbModels.Items.Clear();
            cbbModels.ItemsSource = AllModelFolder;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Image1_Canvas.Children?.Clear();
            Camera1_Canvas.Children?.Clear();
            foreach (var step in IcopModel.Steps)
            {
                //step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                step.PlaceIn(Image1_Canvas, Camera1_Canvas);
            }
        }

        private void ImageModelCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CurrentStep?.PlaceFuncLabel(ImageModelCanvas);
        }

        /////////////////////////////////////////////////////////

        #region CAMERA
        private void slCamsetting_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CheckBox_SetCamera.IsChecked == false)
            {
                cbbCamera.IsEnabled = true;
            }
            if (CheckBox_SetCamera.IsChecked == true)
            {
                cbbCamera.IsEnabled = false;
                var Control = sender as Slider;
                switch (Control.Name)
                {
                    case "sl_Exposure":
                        cameraStreaming1?.SetParammeter(CameraStreaming.VideoProperties.Exposure, (int)(sender as Slider).Value);
                        cameraStreaming2?.SetParammeter(CameraStreaming.VideoProperties.Exposure, (int)(sender as Slider).Value);
                        cameraStreaming3?.SetParammeter(CameraStreaming.VideoProperties.Exposure, (int)(sender as Slider).Value);
                        cameraStreaming4?.SetParammeter(CameraStreaming.VideoProperties.Exposure, (int)(sender as Slider).Value);
                        break;
                    case "sl_Focus":
                        cameraStreaming1?.SetParammeter(CameraStreaming.VideoProperties.Focus, (int)(sender as Slider).Value);
                        cameraStreaming2?.SetParammeter(CameraStreaming.VideoProperties.Focus, (int)(sender as Slider).Value);
                        cameraStreaming3?.SetParammeter(CameraStreaming.VideoProperties.Focus, (int)(sender as Slider).Value);
                        cameraStreaming4?.SetParammeter(CameraStreaming.VideoProperties.Focus, (int)(sender as Slider).Value);
                        break;
                    case "sl_WhiteBalance":
                        cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.WhiteBalance, (int)(sender as Slider).Value);
                        cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.WhiteBalance, (int)(sender as Slider).Value);
                        cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.WhiteBalance, (int)(sender as Slider).Value);
                        cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.WhiteBalance, (int)(sender as Slider).Value);
                        break;
                    case "sl_Zoom":
                        cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Zoom, (int)(sender as Slider).Value);
                        cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Zoom, (int)(sender as Slider).Value);
                        cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Zoom, (int)(sender as Slider).Value);
                        cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Zoom, (int)(sender as Slider).Value);
                        break;
                    case "sl_Brightness":
                        cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Brightness, (int)(sender as Slider).Value);
                        cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Brightness, (int)(sender as Slider).Value);
                        cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Brightness, (int)(sender as Slider).Value);
                        cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Brightness, (int)(sender as Slider).Value);
                        break;
                    case "sl_Contrast":
                        cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Contrast, (int)(sender as Slider).Value);
                        cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Contrast, (int)(sender as Slider).Value);
                        cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Contrast, (int)(sender as Slider).Value);
                        cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Contrast, (int)(sender as Slider).Value);
                        break;
                    case "sl_Sharpness":
                        cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Sharpness, (int)(sender as Slider).Value);
                        cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Sharpness, (int)(sender as Slider).Value);
                        cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Sharpness, (int)(sender as Slider).Value);
                        cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Sharpness, (int)(sender as Slider).Value);
                        break;
                    //case "sl_Saturation":
                    //    cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Satuation, (int)(sender as Slider).Value);
                    //    cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Satuation, (int)(sender as Slider).Value);
                    //    cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Satuation, (int)(sender as Slider).Value);
                    //    cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Satuation, (int)(sender as Slider).Value);
                    //    break;
                    default:
                        break;
                }
            }
            else
            {
                if (cbbCamera.Text == "CAM1")
                {
                    var Control = sender as Slider;
                    switch (Control.Name)
                    {
                        case "sl_Exposure":
                            cameraStreaming1?.SetParammeter(CameraStreaming.VideoProperties.Exposure, (int)(sender as Slider).Value);

                            break;
                        case "sl_Focus":
                            cameraStreaming1?.SetParammeter(CameraStreaming.VideoProperties.Focus, (int)(sender as Slider).Value);

                            break;
                        case "sl_WhiteBalance":
                            cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.WhiteBalance, (int)(sender as Slider).Value);

                            break;
                        case "sl_Zoom":
                            cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Zoom, (int)(sender as Slider).Value);

                            break;
                        case "sl_Brightness":
                            cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Brightness, (int)(sender as Slider).Value);
                            break;
                        case "sl_Contrast":
                            cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Contrast, (int)(sender as Slider).Value);
                            break;
                        case "sl_Sharpness":
                            cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Sharpness, (int)(sender as Slider).Value);
                            break;
                        //case "sl_Saturation":
                        //    cameraStreaming1.SetParammeter(CameraStreaming.VideoProperties.Satuation, (int)(sender as Slider).Value);
                        //    break;
                        default:
                            break;
                    }
                }

                if (cbbCamera.Text == "CAM2")
                {
                    var Control = sender as Slider;
                    switch (Control.Name)
                    {
                        case "sl_Exposure":
                            cameraStreaming2?.SetParammeter(CameraStreaming.VideoProperties.Exposure, (int)(sender as Slider).Value);
                            break;
                        case "sl_Focus":
                            cameraStreaming2?.SetParammeter(CameraStreaming.VideoProperties.Focus, (int)(sender as Slider).Value);
                            break;
                        case "sl_WhiteBalance":
                            cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.WhiteBalance, (int)(sender as Slider).Value);
                            break;
                        case "sl_Zoom":
                            cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Zoom, (int)(sender as Slider).Value);
                            break;
                        case "sl_Brightness":
                            cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Brightness, (int)(sender as Slider).Value);
                            break;
                        case "sl_Contrast":
                            cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Contrast, (int)(sender as Slider).Value);
                            break;
                        case "sl_Sharpness":
                            cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Sharpness, (int)(sender as Slider).Value);
                            break;
                        //case "sl_Saturation":
                        //    cameraStreaming2.SetParammeter(CameraStreaming.VideoProperties.Satuation, (int)(sender as Slider).Value);
                        //    break;
                        default:
                            break;
                    }
                }

                if (cbbCamera.Text == "CAM3")
                {
                    var Control = sender as Slider;
                    switch (Control.Name)
                    {
                        case "sl_Exposure":
                            cameraStreaming3?.SetParammeter(CameraStreaming.VideoProperties.Exposure, (int)(sender as Slider).Value);
                            break;
                        case "sl_Focus":
                            cameraStreaming3?.SetParammeter(CameraStreaming.VideoProperties.Focus, (int)(sender as Slider).Value);
                            break;
                        case "sl_WhiteBalance":
                            cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.WhiteBalance, (int)(sender as Slider).Value);
                            break;
                        case "sl_Zoom":
                            cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Zoom, (int)(sender as Slider).Value);
                            break;
                        case "sl_Brightness":
                            cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Brightness, (int)(sender as Slider).Value);
                            break;
                        case "sl_Contrast":
                            cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Contrast, (int)(sender as Slider).Value);
                            break;
                        case "sl_Sharpness":
                            cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Sharpness, (int)(sender as Slider).Value);
                            break;
                        //case "sl_Saturation":
                        //    cameraStreaming3.SetParammeter(CameraStreaming.VideoProperties.Satuation, (int)(sender as Slider).Value);
                        //    break;
                        default:
                            break;
                    }

                }

                if (cbbCamera.Text == "CAM4")
                {
                    var Control = sender as Slider;
                    switch (Control.Name)
                    {
                        case "sl_Exposure":
                            cameraStreaming4?.SetParammeter(CameraStreaming.VideoProperties.Exposure, (int)(sender as Slider).Value);

                            break;
                        case "sl_Focus":
                            cameraStreaming4?.SetParammeter(CameraStreaming.VideoProperties.Focus, (int)(sender as Slider).Value);

                            break;
                        case "sl_WhiteBalance":
                            cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.WhiteBalance, (int)(sender as Slider).Value);

                            break;
                        case "sl_Zoom":
                            cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Zoom, (int)(sender as Slider).Value);

                            break;
                        case "sl_Brightness":
                            cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Brightness, (int)(sender as Slider).Value);
                            break;
                        case "sl_Contrast":
                            cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Contrast, (int)(sender as Slider).Value);
                            break;
                        case "sl_Sharpness":
                            cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Sharpness, (int)(sender as Slider).Value);
                            break;
                        //case "sl_Saturation":
                        //    cameraStreaming4.SetParammeter(CameraStreaming.VideoProperties.Satuation, (int)(sender as Slider).Value);
                        //    break;
                        default:
                            break;
                    }
                }
            }
        }

        private async void SetCamera(CameraStreaming camera, System.Windows.Controls.Image cameraHolder, int id)
        {

            List<CameraDevice> cameras = new List<CameraDevice>(4);
            try
            {
                cameras = CameraDevicesEnumerator.GetAllConnectedCameras();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return;
            }
            if (cameras.Count < id + 1)
            {
                return;
            }
            else
            {
                if (cameras[id].Name.ToLower().Contains("virtual"))
                {
                    return;
                }
            }

            var selectedCameraDeviceId = cameras[id].OpenCvId;
            if (camera == null || camera.CameraDeviceId != selectedCameraDeviceId)
            {
                camera?.Dispose();
                camera = new CameraStreaming(imageControlForRendering: cameraHolder, frameWidth: 1920, frameHeight: 1080, cameraDeviceId: cameras[id].OpenCvId);
            }
            try
            {
                await camera.Start();

            }
            catch (Exception)
            {

            }
        }

        public void CameraDisponse()
        {
            //Camstopped?.Invoke(null, null);
        }

        private void CAMERA_Click(object sender, RoutedEventArgs e)
        {

            Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
            Storyboard sbop = Resources["OpenMenu"] as Storyboard;
            lbCam1.Visibility = Visibility.Collapsed;
            lbCam2.Visibility = Visibility.Collapsed;
            lbCam3.Visibility = Visibility.Collapsed;
            lbCam4.Visibility = Visibility.Collapsed;
            if (!GR_CAM_SHOW)
            {
                GR_CAMERA.Visibility = Visibility.Visible;
                sbop.Begin(GR_CAMERA);
                if (GR_COM_SHOW) sbcl.Begin(GR_COM);
                if (GR_PROGRAM_SHOW) sbcl.Begin(GR_PROGRAM);
                if (GR_SETTING_SHOW) sbcl.Begin(GR_SETTING);
                GR_PROGRAM_SHOW = false;
                GR_COM_SHOW = false;
                GR_SETTING_SHOW = false;
                if (GR_CAMERA.Visibility == Visibility.Visible)
                {
                    lbCam1.Visibility = Visibility.Visible;
                    lbCam2.Visibility = Visibility.Visible;
                    lbCam3.Visibility = Visibility.Visible;
                    lbCam4.Visibility = Visibility.Visible;
                }
            }
            else
            {
                sbcl.Begin(GR_CAMERA);
            }
            GR_CAM_SHOW = !GR_CAM_SHOW;
        }

        string jsonString1;
        string jsonString2;
        string jsonString3;
        string jsonString4;

        private void bt_SAVE_Click(object sender, RoutedEventArgs e)
        {

            jsonString1 = JsonSerializer.Serialize(cameraStreaming1.cameraSetting);
            jsonString2 = JsonSerializer.Serialize(cameraStreaming2.cameraSetting);
            jsonString3 = JsonSerializer.Serialize(cameraStreaming3.cameraSetting);
            jsonString4 = JsonSerializer.Serialize(cameraStreaming4.cameraSetting);
            string[] jsonString = { jsonString1, jsonString2, jsonString3, jsonString4 };
            var Setting = DirectoryF.Setting + "\\CamSetting.txt";
            File.WriteAllLines(Setting, jsonString);
        }

        private void bt_LOADFILE_Click(object sender, RoutedEventArgs e)
        {
            string[] Set_ComPort = File.ReadAllLines(DirectoryF.Setting + "\\Comsetting.txt");
            if (Set_ComPort[0].Replace("\"", "") == "COM1")
            {
                MessageBox.Show("Choose COM Port, then save as new parameter. ");

                SerialPort.Close();
            }
            else
            {
                cbb_COM_PORT.Text = Set_ComPort[0].Replace("\"", "");
                cbb_BAUD_RATE.Text = Set_ComPort[1];
                if (Set_ComPort[2] == "0")
                {
                    cbb_PARITY.Text = "None";
                }
                if (Set_ComPort[2] == "1")
                {
                    cbb_PARITY.Text = "Odd";
                }
                if (Set_ComPort[2] == "2")
                {
                    cbb_PARITY.Text = "Even";
                }
                if (Set_ComPort[2] == "3")
                {
                    cbb_PARITY.Text = "Mark";
                }
                if (Set_ComPort[2] == "4")
                {
                    cbb_PARITY.Text = "Space";
                }
                // cbb_PARITY.Text = Set_ComPort[2].ToString();
                cbb_STOPBIT.Text = Set_ComPort[3];
                tb_ReadTimeout.Text = Set_ComPort[4];
                tb_WriteTimeout.Text = Set_ComPort[5];
                if (cbb_COM_PORT.Text.Length > 1)
                {

                    if (SerialPort.IsOpen)
                    {

                    }
                    else
                    {
                        SerialPort.PortName = cbb_COM_PORT.Text;
                        SerialPort.BaudRate = Convert.ToInt32(cbb_BAUD_RATE.Text);
                        SerialPort.Parity = (Parity)Enum.Parse(typeof(Parity), cbb_PARITY.Text);
                        SerialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbb_STOPBIT.Text);
                        SerialPort.ReadTimeout = Convert.ToInt32(tb_ReadTimeout.Text);
                        SerialPort.WriteTimeout = Convert.ToInt32(tb_WriteTimeout.Text);
                        try
                        {
                            SerialPort.Open();
                            el_COM.Fill = System.Windows.Media.Brushes.Green;
                        }
                        catch (Exception err)
                        {
                            el_COM.Fill = System.Windows.Media.Brushes.Gray;
                            Console.WriteLine(err.Message);
                        }
                        SerialPort.DataReceived += new SerialDataReceivedEventHandler(RecievedData_);
                    }

                }
            }
            var OpenFile = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> Success = OpenFile.ShowDialog();
            if (Success == true)
            {
                if (File.Exists(DirectoryF.Setting + "\\CamSetting.txt"))
                {
                    //  string SetCamera = File.ReadAllText(DirectoryF.Setting + "\\Setting");
                    //string[] SetCamera = File.ReadAllLines(DirectoryF.Setting + "\\CamSetting.txt");
                    //string SetCamera1 = SetCamera[0];
                    //string SetCamera2 = SetCamera[1];
                    //string SetCamera3 = SetCamera[2];
                    //string SetCamera4 = SetCamera[3];
                    //cameraStreaming1.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera1);
                    //cameraStreaming1.SetParammeter(cameraStreaming1.cameraSetting);
                    //sl_Exposure.Value = cameraStreaming1.cameraSetting.Exposure;
                    //sl_Focus.Value = cameraStreaming1.cameraSetting.Focus;
                    //sl_Saturation.Value = cameraStreaming1.cameraSetting.Saturation;
                    //sl_Contrast.Value = cameraStreaming1.cameraSetting.Contrast;
                    //sl_Sharpness.Value = cameraStreaming1.cameraSetting.Sharpness;
                    //sl_Zoom.Value = cameraStreaming1.cameraSetting.Zoom;
                    //sl_Brightness.Value = cameraStreaming1.cameraSetting.Brightness;

                    //cameraStreaming2.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera2);
                    //cameraStreaming2.SetParammeter(cameraStreaming2.cameraSetting);
                    //sl_Exposure.Value = cameraStreaming2.cameraSetting.Exposure;
                    //sl_Focus.Value = cameraStreaming2.cameraSetting.Focus;
                    //sl_Saturation.Value = cameraStreaming2.cameraSetting.Saturation;
                    //sl_Contrast.Value = cameraStreaming2.cameraSetting.Contrast;
                    //sl_Sharpness.Value = cameraStreaming2.cameraSetting.Sharpness;
                    //sl_Zoom.Value = cameraStreaming2.cameraSetting.Zoom;
                    //sl_Brightness.Value = cameraStreaming2.cameraSetting.Brightness;

                    //cameraStreaming3.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera3);
                    //cameraStreaming3.SetParammeter(cameraStreaming3.cameraSetting);
                    //sl_Exposure.Value = cameraStreaming3.cameraSetting.Exposure;
                    //sl_Focus.Value = cameraStreaming3.cameraSetting.Focus;
                    //sl_Saturation.Value = cameraStreaming3.cameraSetting.Saturation;
                    //sl_Contrast.Value = cameraStreaming3.cameraSetting.Contrast;
                    //sl_Sharpness.Value = cameraStreaming3.cameraSetting.Sharpness;
                    //sl_Zoom.Value = cameraStreaming3.cameraSetting.Zoom;
                    //sl_Brightness.Value = cameraStreaming3.cameraSetting.Brightness;

                    //cameraStreaming4.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera4);
                    //cameraStreaming4.SetParammeter(cameraStreaming4.cameraSetting);
                    //sl_Exposure.Value = cameraStreaming4.cameraSetting.Exposure;
                    //sl_Focus.Value = cameraStreaming4.cameraSetting.Focus;
                    //sl_Saturation.Value = cameraStreaming4.cameraSetting.Saturation;
                    //sl_Contrast.Value = cameraStreaming4.cameraSetting.Contrast;
                    //sl_Sharpness.Value = cameraStreaming4.cameraSetting.Sharpness;
                    //sl_Zoom.Value = cameraStreaming4.cameraSetting.Zoom;
                    //sl_Brightness.Value = cameraStreaming4.cameraSetting.Brightness;
                }
            }
        }

        private void cbbCamera_DropDownClosed(object sender, EventArgs e)
        {
            string SetCamera1;
            string SetCamera2;
            string SetCamera3;
            string SetCamera4;
            if (!(bool)CheckBox_SetCamera.IsChecked)
            {
                if (File.Exists(DirectoryF.Setting + "\\CamSetting.txt"))
                {
                    //  string SetCamera = File.ReadAllText(DirectoryF.Setting + "\\Setting");
                    string[] SetCamera = File.ReadAllLines(DirectoryF.Setting + "\\CamSetting.txt");

                    SetCamera1 = SetCamera[0];
                    SetCamera2 = SetCamera[1];
                    SetCamera3 = SetCamera[2];
                    SetCamera4 = SetCamera[3];

                    switch ((sender as ComboBox).Text)
                    {
                        case "CAM1":
                            cameraStreaming1.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera1);
                            cameraStreaming1.SetParammeter(cameraStreaming1.cameraSetting);
                            sl_Exposure.Value = cameraStreaming1.cameraSetting.Exposure;
                            sl_Focus.Value = cameraStreaming1.cameraSetting.Focus;
                            //    sl_Saturation.Value = cameraStreaming1.cameraSetting.Saturation;
                            sl_Contrast.Value = cameraStreaming1.cameraSetting.Contrast;
                            sl_Sharpness.Value = cameraStreaming1.cameraSetting.Sharpness;
                            sl_Zoom.Value = cameraStreaming1.cameraSetting.Zoom;
                            sl_Brightness.Value = cameraStreaming1.cameraSetting.Brightness;
                            break;
                        case "CAM2":
                            cameraStreaming2.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera2);
                            cameraStreaming2.SetParammeter(cameraStreaming2.cameraSetting);
                            sl_Exposure.Value = cameraStreaming2.cameraSetting.Exposure;
                            sl_Focus.Value = cameraStreaming2.cameraSetting.Focus;
                            //     sl_Saturation.Value = cameraStreaming2.cameraSetting.Saturation;
                            sl_Contrast.Value = cameraStreaming2.cameraSetting.Contrast;
                            sl_Sharpness.Value = cameraStreaming2.cameraSetting.Sharpness;
                            sl_Zoom.Value = cameraStreaming2.cameraSetting.Zoom;
                            sl_Brightness.Value = cameraStreaming2.cameraSetting.Brightness;
                            break;
                        case "CAM3":
                            cameraStreaming3.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera3);
                            cameraStreaming3.SetParammeter(cameraStreaming3.cameraSetting);
                            sl_Exposure.Value = cameraStreaming3.cameraSetting.Exposure;
                            sl_Focus.Value = cameraStreaming3.cameraSetting.Focus;
                            //    sl_Saturation.Value = cameraStreaming3.cameraSetting.Saturation;
                            sl_Contrast.Value = cameraStreaming3.cameraSetting.Contrast;
                            sl_Sharpness.Value = cameraStreaming3.cameraSetting.Sharpness;
                            sl_Zoom.Value = cameraStreaming3.cameraSetting.Zoom;
                            sl_Brightness.Value = cameraStreaming3.cameraSetting.Brightness;
                            break;
                        case "CAM4":
                            cameraStreaming4.cameraSetting = JsonSerializer.Deserialize<CameraSetting>(SetCamera4);
                            cameraStreaming4.SetParammeter(cameraStreaming4.cameraSetting);
                            sl_Exposure.Value = cameraStreaming4.cameraSetting.Exposure;
                            sl_Focus.Value = cameraStreaming4.cameraSetting.Focus;
                            //     sl_Saturation.Value = cameraStreaming4.cameraSetting.Saturation;
                            sl_Contrast.Value = cameraStreaming4.cameraSetting.Contrast;
                            sl_Sharpness.Value = cameraStreaming4.cameraSetting.Sharpness;
                            sl_Zoom.Value = cameraStreaming4.cameraSetting.Zoom;
                            sl_Brightness.Value = cameraStreaming4.cameraSetting.Brightness;
                            break;
                        default:
                            break;
                    }
                }
                //  cbbCamera.IsEnabled = true;
                switch ((sender as ComboBox).Text)
                {
                    case "CAM1":
                        //cameraStreaming1.cameraSetting = cameraStreaming1.GetParammeter();
                        sl_Exposure.Value = cameraStreaming1.cameraSetting.Exposure;
                        sl_Focus.Value = cameraStreaming1.cameraSetting.Focus;
                        //    sl_Saturation.Value = cameraStreaming1.cameraSetting.Saturation;
                        sl_Contrast.Value = cameraStreaming1.cameraSetting.Contrast;
                        sl_Sharpness.Value = cameraStreaming1.cameraSetting.Sharpness;
                        sl_Zoom.Value = cameraStreaming1.cameraSetting.Zoom;
                        sl_Brightness.Value = cameraStreaming1.cameraSetting.Brightness;

                        break;
                    case "CAM2":
                        //cameraStreaming2.cameraSetting = cameraStreaming2.GetParammeter();
                        sl_Exposure.Value = cameraStreaming2.cameraSetting.Exposure;
                        sl_Focus.Value = cameraStreaming2.cameraSetting.Focus;
                        // sl_Saturation.Value = cameraStreaming2.cameraSetting.Saturation;
                        sl_Contrast.Value = cameraStreaming2.cameraSetting.Contrast;
                        sl_Sharpness.Value = cameraStreaming2.cameraSetting.Sharpness;
                        sl_Zoom.Value = cameraStreaming2.cameraSetting.Zoom;
                        sl_Brightness.Value = cameraStreaming2.cameraSetting.Brightness;
                        break;
                    case "CAM3":
                        //cameraStreaming3.cameraSetting = cameraStreaming3.GetParammeter();
                        sl_Exposure.Value = cameraStreaming3.cameraSetting.Exposure;
                        sl_Focus.Value = cameraStreaming3.cameraSetting.Focus;
                        //  sl_Saturation.Value = cameraStreaming3.cameraSetting.Saturation;
                        sl_Contrast.Value = cameraStreaming3.cameraSetting.Contrast;
                        sl_Sharpness.Value = cameraStreaming3.cameraSetting.Sharpness;
                        sl_Zoom.Value = cameraStreaming3.cameraSetting.Zoom;
                        sl_Brightness.Value = cameraStreaming3.cameraSetting.Brightness;
                        break;
                    case "CAM4":
                        sl_Exposure.Value = cameraStreaming4.cameraSetting.Exposure;
                        sl_Focus.Value = cameraStreaming4.cameraSetting.Focus;
                        //   sl_Saturation.Value = cameraStreaming4.cameraSetting.Saturation;
                        sl_Contrast.Value = cameraStreaming4.cameraSetting.Contrast;
                        sl_Sharpness.Value = cameraStreaming4.cameraSetting.Sharpness;
                        sl_Zoom.Value = cameraStreaming4.cameraSetting.Zoom;
                        sl_Brightness.Value = cameraStreaming4.cameraSetting.Brightness;
                        break;


                    default:
                        break;
                }
            }
        }

        private void CheckBox_SetCamera_Click(object sender, RoutedEventArgs e)
        {
            cbbCamera.IsEnabled = !(bool)(sender as CheckBox).IsChecked;
        }

        private void Position_Camera_Click(object sender, RoutedEventArgs e)
        {
            if (Camera1_Image != null && Camera2_Image != null && Camera3_Image != null && Camera4_Image != null)
            {
                int Position_Camera1 = int.Parse(cbbCam1.Text.ToString());
                int Position_Camera2 = int.Parse(cbbCam2.Text.ToString());
                int Position_Camera3 = int.Parse(cbbCam3.Text.ToString());
                int Position_Camera4 = int.Parse(cbbCam4.Text.ToString());
                if (Position_Camera1 != Position_Camera2 && Position_Camera1 != Position_Camera3 && Position_Camera1 != Position_Camera4
                    && Position_Camera2 != Position_Camera3 && Position_Camera2 != Position_Camera4
                    && Position_Camera3 != Position_Camera4)
                {
                    SetCamPosition(cameraStreaming1, Position_Camera1);
                    SetCamPosition(cameraStreaming2, Position_Camera2);
                    SetCamPosition(cameraStreaming3, Position_Camera3);
                    SetCamPosition(cameraStreaming4, Position_Camera4);
                    lb_Position.Content = "";
                }
                else
                {
                    lb_Position.Content = "Camera positions must not overlap.";
                }
                string jsonString1 = JsonSerializer.Serialize(Position_Camera1);
                string jsonString2 = JsonSerializer.Serialize(Position_Camera2);
                string jsonString3 = JsonSerializer.Serialize(Position_Camera3);
                string jsonString4 = JsonSerializer.Serialize(Position_Camera4);
                string[] jsonString = { jsonString1, jsonString2, jsonString3, jsonString4 };
                var Setting = DirectoryF.Setting + "\\SetPositionCam.txt";
                File.WriteAllLines(Setting, jsonString);
            }
        }

        private void SetCamPosition(CameraStreaming camera, int positision)
        {
            switch (positision)
            {
                case 1:
                    camera._imageControlForRendering = Camera1_Image;
                    break;
                case 2:
                    camera._imageControlForRendering = Camera2_Image;
                    break;
                case 3:
                    camera._imageControlForRendering = Camera3_Image;
                    break;
                case 4:
                    camera._imageControlForRendering = Camera4_Image;
                    break;
            }
        }

        #endregion


        ////////////////////////////////////////////////////////

        #region COM
        private void COM_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
            Storyboard sbop = Resources["OpenMenu"] as Storyboard;
            // var OpenFile = new Microsoft.Win32.OpenFileDialog();
            // Nullable<bool> Success = OpenFile.ShowDialog();
            //  if (Success == true)
            lbCam1.Visibility = Visibility.Collapsed;
            lbCam2.Visibility = Visibility.Collapsed;
            lbCam3.Visibility = Visibility.Collapsed;
            lbCam4.Visibility = Visibility.Collapsed;
            if (!GR_COM_SHOW)
            {
                GR_COM.Visibility = Visibility.Visible;
                sbop.Begin(GR_COM);
                if (GR_CAM_SHOW) sbcl.Begin(GR_CAMERA);
                if (GR_PROGRAM_SHOW) sbcl.Begin(GR_PROGRAM);
                if (GR_SETTING_SHOW) sbcl.Begin(GR_SETTING);
                GR_PROGRAM_SHOW = false;
                GR_CAM_SHOW = false;
                GR_SETTING_SHOW = false;
            }
            else
            {
                //GR_COM.Visibility = Visibility.Collapsed;
                sbcl.Begin(GR_COM);
            }
            GR_COM_SHOW = !GR_COM_SHOW;
        }

        #endregion 

        ///////////////////////////////////////////////////////

        #region function_buttons

        private void Program_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
            Storyboard sbop = Resources["OpenMenu"] as Storyboard;
            lbCam1.Visibility = Visibility.Collapsed;
            lbCam2.Visibility = Visibility.Collapsed;
            lbCam3.Visibility = Visibility.Collapsed;
            lbCam4.Visibility = Visibility.Collapsed;
            if (!GR_PROGRAM_SHOW)
            {
                GR_PROGRAM.Visibility = Visibility.Visible;
                sbop.Begin(GR_PROGRAM);
                if (GR_CAM_SHOW) sbcl.Begin(GR_CAMERA);
                if (GR_COM_SHOW) sbcl.Begin(GR_COM);
                if (GR_SETTING_SHOW) sbcl.Begin(GR_SETTING);
                GR_COM_SHOW = false;
                GR_CAM_SHOW = false;
                GR_SETTING_SHOW = false;
            }
            else
            {
                //GR_COM.Visibility = Visibility.Collapsed;
                sbcl.Begin(GR_PROGRAM);
            }
            GR_PROGRAM_SHOW = !GR_PROGRAM_SHOW;
        }

        private void SETTING_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
            Storyboard sbop = Resources["OpenMenu"] as Storyboard;
            lbCam1.Visibility = Visibility.Collapsed;
            lbCam2.Visibility = Visibility.Collapsed;
            lbCam3.Visibility = Visibility.Collapsed;
            lbCam4.Visibility = Visibility.Collapsed;
            if (!GR_SETTING_SHOW)
            {
                GR_SETTING.Visibility = Visibility.Visible;
                sbop.Begin(GR_SETTING);
                if (GR_CAM_SHOW) sbcl.Begin(GR_CAMERA);
                if (GR_PROGRAM_SHOW) sbcl.Begin(GR_PROGRAM);
                if (GR_COM_SHOW) sbcl.Begin(GR_COM);
                GR_PROGRAM_SHOW = false;
                GR_CAM_SHOW = false;
                GR_COM_SHOW = false;
            }
            else
            {
                //GR_COM.Visibility = Visibility.Collapsed;
                sbcl.Begin(GR_SETTING);
            }
            GR_SETTING_SHOW = !GR_SETTING_SHOW;
        }

        private void btPass_Click(object sender, RoutedEventArgs e)
        {
            Bybassing = !Bybassing;
            if (Bybassing)
            {
                bdBybassing.Visibility = Visibility.Visible;
                Show_app_status(APP_STATUS.PASSING);
            }
            else
            {
                bdBybassing.Visibility = Visibility.Collapsed;
                lbSttPassing.Visibility = Visibility.Collapsed;
            }
        }
     
        private void StatusTest_Load(object sender, EventArgs e)
        {
            LoadBar.Value = IcopModel.statustest;
            Console.WriteLine("Progress" + IcopModel.statustest);
        }

        public void LoadBar_Delay()
        {
            //   await Task.Delay(1000);
            LoadBar.Value = 0;
        }


        private void bt_Test_Click(object sender, RoutedEventArgs e)
        {
            Show_app_status(APP_STATUS.Testting);
            if (btAuto.IsChecked == false)
            {
                lbCam1.Visibility = Visibility.Collapsed;
                lbCam2.Visibility = Visibility.Collapsed;
                lbCam3.Visibility = Visibility.Collapsed;
                lbCam4.Visibility = Visibility.Collapsed;
                string[] Set_CPositionCam = File.ReadAllLines(DirectoryF.Setting + "\\SetPositionCam.txt");
                if (Int32.TryParse(Set_CPositionCam[0], out int cam0ps)) cbbCam1.SelectedIndex = cam0ps - 1;
                if (Int32.TryParse(Set_CPositionCam[1], out int cam1ps)) cbbCam2.SelectedIndex = cam1ps - 1;
                if (Int32.TryParse(Set_CPositionCam[2], out int cam2ps)) cbbCam3.SelectedIndex = cam2ps - 1;
                if (Int32.TryParse(Set_CPositionCam[3], out int cam3ps)) cbbCam4.SelectedIndex = cam3ps - 1;
                int Position_Camera1 = int.Parse(cbbCam1.Text.ToString());
                int Position_Camera2 = int.Parse(cbbCam2.Text.ToString());
                int Position_Camera3 = int.Parse(cbbCam3.Text.ToString());
                int Position_Camera4 = int.Parse(cbbCam4.Text.ToString());
                BitmapSource Image1 = cameraStreaming1?.LastFrame;
                BitmapSource Image2 = cameraStreaming2?.LastFrame;
                BitmapSource Image3 = cameraStreaming3?.LastFrame;
                BitmapSource Image4 = cameraStreaming4?.LastFrame;
                IcopModel.StatusTest_Loadd -= StatusTest_Load;
                IcopModel.StatusTest_Loadd += StatusTest_Load;
                if (IcopModel.LoadImageTestAuto(Image1, Image2, Image3, Image4))
                {
                    lbTestTimes.Content = (IcopModel.RunTest(Position_Camera1, Position_Camera2, Position_Camera3, Position_Camera4, Image1, Image2, Image3, Image4)).ToString("F2") + "s";
                }
                var ImageTest = IcopModel.StitchBitmaps(Position_Camera1, Position_Camera2, Position_Camera3, Position_Camera4, cameraStreaming1?.LastFrame, cameraStreaming2?.LastFrame, cameraStreaming3?.LastFrame, cameraStreaming4?.LastFrame);
                ImageProgram.Source = ImageTest;
                int Total = Convert.ToInt32(lbTotal_value.Content);
                int NG_Number = Convert.ToInt32(lbNG_value.Content);
                int OK_Number = Convert.ToInt32(lbOK_value.Content);
                string LineName = lbLineName.Content.ToString();
                IcopModel.SaveResultImage(ImageTest, Total, NG_Number, OK_Number, LineName);
                if (lbTesttingFinished.Content.ToString() == "NG")
                {
                    foreach (var step in IcopModel.Steps)
                    {
                        if (step.ResutlTest == false)
                        {
                            IcopModell.Steps.Clear();
                            dgrModelSteps.ItemsSource = IcopModell.Steps;
                        }
                    }
                    foreach (var step in IcopModel.Steps)
                    {
                        if (step.ResutlTest == false)
                        {
                            IcopModell.Steps.Add(step);
                        }
                    }

                }
                else if (lbTesttingFinished.Content.ToString() == "All")
                {
                    foreach (var step in IcopModel.Steps)
                    {
                        IcopModell.Steps.Clear();
                        dgrModelSteps.ItemsSource = IcopModell.Steps;
                    }

                    foreach (var step in IcopModel.Steps)
                    {
                        IcopModell.Steps.Add(step);
                    }
                }
                dgrModelSteps.ItemsSource = null;
                dgrModelSteps.ItemsSource = IcopModell.Steps;  //Show all steps Test
                dgrModelSteps.SelectedIndex = 0;
                dgrModelSteps.Focus();
                //    
                Image1.Freeze();
                Image2.Freeze();
                Image3.Freeze();
                Image4.Freeze();
            }
            else
            {
                MessageBox.Show("        Off auto mode             ");

            }
            //    Show_app_status(APP_STATUS.READY);
        }

        private void btHelp_Checked(object sender, RoutedEventArgs e)
        {
            bdHelp.Visibility = Visibility.Visible;

            if (bdBybassing.Visibility == Visibility.Visible)
                bdBybassing.Visibility = Visibility.Collapsed;
        }

        private void btHelp_Unchecked(object sender, RoutedEventArgs e)
        {
            bdHelp.Visibility = Visibility.Collapsed;
        }

        private void btAuto_Checked(object sender, RoutedEventArgs e)
        {
            btPause.IsChecked = false;
            btAuto.IsChecked = true;
            isAutoTest = true;
            lbCam1.Visibility = Visibility.Collapsed;
            lbCam2.Visibility = Visibility.Collapsed;
            lbCam3.Visibility = Visibility.Collapsed;
            lbCam4.Visibility = Visibility.Collapsed;
        }

        private void btAuto_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isTesting & IsInitialized)
            {
                btAuto.IsChecked = false;
                btPause.IsChecked = true;
            }

            lbCam1.Visibility = Visibility.Collapsed;
            lbCam2.Visibility = Visibility.Collapsed;
            lbCam3.Visibility = Visibility.Collapsed;
            lbCam4.Visibility = Visibility.Collapsed;
        }

        private void btTestActions_Click(object sender, RoutedEventArgs e)
        {
            if (SerialPort != null)
            {
                el_COM.Fill = System.Windows.Media.Brushes.Green;
                tgbtUpCylinder.IsChecked = false;
                el_UpCylinder.Fill = new SolidColorBrush(Colors.Gray);
                tgbtDownCylinder.IsChecked = false;
                el_DownCylinder.Fill = new SolidColorBrush(Colors.Gray);
                tgbtPassCylinder.IsChecked = false;
                el_PassCylinder.Fill = new SolidColorBrush(Colors.Gray);
                switch ((sender as Button).Name)
                {
                    case "btReset":
                        Bybassing = false;

                        if (Bybassing)
                        {
                            bdBybassing.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            bdBybassing.Visibility = Visibility.Collapsed;
                            Show_app_status(APP_STATUS.READY);
                        }
                        int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
                        SerialSend((char)_ModeOut + "");
                        tgbtDownCylinder.IsChecked = true;
                        el_DownCylinder.Fill = new SolidColorBrush(Colors.Yellow);
                        bdHelp.Visibility = Visibility.Collapsed;
                        break;
                    case "btPass": // hạ cylinder đầu ra
                        int _ModePass = (int)Conveyor.StatusCommand.ModePass;
                        SerialSend((char)_ModePass + "");
                        tgbtPassCylinder.IsChecked = true;
                        el_PassCylinder.Fill = new SolidColorBrush(Colors.Yellow);
                        break;
                    case "btUp": // nâng tất cả xylinder
                        int _ModeUp = (int)Conveyor.StatusCommand.ModeUp;
                        SerialSend((char)_ModeUp + "");
                        tgbtUpCylinder.IsChecked = true;
                        el_UpCylinder.Fill = new SolidColorBrush(Colors.Yellow);
                        break;
                    default:
                        break;
                }
            }
        }

        private void btOpenModelPage_Click(object sender, RoutedEventArgs e)
        {
            if (modelWindowClosed)
            {
                if (modelWindow.WindowState == WindowState.Minimized)
                {
                    modelWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    modelWindowClosed = true;
                    modelWindow = new Model();
                    modelWindow.ModelClose += ModelWindow_ModelClose;
                    modelWindow.Show();
                    lbCam1.Visibility = Visibility.Collapsed;
                    lbCam2.Visibility = Visibility.Collapsed;
                    lbCam3.Visibility = Visibility.Collapsed;
                    lbCam4.Visibility = Visibility.Collapsed;
                }

            }

            ////Process.Start("ICOP3_ModelEdit.exe");
            //Process objProcess = new Process();
            //objProcess.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "\\ICOP3_ModelEdit.exe";
            //var start = objProcess.Start();
            //Console.WriteLine(start);
            //

        }

        private void btTakePhoto_Click(object sender, RoutedEventArgs e)
        {
            lbCam1.Visibility = Visibility.Collapsed;
            lbCam2.Visibility = Visibility.Collapsed;
            lbCam3.Visibility = Visibility.Collapsed;
            lbCam4.Visibility = Visibility.Collapsed;
            string[] Set_CPositionCam = File.ReadAllLines(DirectoryF.Setting + "\\SetPositionCam.txt");
            if (Int32.TryParse(Set_CPositionCam[0], out int cam0ps)) cbbCam1.SelectedIndex = cam0ps - 1;
            if (Int32.TryParse(Set_CPositionCam[1], out int cam1ps)) cbbCam2.SelectedIndex = cam1ps - 1;
            if (Int32.TryParse(Set_CPositionCam[2], out int cam2ps)) cbbCam3.SelectedIndex = cam2ps - 1;
            if (Int32.TryParse(Set_CPositionCam[3], out int cam3ps)) cbbCam4.SelectedIndex = cam3ps - 1;
            int Position_Camera1 = int.Parse(cbbCam1.Text.ToString());
            int Position_Camera2 = int.Parse(cbbCam2.Text.ToString());
            int Position_Camera3 = int.Parse(cbbCam3.Text.ToString());
            int Position_Camera4 = int.Parse(cbbCam4.Text.ToString());
            if (Position_Camera1 == 1)
            {
                Cam1 = "cam1";
            }
            if (Position_Camera1 == 2)
            {
                Cam1 = "cam2";
            }
            if (Position_Camera1 == 3)
            {
                Cam1 = "cam3";
            }
            if (Position_Camera1 == 4)
            {
                Cam1 = "cam4";
            }
            /////////////
            if (Position_Camera2 == 1)
            {
                Cam2 = "cam1";
            }
            if (Position_Camera2 == 2)
            {
                Cam2 = "cam2";
            }
            if (Position_Camera2 == 3)
            {
                Cam2 = "cam3";
            }
            if (Position_Camera2 == 4)
            {
                Cam2 = "cam4";
            }

            ///////////
            if (Position_Camera3 == 1)
            {
                Cam3 = "cam1";
            }
            if (Position_Camera3 == 2)
            {
                Cam3 = "cam2";
            }
            if (Position_Camera3 == 3)
            {
                Cam3 = "cam3";
            }
            if (Position_Camera3 == 4)
            {
                Cam3 = "cam4";
            }
            //////////
            if (Position_Camera4 == 1)
            {
                Cam4 = "cam1";
            }
            if (Position_Camera4 == 2)
            {
                Cam4 = "cam2";
            }
            if (Position_Camera4 == 3)
            {
                Cam4 = "cam3";
            }
            if (Position_Camera4 == 4)
            {
                Cam4 = "cam4";
            }
            try
            {
                using (var fileStream = new FileStream(DirectoryF.Photo + "\\" + Cam1 + ".png", FileMode.Create, FileAccess.Write))
                {

                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(cameraStreaming1.LastFrame));
                    encoder.Save(fileStream);
                }
                using (var fileStream = new FileStream(DirectoryF.Photo + "\\" + Cam2 + ".png", FileMode.Create, FileAccess.Write))
                {

                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(cameraStreaming2.LastFrame));
                    encoder.Save(fileStream);
                }
                using (var fileStream = new FileStream(DirectoryF.Photo + "\\" + Cam3 + ".png", FileMode.Create, FileAccess.Write))
                {

                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(cameraStreaming3.LastFrame));
                    encoder.Save(fileStream);
                }
                using (var fileStream = new FileStream(DirectoryF.Photo + "\\" + Cam4 + ".png", FileMode.Create, FileAccess.Write))
                {

                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(cameraStreaming4.LastFrame));
                    encoder.Save(fileStream);
                }
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
                MessageBox.Show(" The image is being used in the Model so can't save new image. Reload the Project in the model creation window, then take photto again");
            }
            cameraStreaming1.LastFrame.Freeze();
            cameraStreaming2.LastFrame.Freeze();
            cameraStreaming3.LastFrame.Freeze();
            cameraStreaming4.LastFrame.Freeze();
        }

        #endregion

        //////////////////////////////////////////////////////
        #region Model 
        private void LoadModel()
        {
        }

        private void cbbModels_DropDownClosed(object sender, EventArgs e)
        {
            btAuto.IsEnabled = true;
            isAutoTest = true;
            var projectSeleted = cbbProjects.Text;
            var modelSeleted = cbbModels.Text;

            if (projectSeleted != null)
            {
                if (modelSeleted != null)
                {
                    if (File.Exists(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + DirectoryF.ICOP_EXT))
                    {
                        string JsonStr = File.ReadAllText(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + DirectoryF.ICOP_EXT);
                        IcopModel = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr);

                        string ImagePath = DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + ".png";
                        using (var bmpTemp = new Bitmap(ImagePath))
                        {
                            IcopModel.ImageSource = new Bitmap(bmpTemp);
                        }

                        IcopModel.MergeSource = IcopModel.ImageSource.ToBitmapSource();
                        //  IcopModel.MergeSource = new BitmapImage(new Uri((DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + ".png")));
                        IcopModel.MergeSource.Freeze();
                        ImageProgram.Source = IcopModel.MergeSource;
                        try
                        {
                            string JsonStr_Project = File.ReadAllText(DirectoryF.Model + "\\" + projectSeleted + "\\" + projectSeleted + DirectoryF.ICOP_EXT);
                            IcopProject = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr_Project);
                            IcopProjectt.Steps.Clear();
                            foreach (var step in IcopProject.Steps)
                            {
                                IcopProjectt.Steps.Insert(0, step);
                            }
                            foreach (var step in IcopProjectt.Steps)
                            {
                                IcopModel.Steps.Insert(0, step);
                            }
                            dgrModelSteps.ItemsSource = null;
                            dgrModelSteps.ItemsSource = IcopModel.Steps;
                            Image1_Canvas.Children.Clear();
                            Camera1_Canvas.Children.Clear();
                            foreach (var step in IcopModel.Steps)
                            {
                                step.PlaceIn(Image1_Canvas, Camera1_Canvas);
                            }
                            Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
                            Storyboard sbop = Resources["OpenMenu"] as Storyboard;
                            GR_PROGRAM.Visibility = Visibility.Visible;
                            sbop.Begin(GR_PROGRAM);
                            if (GR_CAM_SHOW) sbcl.Begin(GR_CAMERA);
                            if (GR_COM_SHOW) sbcl.Begin(GR_COM);
                            if (GR_SETTING_SHOW) sbcl.Begin(GR_SETTING);
                            GR_COM_SHOW = false;
                            GR_CAM_SHOW = false;
                            GR_SETTING_SHOW = false;
                            GR_PROGRAM_SHOW = true;
                            IcopModel.StateRunTest -= IcopModel_StateRunTest;
                            IcopModel.StateRunTest += IcopModel_StateRunTest;
                            IcopModel.SaveHistory_Complete -= ModelProgram_SaveHistory_Complete;
                            IcopModel.SaveHistory_Complete += ModelProgram_SaveHistory_Complete;
                        }
                        catch (FileNotFoundException ee)
                        {
                            MessageBox.Show(ee.Message + " Please select Project.");
                        }
                        dgrModelSteps.SelectedIndex = 0;
                        dgrModelSteps.Focus();
                    }
                }
            }
        }

        private void dgrModelSteps_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (btAuto.IsChecked == true)
            {
                if (e.Key == Key.Escape)
                {
                    int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
                    SerialSend((char)_ModeOut + "");
                    GR_PROGRAM.Visibility = Visibility.Collapsed;
                    Show_app_status(APP_STATUS.PASSING);
                }
            }
            if (e.Key == Key.Space)
            {
                int index = dgrModelSteps.SelectedIndex;
                if (index + 1 < dgrModelSteps.Items.Count)
                {
                    dgrModelSteps.SelectedIndex++;
                    dgrModelSteps.ScrollIntoView(dgrModelSteps.SelectedItem);
                }
                if (index + 1 == dgrModelSteps.Items.Count)
                {
                    int _ModePass = (int)Conveyor.StatusCommand.ModePass;
                    SerialSend((char)_ModePass + "");
                    GR_PROGRAM.Visibility = Visibility.Collapsed;
                    Show_app_status(APP_STATUS.PASSING);
                }
            }


        }

        private void dgrModelSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in IcopModel.Steps)
            {
                if (item.ResutlTest == true)
                {
                    item.Label.BorderBrush = new SolidColorBrush(Colors.Green);
                    item.LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Green);
                    item.Label.Foreground = new SolidColorBrush(Colors.Green);
                    item.LabelDisplay.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    item.Label.BorderBrush = new SolidColorBrush(Colors.Red);
                    item.LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Red);
                    item.Label.Foreground = new SolidColorBrush(Colors.Red);
                    item.LabelDisplay.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
            var grid = (sender as DataGrid);
            if (grid.SelectedItem != null)
            {
                try
                {
                    CurrentStep = (ModelLogic.Step)grid.SelectedItem;
                    CurrentStep.ImageSource.Freeze();
                    CurrentStep.ImageTestReuslt.Freeze();
                    ImageModel.Source = CurrentStep.ImageSource;
                    ImageTest.Source = CurrentStep.ImageTestReuslt;
                    CurrentStep.PlaceFuncLabel(ImageModelCanvas);
                    CurrentStep.Label.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.Label.Foreground = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.LabelDisplay.Foreground = new SolidColorBrush(Colors.Yellow);

                }
                catch (Exception)
                {

                }
            }
        }

        private void ModelProgram_SaveHistory_Complete(object sender, EventArgs e)
        {
            lbTotal_value.Content = IcopModel.statitis.Total.ToString();
            lbNG_value.Content = IcopModel.statitis.NG_Number.ToString();
            lbOK_value.Content = IcopModel.statitis.OK_Number.ToString();
        }

        private void Image1_Canvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IcopModel.Steps.Count > 0)
            {
                ModelLogic.Step stepToDelete = new ModelLogic.Step();
                foreach (var item in IcopModel.Steps)
                {
                    if (item.IsForcus)
                    {
                        stepToDelete = item;
                    }
                }
                if (e.Key == Key.Delete)
                {
                    Image1_Canvas.Children.Remove(stepToDelete.LabelDisplay);
                    Camera1_Canvas.Children.Remove(stepToDelete.Label);
                    IcopModel.Steps.Remove(stepToDelete);

                }
            }

            if (IcopProject.Steps.Count > 0)
            {
                ModelLogic.Step stepToDelete = new ModelLogic.Step();
                foreach (var item in IcopProject.Steps)
                {
                    if (item.IsForcus)
                    {
                        stepToDelete = item;
                    }
                }
                foreach (var item in IcopProject.Steps)
                {
                    if (item.IsForcus)
                    {
                        stepToDelete = item;
                    }
                }
                if (e.Key == Key.Delete)
                {
                    Image1_Canvas.Children.Remove(stepToDelete.LabelDisplay);
                    Camera1_Canvas.Children.Remove(stepToDelete.Label);
                    IcopProject.Steps.Remove(stepToDelete);
                }
            }
        }
        #endregion
        //////////////////////////////////////////////////////

        #region Project 

        private void cbbProjects_DropDownClosed(object sender, EventArgs e)
        {
            var projectSeleted = cbbProjects.Text;
            if (projectSeleted != null)
            {
                if (cbbProjects != null)
                {
                    var AllModelFolder = Directory.GetDirectories(DirectoryF.Model + "\\" + projectSeleted);
                    for (int i = 0; i < AllModelFolder.Count(); i++)
                    {
                        var foldername = AllModelFolder[i].Split('\\');
                        AllModelFolder[i] = foldername[foldername.Count() - 1];
                    }

                    cbbModels.ItemsSource = AllModelFolder;
                }
            }

        }

        private void cbbProjects_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var AllProjectFolder = Directory.GetDirectories(DirectoryF.Model);
            for (int i = 0; i < AllProjectFolder.Count(); i++)
            {
                var foldername = AllProjectFolder[i].Split('\\');
                AllProjectFolder[i] = foldername[foldername.Count() - 1];
            }
            cbbProjects.ItemsSource = AllProjectFolder;
        }
        #endregion


        /////////////////////////////////////////////////////

        #region Setting

        ICOP_3.ModelLogic.IcopModel IcopModel_Config = new ICOP_3.ModelLogic.IcopModel();
        private void tgbtTesttingFinished_MouseDown(object sender, MouseButtonEventArgs e)
        {

            //foreach (var step in IcopModel.Steps)
            //{
            //    IcopModel_Config.Steps.Add(step);
            //}
            if ((bool)tgbtTesttingFinished.IsChecked == true)
            {
                lbTesttingFinished.Background = new SolidColorBrush(Colors.LightGreen);
                lbTesttingFinished.Content = "NG";

                //if (lbTesttingFinished.Content.ToString() == "NG")
                //{
                //    foreach (var step in IcopModel.Steps)
                //    {
                //        if (step.ResutlTest == false)
                //        {
                //            IcopModel_Config.Steps.Clear();
                //            dgrModelSteps.ItemsSource = IcopModel_Config.Steps;
                //        }
                //    }
                //    foreach (var step in IcopModel.Steps)
                //    {
                //        if (step.ResutlTest == false)
                //        {
                //            IcopModel_Config.Steps.Add(step);

                //        }
                //    }
                //}
            }
            else
            {
                lbTesttingFinished.Background = new SolidColorBrush(Colors.Yellow);
                lbTesttingFinished.Content = "All";

                //if (lbTesttingFinished.Content.ToString() == "All")
                //{
                //    foreach (var step in IcopModel.Steps)
                //    {
                //        IcopModel_Config.Steps.Clear();
                //        dgrModelSteps.ItemsSource = IcopModel_Config.Steps;
                //    }

                //    foreach (var step in IcopModel.Steps)
                //    {
                //        IcopModel_Config.Steps.Add(step);
                //    }
                //}
            }
            dgrModelSteps.ItemsSource = IcopModel_Config.Steps;
        }

        private void btApllyLineName_Click(object sender, RoutedEventArgs e)
        {
            lbLineName.Content = tbSetNameLine.Text;
            btApllyLine.Visibility = Visibility.Hidden;
        }
        #endregion

        ///////////////////

        #region Status

        APP_STATUS app_status = APP_STATUS.BUSY;
        DispatcherTimer ShowReadyStatus = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };
        enum APP_STATUS
        {
            Testting = 0,
            OK = 1,
            NG = 2,
            READY = 3,
            BUSY = 4,
            PASSING = 5
        }

        private void ShowReadyStatus_Tick(object sender, object e)
        {
            //  Show_app_status(APP_STATUS.READY);
            ShowReadyStatus.Stop();
        }

        private void Show_app_status(APP_STATUS _STATUS)
        {
            ShowReadyStatus.Tick -= ShowReadyStatus_Tick;
            ShowReadyStatus.Tick += ShowReadyStatus_Tick;
            lbSttTesting.Visibility = Visibility.Collapsed;
            lbSttOK.Visibility = Visibility.Collapsed;
            lbSttNG.Visibility = Visibility.Collapsed;
            lbSttReady.Visibility = Visibility.Collapsed;
            lbSttBusy.Visibility = Visibility.Collapsed;
            lbSttPassing.Visibility = Visibility.Collapsed;
            app_status = _STATUS;
            switch (app_status)
            {
                case APP_STATUS.Testting:
                    lbSttTesting.Visibility = Visibility.Visible;
                    break;
                case APP_STATUS.OK:
                    lbSttOK.Visibility = Visibility.Visible;
                    ShowReadyStatus.Start();
                    break;
                case APP_STATUS.NG:
                    lbSttNG.Visibility = Visibility.Visible;
                    ShowReadyStatus.Start();
                    break;
                case APP_STATUS.READY:
                    lbSttReady.Visibility = Visibility.Visible;
                    break;
                case APP_STATUS.BUSY:
                    lbSttBusy.Visibility = Visibility.Visible;
                    break;
                case APP_STATUS.PASSING:
                    lbSttPassing.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void IcopModel_StateRunTest(object sender, EventArgs e)
        {
            // what state need check result ??? /////////////////////////////////////////

            //if (isAutoTest   && btAuto.IsChecked == true)
            //{
            //    var resultList = IcopModel.Steps.Select(o => o.ResutlTest).ToList();
            //    if (resultList.Contains(false))
            //    {
            //        Show_app_status(APP_STATUS.NG);
            //        int _ModeNG = (int)Conveyor.StatusCommand.ModeNG;
            //        SerialSend((char)_ModeNG + "");
            //        Console.WriteLine("NG");
            //        GR_PROGRAM.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        Show_app_status(APP_STATUS.OK);
            //        int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
            //        SerialSend((char)_ModeOut + "");
            //        Console.WriteLine("OK");
            //    }
            //}
        }

        private void Conveyor_ReadyTestRequest(object sender, EventArgs e)
        {
            tgbtUpCylinder.IsChecked = false;
            el_UpCylinder.Fill = new SolidColorBrush(Colors.Gray);
            tgbtDownCylinder.IsChecked = false;
            el_DownCylinder.Fill = new SolidColorBrush(Colors.Gray);
            tgbtPassCylinder.IsChecked = false;
            el_PassCylinder.Fill = new SolidColorBrush(Colors.Gray);
            if ((sender as string) == "Board Remover")
            {
                int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
                SerialSend((char)_ModeOut + "");
                tgbtPassCylinder.IsChecked = true;
                el_PassCylinder.Fill = new SolidColorBrush(Colors.Yellow);
                isTesting = false;
                // el_OutSensor.Fill = new SolidColorBrush(Colors.Red);
            }
        }

        bool isTrue_NoQR;

        //  bool isQRcode;
        public async void Runtest_Delay()
        {
            await Task.Delay(2500);
            GR_PROGRAM.Visibility = Visibility.Collapsed;
            string[] Set_CPositionCam = File.ReadAllLines(DirectoryF.Setting + "\\SetPositionCam.txt");
            if (Int32.TryParse(Set_CPositionCam[0], out int cam0ps)) cbbCam1.SelectedIndex = cam0ps - 1;
            if (Int32.TryParse(Set_CPositionCam[1], out int cam1ps)) cbbCam2.SelectedIndex = cam1ps - 1;
            if (Int32.TryParse(Set_CPositionCam[2], out int cam2ps)) cbbCam3.SelectedIndex = cam2ps - 1;
            if (Int32.TryParse(Set_CPositionCam[3], out int cam3ps)) cbbCam4.SelectedIndex = cam3ps - 1;
            int Position_Camera1 = int.Parse(cbbCam1.Text.ToString());
            int Position_Camera2 = int.Parse(cbbCam2.Text.ToString());
            int Position_Camera3 = int.Parse(cbbCam3.Text.ToString());
            int Position_Camera4 = int.Parse(cbbCam4.Text.ToString());
            if (!isTesting)
            {
                try
                {
                    BitmapSource Image1 = cameraStreaming1?.LastFrame;
                    BitmapSource Image2 = cameraStreaming2?.LastFrame;
                    BitmapSource Image3 = cameraStreaming3?.LastFrame;
                    BitmapSource Image4 = cameraStreaming4?.LastFrame;
                    if (IcopModel.LoadImageTestAuto(Image1, Image2, Image3, Image4))
                    {
                        tgbtUpCylinder.IsChecked = true;
                        el_UpCylinder.Fill = new SolidColorBrush(Colors.Yellow);
                        var ImageTest = IcopModel.StitchBitmaps(Position_Camera1, Position_Camera2, Position_Camera3, Position_Camera4, Image1, Image2, Image3, Image4);      //khâu ảnh
                        ImageProgram.Source = ImageTest;
                        lbTestTimes.Content = (2.5 + IcopModel.RunTest(Position_Camera1, Position_Camera2, Position_Camera3, Position_Camera4, Image1, Image2, Image3, Image4)).ToString("F2") + "s";
                        IcopModel.StateRunTest -= IcopModel_StateRunTest;
                        isTrue_NoQR = true;
                        foreach (var step in IcopModel.Steps)
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
                        if (lbTesttingFinished.Content.ToString() == "NG")
                        {
                            IcopModell.Steps.Clear();
                            foreach (var step in IcopModel.Steps)
                            {
                                if (step.ResutlTest == false)
                                {
                                    IcopModell.Steps.Add(step);
                                }
                            }
                            dgrModelSteps.ItemsSource = null;
                            dgrModelSteps.ItemsSource = IcopModell.Steps;
                        }

                        else if (lbTesttingFinished.Content.ToString() == "All")
                        {

                            IcopModell.Steps.Clear();
                            foreach (var step in IcopModel.Steps)
                            {
                                IcopModell.Steps.Add(step);
                            }
                            dgrModelSteps.ItemsSource = null;
                            dgrModelSteps.ItemsSource = IcopModell.Steps;
                        }
                        if (isAutoTest && btAuto.IsChecked == true)
                        {
                            var resultList = IcopModel.Steps.Select(o => o.ResutlTest).ToList();
                            if (resultList.Contains(false))
                            {
                                if (isTrue_NoQR == false)
                                {
                                    Show_app_status(APP_STATUS.NG);
                                    int _ModeNG = (int)Conveyor.StatusCommand.ModeNG;
                                    SerialSend((char)_ModeNG + "");
                                    Console.WriteLine("NG");
                                    Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
                                    Storyboard sbop = Resources["OpenMenu"] as Storyboard;
                                    GR_PROGRAM.Visibility = Visibility.Visible;
                                    sbop.Begin(GR_PROGRAM);
                                    GR_PROGRAM_SHOW = true;
                                }
                                else
                                {
                                    Show_app_status(APP_STATUS.OK);
                                    int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
                                    SerialSend((char)_ModeOut + "");
                                    Console.WriteLine("OK");
                                    Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
                                    Storyboard sbop = Resources["OpenMenu"] as Storyboard;
                                    GR_PROGRAM.Visibility = Visibility.Collapsed;
                                    sbcl.Begin(GR_PROGRAM);
                                    GR_PROGRAM_SHOW = false;
                                }
                            }
                            else
                            {
                                Show_app_status(APP_STATUS.OK);
                                int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
                                SerialSend((char)_ModeOut + "");
                                Console.WriteLine("OK");
                                Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
                                Storyboard sbop = Resources["OpenMenu"] as Storyboard;
                                GR_PROGRAM.Visibility = Visibility.Collapsed;
                                sbcl.Begin(GR_PROGRAM);
                                GR_PROGRAM_SHOW = false;
                            }

                        }

                        // Show_app_status(APP_STATUS.READY);
                        int Total = Convert.ToInt32(lbTotal_value.Content);
                        int NG_Number = Convert.ToInt32(lbNG_value.Content);
                        int OK_Number = Convert.ToInt32(lbOK_value.Content);
                        string LineName = lbLineName.Content.ToString();
                        IcopModel.SaveResultImage(ImageTest, Total, NG_Number, OK_Number, LineName);
                        IcopModel.SaveHistory_Complete += ModelProgram_SaveHistory_Complete;
                    }
                    dgrModelSteps.SelectedIndex = 0;
                    Keyboard.Focus(dgrModelSteps);
                    dgrModelSteps.Focus();
                    Image1.Freeze();
                    Image2.Freeze();
                    Image3.Freeze();
                    Image4.Freeze();
                    modelWindow.BringIntoView();

                    //if (WindowState == WindowState.Normal)
                    //{
                    //    modelWindow.WindowState = WindowState.Minimized;
                    //    if (modelWindow.WindowState == WindowState.Minimized)
                    //    {
                    //        modelWindow.WindowState = WindowState.Normal;
                    //    }
                    //}
                }
                catch
                {

                }
            }

            GC.Collect();
        }
        private void Conveyor_StartTestRequest(object sender, EventArgs e)
        {
            tgbtUpCylinder.IsChecked = false;
            el_UpCylinder.Fill = new SolidColorBrush(Colors.Gray);
            tgbtDownCylinder.IsChecked = false;
            el_DownCylinder.Fill = new SolidColorBrush(Colors.Gray);
            tgbtPassCylinder.IsChecked = false;
            el_PassCylinder.Fill = new SolidColorBrush(Colors.Gray);

            if (Bybassing)
            {
                int _ModeOut = (int)Conveyor.StatusCommand.ModeOut;
                SerialSend((char)_ModeOut + "");
                tgbtPassCylinder.IsChecked = true;
                el_PassCylinder.Fill = new SolidColorBrush(Colors.Yellow);
            }
            else if (btAuto.IsEnabled && (bool)btAuto.IsChecked)
            {

                if (cbbModels.SelectedItem != null && cbbProjects.SelectedItem != null)
                {
                    Show_app_status(APP_STATUS.Testting);
                    Runtest_Delay();
                }
            }
        }
        #endregion

        private void dgrModelSteps_Loaded(object sender, KeyEventArgs e)
        {
            var uiElement = e.OriginalSource as UIElement;
            var grid = (sender as DataGrid);
            //if (e.Key == Key.Space)
            //{
            //    int index = dgrModelSteps.SelectedIndex;
            //    if (index + 1 < dgrModelSteps.Items.Count - 1)
            //    {
            //        dgrModelSteps.SelectedIndex++;
            //    }
            //}
        }

        private void ClearNumberHistory_Click(object sender, RoutedEventArgs e)
        {
            string nowyear = DateTime.Now.ToString("yyyy");
            string nowmonth = DateTime.Now.ToString("MM");
            string nowday = DateTime.Now.ToString("dd");
            string NumberNG = "0";
            string NumberOK = "0";
            string NumberTotal = "0";
            string LineName_ = "Line Name";
            string[] Number = { NumberNG, NumberOK, NumberTotal, LineName_ };
            var path = DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\NumberResult.txt";
            if (File.Exists(DirectoryF.History + "\\" + nowyear + "\\" + nowmonth + "\\" + nowday + "\\NumberResult.txt"))
            {
                File.WriteAllLines(path, Number);
                lbOK_value.Content = "0";
                lbNG_value.Content = "0";
                lbTotal_value.Content = "0";
                lbLineName.Content = "Line Name";
            }
        }

        public DateTime DateTime = DateTime.Now;
        private bool modelWindowClosed = true;

        private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (DateSelect.SelectedDate != null)
            {
                DateTime = (DateTime)DateSelect.SelectedDate;
            }

        }

        private void ClearImageHistory_Click(object sender, RoutedEventArgs e)
        {
            var imagePath = DirectoryF.History + "\\" + DateTime.ToString("yyyy") + "\\" + DateTime.ToString("MM") + "\\" + DateTime.ToString("dd") + "\\" + DateTime.ToString("yyyyMMdd") + "_image";

            if (Directory.Exists(imagePath))
            {
                Directory.Delete(imagePath, true);
            }
        }

        private void bt_LoadModel_Click(object sender, RoutedEventArgs e)
        {
            btAuto.IsEnabled = true;
            isAutoTest = true;
            var projectSeleted = cbbProjects.Text;
            var modelSeleted = cbbModels.Text;

            if (projectSeleted != null)
            {
                if (modelSeleted != null)
                {
                    if (File.Exists(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + DirectoryF.ICOP_EXT))
                    {
                        string JsonStr = File.ReadAllText(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + DirectoryF.ICOP_EXT);
                        IcopModel = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr);

                        string ImagePath = DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + ".png";
                        using (var bmpTemp = new Bitmap(ImagePath))
                        {
                            IcopModel.ImageSource = new Bitmap(bmpTemp);
                        }

                        IcopModel.MergeSource = IcopModel.ImageSource.ToBitmapSource();
                        //  IcopModel.MergeSource = new BitmapImage(new Uri((DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + ".png")));
                        IcopModel.MergeSource.Freeze();
                        ImageProgram.Source = IcopModel.MergeSource;
                        try
                        {
                            string JsonStr_Project = File.ReadAllText(DirectoryF.Model + "\\" + projectSeleted + "\\" + projectSeleted + DirectoryF.ICOP_EXT);
                            IcopProject = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr_Project);
                            IcopProjectt.Steps.Clear();
                            foreach (var step in IcopProject.Steps)
                            {
                                IcopProjectt.Steps.Insert(0, step);
                            }
                            foreach (var step in IcopProjectt.Steps)
                            {
                                IcopModel.Steps.Insert(0, step);
                            }
                            dgrModelSteps.ItemsSource = null;
                            dgrModelSteps.ItemsSource = IcopModel.Steps;
                            Image1_Canvas.Children.Clear();
                            Camera1_Canvas.Children.Clear();
                            foreach (var step in IcopModel.Steps)
                            {
                                step.PlaceIn(Image1_Canvas, Camera1_Canvas);
                            }
                            Storyboard sbcl = Resources["CloseMenu"] as Storyboard;
                            Storyboard sbop = Resources["OpenMenu"] as Storyboard;
                            GR_PROGRAM.Visibility = Visibility.Visible;
                            sbop.Begin(GR_PROGRAM);
                            if (GR_CAM_SHOW) sbcl.Begin(GR_CAMERA);
                            if (GR_COM_SHOW) sbcl.Begin(GR_COM);
                            if (GR_SETTING_SHOW) sbcl.Begin(GR_SETTING);
                            GR_COM_SHOW = false;
                            GR_CAM_SHOW = false;
                            GR_SETTING_SHOW = false;
                            GR_PROGRAM_SHOW = true;
                            IcopModel.StateRunTest -= IcopModel_StateRunTest;
                            IcopModel.StateRunTest += IcopModel_StateRunTest;
                            IcopModel.SaveHistory_Complete -= ModelProgram_SaveHistory_Complete;
                            IcopModel.SaveHistory_Complete += ModelProgram_SaveHistory_Complete;
                        }
                        catch (FileNotFoundException ee)
                        {
                            MessageBox.Show(ee.Message + " Please select Project.");
                        }
                        dgrModelSteps.SelectedIndex = 0;
                        dgrModelSteps.Focus();
                    }
                }
            }
        }
    }
}


