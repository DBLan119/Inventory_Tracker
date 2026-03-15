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

namespace QuanLyKho
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isAmountFormatting = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Chỉ cho phép nhập số
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

        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isAmountFormatting) return;

            var textBox = sender as TextBox;
            if (textBox == null) return;

            isAmountFormatting = true;

            // Lưu vị trí con trỏ
            int cursorPosition = textBox.SelectionStart;
            string text = textBox.Text;

            // Xóa tất cả dấu phẩy
            string cleanText = text.Replace(",", "");

            // Nếu là số hợp lệ, format lại VÀ cập nhật vào ViewModel
            if (!string.IsNullOrEmpty(cleanText) && long.TryParse(cleanText, out long number))
            {
                // Cập nhật giá trị vào ViewModel ngay lập tức
                if (textBox.DataContext is ViewModels.MainViewModel vm)
                {
                    vm.Amount = number;
                }

                // Format số với dấu phẩy
                string formattedText = number.ToString("N0", CultureInfo.InvariantCulture);
                
                // Tính toán vị trí con trỏ mới
                int commasBeforeCursor = text.Substring(0, Math.Min(cursorPosition, text.Length))
                    .Count(c => c == ',');
                int commasInFormatted = formattedText.Substring(0, Math.Min(cursorPosition - commasBeforeCursor, formattedText.Length))
                    .Count(c => c == ',');
                
                textBox.Text = formattedText;
                
                // Đặt lại vị trí con trỏ
                int newPosition = cursorPosition - commasBeforeCursor + commasInFormatted;
                textBox.SelectionStart = Math.Min(newPosition, formattedText.Length);
            }
            else if (string.IsNullOrEmpty(cleanText))
            {
                // Nếu rỗng, set Amount = 0
                if (textBox.DataContext is ViewModels.MainViewModel vm)
                {
                    vm.Amount = 0;
                }
            }

            isAmountFormatting = false;

            // Refresh CanExecute của tất cả commands
            CommandManager.InvalidateRequerySuggested();
        }

        private void AmountTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                // Xóa format khi focus để dễ edit
                textBox.Text = textBox.Text.Replace(",", "");
            }
        }

        private void AmountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && !string.IsNullOrEmpty(textBox.Text))
            {
                // Format lại khi mất focus
                string cleanText = textBox.Text.Replace(",", "");
                if (long.TryParse(cleanText, out long number))
                {
                    textBox.Text = number.ToString("N0", CultureInfo.InvariantCulture);
                }
            }
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Đảm bảo Amount đã được cập nhật từ TextBox (nếu có)
            if (!string.IsNullOrEmpty(AmountTextBox.Text))
            {
                string cleanText = AmountTextBox.Text.Replace(",", "");
                if (long.TryParse(cleanText, out long number))
                {
                    if (DataContext is ViewModels.MainViewModel vm)
                    {
                        vm.Amount = number;
                    }
                }
            }

            // Force refresh CanExecute của commands
            CommandManager.InvalidateRequerySuggested();
        }

        private void TransactionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel vm)
            {
                var selectedItems = TransactionsDataGrid.SelectedItems;
                vm.UpdateSelectedTransactions(selectedItems.Cast<Transaction>().ToList());
            }
        }
    }

    public class TransactionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                return type == TransactionType.Import ? "Nhập" : "Xuất";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}