using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace TreeView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonShow_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                imageForTreeView.Visibility = Visibility.Hidden;
                mainWindow.Title = folderBrowserDialog.SelectedPath;
                BuildTreeView(folderBrowserDialog.SelectedPath);
            }
            else
                imageForTreeView.Visibility = Visibility.Visible;
        }
        private void progBarPlus()
        {
            progressBar.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                progressBar.Value++;
            }));
        }
        private async void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)e.OriginalSource;
                item.Items.Clear();
                DirectoryInfo[] dirs;
                FileInfo[] files = null;
                if (item == null)
                    return;
                if (item.Tag is DriveInfo)
                    dirs = new DirectoryInfo(item.Header.ToString()).GetDirectories();
                else
                {
                    dirs = ((DirectoryInfo)item.Tag).GetDirectories();
                    files = ((DirectoryInfo)item.Tag).GetFiles();
                }
                progressBar.Value = 0;
                progressBar.Maximum = dirs.Count();
                if (files != null)
                    progressBar.Maximum += files.Count();
                foreach (DirectoryInfo dir in dirs)
                {
                    TreeViewItem sItem = new TreeViewItem();
                    sItem.Header = dir.Name;
                    sItem.Tag = dir;
                    sItem.Items.Add(dir);
                    item.Items.Add(sItem);
                    await Task.Run(() => progBarPlus());
                }
                if (files != null)
                    foreach (FileInfo file in files)
                    {
                        item.Items.Add(file);
                        await Task.Run(() => progBarPlus());
                    }
                progressBar.Value = 0;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        void BuildTreeView(string dir)
        {
            try
            {
                treeView.Items.Clear();
                DirectoryInfo curDir = new DirectoryInfo(dir);
                TreeViewItem item = new TreeViewItem();
                item.Header = curDir.Name;
                item.Items.Add(curDir);
                item.Tag = curDir;
                treeView.Items.Add(item);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void TreeView_Selected(object sender, RoutedEventArgs e)
        {
            FileInfo file = treeView.SelectedItem as FileInfo;
            if (file != null)
            {
                textBlockFileInfo.Text = file.Name + "\n" + file.CreationTime + "\n" + file.Length + " bytes";
                popup.IsOpen = true;
            }
        }
    }
}
