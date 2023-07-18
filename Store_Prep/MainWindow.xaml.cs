using Microsoft.Win32;
using System.Windows.Forms;
using System.Windows;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Store_Prep.View;
using System.IO;
using System.Collections.Generic;
using MessageBox = System.Windows.MessageBox;
using System;

namespace Store_Prep
{
    public partial class MainWindow : Window
    {

        private Dictionary<string, string> _accountPaths = new Dictionary<string, string>();
        private string _sourcePath;

        public MainWindow()
        {
            InitializeComponent();
            
            // Create the file if it doesn't already exist
            string filePath = @"C:\Prep\accounts.txt";
            if (!File.Exists(filePath))
            {
                //Create the folder
                string folderPath = @"C:\Prep";
                Directory.CreateDirectory(folderPath);
                // Create the file.
                File.Create(filePath).Dispose();
            }

            LoadAccounts();
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Save the selected path to a field.
                    _sourcePath = folderBrowser.SelectedPath;
                    OpenAddAccountWindow(_sourcePath);
                }
            }
        }


        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // You can use openFileDialog.FileName
            }
        }

        private void AccountComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selectedAccount = AccountComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedAccount) && _accountPaths.ContainsKey(selectedAccount))
            {
                _sourcePath = _accountPaths[selectedAccount];
                // Pass this sourcePath to the new window
                OpenAddAccountWindow(_sourcePath, selectedAccount);
            }

            AccountComboBox.SelectedIndex = -1;
        }

        private void OpenAddAccountWindow(string _sourcePath, string accountName = "")
        {
            if (!string.IsNullOrEmpty(_sourcePath))
            {
                AddAccount addAccountWindow = new AddAccount(this, _sourcePath, accountName);
                addAccountWindow.Owner = this;
                addAccountWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a source path first.");
            }
        }

        private void LoadAccounts()
        {
            string filePath = @"C:\Prep\accounts.txt";
            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length > 2)
                        {
                            AccountComboBox.Items.Add(parts[1]);
                            _accountPaths[parts[1]] = parts[2];
                        }
                    }
                }

                AccountComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}");
            }
        }

    }
}
