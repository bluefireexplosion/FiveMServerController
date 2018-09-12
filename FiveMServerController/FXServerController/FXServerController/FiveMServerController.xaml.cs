using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;

namespace FXServerController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static System.Windows.Forms.Timer restartTimer = new System.Windows.Forms.Timer();
        private static int interval;
        private static int[] serverErrors = { 0, 0, 0 };
        private static DateTime rTime;

        public MainWindow()
        {
            InitializeComponent();
            string[] paths;
            if (File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "defaults.lol")))
            {
                paths = ReadFromBinaryFile<string[]>(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "defaults.lol"));
                if (paths.Length < 11)
                {
                    paths = new string[] { "C:\\", "C:\\", "C:\\", "false", "true", "false", "false", "false", "false", "4.0", "Restarting Server!" };
                    WriteToBinaryFile<string[]>(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "defaults.lol"), paths, false);
                }
            }
            else
            {
                paths = new string[] { "C:\\", "C:\\", "C:\\", "false", "true", "false", "false", "false", "false", "4.0", "Restarting Server!" };
                WriteToBinaryFile<string[]>(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "defaults.lol"), paths, false);
            }
            //Release
            Debugger.Visibility = Visibility.Hidden;
            Debugger2.Visibility = Visibility.Hidden;
            Server1.Text = paths[0];
            Server2.Text = paths[1];
            Server3.Text = paths[2];
            ClearCache.IsChecked = bool.Parse(paths[4]);
            EnableServer1.IsChecked = bool.Parse(paths[5]);
            EnableServer2.IsChecked = bool.Parse(paths[6]);
            EnableServer3.IsChecked = bool.Parse(paths[7]);
            AutoRestart.IsChecked = bool.Parse(paths[8]);
            Slider.Value = double.Parse(paths[9]);
            RestartMessage.Text = paths[10];
            Slider.Minimum = 1.0;
            Slider.Maximum = 24.0;
            SetupTimers();
            Error.Visibility = Visibility.Hidden;
            Scroller.Visibility = Visibility.Hidden;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateDefaults();
        }
        private void StartServer(string path)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                string[] files = System.IO.Directory.GetFiles(path, "*.bat", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    if (file == null || file.Length < 1 || file.Equals(""))
                    {
                        return;
                    }
                    else
                    {
                        startInfo.Arguments = "/C cd " + path + " && start " + file;
                        process.StartInfo = startInfo;
                        process.Start();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        private void RestartServers()
        {
            try
            {
                KillWithWindowTitle("bat", "cmd");
            }
            catch (Exception e)
            {

            }
        }
        private void RestartServer(string path)
        {
            try
            {
                KillProcessesNamedAtPath("cmd", path);
                StartServer(path);
            }
            catch (Exception e)
            { }
        }
        private void test()
        {
            //check if we're running
            SliderVal.Content = "We'Re rUnIng";

        }
        private bool DoesProcessExist(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                //processes[0].CloseMainWindow();
                return true;
            }
            else
            {
                return false;
            }
        }
        private void KillWithWindowTitle(string title, string name)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(name);
                String Title = String.Empty;
                foreach (Process p in processes)
                {
                    Title = p.MainWindowTitle;
                    if (Title.Contains(title))
                    {
                        p.CloseMainWindow();
                        p.Dispose();
                        p.Close();
                        p.Kill();
                    }
                }
                /*Process[] processes2 = Process.GetProcessesByName(name);
                if (processes2.Length >= 1)
                {
                    processes2[0].Kill();
                }*/
            }
            catch (Exception e)
            {

            }
        }
        private void KillProcessesNamedAtPath(string name, string path)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(name);
                String Title = String.Empty;
                string[] files = System.IO.Directory.GetFiles(path, "*.bat", SearchOption.TopDirectoryOnly);
                foreach (Process p in processes)
                {
                    Title = p.MainWindowTitle;
                    foreach (string file in files)
                    {
                        Debugger2.Content = file;
                        Debugger.Content = Title + (Title == file);
                        if (Title.Contains(file))
                        {
                            p.CloseMainWindow();
                            p.Dispose();
                            p.Close();
                            p.Kill();
                        }
                    }
                }
                /*Process[] processes2 = Process.GetProcessesByName(name);
                if (processes2.Length >= 1)
                {
                    processes2[0].Kill();
                }*/
            }
            catch (Exception e)
            {

            }

            /*Process[] processes = Process.GetProcessesByName(name);
            if (processes.Length >= 1)
            {
                foreach (Process p in processes)
                {
                    if (p.ProcessName.Contains("FXServerController"))
                    {
                        return;
                    }
                    else
                    {
                        try
                        {
                            if (p.MainModule.FileName == path)
                            {
                                p.Kill();
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }*/
        }
        private void UpdateDefaults()
        {
            string[] paths = { Server1.Text, Server2.Text, Server3.Text, "true", ClearCache.IsChecked.ToString(), EnableServer1.IsChecked.ToString(), EnableServer2.IsChecked.ToString(), EnableServer3.IsChecked.ToString(), AutoRestart.IsChecked.ToString(), Slider.Value.ToString(), RestartMessage.Text };
            WriteToBinaryFile<string[]>(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "defaults.lol"), paths, false);
        }
        private void SetupTimers()
        {
            //restartTimer = new Timer(newInterval * 60 * 60 * 1000);
            restartTimer = new System.Windows.Forms.Timer
            {
                Interval = 100
            };
            restartTimer.Tick += OnTimedEvent;
            restartTimer.Enabled = true;
        }
        private void CheckErrors()
        {
            Error.Text = "";
            Error.Text += CheckServerFilepath(Server1.Text, 1);
            Error.Text += CheckServerFilepath(Server2.Text, 2);
            Error.Text += CheckServerFilepath(Server3.Text, 3);
            if (Error.Text.Length > 1)
            {
                Scroller.Visibility = Visibility.Visible;
                Error.Visibility = Visibility.Visible;
            }
            else
            {
                Scroller.Visibility = Visibility.Hidden;
                Error.Visibility = Visibility.Hidden;
            }
        }
        private void OnTimedEvent(Object source, EventArgs e)
        {
            CheckErrors();
            string path = Server1.Text;
            string path2 = Server2.Text;
            string path3 = Server3.Text;
            CurrTime.Content = DateTime.Now.ToString();
            if (AutoRestart.IsChecked == true)
            {
                HoursRem.Content = rTime.ToString();
                if (rTime == DateTime.Now)
                {
                    if (EnableServer1.IsChecked == true)
                    {
                        RestartServer(path);
                    }
                    if (EnableServer2.IsChecked == true)
                    {
                        RestartServer(path2);
                    }
                    if (EnableServer3.IsChecked == true)
                    {
                        RestartServer(path3);
                    }
                    rTime = DateTime.Now.AddHours((int)Slider.Value);
                }
            }
            else if (AutoRestart.IsChecked == false)
            {
                rTime = DateTime.Now.AddHours((int)Slider.Value);
                HoursRem.Content = "Disabled";
            }
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            double value = slider.Value;
            interval = (int)value;
            SliderVal.Content = interval + " hrs";
            rTime = DateTime.Now.AddHours((int)Slider.Value);
        }
        private string CheckServerFilepath(string path, int server)
        {
            string errors = String.Empty;
            int count = 0;
            if (path.Length <= 1)
            {
                count++;
                errors += String.Format("Error {1}:{0}: The server path for Server {1} is null.\n", count, server);
            }
            else if (!Directory.Exists(path))
            {
                count++;
                errors += String.Format("Error {1}:{0}: The server directory for Server {1} doesn't exist.\n", count, server);
            }
            else if (path.Contains(" "))
            {
                count++;
                errors += String.Format("Error {1}:{0}: The server path for server {1} contains a space.\n", count, server);
            }
            else if (System.IO.Directory.GetFiles(path, "*.bat", SearchOption.TopDirectoryOnly).Length < 1)
            {
                count++;
                errors += String.Format("Error {1}:{0}: The server folder for server {1} does not contain any .bat files. These are needed to execute the server.\n", count, server);
            }
            if (count > 0)
            {
                errors += String.Format("Unable to Start Server {0}: {1} Error(s) occurred when attempting to start.\n", server, count);
            }
            serverErrors[server - 1] = count;
            return errors;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            if (s.Name == "StartServer1" && serverErrors[0] < 1 && EnableServer1.IsChecked == true)
            {
                StartServer(Server1.Text);
            }
            else if (s.Name == "StartServer2" && serverErrors[1] < 1 && EnableServer2.IsChecked == true)
            {
                StartServer(Server2.Text);
            }
            else if (s.Name == "StartServer3" && serverErrors[2] < 1 && EnableServer3.IsChecked == true)
            {
                StartServer(Server3.Text);
            }
            else if (s.Content.ToString() == "Restart Server 1" && serverErrors[0] < 1 && EnableServer1.IsChecked == true)
            {
                string path = Server1.Text;
                if (ClearCache.IsChecked == true)
                {
                    if (Directory.Exists(path + "\\cache"))
                    {
                        Directory.Delete(path + "\\cache", true);
                    }
                    else if (Directory.Exists(path + "\\cfx-server-data-master\\cache"))
                    {
                        Directory.Delete(path + "\\cfx-server-data-master\\cache", true);
                    }
                    else if (Directory.Exists(path + "\\cfx-server-data\\cache"))
                    {
                        Directory.Delete(path + "\\cfx-server-data\\cache", true);
                    }
                }
                if (UseRestartMessage.IsChecked == true)
                {
                    //display the restart message
                    //Need to figure out how to do this
                }
                RestartServer(path);
            }
            else if (s.Content.ToString() == "Restart Server 2" && serverErrors[1] < 1 && EnableServer2.IsChecked == true)
            {
                string path = Server2.Text;
                if (ClearCache.IsChecked == true)
                {
                    if (Directory.Exists(path + "\\cache"))
                    {
                        Directory.Delete(path + "\\cache", true);
                    }
                    else if (Directory.Exists(path + "\\cfx-server-data-master\\cache"))
                    {
                        Directory.Delete(path + "\\cfx-server-data-master\\cache", true);
                    }
                    else if (Directory.Exists(path + "\\cfx-server-data\\cache"))
                    {
                        Directory.Delete(path + "\\cfx-server-data\\cache", true);
                    }
                }
                RestartServer(path);
            }
            else if (s.Content.ToString() == "Restart Server 3" && serverErrors[2] < 1 && EnableServer3.IsChecked == true)
            {
                string path = Server3.Text;
                if (ClearCache.IsChecked == true)
                {
                    if (Directory.Exists(path + "\\cache"))
                    {
                        Directory.Delete(path + "\\cache", true);
                    }
                    else if (Directory.Exists(path + "\\cfx-server-data-master\\cache"))
                    {
                        Directory.Delete(path + "\\cfx-server-data-master\\cache", true);
                    }
                    else if (Directory.Exists(path + "\\cfx-server-data\\cache"))
                    {
                        Directory.Delete(path + "\\cfx-server-data\\cache", true);
                    }
                }
                RestartServer(path);
            }
            else if (s.Name == "KillServer1" && serverErrors[0] < 1 && EnableServer1.IsChecked == true)
            {
                string path = Server1.Text;
                KillProcessesNamedAtPath("cmd", path);
            }
            else if (s.Name == "KillServer2" && serverErrors[1] < 1 && EnableServer2.IsChecked == true)
            {
                string path = Server2.Text;
                KillProcessesNamedAtPath("cmd", path);
            }
            else if (s.Name == "KillServer3" && serverErrors[2] < 1 && EnableServer3.IsChecked == true)
            {
                string path = Server3.Text;
                KillProcessesNamedAtPath("cmd", path);
            }
            else if (s.Name == "BrowseServer1")
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (dialog.SelectedPath.Length < 2)
                    {
                        Server1.Text = Server1.Text;
                    }
                    else
                    {
                        Server1.Text = dialog.SelectedPath;
                    }
                }
            }
            else if (s.Name == "BrowseServer2")
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (dialog.SelectedPath.Length < 2)
                    {
                        Server2.Text = Server2.Text;
                    }
                    else
                    {
                        Server2.Text = dialog.SelectedPath;
                    }
                }
            }
            else if (s.Name == "BrowseServer3")
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (dialog.SelectedPath.Length < 2)
                    {
                        Server3.Text = Server3.Text;
                    }
                    else
                    {
                        Server3.Text = dialog.SelectedPath;
                    }
                }
            }
            else if (s.Name == "CopyRestartMSG")
            {
                Clipboard.SetText(RestartMessage.Text);
            }
        }
        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
