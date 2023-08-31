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
        public MainWindow()
        {
            InitializeComponent();
        }
        #region UI stuff
        // Allows to move the main window around while holding down left mouse button, yoinked from yt comments
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
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

        public static bool validPath;
        public static string GamePath;
        public static string rawPath = $"{"C:\\Users\\"}{Environment.UserName}{"\\Documents\\Forza Model Tool\\"}";

        //click on path button > select game path, if wrong throws error message, if correct move on
        public void BTN_Path_Click(object sender, RoutedEventArgs e)
        {
            //Pop-up to select Folder, yoinked from YT
            FolderBrowserDialog dialog = new();
            dialog.ShowDialog();
            GamePath = dialog.SelectedPath;

            //if game path includes ForzaHorizon5.exe or not
            if (File.Exists(GamePath + "\\ForzaHorizon5.exe"))
            {
                validPath = true;
                TXT_GamePath.Text = new FileInfo(GamePath).FullName;
                TXT_NoPath.Visibility = Visibility.Hidden;
                BTN_Path.IsEnabled = false;
            }
            else
            {
                System.Windows.MessageBox.Show("Path is wrong or not selected.", "Error", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Error);
                validPath = false;
                TXT_NoPath.Visibility = Visibility.Visible;
            }
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
