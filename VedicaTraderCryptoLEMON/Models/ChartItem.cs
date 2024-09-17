using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VedicaTraderCryptoLEMON.Models
{
    public class ChartItem
    {
        public string DataSignal { get; set; }
        public string DataSignal2 { get; set; }
        public string ClosePrice { get; set; }
        public string Gain { get; set; }
        public string Volume { get; set; }
        public string Trend { get; set; }
        public string Uppper { get; set; }
        public string Lower { get; set; }
        public string Midline { get; set; }

        public string Lavg1 { get; set; }
        public string Lavg2 { get; set; }

        public string Lavg3 { get; set; }
        public string Lavg4 { get; set; }

        public string StringDateTime { get; set; }
        public List<string> DataList { get; set; }
        public List<string> LDataList { get; set; }
        public List<string> TDataList { get; set; }

        public List<string> MappingCalls { get; set; }

        public List<string> MappingPuts { get; set; }

        public List<string> MappingCallNames { get; set; }

        public List<string> MappingPutNames { get; set; }

        public ChartItem()
        {
            TDataList = new List<string>();
            LDataList = new List<string>();
            DataList = new List<string>();
        }
    }
}
