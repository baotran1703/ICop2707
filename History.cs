using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace ICOP_3
{
    public class History 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Result { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;

                }
            }
        }
        public string ImageResultName { get; set; }
        public DateTime TestTime { get; set; }
        public int PCB { get; set; }
        private Functions func;
        public Functions FUNC
        {
            get { return func; }
            set
            {
                if (func != value)
                {
                    func = value;
                    NotifyPropertyChanged();
                }
            }
        }
        //private string get1;
        //private string get2;
        //private string get3;
        //private string get4;
        //private string set1;
        //private string set2;
        //private string set3;
        //private string set4;
        //private bool skip;
        //public string Get1
        //{
        //    get { return get1; }
        //    set
        //    {
        //        if (get1 != value)
        //        {
        //            get1 = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}
        //public string Get2
        //{
        //    get { return get2; }
        //    set
        //    {
        //        if (get2 != value)
        //        {
        //            get2 = value;

        //            NotifyPropertyChanged();
        //        }
        //    }
        //}
        //public string Get3
        //{
        //    get { return get3; }
        //    set
        //    {
        //        if (get3 != value)
        //        {
        //            get3 = value;

        //        }
        //    }
        //}
        //public string Get4
        //{
        //    get { return get4; }
        //    set
        //    {
        //        if (get4 != value)
        //        {
        //            get4 = value;

        //        }
        //    }
        //}
        //public string Set1
        //{
        //    get { return set1; }
        //    set
        //    {
        //        if (set1 != value)
        //        {
        //            set1 = value;

        //            NotifyPropertyChanged();
        //        }
        //    }
        //}
        //public string Set2
        //{
        //    get { return set2; }
        //    set
        //    {
        //        if (set2 != value)
        //        {
        //            set2 = value;

        //            NotifyPropertyChanged();
        //        }
        //    }
        //}
        //public string Set3
        //{
        //    get { return set3; }
        //    set
        //    {
        //        if (set3 != value)
        //        {
        //            set3 = value;

        //            NotifyPropertyChanged();
        //        }
        //    }
        //}
        //public string Set4
        //{
        //    get { return set4; }
        //    set
        //    {
        //        if (set4 != value)
        //        {
        //            set4 = value;
        //            NotifyPropertyChanged();

        //        }
        //    }
        //}
        //public bool Skip
        //{
        //    get { return skip; }
        //    set
        //    {
        //        if (skip != value)
        //        {
        //            skip = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}
        public int NumberNG1 ;
        public int NumberNG2 ;
        public int NumberNG3 ;
        public int NumberNG4 ;
        public int NumberOK1 ;
        public int NumberOK2 ;
        public int NumberOK3 ;
        public int NumberOK4 ;


        public History() { }
        public List<NameQR> NameQRs { get; set; }
        public List<HistoryItem> HistoryItems { get; set; }
        
    }

    public class HistoryItem
    {

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;

                }
            }
        }

        public enum PM_STEP
        {
            P = 0,
            M = 1,
        }

        private PM_STEP pm;

        public string PM
        {
            get { return pm.ToString(); }
            set
            {
                pm = value == "P" ? PM_STEP.P : PM_STEP.M;
            }
        }

        public int PCB { get; set; }

        private Functions func;

        public Functions FUNC
        {

            get { return func; }
            set
            {
                if (func != value)
                {
                    func = value;
                }
            }
        }

        private string get1;
        private string get2;
        private string get3;
        private string get4;
        private string set1;
        private string set2;
        private string set3;
        private string set4;
        private bool skip;
        public string Get1
        {
            get { return get1; }
            set
            {
                if (get1 != value)
                {
                    get1 = value;

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

                }
            }
        }
        private string result;
        public string Result
        {
            get { return result; }
            set
            {
                if (result != value)
                {
                    result = value;

                }
            }
        }
        bool ResultTest { get; set; }
       
       
    }


    public class NameQR
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;

                }
            }
        }
    }
}