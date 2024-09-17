using Postgrest.Attributes;
using Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON.Models
{
    [Table("lemon_license_key_history")]
    public class SupaDataModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("_Date")]
        public string _Date { get; set; }

        [Column("_ProductID")]
        public string _ProductID { get; set; }

        [Column("_ProductName")]
        public string _ProductName { get; set; }

        [Column("_LicenseKey")]
        public string _LicenseKey { get; set; }

        [Column("_DeviceID")]
        public string _DeviceID { get; set; }

        public bool ModelCompare(SupaDataModel model)
        {
            bool ret = false;
            if (id == model.id && _ProductID == model._ProductID)
            {
                ret = true;
            }
            return ret;
        }
    }

    [Table("deviceid_banned")]
    public class SupaDeviceIDModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("_DeviceID")]
        public string _DeviceID { get; set; }

        [Column("_ProductName")]
        public string _ProductName { get; set; }

        [Column("_ProductID")]
        public string _ProductID { get; set; }

        [Column("_SellingPlatform")]
        public string _SellingPlatform { get; set; }

        [Column("_Note")]
        public string _Note { get; set; }

        public bool ModelCompare(SupaDeviceIDModel model)
        {
            bool ret = false;
            if (id == model.id && _DeviceID == model._DeviceID)
            {
                ret = true;
            }
            return ret;
        }
    }
    public class ConfigModel : SettingsBase
    {
        public string _LicenseKey { get; set; }
        public string _InstanceID { get; set; }

        public void RemoveConfig()
        {
            _LicenseKey = null;
            _InstanceID = null;
            this.Save();
        }
        public ConfigModel()
        {
            _InstanceID = null;
        }
    }
    public class RDataRecord
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Close { get; set; }
        public string Volume { get; set; }
        public string Stcb { get; set; }
        public string Stcs { get; set; }
        public string Trend { get; set; }
        public string UpperBand { get; set; }
        public string LowerBand { get; set; }
        public string MidLine { get; set; }
        public string Lavg1 { get; set; }
        public string Lavg2 { get; set; }
        public string Lavg3 { get; set; }
        public string Lavg4 { get; set; }
        public string LValues { get; set; }
        public string TValues { get; set; }
    }
    public class ChartInfo
    {
        public ChartInfo(string symbol, string name)
        {
            _symbol = symbol;
            _data = new ObservableCollection<RDataRecord>();
            _name = name;
        }

        public ChartInfo()
        {
        }
        public ObservableCollection<RDataRecord> _data;
        public string _symbol { get; set; }
        public string _name { get; set; }
    }
    public class WorkspaceConfig
    {
        public WorkspaceConfig(bool isdefault)
        {
            if (isdefault)
            {
                _isdefault = true;
                _name = "DEFAULT";
                _symbols = new List<ChartInfo>();
                foreach(string  dline in HardCode.DEFAULT_SYMBOLS)
                {
                    string[] datas = dline.Split('=');
                    _symbols.Add(new ChartInfo(datas[1], datas[0]));
                }
                _datetime = DateTime.Now;
            }
            else
            {
                _isdefault = false;
                _name = "";
                _symbols = new List<ChartInfo>();
                _datetime = DateTime.Now;
            }
        }

        public string _WPID { get; set; }

        public bool _isdefault { get; set; }

        public string _name { get; set; }
        public List<ChartInfo> _symbols { get; set; }
        public DateTime _datetime { get; set; }

        public void Save()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(_name);
            sb.AppendLine(_datetime.ToString());
            foreach (ChartInfo ch in _symbols)
            {
                StringBuilder tsb = new StringBuilder();
                tsb.Append(String.Format("{0}", ch._symbol));
                tsb.Append(String.Format(",{0},{1}", 0, 0));
                tsb.Append(String.Format(",{0},{1}", 0, 0));
                sb.AppendLine(tsb.ToString());
            }
            File.WriteAllText(UtilMethods.GetSaveFilePath(this._name), UtilMethods.Encrypt(sb.ToString(), HardCode.CRYPTO_KEY));
        }
    }
    public class SettingsBase
    {
        public void Save()
        {

            using (StreamWriter writer = new StreamWriter(HardCode.STR_SAVE_FOLDER_PATH + "\\" + this.GetType().Name + ".xml"))
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                xmlSerializer.Serialize(writer, this);
            }
        }

        public static T Load<T>() where T : SettingsBase
        {
            T result = null;
            if (File.Exists(HardCode.STR_SAVE_FOLDER_PATH + "\\" + typeof(T).Name + ".xml"))
            {
                using (StreamReader reader = new StreamReader(HardCode.STR_SAVE_FOLDER_PATH + "\\" + typeof(T).Name + ".xml"))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    result = (T)xmlSerializer.Deserialize(reader);
                }
            }
            return result;
        }
    }

    public class LoginInfo : SettingsBase
    {
        public string _username { get; set; }
        public string _password { get; set; }
        public bool _rememeber { get; set; }

        public LoginInfo()
        {
            _username = "";
            _password = "";
            _rememeber = false;
        }

        public bool IsEmpty()
        {
            if (_username == "" || _password == "")
            {
                return true;
            }
            return false;
        }
    }

    public class ManagerSoundPlayer
    {
        public SoundPlayer _soundplayer;
        public ManagerSoundPlayer()
        {
            this._soundplayer = new SoundPlayer();
        }
        public void Play()
        {
            this._soundplayer.Play();
        }
    }

}
