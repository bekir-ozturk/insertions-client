// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Net.Insertions.Api;
using Microsoft.Net.Insertions.Api.Providers;
using Microsoft.Net.Insertions.Clients.Images;
using Microsoft.Net.Insertions.Clients.Logging;
using Microsoft.Net.Insertions.Clients.Models;
using Microsoft.Net.Insertions.Common.Constants;
using Microsoft.Net.Insertions.Common.Json;
using Microsoft.Net.Insertions.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Microsoft.Net.Insertions.Clients
{
    // TODO: intersect log messages and display in GUI

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Images/AppIcon.png")).Stream);

            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            LogFile = Path.Combine(logDirectory, $"log_{DateTime.Now.Ticks}.txt");
            Trace.AutoFlush = true;
            _ = Trace.Listeners.Add(new TextBoxTextWriterTraceListener(LogFile, "tracelistener", tbxOutput));

            Height = SystemParameters.PrimaryScreenHeight * 0.8;

            bttnStart.InitializeLookAndFeel(ImageOption.Start, "start version updates");
            bttnHelp.InitializeLookAndFeel(ImageOption.Help, "help", true);
            bttnSaveInput.InitializeLookAndFeel(ImageOption.Save, "save input for reuse in future sessions", true);
            bttnLoadInputFile.InitializeLookAndFeel(ImageOption.LoadInputFile, "Load input from file", true);

            partNugetPat.chbx.Content = "OPTIONAL: NuGet PAT";
            partNugetPat.partInput.bttnFindFile.Visibility = Visibility.Hidden;

            partPropsDirectory.chbx.Content = "OPTIONAL: .props Directory";
            partPropsDirectory.partInput.bttnFindDirectory.Visibility = Visibility.Visible;
            partPropsDirectory.partInput.bttnFindFile.Visibility = Visibility.Hidden;
            partPropsDirectory.partInput.OpenAction = x => partPropsDirectory.partInput.tbx.Text = x;

            partManifestJson.Filter = "Json files (*.json)|*.json";
            partManifestJson.OpenAction = x => partManifestJson.tbx.Text = x;


            partIgnoreFileInput.Filter = "Ignore ASM files (*.txt)|*.txt";
            partIgnoreFileInput.OpenAction = x => partIgnoreFileInput.tbx.Text = x;

            partDefaultConfig.Filter = "Default.config files (*.xml)|*.xml";
            partDefaultConfig.OpenAction = x => partDefaultConfig.tbx.Text = x;

            Trace.WriteLine("Ready");
        }


        internal static TaskScheduler AppScheduler { get; } = TaskScheduler.FromCurrentSynchronizationContext();


        private string LogFile { get; }


        private static bool ValidateFile(TextBox tbx, string label)
        {
            if (!File.Exists(tbx?.Text))
            {
                _ = MessageBox.Show($"\"{label}\" file \"{tbx?.Text ?? string.Empty}\" does not exist.",
                    "warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private static bool ValidateDirectory(TextBox tbx, string label)
        {
            if (!Directory.Exists(tbx?.Text))
            {
                _ = MessageBox.Show($"\"{label}\" directory \"{tbx?.Text ?? string.Empty}\" does not exist.",
                    "warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private static string ValidateTextBoxValue(TextBox tbx, string itemName)
        {
            return string.IsNullOrWhiteSpace(tbx?.Text) ? $"{itemName} not set." : string.Empty;
        }

        private bool CheckVersionUpdateReadiness()
        {
            foreach (var item in new[]
            {
                new { Tbx = partDefaultConfig.tbx, Label = InsertionConstants.DefaultConfigFile},
                new { Tbx = partManifestJson.tbx, Label = InsertionConstants.ManifestFile},
            })
            {
                if (!ValidateFile(item.Tbx, item.Label))
                {
                    return false;
                }
            }

            if (chbxIgnoreOption.IsChecked == true)
            {
                if (rbttnIgoreFile.IsChecked == true)
                {
                    if (!ValidateFile(partIgnoreFileInput.tbx, "ignore assemblies list"))
                    {
                        return false;
                    }
                }
            }

            if (partNugetPat.chbx.IsChecked == true)
            {
                string error = ValidateTextBoxValue(partNugetPat.partInput.tbx, "NuGetDownload PAT");
                if (error.Length > 0)
                {
                    _ = MessageBox.Show($"\"NuGetDownload PAT\" cannot be null.",
                        "warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }
            }

            if (partPropsDirectory.chbx.IsChecked == true)
            {
                if (!ValidateDirectory(partPropsDirectory.partInput.tbx, ".props directory"))
                {
                    return false;
                }
            }

            return true;
        }

        private string ValidateAndGetValidInteger(TextBox tbx, int minValue, int maxValue, string itemName, out int value)
        {
            value = int.MinValue;
            string error = ValidateTextBoxValue(tbx, itemName);
            if (error.Length > 0)
            {
                return error;
            }

            if (!int.TryParse(tbx.Text, out value)
                || value > maxValue || value < minValue)
            {
                value = minValue;
            }

            return string.Empty;
        }

        private ToolInput DeserializeToolInputFomUI()
        {
            return new ToolInput
            {
                DefaultConfig = partDefaultConfig.tbx.Text,
                ManifestJson = partManifestJson.tbx.Text,
                IgnoreBlackListAssemblies = chbxIgnoreOption.IsChecked == true & rbbtnDefaultAsms.IsChecked == true,
                IgnoreFile = new OptionalInput
                {
                    IsChosen = chbxIgnoreOption.IsChecked == true & rbttnIgoreFile.IsChecked == true,
                    Value = partIgnoreFileInput.tbx.Text
                },
                NuGetPat = new OptionalInput
                {
                    IsChosen = partNugetPat.chbx.IsChecked == true,
                    Value = partNugetPat.partInput.tbx.Text
                },
                PropsDirectory = new OptionalInput
                {
                    IsChosen = partNugetPat.chbx.IsChecked == true,
                    Value = partNugetPat.partInput.tbx.Text
                }
            };
        }

        private void SerializeToUI(ToolInput input)
        {
            partDefaultConfig.tbx.Text = input.DefaultConfig;
            partManifestJson.tbx.Text = input.ManifestJson;
            chbxIgnoreOption.IsChecked = input.IgnoreBlackListAssemblies || input.IgnoreFile.IsChosen;
            partIgnoreFileInput.tbx.Text = input.IgnoreFile.Value;
            partNugetPat.chbx.IsChecked = input.NuGetPat.IsChosen;
            partNugetPat.partInput.tbx.Text = input.NuGetPat.Value;
            partPropsDirectory.chbx.IsChecked = input.PropsDirectory.IsChosen;
            partPropsDirectory.partInput.tbx.Text = input.PropsDirectory.Value;
        }

        private bool TryGetApiSettings(out int maxDuration, out int maxDownloadTime, out int maxConcurrency)
        {
            string maxDurationError = ValidateAndGetValidInteger(tbxMaxDuration, 50, 200, "max udpate processing duration", out maxDuration);
            string maxDownloadTimeError = ValidateAndGetValidInteger(tbxMaxNugetDownloadTime, 200, 500, "max nuget download time", out maxDownloadTime);
            string maxConcurrencyError = ValidateAndGetValidInteger(tbxMaxConcurrency, 5, 20, "max concurrency", out maxConcurrency);

            foreach (string error in new string[]
            {
                maxDurationError,
                maxDownloadTimeError,
                maxConcurrencyError
            })
            {
                if (!string.IsNullOrWhiteSpace(error))
                {
                    _ = MessageBox.Show(error, "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            return true;
        }

        private UpdateResults UpdatePackageVersions(ToolInput input, int maxDuration, int maxDownloadTime, int maxConcurrency)
        {
            IInsertionApiFactory apiFactory = new InsertionApiFactory();
            IInsertionApi api = apiFactory.Create(TimeSpan.FromSeconds(maxDuration), TimeSpan.FromSeconds(maxDownloadTime), maxConcurrency);

            UpdateResults results;
            if (!input.IgnoreBlackListAssemblies && !input.IgnoreFile.IsChosen)
            {
                results = api.UpdateVersions(input.ManifestJson,
                    input.DefaultConfig,
                    (HashSet<string>?)null,
                    input.NuGetPat.IsChosen ? input.NuGetPat.Value : null,
                    input.PropsDirectory.IsChosen ? input.PropsDirectory.Value : null);
            }
            else if (input.IgnoreBlackListAssemblies)
            {
                results = api.UpdateVersions(input.ManifestJson,
                    input.DefaultConfig,
                    InsertionConstants.DefaultDevUxTeamPackages,
                    input.NuGetPat.IsChosen ? input.NuGetPat.Value : null,
                    input.PropsDirectory.IsChosen ? input.PropsDirectory.Value : null);
            }
            else
            {
                results = api.UpdateVersions(input.ManifestJson,
                    input.DefaultConfig,
                    input.IgnoreFile.Value,
                    input.NuGetPat.IsChosen ? input.NuGetPat.Value : null,
                    input.PropsDirectory.IsChosen ? input.PropsDirectory.Value : null);
            }
            return results;
        }

        private void LaunchProcess(string processParameter)
        {
            using Process process = new Process()
            {
                StartInfo = new ProcessStartInfo(processParameter)
                {
                    UseShellExecute = true
                }
            };
            _ = process.Start();
        }


        private void HandlerLoadInputFile(object sender, RoutedEventArgs e)
        {
            try
            {
                App.OpenFile(x =>
                {
                    SerializeToUI(Serializer.Deserialize<ToolInput>(File.ReadAllText(x)));
                    FileInfo fileInfo = new FileInfo(x);
                    tbkStatusMessage.Text = $"Loaded input from file {fileInfo.Name}";
                }, "Json files (*.json)|*.json");
            }
            catch (Exception ee)
            {
                _ = MessageBox.Show($"Problems loading file{Environment.NewLine}{ee}", "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void HandlerHelp(object sender, RoutedEventArgs e)
        {
            LaunchProcess("https://github.com/bozturkMSFT/insertions-client/tree/master/docs");
        }

        private void HandlerStart(object sender, RoutedEventArgs e)
        {
            if (!TryGetApiSettings(out int maxDuration, out int maxDownloadTime, out int maxConcurrency)
                || !CheckVersionUpdateReadiness())
            {
                return;
            }

            ToolInput input = DeserializeToolInputFomUI();
            UpdateResults results = UpdatePackageVersions(input, maxDuration, maxDownloadTime, maxConcurrency);
        }

        private void HandlerSaveInput(object sender, RoutedEventArgs e)
        {
            App.SaveFile(x =>
            {
                ToolInput input = DeserializeToolInputFomUI();
                string json = Serializer.Serialize(input);
                File.WriteAllText(x, json);
            }, "Json files (*.json)|*.json");
        }

        private void HandlerIgoreOptionChecked(object sender, RoutedEventArgs e)
        {
            gridignore.IsEnabled = chbxIgnoreOption.IsChecked == true;
        }

        private void HandlerShowLog(object sender, RoutedEventArgs e)
        {
            LaunchProcess(LogFile);
        }
    }
}
