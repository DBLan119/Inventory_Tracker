using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using QuanLyKho.Models;

namespace QuanLyKho.Views
{
    public partial class DebtWindow : Window
    {
        private bool isAmountFormatting = false;

        public DebtWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                Regex regex = new Regex("[^0-9]+");
                if (regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void DebtAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isAmountFormatting) return;

            var textBox = sender as TextBox;
            if (textBox == null) return;

            isAmountFormatting = true;

            int cursorPosition = textBox.SelectionStart;
            string text = textBox.Text;

            string cleanText = text.Replace(",", "");

            if (!string.IsNullOrEmpty(cleanText) && long.TryParse(cleanText, out long number))
            {
                if (textBox.DataContext is ViewModels.DebtViewModel vm)
                {
                    vm.DebtAmount = number;
                }

                string formattedText = number.ToString("N0", CultureInfo.InvariantCulture);
                
                int commasBeforeCursor = text.Substring(0, Math.Min(cursorPosition, text.Length))
                    .Count(c => c == ',');
                int commasInFormatted = formattedText.Substring(0, Math.Min(cursorPosition - commasBeforeCursor, formattedText.Length))
                    .Count(c => c == ',');
                
                textBox.Text = formattedText;
                
                int newPosition = cursorPosition - commasBeforeCursor + commasInFormatted;
                textBox.SelectionStart = Math.Min(newPosition, formattedText.Length);
            }
            else if (string.IsNullOrEmpty(cleanText))
            {
                if (textBox.DataContext is ViewModels.DebtViewModel vm)
                {
                    vm.DebtAmount = 0;
                }
            }

            isAmountFormatting = false;

            CommandManager.InvalidateRequerySuggested();
        }

        private void DebtAmountTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.Text = textBox.Text.Replace(",", "");
            }
        }

        private void DebtAmountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && !string.IsNullOrEmpty(textBox.Text))
            {
                string cleanText = textBox.Text.Replace(",", "");
                if (long.TryParse(cleanText, out long number))
                {
                    textBox.Text = number.ToString("N0", CultureInfo.InvariantCulture);
                }
            }
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(DebtAmountTextBox.Text))
            {
                string cleanText = DebtAmountTextBox.Text.Replace(",", "");
                if (long.TryParse(cleanText, out long number))
                {
                    if (DataContext is ViewModels.DebtViewModel vm)
                    {
                        vm.DebtAmount = number;
                    }
                }
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private void DebtsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModels.DebtViewModel vm)
            {
                var selectedItems = DebtsDataGrid.SelectedItems;
                vm.UpdateSelectedDebts(selectedItems.Cast<Debt>().ToList());
            }
        }
    }

    public class DebtStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
            {
                return isPaid ? "Đã trả" : "Chưa trả";
            }
            return "Chưa trả";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
