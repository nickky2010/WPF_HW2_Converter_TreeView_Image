using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Converter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Currency> currencies = new List<Currency>();
        DispatcherTimer timer = new DispatcherTimer();
        string dir = Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName;

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Tick += Timer_Tick;
            timer.Start();
            string[] files = Directory.GetFiles(dir + @"\App_Data\");
            foreach (string file in files)
            {
                comboBoxBanks.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }
            if (comboBoxBanks.Items.Count != 0)
                comboBoxBanks.SelectedIndex = 0;
        }

        private void SetDataGrid()
        {
            dataGrid.AutoGenerateColumns = false;
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            dataGrid.Columns.Clear();
            //dataGrid.ItemsSource = currencies; // it's too easy
            dataGrid.Columns.Add(new DataGridTextColumn());
            dataGrid.Columns[0].Header = "Abbreviation";
            dataGrid.Columns.Add(new DataGridTextColumn());
            dataGrid.Columns[1].Header = "Full Title";
            dataGrid.Columns.Add(new DataGridTextColumn());
            dataGrid.Columns[2].Header = "Rate";
            ((DataGridTextColumn)dataGrid.Columns[0]).IsReadOnly = true;
            ((DataGridTextColumn)dataGrid.Columns[1]).IsReadOnly = true;
            ((DataGridTextColumn)dataGrid.Columns[2]).IsReadOnly = true;
            ((DataGridTextColumn)dataGrid.Columns[1]).Width = 210;
            ((DataGridTextColumn)dataGrid.Columns[0]).Binding = new Binding("Abbreviation");
            ((DataGridTextColumn)dataGrid.Columns[1]).Binding = new Binding("Name");
            ((DataGridTextColumn)dataGrid.Columns[2]).Binding = new Binding("Rate");
            foreach (Currency c in currencies)
            {
                dataGrid.Items.Add(c);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            popup.IsOpen = true;
        }

        private void Read(string pathFile)
        {
            using (StreamReader reader = new StreamReader(pathFile))
            {
                currencies.Clear();
                while (!reader.EndOfStream)
                {
                    try
                    {
                        string[] temp = reader.ReadLine().Split(';');
                        currencies.Add(new Currency() { Name = temp[0], Abbreviation = temp[1], Rate = double.Parse(temp[2].Replace('.', ',')) });
                    }
                    catch
                    {
                        MessageBox.Show("Could not read file "+ pathFile);
                    }
                }
            }
        }

        private void Write(string pathFile)
        {
            using (StreamWriter writer = new StreamWriter(pathFile))
            {
                foreach (Currency item in dataGrid.Items)
                {
                    writer.WriteLine(item.Name + ";" + item.Abbreviation + ";" + item.Rate);
                }
            }
        }

        private void ButtonConvert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal AmountInRoubles = decimal.Parse(textBoxAmountInRoubles.Text);
                if (AmountInRoubles < 0)
                    throw new Exception();
                int rowIndex = dataGrid.SelectedIndex;
                if (rowIndex != -1)
                {
                    textBoxAmountInCurrency.Text = (AmountInRoubles / (decimal)((Currency)dataGrid.Items[rowIndex]).Rate).ToString("f2");
                }
                else
                    MessageBox.Show("Please, choose currency row!");
            }
            catch
            {
                MessageBox.Show("Amount in roubles is not corrected!");
            }
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                Write(saveFileDialog.FileName);
        }
        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            WindowHelp windowHelp = new WindowHelp();
            windowHelp.Show();
        }

        private void ComboBoxBanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetCourses();
            groupBoxTable.Header = "Exchange rates in the bank " + comboBoxBanks.SelectedItem;
            SetDataGrid();
        }
        private void GetCourses()
        {
            string[] files = Directory.GetFiles(dir + @"\App_Data\");
            foreach (string file in files)
            {
                if (System.IO.Path.GetFileNameWithoutExtension(file) == comboBoxBanks.SelectedItem.ToString())
                {
                    Read(file);
                    break;
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int rowIndex = dataGrid.SelectedIndex;
            if (rowIndex != -1)
            {
                labelAmountInCurrency.Content = "Amount in currency ("+ ((Currency)dataGrid.Items[rowIndex]).Abbreviation +"):";
                labelCurrency.Content = ((Currency)dataGrid.Items[rowIndex]).Name;
            }
        }
    }
}
