using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VedicaTraderCryptoLEMON.Utils
{
    public class HardCode
    {
        public static bool IsTest = false;

        public static bool ENABLE_LOG = false;
        public static int LayoutState = 0;
        public static string CRYPTO_KEY = "l123l12l";
        public static string USERNAME = "umesh";
        public static string PNL_SPLIT_COLOR = "#00FFFF";

        // supabse infor
        public static string SUPA_URL = "https://lxknswuypitpvkqmymky.supabase.co";
        public static string SUPA_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imx4a25zd3V5cGl0cHZrcW15bWt5Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MjQ5ODUyNTIsImV4cCI6MjA0MDU2MTI1Mn0.TQBY3FWt9L-6v4fYER0IoN3SOWC-BmNXLcAnD1dljVA";
        public static string SUPA_TABLE = "wop_license_key";

        // Infors
        public static string PRODUCT_NAME = "crypto lemon";
        public static string LOGIN_URL = "https://api.lemonsqueezy.com/v1/licenses/activate";
        public static string DEACTIVATE_URL = "https://api.lemonsqueezy.com/v1/licenses/deactivate";
        public static string VALIDATTION_URL = "https://api.lemonsqueezy.com/v1/licenses/validate";
        public static string LOGIN_PRODUCT_ID = "342973";

        public static bool RT_UPDATE_YES = false;
        public static bool IS_EOD = false;
        public static string STR_SAVE_FOLDER_PATH = "C:\\Users";

        // Wasabi Info for symbol zip
        public static string WAS_ACCESS_KEY = "DO006FGTCRRWM269LRJM";
        public static string WAS_SECRET_KEY = "rNp/jPAHcoh+YNEXl7lpjRAA0VvCkmQtS4HRE86VNpE";
        public static string WAS_ENDPOINT_URL1 = "https://s3.us-east-1.wasabisys.com";
        public static string WAS_ENDPOINT_URL = "https://s3.wasabisys.com";
        public static string WAS_BUCKET_NAME = "wasabitestnew";
        public static string WAS_SUB_FOLDER = "Primary/";
        public static string WAS_SUB_FOLDER_ATST = "ATP/";
        public static string WAS_SUB_FOLDER_OHCL = "";
        public static string WAS_SUB_FOLDER_GARUDA = "";

        public static string WAS_FILE_NAME = "Master_Folder_SRI.zip";
        public static string WAS_FILE_NAME_ATST = "Master_Folder_ATST.zip";
        public static string WAS_FILE_NAME_OHLC = "Master_OHLC_Data.zip";
        public static string WAS_FILE_NAME_GARUDA = "GARUDA.zip";


        // wasabi Info for deviceId
        public static string WAS_DEVICEID_ACCESS_KEY = "DO006FGTCRRWM269LRJM";
        public static string WAS_DEVICEID_SECRET_KEY = "rNp/jPAHcoh+YNEXl7lpjRAA0VvCkmQtS4HRE86VNpE";
        public static string WAS_DEVICEID_BUCKET_NAME = "us-demo-users-test";
        public static string WAS_DEVICEID_FILE_NAME = "device_id.txt";
        public static string WAS_DEVICEID_SUB_FOLDER = "users/";

        // Firebase
        public static string FB_URL = "https://futureslive-beta-default-rtdb.firebaseio.com/";
        public static string FB_SECRET = "p9GwHGnL0VpVjfdZ6MWWOj3CKSp5VhPdGtoR4G7l";

        // Default Symbol when load And If DataFormat ATST is T1 to TX and L1 to LX and SRI is L1 to LX
        public static string[] DEFAULT_SYMBOLS = { "EURUSD=C:EURUSD-ATP", "BTCUSD=X:BTCUSD-ATP" };
        public static string DataFormat_Type = "SRI";

        // Symbols In Firebase
        public static string[] FB_SYMBOLS_CRYPTO =
        {
            "BTCUSD=X:BTCUSD-ATP=TRUE",
            "ETHUSD=X:ETHUSD-ATP=TRUE",
            "SOLUSD=X:SOLUSD-ATP=TRUE",
            "XRPUSD=X:XRPUSD-ATP=TRUE",
            "DOGEUSD=X:DOGEUSD-ATP=TRUE",
            "ADAUSD=X:ADAUSD-ATP=TRUE",
            "LTCUSD=X:LTCUSD-ATP=TRUE",
            "DIAUSD=X:DIAUSD-ATP=TRUE"
        };

        // DualChart Settings
        public static bool IsCalculateTickGL = true;
        public static int ROWTOLOAD = 500;
        public static int CHART_SET_PIXEL_SIZE = 16;
        public static bool IS_CHART_REF_LINE = false;
        public static int CHART_REF_LINE1 = 22;
        public static int CHART_REF_LINE2 = 55;
        public static int CHART_FONTSIZE = 11;

        public static bool CHART_ENABLE_ZOOM = false;

        public static int CHART_ROW_TO_LOAD = 1000;

        public static bool CHART_SHOW_LAVG1 = false;
        public static bool CHART_SHOW_LAVG2 = false;
        public static bool CHART_SHOW_LAVG3 = false;
        public static bool CHART_SHOW_LAVG4 = false;
        public static bool CHART_SHOW_TREND = false;
        public static bool CHART_SHOW_UPPER = false;
        public static bool CHART_SHOW_LOWER = false;
        public static bool CHART_SHOW_MIDLINE = false;

        public static string CHART_IGNORE_FILE_NAME = "-R";
        public static int CHART_LOAD_MORE_COUNT = 500;
        public static bool CHART_LOAD_ALL_L_COLUMNS = true;
        public static bool CHART_SHOW_CLOSE_DATE = true;
        public static bool CHART_SHOW_DATETIME_FOR_ONLY_SIGNAL = true;

        public static double _UpperBuy = 0;
        public static double _UpperSell = 0;
        public static string _UpperBuyColor = "#006400";
        public static string _UpperSellColor = "#8b0000";

        public static double _LowerBuy = 0;
        public static double _LowerSell = 0;
        public static string _LowerBuyColor = "#006400";
        public static string _LowerSellColor = "#8b0000";

        public static double _TrendBuy = 0;
        public static double _TrendSell = 0;
        public static string _TrendBuyColor = "#006400";
        public static string _TrendSellColor = "#8b0000";

        public static double _MidLineBuy = 0;
        public static double _MidLineSell = 0;
        public static string _MidLineBuyColor = "#006400";
        public static string _MidLineSellColor = "#8b0000";

        public static double _Lagv1Buy = 0;
        public static double _Lagv1Sell = 0;
        public static double _Lagv2Buy = 0;
        public static double _Lagv2Sell = 0;
        public static double _Lagv3Buy = 0;
        public static double _Lagv3Sell = 0;
        public static double _Lagv4Buy = 0;
        public static double _Lagv4Sell = 0;

        public static bool _IsShowValue = false;
    }
}
