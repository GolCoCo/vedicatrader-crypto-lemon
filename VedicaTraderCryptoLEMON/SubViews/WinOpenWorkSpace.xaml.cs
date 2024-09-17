using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.IO;
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
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON.SubViews
{
    /// <summary>
    /// Interaction logic for WinNewWorkSpace.xaml
    /// </summary>
    public partial class WinOpenWorkSpace : ChromelessWindow
    {
        private WinOpenWorkSpace _form;
        public string _name = "";
        public WinOpenWorkSpace()
        {
            InitializeComponent();
            _form = this;
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] allfiles = Directory.GetFiles(HardCode.STR_SAVE_FOLDER_PATH, "*.cfg");
                foreach (string file in allfiles)
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    this.cbxName.Items.Add(name);
                }
                if (allfiles.Length != 0) this.cbxName.SelectedIndex = 0;
                GC.Collect();
            }
            catch (Exception)
            {

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _form.DialogResult = false;
            _form.Close();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (_name.Trim() == "")
            {
                UtilMethods.ShowMessageBox("Plesae enter the workspace's name correctly.");
                return;
            }
            _form.DialogResult = true;
            _form.Close();
        }

        private void cbxName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _name = this.cbxName.SelectedItem.ToString();
        }


    }
}
