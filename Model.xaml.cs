using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using OpenCvSharp.WpfExtensions;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using Rect = System.Windows.Rect;
using Window = System.Windows.Window;

namespace ICOP_3
{
    /// <summary>
    /// Interaction logic for Model.xaml
    /// </summary>
    public partial class Model : Window
    {
        public event EventHandler ModelClose;

        ModelLogic.IcopModel IcopModel = new ModelLogic.IcopModel();
        ModelLogic.IcopModel IcopModell = new ModelLogic.IcopModel();
        ModelLogic.IcopModel IcopProject = new ModelLogic.IcopModel();
        ModelLogic.IcopModel IcopProjectt = new ModelLogic.IcopModel();

        ModelLogic.IcopModel IcopProjectt_ = new ModelLogic.IcopModel();
        Password password = new Password();
        public List<string> Projects = new List<string>() { };
        public List<string> Models = new List<string>() { };
        public Model()
        {
            InitializeComponent();
            tgbProject.IsChecked = true;
            lbtgbtProject.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lbtgbtModel.Foreground = new SolidColorBrush(Colors.Gray);
            lb_Project.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lb_Model.Foreground = new SolidColorBrush(Colors.Gray);
            cbbAddNewModel.ItemsSource = Models;
            cbbAddNewProject.ItemsSource = Projects;
            dgrModelSteps.ItemsSource = IcopModel.Steps;
            // lbTime.Content = DateTime.Now.ToString();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            var gcTimer = new DispatcherTimer();
            gcTimer.Tick += (sender, e) => { GC.Collect(); };
            gcTimer.Interval = TimeSpan.FromSeconds(1);
            gcTimer.Start();


        }
        ModelLogic.Step currentStep = new ModelLogic.Step();
        ModelLogic.Step CurrentStep
        {
            get { return currentStep; }
            set
            {
                if (currentStep != value)
                {
                    currentStep = value;
                    cbb_Function.SelectedIndex = (int)currentStep.FUNC;
                    cbSkip.IsChecked = currentStep.Skip;

                    switch (value.FUNC)
                    {
                        case ModelLogic.Step.Functions.Matchtemplate:
                            {
                                GR_Capacitor.Visibility = Visibility.Hidden;
                                GR_Capacitor_Result.Visibility = Visibility.Hidden;

                                bdParameterMatchTemplate.Visibility = Visibility.Visible;
                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                bdReadQR.Visibility = Visibility.Collapsed;
                                bdParameterPolarized_Capacitors.Visibility = Visibility.Collapsed;

                            }
                            break;
                        case ModelLogic.Step.Functions.Polarized_Capacitors:
                            {
                                GR_Capacitor.Visibility = Visibility.Visible;
                                GR_Capacitor_Result.Visibility = Visibility.Visible;

                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                bdReadQR.Visibility = Visibility.Collapsed;
                                bdParameterPolarized_Capacitors.Visibility = Visibility.Visible;
                            }
                            break;
                        case ModelLogic.Step.Functions.Ceramic_Capacitors:
                            {
                                GR_Capacitor.Visibility = Visibility.Hidden;
                                GR_Capacitor_Result.Visibility = Visibility.Hidden;

                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                bdReadQR.Visibility = Visibility.Collapsed;
                                bdParameterPolarized_Capacitors.Visibility = Visibility.Collapsed;
                            }
                            break;
                        case ModelLogic.Step.Functions.Read_QR:
                            {
                                GR_Capacitor.Visibility = Visibility.Hidden;
                                GR_Capacitor_Result.Visibility = Visibility.Hidden;

                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                bdReadQR.Visibility = Visibility.Visible;
                                bdParameterPolarized_Capacitors.Visibility = Visibility.Collapsed;
                            }
                            break;
                    }
                }
            }
        }
        public DateTime TestTime { get; internal set; }
        public string ImageResultName { get; internal set; }
        public string ResutlTest { get; internal set; }
        void timer_Tick(object sender, EventArgs e)
        {
            lbTime.Content = DateTime.Now.ToString("yyyy/MM/dd  hh:mm:ss");
        }
        private void Modell_Click(object sender, RoutedEventArgs e)
        {
            GR_PROGRAM.Visibility = GR_PROGRAM.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            //var projectSeleted = cbbAddNewProject.Text;
            //if (projectSeleted != null)
            //{
            //    // cbbAddNewProject.Visibility = Visibility.Visible;
            //    if (cbbAddNewProject != null)
            //    {
            //        var AllModelFolder = Directory.GetDirectories(DirectoryF.Model);
            //        for (int i = 0; i < AllModelFolder.Count(); i++)
            //        {
            //            var foldername = AllModelFolder[i].Split('\\');
            //            AllModelFolder[i] = foldername[foldername.Count() - 1];
            //        }

            //        Projects = AllModelFolder.ToList();
            //        cbbAddNewProject.Items.Refresh();
            //        // cbbAddNewModel.ItemsSource = AllModelFolder;
            //    }
            //}
            //  GR_PROGRAM.Visibility = GR_PROGRAM.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

        }
        private void bt_LoadImage_Click(object sender, RoutedEventArgs e)
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
                        //Camera1_Image.Source = new BitmapImage(new Uri(item));
                        using (var bmpTemp = new Bitmap(item))
                        {
                            IcopModel.Image1 = new Bitmap(bmpTemp);
                        }
                        using (var bmpTemp = new Bitmap(item))
                        {
                            IcopProject.Image1 = new Bitmap(bmpTemp);
                        }

                        //IcopModel.Image1 = new Bitmap(item);
                        //IcopModel.PathImage1 = item;

