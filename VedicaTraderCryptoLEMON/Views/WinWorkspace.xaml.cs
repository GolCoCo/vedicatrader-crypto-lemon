using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VedicaTraderCryptoLEMON.Models;

namespace VedicaTraderCryptoLEMON.Views
{
    /// <summary>
    /// Interaction logic for WinWorkSpace.xaml
    /// </summary>
    public partial class WinWorkspace : ChromelessWindow
    {

        private Timer _stimer = null;
        public WinWorkspace()
        {
            InitializeComponent();
            RunTimerForPCDateTime();
        }

        public void RunTimerForPCDateTime()
        {
            _stimer = new Timer();
            _stimer.Elapsed += new ElapsedEventHandler(OnPCTimeUpdate);
            _stimer.Interval = 1000;
            _stimer.Enabled = true;
            _stimer.Start();
        }

        private void OnPCTimeUpdate(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.lblPCDateTime.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            });
        }

        private void ChromelessWindow_Closed(object sender, EventArgs e)
        {
            if (_stimer != null) _stimer.Stop();
        }
    }
}
