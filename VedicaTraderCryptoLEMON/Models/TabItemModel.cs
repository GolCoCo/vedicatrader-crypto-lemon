using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VedicaTraderCryptoLEMON.Models;

namespace VedicaTraderCryptoLEMON
{
    public class TabItemModel
    {
        public string Header { get; set; }
        public string Content { get; set; }
        public bool IsClose { get; set; }
        public string CloseState { get; set; }

        public ObservableCollection<ChartInfo> ChartModels { get; set; }

        public TabItemModel(string header, bool isClose = true)
        {
            Header = header;
            IsClose = isClose;
            CloseState = !isClose ? "Hidden" : "Visible";
            ChartModels = new ObservableCollection<ChartInfo>();
        }
    }
}
