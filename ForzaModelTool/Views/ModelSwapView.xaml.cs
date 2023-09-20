using System;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace ForzaModelTool.Views
{
    public partial class ModelSwapView : UserControl
    {
        public ModelSwapView()
        {
            InitializeComponent();
        }

        string GamePath;
        string[] Cars;
        string GamePathMedia = "\\media\\cars";
        readonly string rawPath = MainWindow.rawPath;

        #region Clickable Things
        public void SelectableOn()
        {
            LST_DonorCar.IsEnabled = true;
            LST_DonorModel.IsEnabled = true;
            LST_TargetCar.IsEnabled = true;
            LST_TargetModel.IsEnabled = true;
            BTN_Swapper.IsEnabled = true;
            BTN_Zip.IsEnabled = true;
            BTN_CLR.IsEnabled = true;
        }

        public void SelectableOff()
        {
            LST_DonorCar.IsEnabled = false;
            LST_DonorModel.IsEnabled = false;
            LST_TargetCar.IsEnabled = false;
            LST_TargetModel.IsEnabled = false;
            BTN_Swapper.IsEnabled = false;
            BTN_Zip.IsEnabled = false;
            BTN_CLR.IsEnabled = false;
        }
        #endregion

        // if lists are empty, fill car lists
        public void CarLists()
        {
            GamePath = MainWindow.GamePath;
            // scrap car zips from GamePath + \media\cars
            if (LST_DonorCar.Items.IsEmpty)
            {
                Cars = Directory.GetFiles(GamePath + GamePathMedia);
                foreach (string car in Cars)
                {
                    // removes path + file extension so it looks cleaner
                    LST_DonorCar.Items.Add(Path.GetFileNameWithoutExtension(car));
                    LST_TargetCar.Items.Add(Path.GetFileNameWithoutExtension(car));
                }
            }

            if (MainWindow.curPath != GamePath)
            {
                LST_DonorCar.Items.Clear();
                LST_TargetCar.Items.Clear();

                MainWindow.curPath = GamePath;

                CarLists();
            }
        }

        //looks in GamePath + CarName.zip from list for .modelbin, lists all in new dropdown list
        private void ModelLists(string models)
        {
            using (ZipArchive archive = ZipFile.OpenRead($"{GamePath}{GamePathMedia}{"\\"}{models}{".zip"}"))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (models == (string)LST_DonorCar.SelectedItem)
                    {
                        if (entry.FullName.EndsWith(".modelbin")) LST_DonorModel.Items.Add(entry);
                    }
                    else if (models == (string)LST_TargetCar.SelectedItem)
                    {
                        if (entry.FullName.EndsWith(".modelbin")) LST_TargetModel.Items.Add(entry);
                    }
                }
            }
        }

        //checks if item from dropdown list is not null, clears list and runs ModelLists method to populate list again
        private void LST_DonorCar_SelectionChanged(object sender, EventArgs e)
        {
            if (LST_DonorCar.SelectedItem != null)
            {
                LST_DonorModel.Items.Clear();
                ModelLists((string)LST_DonorCar.SelectedItem);
            }
        }
        private void LST_TargetCar_SelectionChanged(object sender, EventArgs e)
        {
            if (LST_TargetCar.SelectedItem != null)
            {
                LST_TargetModel.Items.Clear();
                ModelLists((string)LST_TargetCar.SelectedItem);
            }
        }

        // call CarList() to make the combobox actually work (idfk why i have to do this all of a sudden, its so fucking retarded)
        private void LST_TargetCar_Opened(object sender, EventArgs e)
        {
            if (MainWindow.validPath)
            {
                SelectableOn();
                CarLists();
            }
        }

        private void LST_DonorCar_Opened(object sender, EventArgs e)
        {
            if (MainWindow.validPath)
            {
                SelectableOn();
                CarLists();
            }
        }

        private void BTN_Swapper_Click(object sender, RoutedEventArgs e)
        {
            // if dono + target models are selected disable buttons and run Swapper()
            if (LST_DonorModel.SelectedItem != null && LST_TargetModel.SelectedItem != null)
            {
                MainWindow.FolderCheck();
                SelectableOff();
                Swapper();
                SelectableOn();
            }
            else if (LST_DonorModel.SelectedItem == null && LST_TargetModel.SelectedItem != null)
            {
                System.Windows.MessageBox.Show("No Donator Model selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (LST_DonorModel.SelectedItem != null && LST_TargetModel.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("No Target Model selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else System.Windows.MessageBox.Show("No Models selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BTN_Zip_Click(object sender, RoutedEventArgs e)
        {
            //Import + Export strings to make things easier
            string GamePath2 = $"{GamePath}{"\\media\\Stripped\\MediaOverride\\RC0\\"}{"Cars"}";

            // 
            if (Directory.Exists($"{rawPath}{"Model Swap\\Export\\"}"))
            {
                string ExportPath = $"{rawPath}{"Model Swap\\Export\\"}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}";
                //if zip does not exist
                if (!File.Exists($"{ExportPath}{".zip"}"))
                {
                    if (!Directory.Exists(ExportPath))
                        System.Windows.MessageBox.Show($"{"The folder \""}{LST_TargetCar.SelectedItem}{"\" is missing. \nPlease swap a model first before trying to create the zip."}", "Missing Folder", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        //create zip of target model(s) with name of target car
                        ZipFile.CreateFromDirectory(ExportPath, $"{ExportPath}{".zip"}");

                        //if target car zip in '\media\Stripped\MediaOverride\RC0\Cars' does not exist
                        if (!File.Exists($"{GamePath2}{"\\"}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".zip"}"))
                        {
                            //move said zip to '\media\Stripped\MediaOverride\RC0\Cars'
                            File.Move($"{ExportPath}{".zip"}", $"{GamePath2}{"\\"}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".zip"}");
                            System.Windows.MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".zip"}" + " was successfully created!", ":)");
                        }
                        else
                        {
                            //throws "error" popup that the target car zip already exists in that directory, user can overwrite said zip by pressing yes button
                            MessageBoxResult result;
                            result = System.Windows.MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".zip"}" + " already exists. Do you want to overwrite it?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.Yes)
                            {
                                //move + overwrite created target zip to '\media\Stripped\MediaOverride\RC0\Cars'
                                File.Move($"{ExportPath}{".zip"}", $"{GamePath2}{"\\"}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".zip"}", true);
                                System.Windows.MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".zip"}" + " was successfully created!", ":)");
                            }
                            //if no, target car zip inside export folder gets deleted
                            else
                            {
                                File.Delete($"{ExportPath}{".zip"}");
                                System.Windows.MessageBox.Show($"{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".zip"}" + " was not created.", ":(");
                            }
                        }
                    }
                }
            }
            else System.Windows.MessageBox.Show("You need to swap a model first before you can create a zip!", "Missing Folder/Model", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Swapper()
        {
            //Import + Export strings to make things easier
            string ExportPath = $"{rawPath}{"Model Swap\\Export\\"}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}";
            string ImportPath = $"{rawPath}{"Model Swap\\Import\\"}{Path.GetFileNameWithoutExtension(LST_DonorCar.SelectedItem.ToString())}";

            //check if donor car folder inside 'temp\forza model tool\import' does not exist
            if (!Directory.Exists(ImportPath))
            {
                //Extract selected dono car zip to new Folder inside 'temp\forza model tool\import'
                ZipFile.ExtractToDirectory($"{GamePath}{GamePathMedia}{"\\"}{LST_DonorCar.SelectedItem}{".zip"}", ImportPath);
            }
            //check if temp car folder inside 'temp\forza model tool\export' + folder structure of target model does not exist
            if (!Directory.Exists($"{ExportPath}{"\\"}{Path.GetDirectoryName(LST_TargetModel.SelectedItem.ToString())}"))
            {
                //Create necessary folder structure inside selected Target car folder (folder of car name + folders like "scene", "exterior", etc.)
                Directory.CreateDirectory($"{ExportPath}{"\\"}{Path.GetDirectoryName(LST_TargetModel.SelectedItem.ToString())}");
            }

            //copy + overwrite selected donor model to selected target model path (folders are being "constructed"[combined])
            File.Copy(Path.Combine(ImportPath, LST_DonorModel.SelectedItem.ToString()), Path.Combine(ExportPath, LST_TargetModel.SelectedItem.ToString()), true);

            CarbinSwap();
        }

        private void CarbinSwap()
        {
            //Import + Export strings to make things easier
            string ExportPath = $"{rawPath}{"Model Swap\\Export\\"}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}";
            string ImportPath = $"{rawPath}{"Model Swap\\Import\\"}";
            string DonorCarbin = $"{ImportPath}{Path.GetFileNameWithoutExtension(LST_DonorCar.SelectedItem.ToString())}{"\\"}{Path.GetFileNameWithoutExtension(LST_DonorCar.SelectedItem.ToString())}{".carbin"}";
            string TargetCarbin = $"{ExportPath}{"\\"}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".carbin"}";

            if (!Directory.Exists($"{ImportPath}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}"))
            {
                ZipFile.ExtractToDirectory($"{GamePath}{GamePathMedia}{"\\"}{LST_TargetCar.SelectedItem}{".zip"}", $"{ImportPath}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}");
            }
            if (!File.Exists(TargetCarbin))
            {
                File.Copy($"{ImportPath}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{"\\"}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{".carbin"}", TargetCarbin);
            }

            // searches for first byte of 'pattern' inside 'src', if first byte found try to match 'pattern'
            int Search(byte[] src, byte[] pattern, int startOfIndex)
            {
                int maxFirstCharSlot = src.Length - pattern.Length + 1;
                for (int i = startOfIndex; i < maxFirstCharSlot; i++)
                {
                    if (src[i] != pattern[0]) // compare only first byte
                        continue;

                    // found a match on first byte, now try to match rest of the pattern
                    for (int j = pattern.Length - 1; j >= 1; j--)
                    {
                        if (src[i + j] != pattern[j]) break;
                        if (j == 1) return i;
                    }
                }
                return 69;
            }

            byte[] PatternFF = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] DonoFileBytes = File.ReadAllBytes(DonorCarbin);
            byte[] DonoSearchBytes = Encoding.ASCII.GetBytes(Path.GetFileName(LST_DonorModel.SelectedItem.ToString().ToLower()));
            int DonoStartPosition = Search(DonoFileBytes, DonoSearchBytes, 0);
            int DonoEndPosition = Search(DonoFileBytes, PatternFF, DonoStartPosition);

            byte[] TargFileBytes = File.ReadAllBytes(TargetCarbin);
            byte[] TargSearchBytes = Encoding.ASCII.GetBytes(Path.GetFileName(LST_TargetModel.SelectedItem.ToString().ToLower()));
            int TargStartPosition = Search(TargFileBytes, TargSearchBytes, 0);
            int TargEndPosition = Search(TargFileBytes, PatternFF, TargStartPosition);

            // read carbin from target car, dump everything till selected target model texture entry into temp.dat
            using (FileStream targCarb = File.OpenRead(TargetCarbin))
            using (FileStream writeStream = File.OpenWrite(rawPath + "Model Swap\\temp.dat"))
            {
                // create a buffer to hold the bytes 
                byte[] buffer = new byte[1024];
                int bytesRead;

                // while beginning of selected target model texture entry is bigger than current position read + write into file
                while (TargStartPosition + Path.GetFileName(LST_TargetModel.SelectedItem.ToString()).Length + 70 > targCarb.Position)
                {
                    bytesRead = targCarb.Read(buffer, 0, 1);
                    writeStream.Write(buffer, 0, bytesRead);
                }
                targCarb.Flush();
                targCarb.Close();
            }

            // read carbin from dono car, dump selected model texture entry into temp.dat
            using (FileStream donoCarb = File.OpenRead(DonorCarbin))
            using (FileStream writeStream = File.OpenWrite(rawPath + "Model Swap\\temp.dat"))
            {
                byte[] buffer = new Byte[1024];
                int bytesRead;
                int yeppers = 0;

                // seek inside carbin to beginning of needed texture entry. seek inside temp.dat to end of the file
                donoCarb.Seek(DonoStartPosition + Path.GetFileName(LST_DonorModel.SelectedItem.ToString()).Length + 70, SeekOrigin.Begin);
                // seek to end of temp.dat
                writeStream.Seek(TargStartPosition + Path.GetFileName(LST_TargetModel.SelectedItem.ToString()).Length + 70, SeekOrigin.Begin);
                // while selected texture entry is bigger than 'yeppers' read and write into file. yeppers = 0 and gets increased by 1 after each read + write
                while (DonoEndPosition - (DonoStartPosition + Path.GetFileName(LST_DonorModel.SelectedItem.ToString()).Length + 70) > yeppers)
                {
                    bytesRead = donoCarb.Read(buffer, 0, 1);
                    writeStream.Write(buffer, 0, bytesRead);
                    yeppers++;
                }
                writeStream.Flush();
                writeStream.Close();
            }

            // read target car carbin, dump rest of the file
            using (FileStream targCarb = File.OpenRead(TargetCarbin))
            using (FileStream writeStream = File.Open(rawPath + "Model Swap\\temp.dat", FileMode.Open))
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                // seek to end of
                targCarb.Seek(TargEndPosition, SeekOrigin.Begin);
                // seek to end of temp.dat again
                writeStream.Seek(TargStartPosition + Path.GetFileName(LST_TargetModel.SelectedItem.ToString()).Length + 70 + DonoEndPosition - (DonoStartPosition + Path.GetFileName(LST_DonorModel.SelectedItem.ToString()).Length + 70), SeekOrigin.Begin);
                while ((bytesRead = targCarb.Read(buffer, 0, 1024)) > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                }
                targCarb.Flush();
                targCarb.Close();
            }

            // delete target carbin, rename "temp.dat" to target carbin and move to its location
            File.Delete(TargetCarbin);
            File.Copy(rawPath + "Model Swap\\temp.dat", TargetCarbin);
            File.Delete(rawPath + "Model Swap\\temp.dat");

            System.Windows.MessageBox.Show($"{"Successfully swapped \""}{Path.GetFileName(LST_DonorModel.SelectedItem.ToString())}{"\" from \""}{Path.GetFileNameWithoutExtension(LST_DonorCar.SelectedItem.ToString())}" +
                $"{"\" to \""}{Path.GetFileName(LST_TargetModel.SelectedItem.ToString())}{"\" from \""}{Path.GetFileNameWithoutExtension(LST_TargetCar.SelectedItem.ToString())}{"\"."}", "Swap successful!");
        }

        private void BTN_CLR_Click(object sender, RoutedEventArgs e)
        {
            string ModelSwapPath = $"{rawPath}{"Model Swap\\"}";
            string WheelSwapPath = $"{rawPath}{"Wheel Swap\\"}";

            // if Export folder exists remove both Export and Import folders
            if (Directory.Exists(ModelSwapPath))
            {
                MessageBoxResult result;
                result = System.Windows.MessageBox.Show("This will delete all created folders + files inside \"" + this.rawPath + " \nAre you sure you want to proceed?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Directory.Delete(ModelSwapPath, true);
                    Directory.Delete(WheelSwapPath, true);
                }
            }
            else System.Windows.MessageBox.Show("There is nothing to delete.");
        }
    }
}
