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
using System.Xml.Linq;
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON.SubViews
{
    /// <summary>
    /// Interaction logic for WinNewWorkSpace.xaml
    /// </summary>
    public partial class WinNewWorkSpace : ChromelessWindow
    {
        private WinNewWorkSpace _form;
        public string _name = "";
        public WinNewWorkSpace()
        {
            InitializeComponent();
            _form = this;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _form.DialogResult = false;
            _form.Close();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tbxName.Text.Trim() == "")
            {
                UtilMethods.ShowMessageBox("Plesae enter the workspace's name correctly.");
                return;
            }
            _name = tbxName.Text;
            _form.DialogResult = true;
            _form.Close();
        }
    }
}
