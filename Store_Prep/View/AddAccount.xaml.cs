using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using MessageBox = System.Windows.MessageBox;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using ModernWpf.Controls;
using System.Windows.Threading;
using Application = System.Windows.Application;
using System.Printing;
using Microsoft.VisualBasic;
using System.Windows.Controls;
using System.Windows.Media;
using ListBox = System.Windows.Controls.ListBox;

namespace Store_Prep.View
{
    public partial class AddAccount : Window
    {

        private MainWindow _mainWindow;
        private ArrayList accountNumbers;
        private string _sourcePath;
        private string _accountName;

        public AddAccount(MainWindow mainWindow, string sourcePath, string accountName)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _sourcePath = sourcePath;
            _accountName = accountName;
            AccountNameTextBox.Text = _accountName; // set the text of AccountNameTextBox to the passed account name
            accountNumbers = new ArrayList();
        }

        private void BrowseDestinationButton_Click(object sender, RoutedEventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Display the selected path in the TextBox
                    SelectedFolderPathTextBox.Text = folderBrowser.SelectedPath;
                }
            }
        }


        private void AccountNumberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); // Regex to only allow integers
            return !regex.IsMatch(text);
        }

        private int GetNextAccountId(string filePath)
        {
            int maxId = 0;
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int id))
                    {
                        maxId = Math.Max(maxId, id);
                    }
                }
            }
            return maxId + 1;
        }

        private void AccountNumberEnterButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the textbox is not empty and contains a valid number
            if (!string.IsNullOrWhiteSpace(AccountNumberTextBox.Text) && int.TryParse(AccountNumberTextBox.Text, out int number))
            {
                accountNumbers.Add(number);
                AccountNumbersListBox.Items.Add(number);
                AccountNumberTextBox.Clear(); // Clear the textbox
            }
            else
            {
                MessageBox.Show("Please enter a valid account number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveAccountNumberButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccountNumber = AccountNumbersListBox.SelectedItem;
            if (selectedAccountNumber != null)
            {
                accountNumbers.Remove(selectedAccountNumber);
                AccountNumbersListBox.Items.Remove(selectedAccountNumber);
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Prep\accounts.txt";
            string accountName = AccountNameTextBox.Text.ToUpper();

            try
            {
                // Check if any of the fields are empty
                if (string.IsNullOrWhiteSpace(accountName) ||
                    string.IsNullOrWhiteSpace(SelectedFolderPathTextBox.Text) ||
                    AccountFormatComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Please fill in all fields.", "Missing Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Initialize the progress bar value

                // Only append the new account name to the accounts.txt and add it to the ComboBox if it's not already in the ComboBox
                if (!_mainWindow.AccountComboBox.Items.Contains(accountName))
                {
                    int accountId = GetNextAccountId(filePath);
                    string newEntry = $"{accountId},{accountName},{_sourcePath}";
                    File.AppendAllText(filePath, newEntry + Environment.NewLine);
                    _mainWindow.AccountComboBox.Items.Add(accountName);
                }

                CopyFolders(_sourcePath, SelectedFolderPathTextBox.Text, accountNumbers);
                CleanAndOrganizeFolders(SelectedFolderPathTextBox.Text);
                MessageBox.Show("Folders created!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing request: {ex.Message}");

            }
        }


        private void CopyFolders(string sourcePath, string destinationPath, ArrayList accountNumbers)
        {
            // Get all directories in the source path
            var directories = Directory.GetDirectories(sourcePath);

            // If the 3rd item in AccountFormatComboBox is chosen
            if (AccountFormatComboBox.SelectedIndex == 2)
            {
                // Skip to finding and copying the folder with the largest 6-digit integer
                var childDirectories = Directory.GetDirectories(sourcePath);

                // Get the child directory with the largest 6-digit number in its name
                var largestNumberDirectory = childDirectories.OrderByDescending(d => GetLargestNumberInString(Path.GetFileName(d))).FirstOrDefault();

                if (largestNumberDirectory != null)
                {
                    // Copy the directory to the destination path
                    DirectoryCopy(largestNumberDirectory, Path.Combine(destinationPath, Path.GetFileName(largestNumberDirectory)), true);
                }
            }

            foreach (var dir in directories)
            {
                foreach (int accountNumber in accountNumbers)
                {
                    // If the 2nd item in AccountFormatComboBox is chosen
                    if (AccountFormatComboBox.SelectedIndex == 1)
                    {
                        if (accountNumber.ToString().Length == 4)
                        {
                            // If the directory, starting from the 6th character, starts with the account number
                            if (Path.GetFileName(dir).Substring(5).StartsWith(accountNumber.ToString()))
                            {
                                CopyFolder(dir, destinationPath);
                            }
                        }
                        else
                        {
                            // If the directory, starting from the 6th character, starts with the account number
                            if (Path.GetFileName(dir).Substring(6).StartsWith(accountNumber.ToString()))
                            {
                                CopyFolder(dir, destinationPath);
                            }
                        }
                    }
                    else // If not the 2nd or the 3rd item
                    {
                        // Format the account number to 3 digits with leading zeroes
                        string formattedAccountNumber = accountNumber.ToString().PadLeft(3, '0');

                        // If the directory starts with the formatted account number
                        if (Path.GetFileName(dir).StartsWith(formattedAccountNumber))
                        {
                            CopyFolder(dir, destinationPath);
                        }
                    }

                }
            }
        }


        private void CopyFolder(string dir, string destinationPath)
        {
            var childDirectories = Directory.GetDirectories(dir);

            // Get the child directory with the largest 6-digit number in its name
            var largestNumberDirectory = childDirectories.OrderByDescending(d => GetLargestNumberInString(Path.GetFileName(d))).FirstOrDefault();

            if (largestNumberDirectory != null)
            {
                // Copy the directory to the destination path
                DirectoryCopy(largestNumberDirectory, Path.Combine(destinationPath, Path.GetFileName(largestNumberDirectory)), true);
            }
        }


        private int GetLargestNumberInString(string s)
        {
            return Regex.Matches(s, @"\d+")
                        .Cast<Match>()
                        .Select(m => int.Parse(m.Value))
                        .DefaultIfEmpty(0)
                        .Max();
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void CleanAndOrganizeFolders(string destinationPath)
        {
            var directories = Directory.GetDirectories(destinationPath);
            // Get the parent directory of destinationPath
            DirectoryInfo destinationInfo = new DirectoryInfo(destinationPath);
            DirectoryInfo parentInfo = destinationInfo.Parent;

            List<string> newFolderPaths = new List<string>();

            foreach (var dir in directories)
            {
                // Remove files containing "timesheet" (case insensitive) and/or " TS" (case sensitive - space included)
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                FileInfo[] filesToRemove = directoryInfo.GetFiles().Where(f => f.Name.ToLower().Contains("timesheet") || f.Name.Contains(" TS")).ToArray();
                foreach (FileInfo file in filesToRemove)
                {
                    file.Delete();
                }

                // Get the new folder name by removing the 6 digits from the current folder name
                string newFolderName = Regex.Replace(directoryInfo.Name, @"\d{6}", "").Trim();
                // Combine parentInfo.FullName with newFolderName
                string newFolderPath = Path.Combine(parentInfo.FullName, newFolderName);

                // Create the new folder
                Directory.CreateDirectory(newFolderPath);

                newFolderPaths.Add(newFolderPath);
            }

            // Now we move the files and folders from the parent directory of destinationPath to the new folders
            string[] keywords = { "To ", "Teams", "Price", "Cards" };
            foreach (string keyword in keywords)
            {
                string[] filesToMove = Directory.GetFiles(parentInfo.FullName, $"*{keyword}*", SearchOption.TopDirectoryOnly);
                foreach (string file in filesToMove)
                {
                    foreach (string newFolderPath in newFolderPaths)
                    {
                        string fileName = Path.GetFileName(file);
                        string newFilePath = Path.Combine(newFolderPath, fileName);
                        // Check if file exists before moving
                        if (File.Exists(file) && !File.Exists(newFilePath))
                        {
                            File.Copy(file, newFilePath);
                        }
                    }
                    // Delete the file from parent directory after copying
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }

                string[] foldersToMove = Directory.GetDirectories(parentInfo.FullName, $"*{keyword}*", SearchOption.TopDirectoryOnly);
                foreach (string folder in foldersToMove)
                {
                    foreach (string newFolderPath in newFolderPaths)
                    {
                        string folderName = Path.GetFileName(folder);
                        string newFolderPath2 = Path.Combine(newFolderPath, folderName);
                        // Check if folder exists before moving
                        if (Directory.Exists(folder) && !Directory.Exists(newFolderPath2))
                        {
                            CopyDirectory(folder, newFolderPath2);
                        }
                    }
                    // Delete the folder from parent directory after copying
                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder, true); // true to remove folders and its contents
                    }
                }
            }
        }

        private void CopyDirectory(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }

        private void AccountNumberTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AccountNumberEnterButton_Click(sender, e);

            }
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(listBox);

            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        private void AccountNumbersListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(AccountNumbersListBox, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (item != null)
            {
                if (item.IsSelected)
                {
                    // If the clicked item is already selected, deselect it.
                    item.IsSelected = false;
                    e.Handled = true;
                }
            }
        }



    }
}
