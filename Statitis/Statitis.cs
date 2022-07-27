using ICOP_3.ModelLogic;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ICOP_3.Statitis
{
    public class Statitis
    {
        public Statitis statitis { get; set; }
        public int OK_Number { get; set; }
        public int NG_Number { get; set; }
        public int Total { get; set; }
        public int FakeNG_Number { get; set; }
        public int LiftNumber { get; set; }
        public int MissingNumber { get; set; }
        public int ReverseNumber { get; set; }
        public int WrongInsertNumber { get; set; }
    }



}