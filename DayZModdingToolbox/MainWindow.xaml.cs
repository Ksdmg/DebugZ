using System.Threading.Tasks;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DayZModdingToolbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainWindowViewModel();
            this.DataContext = vm;
        }

        public async Task<string> GetUserInput(string title, string message)
        {
            return await this.ShowInputAsync(title, message);
        }

        public async Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings? settings = null)
        {
            return await this.ShowMessageAsync(title, message);
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            this.HamburgerMenuControl.Content = e.InvokedItem;
        }
    }
}