                        //IcopProject.Image1 = new Bitmap(item);
                        //IcopProject.PathImage1 = item;
                    }
                    if (item.Contains("cam2"))
                    {
                        //Camera2_Image.Source = new BitmapImage(new Uri(item));
                        using (var bmpTemp = new Bitmap(item))
                        {
                            IcopModel.Image2 = new Bitmap(bmpTemp);
                        }
                        using (var bmpTemp = new Bitmap(item))
                        {
                            IcopProject.Image2 = new Bitmap(bmpTemp);
                        }

                        //IcopModel.Image2 = new Bitmap(item);
                        //IcopModel.PathImage2 = item;

                        //IcopProject.Image2 = new Bitmap(item);
                        //IcopProject.PathImage2 = item;
                    }
                    if (item.Contains("cam3"))
                    {
                        //Camera3_Image.Source = new BitmapImage(new Uri(item));
                        using (var bmpTemp = new Bitmap(item))
                        {
                            IcopModel.Image3 = new Bitmap(bmpTemp);
                        }
                        using (var bmpTemp = new Bitmap(item))
                        {
                            IcopProject.Image3 = new Bitmap(bmpTemp);
                        }

                        //IcopModel.PathImage3 = item;
                        //IcopModel.Image3 = new Bitmap(item);

                        //IcopProject.Image3 = new Bitmap(item);
                        //IcopProject.PathImage3 = item;
                    }
                    if (item.Contains("cam4"))
                    {
                        using (var bmpTemp = new Bitmap(item))
                        {
                            IcopModel.Image4 = new Bitmap(bmpTemp);
                        }
                        using (var bmpTemp = new Bitmap(item))
                        {
                            IcopProject.Image4 = new Bitmap(bmpTemp);
                        }

                        //Camera4_Image.Source = new BitmapImage(new Uri(item));
                        //IcopModel.PathImage4 = item;
                        //IcopModel.Image4 = new Bitmap(item);

                        //IcopProject.Image4 = new Bitmap(item);
                        //IcopProject.PathImage4 = item;
                    }
                }
                if (tgbtModel.IsChecked == true)
                {
                    IcopModel.MergedBitmaps();
                    Camera1_Image.Source = IcopModel.MergeSource;
                    Image1.Source = IcopModel.MergeSource;
                }

                if (tgbProject.IsChecked == true)
                {
                    IcopProject.MergedBitmaps();
                    Camera1_Image.Source = IcopProject.MergeSource;
                    Image1.Source = IcopProject.MergeSource;
                }
            }
        }
        private void LoadModel()
        {

            IcopModel.Image1 = new Bitmap(IcopModel.PathImage1);
            IcopModel.PathImage1 = IcopModel.PathImage1;

            IcopModel.Image2 = new Bitmap(IcopModel.PathImage2);
            IcopModel.PathImage2 = IcopModel.PathImage2;

            IcopModel.PathImage3 = IcopModel.PathImage3;
            IcopModel.Image3 = new Bitmap(IcopModel.PathImage3);

            IcopModel.PathImage4 = IcopModel.PathImage4;
            IcopModel.Image4 = new Bitmap(IcopModel.PathImage4);

            BitmapFrame frame1 = BitmapDecoder.Create(new Uri(IcopModel.PathImage1), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame2 = BitmapDecoder.Create(new Uri(IcopModel.PathImage2), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame3 = BitmapDecoder.Create(new Uri(IcopModel.PathImage3), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame4 = BitmapDecoder.Create(new Uri(IcopModel.PathImage4), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();

            int imageWidth = frame1.PixelWidth;
            int imageHeight = frame1.PixelHeight;

            // Draws the images into a DrawingVisual component
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(frame1, new Rect(0, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(frame2, new Rect(imageWidth, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(frame3, new Rect(0, imageHeight, imageWidth, imageHeight));
                drawingContext.DrawImage(frame4, new Rect(imageWidth, imageHeight, imageWidth, imageHeight));
            }

            // Converts the Visual (DrawingVisual) into a BitmapSource
            RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * 2, imageHeight * 2, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            IcopModel.MergeSource = bmp;
            Camera1_Image.Source = IcopModel.MergeSource;
            dgrModelSteps.ItemsSource = IcopModel.Steps;

            foreach (var step in IcopModel.Steps)
            {
                step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
            }
        }
        private void LoadProject()
        {
            IcopProject.Image1 = new Bitmap(IcopProject.PathImage1);
            IcopProject.PathImage1 = IcopProject.PathImage1;

            IcopProject.Image2 = new Bitmap(IcopProject.PathImage2);
            IcopProject.PathImage2 = IcopProject.PathImage2;

            IcopProject.PathImage3 = IcopProject.PathImage3;
            IcopProject.Image3 = new Bitmap(IcopProject.PathImage3);

            IcopProject.PathImage4 = IcopProject.PathImage4;
            IcopProject.Image4 = new Bitmap(IcopProject.PathImage4);

            BitmapFrame frame1 = BitmapDecoder.Create(new Uri(IcopProject.PathImage1), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame2 = BitmapDecoder.Create(new Uri(IcopProject.PathImage2), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame3 = BitmapDecoder.Create(new Uri(IcopProject.PathImage3), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame4 = BitmapDecoder.Create(new Uri(IcopProject.PathImage4), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();

            int imageWidth = frame1.PixelWidth;
            int imageHeight = frame1.PixelHeight;

            // Draws the images into a DrawingVisual component
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(frame1, new Rect(0, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(frame2, new Rect(imageWidth, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(frame3, new Rect(0, imageHeight, imageWidth, imageHeight));
                drawingContext.DrawImage(frame4, new Rect(imageWidth, imageHeight, imageWidth, imageHeight));
            }

            // Converts the Visual (DrawingVisual) into a BitmapSource
            RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * 2, imageHeight * 2, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            IcopProject.MergeSource = bmp;
            Camera1_Image.Source = IcopProject.MergeSource;
            dgrModelSteps.ItemsSource = IcopProject.Steps;

            foreach (var step in IcopProject.Steps)
            {
                step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ModelClose?.Invoke(this, new EventArgs());
        }
        #region button function
        private void btOpenNewModelPage_Click(object sender, RoutedEventArgs e)
        {
            bd_NewProject.Visibility = Visibility.Visible;


        }
        private void btAddNewModel_Click(object sender, RoutedEventArgs e)
        {
            bdAddNewModel.Visibility = Visibility.Visible;
            tbAddNewModel.Visibility = Visibility.Visible;
            btOK.Visibility = Visibility.Visible;

        }
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            bd_NewProject.Visibility = Visibility.Collapsed;

            bdAddNewModel.Visibility = Visibility.Collapsed;
        }
        private void bt_OK_Project_Click(object sender, RoutedEventArgs e)
        {
            //  lb_Project.Content = tbNewProjectSetting.Text;
            tgbProject.IsChecked = true;
            tgbtModel.IsChecked = false;

            lbtgbtProject.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lbtgbtModel.Foreground = new SolidColorBrush(Colors.Gray);

            lb_Project.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lb_Model.Foreground = new SolidColorBrush(Colors.Gray);

            bd_NewProject.Visibility = Visibility.Collapsed;
            Camera1_Canvas.Children.Clear();
            Image1_Canvas.Children.Clear();

            if (tbNewProjectSetting.Text.Length < 1)
            {
                MessageBox.Show("Invalid name");
                return;
            }

            var project = tbNewProjectSetting.Text;

            if (Directory.Exists(DirectoryF.Model + "\\" + project))
            {
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("Project already exists. Still continue to create?", " ", button);

                if (result == MessageBoxResult.Yes)
                {

                    IcopModel.Steps.Clear();

                    IcopProject = new ModelLogic.IcopModel()
                    {
                        Name = tbNewProjectSetting.Text,
                        Number_PBA = Convert.ToInt32(cbbNumberPBA.Text),
                        QR_Lenght = Int32.TryParse(tbQRLenght.Text, out int lenght) ? lenght : 0,

                    };
                    for (int i = 0; i < IcopProject.Number_PBA; i++)
                    {
                        IcopModel.Steps.Add(new ModelLogic.Step(IcopModel.Steps.Count + 1, "QR " + i.ToString(), Camera1_Canvas, Image1_Canvas, null)
                        {
                            PM = "P",
                            FUNC = ModelLogic.Step.Functions.Read_QR,
                        });
                        IcopProject.Steps.Add(new ModelLogic.Step(IcopProject.Steps.Count + 1, "QR " + i.ToString(), Camera1_Canvas, Image1_Canvas, null)
                        {
                            PM = "P",
                            FUNC = ModelLogic.Step.Functions.Read_QR,
                        });

                    }
                    foreach (var item in IcopModel.Steps)
                    {
                        item.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                    }
                    OpenFileDialog openImage = new OpenFileDialog();
                    openImage.Multiselect = true;
                    openImage.Title = "Select a picture";
                    if (openImage.ShowDialog() == true)
                    {
                        foreach (var item in openImage.FileNames)
                        {
                            if (item.Contains("cam1"))
                            {
                                //Camera1_Image.Source = new BitmapImage(new Uri(item));
                                IcopModel.Image1 = new Bitmap(item);
                                IcopModel.PathImage1 = item;
                                IcopProject.Image1 = new Bitmap(item);
                                IcopProject.PathImage1 = item;

                            }
                            if (item.Contains("cam2"))
                            {
                                //Camera2_Image.Source = new BitmapImage(new Uri(item));
                                IcopModel.Image2 = new Bitmap(item);
                                IcopModel.PathImage2 = item;
                                IcopProject.Image2 = new Bitmap(item);
                                IcopProject.PathImage2 = item;

                            }
                            if (item.Contains("cam3"))
                            {
                                //Camera3_Image.Source = new BitmapImage(new Uri(item));
                                IcopModel.PathImage3 = item;
                                IcopModel.Image3 = new Bitmap(item);
                                IcopProject.Image3 = new Bitmap(item);
                                IcopProject.PathImage3 = item;
                            }
                            if (item.Contains("cam4"))
                            {
                                //Camera4_Image.Source = new BitmapImage(new Uri(item));
                                IcopModel.PathImage4 = item;
                                IcopModel.Image4 = new Bitmap(item);
                                IcopProject.Image4 = new Bitmap(item);
                                IcopProject.PathImage4 = item;
                            }

                        }
                        tgbProject.IsChecked = true;
                        IcopProject.MergedBitmaps();
                        Camera1_Image.Source = IcopProject.MergeSource;
                        Image1.Source = IcopProject.MergeSource;
                    }
                    dgrModelSteps.ItemsSource = IcopModel.Steps;
                    DirectoryF.SaveProject(IcopProject, IcopProject.Name);

                    Projects.Add(IcopProject.Name);
                    //     cbbAddNewProject.Items.Refresh();
                    cbbAddNewProject.SelectedItem = IcopProject.Name;
                    lb_Project.Content = tbNewProjectSetting.Text;

                }


            }
            else
            {
                IcopModel.Steps.Clear();
                IcopProject = new ModelLogic.IcopModel()
                {
                    Name = tbNewProjectSetting.Text,
                    Number_PBA = Convert.ToInt32(cbbNumberPBA.Text),
                    QR_Lenght = Int32.TryParse(tbQRLenght.Text, out int lenght) ? lenght : 0,

                };
                for (int i = 0; i < IcopProject.Number_PBA; i++)
                {
                    IcopModel.Steps.Add(new ModelLogic.Step(IcopModel.Steps.Count + 1, "QR " + i.ToString(), Camera1_Canvas, Image1_Canvas, null)
                    {
                        PM = "P",
                        FUNC = ModelLogic.Step.Functions.Read_QR,
                    });
                    IcopProject.Steps.Add(new ModelLogic.Step(IcopProject.Steps.Count + 1, "QR " + i.ToString(), Camera1_Canvas, Image1_Canvas, null)
                    {
                        PM = "P",
                        FUNC = ModelLogic.Step.Functions.Read_QR,
                    });

                }
                foreach (var item in IcopModel.Steps)
                {
                    item.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                }
                OpenFileDialog openImage = new OpenFileDialog();
                openImage.Multiselect = true;
                openImage.Title = "Select a picture";
                if (openImage.ShowDialog() == true)
                {
                    foreach (var item in openImage.FileNames)
                    {
                        if (item.Contains("cam1"))
                        {
                            //Camera1_Image.Source = new BitmapImage(new Uri(item));
                            IcopModel.Image1 = new Bitmap(item);
                            IcopModel.PathImage1 = item;
                            IcopProject.Image1 = new Bitmap(item);
                            IcopProject.PathImage1 = item;

                        }
                        if (item.Contains("cam2"))
                        {
                            //Camera2_Image.Source = new BitmapImage(new Uri(item));
                            IcopModel.Image2 = new Bitmap(item);
                            IcopModel.PathImage2 = item;
                            IcopProject.Image2 = new Bitmap(item);
                            IcopProject.PathImage2 = item;

                        }
                        if (item.Contains("cam3"))
                        {
                            //Camera3_Image.Source = new BitmapImage(new Uri(item));
                            IcopModel.PathImage3 = item;
                            IcopModel.Image3 = new Bitmap(item);
                            IcopProject.Image3 = new Bitmap(item);
                            IcopProject.PathImage3 = item;
                        }
                        if (item.Contains("cam4"))
                        {
                            //Camera4_Image.Source = new BitmapImage(new Uri(item));
                            IcopModel.PathImage4 = item;
                            IcopModel.Image4 = new Bitmap(item);
                            IcopProject.Image4 = new Bitmap(item);
                            IcopProject.PathImage4 = item;
                        }

                    }
                    tgbProject.IsChecked = true;
                    IcopProject.MergedBitmaps();
                    Camera1_Image.Source = IcopProject.MergeSource;
                    Image1.Source = IcopProject.MergeSource;
                }
                dgrModelSteps.ItemsSource = IcopModel.Steps;
                DirectoryF.SaveProject(IcopProject, IcopProject.Name);

                lb_Project.Content = tbNewProjectSetting.Text;
                Projects.Add(IcopProject.Name);
                //    cbbAddNewProject.Items.Refresh();
                cbbAddNewProject.SelectedItem = IcopProject.Name;
            }

        }
        private void btOK_Model_Click(object sender, RoutedEventArgs e)
        {
            string project;
            if (IcopProject.Name == "New model")
            {
                MessageBox.Show("No projects yet.");
            }
            else
            {
                tgbProject.IsChecked = false;
                tgbtModel.IsChecked = true;
                lbtgbtProject.Foreground = new SolidColorBrush(Colors.Gray);
                lbtgbtModel.Foreground = new SolidColorBrush(Colors.OrangeRed);
                lb_Project.Foreground = new SolidColorBrush(Colors.Gray);
                lb_Model.Foreground = new SolidColorBrush(Colors.OrangeRed);

                if (tbAddNewModel.Text.Length < 1)
                {
                    MessageBox.Show("Invalid name.");
                    return;
                }

                if (tbNewProjectSetting.Text.Length > 1)
                {
                    project = tbNewProjectSetting.Text;
                }
                {
                    project = cbbAddNewProject.Text;
                }
                var model = tbAddNewModel.Text;
                if (Directory.Exists(DirectoryF.Model + "\\" + project + "\\" + model))
                {
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxResult result = MessageBox.Show("Model already exists. Still continue to create?", " ", button);
                    if (result == MessageBoxResult.Yes)
                    {
                        IcopProject.Steps.Clear();
                        foreach (var step in IcopModel.Steps)
                        {
                            if (step.PM == "P")
                            {
                                IcopProject.Steps.Add(step);
                            }
                        }

                        IcopModel = new ModelLogic.IcopModel()
                        {
                            Name = tbAddNewModel.Text,
                            Number_PBA = IcopProject.Number_PBA,
                            QR_Lenght = Int32.TryParse(tbQRLenght.Text, out int lenght) ? lenght : 0,
                        };


                        IcopProjectt.Steps.Clear();
                        foreach (ModelLogic.Step step in IcopProject.Steps)
                        {

                            IcopProjectt.Steps.Add(step);

                        }

                        IcopModell.Steps.Clear();
                        foreach (ModelLogic.Step step in IcopModel.Steps)
                        {
                            if (step.PM == "M")
                            {
                                IcopModell.Steps.Add(step);
                            }



                        }
                        IcopModel.Steps.Clear();
                        foreach (var step in IcopModell.Steps)
                        {
                            IcopModel.Steps.Add(step);
                        }

                        Camera1_Canvas.Children.Clear();
                        Image1_Canvas.Children.Clear();

                        //foreach (var item in IcopProject.Steps)
                        //{
                        //    IcopModel.Steps.Insert(0, item);
                        //}

                        foreach (var item in IcopModel.Steps)
                        {
                            item.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                        }
                        OpenFileDialog openImage = new OpenFileDialog();
                        openImage.Multiselect = true;
                        openImage.Title = "Select a picture";
                        if (openImage.ShowDialog() == true)
                        {
                            foreach (var item in openImage.FileNames)
                            {
                                if (item.Contains("cam1"))
                                {
                                    IcopModel.Image1 = new Bitmap(item);
                                    IcopModel.PathImage1 = item;

                                    IcopProject.Image1 = new Bitmap(item);
                                    IcopProject.PathImage1 = item;
                                }
                                if (item.Contains("cam2"))
                                {
                                    IcopModel.Image2 = new Bitmap(item);
                                    IcopModel.PathImage2 = item;

                                    IcopProject.Image2 = new Bitmap(item);
                                    IcopProject.PathImage2 = item;
                                }
                                if (item.Contains("cam3"))
                                {
                                    IcopModel.PathImage3 = item;
                                    IcopModel.Image3 = new Bitmap(item);

                                    IcopProject.Image3 = new Bitmap(item);
                                    IcopProject.PathImage3 = item;
                                }
                                if (item.Contains("cam4"))
                                {
                                    IcopModel.PathImage4 = item;
                                    IcopModel.Image4 = new Bitmap(item);

                                    IcopProject.Image4 = new Bitmap(item);
                                    IcopProject.PathImage4 = item;
                                }

                            }
                            if (tgbtModel.IsChecked == true)
                            {
                                IcopModel.MergedBitmaps();
                                Camera1_Image.Source = IcopModel.MergeSource;
                                Image1.Source = IcopModel.MergeSource;
                            }

                            if (tgbProject.IsChecked == true)
                            {
                                IcopProject.MergedBitmaps();
                                Camera1_Image.Source = IcopProject.MergeSource;
                                Image1.Source = IcopProject.MergeSource;
                            }
                        }
                        dgrModelSteps.ItemsSource = IcopModel.Steps;
                        foreach (var item in IcopModel.Steps)
                        {
                            item.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                            item.ReInit(Camera1_Canvas, Image1_Canvas, null);
                        }
                        DirectoryF.saveModel(IcopModel, IcopProject.Name, IcopModel.Name);
                        lb_Model.Content = tbAddNewModel.Text;
                        Models.Add(IcopModel.Name);
                        cbbAddNewModel.Items.Refresh();
                        cbbAddNewModel.SelectedItem = IcopModel.Name;


                        IcopProjectt.Steps.Clear();
                        IcopModell.Steps.Clear();
                    }

                }
                else
                {
                    IcopProject.Steps.Clear();
                    foreach (var step in IcopModel.Steps)
                    {
                        if (step.PM == "P")
                        {
                            IcopProject.Steps.Add(step);
                        }
                    }
                    IcopModel = new ModelLogic.IcopModel()
                    {
                        Name = tbAddNewModel.Text,
                        Number_PBA = IcopProject.Number_PBA,
                        QR_Lenght = Int32.TryParse(tbQRLenght.Text, out int lenght) ? lenght : 0,
                    };
                    Camera1_Canvas.Children.Clear();
                    Image1_Canvas.Children.Clear();
                    //foreach (var item in IcopProject.Steps)
                    //{
                    //    IcopModel.Steps.Insert(0, item);
                    //}
                    foreach (var item in IcopModel.Steps)
                    {
                        item.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                    }
                    OpenFileDialog openImage = new OpenFileDialog();
                    openImage.Multiselect = true;
                    openImage.Title = "Select a picture";
                    if (openImage.ShowDialog() == true)
                    {
                        foreach (var item in openImage.FileNames)
                        {
                            if (item.Contains("cam1"))
                            {
                                IcopModel.Image1 = new Bitmap(item);
                                IcopModel.PathImage1 = item;

                                IcopProject.Image1 = new Bitmap(item);
                                IcopProject.PathImage1 = item;
                            }
                            if (item.Contains("cam2"))
                            {
                                IcopModel.Image2 = new Bitmap(item);
                                IcopModel.PathImage2 = item;

                                IcopProject.Image2 = new Bitmap(item);
                                IcopProject.PathImage2 = item;
                            }
                            if (item.Contains("cam3"))
                            {
                                IcopModel.PathImage3 = item;
                                IcopModel.Image3 = new Bitmap(item);

                                IcopProject.Image3 = new Bitmap(item);
                                IcopProject.PathImage3 = item;
                            }
                            if (item.Contains("cam4"))
                            {
                                IcopModel.PathImage4 = item;
                                IcopModel.Image4 = new Bitmap(item);

                                IcopProject.Image4 = new Bitmap(item);
                                IcopProject.PathImage4 = item;
                            }

                        }
                        if (tgbtModel.IsChecked == true)
                        {
                            IcopModel.MergedBitmaps();
                            Camera1_Image.Source = IcopModel.MergeSource;
                            Image1.Source = IcopModel.MergeSource;
                        }

                        if (tgbProject.IsChecked == true)
                        {
                            IcopProject.MergedBitmaps();
                            Camera1_Image.Source = IcopProject.MergeSource;
                            Image1.Source = IcopProject.MergeSource;
                        }
                    }
                    dgrModelSteps.ItemsSource = null;

                    dgrModelSteps.ItemsSource = IcopModel.Steps;
                    DirectoryF.saveModel(IcopModel, IcopProject.Name, IcopModel.Name);
                    lb_Model.Content = tbAddNewModel.Text;
                    Models.Add(IcopModel.Name);
                    cbbAddNewModel.Items.Refresh();
                    cbbAddNewModel.SelectedItem = IcopModel.Name;

                }
            }
            bdAddNewModel.Visibility = Visibility.Hidden;
        }
        private void bt_SAVE_Click(object sender, RoutedEventArgs e)
        {
            var project = cbbAddNewProject.Text;
            var model = cbbAddNewModel.Text;

            IcopProjectt.Steps.Clear();
            foreach (var step in IcopProject.Steps)
            {
                IcopProjectt.Steps.Add(step);
            }

            IcopProject.Steps.Clear();

            foreach (ModelLogic.Step step in IcopModel.Steps)
            {
                if (step.PM == "P")
                {
                    IcopProject.Steps.Add(step);
                }
            }

            if (tgbProject.IsChecked == true)
            {
                IcopProject.Steps.Clear();

                if (IcopProject.MergeSource != null)
                {
                    IcopProject.MergedBitmaps();
                    Camera1_Image.Source = IcopProject.MergeSource;
                    Image1.Source = IcopProject.MergeSource;
                    IcopModell.Steps.Clear();
                    foreach (var step in IcopModel.Steps)
                    {
                        if (step.PM == "P")
                        {
                            IcopModell.Steps.Insert(0, step);
                        }
                    }
                    foreach (var step in IcopModell.Steps)
                    {
                        if (step.PM == "P")
                        {
                            IcopProject.Steps.Insert(0, step);
                        }
                    }
                    dgrModelSteps.ItemsSource = null;
                    dgrModelSteps.ItemsSource = IcopProject.Steps;
                    DirectoryF.SaveProject(IcopProject, project);
                    MessageBox.Show("Save Project " + IcopProject.Name + " successfully.\r\n"); //"Select Model again and continue other operations."
                }
                else
                {
                    MessageBox.Show(" Project not have image");

                }
            }


            if (tgbtModel.IsChecked == true)
            {
                Camera1_Image.Source = null;
                if (IcopModel.MergeSource != null)
                {
                    IcopModel.MergedBitmaps();

                    Camera1_Image.Source = IcopModel.MergeSource;
                    Image1.Source = IcopModel.MergeSource;
                    Image1_Canvas.Children.Clear();
                    Camera1_Canvas.Children.Clear();
                    foreach (ModelLogic.Step step in IcopModel.Steps)
                    {
                        if (step.PM == "M")
                        {
                            IcopModell.Steps.Add(step);

                        }
                    }
                    IcopModel.Steps.Clear();
                    foreach (var step in IcopModell.Steps)
                    {
                        if (step.PM == "M")
                        {
                            IcopModel.Steps.Add(step);
                        }
                    }
                    Image1_Canvas.Children.Clear();
                    Camera1_Canvas.Children.Clear();
                    foreach (var step in IcopModel.Steps)
                    {
                        step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                        step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                    }
                    if (File.Exists(DirectoryF.Model + "\\" + model + "\\" + model + ".png"))
                    {
                        File.Delete(DirectoryF.Model + "\\" + model + "\\" + model + ".png");
                    }
                    //   dgrModelSteps.ItemsSource = null;
                    DirectoryF.saveModel(IcopModel, project, model);
                    MessageBox.Show("Save Project " + IcopProject.Name + " successfully.\r\n" + "Save Model " + IcopModel.Name + " successfully.\r\n"); // "Select Model again and continue other operations."
                    foreach (var step in IcopProject.Steps)
                    {
                        IcopModel.Steps.Insert(0, step);
                    }

                    dgrModelSteps.ItemsSource = IcopModel.Steps;
                    IcopModell.Steps.Clear();
                }
                else
                {
                    MessageBox.Show(" Model not have image");
                }

            }


            {
                //if (tgbtModel.IsChecked == true)
                //{

                //    //tgbProject.IsChecked = true;
                //    //tgbtModel.IsChecked = false;

                //    //lbtgbtProject.Foreground = new SolidColorBrush(Colors.Orange);
                //    //lbtgbtModel.Foreground = new SolidColorBrush(Colors.Gray);

                //    //lb_Project.Foreground = new SolidColorBrush(Colors.Orange);
                //    //lb_Model.Foreground = new SolidColorBrush(Colors.Gray);


                //    Camera1_Image.Source = null;
                //    if (IcopModel.MergeSource != null)
                //    {
                //        IcopModel.MergedBitmaps();
                //        Camera1_Image.Source = IcopModel.MergeSource;
                //        Image1.Source = IcopModel.MergeSource;
                //        Image1_Canvas.Children.Clear();
                //        Camera1_Canvas.Children.Clear();

                //        foreach (ModelLogic.Step step in IcopModel.Steps)
                //        {
                //            if (step.PM == "M")
                //            {
                //                IcopModell.Steps.Add(step);

                //            }
                //        }

                //        IcopModel.Steps.Clear();
                //        foreach (var step in IcopModell.Steps)
                //        {
                //            if (step.PM == "M")
                //            {
                //                IcopModel.Steps.Add(step);
                //            }
                //        }
                //        Image1_Canvas.Children.Clear();
                //        Camera1_Canvas.Children.Clear();

                //        foreach (var step in IcopModel.Steps)
                //        {
                //            step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                //            step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                //        }
                //        if (File.Exists(DirectoryF.Model + "\\" + model + "\\" + model + ".png"))
                //        {

                //            File.Delete(DirectoryF.Model + "\\" + model + "\\" + model + ".png");
                //        }
                //        //   dgrModelSteps.ItemsSource = null;
                //        DirectoryF.saveModel(IcopModel, project, model);
                //        MessageBox.Show("Save Project " + IcopProject.Name + " successfully.\r\n" + "Save Model " + IcopModel.Name + " successfully.\r\n"); // "Select Model again and continue other operations."


                //        foreach (var step in IcopProject.Steps)
                //        {
                //            if (step.PM == "P")
                //            {
                //                IcopModel.Steps.Insert(0, step);

                //            }

                //        }
                //        dgrModelSteps.ItemsSource = IcopModel.Steps;
                //        IcopModell.Steps.Clear();
                //    }
                //    else
                //    {
                //        MessageBox.Show(" Model not have image");
                //    }
                //}
            }

        }
        public int Index;
        private void bt_Test_Click(object sender, RoutedEventArgs e)
        {
            if (ImageModel == null)
            {
                MessageBox.Show(" Choose step to test. ");
            }
            else
            {
                if (IcopModel.Steps.Count > 0)
                {
                    Index = IcopModel.Steps.Count;
                    dgrModelSteps.Focus();
                    if (IcopModel.MergeSource != null)
                    {
                        if (CurrentStep != new ModelLogic.Step())
                        {
                            //  Cv2.ImShow("dddd", IcopModel.MergeSource.ToMat()); đúng
                            var Imagee = CurrentStep.GetImage(IcopModel.MergeSource);
                            if (Imagee != null)
                            {
                                switch (CurrentStep.FUNC)
                                {
                                    case ModelLogic.Step.Functions.Read_QR:
                                        if (Imagee != null)
                                        {
                                            CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
                                            ImageTest.Source = CurrentStep.ImageTestReuslt;
                                            //   GC.Collect();                                           
                                            //Cv2.ImShow("ssss", CurrentStep.ImageTestReuslt.ToMat());
                                        }
                                        break;
                                    ////////////////////
                                    case ModelLogic.Step.Functions.Polarized_Capacitors:
                                        var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
                                        if (position_Polarized != 10)
                                            ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
                                        //   lb_Motathuattoan.Content = "Xác định vị trí cực của tụ phân cực ";
                                        bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                        bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                        lbpolarization.Content = CurrentStep.Get1;
                                        break;
                                    /////////////////////
                                    case ModelLogic.Step.Functions.Ceramic_Capacitors:
                                        double ThreasoldDoubleTop = 0;
                                        double ThreasoldDoubleBottom = 0;
                                        CurrentStep.Set3 = ((double)0).ToString("F2");
                                        CurrentStep.Set4 = ((double)0).ToString("F2");
                                        bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
                                        bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);
                                        if (isNumericTop == true && isNumericBottom == true)
                                        {
                                            var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            if ((Position_Ceramic != null) && (Imagee != null))
                                            {
                                                double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                //////////////////////////////////////////
                                                CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
                                                CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
                                                ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
                                                lbThreasoldColor.Content = CurrentStep.Get2;
                                                lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
                                                tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
                                                tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);
                                            }
                                        }
                                        else
                                        {
                                            var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            if ((Position_Ceramic != null) && (Imagee != null))
                                            {
                                                double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
                                                lbThreasoldColor.Content = CurrentStep.Get2;
                                                lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
                                                tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
                                                tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);
                                            }
                                        }
                                        break;
                                    /////////////////////
                                    case ModelLogic.Step.Functions.Matchtemplate:
                                        double ThreasoldDouble = 0;
                                        bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
                                        bdParameterMatchTemplate.Visibility = Visibility.Visible;
                                        bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                        if (isNumeric == true)
                                        {
                                            string Threasold = tb_Threasold.Text;
                                            CurrentStep.Set1 = Threasold;
                                            if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
                                                ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
                                            // lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh hiện (tại khu vực được chọn). ";
                                            tb_Threasold.Background = new SolidColorBrush(Colors.White);
                                        }
                                        else
                                            tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
                                        //   MessageBox.Show("The threshold must be a double number of type and less than 1 ");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                if (IcopProject.Steps.Count > 0)
                {
                    Index = IcopProject.Steps.Count;
                    dgrModelSteps.Focus();
                    if (IcopProject.MergeSource != null)
                    {
                        if (CurrentStep != new ModelLogic.Step())
                        {
                            var Imagee = CurrentStep.GetImage(IcopProject.MergeSource);
                            if (Imagee != null)
                            {
                                switch (CurrentStep.FUNC)
                                {
                                    case ModelLogic.Step.Functions.Read_QR:
                                        if (Imagee != null)
                                        {
                                            CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
                                            ImageTest.Source = CurrentStep.ImageTestReuslt;
                                            //   GC.Collect();
                                        }
                                        break;
                                    ////////////////////
                                    case ModelLogic.Step.Functions.Polarized_Capacitors:
                                        var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
                                        if (position_Polarized != 10)
                                            ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
                                        //   lb_Motathuattoan.Content = "Xác định vị trí cực của tụ phân cực ";
                                        bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                        bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                        lbpolarization.Content = CurrentStep.Get1;
                                        break;
                                    /////////////////////
                                    case ModelLogic.Step.Functions.Ceramic_Capacitors:
                                        double ThreasoldDoubleTop = 0;
                                        double ThreasoldDoubleBottom = 0;
                                        bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
                                        bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);
                                        if (isNumericTop == true && isNumericBottom == true)
                                        {
                                            var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            if ((Position_Ceramic != null) && (Imagee != null))
                                            {
                                                double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                //////////////////////////////////////////
                                                CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
                                                CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
                                                ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
                                                lbThreasoldColor.Content = CurrentStep.Get2;
                                                lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
                                                tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
                                                tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);
                                            }
                                        }
                                        else
                                        {
                                            var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            if ((Position_Ceramic != null) && (Imagee != null))
                                            {
                                                double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                //////////////////////////////////////////
                                                //  CurrentStep.Set3 = ((double)tbThreasoldColorTop.Value).ToString("F2");
                                                //  CurrentStep.Set4 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
                                                ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
                                                lbThreasoldColor.Content = CurrentStep.Get2;
                                                lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
                                                tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
                                                tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);

                                            }
                                        }
                                        break;
                                    /////////////////////
                                    case ModelLogic.Step.Functions.Matchtemplate:
                                        double ThreasoldDouble = 0;
                                        bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
                                        bdParameterMatchTemplate.Visibility = Visibility.Visible;
                                        bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                        if (isNumeric == true)
                                        {
                                            string Threasold = tb_Threasold.Text;
                                            CurrentStep.Set1 = Threasold;
                                            if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
                                                ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
                                            // lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh hiện (tại khu vực được chọn). ";
                                            tb_Threasold.Background = new SolidColorBrush(Colors.White);
                                        }
                                        else
                                            tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
                                        break;
                                    default:
                                        break;
                                }

                                string FUNCTIONTEST = cbb_Function.Text;
                                switch (FUNCTIONTEST)
                                {
                                    case "Read_QR":
                                        if (Imagee != null)
                                        {
                                            CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
                                            ImageTest.Source = CurrentStep.ImageTestReuslt;
                                            // GC.Collect();
                                        }
                                        break;
                                    ////////////////////
                                    case "Polarized_Capacitors":
                                        var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
                                        if (position_Polarized != 10)
                                            ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
                                        //   lb_Motathuattoan.Content = "Xác định vị trí cực của tụ phân cực ";
                                        bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                        bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                        lbpolarization.Content = CurrentStep.Get1;
                                        break;
                                    /////////////////////
                                    case "Ceramic_Capacitors":
                                        double ThreasoldDoubleTop = 0;
                                        double ThreasoldDoubleBottom = 0;
                                        bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
                                        bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);

                                        if (isNumericTop == true && isNumericBottom == true)
                                        {
                                            var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            if ((Position_Ceramic != null) && (Imagee != null))
                                            {
                                                double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                //////////////////////////////////////////
                                                CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
                                                CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
                                                ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
                                                lbThreasoldColor.Content = CurrentStep.Get2;
                                                lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
                                                tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
                                                tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);

                                            }

                                        }
                                        else
                                        {
                                            var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            if ((Position_Ceramic != null) && (Imagee != null))
                                            {
                                                double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                //////////////////////////////////////////
                                                //  CurrentStep.Set3 = ((double)tbThreasoldColorTop.Value).ToString("F2");
                                                //  CurrentStep.Set4 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
                                                ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
                                                lbThreasoldColor.Content = CurrentStep.Get2;
                                                lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
                                                tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
                                                tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);

                                            }
                                        }
                                        break;
                                    /////////////////////
                                    case "Matchtemplate":
                                        double ThreasoldDouble = 0;
                                        bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
                                        bdParameterMatchTemplate.Visibility = Visibility.Visible;
                                        bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                        if (isNumeric == true)
                                        {
                                            string Threasold = tb_Threasold.Text;
                                            CurrentStep.Set1 = Threasold;
                                            if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
                                                ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
                                            // lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh hiện (tại khu vực được chọn). ";
                                            tb_Threasold.Background = new SolidColorBrush(Colors.White);
                                        }
                                        else
                                            tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                }

            }

            Index = 0;

        }

        private void btAddProjectandModel_Click(object sender, RoutedEventArgs e)
        {
            bd_NewProject.Visibility = Visibility.Visible;
        }
        #endregion
        #region DrawingCanvas
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Handled == false)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var focusElement = Keyboard.FocusedElement;
                    if (focusElement != null && focusElement.GetType() == typeof(Label) && (e.Source as FrameworkElement) == Camera1_Canvas)
                    {
                        Console.WriteLine(sender.ToString() + " + " + focusElement.ToString());
                        focusElement.RaiseEvent(e);
                        Console.WriteLine("Fire event to focus Element");
                    }
                }
            }
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var grid = (sender as DataGrid);
            if (!e.Handled)
            {
                var focusElement = Keyboard.FocusedElement;
                if (focusElement != null)
                {
                    Console.WriteLine("Remover focus from " + Keyboard.FocusedElement.ToString());
                }
                if ((e.Source as FrameworkElement) == Camera1_Canvas)
                    Keyboard.ClearFocus();
                FocusManager.SetFocusedElement(Camera1_Canvas, null);

                if (IcopModel.Steps.Count > 0)
                {
                    try
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
                            if (item.IsForcus)
                            {
                                if (IcopModel.Steps.Contains(item))
                                {
                                    dgrModelSteps.SelectedItem = item;
                                    dgrModelSteps.ScrollIntoView(dgrModelSteps.SelectedItem);
                                }
                                
                                dgrModelSteps.Focus();
                                lb_Component.Content = item.Name;
                                CurrentStep.Label.BorderBrush = new SolidColorBrush(Colors.Yellow);
                                CurrentStep.Label.Foreground = new SolidColorBrush(Colors.Yellow);
                                CurrentStep.LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Yellow);
                                CurrentStep.LabelDisplay.Foreground = new SolidColorBrush(Colors.Yellow);
                            }
                        }
                    }

                    catch
                    {
                        
                    }
                }
            }
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IcopModel.Steps.Count > 0)
            {
                foreach (var item in IcopModel.Steps)
                {
                    if (item.IsForcus)
                    {
                        CurrentStep = item;
                        if (IcopModel.MergeSource != null)
                        {
                            ImageModel.Source = item.GetImage(IcopModel.MergeSource);
                        }
                    }
                }
            }
        }

        private void ImageModelCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentStep != new ModelLogic.Step())
            {
                CurrentStep.SetFuncLabel((sender as Canvas), e);
            }
        }

        private void ImageModelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentStep != new ModelLogic.Step())
            {
                CurrentStep.DrawFuncLabel((sender as Canvas), e);
            }
        }

        private void ImageModelCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CurrentStep != new ModelLogic.Step())
            {
                CurrentStep.DrawFuncLabel((sender as Canvas), e);
            }

        }

        private void Image1_Canvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IcopModel.Steps.Count > 0)
            {
                ModelLogic.Step stepToDelete = new ModelLogic.Step();

                ModelLogic.Step stepToCopy = new ModelLogic.Step();

                foreach (var item in IcopModel.Steps)
                {
                    if (item.IsForcus)
                    {
                        stepToDelete = item;

                        stepToCopy = item;
                    }
                }
                if (e.Key == Key.Delete)
                {
                    Image1_Canvas.Children.Remove(stepToDelete.LabelDisplay);
                    Camera1_Canvas.Children.Remove(stepToDelete.Label);
                    IcopModel.Steps.Remove(stepToDelete);
                }

                if (e.Key == Key.C)
                {
                    //  IcopModel.Steps.Add(stepToCopy);

                    if (tgbProject.IsChecked == true)
                    {
                        IcopModel.Steps.Add(new ModelLogic.Step(IcopModel.Steps.Count + 1, stepToCopy.Name, Camera1_Canvas, Image1_Canvas, null)

                        {
                            PM = stepToCopy.PM,
                            FUNC = stepToCopy.FUNC,
                            Set1 = stepToCopy.Set1,
                            Get1 = stepToCopy.Get1,
                            Set2 = stepToCopy.Set2,
                            Get2 = stepToCopy.Get2,
                            Set3 = stepToCopy.Set3,
                            Set4 = stepToCopy.Set4,
                            PCB = stepToCopy.PCB
                        }); ;
                        IcopModel.Steps[IcopModel.Steps.Count - 1 > 0 ? IcopModel.Steps.Count - 1 : 0].PlaceIn(Camera1_Canvas, Image1_Canvas, null);

                    }



                    if (tgbtModel.IsChecked == true)
                    {
                        IcopModel.Steps.Add(new ModelLogic.Step(IcopModel.Steps.Count + 1, stepToCopy.Name, Camera1_Canvas, Image1_Canvas, null)

                        {
                            PM = stepToCopy.PM,
                            FUNC = stepToCopy.FUNC,
                            Set1 = stepToCopy.Set1,
                            Get1 = stepToCopy.Get1,
                            Set2 = stepToCopy.Set2,
                            Get2 = stepToCopy.Get2,
                            Set3 = stepToCopy.Set3,
                            Set4 = stepToCopy.Set4,
                            PCB = stepToCopy.PCB,
                            ResutlTest = stepToCopy.ResutlTest,
                            Result = stepToCopy.Result,
                            Skip = stepToCopy.Skip,

                        }); ;
                        IcopModel.Steps[IcopModel.Steps.Count - 1 > 0 ? IcopModel.Steps.Count - 1 : 0].PlaceIn(Camera1_Canvas, Image1_Canvas, null);

                    }

                }

            }

            if (IcopProject.Steps.Count > 0)
            {
                ModelLogic.Step stepToDelete = new ModelLogic.Step();
                ModelLogic.Step stepToCopy = new ModelLogic.Step();
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

        private void ImageModelCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CurrentStep?.PlaceFuncLabel(ImageModelCanvas);
        }

        #endregion
        ////#region combobox

        private void cbb_Function_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        private void cbb_Function_DropDownClosed(object sender, EventArgs e)
        {
            switch (cbb_Function.Text)
            {
                case "Matchtemplate":

                    {
                        GR_Capacitor.Visibility = Visibility.Hidden;
                        GR_Capacitor_Result.Visibility = Visibility.Hidden;
                        if (CurrentStep != new ModelLogic.Step()) CurrentStep.FUNC = ModelLogic.Step.Functions.Matchtemplate;
                        bdParameterMatchTemplate.Visibility = Visibility.Visible;
                        bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;

                    }
                    break;
                case "Polarized_Capacitors":
                    {
                        GR_Capacitor.Visibility = Visibility.Visible;
                        GR_Capacitor_Result.Visibility = Visibility.Visible;
                        if (CurrentStep != new ModelLogic.Step()) CurrentStep.FUNC = ModelLogic.Step.Functions.Polarized_Capacitors;

                    }
                    break;
                case "Ceramic_Capacitors":
                    {
                        GR_Capacitor.Visibility = Visibility.Hidden;
                        GR_Capacitor_Result.Visibility = Visibility.Hidden;
                        if (CurrentStep != new ModelLogic.Step()) CurrentStep.FUNC = ModelLogic.Step.Functions.Ceramic_Capacitors;
                        bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                        bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                    }
                    break;

                case "Read_QR":
                    {
                        GR_Capacitor.Visibility = Visibility.Hidden;
                        GR_Capacitor_Result.Visibility = Visibility.Hidden;
                        if (CurrentStep != new ModelLogic.Step()) CurrentStep.FUNC = ModelLogic.Step.Functions.Read_QR;
                        bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                        bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                        bdParameterPolarized_Capacitors.Visibility = Visibility.Collapsed;
                    }
                    break;
            }

            switch (CurrentStep.FUNC)
            {
                case ModelLogic.Step.Functions.Read_QR:
                    lb_Motathuattoan.Content = "Giải mã QR, độ dài 23 kí tự.\r\n" + "QR decoding, length 23 characters";
                    bdReadQR.Visibility = Visibility.Visible;
                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                    bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                    bdParameterPolarized_Capacitors.Visibility = Visibility.Collapsed;
                    break;

                case ModelLogic.Step.Functions.Polarized_Capacitors:
                    lb_Motathuattoan.Content = "Xác định vị trí cực.\r\n" + "Determine the polarity of the polarizing capacitor.";
                    bdParameterPolarized_Capacitors.Visibility = Visibility.Visible;
                    bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                    break;

                case ModelLogic.Step.Functions.Ceramic_Capacitors:
                    bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                    bdParameterPolarized_Capacitors.Visibility = Visibility.Collapsed;
                    lb_Motathuattoan.Content = "Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt.\r\n" + "Check the existence of objects in the image by color according to the setting threshold.";
                    break;

                case ModelLogic.Step.Functions.Matchtemplate:
                    bdParameterMatchTemplate.Visibility = Visibility.Visible;
                    bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                    bdParameterPolarized_Capacitors.Visibility = Visibility.Collapsed;
                    lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh (tại khu vực được chọn).\r\n" + "Look for similar objects in the current image (in the selected area).";
                    break;
                default:
                    break;
            }
        }

        private void cbbAddNewProject_DropDownClosed(object sender, EventArgs e)
        {

            tbNewProjectSetting.Text = "";
            tgbProject.IsChecked = true;
            tgbtModel.IsChecked = false;
            lbtgbtProject.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lbtgbtModel.Foreground = new SolidColorBrush(Colors.Gray);
            lb_Project.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lb_Model.Foreground = new SolidColorBrush(Colors.Gray);
            var projectSeleted = cbbAddNewProject.Text;


            if (!Directory.Exists(DirectoryF.Model + "\\" + projectSeleted))
            {
                cbbAddNewModel.Items.Refresh();
                return;

            }
            {
                if (projectSeleted != "")
                {
                    if (cbbAddNewProject.Text != null)
                    {

                        var AllModelFolder = Directory.GetDirectories(DirectoryF.Model + "\\" + projectSeleted);
                        for (int i = 0; i < AllModelFolder.Count(); i++)
                        {
                            var foldername = AllModelFolder[i].Split('\\');
                            AllModelFolder[i] = foldername[foldername.Count() - 1];
                        }
                        cbbAddNewModel.ItemsSource = null;
                        cbbAddNewModel.ItemsSource = AllModelFolder;
                    }
                }
                else
                {
                    cbbAddNewModel.Items.Refresh();
                }
            }


            var modelSeleted = cbbAddNewModel.Text;
            if (File.Exists(DirectoryF.Model + "\\" + projectSeleted + "\\" + projectSeleted + DirectoryF.ICOP_EXT))
            {
                string JsonStr = File.ReadAllText(DirectoryF.Model + "\\" + projectSeleted + "\\" + projectSeleted + DirectoryF.ICOP_EXT);
                IcopProject = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr);

                if (File.Exists(DirectoryF.Model + "\\" + projectSeleted + "\\" + projectSeleted + ".png"))
                {
                    BitmapImage projectImage = new BitmapImage();
                    projectImage.BeginInit();
                    projectImage.UriSource = new Uri(DirectoryF.Model + "\\" + projectSeleted + "\\" + projectSeleted + ".png");
                    projectImage.CacheOption = BitmapCacheOption.OnLoad;
                    projectImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    projectImage.EndInit();
                    if (Image1 != null)
                    {
                        IcopProject.MergeSource = projectImage;
                        Image1.Source = IcopProject.MergeSource;
                        Camera1_Image.Source = IcopProject.MergeSource;
                    }
                }
                else
                {
                }
                Image1_Canvas.Children.Clear();
                Camera1_Canvas.Children.Clear();
                foreach (var step in IcopProject.Steps)
                {
                    step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                    step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                }
            }
            IcopModel = new ModelLogic.IcopModel();
            foreach (var item in IcopProject.Steps)
            {
                IcopModel.Steps.Add(item);
            }
            dgrModelSteps.ItemsSource = IcopModel.Steps;
            dgrModelSteps.SelectedIndex = 0;
            dgrModelSteps.Focus();
            dgrModelSteps.Items.Refresh();
            lb_Project.Content = cbbAddNewProject.Text;
            //  cbbAddNewProject.ItemsSource = null;
        }

        private void cbbAddNewModel_DropDownClosed(object sender, EventArgs e)
        {
            dgrModelSteps.SelectedIndex = 0;
            dgrModelSteps.Focus();
            tbAddNewModel.Text = "";
            var projectSeleted = cbbAddNewProject.Text;
            var modelSeleted = cbbAddNewModel.Text;
            try
            {
                if (File.Exists(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + DirectoryF.ICOP_EXT))
                {
                    string JsonStr = File.ReadAllText(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + DirectoryF.ICOP_EXT);
                    IcopModel = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr);
                    if (File.Exists(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + ".png"))
                    {
                        var path = DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + ".png";
                        try
                        {
                            if (File.Exists(path))
                            {
                                IcopProjectt.Steps.Clear();

                                //foreach (ModelLogic.Step step in IcopModel.Steps)
                                //{
                                //    if (step.PM == "P")
                                //    {
                                //        IcopProject.Steps.Add(step);
                                //    }
                                //}

                                foreach (var step in IcopProject.Steps)
                                {

                                    IcopProjectt.Steps.Insert(0, step);
                                    Image1_Canvas.Children.Clear();
                                    Camera1_Canvas.Children.Clear();
                                    //IcopProjectt.Steps.Add(step);
                                }

                                IcopProject.Steps.Clear();
                                //foreach (var step in IcopModel.Steps)
                                //{
                                //    if (step.PM == "P")
                                //    {
                                //        IcopProject.Steps.Add(step);
                                //    }
                                //}
                                BitmapImage modelImage = new BitmapImage();
                                modelImage.BeginInit();
                                modelImage.UriSource = new Uri(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + ".png");
                                modelImage.CacheOption = BitmapCacheOption.OnLoad;
                                modelImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                                modelImage.EndInit();
                                if (Image1 != null)
                                {
                                    //Image1_Canvas.Children.Clear();
                                    //Camera1_Canvas.Children.Clear();
                                    IcopModel.MergeSource = modelImage;
                                    Image1.Source = null;
                                    Camera1_Image.Source = null;
                                    Image1.Source = IcopModel.MergeSource;
                                    Camera1_Image.Source = IcopModel.MergeSource;
                                    foreach (var step in IcopModel.Steps)
                                    {
                                        step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                                        step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                                    }
                                }
                            }
                            else
                            {
                                IcopModel.MergeSource = null;
                                Image1.Source = null;
                                Camera1_Image.Source = null;

                            }

                            Image1_Canvas.Children.Clear();
                            Camera1_Canvas.Children.Clear();
                            foreach (var step in IcopModel.Steps)
                            {
                                step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                                step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);

                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            throw;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Model not have origin image");
                    }
                    foreach (var step in IcopProjectt.Steps)
                    {

                          IcopModel.Steps.Insert(0, step);
                      //  IcopModel.Steps.Add(step);
                    }
                    IcopProjectt.Steps.Clear();
                    Image1_Canvas.Children.Clear();
                    Camera1_Canvas.Children.Clear();
                    foreach (var step in IcopModel.Steps)
                    {
                        step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                        step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                    }

                    for (int i = 0; i < IcopModel.Number_PBA; i++)
                    dgrModelSteps.SelectedIndex = 0;
                    dgrModelSteps.Focus();
                    dgrModelSteps.ItemsSource = IcopModel.Steps;
                    dgrModelSteps.Items.Refresh();
                }
            }
            catch
            {

            }


            tgbProject.IsChecked = false;
            tgbtModel.IsChecked = true;

            lbtgbtProject.Foreground = new SolidColorBrush(Colors.White);
            lbtgbtModel.Foreground = new SolidColorBrush(Colors.OrangeRed);

            lb_Project.Foreground = new SolidColorBrush(Colors.White);
            lb_Model.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lb_Model.Content = cbbAddNewModel.Text;
        }

        private void cbb_PCB_DropDownClosed(object sender, EventArgs e)
        {
            if (CurrentStep != new ModelLogic.Step())
            {
                if (cbb_PCB.Text == null)
                {
                }
                {
                    if (tgbtModel.IsChecked == true)
                    {
                        int PCB = Convert.ToInt32(cbb_PCB.Text);

                        int PBA = IcopModel.Number_PBA;
                        if (PCB < PBA)
                        {
                            CurrentStep.PCB = PCB;
                            cbb_PCB.Text = CurrentStep.PCB.ToString();
                        }
                        else
                        {
                            CurrentStep.PCB = PBA;
                            cbb_PCB.Text = CurrentStep.PCB.ToString();
                            MessageBox.Show("PCB number must be less than Project's PBA number ");
                        }
                    }


                    if (tgbProject.IsChecked == true)
                    {
                        int PCB = Convert.ToInt32(cbb_PCB.Text);
                        int PBA = IcopProject.Number_PBA;
                        if (PCB < PBA)
                        {
                            CurrentStep.PCB = PCB;
                            cbb_PCB.Text = CurrentStep.PCB.ToString();

                        }
                        else
                        {
                            CurrentStep.PCB = PBA;
                            cbb_PCB.Text = CurrentStep.PCB.ToString();
                            MessageBox.Show("PCB number must be less than Project's PBA number ");
                        }
                    }
                }
            }
        }

        private void cbbAddNewProject_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var project = cbbAddNewProject.Text;
            var model = cbbAddNewModel.Text;
            IcopProjectt.Steps.Clear();

            foreach (var step in IcopProject.Steps)
            {
                IcopProjectt.Steps.Add(step);
            }
            IcopProject.Steps.Clear();

            foreach (ModelLogic.Step step in IcopModel.Steps)
            {
                if (step.PM == "P")
                {
                    IcopProject.Steps.Add(step);
                }
            }

            if (tgbtModel.IsChecked == true)
            {
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("       Do you want to save model", " ", button);
                if (result == MessageBoxResult.Yes)
                {
                    if (tgbtModel.IsChecked == true)
                    {
                        Camera1_Image.Source = null;
                        if (IcopModel.MergeSource != null)
                        {
                            IcopModel.MergedBitmaps();
                            Camera1_Image.Source = IcopModel.MergeSource;
                            Image1.Source = IcopModel.MergeSource;
                            Image1_Canvas.Children.Clear();
                            Camera1_Canvas.Children.Clear();

                            foreach (ModelLogic.Step step in IcopModel.Steps)
                            {
                                if (step.PM == "M")
                                {
                                    IcopModell.Steps.Add(step);

                                }
                            }

                            IcopModel.Steps.Clear();
                            foreach (var step in IcopModell.Steps)
                            {
                                if (step.PM == "M")
                                {
                                    IcopModel.Steps.Add(step);
                                }
                            }
                            Image1_Canvas.Children.Clear();
                            Camera1_Canvas.Children.Clear();

                            foreach (var step in IcopModel.Steps)
                            {
                                step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                                step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                            }
                            if (File.Exists(DirectoryF.Model + "\\" + model + "\\" + model + ".png"))
                            {

                                File.Delete(DirectoryF.Model + "\\" + model + "\\" + model + ".png");
                            }
                            //   dgrModelSteps.ItemsSource = null;
                            DirectoryF.saveModel(IcopModel, project, model);
                            MessageBox.Show("Save Project " + IcopProject.Name + " successfully.\r\n" + "Save Model " + IcopModel.Name + " successfully.\r\n"); // "Select Model again and continue other operations."


                            foreach (var step in IcopProject.Steps)
                            {
                                if (step.PM == "P")
                                {
                                    IcopModel.Steps.Insert(0, step);

                                }

                            }

                           dgrModelSteps.ItemsSource = IcopModel.Steps;
                            IcopModell.Steps.Clear();
                        }
                        else
                        {
                            MessageBox.Show(" Model not have image");
                        }
                    }
                }
                else if (result == MessageBoxResult.No)
                {

                    try
                    {
                        tgbProject.IsChecked = true;
                        tgbtModel.IsChecked = false;
                        lbtgbtProject.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        lbtgbtModel.Foreground = new SolidColorBrush(Colors.Gray);
                        lb_Project.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        lb_Model.Foreground = new SolidColorBrush(Colors.Gray);
                        var AllModelFolder = Directory.GetDirectories(DirectoryF.Model);
                        for (int i = 0; i < AllModelFolder.Count(); i++)
                        {
                            var foldername = AllModelFolder[i].Split('\\');
                            AllModelFolder[i] = foldername[foldername.Count() - 1];
                        }
                        Projects = AllModelFolder.ToList();
                        cbbAddNewProject.ItemsSource = Projects;
                        foreach (ModelLogic.Step step in IcopModel.Steps)
                        {
                            if (step.PM == "P")
                            {
                                IcopProjectt.Steps.Insert(0, step);

                            }
                        }

                        IcopProject.Steps.Clear();
                        foreach (var step in IcopProjectt.Steps)
                        {
                            IcopProject.Steps.Insert(0, step);

                        }
                        Image1_Canvas.Children.Clear();
                        Camera1_Canvas.Children.Clear();
                        Camera1_Image.Source = IcopProject.MergeSource;
                        Image1.Source = IcopProject.MergeSource;
                        foreach (var step in IcopProject.Steps)
                        {
                            step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                            step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                        }
                        IcopProjectt.Steps.Clear();
                        dgrModelSteps.ItemsSource = null;
                        dgrModelSteps.ItemsSource = IcopProject.Steps;
                    }
                    catch
                    {


                    }



                }
            }

            else
            {
                // cbbAddNewProject.Items.Refresh();
                var AllModelFolder = Directory.GetDirectories(DirectoryF.Model);
                for (int i = 0; i < AllModelFolder.Count(); i++)
                {
                    var foldername = AllModelFolder[i].Split('\\');
                    AllModelFolder[i] = foldername[foldername.Count() - 1];
                }
                Projects = AllModelFolder.ToList();
                //   cbbAddNewProject.ItemsSource = null;
                cbbAddNewProject.ItemsSource = Projects;


            }
        }

        //#endregion

        #region toggle button

        private void tgbProject_Click(object sender, RoutedEventArgs e)
        {
            tgbtModel.IsChecked = false;
            var projectSeleted = cbbAddNewProject.Text;
            // bdtgbtProject.Background = new SolidColorBrush(Colors.CadetBlue);
            lbtgbtProject.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lbtgbtModel.Foreground = new SolidColorBrush(Colors.Gray);

            lb_Project.Foreground = new SolidColorBrush(Colors.OrangeRed);
            lb_Model.Foreground = new SolidColorBrush(Colors.Gray);

            Image1.Source = IcopProject.MergeSource;
            Camera1_Image.Source = IcopProject.MergeSource;

            Image1_Canvas.Children.Clear();
            Camera1_Canvas.Children.Clear();
            foreach (var step in IcopModel.Steps)
            {
                step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
            }




        }

        private void tgbtModel_Click(object sender, RoutedEventArgs e)
        {
            tgbProject.IsChecked = false;
            lbtgbtProject.Foreground = new SolidColorBrush(Colors.Gray);
            lbtgbtModel.Foreground = new SolidColorBrush(Colors.OrangeRed);

            lb_Project.Foreground = new SolidColorBrush(Colors.Gray);
            lb_Model.Foreground = new SolidColorBrush(Colors.OrangeRed);

            var projectSeleted = cbbAddNewProject.Text;
            var modelSeleted = cbbAddNewModel.Text;

            Image1.Source = IcopModel.MergeSource;
            Camera1_Image.Source = IcopModel.MergeSource;

            Image1_Canvas.Children.Clear();
            Camera1_Canvas.Children.Clear();
            foreach (var step in IcopModel.Steps)
            {
                step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
            }
        }


        private void tgbDrawNewTest_Click(object sender, RoutedEventArgs e)
        {

            //if (tgbProject.IsChecked == true)
            //{
            //    IcopModel.Steps.Add(new ModelLogic.Step(IcopModel.Steps.Count + 1, IcopModel.Steps.Count.ToString() + "." + " P - Component ", Camera1_Canvas, Image1_Canvas, null)
            //    { PM = "P" });
            //    IcopModel.Steps[IcopModel.Steps.Count - 1 > 0 ? IcopModel.Steps.Count - 1 : 0].PlaceIn(Camera1_Canvas, Image1_Canvas, null);
            //    dgrModelSteps.Items.Refresh();
            //}
            //if (tgbtModel.IsChecked == true)
            //{
            //    IcopModel.Steps.Add(new ModelLogic.Step(IcopModel.Steps.Count + 1, IcopModel.Steps.Count.ToString() + "." + " M - Component  ", Camera1_Canvas, Image1_Canvas, null)
            //    { PM = "M" });
            //    IcopModel.Steps[IcopModel.Steps.Count - 1 > 0 ? IcopModel.Steps.Count - 1 : 0].PlaceIn(Camera1_Canvas, Image1_Canvas, null);
            //    dgrModelSteps.Items.Refresh();
            //   }
        }

        #endregion

        private void dgrModelSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in IcopModel.Steps)
            {
                cbb_PCB.SelectedItem = item.PCB;
                if (item.ResutlTest == true)
                {
                    item.Label.BorderBrush = new SolidColorBrush(Colors.Green);
                    item.LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Green);
                    item.Label.Foreground = new SolidColorBrush(Colors.Green);
                    item.LabelDisplay.Foreground = new SolidColorBrush(Colors.Green);
                    //  cbb_PCB.Text = item.PCB.ToString();
                }
                else
                {
                    item.Label.BorderBrush = new SolidColorBrush(Colors.Red);
                    item.LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Red);
                    item.Label.Foreground = new SolidColorBrush(Colors.Red);
                    item.LabelDisplay.Foreground = new SolidColorBrush(Colors.Red);
                    // cbb_PCB.Text = item.PCB.ToString();
                }
            }

            var grid = (sender as DataGrid);
            if (tgbtModel.IsChecked == true)
            {
                if (grid.SelectedItem != null)
                {
                    CurrentStep = (ModelLogic.Step)grid.SelectedItem;
                    ImageModel.Source = CurrentStep.GetImage(IcopModel.MergeSource);
                    tbComponent.Content = CurrentStep.Name;
                    lb_Component.Content = CurrentStep.Name;
                    cbb_PCB.Text = CurrentStep.PCB.ToString();
                    CurrentStep.Label.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.Label.Foreground = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.LabelDisplay.Foreground = new SolidColorBrush(Colors.Yellow);
                    dgrModelSteps.ScrollIntoView(dgrModelSteps.SelectedItem);
                    switch (CurrentStep.FUNC)
                    {
                        case ModelLogic.Step.Functions.Matchtemplate:
                            //if (CurrentStep.Set1.Length < 1)
                            //{
                            //    tb_Threasold.Text = "0.75";
                            //}
                            tb_Threasold.Text = CurrentStep.Set1;
                            lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh (tại khu vực được chọn).\r\n" + "Look for similar objects in the current image (in the selected area).";
                            break;
                        case ModelLogic.Step.Functions.Polarized_Capacitors:
                            lbpolarization.Content = CurrentStep.Set1;
                            lb_Motathuattoan.Content = "Xác định vị trí cực.\r\n" + "Determine the polarity of the polarizing capacitor.";
                            break;
                        case ModelLogic.Step.Functions.Ceramic_Capacitors:
                            tbThreasoldColorBottom.Text = CurrentStep.Set3;
                            tbThreasoldColorTop.Text = CurrentStep.Set4.ToString();
                            lbThreasoldColor.Content = CurrentStep.Get2.ToString();
                            lbNguongTren.Content = CurrentStep.Set2.ToString();
                            lbNguongDuoi.Content = CurrentStep.Set1.ToString();
                            lb_Motathuattoan.Content = "Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt.\r\n" + "Check the existence of objects in the image by color according to the setting threshold.";
                            break;
                        case ModelLogic.Step.Functions.Read_QR:
                            lb_Motathuattoan.Content = "Giải mã QR, độ dài 23 kí tự.\r\n" + "QR decoding, length 23 characters"; ;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (tgbProject.IsChecked == true)
            {
                if (grid.SelectedItem != null)
                {
                    CurrentStep = (ModelLogic.Step)grid.SelectedItem;
                    ImageModel.Source = null;
                    ImageModel.Source = CurrentStep.GetImage(IcopProject.MergeSource);
                    tbComponent.Content = CurrentStep.Name;
                    lb_Component.Content = CurrentStep.Name;
                    CurrentStep.Label.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.Label.Foreground = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    CurrentStep.LabelDisplay.Foreground = new SolidColorBrush(Colors.Yellow);
                    dgrModelSteps.ScrollIntoView(dgrModelSteps.SelectedItem);
                    cbb_PCB.Text = CurrentStep.PCB.ToString();
                    switch (CurrentStep.FUNC)
                    {
                        case ModelLogic.Step.Functions.Matchtemplate:
                            //if (CurrentStep.Set1.Length < 1)
                            //{
                            //    tb_Threasold.Text = "0.75";
                            //}
                            tb_Threasold.Text = CurrentStep.Set1;
                            lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh (tại khu vực được chọn).\r\n" + "Look for similar objects in the current image (in the selected area).";
                            break;
                        case ModelLogic.Step.Functions.Polarized_Capacitors:
                            lbpolarization.Content = CurrentStep.Set1;
                            lb_Motathuattoan.Content = "Xác định vị trí cực.\r\n" + "Determine the polarity of the polarizing capacitor.";
                            break;
                        case ModelLogic.Step.Functions.Ceramic_Capacitors:
                            tbThreasoldColorBottom.Text = CurrentStep.Set3;
                            tbThreasoldColorTop.Text = CurrentStep.Set4.ToString();
                            lbThreasoldColor.Content = CurrentStep.Get2.ToString();
                            lbNguongTren.Content = CurrentStep.Set2.ToString();
                            lbNguongDuoi.Content = CurrentStep.Set1.ToString();
                            lb_Motathuattoan.Content = "Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt.\r\n" + "Check the existence of objects in the image by color according to the setting threshold.";
                            break;
                        case ModelLogic.Step.Functions.Read_QR:
                            lb_Motathuattoan.Content = "Giải mã QR, độ dài 23 kí tự.\r\n" + "QR decoding, length 23 characters"; ;
                            break;
                        default:
                            break;
                    }
                }
            }
            //  dgrModelSteps.ScrollIntoView(dgrModelSteps.SelectedItem);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Model.txt"))
            {
                string JsonStr = File.ReadAllText("Model.txt");
                IcopModel = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr);
                LoadModel();

                IcopProject = JsonSerializer.Deserialize<ModelLogic.IcopModel>(JsonStr);
                LoadProject();

            }
        }

        private void cbSkip_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentStep != null)
            {
                CurrentStep.Skip = (bool)cbSkip.IsChecked;
            }
        }

        private void Load_Model_New_Click(object sender, RoutedEventArgs e)
        {

            var projectSeleted = cbbAddNewProject.Text;
            if (cbbAddNewProject != null)
            {
                if (cbbAddNewModel != null)
                {
                    var modelSeleted = cbbAddNewModel.Text;
                    if (File.Exists(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + DirectoryF.ICOP_EXT))
                    {
                        string JsonStr = File.ReadAllText(DirectoryF.Model + "\\" + projectSeleted + "\\" + modelSeleted + "\\" + modelSeleted + DirectoryF.ICOP_EXT);
                        IcopModel = new ModelLogic.IcopModel();

                    }
                    dgrModelSteps.ItemsSource = IcopModel.Steps;
                    dgrModelSteps.Items.Refresh();
                }

            }
        }

        System.Windows.Point _start;

        private void Camera1_Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _start = Mouse.GetPosition(Camera1_Canvas);
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (tgbProject.IsChecked == true)
                {
                    IcopModel.Steps.Add(new ModelLogic.Step(IcopModel.Steps.Count + 1, IcopModel.Steps.Count.ToString() + "." + "P_Component", Camera1_Canvas, Image1_Canvas, null)

                    {
                        PM = "P",
                        Set1 = "0.75",
                    }
                    );
                    IcopModel.Steps[IcopModel.Steps.Count - 1 > 0 ? IcopModel.Steps.Count - 1 : 0].PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                    dgrModelSteps.Items.Refresh();

                }

                if (tgbtModel.IsChecked == true)
                {
                    IcopModel.Steps.Add(new ModelLogic.Step(IcopModel.Steps.Count + 1, IcopModel.Steps.Count.ToString() + "." + "M_Component", Camera1_Canvas, Image1_Canvas, null)

                    {
                        PM = "M",
                        Set1 = "0.75",

                    });

                    IcopModel.Steps[IcopModel.Steps.Count - 1 > 0 ? IcopModel.Steps.Count - 1 : 0].PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                    dgrModelSteps.Items.Refresh();

                }
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.F1)
            {
                var project = cbbAddNewProject.Text;
                var model = cbbAddNewModel.Text;

                IcopProjectt.Steps.Clear();

                foreach (var step in IcopProject.Steps)
                {
                    IcopProjectt.Steps.Add(step);
                }

                IcopProject.Steps.Clear();

                foreach (ModelLogic.Step step in IcopModel.Steps)
                {
                    if (step.PM == "P")
                    {
                        IcopProject.Steps.Add(step);
                    }
                }

                if (tgbProject.IsChecked == true)
                {
                    IcopProject.Steps.Clear();

                    if (IcopProject.MergeSource != null)
                    {
                        IcopProject.MergedBitmaps();
                        Camera1_Image.Source = IcopProject.MergeSource;
                        Image1.Source = IcopProject.MergeSource;
                        IcopModell.Steps.Clear();
                        foreach (var step in IcopModel.Steps)
                        {
                            if (step.PM == "P")
                            {
                                IcopModell.Steps.Insert(0, step);
                            }
                        }
                        foreach (var step in IcopModell.Steps)
                        {
                            if (step.PM == "P")
                            {
                                IcopProject.Steps.Insert(0, step);
                            }
                        }
                        dgrModelSteps.ItemsSource = null;
                        dgrModelSteps.ItemsSource = IcopProject.Steps;
                        DirectoryF.SaveProject(IcopProject, project);
                        MessageBox.Show("Save Project " + IcopProject.Name + " successfully.\r\n"); //"Select Model again and continue other operations."
                    }
                    else
                    {
                        MessageBox.Show(" Project not have image");

                    }
                }


                if (tgbtModel.IsChecked == true)
                {
                    Camera1_Image.Source = null;
                    if (IcopModel.MergeSource != null)
                    {
                        //Camera1_Image.Source = null;
                        //Image1.Source = null;
                        IcopModel.MergedBitmaps();
                        Camera1_Image.Source = IcopModel.MergeSource;
                        Image1.Source = IcopModel.MergeSource;
                        //Image1_Canvas.Children.Clear();
                        //Camera1_Canvas.Children.Clear();
                        foreach (ModelLogic.Step step in IcopModel.Steps)
                        {
                            if (step.PM == "M")
                            {
                                IcopModell.Steps.Add(step);
                            }
                        }
                        IcopModel.Steps.Clear();
                        foreach (var step in IcopModell.Steps)
                        {
                            if (step.PM == "M")
                            {
                                IcopModel.Steps.Add(step);
                            }
                        }
                     
                        foreach (var step in IcopModel.Steps)
                        {
                            step.ReInit(Camera1_Canvas, Image1_Canvas, null);
                            step.PlaceIn(Camera1_Canvas, Image1_Canvas, null);
                        }
                        if (File.Exists(DirectoryF.Model + "\\" + model + "\\" + model + ".png"))
                        {
                            File.Delete(DirectoryF.Model + "\\" + model + "\\" + model + ".png");
                        }
                        DirectoryF.saveModel(IcopModel, project, model);
                        MessageBox.Show("Save Project " + IcopProject.Name + " successfully.\r\n" + "Save Model " + IcopModel.Name + " successfully.\r\n"); // "Select Model again and continue other operations."
                        foreach (var step in IcopProject.Steps)
                        {
                            IcopModel.Steps.Insert(0, step);
                        }

                        dgrModelSteps.ItemsSource = IcopModel.Steps;
                        IcopModell.Steps.Clear();
                    }
                    else
                    {
                        MessageBox.Show(" Model not have image");
                    }

                }

            }

            if (e.Key == Key.F2)
            {
                if (ImageModel == null)
                {
                    MessageBox.Show(" Choose step to test. ");
                }
                else
                {
                    if (IcopModel.Steps.Count > 0)
                    {
                        Index = IcopModel.Steps.Count;
                        dgrModelSteps.Focus();
                        if (IcopModel.MergeSource != null)
                        {
                            if (CurrentStep != new ModelLogic.Step())
                            {
                               
                                var Imagee = CurrentStep.GetImage(IcopModel.MergeSource);
                                if (Imagee != null)
                                {
                                    switch (CurrentStep.FUNC)
                                    {
                                        case ModelLogic.Step.Functions.Read_QR:
                                            if (Imagee != null)
                                            {
                                                CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
                                                ImageTest.Source = CurrentStep.ImageTestReuslt;
                                                                                       
                                                
                                            }
                                            break;
                                        ////////////////////
                                        case ModelLogic.Step.Functions.Polarized_Capacitors:
                                            var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
                                            if (position_Polarized != 10)
                                                ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
                                          
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            lbpolarization.Content = CurrentStep.Get1;
                                            break;
                                        /////////////////////
                                        case ModelLogic.Step.Functions.Ceramic_Capacitors:
                                            double ThreasoldDoubleTop = 0;
                                            double ThreasoldDoubleBottom = 0;
                                            CurrentStep.Set3 = ((double)0).ToString("F2");
                                            CurrentStep.Set4 = ((double)0).ToString("F2");
                                            bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
                                            bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);
                                            if (isNumericTop == true && isNumericBottom == true)
                                            {
                                                var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                                bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                                if ((Position_Ceramic != null) && (Imagee != null))
                                                {
                                                    double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                    double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                    CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                    CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                    //////////////////////////////////////////
                                                    CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
                                                    CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
                                                    ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                    
                                                    lbThreasoldColor.Content = CurrentStep.Get2;
                                                    lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
                                                    tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
                                                    tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);
                                                }
                                            }
                                            else
                                            {
                                                var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                                bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                                if ((Position_Ceramic != null) && (Imagee != null))
                                                {
                                                    double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                    double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                    CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                    CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                    ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                    lbThreasoldColor.Content = CurrentStep.Get2;
                                                    lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
                                                    tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
                                                    tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);
                                                }
                                            }
                                            break;
                                        /////////////////////
                                        case ModelLogic.Step.Functions.Matchtemplate:
                                            double ThreasoldDouble = 0;
                                            bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
                                            bdParameterMatchTemplate.Visibility = Visibility.Visible;
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                            if (isNumeric == true)
                                            {
                                                string Threasold = tb_Threasold.Text;
                                                CurrentStep.Set1 = Threasold;
                                                if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
                                                    ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
                                                tb_Threasold.Background = new SolidColorBrush(Colors.White);
                                            }
                                            else
                                                tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    if (IcopProject.Steps.Count > 0)
                    {
                        Index = IcopProject.Steps.Count;
                        dgrModelSteps.Focus();
                        if (IcopProject.MergeSource != null)
                        {
                            if (CurrentStep != new ModelLogic.Step())
                            {
                                var Imagee = CurrentStep.GetImage(IcopProject.MergeSource);
                                if (Imagee != null)
                                {
                                    switch (CurrentStep.FUNC)
                                    {
                                        case ModelLogic.Step.Functions.Read_QR:
                                            if (Imagee != null)
                                            {
                                                CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
                                                ImageTest.Source = CurrentStep.ImageTestReuslt;
                                            }
                                            break;
                                        ////////////////////
                                        case ModelLogic.Step.Functions.Polarized_Capacitors:
                                            var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
                                            if (position_Polarized != 10)
                                                ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            lbpolarization.Content = CurrentStep.Get1;
                                            break;
                                        /////////////////////
                                        case ModelLogic.Step.Functions.Ceramic_Capacitors:
                                            double ThreasoldDoubleTop = 0;
                                            double ThreasoldDoubleBottom = 0;
                                            bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
                                            bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);
                                            if (isNumericTop == true && isNumericBottom == true)
                                            {
                                                var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                                bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                                if ((Position_Ceramic != null) && (Imagee != null))
                                                {
                                                    double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                    double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                    CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                    CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                    //////////////////////////////////////////
                                                    CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
                                                    CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
                                                    ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                    lbThreasoldColor.Content = CurrentStep.Get2;
                                                    lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
                                                    tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
                                                    tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);
                                                }
                                            }
                                            else
                                            {
                                                var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                                bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                                if ((Position_Ceramic != null) && (Imagee != null))
                                                {
                                                    double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                    double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                    CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                    CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                    //////////////////////////////////////////
                                                    ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                    lbThreasoldColor.Content = CurrentStep.Get2;
                                                    lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
                                                    tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
                                                    tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);

                                                }
                                            }
                                            break;
                                        /////////////////////
                                        case ModelLogic.Step.Functions.Matchtemplate:
                                            double ThreasoldDouble = 0;
                                            bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
                                            bdParameterMatchTemplate.Visibility = Visibility.Visible;
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                            if (isNumeric == true)
                                            {
                                                string Threasold = tb_Threasold.Text;
                                                CurrentStep.Set1 = Threasold;
                                                if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
                                                    ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
                                                tb_Threasold.Background = new SolidColorBrush(Colors.White);
                                            }
                                            else
                                                tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
                                            break;
                                        default:
                                            break;
                                    }

                                    string FUNCTIONTEST = cbb_Function.Text;
                                    switch (FUNCTIONTEST)
                                    {
                                        case "Read_QR":
                                            if (Imagee != null)
                                            {
                                                CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
                                                ImageTest.Source = CurrentStep.ImageTestReuslt;
                                            }
                                            break;
                                        ////////////////////
                                        case "Polarized_Capacitors":
                                            var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
                                            if (position_Polarized != 10)
                                                ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                            bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                            lbpolarization.Content = CurrentStep.Get1;
                                            break;
                                        /////////////////////
                                        case "Ceramic_Capacitors":
                                            double ThreasoldDoubleTop = 0;
                                            double ThreasoldDoubleBottom = 0;
                                            bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
                                            bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);

                                            if (isNumericTop == true && isNumericBottom == true)
                                            {
                                                var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                                bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                                if ((Position_Ceramic != null) && (Imagee != null))
                                                {
                                                    double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                    double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                    CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                    CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                    //////////////////////////////////////////
                                                    CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
                                                    CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
                                                    ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                    lbThreasoldColor.Content = CurrentStep.Get2;
                                                    lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
                                                    tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
                                                    tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);

                                                }

                                            }
                                            else
                                            {
                                                var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
                                                bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
                                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
                                                if ((Position_Ceramic != null) && (Imagee != null))
                                                {
                                                    double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
                                                    double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
                                                    CurrentStep.Set2 = lbNguongTren.Content.ToString();
                                                    CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
                                                    //////////////////////////////////////////
                                                    ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
                                                    lbThreasoldColor.Content = CurrentStep.Get2;
                                                    lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
                                                    tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
                                                    tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);

                                                }
                                            }
                                            break;
                                        /////////////////////
                                        case "Matchtemplate":
                                            double ThreasoldDouble = 0;
                                            bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
                                            bdParameterMatchTemplate.Visibility = Visibility.Visible;
                                            bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
                                            if (isNumeric == true)
                                            {
                                                string Threasold = tb_Threasold.Text;
                                                CurrentStep.Set1 = Threasold;
                                                if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
                                                    ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
                                                    tb_Threasold.Background = new SolidColorBrush(Colors.White);
                                            }
                                            else
                                                tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                    }

                }

                Index = 0;

            }

        }

        private void dgrModelSteps_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            

        }

        private void Camera1_Canvas_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void ImageModelCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            // if (e.Key == Key.F2)
            //{
            //    if (ImageModel == null)
            //    {
            //        MessageBox.Show(" Choose step to test. ");
            //    }
            //    else
            //    {
            //        if (IcopModel.Steps.Count > 0)
            //        {
            //            Index = IcopModel.Steps.Count;
            //            dgrModelSteps.Focus();
            //            if (IcopModel.MergeSource != null)
            //            {
            //                if (CurrentStep != new ModelLogic.Step())
            //                {
            //                    //  Cv2.ImShow("dddd", IcopModel.MergeSource.ToMat()); đúng
            //                    var Imagee = CurrentStep.GetImage(IcopModel.MergeSource);
            //                    if (Imagee != null)
            //                    {
            //                        switch (CurrentStep.FUNC)
            //                        {
            //                            case ModelLogic.Step.Functions.Read_QR:
            //                                if (Imagee != null)
            //                                {
            //                                    CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
            //                                    ImageTest.Source = CurrentStep.ImageTestReuslt;
            //                                    //   GC.Collect();                                           
            //                                    //Cv2.ImShow("ssss", CurrentStep.ImageTestReuslt.ToMat());
            //                                }
            //                                break;
            //                            ////////////////////
            //                            case ModelLogic.Step.Functions.Polarized_Capacitors:
            //                                var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
            //                                if (position_Polarized != 10)
            //                                    ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
            //                                //   lb_Motathuattoan.Content = "Xác định vị trí cực của tụ phân cực ";
            //                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
            //                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                lbpolarization.Content = CurrentStep.Get1;
            //                                break;
            //                            /////////////////////
            //                            case ModelLogic.Step.Functions.Ceramic_Capacitors:
            //                                double ThreasoldDoubleTop = 0;
            //                                double ThreasoldDoubleBottom = 0;
            //                                CurrentStep.Set3 = ((double)0).ToString("F2");
            //                                CurrentStep.Set4 = ((double)0).ToString("F2");
            //                                bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
            //                                bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);
            //                                if (isNumericTop == true && isNumericBottom == true)
            //                                {
            //                                    var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
            //                                    bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
            //                                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                    if ((Position_Ceramic != null) && (Imagee != null))
            //                                    {
            //                                        double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
            //                                        double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
            //                                        CurrentStep.Set2 = lbNguongTren.Content.ToString();
            //                                        CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
            //                                        //////////////////////////////////////////
            //                                        CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
            //                                        CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
            //                                        ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
            //                                        // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
            //                                        lbThreasoldColor.Content = CurrentStep.Get2;
            //                                        lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
            //                                        tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
            //                                        tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
            //                                    bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
            //                                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                    if ((Position_Ceramic != null) && (Imagee != null))
            //                                    {
            //                                        double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
            //                                        double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
            //                                        CurrentStep.Set2 = lbNguongTren.Content.ToString();
            //                                        CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
            //                                        ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
            //                                        // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
            //                                        lbThreasoldColor.Content = CurrentStep.Get2;
            //                                        lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
            //                                        tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
            //                                        tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);
            //                                    }
            //                                }
            //                                break;
            //                            /////////////////////
            //                            case ModelLogic.Step.Functions.Matchtemplate:
            //                                double ThreasoldDouble = 0;
            //                                bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
            //                                bdParameterMatchTemplate.Visibility = Visibility.Visible;
            //                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
            //                                if (isNumeric == true)
            //                                {
            //                                    string Threasold = tb_Threasold.Text;
            //                                    CurrentStep.Set1 = Threasold;
            //                                    if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
            //                                        ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
            //                                    // lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh hiện (tại khu vực được chọn). ";
            //                                    tb_Threasold.Background = new SolidColorBrush(Colors.White);
            //                                }
            //                                else
            //                                    tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
            //                                //   MessageBox.Show("The threshold must be a double number of type and less than 1 ");
            //                                break;
            //                            default:
            //                                break;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        if (IcopProject.Steps.Count > 0)
            //        {
            //            Index = IcopProject.Steps.Count;
            //            dgrModelSteps.Focus();
            //            if (IcopProject.MergeSource != null)
            //            {
            //                if (CurrentStep != new ModelLogic.Step())
            //                {
            //                    var Imagee = CurrentStep.GetImage(IcopProject.MergeSource);
            //                    if (Imagee != null)
            //                    {
            //                        switch (CurrentStep.FUNC)
            //                        {
            //                            case ModelLogic.Step.Functions.Read_QR:
            //                                if (Imagee != null)
            //                                {
            //                                    CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
            //                                    ImageTest.Source = CurrentStep.ImageTestReuslt;
            //                                    //   GC.Collect();
            //                                }
            //                                break;
            //                            ////////////////////
            //                            case ModelLogic.Step.Functions.Polarized_Capacitors:
            //                                var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
            //                                if (position_Polarized != 10)
            //                                    ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
            //                                //   lb_Motathuattoan.Content = "Xác định vị trí cực của tụ phân cực ";
            //                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
            //                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                lbpolarization.Content = CurrentStep.Get1;
            //                                break;
            //                            /////////////////////
            //                            case ModelLogic.Step.Functions.Ceramic_Capacitors:
            //                                double ThreasoldDoubleTop = 0;
            //                                double ThreasoldDoubleBottom = 0;
            //                                bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
            //                                bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);
            //                                if (isNumericTop == true && isNumericBottom == true)
            //                                {
            //                                    var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
            //                                    bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
            //                                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                    if ((Position_Ceramic != null) && (Imagee != null))
            //                                    {
            //                                        double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
            //                                        double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
            //                                        CurrentStep.Set2 = lbNguongTren.Content.ToString();
            //                                        CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
            //                                        //////////////////////////////////////////
            //                                        CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
            //                                        CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
            //                                        ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
            //                                        // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
            //                                        lbThreasoldColor.Content = CurrentStep.Get2;
            //                                        lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
            //                                        tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
            //                                        tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
            //                                    bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
            //                                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                    if ((Position_Ceramic != null) && (Imagee != null))
            //                                    {
            //                                        double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
            //                                        double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
            //                                        CurrentStep.Set2 = lbNguongTren.Content.ToString();
            //                                        CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
            //                                        //////////////////////////////////////////
            //                                        //  CurrentStep.Set3 = ((double)tbThreasoldColorTop.Value).ToString("F2");
            //                                        //  CurrentStep.Set4 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
            //                                        ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
            //                                        // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
            //                                        lbThreasoldColor.Content = CurrentStep.Get2;
            //                                        lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
            //                                        tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
            //                                        tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);

            //                                    }
            //                                }
            //                                break;
            //                            /////////////////////
            //                            case ModelLogic.Step.Functions.Matchtemplate:
            //                                double ThreasoldDouble = 0;
            //                                bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
            //                                bdParameterMatchTemplate.Visibility = Visibility.Visible;
            //                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
            //                                if (isNumeric == true)
            //                                {
            //                                    string Threasold = tb_Threasold.Text;
            //                                    CurrentStep.Set1 = Threasold;
            //                                    if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
            //                                        ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
            //                                    // lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh hiện (tại khu vực được chọn). ";
            //                                    tb_Threasold.Background = new SolidColorBrush(Colors.White);
            //                                }
            //                                else
            //                                    tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
            //                                break;
            //                            default:
            //                                break;
            //                        }

            //                        string FUNCTIONTEST = cbb_Function.Text;
            //                        switch (FUNCTIONTEST)
            //                        {
            //                            case "Read_QR":
            //                                if (Imagee != null)
            //                                {
            //                                    CurrentStep.Get1 = Function.ReadQR(Imagee, CurrentStep);
            //                                    ImageTest.Source = CurrentStep.ImageTestReuslt;
            //                                    // GC.Collect();
            //                                }
            //                                break;
            //                            ////////////////////
            //                            case "Polarized_Capacitors":
            //                                var position_Polarized = CurrentStep.GetFuncImage_Capacitor(ImageModelCanvas, Imagee);
            //                                if (position_Polarized != 10)
            //                                    ImageTest.Source = Function.Polarized_Capacitors(CurrentStep, Imagee, position_Polarized);
            //                                //   lb_Motathuattoan.Content = "Xác định vị trí cực của tụ phân cực ";
            //                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
            //                                bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                lbpolarization.Content = CurrentStep.Get1;
            //                                break;
            //                            /////////////////////
            //                            case "Ceramic_Capacitors":
            //                                double ThreasoldDoubleTop = 0;
            //                                double ThreasoldDoubleBottom = 0;
            //                                bool isNumericTop = double.TryParse(tbThreasoldColorTop.Text.ToString(), out ThreasoldDoubleTop);
            //                                bool isNumericBottom = double.TryParse(tbThreasoldColorBottom.Text.ToString(), out ThreasoldDoubleBottom);

            //                                if (isNumericTop == true && isNumericBottom == true)
            //                                {
            //                                    var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
            //                                    bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
            //                                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                    if ((Position_Ceramic != null) && (Imagee != null))
            //                                    {
            //                                        double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
            //                                        double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
            //                                        CurrentStep.Set2 = lbNguongTren.Content.ToString();
            //                                        CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
            //                                        //////////////////////////////////////////
            //                                        CurrentStep.Set4 = ((double)tbThreasoldColorTop.Value).ToString("F2");
            //                                        CurrentStep.Set3 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
            //                                        ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
            //                                        // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
            //                                        lbThreasoldColor.Content = CurrentStep.Get2;
            //                                        lbThreasoldColor.Foreground = new SolidColorBrush(Colors.White);
            //                                        tbThreasoldColorTop.Background = new SolidColorBrush(Colors.White);
            //                                        tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.White);

            //                                    }

            //                                }
            //                                else
            //                                {
            //                                    var Position_Ceramic = CurrentStep.GetFuncImage_Ceramic_Capacitor(ImageModelCanvas, Imagee);
            //                                    bdParameterCeramicCapacitors.Visibility = Visibility.Visible;
            //                                    bdParameterMatchTemplate.Visibility = Visibility.Collapsed;
            //                                    if ((Position_Ceramic != null) && (Imagee != null))
            //                                    {
            //                                        double ThreasoldTop = double.Parse(lbNguongTren.Content.ToString());
            //                                        double ThreasoldBottom = double.Parse(lbNguongDuoi.Content.ToString());
            //                                        CurrentStep.Set2 = lbNguongTren.Content.ToString();
            //                                        CurrentStep.Set1 = lbNguongDuoi.Content.ToString();
            //                                        //////////////////////////////////////////
            //                                        //  CurrentStep.Set3 = ((double)tbThreasoldColorTop.Value).ToString("F2");
            //                                        //  CurrentStep.Set4 = ((double)tbThreasoldColorBottom.Value).ToString("F2");
            //                                        ImageTest.Source = Function.Ceramic_Capacitors(CurrentStep, Imagee, ThreasoldTop, ThreasoldBottom, Position_Ceramic[0], Position_Ceramic[1]);
            //                                        // lb_Motathuattoan.Content = " Kiểm tra sự tồn tại đối tượng trong ảnh bằng ngưỡng màu theo cài đặt. ";
            //                                        lbThreasoldColor.Content = CurrentStep.Get2;
            //                                        lbThreasoldColor.Foreground = new SolidColorBrush(Colors.Yellow);
            //                                        tbThreasoldColorTop.Background = new SolidColorBrush(Colors.Yellow);
            //                                        tbThreasoldColorBottom.Background = new SolidColorBrush(Colors.Yellow);

            //                                    }
            //                                }
            //                                break;
            //                            /////////////////////
            //                            case "Matchtemplate":
            //                                double ThreasoldDouble = 0;
            //                                bool isNumeric = double.TryParse(tb_Threasold.Text.ToString(), out ThreasoldDouble);
            //                                bdParameterMatchTemplate.Visibility = Visibility.Visible;
            //                                bdParameterCeramicCapacitors.Visibility = Visibility.Collapsed;
            //                                if (isNumeric == true)
            //                                {
            //                                    string Threasold = tb_Threasold.Text;
            //                                    CurrentStep.Set1 = Threasold;
            //                                    if (CurrentStep.GetFuncImage(ImageModelCanvas, Imagee))
            //                                        ImageTest.Source = Function.Matchtemplate(CurrentStep, Imagee, Threasold);
            //                                    // lb_Motathuattoan.Content = "Tìm kiếm đối tượng tương đồng trong hình ảnh hiện (tại khu vực được chọn). ";
            //                                    tb_Threasold.Background = new SolidColorBrush(Colors.White);
            //                                }
            //                                else
            //                                    tb_Threasold.Background = new SolidColorBrush(Colors.Yellow);
            //                                break;
            //                            default:
            //                                break;
            //                        }
            //                    }
            //                }
            //            }

            //        }

            //    }

            //    Index = 0;

            //}
        }
        
    }

}