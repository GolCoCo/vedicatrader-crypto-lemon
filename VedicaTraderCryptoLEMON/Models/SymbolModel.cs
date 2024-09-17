using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VedicaTraderCryptoLEMON.Models
{
    public class SymbolModel : IComparable<SymbolModel>
    {
        public string _FilePath { set; get; }
        public string _Symbol { set; get; }
        public string _DateTime { set; get; }
        public double _BuyValue { set; get; }
        public double _SellValue { set; get; }
        public int _BarRowsFor1hour { set; get; }
        public int _BarRowsToday { set; get; }
        public int _BarRowsTodayTime { set; get; }
        public double _LRPrice { set; get; }
        public string _LastSDateTime { set; get; }
        public string _HeadLastRowsTimeStamp { set; get; }
        public double _HeadLastRowClose { set; get; }
        public string _HeadLastSDateTime { set; get; }
        public string _HeadLastSPrice { set; get; }
        public string _HeadLastSType { set; get; }
        public int _FirstRowTodayIndex { set; get; }
        public string _LasSignalDateTime2 { get; set; }
        public string _LastSignalType2 { set; get; }
        public int _HighLightIndex { set; get; }
        public bool _IsMarkReviewed { set; get; }
        public List<List<string>> _DataLines { set; get; }
        public int _MisMatch { set; get; }
        public double _HighestLoss { set; get; }
        public double _HighestGain { set; get; }
        public string _ChartFilePath { set; get; }
        public List<string> _DateList { set; get; }

        public SymbolModel()
        {
            _FilePath = "";
            _ChartFilePath = "";
            _DateTime = "";
            _BuyValue = 0.0;
            _SellValue = 0.0;
            _BarRowsFor1hour = -1;
            _BarRowsToday = -1;
            _BarRowsTodayTime = -1;
            _LastSDateTime = "";
            _LRPrice = 0.0;
            _HeadLastRowClose = 0.0;
            _HeadLastRowsTimeStamp = "";
            _HeadLastSDateTime = "";
            _HeadLastSPrice = "";
            _HeadLastSType = "";
            _FirstRowTodayIndex = -1;
            _LasSignalDateTime2 = "";
            _LastSignalType2 = "";
            _HighLightIndex = -1;
            _IsMarkReviewed = false;
            _DataLines = new List<List<string>>();

            _MisMatch = 0;
            _HighestLoss = -10000.0;
            _HighestGain = -10000.0;

            _DateList = new List<string>();
        }

        public string GetHeaderInfor()
        {
            return $"{_HeadLastRowsTimeStamp} | {_HeadLastSDateTime} {_HeadLastSPrice} {_HeadLastSType}";
        }
        public int CompareTo(SymbolModel other)
        {
            int ret = 0;
            DateTime dt1 = DateTime.Parse(_DateTime);
            DateTime dt2 = DateTime.Parse(other._DateTime);
            if (_BarRowsToday > other._BarRowsToday)
            {
                ret = -1;

            }
            else if (_BarRowsToday < other._BarRowsToday)
            {
                ret = 1;
            }
            else
            {
                ret = dt2.CompareTo(dt1);
            }
            return ret;
        }

        public int GetShortByFastMovingStock()
        {

            return 0;
        }

        public bool IsValildByFilter(int SignalType, bool IsOnlySignalToday, bool IsSignalAfterToday, string SignalValueAfterToday,
            bool IsOnlyStockRows, int OnlyStockRows)
        {
            bool ret = true;

            if (SignalType == 0)
            {
                //if (_SellValue <= 0.0 && _BuyValue <= 0.0) return false;

            }
            else if (SignalType == 1)
            {
                if (_BuyValue <= 0.0) return false;
            }
            else
            {
                if (_SellValue <= 0.0) return false;
            }
            DateTime dt1 = DateTime.Parse(_DateTime);

            if (IsOnlySignalToday)
            {
                if (dt1.Date.CompareTo(DateTime.Now.Date) == 0 || dt1.Date.CompareTo(DateTime.Now.AddDays(-1).Date) == 0)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            if (IsSignalAfterToday)
            {
                string stime = SignalValueAfterToday;
                string tdate = DateTime.Now.Date.ToString("yyyy/MM/dd");
                DateTime sDateTime = DateTime.Parse($"{tdate} {stime}");
                if (dt1.CompareTo(sDateTime) > 0)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            if (IsOnlyStockRows)
            {
                if (_BarRowsFor1hour >= OnlyStockRows)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            return ret;

        }

        public bool IsValildByFilterOptionGrid(int SignalType, bool IsOnlySignalToday, bool IsSignalAfterToday, string SignalValueAfterToday)
        {
            bool ret = true;

            if (SignalType == 0)
            {
                //if (_SellValue <= 0.0 && _BuyValue <= 0.0) return false;

            }
            else if (SignalType == 1)
            {
                if (_BuyValue <= 0.0) return false;
            }
            else
            {
                if (_SellValue <= 0.0) return false;
            }
            DateTime dt1 = DateTime.Parse(_DateTime);

            if (IsOnlySignalToday)
            {
                if (dt1.Date.CompareTo(DateTime.Now.Date) == 0 || dt1.Date.CompareTo(DateTime.Now.AddDays(-1).Date) == 0)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            if (IsSignalAfterToday)
            {
                string stime = SignalValueAfterToday;
                string tdate = DateTime.Now.Date.ToString("yyyy/MM/dd");
                DateTime sDateTime = DateTime.Parse($"{tdate} {stime}");
                if (dt1.CompareTo(sDateTime) > 0)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            return ret;

        }

        public bool IsValildByFilterOptionView(int SignalType, bool IsOnlySignalToday)
        {
            bool ret = true;

            if (SignalType == 0)
            {
                //if (_SellValue <= 0.0 && _BuyValue <= 0.0) return false;

            }
            else if (SignalType == 1)
            {
                if (_BuyValue <= 0.0) return false;
            }
            else
            {
                if (_SellValue <= 0.0) return false;
            }
            DateTime dt1 = DateTime.Parse(_DateTime);

            if (IsOnlySignalToday)
            {
                if (dt1.Date.CompareTo(DateTime.Now.Date) == 0 || dt1.Date.CompareTo(DateTime.Now.AddDays(-1).Date) == 0)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            return ret;

        }

        public void SetFirstRowTodayIndex()
        {
            int totalRowCount = _DataLines.Count;
            DateTime cdate = DateTime.Now.Date;
            if (totalRowCount > 1)
            {
                int cnt = 0;
                for (int idx = totalRowCount - 1; idx > 1; idx--)
                {
                    List<string> datas = _DataLines[idx];
                    DateTime tdt = DateTime.Parse($"{datas[0]} {datas[1]}");
                    if (tdt.CompareTo(cdate) >= 0)
                    {
                        cnt++;
                        _FirstRowTodayIndex = cnt;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
