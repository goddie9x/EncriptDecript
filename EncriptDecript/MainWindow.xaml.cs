using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;

namespace TEncryptDecrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        private bool isEnableButton = false;
        private string key;
        private string _Key
        {
            get => key;
            set
            {
                if (value.Trim() != "")
                {
                    key = value;
                    isEnableButton = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Key must not empty!");
                    isEnableButton = false;
                }
                 ESelectButton.IsEnabled = isEnableButton;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        #region event
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            startEnOrDeCrypt();
        }
        private void KeyEncrypt_TextChanged(object sender, TextChangedEventArgs e)
        {
            _Key = KeyEncrypt.Text;
        }
        #endregion
        #region method 
        private string EncryptOrDecrypt(string rawText, string rawKey)
        {
            byte[] text = Encoding.Unicode.GetBytes(rawText);
            byte[] key = Encoding.Unicode.GetBytes(rawKey);
            byte[] xor = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                xor[i] = (byte)(text[i] ^ key[i % key.Length]);
            }
            string result = Encoding.Unicode.GetString(xor);
            return result;
        }
        private void startEnOrDeCrypt()
        {
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                KeyEncrypt.IsEnabled = false;
                byte[] edKey = Encoding.Unicode.GetBytes(_Key);
                string[] files = Directory.GetFiles(folderBrowserDialog.SelectedPath);
                System.Windows.MessageBoxResult choice =
                    System.Windows.MessageBox.Show("Do you want to encrypt all file inside this folder? " + folderBrowserDialog.SelectedPath + " ?", "Warning!!!", MessageBoxButton.YesNo);
                ProgressBar.Visibility = Visibility.Visible;
                if (choice == MessageBoxResult.Yes)
                {
                    foreach (var filePath in files)
                    {
                        ProgressBar.Value = 30;
                        try
                        {
                            File.Copy(filePath, filePath + ".back");
                            string raw = File.ReadAllText(filePath);
                            string enText = EncryptOrDecrypt(raw, _Key);
                            File.WriteAllText(filePath, enText);
                            File.Delete(filePath + ".back");
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                        ProgressBar.Value = 50;
                    }
                    ProgressBar.Value = 100;
                    System.Windows.MessageBox.Show("Action success");
                    ProgressBar.Visibility = Visibility.Hidden;
                }
                KeyEncrypt.IsEnabled = true;
            }
        }
        #endregion
    }
}
