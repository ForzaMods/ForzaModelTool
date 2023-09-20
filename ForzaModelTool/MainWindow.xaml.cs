using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ForzaModelTool
{
    public partial class MainWindow : Window
    {
        public static bool validPath;
        public static string GamePath;
        public static string rawPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Forza Model Tool\";
        public static string curPath;

        public MainWindow()
        {
            if (!Directory.Exists(rawPath))
                FolderCheck();

            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // check if file exists and if its content isnt null
                if (File.Exists(rawPath + @"\Path.txt") && (File.ReadAllText(rawPath + @"\Path.txt") != ""))
                {
                    // if its not null check if the path contains fh5 exe
                    if (File.Exists(File.ReadAllText(rawPath + @"\Path.txt") + @"\ForzaHorizon5.exe"))
                    {
                        TXT_GamePath.Text = File.ReadAllText(rawPath + @"\Path.txt");
                        GamePath = File.ReadAllText(rawPath + @"\Path.txt");
                        curPath = GamePath;
                        TXT_NoPath.Visibility = Visibility.Hidden;
                        validPath = true;
                    }
                }
            }
            catch { TXT_NoPath.Visibility = Visibility.Visible; }
        }
        #region UI stuff
        // Allows to move the main window around while holding down left mouse button, yoinked from yt comments
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion

        //click on path button > select game path, if wrong throws error message, if correct move on
        public void BTN_Path_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new();
            DialogResult result = dialog.ShowDialog();
            GamePath = dialog.SelectedPath;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //if game path includes ForzaHorizon5.exe or not
                if (File.Exists(GamePath + "\\ForzaHorizon5.exe"))
                {
                    validPath = true;
                    TXT_GamePath.Text = new FileInfo(GamePath).FullName;
                    TXT_NoPath.Visibility = Visibility.Hidden;

                    if (!File.Exists(rawPath + @"\Path.txt"))
                        using (File.Create(rawPath + @"\Path.txt")) { } // Prevent crash from "file in use"

                    File.WriteAllText(rawPath + @"\Path.txt", GamePath);
                }
                else
                {
                    System.Windows.MessageBox.Show("Path is wrong or not selected.", "Error", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Error);
                    validPath = false;
                    TXT_NoPath.Visibility = Visibility.Visible;
                }
            }
            else GamePath = curPath;
        }

        // view switching (from szaamerik)
        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayFrame.Source = new Uri((string)((System.Windows.Controls.Button)sender).Tag, UriKind.Relative);
        }

        // check/create base folders
        public static void FolderCheck()
        {
            Directory.CreateDirectory(System.IO.Path.Combine(rawPath));
            Directory.CreateDirectory(System.IO.Path.Combine(rawPath + "Model Swap"));
            Directory.CreateDirectory(System.IO.Path.Combine(rawPath + "Wheel Swap"));
        }
    }
}
