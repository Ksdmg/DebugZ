using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DayZModdingToolbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static MainWindow instance;

        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainWindowViewModel();
            this.DataContext = vm;
            instance = this;
        }

        public static async Task<string> GetUserInput(string title, string message)
        {
            return await instance.ShowInputAsync(title, message);
        }

        public static async Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await instance.ShowMessageAsync(title, message);
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            this.HamburgerMenuControl.Content = e.InvokedItem;
        }
    }
}