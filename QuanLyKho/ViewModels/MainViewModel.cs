using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using QuanLyKho.Models;
using QuanLyKho.Services;

namespace QuanLyKho.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly DataService _dataService;
        private DateTime _selectedDate;
        private string _dealerName;
        private string _itemName;
        private int _quantity;
        private decimal _amount;
        private Transaction _selectedTransaction;
        private int _selectedYear;
        private int _selectedMonth;
        private List<Transaction> _selectedTransactions;
        private string _filterDealerName;

        public ObservableCollection<Transaction> Transactions { get; set; }
        public ObservableCollection<int> Years { get; set; }
        public ObservableCollection<int> Months { get; set; }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public string DealerName
        {
            get => _dealerName;
            set => SetProperty(ref _dealerName, value);
        }

        public string ItemName
        {
            get => _itemName;
            set => SetProperty(ref _itemName, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public Transaction SelectedTransaction
        {
            get => _selectedTransaction;
            set => SetProperty(ref _selectedTransaction, value);
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (SetProperty(ref _selectedYear, value))
                {
                    LoadTransactions();
                }
            }
        }

        public int SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                {
                    LoadTransactions();
                }
            }
        }

        public string FilterDealerName
        {
            get => _filterDealerName;
            set
            {
                if (SetProperty(ref _filterDealerName, value))
                {
                    LoadTransactions();
                }
            }
        }

        public decimal TotalImport => Transactions
            .Where(t => t.Type == TransactionType.Import)
            .Sum(t => t.Amount);

        public decimal TotalExport => Transactions
            .Where(t => t.Type == TransactionType.Export)
            .Sum(t => t.Amount);

        public decimal Balance => TotalImport - TotalExport;

        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DeleteMultipleCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand OpenDebtCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            _dataService = new DataService();
            
            Transactions = new ObservableCollection<Transaction>();
            Years = new ObservableCollection<int>();
            Months = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Initialize years (current year and 5 years back)
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear - 5; i <= currentYear + 1; i++)
            {
                Years.Add(i);
            }

            _selectedYear = currentYear;
            _selectedMonth = DateTime.Now.Month;
            _selectedDate = DateTime.Now;
            _selectedTransactions = new List<Transaction>();

            ImportCommand = new RelayCommand(ImportItem, CanAddTransaction);
            ExportCommand = new RelayCommand(ExportItem, CanAddTransaction);
            DeleteCommand = new RelayCommand(DeleteTransaction, CanDeleteTransaction);
            DeleteMultipleCommand = new RelayCommand(DeleteMultipleTransactions, CanDeleteMultipleTransactions);
            ExportToExcelCommand = new RelayCommand(ExportToExcel);
            OpenDebtCommand = new RelayCommand(OpenDebtWindow);
            LogoutCommand = new RelayCommand(Logout);

            LoadTransactions();
        }

        private bool CanAddTransaction(object parameter)
        {
            return !string.IsNullOrWhiteSpace(DealerName) &&
                   Amount > 0;
        }

        private bool CanDeleteTransaction(object parameter)
        {
            return SelectedTransaction != null;
        }

        private bool CanDeleteMultipleTransactions(object parameter)
        {
            return _selectedTransactions != null && _selectedTransactions.Count > 0;
        }

        public void UpdateSelectedTransactions(List<Transaction> selectedItems)
        {
            _selectedTransactions = selectedItems ?? new List<Transaction>();
            CommandManager.InvalidateRequerySuggested();
        }

        private void ImportItem(object parameter)
        {
            var transaction = new Transaction
            {
                Date = SelectedDate,
                Type = TransactionType.Import,
                DealerName = DealerName,
                ItemName = ItemName,
                Quantity = Quantity,
                Amount = Amount
            };

            _dataService.AddTransaction(transaction);
            LoadTransactions();
            ClearForm();
            MessageBox.Show("Đã nhập kho thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportItem(object parameter)
        {
            var transaction = new Transaction
            {
                Date = SelectedDate,
                Type = TransactionType.Export,
                DealerName = DealerName,
                ItemName = ItemName,
                Quantity = Quantity,
                Amount = Amount
            };

            _dataService.AddTransaction(transaction);
            LoadTransactions();
            ClearForm();
            MessageBox.Show("Đã xuất kho thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteTransaction(object parameter)
        {
            if (SelectedTransaction != null)
            {
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa giao dịch này?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _dataService.DeleteTransaction(SelectedTransaction);
                    LoadTransactions();
                    MessageBox.Show("Đã xóa giao dịch thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteMultipleTransactions(object parameter)
        {
            if (_selectedTransactions != null && _selectedTransactions.Count > 0)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa {_selectedTransactions.Count} giao dịch đã chọn?",
                    "Xác nhận xóa nhiều",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Tạo bản sao để tránh lỗi khi xóa trong vòng lặp
                    var transactionsToDelete = _selectedTransactions.ToList();
                    
                    foreach (var transaction in transactionsToDelete)
                    {
                        _dataService.DeleteTransaction(transaction);
                    }
                    
                    LoadTransactions();
                    MessageBox.Show($"Đã xóa {transactionsToDelete.Count} giao dịch thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ExportToExcel(object parameter)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"{SelectedYear}_{SelectedMonth:D2}.xlsx",
                    Title = "Chọn nơi lưu file Excel"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _dataService.ExportToExcel(SelectedYear, SelectedMonth, saveFileDialog.FileName);
                    MessageBox.Show(
                        $"Đã xuất dữ liệu ra file Excel thành công!\n\nĐường dẫn: {saveFileDialog.FileName}",
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi xuất Excel: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadTransactions()
        {
            var transactions = _dataService.GetTransactionsByMonth(SelectedYear, SelectedMonth);
            
            // Áp dụng filter theo tên đại lý nếu có
            if (!string.IsNullOrWhiteSpace(FilterDealerName))
            {
                transactions = transactions
                    .Where(t => t.DealerName.Contains(FilterDealerName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            
            Transactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }

            OnPropertyChanged(nameof(TotalImport));
            OnPropertyChanged(nameof(TotalExport));
            OnPropertyChanged(nameof(Balance));
        }

        private void ClearForm()
        {
            DealerName = string.Empty;
            ItemName = string.Empty;
            Quantity = 0;
            Amount = 0;
            SelectedDate = DateTime.Now;
        }

        private void OpenDebtWindow(object parameter)
        {
            var debtWindow = new Views.DebtWindow();
            debtWindow.ShowDialog();
        }

        private void Logout(object parameter)
        {
            // Lấy window hiện tại
            var currentWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            
            // Tạo và hiển thị LoginWindow
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
            
            // Đặt LoginWindow làm MainWindow mới
            Application.Current.MainWindow = loginWindow;
            
            // Đóng window hiện tại
            currentWindow?.Close();
        }
    }
}
