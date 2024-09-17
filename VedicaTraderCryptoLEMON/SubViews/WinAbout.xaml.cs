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
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON.SubViews
{
    /// <summary>
    /// Interaction logic for WinNewWorkSpace.xaml
    /// </summary>
    public partial class WinAbout : ChromelessWindow
    {
        private WinAbout _form;
        public WinAbout()
        {
            InitializeComponent();
            _form = this;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _form.Close();
        }
    }
}
