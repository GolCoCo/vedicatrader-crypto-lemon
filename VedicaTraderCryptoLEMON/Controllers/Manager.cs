using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Newtonsoft.Json.Linq;
using Syncfusion.SfSkinManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VedicaTraderCryptoLEMON.Views;
using VedicaTraderCryptoLEMON.Models;
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON.Controllers
{
    public class SupabaseManager
    {
        public Supabase.Client _supaClient = null;
        public SupabaseManager()
        {
            Supabase.SupabaseOptions options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
            _supaClient = new Supabase.Client(HardCode.SUPA_URL, HardCode.SUPA_KEY, options);
        }
        public async void IntializeClient()
        {
            try
            {
                if (!HardCode.IsTest)
                {
                    await _supaClient.InitializeAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // gum license key history table
        public async Task<List<SupaDataModel>> ReadAllSupaDataFromTableAsync()
        {
            var result = await _supaClient.From<SupaDataModel>().Get();
            List<SupaDataModel> users = result.Models;
            return users;
        }

        public async Task<string> InsertSupaDataInTableAsync(SupaDataModel model)
        {
            var result = await _supaClient.From<SupaDataModel>().Insert(model);
            string res = result.ResponseMessage.ToString();
            return res;
        }

        public async Task RemoveSupaDataFromTableAsync(SupaDataModel model)
        {
            await _supaClient.From<SupaDataModel>().Where(x => x.id == model.id).Delete();
        }

        // deviceid_banned
        public async Task<List<SupaDeviceIDModel>> ReadAllDeviceIDBannedFromTableAsync()
        {
            var result = await _supaClient.From<SupaDeviceIDModel>().Get();
            List<SupaDeviceIDModel> users = result.Models;
            return users;
        }
        public async Task<string> InsertDeviceIDBannedInTableAsync(SupaDeviceIDModel model)
        {
            var result = await _supaClient.From<SupaDeviceIDModel>().Insert(model);
            string res = result.ResponseMessage.ToString();
            return res;
        }

        public async Task RemoveDeviceIDBannedFromTableAsyn(SupaDeviceIDModel model)
        {
            await _supaClient.From<SupaDeviceIDModel>().Where(x => x.id == model.id).Delete();
        }
    }
    public class Manager
    {
        public class ActivationCodeCheckAndRunMain
        {
            private ConfigModel _configModel;
            private SupabaseManager _supabaseManager;
            public ActivationCodeCheckAndRunMain()
            {
                HardCode.STR_SAVE_FOLDER_PATH = UtilMethods.GetSaveFolderPath();
                _configModel = ConfigModel.Load<ConfigModel>();

                if (_configModel == null)
                {
                    _configModel = new ConfigModel();
                }
                _configModel.Save();

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

            public async Task CheckIfIsValidAndRunMainFormAsync()
            {
                try
                {
                    if (HardCode.IsTest)
                    {
                        if (_configModel._LicenseKey == null || _configModel._LicenseKey == "")
                        {

                            // show activate form
                            WinActivation form = new WinActivation(_configModel);
                            SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
                            form.ShowDialog();
                        }
                        else
                        {
                            await SendGetRequestForValidTest();
                        }
                    }
                    else
                    {
                        if (_configModel._LicenseKey != null && _configModel._InstanceID != null)
                        {
                            await SendPostValidationRequestAsync();
                        }
                        else
                        {
                            // show activate form
                            WinActivation form = new WinActivation(_configModel);
                            SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
                            form.ShowDialog();
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    Application.Current.Shutdown();
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

            private async Task SendPostValidationRequestAsync()
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

                    var response = await client.PostAsync(HardCode.VALIDATTION_URL, requestContent);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                    }
                    else
                    {
                        UtilMethods.ShowMessageBox("License Key is Invalid.");
                        _configModel.RemoveConfig();
                        Application.Current.Shutdown();
                    }


                }
            }

            private async Task CheckIfActivatedWithResponseAsync(string responseBody)
            {
                JObject jsonObject = JObject.Parse(responseBody);

                bool valid = (bool)jsonObject["valid"];
                if (!valid)
                {
                    UtilMethods.ShowMessageBox("Invalid License Key. Please check the license key again.");
                    Application.Current.Shutdown();
                }


                string license = (string)jsonObject["license_key"]["key"];
                if (license != _configModel._LicenseKey)
                {
                    UtilMethods.ShowMessageBox("License Key was not matched.");
                    Application.Current.Shutdown();
                }

                string status = (string)jsonObject["license_key"]["status"];
                if (status == "inactive")
                {
                    UtilMethods.ShowMessageBox("License Key Is Inactive.\nPlease Renew the Membership or contact support.");
                    Application.Current.Shutdown();
                }

                if (status == "expired")
                {
                    UtilMethods.ShowMessageBox("License Key Is Expired, Please Renew the Membership.");
                    Application.Current.Shutdown();
                }
                if (status == "disabled")
                {
                    UtilMethods.ShowMessageBox("License Key Is Disabled.\nPlease Contact Support for more Information.");
                    Application.Current.Shutdown();
                }
                string instanceid = (string)jsonObject["instance"]["id"];
                if (instanceid != _configModel._InstanceID)
                {
                    UtilMethods.ShowMessageBox("Instance Key is Invalid, Please contact support.");
                    Application.Current.Shutdown();
                }

                string instance_name = (string)jsonObject["instance"]["name"];
                string device_id = UtilMethods.GetDeviceID();
                if (instance_name != device_id)
                {
                    UtilMethods.ShowMessageBox("License Key Does Not Match the DeviceID.\n Please Unregister Existing Device To Register New Device.");
                    Application.Current.Shutdown();
                }
            }



            /// <summary>
            /// //
            /// </summary>
            /// <returns></returns>

            public async Task SendGetRequestForValidTest()
            {
                string url = "https://api.whop.com/api/v2/memberships/" + _configModel._LicenseKey;

                if (true)
                {
                    _configModel.Save();
                    WinActivation form = new WinActivation(_configModel);
                    SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
                    form.ShowDialog();
                }
                else
                {
                    UtilMethods.ShowMessageBox("No Such Membership found with the provided License Key.");
                    Application.Current.Shutdown();
                }
            }
        }
        // Manager wasabi for downloading symbol zip files
        public class SymbolWasabiAPI
        {

            private AmazonS3Client _client;

            public SymbolWasabiAPI()
            {
                try
                {
                    var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(HardCode.WAS_ACCESS_KEY, HardCode.WAS_SECRET_KEY);
                    var awsConfig = new AmazonS3Config { ServiceURL = HardCode.WAS_ENDPOINT_URL };

                    _client = new AmazonS3Client(awsCredentials, awsConfig);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            public async Task<Dictionary<string, List<List<string>>>> DownloadAndGetAllDatasFileNameByZIPAsync(string subfolder, string filename)
            {
                try
                {
                    string _filename = subfolder + filename;
                    if (_client == null) return null;

                    TransferUtility utility = new TransferUtility(_client);
                    TransferUtilityOpenStreamRequest streamrequest = new TransferUtilityOpenStreamRequest();
                    streamrequest.BucketName = HardCode.WAS_BUCKET_NAME;
                    streamrequest.Key = _filename;
                    Stream stream = await utility.OpenStreamAsync(streamrequest);
                    if (stream.CanRead)
                    {
                        MemoryStream memorySteam = new MemoryStream();
                        stream.CopyTo(memorySteam);
                        Dictionary<string, List<List<string>>> ret = UtilMethods.GetAllDatasFromStream(memorySteam);
                        stream.Close();
                        return ret;
                    }
                    else
                    {
                        stream.Close();
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public void Close()
            {
                _client.Dispose();
            }
        }

        // Manager wasabi for downloading and uploading the deviceid.txt 
        public class DeviceIDWasabiAPI
        {

            private AmazonS3Client _client = null;

            public DeviceIDWasabiAPI()
            {
                try
                {
                    var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(HardCode.WAS_DEVICEID_ACCESS_KEY, HardCode.WAS_DEVICEID_SECRET_KEY);
                    var awsConfig = new AmazonS3Config { ServiceURL = HardCode.WAS_ENDPOINT_URL };

                    _client = new AmazonS3Client(awsCredentials, awsConfig);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            public async void CreateFolder(string bucketName, string folderName)
            {
                var folderKey = folderName + "/";
                PutObjectRequest request = new PutObjectRequest();
                request.BucketName = bucketName;
                request.StorageClass = S3StorageClass.Standard;
                request.ServerSideEncryptionMethod = ServerSideEncryptionMethod.None;
                request.Key = folderKey;
                request.ContentBody = string.Empty;
                PutObjectResponse response = await _client.PutObjectAsync(request);
            }

            public async Task<bool> DoesExistObjectAsync(string folderName, string bucketName)
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = bucketName;
                request.Prefix = folderName;
                request.MaxKeys = 1;

                ListObjectsResponse response = await _client.ListObjectsAsync(request);
                return (response.S3Objects.Count > 0);
            }

            public async Task<bool> CheckIfNewUserAsync()
            {
                bool ret = true;
                try
                {
                    string folderPath = @String.Format("{0}{1}/", HardCode.WAS_DEVICEID_SUB_FOLDER, HardCode.USERNAME);
                    bool isExists = await DoesExistObjectAsync(folderPath, HardCode.WAS_DEVICEID_BUCKET_NAME);
                    if (isExists)
                    {
                        ret = false;
                        //string filePath = @String.Format("{0}{1}", folderPath, HardCode.WAS_DEVICEID_FILE_NAME);
                        //bool isFileExists = await DoesExistObjectAsync(filePath, HardCode.WAS_DEVICEID_BUCKET_NAME);
                        //if (isFileExists)
                        //{

                        //}
                    }
                    else
                    {
                        CreateFolder(HardCode.WAS_DEVICEID_BUCKET_NAME, folderPath);
                    }
                    return ret;

                }
                catch (Exception)
                {
                    return ret;
                }
            }
            public async Task<string> DownloadDeviceInfoAsync()
            {
                try
                {
                    string filePath = String.Format("{0}{1}/{2}", HardCode.WAS_DEVICEID_SUB_FOLDER, HardCode.USERNAME, HardCode.WAS_DEVICEID_FILE_NAME);

                    //bool isFileExists = await DoesExistObjectAsync(filePath, HardCode.WAS_DEVICEID_BUCKET_NAME);
                    //if (!isFileExists)
                    //{
                    //    return null;
                    //}

                    TransferUtility utility = new TransferUtility(_client);

                    TransferUtilityOpenStreamRequest streamrequest = new TransferUtilityOpenStreamRequest();
                    streamrequest.BucketName = HardCode.WAS_DEVICEID_BUCKET_NAME;
                    streamrequest.Key = filePath;
                    Stream stream = await utility.OpenStreamAsync(streamrequest);

                    if (stream.CanRead)
                    {
                        StreamReader reader = new StreamReader(stream);
                        string deviceid = reader.ReadToEnd();
                        stream.Close();
                        return deviceid;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }

            }

            public async Task DeleteObjectAsync(string filePath, string bucketName)
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = filePath,
                };

                PutObjectResponse response = await _client.PutObjectAsync(request);

                // Delete the object by specifying an object key and a version ID.
                DeleteObjectRequest request1 = new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = filePath,
                    VersionId = response.VersionId,
                };
                await _client.DeleteObjectAsync(request1);
            }

            public async void UploadDeviceInfoAsync(string DeviceID)
            {
                try
                {
                    if (_client == null || HardCode.USERNAME == "") return;
                    string filePath = @String.Format("{0}{1}/{2}", HardCode.WAS_DEVICEID_SUB_FOLDER, HardCode.USERNAME, HardCode.WAS_DEVICEID_FILE_NAME);

                    bool isFileExists = await DoesExistObjectAsync(filePath, HardCode.WAS_DEVICEID_BUCKET_NAME);
                    if (isFileExists)
                    {
                        await DeleteObjectAsync(filePath, HardCode.WAS_DEVICEID_BUCKET_NAME);
                    }

                    TransferUtility utility = new TransferUtility(_client);

                    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
                    request.BucketName = HardCode.WAS_DEVICEID_BUCKET_NAME;
                    request.Key = filePath;
                    request.InputStream = UtilMethods.GenerateStreamFromString(DeviceID);
                    await utility.UploadAsync(request);
                }
                catch (Exception)
                {
                }

            }

            public void Close()
            {
                _client.Dispose();
            }
        }
    }
}
