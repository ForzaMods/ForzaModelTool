using System;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace ForzaModelTool.Views
{
    public partial class WheelSwapView : UserControl
    {
        public WheelSwapView()
        {
            InitializeComponent();
        }

        string GamePath;
        string[] Cars;
        string[] Wheels;
        string GamePathMediaC = "\\media\\cars";
        string GamePathMediaT = "\\media\\cars\\_library";
        string GamePathMediaW = "\\media\\cars\\_library\\scene\\wheels";
        readonly string rawPath = MainWindow.rawPath;

        #region Clickable Things
        public void SelectableOn()
        {
            LST_DonorWheel.IsEnabled = true;
            LST_TargetWheel.IsEnabled = true;
            LST_WheelVar.IsEnabled = true;
            BTN_Swapper.IsEnabled = true;
        }

        public void SelectableOff()
        {
            LST_DonorWheel.IsEnabled = false;
            LST_TargetWheel.IsEnabled = false;
            LST_WheelVar.IsEnabled = false;
            BTN_Swapper.IsEnabled = false;
        }
        #endregion

        #region Lists + Checkbox
        private void LST_DonorWheel_Opened(object sender, EventArgs e)
        {
            if (MainWindow.validPath)
            {
                SelectableOn();
                CarList();
            }
        }

        private void LST_DonorWheel_SelectionChanged(object sender, EventArgs e)
        {
            if (LST_DonorWheel.SelectedItem != null)
            {
                LST_WheelVar.Items.Clear();
                WheelVariant((string)LST_DonorWheel.SelectedItem);
            }
        }

        private void CBOX_Addon_Clicked(object sender, RoutedEventArgs e)
        {
            if (CBOX_Addon.IsChecked == true)
            {
                if (LST_TargetWheel.Items != null)
                    LST_TargetWheel.Items.Clear();
                LST_TargetWheel.IsEnabled = false;
            }
            else
                LST_TargetWheel.IsEnabled = true;
        }

        private void LST_TargetWheel_Opened(object sender, EventArgs e)
        {
            if (MainWindow.validPath)
            {
                SelectableOn();
                WheelList();
            }
        }
        #endregion

        // if list empty, fill car list
        public void CarList()
        {
            GamePath = MainWindow.GamePath;
            // scrap car zips from GamePath + \media\cars
            if (LST_DonorWheel.Items.IsEmpty)
            {
                Cars = Directory.GetFiles(GamePath + GamePathMediaC);
                foreach (string car in Cars)
                    LST_DonorWheel.Items.Add(Path.GetFileNameWithoutExtension(car));
            }
        }

        // if list empty, fill wheel list (aftermarket wheels)
        public void WheelList()
        {
            GamePath = MainWindow.GamePath;
            if (LST_TargetWheel.Items.IsEmpty)
            {
                Wheels = Directory.GetFiles(GamePath + GamePathMediaW);
                foreach (string wheel in Wheels)
                    LST_TargetWheel.Items.Add(Path.GetFileNameWithoutExtension(wheel));
            }
        }

        // populate wheel variant list (some cars have wheels with different depths aka variants)
        private void WheelVariant(string car)
        {
            using (ZipArchive archive = ZipFile.OpenRead($"{GamePath}{GamePathMediaC}{"\\"}{car}{".zip"}"))
                foreach (ZipArchiveEntry entry in archive.Entries)
                    if (car == (string)LST_DonorWheel.SelectedItem)
                        if (entry.FullName.Contains("_wheel", StringComparison.OrdinalIgnoreCase) && !entry.FullName.Contains(".swatchbin"))
                            LST_WheelVar.Items.Add(entry.Name);
            LST_WheelVar.SelectedIndex = 0;
        }

        private void BTN_Swapper_Click(object sender, RoutedEventArgs e)
        {
            // if dono + target models are selected disable buttons and run Swapper()
            if (LST_DonorWheel.SelectedItem != null && LST_WheelVar.SelectedItem != null && LST_TargetWheel.SelectedItem != null)
            {
                MainWindow.FolderCheck();
                SelectableOff();
                Swapper(false);
                SelectableOn();
            }
            else if (LST_DonorWheel.SelectedItem != null && LST_WheelVar.SelectedItem != null && LST_TargetWheel.SelectedItem == null)
            {
                if (CBOX_Addon.IsChecked == true)
                {
                    MainWindow.FolderCheck();
                    SelectableOff();
                    Swapper(true);
                    SelectableOn();
                }
                else
                    MessageBox.Show("No Target Wheel selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (LST_DonorWheel.SelectedItem == null && LST_WheelVar.SelectedItem != null && LST_TargetWheel.SelectedItem != null)
                MessageBox.Show("No Car Wheel selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (LST_DonorWheel.SelectedItem != null && LST_WheelVar.SelectedItem == null && LST_TargetWheel.SelectedItem != null)
                MessageBox.Show("No Car Wheel Variant selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show("No Wheels selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Swapper(bool bChecked)
        {
            string ExportCPath = $"{rawPath}{"Wheel Swap\\"}{Path.GetFileNameWithoutExtension(LST_DonorWheel.SelectedItem.ToString())}";
            string TexturePath = "\\Textures\\AO";
            string GamePathMediaOW = GamePath + "\\media\\stripped\\mediaoverride\\rc0\\cars\\_library\\scene\\wheels";
            string[] textureZips = { "\\Textures.zip", "\\Textures_pri_44.zip", "\\Textures_pri_45.zip", "\\Textures_pri_301.zip" };
            bool foundTexture = false;
            bool textureError = false;

            // if add-on checkbox checked, create wheel zip with car name, else create wheel zip of selected car wheel renamed to target wheel
            if (bChecked)
            {
                if (!Directory.Exists(ExportCPath))
                    Directory.CreateDirectory($"{ExportCPath}{"\\"}{Path.GetDirectoryName(LST_DonorWheel.SelectedItem.ToString())}{TexturePath}");

                using (ZipArchive archive = ZipFile.OpenRead($"{GamePath}{GamePathMediaC}{"\\"}{LST_DonorWheel.SelectedItem}{".zip"}"))
                    foreach (ZipArchiveEntry entry in archive.Entries)
                        if (entry.FullName.Contains(LST_WheelVar.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase))
                            archive.GetEntry(entry.FullName).ExtractToFile($"{ExportCPath}{"\\"}{LST_DonorWheel.SelectedItem}{"_wheelLF.modelbin"}", true);

                while (foundTexture == false && textureError == false)
                {
                    using (ZipArchive archive = ZipFile.OpenRead($"{GamePath}{GamePathMediaC}{"\\"}{LST_DonorWheel.SelectedItem}{".zip"}"))
                        foreach (ZipArchiveEntry entry in archive.Entries)
                            if (entry.FullName.Contains("_wheellf_ao", StringComparison.OrdinalIgnoreCase))
                            {
                                archive.GetEntry(entry.FullName).ExtractToFile($"{ExportCPath}{"\\"}{TexturePath}{"\\"}{entry.Name}", true);
                                foundTexture = true;
                                break;
                            }

                    foreach (string zip in textureZips)
                    {
                        if (File.Exists(GamePath + GamePathMediaT + zip))
                        {
                            using (ZipArchive archive = ZipFile.OpenRead($"{GamePath}{GamePathMediaT}{zip}"))
                            {
                                foreach (ZipArchiveEntry entry in archive.Entries)
                                    if (entry.FullName.Contains(LST_DonorWheel.SelectedItem.ToString() + "_wheellf_ao", StringComparison.OrdinalIgnoreCase))
                                    {
                                        archive.GetEntry(entry.FullName).ExtractToFile($"{ExportCPath}{"\\"}{TexturePath}{"\\"}{entry.Name}", true);
                                        foundTexture = true;
                                        break;
                                    }
                            }
                        }
                    }
                    if (!foundTexture)
                    {
                        MessageBox.Show("Texture was not found.", "Missing Texture", MessageBoxButton.OK, MessageBoxImage.Error);
                        textureError = true;
                    }
                }

                // if texture file found, create + move zip to \media\Stripped\MediaOverride\RC0\Cars\_library\scene\wheels
                if (foundTexture)
                {
                    if (!Directory.Exists(GamePathMediaOW))
                        Directory.CreateDirectory(GamePathMediaOW);

                    if (!File.Exists($"{ExportCPath}{".zip"}"))
                    {
                        ZipFile.CreateFromDirectory(ExportCPath, $"{GamePathMediaOW}{"\\"}{LST_DonorWheel.SelectedItem}{".zip"}");
                        MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_DonorWheel.SelectedItem.ToString())}{".zip"}" + " was successfully created!", ":)");
                    }
                    else
                    {
                        //throws "error" popup that the target car zip already exists in that directory, user can overwrite said zip by pressing yes button
                        MessageBoxResult result;
                        result = MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_DonorWheel.SelectedItem.ToString())}{".zip"}" + " already exists. Do you want to overwrite it?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            //move + overwrite created target zip to '\media\Stripped\MediaOverride\RC0\Cars\_library\scene\wheels'
                            File.Move($"{ExportCPath}{".zip"}", $"{GamePathMediaOW}{"\\"}{LST_DonorWheel.SelectedItem}{".zip"}", true);
                            MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_DonorWheel.SelectedItem.ToString())}{".zip"}" + " was successfully created!", ":)");
                        }
                        //if no, target car zip inside export folder gets deleted
                        else
                        {
                            File.Delete($"{ExportCPath}{".zip"}");
                            MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_DonorWheel.SelectedItem.ToString())}{".zip"}" + " was not created.", ":(");
                        }
                    }
                }
                // delete leftover folders/files
                if (Directory.Exists(ExportCPath))
                    Directory.Delete(ExportCPath, true);
            }
            else
            {
                string ExportWPath = $"{rawPath}{"Wheel Swap\\"}{Path.GetFileNameWithoutExtension(LST_TargetWheel.SelectedItem.ToString())}";

                if (!Directory.Exists(ExportWPath))
                    Directory.CreateDirectory($"{ExportWPath}{"\\"}{Path.GetDirectoryName(LST_TargetWheel.SelectedItem.ToString())}{TexturePath}");

                using (ZipArchive archive = ZipFile.OpenRead($"{GamePath}{GamePathMediaC}{"\\"}{LST_DonorWheel.SelectedItem}{".zip"}"))
                    foreach (ZipArchiveEntry entry in archive.Entries)
                        if (entry.FullName.Contains(LST_WheelVar.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase))
                            archive.GetEntry(entry.FullName).ExtractToFile($"{ExportWPath}{"\\"}{LST_TargetWheel.SelectedItem}{"_wheelLF.modelbin"}", true);

                while (foundTexture == false && textureError == false)
                {
                    using (ZipArchive archive = ZipFile.OpenRead($"{GamePath}{GamePathMediaC}{"\\"}{LST_DonorWheel.SelectedItem}{".zip"}"))
                        foreach (ZipArchiveEntry entry in archive.Entries)
                            if (entry.FullName.Contains("_wheellf_ao", StringComparison.OrdinalIgnoreCase))
                            {
                                archive.GetEntry(entry.FullName).ExtractToFile($"{ExportWPath}{"\\"}{TexturePath}{"\\"}{entry.Name}", true);
                                foundTexture = true;
                                break;
                            }

                    foreach (string zip in textureZips)
                    {
                        if (File.Exists(GamePath + GamePathMediaT + zip))
                        {
                            using (ZipArchive archive = ZipFile.OpenRead($"{GamePath}{GamePathMediaT}{zip}"))
                            {
                                foreach (ZipArchiveEntry entry in archive.Entries)
                                    if (entry.FullName.Contains(LST_DonorWheel.SelectedItem.ToString() + "_wheellf_ao", StringComparison.OrdinalIgnoreCase))
                                    {
                                        archive.GetEntry(entry.FullName).ExtractToFile($"{ExportWPath}{"\\"}{TexturePath}{"\\"}{LST_TargetWheel.SelectedItem}{"_wheelLF_AO.swatchbin"}", true);
                                        foundTexture = true;
                                        break;
                                    }
                            }
                        }
                    }
                    if (!foundTexture)
                    {
                        MessageBox.Show("Texture was not found.", "Missing Texture", MessageBoxButton.OK, MessageBoxImage.Error);
                        textureError = true;
                    }

                    // if texture file found, create + move zip to \media\Stripped\MediaOverride\RC0\Cars\_library\scene\wheels
                    if (foundTexture)
                    {
                        if (!Directory.Exists(GamePathMediaOW))
                            Directory.CreateDirectory(GamePathMediaOW);

                        if (!File.Exists($"{ExportWPath}{".zip"}"))
                        {
                            ZipFile.CreateFromDirectory(ExportWPath, $"{GamePathMediaOW}{"\\"}{LST_TargetWheel.SelectedItem}{".zip"}");
                            MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_TargetWheel.SelectedItem.ToString())}{".zip"}" + " was successfully created!", ":)");
                        }
                        else
                        {
                            //throws "error" popup that the target car zip already exists in that directory, user can overwrite said zip by pressing yes button
                            MessageBoxResult result;
                            result = MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_TargetWheel.SelectedItem.ToString())}{".zip"}" + " already exists. Do you want to overwrite it?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.Yes)
                            {
                                //move + overwrite created target zip to '\media\Stripped\MediaOverride\RC0\Cars\_library\scene\wheels'
                                File.Move($"{ExportWPath}{".zip"}", $"{GamePathMediaOW}{"\\"}{LST_TargetWheel.SelectedItem}{".zip"}", true);
                                MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_TargetWheel.SelectedItem.ToString())}{".zip"}" + " was successfully created!", ":)");
                            }
                            //if no, target car zip inside export folder gets deleted
                            else
                            {
                                File.Delete($"{ExportCPath}{".zip"}");
                                MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_TargetWheel.SelectedItem.ToString())}{".zip"}" + " was not created.", ":(");
                            }
                        }
                    }
                    // delete leftover folders/files
                    if (Directory.Exists(ExportCPath))
                        Directory.Delete(ExportCPath, true);
                }
            }
        }
    }
}
