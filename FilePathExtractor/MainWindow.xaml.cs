using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace FilePathExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "eM-Planner data (*.xml)|*.xml",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                InputPath.Text = dialog.FileName;
                ProgressBar.Value = 0;
                StartButton.IsEnabled = true;
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text Documents (*.txt)|*.txt",
                OverwritePrompt = true
            };


            if (dialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(dialog.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    StartButton.IsEnabled = false;
                    BrowseButton.IsEnabled = false;

                    try
                    {
                        await Utils.Extract(InputPath.Text, fileStream, new Progress<(long max, long value)>(p =>
                        {
                            ProgressBar.Value = (double)p.value / p.max;
                        }));

                        ErrorMessage.Text = "Done";
                    }

                    catch (Exception exception)
                    {
                        ErrorMessage.Text = exception.Message;
                    }

                    finally
                    {
                        fileStream.SetLength(fileStream.Position);

                        StartButton.IsEnabled = true;
                        BrowseButton.IsEnabled = true;
                    }
                }
            }
        }
    }
}
