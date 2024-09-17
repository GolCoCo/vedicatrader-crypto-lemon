using Syncfusion.SfSkinManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VedicaTraderCryptoLEMON.Models;
using VedicaTraderCryptoLEMON.Utils;
using VedicaTraderCryptoLEMON.Views;
using Syncfusion.Windows.Shared;
using VedicaTraderCryptoLEMON.SubViews;
using static VedicaTraderCryptoLEMON.Controllers.Manager;

namespace VedicaTraderCryptoLEMON
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            LicenseKeyLocator.FindandRegisterLicenseKey();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            var window = new WinWorkspace();
            SfSkinManager.SetTheme(window, new Theme() { ThemeName = "Office2019Black" });
            window.Show();
            base.OnStartup(e);
        }
    }
}
