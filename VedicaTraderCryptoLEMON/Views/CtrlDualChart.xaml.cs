using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Newtonsoft.Json.Linq;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VedicaTraderCryptoLEMON.Models;
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON
{
    /// <summary>
    /// Interaction logic for CtrlSriChart.xaml
    /// </summary>
    public partial class CtrlDualChart : UserControl
    {
        private FirebaseClient firebaseClient;
        private int _RowsCount = 0;
        private int _ColsCount = 0;
        private int date_index = 1;
        private int time_index = 1;
        private List<ChartItem> DataList = new List<ChartItem>();
        private SolidColorBrush CLR_DARKGREEN = new SolidColorBrush(Colors.DarkGreen);
        private SolidColorBrush CLR_DARKRED = new SolidColorBrush(Colors.DarkRed);
        private SolidColorBrush CLR_BLACK = new SolidColorBrush(Colors.Black);
        private SolidColorBrush CLR_WHITE = new SolidColorBrush(Colors.White);
        private SolidColorBrush CLR_YELLLOW = new SolidColorBrush(Colors.Yellow);
        private Pen LINEPEN_BLACK = new Pen(Brushes.Black, 0.75);
        private Pen LINEPEN_YELLOW = new Pen(Brushes.LightYellow, 1);
        private double FONTSIZE = HardCode.CHART_FONTSIZE;
        public string Symbol = "";
        private ObservableCollection<RDataRecord> _datas = null;
        private int ShownLavgColCount = 0;
        private bool Initial = true;

        public CtrlDualChart()
        {
            InitializeComponent();

            this.vGrid.Model.HeaderStyle.HorizontalAlignment = HorizontalAlignment.Center;
            this.vGrid.Model.HeaderStyle.VerticalAlignment = VerticalAlignment.Center;

            this.vGrid.Model.RowHeights.DefaultLineSize = HardCode.CHART_SET_PIXEL_SIZE;
            this.vGrid.Model.ColumnWidths.DefaultLineSize = HardCode.CHART_SET_PIXEL_SIZE;
            this.vGrid.Model.TableStyle.ReadOnly = true;

            this.vGrid.Model.TableStyle.Background = CLR_BLACK;
            this.vGrid.Model.TableStyle.Foreground = CLR_YELLLOW;
            this.vGrid.Model.TableStyle.Borders.Right = LINEPEN_BLACK;
            this.vGrid.Model.TableStyle.Borders.Bottom = LINEPEN_BLACK;

            this.vGrid.Model.HeaderStyle.HorizontalAlignment = HorizontalAlignment.Center;
            this.vGrid.Model.HeaderStyle.VerticalAlignment = VerticalAlignment.Center;

            this.vGrid.ColumnWidths.SetHidden(0, 0, true);
            this.vGrid.RowHeights.SetHidden(0, 0, true);
            this.vGrid.QueryCellInfo += new GridQueryCellInfoEventHandler(SriChartGrid_QueryCellInfo);
        }

        private void ConnectFirebase()
        {
            try
            {
                firebaseClient = new FirebaseClient(
                  HardCode.FB_URL,
                  new FirebaseOptions
                  {
                      AuthTokenAsyncFactory = () => Task.FromResult(HardCode.FB_SECRET)
                  });

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        private async Task<ObservableCollection<RDataRecord>> FetchDataAndPopulateListViewAsync(string tableName)
        {
            try
            {
                if (!HardCode.IsTest)
                {
                    ConnectFirebase();
                    ObservableCollection<RDataRecord> RDataSet = new ObservableCollection<RDataRecord>();
                    var RDataSnapshot = await firebaseClient.Child(tableName).OrderByKey().LimitToLast(HardCode.ROWTOLOAD).OnceAsJsonAsync();
                    if (String.IsNullOrEmpty(RDataSnapshot))
                    {
                        return null;
                    }
                    JObject json = JObject.Parse(RDataSnapshot);
                    if (json["data"] != null)
                    {
                        RDataSet = GetDataSetFromString(json["data"].ToString());
                        //firebaseClient.Child(tableName).AsObservable<string>().Subscribe(UpdateListViewRData);
                        return RDataSet;
                    }
                    return null;

                }
                else
                {
                    ObservableCollection<RDataRecord> RDataSet = new ObservableCollection<RDataRecord>();
                    if (HardCode.DataFormat_Type == "ATST")
                    {
                        string data = File.ReadAllText("E://DATA_ATST.txt");
                        RDataSet = GetDataSetFromString(data);
                    }
                    else
                    {
                        string data = File.ReadAllText("E://DATA_SRI.txt");
                        RDataSet = GetDataSetFromString(data);
                    }

                    return RDataSet;
                }
            }
            catch (Exception ex)
            {
                if (HardCode.ENABLE_LOG)
                {
                    UtilMethods.AppendLogs("vedicatrader-futures-logs.txt", $"{ex.ToString()}");
                }
                    
                UtilMethods.ShowMessageBox("There is no such symbol. Please check again.");
                return null;
            }
        }
        private System.Timers.Timer _stimer = null;
        public void RunTimerForPCDateTime()
        {
            if(!Initial)return;
            _stimer = new System.Timers.Timer();
            _stimer.Elapsed += new ElapsedEventHandler(OnPCTimeUpdate);
            _stimer.Interval = 4000;
            _stimer.Enabled = true;
            _stimer.Start();
        }

        private void OnPCTimeUpdate(object sender, ElapsedEventArgs e)
        {
            if (Initial)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsBusy = true;
                    ObservableCollection<RDataRecord> RDataSet = new ObservableCollection<RDataRecord>();
                    string data = File.ReadAllText("E://Data.txt");
                    RDataSet = GetDataSetFromString(data);
                    UpdateData(RDataSet);
                    Initial = false;
                });
            }
        }

        private void UpdateListViewRData(FirebaseEvent<string> firebaseEvent)
        {
            Dispatcher.BeginInvoke(() =>
            {
                busyIndicator.IsBusy = true;
                if (firebaseEvent.Object != null)
                {
                    if (firebaseEvent.EventType == FirebaseEventType.InsertOrUpdate)
                    {
                        ObservableCollection<RDataRecord> newRecords = GetDataSetFromString(firebaseEvent.Object);
                        if (newRecords.Count == 0) return;
                        if (HardCode.ENABLE_LOG)
                        {
                            UtilMethods.AppendLogs("vedicatrader-futures-logs.txt", $"{Symbol}  -> New Record's Count {newRecords.Count} In RealTime");
                        }
                        UpdateData(newRecords);
                    }
                }
            });
        }

        private ObservableCollection<RDataRecord> GetDataSetFromString(string dataStr)
        {
            ObservableCollection<RDataRecord> result = new ObservableCollection<RDataRecord>();
            string[] rows = dataStr.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            int startRow = 0;
            if (HardCode.ROWTOLOAD > 0)
                startRow = rows.Length - HardCode.ROWTOLOAD;
            if (startRow < 0)
                startRow = 0;

            for (int i = startRow; i < rows.Length; i++)
            {
                var cols = rows[i].Split(',');
                if (cols.Length < 14)
                    continue;

                RDataRecord data = new RDataRecord();
                data.Date = cols[0];
                data.Time = cols[1];
                data.Close = cols[2];
                data.Volume = cols[3];
                data.Stcb = cols[4];
                data.Stcs = cols[5];
                data.Trend = cols[6];
                data.UpperBand = cols[7];
                data.LowerBand = cols[8];
                data.MidLine = cols[9];
                data.Lavg1 = cols[10];
                data.Lavg2 = cols[11];
                data.Lavg3 = cols[12];
                data.Lavg4 = cols[13];
                if (cols.Length > 14)
                    data.LValues = cols[14].Replace(";", ",");
                if (cols.Length > 15)
                    data.TValues = cols[15].Replace(";", ",");
                result.Add(data);
            }

            return result;
        }

        public void UpdateClear()
        {
            this.DataList.Clear();
            this.vGrid.Model.RemoveRows(0, _RowsCount);
        }

        public void UpdateData(ObservableCollection<RDataRecord> datas)
        {
            try
            {
                UpdateClear();
                this._datas = datas;
                DoLoadingGridData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public async Task<int> UpdateDataByFetchingAsync(string symbol)
        {
            try
            {
                busyIndicator.IsBusy = true;
                UpdateClear();
                this.Symbol = symbol;

                if (this._datas == null)
                {
                    _datas = await FetchDataAndPopulateListViewAsync(symbol);
                }
                if (this._datas != null)
                {
                    DoLoadingGridData();
                    return 1;
                }
                else
                {
                    busyIndicator.IsBusy = false;
                    return -1;
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                busyIndicator.IsBusy = false;
                return -1;
            }
        }

        private void DoLoadingGridData()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_DoCompleted);
            bw.RunWorkerAsync();
        }
        private void bw_DoCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ScrollToBottom();
            busyIndicator.IsBusy = false;

            if (Initial)
            {
                if (firebaseClient == null) return;
                firebaseClient.Child(Symbol).AsObservable<string>().Subscribe(UpdateListViewRData);
                Initial = false;
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            ShownLavgColCount = 8;
            Dispatcher.BeginInvoke(() =>
            {
                LoadGridData(true);
            });

        }

        private void LoadGridData(bool init = false)
        {
            if (_datas == null) return;

            int totalCount = _datas.Count;
            int colCount = 0;

            if (totalCount <= 1)
            {
                return;
            }

            int IsSignal = 0;
            double TempCloseValue = 0;
            for (int idx = 1; idx < totalCount; idx++)
            {
                ChartItem item = new ChartItem();
                var datas = _datas[idx];
                item.StringDateTime = datas.Date + " " + datas.Time.Substring(0, 5);
                item.ClosePrice = datas.Close;

                double signValue;

                if (HardCode.IsCalculateTickGL)
                {
                    if (IsSignal != 0)
                    {
                        if (_datas.Count > 0)
                        {
                            double lclose = Double.Parse(_datas[idx - 1].Close);
                            double rclose = Double.Parse(item.ClosePrice);
                            if (IsSignal == 1)
                            {
                                TempCloseValue = TempCloseValue + (rclose - lclose);
                                item.Gain = $"{Math.Round(TempCloseValue, 4)}";
                            }
                            else
                            {
                                TempCloseValue = TempCloseValue + (lclose - rclose);
                                item.Gain = $"{Math.Round(TempCloseValue, 4)}";
                            }
                        }
                    }
                }

                if (double.TryParse(datas.Stcb, out signValue))
                {
                    if (signValue > 0)
                    {
                        item.DataSignal = "BUY";
                        IsSignal = 1;
                        TempCloseValue = 0;
                        item.Gain = datas.Close;
                    }
                }


                if (double.TryParse(datas.Stcs, out signValue))
                {
                    if (signValue > 0)
                    {
                        item.DataSignal = "SELL";
                        IsSignal = -1;
                        TempCloseValue = 0;
                        item.Gain = datas.Close;
                    }
                }

                item.Volume = datas.Volume.Trim();
                item.Trend = datas.Trend.Trim();
                item.Uppper = datas.UpperBand.Trim();
                item.Lower = datas.LowerBand.Trim();
                item.Midline = datas.MidLine.Trim();

                item.Lavg1 = datas.Lavg1.Trim();
                item.Lavg2 = datas.Lavg2.Trim();
                item.Lavg3 = datas.Lavg3.Trim();
                item.Lavg4 = datas.Lavg4.Trim();

                if(datas.LValues != null)
                {
                    item.LDataList = datas.LValues.Split(",").ToList<string>();
                }

                if (datas.TValues != null)
                {
                    item.TDataList = datas.TValues.Split(",").ToList<string>();
                }

                colCount = item.LDataList.Count + item.TDataList.Count;

                if (init)
                {
                    DataList.Add(item);
                }
            }

            _RowsCount = DataList.Count;
            _ColsCount = colCount + 3 + ShownLavgColCount;

            this.vGrid.Model.RowCount = _RowsCount;
            this.vGrid.Model.ColumnCount = _ColsCount + 1;

            this.vGrid.Model.ResizeColumnsToFit(GridRangeInfo.Cols(1, 2), GridResizeToFitOptions.ResizeCoveredCells);
            this.vGrid.Model.ResizeColumnsToFit(GridRangeInfo.Cols(2, 3), GridResizeToFitOptions.ResizeCoveredCells);
            this.vGrid.Model.ResizeColumnsToFit(GridRangeInfo.Cols(3, 4), GridResizeToFitOptions.ResizeCoveredCells);
            //this.vGrid.Model.ColumnWidths.SetRange(3, 3, 32);
            if (HardCode.CHART_SHOW_TREND)
            {
                this.vGrid.Model.ColumnWidths.SetRange(4, 4, HardCode.CHART_SET_PIXEL_SIZE);
            }
            else
            {
                this.vGrid.Model.ColumnWidths.SetRange(4, 4, 0);
            }

            if (HardCode.CHART_SHOW_UPPER)
            {
                this.vGrid.Model.ColumnWidths.SetRange(5, 5, HardCode.CHART_SET_PIXEL_SIZE);
            }
            else
            {
                this.vGrid.Model.ColumnWidths.SetRange(5, 5, 0);
            }

            if (HardCode.CHART_SHOW_LOWER)
            {
                this.vGrid.Model.ColumnWidths.SetRange(6, 6, HardCode.CHART_SET_PIXEL_SIZE);
            }
            else
            {
                this.vGrid.Model.ColumnWidths.SetRange(6, 6, 0);
            }

            if (HardCode.CHART_SHOW_MIDLINE)
            {
                this.vGrid.Model.ColumnWidths.SetRange(7, 7, HardCode.CHART_SET_PIXEL_SIZE);
            }
            else
            {
                this.vGrid.Model.ColumnWidths.SetRange(7, 7, 0);
            }

            if (HardCode.CHART_SHOW_LAVG1)
            {
                this.vGrid.Model.ColumnWidths.SetRange(8, 8, HardCode.CHART_SET_PIXEL_SIZE);
            }
            else
            {
                this.vGrid.Model.ColumnWidths.SetRange(8, 8, 0);
            }
            if (HardCode.CHART_SHOW_LAVG2)
            {
                this.vGrid.Model.ColumnWidths.SetRange(9, 9, HardCode.CHART_SET_PIXEL_SIZE);
            }
            else
            {
                this.vGrid.Model.ColumnWidths.SetRange(9, 9, 0);
            }
            if (HardCode.CHART_SHOW_LAVG3)
            {
                this.vGrid.Model.ColumnWidths.SetRange(10, 10, HardCode.CHART_SET_PIXEL_SIZE);
            }
            else
            {
                this.vGrid.Model.ColumnWidths.SetRange(10, 10, 0);
            }
            if (HardCode.CHART_SHOW_LAVG4)
            {
                this.vGrid.Model.ColumnWidths.SetRange(11, 11, HardCode.CHART_SET_PIXEL_SIZE);
            }
            else
            {
                this.vGrid.Model.ColumnWidths.SetRange(11, 11, 0);
            }
            this.vGrid.ScrollToBottom();
        }

        private void SriChartGrid_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (e.Cell.RowIndex > 0)
            {
                if (e.Cell.ColumnIndex > 0)
                {
                    if (DataList.Count == 0)
                    {
                        e.Handled = true;
                        return;
                    }

                    try
                    {
                        ChartItem item = DataList[e.Cell.RowIndex];
                        e.Style.CellType = "Static";
                        e.Style.ShowTooltip = true;
                        switch (e.Cell.ColumnIndex)
                        {
                            case 1:
                                e.Style.VerticalAlignment = VerticalAlignment.Center;
                                //e.Style.HorizontalAlignment = HorizontalAlignment.Center;
                                e.Style.Text = item.StringDateTime;
                                e.Style.Font.FontSize = FONTSIZE;
                                break;
                            case 2:
                                e.Style.VerticalAlignment = VerticalAlignment.Center;
                                
                                e.Style.Text = item.ClosePrice;
                                e.Style.Font.FontSize = FONTSIZE;
                                break;
                            case 3:
                                e.Style.VerticalAlignment = VerticalAlignment.Center;
                                e.Style.HorizontalAlignment = HorizontalAlignment.Center;
                                e.Style.Font.FontSize = FONTSIZE + 2;
                                e.Style.Foreground = CLR_WHITE;
                                if (item.DataSignal == "BUY")
                                {
                                    e.Style.Text = "B";
                                    e.Style.Background = CLR_DARKGREEN;
                                    e.Style.Font.FontWeight = FontWeights.Bold;
                                }
                                else if (item.DataSignal == "SELL")
                                {

                                    e.Style.Text = "S";
                                    e.Style.Background = CLR_DARKRED;
                                    e.Style.Font.FontWeight = FontWeights.Bold;
                                }
                                else
                                {
                                    e.Style.Text = item.Gain;
                                    e.Style.Foreground = CLR_YELLLOW;
                                    e.Style.Font.FontSize = FONTSIZE;
                                }

                                break;
                            case 4:
                                e.Style.VerticalAlignment = VerticalAlignment.Center;
                                e.Style.HorizontalAlignment = HorizontalAlignment.Center;
                                //e.Style.CellValue = item.Trend;
                                e.Style.ToolTip = $"Trend: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.CellValue2 = $"Trend: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                SetCellColor(item.Trend, e);
                                break;
                            case 5:
                                //e.Style.Text = item.Lavg1;
                                e.Style.VerticalAlignment = VerticalAlignment.Center;
                                e.Style.HorizontalAlignment = HorizontalAlignment.Center;
                                //e.Style.CellValue = item.Uppper;
                                e.Style.ToolTip = $"Upper: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.CellValue2 = $"Upper: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                SetCellColor(item.Uppper, e);
                                break;
                            case 6:
                                //e.Style.Text = item.Lavg1;
                                e.Style.VerticalAlignment = VerticalAlignment.Center;
                                e.Style.HorizontalAlignment = HorizontalAlignment.Center;
                                //e.Style.CellValue = item.Lower;
                                e.Style.ToolTip = $"Lower: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.CellValue2 = $"Lower: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                SetCellColor(item.Lower, e);
                                break;
                            case 7:
                                //e.Style.Text = item.Lavg1;
                                e.Style.VerticalAlignment = VerticalAlignment.Center;
                                e.Style.HorizontalAlignment = HorizontalAlignment.Center;
                                //e.Style.CellValue = item.Midline;
                                e.Style.ToolTip = $"MidLine: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.CellValue2 = $"MidLine: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                SetCellColor(item.Midline, e);
                                break;
                            case 8:
                                //e.Style.Text = item.Lavg1;
                                //e.Style.CellValue = item.Lavg1;
                                e.Style.ToolTip = $"Lavg1: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.CellValue2 = $"Lavg1: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                if (HardCode.CHART_SHOW_LAVG4 == false
                                    && HardCode.CHART_SHOW_LAVG3 == false
                                    && HardCode.CHART_SHOW_LAVG2 == false)
                                {
                                    e.Style.Borders.Right = new Pen(Brushes.DarkCyan, 3);
                                }
                                SetCellColor(item.Lavg1, e);
                                break;
                            case 9:
                                //e.Style.CellValue = item.Lavg2;
                                e.Style.ToolTip = $"Lavg2: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.CellValue2 = $"Lavg2: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                if (HardCode.CHART_SHOW_LAVG4 == false
                                    && HardCode.CHART_SHOW_LAVG3 == false)
                                {
                                    e.Style.Borders.Right = new Pen(Brushes.DarkCyan, 3);
                                }
                                SetCellColor(item.Lavg2, e);
                                break;
                            case 10:

                                //e.Style.CellValue = item.Lavg3;
                                e.Style.ToolTip = $"Lavg3: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.CellValue2 = $"Lavg3: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                if (HardCode.CHART_SHOW_LAVG4 == false)
                                {
                                    e.Style.Borders.Right = new Pen(Brushes.DarkCyan, 3);
                                }
                                SetCellColor(item.Lavg3, e);
                                break;
                            case 11:
                                //e.Style.CellValue = item.Lavg4;
                                e.Style.ToolTip = $"Lavg4: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.CellValue2 = $"Lavg4: DateTime: {item.StringDateTime} Price: {item.ClosePrice}";
                                e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                e.Style.Borders.Right = new Pen(Brushes.DarkCyan, 3);
                                SetCellColor(item.Lavg4, e);
                                break;

                            default:
                                if (HardCode.IS_CHART_REF_LINE)
                                {
                                    if (e.Cell.ColumnIndex == HardCode.CHART_REF_LINE1 + ShownLavgColCount + 3)
                                    {

                                        e.Style.Borders.Right = LINEPEN_YELLOW;
                                        e.Style.Borders.Left = LINEPEN_YELLOW;
                                    }
                                    else if (e.Cell.ColumnIndex == HardCode.CHART_REF_LINE2 + ShownLavgColCount + 3)
                                    {
                                        e.Style.Borders.Right = LINEPEN_YELLOW;
                                        e.Style.Borders.Left = LINEPEN_YELLOW;

                                    }
                                }

                                if (HardCode.DataFormat_Type == "ATST")
                                {

                                    int tindex = ShownLavgColCount + 3 + item.LDataList.Count;
                                    if (e.Cell.ColumnIndex <= tindex)
                                    {
                                        string lValue = item.LDataList[e.Cell.ColumnIndex - ShownLavgColCount - 4].Trim();

                                        e.Style.ToolTip = UtilMethods.LColumns[e.Cell.ColumnIndex - ShownLavgColCount - 4]
                                                                + ", Datetime: " + item.StringDateTime + ", Close Price: " + item.ClosePrice;
                                        e.Style.CellValue2 = UtilMethods.LColumns[e.Cell.ColumnIndex - ShownLavgColCount - 4]
                                                                + ", Datetime: " + item.StringDateTime + ", Close Price: " + item.ClosePrice;
                                        e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                        if (e.Cell.ColumnIndex - tindex == 0)
                                        {
                                            e.Style.Borders.Right = new Pen(Brushes.LightYellow, 2);
                                        }
                                        SetCellColor(lValue, e);
                                        e.Style.ShowTooltip = true;
                                    }
                                    else
                                    {

                                        if (item.TDataList.Count > (e.Cell.ColumnIndex - tindex - 1))
                                        {
                                            string lValue = item.TDataList[e.Cell.ColumnIndex - tindex - 1].Trim();

                                            e.Style.ToolTip = "T" + (e.Cell.ColumnIndex - tindex)
                                                                    + ", Datetime: " + item.StringDateTime + ", Close Price: " + item.ClosePrice;
                                            e.Style.CellValue2 = "T" + (e.Cell.ColumnIndex - tindex)
                                                                    + ", Datetime: " + item.StringDateTime + ", Close Price: " + item.ClosePrice;
                                            e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";

                                            SetCellColor(lValue, e);
                                            e.Style.ShowTooltip = true;
                                        }
                                    }
                                }
                                else
                                {

                                    string lValue = item.LDataList[e.Cell.ColumnIndex - ShownLavgColCount - 4].Trim();

                                    e.Style.ToolTip = $"L{e.Cell.ColumnIndex - ShownLavgColCount - 3}"
                                                            + ", Datetime: " + item.StringDateTime + ", Close Price: " + item.ClosePrice;
                                    e.Style.CellValue2 = $"L{e.Cell.ColumnIndex - ShownLavgColCount - 3}"
                                                            + ", Datetime: " + item.StringDateTime + ", Close Price: " + item.ClosePrice;
                                    e.Style.TooltipTemplateKey = "TemplateTooltipSriChartCell";
                                    SetCellColor(lValue, e);
                                    e.Style.ShowTooltip = true;
                                }

                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                e.Handled = true;

            }
        }

        private void SetCellColor(string value, GridQueryCellInfoEventArgs e)
        {
            double cv = 0;

            if (e.Cell.ColumnIndex == date_index || e.Cell.ColumnIndex == time_index)
            {
                e.Style.Background = CLR_BLACK;
            }
            else if (e.Style.ToolTip.ToString().Split(':')[0] == "Trend")
            {

                Double.TryParse(value, out cv);

                if (cv > HardCode._TrendBuy)
                {
                    e.Style.Background = CLR_DARKGREEN;

                    if (!HardCode._IsShowValue)
                    {
                        e.Style.Foreground = CLR_DARKGREEN;
                    }
                    else
                    {
                        e.Style.Foreground = CLR_YELLLOW;
                    }
                }
                else if (cv < HardCode._TrendSell)
                {
                    e.Style.Background = CLR_DARKRED;
                    if (!HardCode._IsShowValue)
                    {
                        e.Style.Foreground = CLR_DARKRED;
                    }
                    else
                    {
                        e.Style.Foreground = CLR_YELLLOW;
                    }
                }
                else
                {
                    e.Style.Background = CLR_BLACK;
                    e.Style.Foreground = CLR_BLACK;
                }

            }
            else if (e.Style.ToolTip.ToString().Split(':')[0] == "Upper")
            {

                Double.TryParse(value, out cv);

                if (cv > HardCode._UpperBuy)
                {
                    e.Style.Background = CLR_DARKGREEN;

                    if (!HardCode._IsShowValue)
                    {
                        e.Style.Foreground = CLR_DARKGREEN;
                    }
                    else
                    {
                        e.Style.Foreground = CLR_YELLLOW;
                    }
                }
                else if (cv < HardCode._UpperSell)
                {
                    e.Style.Background = CLR_DARKRED;
                    if (!HardCode._IsShowValue)
                    {
                        e.Style.Foreground = CLR_DARKRED;
                    }
                    else
                    {
                        e.Style.Foreground = CLR_YELLLOW;
                    }
                }
                else
                {
                    e.Style.Background = CLR_BLACK;
                    e.Style.Foreground = CLR_BLACK;
                }

            }
            else if (e.Style.ToolTip.ToString().Split(':')[0] == "Lower")
            {

                Double.TryParse(value, out cv);

                if (cv > HardCode._LowerBuy)
                {
                    e.Style.Background = CLR_DARKGREEN;

                    if (!HardCode._IsShowValue)
                    {
                        e.Style.Foreground = CLR_DARKGREEN;
                    }
                    else
                    {
                        e.Style.Foreground = CLR_YELLLOW;
                    }
                }
                else if (cv < HardCode._LowerSell)
                {
                    e.Style.Background = CLR_DARKRED;
                    if (!HardCode._IsShowValue)
                    {
                        e.Style.Foreground = CLR_DARKRED;
                    }
                    else
                    {
                        e.Style.Foreground = CLR_YELLLOW;
                    }
                }
                else
                {
                    e.Style.Background = CLR_BLACK;
                    e.Style.Foreground = CLR_BLACK;
                }

            }
            else if (e.Style.ToolTip.ToString().Split(':')[0] == "MidLine")
            {

                Double.TryParse(value, out cv);

                if (cv > HardCode._MidLineBuy)
                {
                    e.Style.Background = CLR_DARKGREEN;

                    if (!HardCode._IsShowValue)
                    {
                        e.Style.Foreground = CLR_DARKGREEN;
                    }
                    else
                    {
                        e.Style.Foreground = CLR_YELLLOW;
                    }
                }
                else if (cv < HardCode._MidLineBuy)
                {
                    e.Style.Background = CLR_DARKRED;
                    if (!HardCode._IsShowValue)
                    {
                        e.Style.Foreground = CLR_DARKRED;
                    }
                    else
                    {
                        e.Style.Foreground = CLR_YELLLOW;
                    }
                }
                else
                {
                    e.Style.Background = CLR_BLACK;
                    e.Style.Foreground = CLR_BLACK;
                }

            }
            else if (e.Style.ToolTip.ToString().Split(':')[0] == "Lavg1")
            {
                Double.TryParse(value, out cv);

                if (cv > HardCode._Lagv1Buy)
                {
                    e.Style.Background = CLR_DARKGREEN;
                    e.Style.Foreground = CLR_DARKGREEN;

                }
                else if (cv < HardCode._Lagv1Sell)
                {
                    e.Style.Background = CLR_DARKRED;
                    e.Style.Foreground = CLR_DARKRED;
                }
                else
                {
                    e.Style.Background = CLR_BLACK;
                    e.Style.Foreground = CLR_BLACK;
                }
            }
            else if (e.Style.ToolTip.ToString().Split(':')[0] == "Lavg2")
            {
                Double.TryParse(value, out cv);

                if (cv > HardCode._Lagv2Buy)
                {
                    e.Style.Background = CLR_DARKGREEN;
                    e.Style.Foreground = CLR_DARKGREEN;

                }
                else if (cv < HardCode._Lagv2Sell)
                {
                    e.Style.Background = CLR_DARKRED;
                    e.Style.Foreground = CLR_DARKRED;
                }
                else
                {
                    e.Style.Background = CLR_BLACK;
                    e.Style.Foreground = CLR_BLACK;
                }
            }
            else if (e.Style.ToolTip.ToString().Split(':')[0] == "Lavg3")
            {
                Double.TryParse(value, out cv);

                if (cv > HardCode._Lagv3Buy)
                {
                    e.Style.Background = CLR_DARKGREEN;
                    e.Style.Foreground = CLR_DARKGREEN;

                }
                else if (cv < HardCode._Lagv3Sell)
                {
                    e.Style.Background = CLR_DARKRED;
                    e.Style.Foreground = CLR_DARKRED;
                }
                else
                {
                    e.Style.Background = CLR_BLACK;
                    e.Style.Foreground = CLR_BLACK;
                }
            }
            else if (e.Style.ToolTip.ToString().Split(':')[0] == "Lavg4")
            {
                Double.TryParse(value, out cv);

                if (cv > HardCode._Lagv4Buy)
                {
                    e.Style.Background = CLR_DARKGREEN;
                    e.Style.Foreground = CLR_DARKGREEN;

                }
                else if (cv < HardCode._Lagv4Sell)
                {
                    e.Style.Background = CLR_DARKRED;
                    e.Style.Foreground = CLR_DARKRED;
                }
                else
                {
                    e.Style.Background = CLR_BLACK;
                    e.Style.Foreground = CLR_BLACK;
                }
            }
            else
            {
                if (value == "")
                {
                    e.Style.Background = CLR_DARKRED;
                }
                else if (value == "0")
                {
                    e.Style.Background = CLR_BLACK;
                }
                else if (value == "1")
                {
                    e.Style.Background = CLR_DARKGREEN;
                }
            }
        }

        public void ScrollToBottom()
        {
            if (vGrid.Model.RowCount == 0) return;
            vGrid.ColumnWidths.SetHidden(0, 0, true);
            vGrid.RowHeights.SetHidden(0, 0, true);
            vScroll.ScrollToBottom();
        }
    }
}
