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
using static VedicaTraderCryptoLEMON.Controllers.Manager;

namespace VedicaTraderCryptoLEMON.Views
{
    /// <summary>
    /// Interaction logic for WinMain.xaml
    /// </summary>
    public partial class WinActivation : ChromelessWindow
    {
        private WinActivation _form;
        private ConfigModel _configModel;
        private bool _Success = false;
        private SupabaseManager _supabaseManager;
        private string _InstanceName = "";
        public WinActivation(ConfigModel configModel)
        {
            InitializeComponent();
            _form = this;
            _configModel = configModel;
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _supabaseManager = new SupabaseManager();
                _supabaseManager.IntializeClient();
                _InstanceName = UtilMethods.GetDeviceID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            string licenseKey = this.tbxLicenseKey.Text;
            if(licenseKey.Trim() == "")
            {
                UtilMethods.ShowMessageBox("Please type the license key.");
                return;
            }
            if (HardCode.IsTest)
            {
                await SendGetRequestForValidWithLicenseKeyTest(licenseKey);
            }
            else {

                SendActivateRequest(licenseKey);
            }
            
        }

        private async void SendActivateRequest(string licenseKey) {

            using (HttpClient client = new HttpClient())
            {
                var postData = new Dictionary<string, string>
            {
                { "license_key", licenseKey },
                { "instance_name", _InstanceName }
            };

                var requestContent = new FormUrlEncodedContent(postData);
                var response = await client.PostAsync(HardCode.LOGIN_URL, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    await CheckIfActivatedWithResponseAsync(responseContent, licenseKey);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(responseContent);
                    string error = (string)jsonObject["error"];
                    UtilMethods.ShowMessageBox($"{error}");
                    Application.Current.Shutdown();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    UtilMethods.ShowMessageBox("An Item could not be found");
                    Application.Current.Shutdown();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                {
                    UtilMethods.ShowMessageBox("A required field was invalid or missing. Try Again");
                    Application.Current.Shutdown();
                }
                else
                {

                }
            }
        }

        private async Task<bool> CheckIfExistDeviceIDBannedInSupabaseAsync(string deviceId)
        {
            List<SupaDeviceIDModel> list = await _supabaseManager.ReadAllDeviceIDBannedFromTableAsync();
            foreach (SupaDeviceIDModel s in list)
            {
                if (s._DeviceID == deviceId)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task SendPostActivateAsync(string licenseKey)
        {
            string url = HardCode.LOGIN_URL;

            using (HttpClient client = new HttpClient())
            {
                var postData = new StringContent($"product_id={HardCode.LOGIN_PRODUCT_ID}&license_key={licenseKey}", Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.PostAsync(url, postData);

                if (response.IsSuccessStatusCode)
                {
                    _configModel._LicenseKey = licenseKey;
                    string responseBody = await response.Content.ReadAsStringAsync();
                    await CheckIfActivatedWithResponseAsync(responseBody, licenseKey);
                    Console.WriteLine(responseBody);
                }
                else
                {
                    UtilMethods.ShowMessageBox("License Key is Invalid.");
                    Application.Current.Shutdown();
                }
            }
        }

        private async Task CheckIfActivatedWithResponseAsync(string responseBody, string licenseKey)
        {
            JObject jsonObject = JObject.Parse(responseBody);

            bool activated = (bool)jsonObject["activated"];
            if (!activated) {
                UtilMethods.ShowMessageBox("Could not Activate. Please check the license again.");
                Application.Current.Shutdown();
            }
            string status = (string)jsonObject["license_key"]["status"];
            if(status != "active")
            {
                UtilMethods.ShowMessageBox("Could not Activate. License was not activated.");
                Application.Current.Shutdown();
            }
            string license = (string)jsonObject["license_key"]["key"];
            if (license != licenseKey) {
                UtilMethods.ShowMessageBox("Could not Activate. License Key was not matched.");
                Application.Current.Shutdown();
            }

            int activation_limit = (int)jsonObject["license_key"]["activation_limit"];
            int activation_usage = (int)jsonObject["license_key"]["activation_usage"];
            if(activation_limit != 1 || activation_usage != 1)
            {
                UtilMethods.ShowMessageBox("Could not Activate. Activation Limit and Usage is not valid.");
                Application.Current.Shutdown();
            }

            string product_id = (string)jsonObject["meta"]["product_id"];
            if (product_id != HardCode.LOGIN_PRODUCT_ID) {
                UtilMethods.ShowMessageBox("Could not Activate. Product Id is not matched.");
                Application.Current.Shutdown();
            }            

            string deviceId = UtilMethods.GetDeviceID();
            bool isExisted = await CheckIfExistDeviceIDBannedInSupabaseAsync(deviceId);
            if (isExisted) {
                UtilMethods.ShowMessageBox("This device has been banned in this platform.\n Please Contact with Support.");
                Application.Current.Shutdown();
            }
            string instance_id = (string)jsonObject["instance"]["id"];
            _configModel._LicenseKey = licenseKey;
            _configModel._InstanceID = instance_id;

            SupaDataModel data = new SupaDataModel();
            data._Date = DateTime.Now.ToString("MM/dd/yyyy");
            data._ProductID = HardCode.LOGIN_PRODUCT_ID;
            data._ProductName = HardCode.PRODUCT_NAME;
            data._DeviceID = deviceId;
            data._LicenseKey = _configModel._LicenseKey;
            await _supabaseManager.InsertSupaDataInTableAsync(data);

            _configModel.Save();
            _form.Hide();
            _Success = true;
            _form.Close();
        }
        private void ChromelessWindow_Closed(object sender, EventArgs e)
        {
            if(!_Success) Application.Current.Shutdown();
        }

        private async void btnDeActivate_Click(object sender, RoutedEventArgs e)
        {
            string licenseKey = this.tbxLicenseKey.Text;
            if (licenseKey.Trim() == "")
            {
                UtilMethods.ShowMessageBox("Please type the license key.");
                return;
            }

            string instanceId = this.tbxInstanceID.Text;
            if (licenseKey.Trim() == "")
            {
                UtilMethods.ShowMessageBox("Please type the instance id.");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                var postData = new Dictionary<string, string>
            {
                { "license_key", licenseKey },
                { "instance_id", instanceId }
            };

                var requestContent = new FormUrlEncodedContent(postData);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await client.PostAsync(HardCode.DEACTIVATE_URL, requestContent);

                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    JObject jsonObject = JObject.Parse(responseContent);

                    string license = (string)jsonObject["license_key"]["key"];
                    if (license != licenseKey)
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
                if(_configModel._LicenseKey == licenseKey)
                {
                    _configModel.RemoveConfig();

                }
            }
        }

        

        private async Task SendGetRequestForValidWithLicenseKeyTest(string licenseKey)
        {
            _configModel._LicenseKey = licenseKey;

            string url = "https://api.whop.com/api/v2/memberships/" + _configModel._LicenseKey;

            if (licenseKey == "S")
            {
                _configModel.Save();
                // go main form
                _form.Hide();
                _Success = true;
                _form.Close();
            }
            else
            {
                UtilMethods.ShowMessageBox("No Such Membership found with the provided License Key.");
                Application.Current.Shutdown();
            }
        }

    }
}
