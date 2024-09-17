using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VedicaTraderCryptoLEMON.Models;
using VedicaTraderCryptoLEMON.SubViews;
using VedicaTraderCryptoLEMON.Utils;
using VedicaTraderCryptoLEMON.Views;
using static VedicaTraderCryptoLEMON.Controllers.Manager;

namespace VedicaTraderCryptoLEMON
{
    public class ViewModel : NotificationObject
    {
        private FirebaseClient firebaseClient;
        private async Task<ObservableCollection<RDataRecord>> FetchDataAndPopulateListViewAsync(string tableName)
        {
            try
            {
                if (!HardCode.IsTest)
                {
                    ConnectColor = "DarkGreen";
                    ConnectStatus = "Connected";
                    ObservableCollection<RDataRecord> RDataSet = new ObservableCollection<RDataRecord>();
                    var RDataSnapshot = await firebaseClient.Child(tableName).OrderByKey().LimitToLast(HardCode.ROWTOLOAD).OnceAsJsonAsync();
                    if (String.IsNullOrEmpty(RDataSnapshot))
                    {
                        ConnectColor = "DarkRed";
                        ConnectStatus = "Disconnect";
                        return null;
                    }
                    JObject json = JObject.Parse(RDataSnapshot);
                    if (json["data"] != null)
                    {
                        RDataSet = GetDataSetFromString(json["data"].ToString());
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
                ex.ToString();
                ConnectColor = "DarkRed";
                ConnectStatus = "Disconnect";

                UtilMethods.ShowMessageBox("There is no such symbol. Please check again.");
                return null;
            }
        }

        private void ConnectFirebase()
        {
            firebaseClient = new FirebaseClient(
              HardCode.FB_URL,
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(HardCode.FB_SECRET)
              });
        }

        // Loading WorkSpaceConfigs
        public List<WorkspaceConfig> _workconfigs = new List<WorkspaceConfig>();

        public Dictionary<string, string> _dualChartsCrypto = new Dictionary<string, string>();


        // Loading DualChartMenu

        // Others
        private int MENUSUBITEM_WIDTH = 160;

        private string appTille = "VedicaTrader Crypto LEMON";
        public string AppTitle
        {
            get { return appTille; }
            set
            {
                this.RaisePropertyChanged("AppTitle");
            }
        }


        private string connectStatus = "Connected";
        public string ConnectStatus
        {
            get { return connectStatus; }
            set
            {
                connectStatus = value;
                this.RaisePropertyChanged("ConnectStatus");
            }
        }

        private string connectColor = "DarkGreen";
        public string ConnectColor
        {
            get { return connectColor; }
            set
            {
                connectColor = value;
                this.RaisePropertyChanged("ConnectColor");
            }
        }

        // Menu
        private ObservableCollection<MenuModel> menuModels;
        public ObservableCollection<MenuModel> MenuModels
        {
            get
            {
                return menuModels;
            }
            set
            {
                menuModels = value;
                RaisePropertyChanged("MenuModels");
            }
        }

        // TabControl
        ObservableCollection<TabItemModel> tabItems = new ObservableCollection<TabItemModel>();

        public ObservableCollection<TabItemModel> TabModels
        {
            get
            {
                return tabItems;
            }
            set
            {
                tabItems = value;
                RaisePropertyChanged("TabModels");
            }
        }

        public ViewModel()
        {
            _dualChartsCrypto = UtilMethods.GetDualChartDic(HardCode.FB_SYMBOLS_CRYPTO);

            InitialzieMenuModels();
            ConnectFirebase();
            InitializeTabModels();
        }

        public async void InitialzieMenuModels()
        {
            ActivationCodeCheckAndRunMain actCheck = new ActivationCodeCheckAndRunMain();
            await actCheck.CheckIfIsValidAndRunMainFormAsync();

            // Menu Model Initialize
            workspaceMenuGridCommand = new DelegateCommand<object>(WorkspaceMenuGridCommandHandler);
            dualchartCryptoMenuGridCommand = new DelegateCommand<object>(DualChartCryptoMenuGridCommandHandler);
            helpMenuGridCommand = new DelegateCommand<object>(HelpMenuGridCommandHandler);
            optionMenuGridCommand = new DelegateCommand<object>(OptionMenuGridCommandHandler);

            ResourceDictionary CommonResourceDictionary = new ResourceDictionary() { Source = new Uri("/VedicaTraderCryptoLEMON;component/Assets/Menu/Icon.xaml", UriKind.RelativeOrAbsolute) };

            MenuModel menuWork = new MenuModel() { Name = "WorkSpace" };
            MenuModel dualChartWorkCrypto = new MenuModel() { Name = "Crypto Charts" };
            MenuModel chartWork = new MenuModel() { Name = "Chart" };
            MenuModel toolsWork = new MenuModel() { Name = "Tools" };
            MenuModel signalWork = new MenuModel() { Name = "Signal Screener" };
            MenuModel garudaWork = new MenuModel() { Name = "GarudaViews" };
            MenuModel optionsWork = new MenuModel() { Name = "Options" };
            MenuModel HelpWork = new MenuModel() { Name = "Help" };

            menuWork.MenuCollection.Add(new MenuModel() { Name = "New", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand, Icon = CommonResourceDictionary["New"] as object, ImageTemplate = CommonResourceDictionary["NewPathIcon"] as DataTemplate });
            menuWork.MenuCollection.Add(new MenuModel() { Name = "Open", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand, Icon = CommonResourceDictionary["Open"] as object, ImageTemplate = CommonResourceDictionary["OpenPathIcon"] as DataTemplate });
            menuWork.MenuCollection.Add(new MenuModel() { Name = "Save", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand, Icon = CommonResourceDictionary["Save"] });
            menuWork.MenuCollection.Add(new MenuModel() { Name = "Save All        ", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand, Icon = CommonResourceDictionary["SaveAll"] });
            menuWork.MenuCollection.Add(new MenuModel() { Name = "Rename", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });
            menuWork.MenuCollection.Add(new MenuModel() { Name = "Delete", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });
            menuWork.MenuCollection.Add(new MenuModel() { Name = "Close", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand});
            menuWork.MenuCollection.Add(new MenuModel() { Name = "Close All       ", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand, Icon = CommonResourceDictionary["Repeat"] });
            menuWork.MenuCollection.Add(new MenuModel() { Name = "Exit", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand, Icon = CommonResourceDictionary["FileDelete"] as object, ImageTemplate = CommonResourceDictionary["DeletePathIcon"] as DataTemplate });

            foreach (string key in _dualChartsCrypto.Keys)
            {
                //dualChartWork.MenuCollection.Add(new MenuModel() { Name = $"{key}  ", Width = MENUSUBITEM_WIDTH, Command = dualchartMenuGridCommand, Icon = CommonResourceDictionary["Chart"]});
                dualChartWorkCrypto.MenuCollection.Add(new MenuModel() { Name = $"{key}  ", Width = MENUSUBITEM_WIDTH, Command = dualchartCryptoMenuGridCommand });
            }

            chartWork.MenuCollection.Add(new MenuModel() { Name = "New Chart      ", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });

            toolsWork.MenuCollection.Add(new MenuModel() { Name = "Grid Chart", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });
            toolsWork.MenuCollection.Add(new MenuModel() { Name = "Column Chart", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });
            toolsWork.MenuCollection.Add(new MenuModel() { Name = "Grid + Column Chart", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });

            signalWork.MenuCollection.Add(new MenuModel() { Name = "Grid Chart    ", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });
            signalWork.MenuCollection.Add(new MenuModel() { Name = "Column Chart  ", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });

            garudaWork.MenuCollection.Add(new MenuModel() { Name = "Industry View ", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });
            garudaWork.MenuCollection.Add(new MenuModel() { Name = "ETF View", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });
            garudaWork.MenuCollection.Add(new MenuModel() { Name = "Index View", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });

            //optionsWork.MenuCollection.Add(new MenuModel() { Name = "Chart Setting  ", Width = MENUSUBITEM_WIDTH, Command = WorkspaceMenuGridCommand });

            optionsWork.MenuCollection.Add(new MenuModel() { Name = "Cascade       ", Width = MENUSUBITEM_WIDTH, Command = optionMenuGridCommand, Icon = CommonResourceDictionary["CascadeWindow"] as object, ImageTemplate = CommonResourceDictionary["CascadeWindowPathIcon"] as DataTemplate });
            optionsWork.MenuCollection.Add(new MenuModel() { Name = "Horizontal    ", Width = MENUSUBITEM_WIDTH, Command = optionMenuGridCommand, Icon = CommonResourceDictionary["ArrangeWindow"] as object, ImageTemplate = CommonResourceDictionary["ArrangeWindowPathIcon"] as DataTemplate });
            optionsWork.MenuCollection.Add(new MenuModel() { Name = "Vertical      ",  Width = MENUSUBITEM_WIDTH, Command = optionMenuGridCommand, Icon = CommonResourceDictionary["SideBySide"] as object, ImageTemplate = CommonResourceDictionary["SideBySidePathIcon"] as DataTemplate });

            HelpWork.MenuCollection.Add(new MenuModel() { Name = "Help Topic        ", Width = MENUSUBITEM_WIDTH, Command = HelpMenuGridCommand, Icon = CommonResourceDictionary["Help"] as object });
            HelpWork.MenuCollection.Add(new MenuModel() { Name = "Support  ", Width = MENUSUBITEM_WIDTH, Command = HelpMenuGridCommand });
            HelpWork.MenuCollection.Add(new MenuModel() { Name = "About", Width = MENUSUBITEM_WIDTH, Command = HelpMenuGridCommand });
            HelpWork.MenuCollection.Add(new MenuModel() { Name = "Deavtivate", Width = MENUSUBITEM_WIDTH, Command = HelpMenuGridCommand });

            MenuModels = new ObservableCollection<MenuModel>();

            menuModels.Add(menuWork);
            menuModels.Add(dualChartWorkCrypto);
            //menuModels.Add(toolsWork);
            //menuModels.Add(signalWork);
            //menuModels.Add(garudaWork);
            menuModels.Add(optionsWork);
            menuModels.Add(HelpWork);
        }

        private void DualChartCryptoMenuGridCommandHandler(object obj)
        {
            string menuItemName = (string)obj;
            menuItemName = menuItemName.Trim();
            if (_dualChartsCrypto.ContainsKey(menuItemName))
            {
                LoadDualChart(_dualChartsCrypto[menuItemName], menuItemName);
            }
        }


        public void InitializeTabModels()
        {
            WorkspaceConfig config = new WorkspaceConfig(true);
            TabItemModel model = new TabItemModel(config._name, false);
            foreach(ChartInfo charInfo in config._symbols)
            {
                model.ChartModels.Add(charInfo);
            }
            tabItems.Add(model);
        }



        public MenuModel GetSubMenuModel(string name, ResourceDictionary CommonResourceDictionary)
        {
            MenuModel model = new MenuModel() { Name = "New", Icon = CommonResourceDictionary["FileChart"] as object, ImageTemplate = CommonResourceDictionary["ChartPathIcon"] as DataTemplate, Command = WorkspaceMenuGridCommand };
            return model;
        }



        public ICommand workspaceMenuGridCommand;
        public ICommand WorkspaceMenuGridCommand
        {
            get
            {
                return workspaceMenuGridCommand;
            }
        }

        public ICommand dualchartCryptoMenuGridCommand;
        public ICommand DualchartCryptoMenuGridCommand
        {
            get
            {
                return dualchartCryptoMenuGridCommand;
            }
        }


        public ICommand helpMenuGridCommand;
        public ICommand HelpMenuGridCommand
        {
            get
            {
                return helpMenuGridCommand;
            }
        }


        public ICommand optionMenuGridCommand;
        public ICommand OptionMenuGridCommand
        {
            get
            {
                return helpMenuGridCommand;
            }
        }

        private bool IsExistedTabItem(string header)
        {
            foreach (TabItemModel model in tabItems)
            {
                if (model.Header == header)
                {
                    return true;
                }
            }
            return false;
        }

        private TabItemModel GetTabItemModel()
        {
            foreach(TabItemModel model in TabModels)
            {
                if(model.Header == CurrentTabName)
                    return model;
            }
            return null;
        }

        private void RenameworkSpace()
        {
            if (CurrentTabName == "DEFAULT") return;
            WinRenameWorkSpace form = new WinRenameWorkSpace();
            SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
            if (form.ShowDialog() == true)
            {
                if (IsExistedTabItem(form._name))
                {
                    UtilMethods.ShowMessageBox($"The workspace's name '{form._name}' is already exsited.");
                    return;
                }
                int index = 0;
                TabItemModel cmodel = null;
                foreach (TabItemModel model in tabItems)
                {
                    if (model.Header == CurrentTabName)
                    {
                        cmodel = model;
                        cmodel.Header = form._name;
                        break;
                    }
                    index++;
                }
                tabItems.RemoveAt(index);
                tabItems.Insert(index, cmodel);
                CurrentTabName = form._name;
            }
            UtilMethods.ShowMessageBox(UtilMethods.MSGB_UPDATE);
        }

        private void OpenworkSpace()
        {
            WinOpenWorkSpace form = new WinOpenWorkSpace();
            SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
            if (form.ShowDialog() == true)
            {
                if (IsExistedTabItem(form._name))
                {
                    UtilMethods.ShowMessageBox($"The workspace's name '{form._name}' is already exsited.");
                    return;
                }
                WorkspaceConfig config = UtilMethods.LoadWorkspaceInfo(form._name);
                TabItemModel model = new TabItemModel(form._name);
                foreach(ChartInfo infor in config._symbols)
                {
                    model.ChartModels.Add(infor);

                }
                tabItems.Add(model);
            }
        }

        private void CloseWorkSpace()
        {
            if (CurrentTabName == "DEFAULT") return;

            int index = 0;
            foreach(TabItemModel model in tabItems)
            {
                if(model.Header == CurrentTabName)
                {
                    break;
                }
                index++;
            }

            tabItems.RemoveAt(index);
        }

        private void CloseAllWorkSpace()
        {
            while(tabItems.Count != 1)
            {
                tabItems.RemoveAt(1);
            }
        }

        private void SaveWorkSpace(string name)
        {
            if (name == null || name == "DEFAULT") return;
            TabItemModel model = GetTabItemModel();
            if (model == null) return;
            if (model.ChartModels.Count == 0) return;
            WorkspaceConfig config = new WorkspaceConfig(false);
            config._name = name;
            config._datetime = DateTime.Now;
            foreach (ChartInfo chartModel in model.ChartModels)
            {
                config._symbols.Add(chartModel);
            }
            config.Save();
            UtilMethods.ShowMessageBox(UtilMethods.MSGB_SAVE);
        }

        private void SaveAllWorkSpace()
        {
            foreach (TabItemModel model in tabItems)
            {
                if (model.Header == "DEFAULT") continue;
                if (model == null) continue;
                if (model.ChartModels.Count == 0) return;
                WorkspaceConfig config = new WorkspaceConfig(false);
                config._name = model.Header;
                config._datetime = DateTime.Now;
                foreach (ChartInfo chartModel in model.ChartModels)
                {
                    config._symbols.Add(chartModel);
                }
                config.Save();
            }

            UtilMethods.ShowMessageBox(UtilMethods.MSGB_SAVE);
        }


        private void WorkspaceMenuGridCommandHandler(object obj)
        {

            string menuItemName = (string) obj;

            switch (menuItemName.Trim())
            {
                case "New":
                    WinNewWorkSpace form = new WinNewWorkSpace();
                    SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
                    if (form.ShowDialog() == true)
                    {
                        if (IsExistedTabItem(form._name))
                        {
                            UtilMethods.ShowMessageBox($"The workspace's name '{form._name}' is already exsited.");
                            return;
                        }
                        TabItemModel model = new TabItemModel(form._name);
                        tabItems.Add(model);
                    }
                    return;
                case "Open":
                    OpenworkSpace();
                    return;
                case "Save":
                    SaveWorkSpace(CurrentTabName);
                    return;
                case "Save All":
                    SaveAllWorkSpace();
                    return;
                case "Delete":
                    if (CurrentTabName == "DEFAULT") return;
                    CloseWorkSpace();
                    string filepathForDel = UtilMethods.GetSaveFilePath(CurrentTabName);
                    if (File.Exists(filepathForDel))
                    {
                        File.Delete(filepathForDel);
                    }
                    UtilMethods.ShowMessageBox(UtilMethods.MSGB_DELETE);
                    return;
                case "Rename":
                    RenameworkSpace();
                    return;
                case "Close":
                    CloseWorkSpace();
                    return;
                case "Close All":
                    CloseAllWorkSpace();
                    return;
                case "Exit":
                    Application.Current.Shutdown();
                    return;
            }
        }

        //private void DoLoadingDualChart(string menuItemName)
        //{
        //    BackgroundWorker bw = new BackgroundWorker();
        //    bw.DoWork += new DoWorkEventHandler(bw_DoLoadingDualChartWork);
        //    bw.RunWorkerAsync(argument: menuItemName);
        //}

        //private void bw_DoLoadingDualChartWork(object? sender, DoWorkEventArgs e)
        //{
        //    string menuItemName = e.Argument as string;
        //    Application.Current.Dispatcher.BeginInvoke(() =>
        //    {
        //        LoadDualChart(menuItemName);
        //    });
        //}

        private async void LoadDualChart(string symbol, string name)
        {
            foreach (TabItemModel model in tabItems)
            {
                if (model.Header == CurrentTabName)
                {
                    ChartInfo chartinfo = new ChartInfo(symbol, name);

                    if (chartinfo._data != null)
                    {
                        CtrlDualChart chart = new CtrlDualChart();
                        var ret = await chart.UpdateDataByFetchingAsync(symbol);
                        if (ret == -1) return;
                        DocumentContainer.SetHeader(chart, name);
                        model.ChartModels.Insert(0, chartinfo);
                        documentContainer.Items.Insert(0, chart);
                    }
                    break;
                }
            }
            Cascade(1);
        }

        private void HelpMenuGridCommandHandler(object obj)
        {
            string menuItemName = (string)obj;
            menuItemName=menuItemName.Trim();
            switch (menuItemName)
            {
                case "Help Topic":
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = UtilMethods.URL_HELP_TOPIC,
                        UseShellExecute = true
                    });
                    return;
                case "Support":
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = UtilMethods.URL_SUPPORT,
                        UseShellExecute = true
                    });
                    return;
                case "About":
                    WinAbout form = new WinAbout();
                    SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
                    form.ShowDialog();
                    return;
                case "Deactivate":
                    WinDeActivation form1 = new WinDeActivation();
                    SfSkinManager.SetTheme(form1, new Theme() { ThemeName = "Office2019Black" });
                    form1.ShowDialog();
                    return;
            }
            MessageBox.Show(menuItemName);
        }

        private void OptionMenuGridCommandHandler(object obj)
        {
            string menuItemName = (string)obj;
            switch (menuItemName.Trim())
            {
                case "Cascade":
                    Cascade(1);
                    return;
                case "Horizontal":
                    TileHorizontal(1);
                    return;
                case "Vertical":
                    TileVertical(1);
                    return;
            }
        }

        private Brush titleBarBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));
        private Brush titleBarForeground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        public Brush TitleBarBackground
        {
            get { return titleBarBackground; }
            set
            {
                titleBarBackground = value;
                this.RaisePropertyChanged("TitleBarBackground");
            }
        }

        private void Cascade(object parameter)
        {
            double top = 0.0;
            double left = 0.0;

            for (int i = documentContainer.Items.Count - 1; i >= 0; i--)
            {
                UIElement element = documentContainer.Items[i] as UIElement;
                DocumentContainer.SetMDIBounds(element, new Rect(left, top, 500, 300));
                left += 60;
                top += 60;
                if (i == 0)
                {
                    documentContainer.ActiveDocument = element;
                }
            }
            HardCode.LayoutState = 0;
        }

        private void TileHorizontal(object parameter)
        {            
            double height = documentContainer.ActualHeight / documentContainer.Items.Count - 3;
            double top = 0.0;
            double left = 0.0;
            foreach (UIElement element in documentContainer.Items)
            {
                DocumentContainer.SetMDIBounds(element, new Rect(left, top, documentContainer.ActualWidth - 4, height));
                top += height;
            }
            HardCode.LayoutState = 2;
        }

        private void TileVertical(object parameter)
        {
            double width = documentContainer.ActualWidth / documentContainer.Items.Count - 4;
            double top = 0.0;
            double left = 0.0;
            foreach (UIElement element in documentContainer.Items)
            {
                DocumentContainer.SetMDIBounds(element, new Rect(left, top, width, documentContainer.ActualHeight - 3));
                left += width;
            }
            HardCode.LayoutState = 1;
        }

        public Brush TitleBarForeground
        {
            get { return titleBarForeground; }
            set
            {
                titleBarForeground = value;
                this.RaisePropertyChanged("TitleBarForeground");
            }
        }


        string title1 = "ASD1";
        string title2 = "ASD2";
        string title3 = "ASD3";
        public string ChartTitle1
        {
            get { return title1; }
            set
            {
                this.RaisePropertyChanged("ChartTitle1");
            }
        }

        public string ChartTitle2
        {
            get { return title2; }
            set
            {
                this.RaisePropertyChanged("ChartTitle2");
            }
        }

        public string ChartTitle3
        {
            get { return title3; }
            set
            {
                this.RaisePropertyChanged("ChartTitle3");
            }
        }

        private static DocumentContainer documentContainer = null;

        public static string GetDocumentContainer(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentContainerProperty);
        }

        public static readonly DependencyProperty DocumentContainerProperty =
    DependencyProperty.RegisterAttached("DocumentContainer", typeof(string), typeof(ViewModel), new FrameworkPropertyMetadata(OnDocumentContainerChanged));

        public static void SetDocumentContainer(DependencyObject obj, DocumentContainer value)
        {
            obj.SetValue(DocumentContainerProperty, value);
        }
        public static void OnDocumentContainerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            documentContainer = obj as DocumentContainer;
        }


        private static string CurrentTabName = null;

        public static string GetCurrentTabName(DependencyObject obj)
        {
            return (string)obj.GetValue(CurrentTabNameProperty);
        }

        public static readonly DependencyProperty CurrentTabNameProperty =
    DependencyProperty.RegisterAttached("CurrentTabName", typeof(string), typeof(ViewModel), new FrameworkPropertyMetadata(OnCurrentTabNameChanged));

        private static void OnCurrentTabNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                CurrentTabName = e.NewValue.ToString();
            }

        }

        public static void SetCurrentTabName(DependencyObject obj, DocumentContainer value)
        {
            obj.SetValue(DocumentContainerProperty, value);
        }

        private UIElement activeDocument;

        public UIElement ActiveDocument
        {
            get
            {
                return activeDocument;
            }

            set
            {
                activeDocument = value;
                RaisePropertyChanged("ActiveDocument");
            }
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

    }
}
