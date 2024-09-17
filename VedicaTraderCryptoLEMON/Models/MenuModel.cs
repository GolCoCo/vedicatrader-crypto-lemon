
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace VedicaTraderCryptoLEMON
{
    /// <summary>
    /// Class represents the command model.
    /// </summary>
    public class MenuModel : NotificationObject
    {
        private string name;

        private object icon;

        private int width;

        private KeyGesture hotkey;

        private ObservableCollection<MenuModel> commands;

        private ICommand command;

        private DataTemplate imageTemplate;

        public DataTemplate ImageTemplate
        {
            get { return imageTemplate; }
            set { imageTemplate = value; RaisePropertyChanged("ImageTemplate"); }
        }

        public MenuModel()
        {
            MenuCollection = new ObservableCollection<MenuModel>();
        }

        public KeyGesture HotKey
        {
            get
            {
                return hotkey;
            }
            set
            {
                hotkey = value;
                RaisePropertyChanged("HotKey");
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                RaisePropertyChanged("Width");
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        public object Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
                RaisePropertyChanged("Icon");
            }
        }
        public ObservableCollection<MenuModel> MenuCollection
        {
            get
            {
                return commands;
            }
            set
            {
                commands = value;
                RaisePropertyChanged("MenuCollection");
            }
        }
        public ICommand Command
        {
            get
            {
                return command;
            }
            set
            {
                command = value;
            }
        }
    }
}
