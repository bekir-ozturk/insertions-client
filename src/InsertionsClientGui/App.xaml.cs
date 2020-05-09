using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace Microsoft.Net.Insertions.Clients
{
    public partial class App : Application
    {
        internal static void OpenDirectory(Action<string> openAction)
        {
            _ = openAction ?? throw new ArgumentNullException(nameof(openAction));

            using System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = Directory.GetCurrentDirectory();
            dialog.Description = "Open directory...";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openAction(dialog.SelectedPath);
            }
        }

        internal static void OpenFile(Action<string> openAction, string filter)
        {
            _ = openAction ?? throw new ArgumentNullException(nameof(openAction));

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = filter,
                InitialDirectory = Directory.GetCurrentDirectory(),
                Multiselect = false,
                Title = "Open File..."
            };

            if (dialog.ShowDialog() == true)
            {
                openAction(dialog.FileName);
            }
        }

        internal static void SaveFile(Action<string> saveAction, string filter)
        {
            _ = saveAction ?? throw new ArgumentNullException(nameof(saveAction));

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = filter,
                InitialDirectory = Directory.GetCurrentDirectory(),
                Title = "Save file"
            };

            if (dialog.ShowDialog() == true)
            {
                saveAction(dialog.FileName);
            }
        }
    }
}
