#region Copyright Syncfusion Inc. 2001-2024.
// Copyright Syncfusion Inc. 2001-2024. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Amazon.S3.Model;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VedicaTraderCryptoLEMON.Models;
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON
{
    /// <summary>
    /// Interaction logic for GridView.xaml
    /// </summary>
    public partial class CtrlDocumentView : UserControl
    {

        public ObservableCollection<ChartInfo> ChartInfos
        {
            get { return (ObservableCollection<ChartInfo>)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
               DependencyProperty.Register("ChartInfos", typeof(ObservableCollection<ChartInfo>), typeof(CtrlDocumentView), new PropertyMetadata(null, new PropertyChangedCallback(OnTabItemSelectChanged)));

        private static void OnTabItemSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CtrlDocumentView UserControl1Control = d as CtrlDocumentView;
            UserControl1Control.OnTabItemSelectChanged(e);
        }

        public CtrlDocumentView()
        {
            InitializeComponent();
            container.CloseButtonClick += Container_CloseButtonClick;
        }

        private void Container_CloseButtonClick(object sender, CloseButtonEventArgs e)
        {
            CtrlDualChart chart = (CtrlDualChart) e.TargetItem;
            string symbol = chart.Symbol;
            DocumentContainer container = (DocumentContainer)sender;
            TabItemModel tabmodel = (TabItemModel)container.DataContext;
            int index = 0;
            foreach (ChartInfo cinfo in tabmodel.ChartModels)
            {
                if (symbol == cinfo._symbol)
                {
                    break;
                }
                index++;
            }
            Dispatcher.BeginInvoke(() =>
            {
                tabmodel.ChartModels.RemoveAt(index);
                container.Items.RemoveAt(index);
            });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private async void OnTabItemSelectChanged(DependencyPropertyChangedEventArgs e)
        {
            ObservableCollection<ChartInfo> tabsymbols = (ObservableCollection<ChartInfo>)e.NewValue;
            ObservableCollection<ChartInfo> oldtabsymbols = (ObservableCollection<ChartInfo>)e.OldValue;
            int LayoutState = HardCode.LayoutState;

            if (tabsymbols == null) return;


            if (container.ActualWidth == 0)
            {
                double top = 0.0;
                double left = 0.0;
                foreach (ChartInfo tab in tabsymbols)
                {
                    CtrlDualChart chart = new CtrlDualChart();
                    chart.Symbol = tab._symbol;
                    int ret = await chart.UpdateDataByFetchingAsync(tab._symbol);
                    if(ret == 1)
                    {
                        DocumentContainer.SetHeader(chart, $"{tab._name}");
                        DocumentContainer.SetMDIBounds(chart, new Rect(left, top, 500, 300));
                        container.Items.Add(chart);
                        left += 60;
                        top += 60;
                    }
                }

                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerAsync();

            }
            else
            {
                container.Items.Clear();

                for (int i = 0; i < tabsymbols.Count; i++)
                {
                    ChartInfo tab = tabsymbols[i];
                    CtrlDualChart chart = new CtrlDualChart();
                    chart.Symbol = tab._symbol;
                    int ret = await chart.UpdateDataByFetchingAsync(tab._symbol);
                    if (ret == 1)
                    {
                        DocumentContainer.SetHeader(chart, $"{tab._name}");
                        DocumentContainer.SetMDIWindowState(chart, MDIWindowState.Normal);
                        container.Items.Add(chart);
                    }
                }

                if (LayoutState == 0)
                {
                    Cascade();

                }else if(LayoutState == 1)
                {
                    TileVertical(tabsymbols);
                }
                else
                {
                    TileHorizontal(tabsymbols);
                }
            }
        }

        private void TileVertical(ObservableCollection<ChartInfo> tabsymbols)
        {
            double width = container.ActualWidth / tabsymbols.Count - 4;
            double top = 0.0;
            double left = 0.0;
            foreach (UIElement element in container.Items)
            {
                DocumentContainer.SetMDIBounds(element, new Rect(left, top, width, container.ActualHeight - 3));
                left += width;
            }
        }

        private void TileHorizontal(ObservableCollection<ChartInfo> tabsymbols)
        {
            double height = container.ActualHeight / tabsymbols.Count - 3;
            double top = 0.0;
            double left = 0.0;
            foreach (UIElement element in container.Items)
            {
                DocumentContainer.SetMDIBounds(element, new Rect(left, top, container.ActualWidth - 4, height));
                top += height;
            }
        }
        private void Cascade()
        {
            double top = 0.0;
            double left = 0.0;

            for (int i = container.Items.Count - 1; i >= 0; i--)
            {
                UIElement element = container.Items[i] as UIElement;
                DocumentContainer.SetMDIBounds(element, new Rect(left, top, 500, 300));
                left += 60;
                top += 60;
            }
        }



        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(100);
            Dispatcher.BeginInvoke(() =>
            {
                double width = container.ActualWidth / container.Items.Count - 4;
                double top = 0.0;
                double left = 0.0;
                
                foreach (CtrlDualChart element in container.Items)
                {
                    element.ScrollToBottom();
                    DocumentContainer.SetMDIBounds(element, new Rect(left, top, width, container.ActualHeight - 3));
                    left += width;
                }
            });

        }
    }
}
