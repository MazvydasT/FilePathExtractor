using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FilePathExtractor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var args = Environment.GetCommandLineArgs().Skip(1).Select(v => v.Trim()).ToArray();

            //args = new[] { @"C:\Users\mtadara1.JLRIEU1\Desktop\output.xml" };

            if (args.Where(v =>
            {
                v = v.ToLower();

                return v == "-h" || v == "-?" || v == "-help";
            }).Count() > 0)
            {
                Console.Error.WriteLine("Usage:");
                Console.Error.WriteLine($"        To extract file paths: {Process.GetCurrentProcess().ProcessName} inputFilePath.xml");
                Console.Error.WriteLine($"To extract file paths to file: {Process.GetCurrentProcess().ProcessName} inputFilePath.xml >> filePath.txt");
                Console.Error.WriteLine($"    To start GUI double click  {Process.GetCurrentProcess().ProcessName}");
                Console.Error.WriteLine($"         To show this message: {Process.GetCurrentProcess().ProcessName} -? OR -h OR -help");
                Application.Current.Shutdown();
            }

            else if (args.Length != 1)
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, 0); // To hide

                new MainWindow().Show();
            }

            else
            {
                Console.OutputEncoding = new UTF8Encoding(false);

                try
                {
                    using (var output = Console.OpenStandardOutput())
                    {
                        await Utils.Extract(args[0], output);
                    }
                }

                catch(Exception exception)
                {
                    Console.Error.WriteLine(exception.Message);
                }

                Application.Current.Shutdown();
            }
        }
    }
}
