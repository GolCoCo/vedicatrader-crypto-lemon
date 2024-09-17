using FastMember;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Newtonsoft.Json.Linq;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using VedicaTraderCryptoLEMON.Controllers;
using VedicaTraderCryptoLEMON.Models;
using VedicaTraderCryptoLEMON.SubViews;
using VedicaTraderCryptoLEMON.Utils;

namespace VedicaTraderCryptoLEMON
{
    public class ActivateViewModel : NotificationObject
    {
        public ActivateViewModel()
        {
        }
        private string appTille = "Activate For VedicaTrader Crypto LEMON";
        public string AppTitle
        {
            get { return appTille; }
            set
            {
                this.RaisePropertyChanged("AppTitle");
            }
        }

        /// <summary>
        /// Gets or set the title bar background
        /// </summary>
        private Brush titleBarBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));

        /// <summary>
        /// Gets or set the title bar foreground
        /// </summary>
        private Brush titleBarForeground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        /// <summary>
        /// Gets or set the title bar background
        /// </summary>
        public Brush TitleBarBackground
        {
            get { return titleBarBackground; }
            set
            {
                titleBarBackground = value;
                this.RaisePropertyChanged("TitleBarBackground");
            }
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

    }
    public class LoginModel : NotificationObject
    {
        public LoginModel()
        {
        }
        private string appTille = "VedicaTrader Crypto LEMON";
        public string AppTitle
        {
            get { return appTille; }
            set
            {
                this.RaisePropertyChanged("AppTitle");
            }
        }

        /// <summary>
        /// Gets or set the title bar background
        /// </summary>
        private Brush titleBarBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));

        /// <summary>
        /// Gets or set the title bar foreground
        /// </summary>
        private Brush titleBarForeground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        /// <summary>
        /// Gets or set the title bar background
        /// </summary>
        public Brush TitleBarBackground
        {
            get { return titleBarBackground; }
            set
            {
                titleBarBackground = value;
                this.RaisePropertyChanged("TitleBarBackground");
            }
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

    }
}
