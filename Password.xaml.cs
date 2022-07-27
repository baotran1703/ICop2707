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
using System.Windows.Shapes;

namespace ICOP_3
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Password : Window
    {
        public Password()
        {
            InitializeComponent();
        }



        private void btPassword_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (pbBoxPassWord.Password == "123456")
            {
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
                this.Close();
            }

        }
    }
}

