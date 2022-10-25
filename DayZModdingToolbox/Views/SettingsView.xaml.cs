using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using DayZModdingToolbox.ViewModels;

namespace DayZModdingToolbox.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ModGrid_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);

            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Parsing dropped files
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                for (int i = 0; i < files.Length ; i++)
                {
                    ((SettingsViewModel)this.DataContext).Mods.Add(new() { ModPath = files[i] });
                }
            }
            e.Handled = true;
        }

        private void ModGrid_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}