using Syncfusion.Licensing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VedicaTraderCryptoLEMON.Models;
using Standart.Hash.xxHash;
using DeviceId;
using ICSharpCode.SharpZipLib.Zip;
using VedicaTraderCryptoLEMON.SubViews;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace VedicaTraderCryptoLEMON.Utils
{
    public static class DataTableExtensions
    {
        public static void SetColumnsOrder(this DataTable table, params String[] columnNames)
        {
            int columnIndex = 0;
            foreach (var columnName in columnNames)
            {
                if (!table.Columns.Contains(columnName))
                    continue;
                table.Columns[columnName].SetOrdinal(columnIndex);
                columnIndex++;
            }
        }
    }
    public static class LicenseKeyLocator
    {
        public static void FindandRegisterLicenseKey()
        {
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXpfcHRXRGhcWUBzWUM=");
        }
    }

    public static class UtilMethods
    {
        public static string RES_SUCCESS = "success";
        public static string RES_EXPIRED = "expired_date";
        public static string RES_NO_SUBS = "no_subscriptions";
        public static string RES_FAIL_CON = "failed for connection";
        public static string RES_WRONG_CRED = "wrong_credential";

        public static string URL_HELP_TOPIC = "https://www.xyz.com/VedicaTraderStocks/help/";
        public static string URL_SUPPORT = "https://www.xyz.com/VedicaTraderStocks/support/";

        public static string URL_SHRI_INVEST = "https://shriinvest.com/";
        public static string URL_YAHOO_FINANCE = "https://finance.yahoo.com/";
        public static string URL_FORGOT_PASSWORD = "https://shriinvest.com/srimember/login?sendpass";
        public static string URL_SIGNUP = "https://shriinvest.com/srimember/signup";

        public static string STR_CONNECTED = "CONNECTED";
        public static string STR_DISCONNECTED = "DISCONNECTED";
        public static string STR_DEFAULT = "DEFAULT";
        public static string STR_RECENT_NAME = "recent.ini";

        public static string[] LColumns = { "L1","L2", "L3", "L4", "L5", "L6", "L7", "L8", "L9", "L10", "L11", "L12", "L13", "L14", "L15", "L16", "L20", "L22" };

        // Messagebox
        public static string MSGB_ALERT = "ALERT!";
        public static string MSGB_WARNING = "WARNING!";
        public static string MSGB_LOGIN_EMPTY = "Please enter your username and password!";
        public static string MSGB_EXPIRED_DATE = "The membership to this product has expired, please renew on the website.";
        public static string MSGB_NO_SUBSCRIPTION = "You do not have a valid subscription for this server.\n Please visit www.proxmox.com to get list available options.";
        public static string MSGB_FAILED_CONNECTION = "The connection for login failed. \n Please confirm it again";
        public static string MSGB_WRONG_CRED = "WRONG CREDENTIALS \n Invaild username or password\nPlease confirm it again";
        public static string MSGB_SAVE = "Successful to save!";
        public static string MSGB_UPDATE = "Successful to update!";
        public static string MSGB_DELETE = "Successful to delete!";
        public static string MSGB_ERROR = "Erorr occured!";
        public static string MSGB_SYMBOL_NO_EXISTENCE = "The CSV file matched by the symbol is no existed.";
        public static string MSGB_WORK_EXISTENCE = "The workspace is already existed.";
        public static string MSGB_WORK_NO_EXISTENCE = "The workspace is no existed.";
        public static string MSGB_CHART_EXISTENCE = "The chart is already showed.";
        public static string MSGB_NEW_DEVICE_DETECTED = "New Device ID was Detected.\n Would you activate with this ID now?";

        public static Color CLR_RED = Color.DarkRed;
        public static Color CLR_GREEN = Color.DarkGreen;

        public static void AppendLogs(string filepath, string content)
        {
            try
            {
                File.AppendAllText(filepath, $"===>  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\n--------------\n{content}\n-----------------\n");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static Dictionary<string, string> GetDualChartDic(string[] symbols)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            List<string> list = symbols.ToList();
            foreach (string line in list)
            {
                List<string> vals = line.Split("=").ToList<string>();
                if (vals[2] == "TRUE")
                {
                    dic.Add(vals[0], vals[1]);
                }
            }

            return dic;
        }
        public static void ShowMessageBox(string content)
        {
            WinMessageBox form = new WinMessageBox(content);
            form.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            form.ShowDialog();
        }
        public static DateTime GetDateTimeFromString(string value)
        {
            DateTime d = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return d;
        }

        public static string GetSaveFilePath(string workname)
        {
            if (workname == STR_RECENT_NAME)
            {
                return Path.Combine(HardCode.STR_SAVE_FOLDER_PATH, String.Format("{0}", workname));
            }
            else
            {
                return Path.Combine(HardCode.STR_SAVE_FOLDER_PATH, String.Format("{0}.cfg", workname));
            }

        }

        public static string GetSaveFolderPath()
        {
            string documentpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\VedicaTraderCryptoLEMON";
            if (!Directory.Exists(@documentpath))
            {
                Directory.CreateDirectory(@documentpath);
            }
            return documentpath;
        }
        public static void WriteRecentData(List<string> recents)
        {
            if (recents.Count == 0) return;
            if (File.Exists(HardCode.STR_SAVE_FOLDER_PATH + "\\" + STR_RECENT_NAME))
            {
                File.Delete(HardCode.STR_SAVE_FOLDER_PATH + "\\" + STR_RECENT_NAME);
            }
            StringBuilder sb = new StringBuilder();

            int index = 0;

            for (int i = recents.Count - 1; i >= 0; i--)
            {
                if (index > 5)
                {
                    break;
                }
                else
                {
                    sb.AppendLine(recents[i]);
                }
                index++;
            }
            File.WriteAllText(@GetSaveFilePath(STR_RECENT_NAME), sb.ToString());
        }

        public static List<string> GetRecentsData()
        {
            if (File.Exists(@GetSaveFilePath(STR_RECENT_NAME)))
            {
                string[] recents = File.ReadAllLines(@GetSaveFilePath(STR_RECENT_NAME));
                return new List<string>(recents);
            }
            return null;
        }


        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static Dictionary<string, List<List<string>>> GetAllDatasFromStream(Stream fs)
        {
            Dictionary<string, List<List<string>>> datas = new Dictionary<string, List<List<string>>>();

            ICSharpCode.SharpZipLib.Zip.ZipFile zipArchive = new ICSharpCode.SharpZipLib.Zip.ZipFile(fs);
            foreach (ZipEntry elementInsideZip in zipArchive)
            {
                String ZipArchiveName = elementInsideZip.Name;
                Stream zipStream = zipArchive.GetInputStream(elementInsideZip);
                List<List<string>> csvdatas = GetValuesFromCSV(new StreamReader(zipStream));
                datas.Add(ZipArchiveName, csvdatas);
            }

            return datas;
        }
        public static List<string> GetEndPointAndBusketAndFileName(String url)
        {
            List<string> retlist = new List<string>();
            Uri myUri = new Uri(url);
            string[] seg = myUri.Segments;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < seg.Length - 1; i++)
            {
                sb.Append(seg[i]);
            }
            string endpoint = myUri.Authority;
            string tmpbusket = sb.ToString();
            string busket = tmpbusket.Substring(1, tmpbusket.Length - 2);
            string filename = seg.Last<string>();
            retlist.Add(endpoint);
            retlist.Add(busket);
            retlist.Add(filename);
            return retlist;
        }

        public static string GetDeviceID()
        {
            DeviceIdBuilder deviceId = new DeviceIdBuilder();
            return deviceId.AddMachineName().AddMacAddress().ToString();
        }

        public static string GenerateHashValueForDeviceID(string data)
        {
            byte[] data1 = Encoding.UTF8.GetBytes(data);

            ulong h64 = xxHash64.ComputeHash(data1, data1.Length);

            return longToHex(h64);
        }
        private static string longToHex(ulong l)
        {

            return String.Format("{0:X}", l);

        }

        public static List<List<String>> GetValuesFromCSV(StreamReader reader)
        {
            string line;
            List<List<String>> datasByRow = new List<List<String>>();
            while ((line = reader.ReadLine()) != null)
            {
                List<string> row = new List<string>();
                string[] X = line.Split(',');
                for (int i = 0; i < X.Length; i++)
                {
                    row.Add(X[i]);
                }
                datasByRow.Add(new List<string>(row));
                row.Clear();
            }
            return datasByRow;
        }

        public static WorkspaceConfig LoadWorkspaceInfo(string workanme)
        {
            try
            {
                WorkspaceConfig config = new Models.WorkspaceConfig(false);
                String cyber = File.ReadAllText(GetSaveFilePath(workanme));
                string[] lines = Decrypt(cyber, HardCode.CRYPTO_KEY).Split('\n');
                int index = 0;
                foreach (string line in lines)
                {
                    if (line == "") continue;
                    if (index == 0)
                    {
                        config._name = line;

                    }
                    else if (index == 1)
                    {
                        config._datetime = DateTime.Parse(line);
                    }
                    else
                    {
                        Models.ChartInfo ch = new ChartInfo();
                        string[] infos = line.Split(',');
                        ch._symbol = infos[0];
                        config._symbols.Add(ch);
                    }

                    index++;
                }
                return config;
            }
            catch (Exception)
            {
                MessageBox.Show(MSGB_WORK_NO_EXISTENCE);
            }

            return null;
        }

        public static string Decrypt(string stringToDecrypt, string key)
        {
            string result = null;

            if (string.IsNullOrEmpty(stringToDecrypt))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cannot decrypt using an empty key. Please supply a decryption key.");
            }

            try
            {
                System.Security.Cryptography.CspParameters cspp = new System.Security.Cryptography.CspParameters();
                cspp.KeyContainerName = key;

                System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(cspp);
                rsa.PersistKeyInCsp = true;

                string[] decryptArray = stringToDecrypt.Split(new string[] { "-" }, StringSplitOptions.None);
                byte[] decryptByteArray = Array.ConvertAll<string, byte>(decryptArray, (s => Convert.ToByte(byte.Parse(s, System.Globalization.NumberStyles.HexNumber))));

                byte[] bytes = rsa.Decrypt(decryptByteArray, true);

                result = System.Text.UTF8Encoding.UTF8.GetString(bytes);
            }
            finally
            {
                // no need for further processing
            }

            return result;
        }

        public static string Encrypt(string stringToEncrypt, string key)
        {
            if (string.IsNullOrEmpty(stringToEncrypt))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cannot encrypt using an empty key. Please supply an encryption key.");
            }

            System.Security.Cryptography.CspParameters cspp = new System.Security.Cryptography.CspParameters();
            cspp.KeyContainerName = key;

            System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            byte[] bytes = rsa.Encrypt(UTF8Encoding.UTF8.GetBytes(stringToEncrypt), true);

            return BitConverter.ToString(bytes);
        }

        public static SymbolModel GetSymbolModel(string filepath, List<List<string>> datalines)
        {
            SymbolModel sm = new SymbolModel();
            sm._FilePath = filepath;
            int totalRowCount = datalines.Count;
            if (totalRowCount <= 1)
            {
                return null;
            }
            double buyValue = 0.0;
            double sellValue = 0.0;
            string date = "";
            string time = "";
            double close = 0.0;
            double lclose = 0.0;
            string ldatetime = "";
            for (int idx = totalRowCount - 1; idx > 0; idx--)
            {

                var datas = datalines[idx];

                double.TryParse(datas[7], out buyValue);
                double.TryParse(datas[8], out sellValue);
                date = datas[0];
                time = datas[1];
                if (idx == totalRowCount - 1)
                {
                    ldatetime = $"{date} {time}";
                    double.TryParse(datas[5], out lclose);
                }
                double.TryParse(datas[5], out close);

                if (buyValue > 0 || sellValue > 0)
                {
                    break;
                }
            }
            string stype = "BUY";

            if (buyValue > 0)
            {
                stype = "BUY";
            }
            if (sellValue > 0)
            {
                stype = "SELL";
            }


            int OnlyStocks = 0;
            int SortByFastM = 0;
            DateTime curDT = DateTime.Now;
            DateTime curD = DateTime.Now.Date;


            if (date == "") return null;


            sm._Symbol = Path.GetFileNameWithoutExtension(filepath).Split('-')[0];
            sm._BuyValue = buyValue;
            sm._SellValue = sellValue;
            sm._DateTime = $"{date} {time}";
            sm._BarRowsFor1hour = OnlyStocks;
            sm._BarRowsToday = SortByFastM;
            sm._HeadLastRowsTimeStamp = DateTime.Parse(ldatetime).ToString("MM/dd HH:mm");
            sm._HeadLastSDateTime = DateTime.Parse($"{date} {time}").ToString("MM/dd HH:mm");
            sm._HeadLastSPrice = $"{close}";
            sm._HeadLastSType = $"{stype}";
            sm._HeadLastRowClose = lclose;
            sm._DataLines = datalines;

            return sm;
        }
    }

}
