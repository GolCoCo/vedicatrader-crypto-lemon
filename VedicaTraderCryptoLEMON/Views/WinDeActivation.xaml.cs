using Newtonsoft.Json.Linq;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using VedicaTraderCryptoLEMON.Controllers;
using VedicaTraderCryptoLEMON.Models;
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON.Views
{
    /// <summary>
    /// Interaction logic for WinMain.xaml
    /// </summary>
    public partial class WinDeActivation : ChromelessWindow
    {
        private WinDeActivation _form;
        private ConfigModel _configModel;
        private bool _Success = true;
        private SupabaseManager _supabaseManager;
        public WinDeActivation()
        {
            _configModel = ConfigModel.Load<ConfigModel>();
            if(_configModel == null)
            {
                _configModel = new ConfigModel();
            }
            InitializeComponent();
            _form = this;
            this.tbxLicenseKey.Text = _configModel._LicenseKey;
            this.tbxInstanceID.Text = _configModel._InstanceID;
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _supabaseManager = new SupabaseManager();
                _supabaseManager.IntializeClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnDeActivate_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                var postData = new Dictionary<string, string>
            {
                { "license_key", _configModel._LicenseKey },
                { "instance_id", _configModel._InstanceID }
            };

                var requestContent = new FormUrlEncodedContent(postData);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await client.PostAsync(HardCode.DEACTIVATE_URL, requestContent);

                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    JObject jsonObject = JObject.Parse(responseContent);

                    string license = (string)jsonObject["license_key"]["key"];
                    if (license != _configModel._LicenseKey)
                    {
                        UtilMethods.ShowMessageBox("Could not Deactivate. License Key was not matched.");
                        Application.Current.Shutdown();
                    }

                    bool deactivated = (bool)jsonObject["deactivated"];
                    if (!deactivated)
                    {
                        UtilMethods.ShowMessageBox("Could not Deactivate. Please check the license key again.");
                        Application.Current.Shutdown();
                    }
                    string status = (string)jsonObject["license_key"]["status"];
                    if (status != "inactive")
                    {
                        UtilMethods.ShowMessageBox("Could not Deactivate. License was not activated.");
                        Application.Current.Shutdown();
                    }

                    int activation_usage = (int)jsonObject["license_key"]["activation_usage"];
                    if (activation_usage > 0)
                    {
                        UtilMethods.ShowMessageBox("Could not Deactivate. Activation Usage is not valid.");
                        Application.Current.Shutdown();
                    }

                    UtilMethods.ShowMessageBox("License Key Is Successfully Unregistered.");
                    tbxInstanceID.Text = "";
                    tbxLicenseKey.Text = "";
                }
                else
                {
                    UtilMethods.ShowMessageBox("Could not deactivate the license key. please check again.");
                }
                _configModel._LicenseKey = null;
                _configModel._InstanceID = null;
                _configModel.Save();
                Console.WriteLine(responseContent);
            }
        }

        private void ChromelessWindow_Closed(object sender, EventArgs e)
        {
            if(!_Success) Application.Current.Shutdown();
        }

 
    }
}
