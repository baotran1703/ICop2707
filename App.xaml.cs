﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ICOP_3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.StackTrace.ToString(), "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            //HVT.Utility.Extensions.LogErr(e.Exception.StackTrace.ToString());
            e.Handled = true;
        }



    }
}
