using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace ICOP_3.ModelLogic
{
    public class Step : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        private string pm;
        private string get1;
        private string get2;
        private string get3;
        private string get4;
        private string set1;
        private string set2;
        private string set3;
        private string set4;
        private bool skip;
        private bool result;
        private int pcb;
        private string name; 
        public string Result { get; set; }
        public DateTime TestTime { get; set; }
        public enum Functions { Matchtemplate, Polarized_Capacitors, Ceramic_Capacitors, Read_QR };
        private Functions func;
        public Functions FUNC
        {
            get { return func; }
            set
            {
                if (func != value)
                {
                    func = value;
                    OnPropertyChanged(nameof(FUNC));
                }
            }
        }

        public string PM
        {
            get { return pm; }
            set
            {
                if (pm != value)
                {
                    pm = value;
                    OnPropertyChanged(nameof(PM));
                }
            }
        }
        public string Get1
        {
            get { return get1; }
            set
            {
                if (get1 != value)
                {
                    get1 = value;
                    OnPropertyChanged(nameof(Get1));

                }
            }
        }
        public string Get2
        {
            get { return get2; }
            set
            {
                if (get2 != value)
                {
                    get2 = value;
                    OnPropertyChanged(nameof(Get2));

                }
            }
        }
        public string Get3
        {
            get { return get3; }
            set
            {
                if (get3 != value)
                {
                    get3 = value;

                }
            }
        }
        public string Get4
        {
            get { return get4; }
            set
            {
                if (get4 != value)
                {
                    get4 = value;

                }
            }
        }
        public string Set1
        {
            get { return set1; }
            set
            {
                if (set1 != value)
                {
                    set1 = value;
                    OnPropertyChanged(nameof(Set1));

                }
            }
        }
        public string Set2
        {
            get { return set2; }
            set
            {
                if (set2 != value)
                {
                    set2 = value;
                    OnPropertyChanged(nameof(Set2));

                }
            }
        }
        public string Set3
        {
            get { return set3; }
            set
            {
                if (set3 != value)
                {
                    set3 = value;
                    OnPropertyChanged(nameof(Set3));

                }
            }
        }
        public string Set4
        {
            get { return set4; }
            set
            {
                if (set4 != value)
                {
                    set4 = value;
                    OnPropertyChanged(nameof(Set4));

                }
            }
        }
        public bool Skip
        {
            get { return skip; }
            set
            {
                if (skip != value)
                {
                    skip = value;
                    OnPropertyChanged(nameof(Skip));

                }
            }
        }
        public bool ResutlTest
        {
            get { return result; }
            set
            {
                if (result != value)
                {
                    if (value)
                    {

                        ManualLabelDisplay.Dispatcher.Invoke(new Action(() =>
                        {
                            ManualLabelDisplay.BorderBrush = new SolidColorBrush(Colors.Green);
                            ManualLabelDisplay.Foreground = new SolidColorBrush(Colors.Green);
                        }));

                        LabelDisplay.Dispatcher.Invoke(new Action(() =>
                        {
                            LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Green);
                            LabelDisplay.Foreground = new SolidColorBrush(Colors.Green);
                        }));

                        Result = "Pass";

                    }
                    else
                    {
                        ManualLabelDisplay.Dispatcher.Invoke(new Action(() =>
                        {
                            ManualLabelDisplay.BorderBrush = new SolidColorBrush(Colors.Red);
                            ManualLabelDisplay.Foreground = new SolidColorBrush(Colors.Red);
                        }));

                        LabelDisplay.Dispatcher.Invoke(new Action(() =>
                        {
                            LabelDisplay.BorderBrush = new SolidColorBrush(Colors.Red);
                            LabelDisplay.Foreground = new SolidColorBrush(Colors.Red);
                        }));
                        Result = "Fail";

                    }

                    result = value;
                    OnPropertyChanged(nameof(ResutlTest));
                }
            }
        }
        public BitmapSource ImageSource;  // image moddel
        public BitmapSource ImageTest;  // image from camera
        public BitmapSource ImageTestReuslt;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    //  History_.Name = value;
                    name = value;
                    Label.Content = name;
                    ManualLabelDisplay.Content = name;
                    LabelDisplay.Content = name;
                    OnPropertyChanged("Name");

                }
            }
        }

        public int PCB
        {
            get { return pcb; }
            set
            {
                if (pcb != value)
                {
                    pcb = value;
                    OnPropertyChanged(nameof(PCB));
                }
            }
        }
        public Step() { }

     //   public Step_copy() { }



        private Canvas parent;
        private Canvas Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                parentSize = new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = value.ActualWidth,
                    Height = value.ActualHeight
                };
            }
        }
        private Canvas display;
        private Canvas Display
        {
            get { return display; }
            set
            {
                display = value;
                displaySize = new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = value.ActualWidth,
                    Height = value.ActualHeight
                };
            }
        }
        private Canvas manualDisplay;
        private Canvas ManualDisplay
        {
            get { return manualDisplay; }
            set
            {
                ManualDisplaySize = new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = value.ActualWidth,
                    Height = value.ActualHeight
                };
                manualDisplay = value;
            }
        }
        public string DetectString = "";
        public BitmapSource DetectedImage;
        public BitmapSource DetectedImageModel;
        public BitmapSource DetectedImageProgram;
        private Rect displaySize;
        public Rect DisplaySize
        {
            get { return displaySize; }
            set { displaySize = value; }
        }
        private Rect parentSize;
        public Rect ParentSize
        {
            get { return parentSize; }
            set { parentSize = value; }
        }
        private Rect manualdisplaySize;
        public Rect ManualDisplaySize
        {
            get { return manualdisplaySize; }
            set { manualdisplaySize = value; }
        }
        private double[] rt = new double[2] { 1, 1 };
        public double[] raito
        {
            get { return rt; }
            set
            {
                if (value.Length == 2)
                {
                    rt = value;
                    Area = new Int32Rect((int)(rect.X * value[0]), (int)(rect.Y * value[1]), (int)(rect.Width * value[0]), (int)(rect.Height * value[1]));

                }
            }
        }
        private Int32Rect area = new Int32Rect(0, 0, 100, 50);
        public Int32Rect Area
        {
            get { return area; }
            set
            {
                area = value;

            }
        }

        private string data;
        public string Data
        {
            get { return data; }
            private set { data = value; }
        }
        private double threshold;
        public double Threshold
        { get { return threshold; } set { threshold = value; } }
        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                Label.Visibility = value;
                LabelDisplay.Visibility = value;
                ManualLabelDisplay.Visibility = value;
                if (value != Visibility.Visible)
                {
                    Keyboard.ClearFocus();
                }
            }
        }

        public bool IsForcus
        {
            get
            {
                bool isforcus = false;
                isforcus = isforcus || Label.IsFocused;
                isforcus = isforcus || LabelBotLeft.IsFocused;
                isforcus = isforcus || LabelBotMid.IsFocused;
                isforcus = isforcus || LabelBotRight.IsFocused;
                isforcus = isforcus || LabelTopLeft.IsFocused;
                isforcus = isforcus || LabelTopMid.IsFocused;
                isforcus = isforcus || LabelTopRight.IsFocused;
                isforcus = isforcus || LabelMidLeft.IsFocused;
                isforcus = isforcus || LabelMidRight.IsFocused;
                return isforcus;
            }

        }

        public Label ManualLabelDisplay = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Focusable = true,
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };

        public Label LabelDisplay = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(2),
            Focusable = true,

            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };

        public Label Label = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(2),
            Focusable = true,
            Cursor = Cursors.SizeAll,
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };
        public Label LabelTopLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNWSE,
            Focusable = true,
        };
        public Label LabelTopMid = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNS,
            Focusable = true,
        };
        public Label LabelTopRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNESW,
            Focusable = true,
        };
        public Label LabelMidLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeWE,
            Focusable = true,
        };
        public Label LabelMidRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeWE,
            Focusable = true,
        };
        public Label LabelBotLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNESW,
            Focusable = true,
        };
        public Label LabelBotMid = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNS,
            Focusable = true,
        };
        public Label LabelBotRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNWSE,
            Focusable = true,
        };

        private Rect OfsetMove = new Rect();

        public Rect rect = new Rect()
        {
            Location = new Point(0, 0),
            Size = new System.Windows.Size(10, 10)
        };

        public Rect rectDisplay = new Rect()
        {
            Location = new Point(0, 0),
            Size = new System.Windows.Size(10, 10)
        };

        public Rect manualRectDisplay = new Rect()
        {
            Location = new Point(0, 0),
            Size = new System.Windows.Size(10, 10)
        };

        public Rect Rect
        {
            get { return rect; }
            set
            {
                if (value.X > 0 && value.X < parentSize.Width - value.Width)
                {
                    rect.X = value.X;
                    rectDisplay.X = value.X * (displaySize.Width / parentSize.Width);
                    manualRectDisplay.X = value.X * (ManualDisplaySize.Width / parentSize.Width);
                    rect.Width = value.Width;
                    rectDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);
                    manualRectDisplay.Width = value.Width * (ManualDisplaySize.Width / parentSize.Width);
                    Label.Width = value.Width;
                    LabelDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);
                    ManualLabelDisplay.Width = value.Width * (ManualDisplaySize.Width / parentSize.Width);
                }

                if (value.Y > 0 && value.Y < parentSize.Height - value.Height)
                {
                    rect.Y = value.Y;
                    rectDisplay.Y = value.Y * (displaySize.Height / parentSize.Height);
                    manualRectDisplay.Y = value.Y * (ManualDisplaySize.Height / parentSize.Height);

                    rect.Height = value.Height;
                    rectDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);
                    manualRectDisplay.Height = value.Height * (ManualDisplaySize.Height / parentSize.Height);

                    Label.Height = value.Height;
                    LabelDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);
                    ManualLabelDisplay.Height = value.Height * (ManualDisplaySize.Height / parentSize.Height);
                }
                Area = new Int32Rect((int)(value.X * raito[0]), (int)(value.Y * raito[1]), (int)(value.Width * raito[0]), (int)(value.Height * raito[1]));
            }

        }
        #region Function support


        public Label FuncLabel = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Focusable = true,
            Cursor = Cursors.SizeAll,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "X",
        };

        public double FuncLabelX = 0;
        public double FuncLabelY = 0;
        public Int32Rect funcRect { get; set; }
        #endregion
        
        public Step(int index, string context, Canvas parent, Canvas Display, Canvas ManualDisplay)
        {
            this.Parent = parent;
            this.Display = Display;
            //this.ManualDisplay = ManualDisplay;
            Name = context;
            var mouse = Mouse.GetPosition(parent);
            this.Rect = new Rect()
            {
                //  X = parent.ActualWidth - 110,
                X = mouse.X,
                Y = mouse.Y,
                Width = 120,
                Height = 70,
            };

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

            Label.KeyDown += Label_KeyDown;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.MouseUp += Label_MouseUp;

            LabelBotLeft.Visibility = Visibility.Hidden;
            LabelBotMid.Visibility = Visibility.Hidden;
            LabelBotRight.Visibility = Visibility.Hidden;
            LabelMidLeft.Visibility = Visibility.Hidden;
            LabelMidRight.Visibility = Visibility.Hidden;
            LabelTopLeft.Visibility = Visibility.Hidden;
            LabelTopMid.Visibility = Visibility.Hidden;
            LabelTopRight.Visibility = Visibility.Hidden;

            LabelBotLeft.MouseMove += LabelBotLeft_MouseMove;
            LabelBotMid.MouseMove += LabelBotMid_MouseMove;
            LabelBotRight.MouseMove += LabelBotRight_MouseMove;
            LabelMidLeft.MouseMove += LabelMidLeft_MouseMove;
            LabelMidRight.MouseMove += LabelMidRight_MouseMove;
            LabelTopLeft.MouseMove += LabelTopLeft_MouseMove;
            LabelTopMid.MouseMove += LabelTopMid_MouseMove;
            LabelTopRight.MouseMove += LabelTopRight_MouseMove;

            LabelBotLeft.MouseDown += LabelResize_MouseDown;
            LabelBotMid.MouseDown += LabelResize_MouseDown;
            LabelBotRight.MouseDown += LabelResize_MouseDown;
            LabelMidLeft.MouseDown += LabelResize_MouseDown;
            LabelMidRight.MouseDown += LabelResize_MouseDown;
            LabelTopLeft.MouseDown += LabelResize_MouseDown;
            LabelTopMid.MouseDown += LabelResize_MouseDown;
            LabelTopRight.MouseDown += LabelResize_MouseDown;

            LabelBotLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelBotMid.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelBotRight.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelMidLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelMidRight.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopMid.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopRight.LostKeyboardFocus += Label_LostKeyboardFocus;


            LabelBotLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelBotMid.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelBotRight.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelMidLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelMidRight.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopMid.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopRight.GotKeyboardFocus += Label_GotKeyboardFocus;
        }

        public void ReInit(Canvas parent, Canvas Display, Canvas manualDisplay)
        {
            this.Parent = parent;
            this.Display = Display;
            //this.ManualDisplay = manualDisplay;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

            Label.KeyDown += Label_KeyDown;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.MouseUp += Label_MouseUp;

            LabelBotLeft.Visibility = Visibility.Hidden;
            LabelBotMid.Visibility = Visibility.Hidden;
            LabelBotRight.Visibility = Visibility.Hidden;
            LabelMidLeft.Visibility = Visibility.Hidden;
            LabelMidRight.Visibility = Visibility.Hidden;
            LabelTopLeft.Visibility = Visibility.Hidden;
            LabelTopMid.Visibility = Visibility.Hidden;
            LabelTopRight.Visibility = Visibility.Hidden;

            LabelBotLeft.MouseMove += LabelBotLeft_MouseMove;
            LabelBotMid.MouseMove += LabelBotMid_MouseMove;
            LabelBotRight.MouseMove += LabelBotRight_MouseMove;
            LabelMidLeft.MouseMove += LabelMidLeft_MouseMove;
            LabelMidRight.MouseMove += LabelMidRight_MouseMove;
            LabelTopLeft.MouseMove += LabelTopLeft_MouseMove;
            LabelTopMid.MouseMove += LabelTopMid_MouseMove;
            LabelTopRight.MouseMove += LabelTopRight_MouseMove;

            LabelBotLeft.MouseDown += LabelResize_MouseDown;
            LabelBotMid.MouseDown += LabelResize_MouseDown;
            LabelBotRight.MouseDown += LabelResize_MouseDown;
            LabelMidLeft.MouseDown += LabelResize_MouseDown;
            LabelMidRight.MouseDown += LabelResize_MouseDown;
            LabelTopLeft.MouseDown += LabelResize_MouseDown;
            LabelTopMid.MouseDown += LabelResize_MouseDown;
            LabelTopRight.MouseDown += LabelResize_MouseDown;

            LabelBotLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelBotMid.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelBotRight.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelMidLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelMidRight.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopMid.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopRight.LostKeyboardFocus += Label_LostKeyboardFocus;


            LabelBotLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelBotMid.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelBotRight.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelMidLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelMidRight.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopMid.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopRight.GotKeyboardFocus += Label_GotKeyboardFocus;

        }

        private void Label_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            Rect areaRect = Rect;
            double distanceMove = 1;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                distanceMove = 30;
            }
            switch (e.Key)
            {
                case Key.Left:
                    areaRect.X = rect.X - distanceMove;
                    break;
                case Key.Up:
                    areaRect.Y = rect.Y - distanceMove;
                    break;
                case Key.Right:
                    areaRect.X = rect.X + distanceMove;
                    break;
                case Key.Down:
                    areaRect.Y = rect.Y + distanceMove;
                    break;
            }
            Rect = areaRect;
            SetPosition();
        }

        private void LabelResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Keyboard.Focus(sender as Label);
            Keyboard.Focus(sender as Label);
        }

        private void LabelTopRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Console.WriteLine("Label top right event");
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(Parent).X - rect.X);
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                areaRect.Y = e.GetPosition(Parent).Y;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelTopMid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                areaRect.Y = e.GetPosition(Parent).Y;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelTopLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(Parent).X);
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                areaRect.Y = e.GetPosition(Parent).Y;
                areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelMidRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(Parent).X - areaRect.X);
                //areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                //areaRect.Y = e.GetPosition(Parent).Y;
                //areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelMidLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(Parent).X);
                //areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                //areaRect.Y = e.GetPosition(Parent).Y;
                areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(Parent).X - rect.X);
                areaRect.Height = Math.Abs(e.GetPosition(Parent).Y - rect.Y);
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotMid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
              //  areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(Parent).X);
                areaRect.Height = Math.Abs(e.GetPosition(Parent).Y - areaRect.Y);
               // areaRect.Y = e.GetPosition(Parent).Y;
               // areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotLeft_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(Parent).X);
                areaRect.Height = Math.Abs(e.GetPosition(Parent).Y - rect.Y);
                //areaRect.Y = e.GetPosition(Parent).Y;
                areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                LabelBotLeft.MouseMove -= LabelBotLeft_MouseMove;
                LabelBotMid.MouseMove -= LabelBotMid_MouseMove;
                LabelBotRight.MouseMove -= LabelBotRight_MouseMove;
                LabelMidLeft.MouseMove -= LabelMidLeft_MouseMove;
                LabelMidRight.MouseMove -= LabelMidRight_MouseMove;
                LabelTopLeft.MouseMove -= LabelTopLeft_MouseMove;
                LabelTopMid.MouseMove -= LabelTopMid_MouseMove;
                LabelTopRight.MouseMove -= LabelTopRight_MouseMove;

                //e.Handled = true;
                Label.Cursor = Cursors.SizeAll;
                Keyboard.Focus(Label);


                OfsetMove = new Rect()
                {
                    Width = Math.Max(Math.Abs(e.GetPosition(Parent).X - rect.X), 5),
                    Height = Math.Max(Math.Abs(e.GetPosition(Parent).Y - rect.Y), 5),
                };

            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Label raise event");
            if (e.LeftButton == MouseButtonState.Pressed && !e.Handled && Label.IsKeyboardFocused)
            {
                LabelBotLeft.MouseMove -= LabelBotLeft_MouseMove;
                LabelBotMid.MouseMove -= LabelBotMid_MouseMove;
                LabelBotRight.MouseMove -= LabelBotRight_MouseMove;
                LabelMidLeft.MouseMove -= LabelMidLeft_MouseMove;
                LabelMidRight.MouseMove -= LabelMidRight_MouseMove;
                LabelTopLeft.MouseMove -= LabelTopLeft_MouseMove;
                LabelTopMid.MouseMove -= LabelTopMid_MouseMove;
                LabelTopRight.MouseMove -= LabelTopRight_MouseMove;

                e.Handled = true;
                Rect areaRect = Rect;
                areaRect.X = e.GetPosition(Parent).X - OfsetMove.Width;
                areaRect.Y = e.GetPosition(Parent).Y - OfsetMove.Height;
                Rect = areaRect;
                SetPosition();
            }

            if (e.LeftButton == MouseButtonState.Pressed && (e.Source as FrameworkElement) == Label)
            {
                var focusElement = Keyboard.FocusedElement;
                if (focusElement != null && focusElement.GetType() == typeof(Label))
                {
                    Console.WriteLine(sender.ToString() + " + " + focusElement.ToString());
                    focusElement.RaiseEvent(e);
                    Console.WriteLine("Fire event to focus Element");
                    //Canvas.SetTop((Label)focusElement, e.GetPosition(DrawingCanvas).Y);
                    //Canvas.SetRight((Label)focusElement, e.GetPosition(DrawingCanvas).X);
                }
            }
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LabelBotLeft.MouseMove += LabelBotLeft_MouseMove;
            LabelBotMid.MouseMove += LabelBotMid_MouseMove;
            LabelBotRight.MouseMove += LabelBotRight_MouseMove;
            LabelMidLeft.MouseMove += LabelMidLeft_MouseMove;
            LabelMidRight.MouseMove += LabelMidRight_MouseMove;
            LabelTopLeft.MouseMove += LabelTopLeft_MouseMove;
            LabelTopMid.MouseMove += LabelTopMid_MouseMove;
            LabelTopRight.MouseMove += LabelTopRight_MouseMove;
        }

        private void Label_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            LabelBotLeft.Visibility = Visibility.Hidden;
            LabelBotMid.Visibility = Visibility.Hidden;
            LabelBotRight.Visibility = Visibility.Hidden;
            LabelMidLeft.Visibility = Visibility.Hidden;
            LabelMidRight.Visibility = Visibility.Hidden;
            LabelTopLeft.Visibility = Visibility.Hidden;
            LabelTopMid.Visibility = Visibility.Hidden;
            LabelTopRight.Visibility = Visibility.Hidden;

        }

        private void Label_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            LabelBotLeft.Visibility = Visibility.Visible;
            LabelBotMid.Visibility = Visibility.Visible;
            LabelBotRight.Visibility = Visibility.Visible;
            LabelMidLeft.Visibility = Visibility.Visible;
            LabelMidRight.Visibility = Visibility.Visible;
            LabelTopLeft.Visibility = Visibility.Visible;
            LabelTopMid.Visibility = Visibility.Visible;
            LabelTopRight.Visibility = Visibility.Visible;
        }

        public void PlaceIn(Canvas placeCanvas, Canvas displayCanvas, Canvas manualCanvasDisplay)
        {
            Parent = placeCanvas;
            Display = displayCanvas;
            //ManualDisplay = manualCanvasDisplay;

            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);

            Canvas.SetTop(this.LabelTopLeft, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopLeft, rect.X - 2);

            Canvas.SetTop(this.LabelTopMid, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopMid, rect.X - 2 + Label.Width / 2);

            Canvas.SetTop(this.LabelTopRight, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopRight, rect.X - 3 + Label.Width);

            Canvas.SetTop(this.LabelMidLeft, rect.Y - 2 + Label.Height / 2);
            Canvas.SetLeft(this.LabelMidLeft, rect.X - 2);

            Canvas.SetTop(this.LabelMidRight, rect.Y - 2 + Label.Height / 2);
            Canvas.SetLeft(this.LabelMidRight, rect.X - 3 + Label.Width);

            Canvas.SetTop(this.LabelBotLeft, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotLeft, rect.X - 2);

            Canvas.SetTop(this.LabelBotMid, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotMid, rect.X - 2 + Label.Width / 2);

            Canvas.SetTop(this.LabelBotRight, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotRight, rect.X - 3 + Label.Width);

           displayCanvas.Children.Add(LabelDisplay);

            //manualCanvasDisplay.Children.Add(ManualLabelDisplay);
            //Canvas.SetTop(this.ManualLabelDisplay, manualRectDisplay.Y);
            //Canvas.SetLeft(this.ManualLabelDisplay, manualRectDisplay.X);

            placeCanvas.Children.Add(Label);

            placeCanvas.Children.Add(LabelTopLeft);
            placeCanvas.Children.Add(LabelTopMid);
            placeCanvas.Children.Add(LabelTopRight);

            placeCanvas.Children.Add(LabelMidLeft);
            placeCanvas.Children.Add(LabelMidRight);

            placeCanvas.Children.Add(LabelBotLeft);
            placeCanvas.Children.Add(LabelBotMid);
            placeCanvas.Children.Add(LabelBotRight);
        }

        public void PlaceIn(Canvas displayCanvas, Canvas manualCanvasDisplay)
        {
            Display = displayCanvas;
            ManualDisplay = manualCanvasDisplay;

            Rect rect = this.rect;
            Rect = rect;

            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);

            Canvas.SetTop(this.ManualLabelDisplay, manualRectDisplay.Y);
            Canvas.SetLeft(this.ManualLabelDisplay, manualRectDisplay.X);

            displayCanvas.Children.Add(LabelDisplay);
            manualCanvasDisplay.Children.Add(ManualLabelDisplay);

        }

        public void SetPosition()
        {
            //rect.X = e.GetPosition(placeCanvas).X;
            //rect.Y = e.GetPosition(placeCanvas).Y;

            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

            Canvas.SetTop(this.ManualLabelDisplay, manualRectDisplay.Y);
            Canvas.SetLeft(this.ManualLabelDisplay, manualRectDisplay.X);

            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);

            Canvas.SetTop(this.LabelTopLeft, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopLeft, rect.X - 2);

            Canvas.SetTop(this.LabelTopMid, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopMid, rect.X - 2 + Label.Width / 2);

            Canvas.SetTop(this.LabelTopRight, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopRight, rect.X - 3 + Label.Width);

            Canvas.SetTop(this.LabelMidLeft, rect.Y - 2 + Label.Height / 2);
            Canvas.SetLeft(this.LabelMidLeft, rect.X - 2);

            Canvas.SetTop(this.LabelMidRight, rect.Y - 2 + Label.Height / 2);
            Canvas.SetLeft(this.LabelMidRight, rect.X - 3 + Label.Width);

            Canvas.SetTop(this.LabelBotLeft, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotLeft, rect.X - 2);

            Canvas.SetTop(this.LabelBotMid, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotMid, rect.X - 2 + Label.Width / 2);

            Canvas.SetTop(this.LabelBotRight, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotRight, rect.X - 3 + Label.Width);

            Console.WriteLine("rect: " + rect.X + " -- " + rect.Y);
        }

        public void SetFuncLabel(Canvas FuncCanvas, MouseButtonEventArgs e)
        {
            FuncCanvas.Children.Clear();
            FuncCanvas.Children.Add(FuncLabel = new Label()
            {
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 255, 0, 0)),
                Foreground = new SolidColorBrush(Colors.Red),
                BorderBrush = new SolidColorBrush(Colors.Red),
                BorderThickness = new Thickness(1),
                Focusable = true,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = "X",
            });
            Canvas.SetTop(FuncLabel, e.GetPosition(FuncCanvas).Y);
            Canvas.SetLeft(FuncLabel, e.GetPosition(FuncCanvas).X);

            FuncLabelX = e.GetPosition(FuncCanvas).X;
            FuncLabelY = e.GetPosition(FuncCanvas).Y;
        }

        public void PlaceFuncLabel(Canvas FuncCanvas)
        {
            FuncCanvas.Children.Clear();
            FuncCanvas.Children.Add(FuncLabel);

            var x = funcRect.X * (FuncCanvas.ActualWidth / Area.Width);
            var w = funcRect.Width * (FuncCanvas.ActualWidth / Area.Width);
            var y = funcRect.Y * (FuncCanvas.ActualHeight / Area.Height);
            var h = funcRect.Height * (FuncCanvas.ActualHeight / Area.Height);

            bool IsNewLabel = true;
            IsNewLabel = IsNewLabel && (x < 0);
            IsNewLabel = IsNewLabel && (y < 0);
            IsNewLabel = IsNewLabel && (w < 0);
            IsNewLabel = IsNewLabel && (h < 0);

            if (!IsNewLabel)
            {
                FuncLabel.Width = w;
                FuncLabel.Height = h;

                Canvas.SetTop(FuncLabel, y);
                Canvas.SetLeft(FuncLabel, x);
            }
        }

        public void DrawFuncLabel(Canvas FuncCanvas, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                FuncLabel.Width = Math.Abs(e.GetPosition(FuncCanvas).X - Canvas.GetLeft(FuncLabel));
                FuncLabel.Height = Math.Abs(e.GetPosition(FuncCanvas).Y - Canvas.GetTop(FuncLabel));
              
            }
        }

        public bool GetFuncImage(Canvas FunctionCanvas, BitmapSource originImage)
        {
            DetectedImage = null;
            if (originImage != null)
            {
                var raito = new double[2]
                    {
                        originImage.Width/FunctionCanvas.ActualWidth,
                        originImage.Height/FunctionCanvas.ActualHeight
                    };
                try
                {
                    Int32Rect funcArea = new Int32Rect()
                    {
                        X = (int)(FuncLabelX * raito[0]),
                        Y = (int)(FuncLabelY * raito[1]),
                        Width = (int)(FuncLabel.ActualWidth * raito[0]),
                        Height = (int)(FuncLabel.ActualHeight * raito[1])
                    };

                    funcRect = funcArea;

                    DetectedImage = new CroppedBitmap(originImage, funcArea);
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool GetFuncImage(BitmapSource originImage)
        {
            DetectedImage = null;
            if (originImage != null)
            {
                try
                {
                    DetectedImage = new CroppedBitmap(originImage, funcRect);
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public CroppedBitmap GetImage(BitmapSource ImageSource)
        {
            if (ImageSource != null)
            {
                raito = new double[2]
                    {
                        ImageSource.Width/parentSize.Width,
                        ImageSource.Height/parentSize.Height
                    };
                CroppedBitmap ImageResult = new CroppedBitmap(ImageSource, Area);
                return ImageResult;
            }
            return null;
        }

        public int GetFuncImage_Capacitor(Canvas FunctionCanvas_Capacitor, BitmapSource originImage_Capacitor)
        {
            DetectedImage = null;
            if (originImage_Capacitor != null)
            {
                var raito = new double[2]
                    {
                        originImage_Capacitor.Width/FunctionCanvas_Capacitor.ActualWidth,
                        originImage_Capacitor.Height/FunctionCanvas_Capacitor.ActualHeight
                    };
                Int32Rect funcArea = new Int32Rect()
                {
                    X = (int)(FuncLabelX * raito[0]),
                    Y = (int)(FuncLabelY * raito[1]),
                    Width = (int)(FuncLabel.ActualWidth * raito[0]),
                    Height = (int)(FuncLabel.ActualHeight * raito[1])
                };
                funcRect = funcArea;
                bool funcAreaAvailable = true;
                funcAreaAvailable = funcAreaAvailable && (originImage_Capacitor.Width > funcArea.X);
                funcAreaAvailable = funcAreaAvailable && (originImage_Capacitor.Height > funcArea.Y);
                funcAreaAvailable = funcAreaAvailable && (0 < funcArea.X);
                funcAreaAvailable = funcAreaAvailable && (0 < funcArea.Y);

                funcAreaAvailable = funcAreaAvailable && (originImage_Capacitor.Width > (funcArea.X + funcArea.Width));
                funcAreaAvailable = funcAreaAvailable && (originImage_Capacitor.Height > (funcArea.Y + funcArea.Height));
                funcAreaAvailable = funcAreaAvailable && (0 < (funcArea.X + funcArea.Width));
                funcAreaAvailable = funcAreaAvailable && (0 < (funcArea.Y + funcArea.Height));

                if (!funcAreaAvailable)
                {
                    return 10;
                }

                DetectedImage = new CroppedBitmap(originImage_Capacitor, funcArea);
                var WidthRect = originImage_Capacitor.Width / 3;
                var HightRect = originImage_Capacitor.Height / 3;
                var positionsRect_Capacitor = 10;
                positionsRect_Capacitor = ((funcArea.X + funcArea.Width / 2) / (int)WidthRect) + ((funcArea.Y + funcArea.Height / 2) / (int)HightRect) * 3 + 1;
                return positionsRect_Capacitor;
            }
            return 10;
        }

        /// <summary>
        /// Lay hinh anh tu hinh anh da cat tu hinh anh duoc ghep
        /// </summary>
        /// <param name="FunctionCanvas"></param>
        /// <param name="originImage"></param>
        /// <returns></returns>
        public int[] GetFuncImage_Ceramic_Capacitor(Canvas FunctionCanvas, BitmapSource originImage)
        {
            DetectedImage = null;
            if (originImage != null)
            {
                var raito = new double[2]
                    {
                        originImage.Width/FunctionCanvas.ActualWidth,
                        originImage.Height/FunctionCanvas.ActualHeight
                    };
                try
                {
                    Int32Rect funcArea = new Int32Rect()
                    {
                        X = (int)(FuncLabelX * raito[0]),
                        Y = (int)(FuncLabelY * raito[1]),
                        Width = (int)(FuncLabel.Width * raito[0]),
                        Height = (int)(FuncLabel.Height * raito[1])
                    };
                    DetectedImage = new CroppedBitmap(originImage, funcArea);
                    return new int[] { funcArea.X, funcArea.Y };
                }
                catch (Exception)
                {

                }
                return null;
            }
            return null;
        }


        public static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }

        //public BitmapSource RatioProgram(Step step, BitmapSource ImageModel, Canvas CanvasModel, BitmapSource ImageProgram, Canvas CanvasProgram)
        //{
        //    {

        //        var ratioModel = new double[2]
        //        {
        //            ImageModel.Width/CanvasModel.ActualWidth,
        //            ImageModel.Height/CanvasModel.ActualHeight,
        //        };

        //        Int32Rect funcAreaModel = new Int32Rect()
        //        {
        //            X = (int)(funcRect.X * ratioModel[0]),
        //            Y = (int)(funcRect.Y * ratioModel[1]),
        //            Width = (int)(funcRect.Width * ratioModel[0]),
        //            Height = (int)(funcRect.Height * ratioModel[1]),
        //        };
        //        DetectedImageModel = new CroppedBitmap(ImageModel, funcAreaModel);
        //        return DetectedImageModel;

        //        var ratioProgram = new double[2]
        //        {
        //            ImageProgram.Width/CanvasProgram.ActualWidth,
        //            ImageProgram.Height/CanvasProgram.ActualHeight,
        //        };

        //        Int32Rect funcAreaProgram = new Int32Rect()
        //        {
        //            X = (int)(funcRect.X * ratioModel[0] * ratioProgram[0]),
        //            Y = (int)(funcRect.Y * ratioModel[1] * ratioProgram[1]),
        //            Width = (int)(funcRect.Width * ratioModel[0] * ratioProgram[0]),
        //            Height = (int)(funcRect.Height * ratioModel[1] * ratioProgram[1]),
        //        };
        //        DetectedImageProgram = new CroppedBitmap(ImageProgram, funcAreaProgram);
        //        return DetectedImageProgram;
        //    }
        //    return null;
        //}


    }

}



