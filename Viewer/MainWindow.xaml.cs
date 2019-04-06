using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Viewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        ImageSourceConverter imgs = new ImageSourceConverter();
        string appDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName;
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
            listBoxPicture.Items.Clear();
            listBoxPicture.Items.Add(image1);
            listBoxPicture.Items.Add(image2);
            listBoxPicture.Items.Add(image3);
            listBoxPicture.Items.Add(image4);
            listBoxPicture.Items.Add(image5);
            listBoxPicture.Items.Add(image6);
            string[] files = Directory.GetFiles(appDir + @"\Images\");
            foreach (string file in files)
            {
                AddPictureInViewerQueue(file);
            }
            listBoxPicture.SelectionChanged += ListBoxPicture_SelectionChanged;
        }

        private void ListBoxPicture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            imageMain.Source = ((Image)listBoxPicture.SelectedItem).Source;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelCurrentTime.Content = DateTime.Now.ToLongTimeString();
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Jpg file (*.jpg)|*.jpg|Jpeg file (*.jpeg)|*.jpeg";
            openFileDialog.InitialDirectory = appDir + @"\Images\";
            if (openFileDialog.ShowDialog() == true)
            {
                AddPictureInViewerQueue(openFileDialog.FileName);
            }
        }
        private void AddPictureInViewerQueue(string filename)
        {
            image6.Source = image5.Source;
            image5.Source = image4.Source;
            image4.Source = image3.Source;
            image3.Source = image2.Source;
            image2.Source = image1.Source;
            image1.SetValue(Image.SourceProperty, imgs.ConvertFromString(filename));
            imageMain.Source = image1.Source;
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Jpeg file (*.jpeg)|*.jpeg";
                saveFileDialog.InitialDirectory = appDir;
                if (saveFileDialog.ShowDialog() == true)
                {
                    filename = saveFileDialog.FileName;
                    JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                    jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(imageMain.Source as BitmapSource));
                    FileStream fileStream = new FileStream(filename, FileMode.CreateNew);
                    jpegBitmapEncoder.Save(fileStream);
                    fileStream.Close();
                    MessageBox.Show("File " + filename + " saved successfully");
                }
            }
            catch
            {
                MessageBox.Show("Could not save file " + filename);
            }
        }
    }
}
