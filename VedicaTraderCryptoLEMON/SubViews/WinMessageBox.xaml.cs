using Syncfusion.Windows.Shared;
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

namespace VedicaTraderCryptoLEMON.SubViews
{
    /// <summary>
    /// Interaction logic for WinMessageBox.xaml
    /// </summary>
    public partial class WinMessageBox : ChromelessWindow
    {
        private WinMessageBox _form;
        public WinMessageBox(string content)
        {
            InitializeComponent();
            _form = this;
            tbxContent.Text = content;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _form.Close();
        }
    }
}
