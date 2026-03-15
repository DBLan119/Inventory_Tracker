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
    public class DebtViewModel : BaseViewModel
    {
        private readonly DebtService _debtService;
        private DateTime _purchaseDate;
        private string _customerName;
        private string _itemName;
        private decimal _debtAmount;
        private Debt _selectedDebt;
        private string _filterCustomerName;
        private List<Debt> _selectedDebts;

        public ObservableCollection<Debt> Debts { get; set; }

        public DateTime PurchaseDate
        {
            get => _purchaseDate;
            set => SetProperty(ref _purchaseDate, value);
        }

        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }

        public string ItemName
        {
            get => _itemName;
            set => SetProperty(ref _itemName, value);
        }

        public decimal DebtAmount
        {
            get => _debtAmount;
            set => SetProperty(ref _debtAmount, value);
        }

        public Debt SelectedDebt
        {
            get => _selectedDebt;
            set => SetProperty(ref _selectedDebt, value);
        }

        public string FilterCustomerName
        {
            get => _filterCustomerName;
            set
            {
                if (SetProperty(ref _filterCustomerName, value))
                {
                    ApplyFilter();
                }
            }
        }

        public decimal TotalDebt => Debts.Where(d => !d.IsPaid).Sum(d => d.DebtAmount);
        public decimal TotalAllDebt => Debts.Sum(d => d.DebtAmount);

        public ICommand AddDebtCommand { get; }
        public ICommand MarkAsPaidCommand { get; }
        public ICommand DeleteDebtCommand { get; }
        public ICommand DeleteMultipleDebtsCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand ExportToExcelCommand { get; }

        public DebtViewModel()
        {
            _debtService = new DebtService();
            
            Debts = new ObservableCollection<Debt>();
            _purchaseDate = DateTime.Now;
            _customerName = string.Empty;
            _itemName = string.Empty;
            _filterCustomerName = string.Empty;
            _selectedDebts = new List<Debt>();

            AddDebtCommand = new RelayCommand(AddDebt, CanAddDebt);
            MarkAsPaidCommand = new RelayCommand(MarkAsPaid, CanMarkAsPaid);
            DeleteDebtCommand = new RelayCommand(DeleteDebt, CanDeleteDebt);
            DeleteMultipleDebtsCommand = new RelayCommand(DeleteMultipleDebts, CanDeleteMultipleDebts);
            ClearFilterCommand = new RelayCommand(ClearFilter);
            ExportToExcelCommand = new RelayCommand(ExportToExcel);

            LoadDebts();
        }

        private bool CanAddDebt(object parameter)
        {
            return !string.IsNullOrWhiteSpace(CustomerName) && DebtAmount > 0;
        }

        private bool CanMarkAsPaid(object parameter)
        {
            return SelectedDebt != null && !SelectedDebt.IsPaid;
        }

        private bool CanDeleteDebt(object parameter)
        {
            return SelectedDebt != null;
        }

        private bool CanDeleteMultipleDebts(object parameter)
        {
            return _selectedDebts != null && _selectedDebts.Count > 0;
        }

        public void UpdateSelectedDebts(List<Debt> selectedItems)
        {
            _selectedDebts = selectedItems ?? new List<Debt>();
            CommandManager.InvalidateRequerySuggested();
        }

        private void AddDebt(object parameter)
        {
            var debt = new Debt
            {
                CustomerName = CustomerName,
                PurchaseDate = PurchaseDate,
                ItemName = ItemName,
                DebtAmount = DebtAmount,
                IsPaid = false
            };

            _debtService.AddDebt(debt);
            LoadDebts();
            ClearForm();
            MessageBox.Show("Đã thêm công nợ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MarkAsPaid(object parameter)
        {
            if (SelectedDebt != null)
            {
                var result = MessageBox.Show(
                    $"Đánh dấu công nợ của '{SelectedDebt.CustomerName}' đã thanh toán?",
                    "Xác nhận thanh toán",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SelectedDebt.IsPaid = true;
                    _debtService.UpdateDebt(SelectedDebt);
                    LoadDebts();
                    MessageBox.Show("Đã đánh dấu thanh toán!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteDebt(object parameter)
        {
            if (SelectedDebt != null)
            {
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa công nợ này?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _debtService.DeleteDebt(SelectedDebt);
                    LoadDebts();
                    MessageBox.Show("Đã xóa công nợ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteMultipleDebts(object parameter)
        {
            if (_selectedDebts != null && _selectedDebts.Count > 0)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa {_selectedDebts.Count} công nợ đã chọn?",
                    "Xác nhận xóa nhiều",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var debtsToDelete = _selectedDebts.ToList();
                    
                    foreach (var debt in debtsToDelete)
                    {
                        _debtService.DeleteDebt(debt);
                    }
                    
                    LoadDebts();
                    MessageBox.Show($"Đã xóa {debtsToDelete.Count} công nợ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(FilterCustomerName))
            {
                LoadDebts();
            }
            else
            {
                var filteredDebts = _debtService.GetDebtsByCustomer(FilterCustomerName);
                Debts.Clear();
                foreach (var debt in filteredDebts)
                {
                    Debts.Add(debt);
                }
                UpdateSummary();
            }
        }

        private void ClearFilter(object parameter)
        {
            FilterCustomerName = string.Empty;
            LoadDebts();
        }

        private void ExportToExcel(object parameter)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = "CongNo.xlsx",
                    Title = "Chọn nơi lưu file Excel"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _debtService.ExportDebtsToExcel(saveFileDialog.FileName);
                    MessageBox.Show(
                        $"Đã xuất dữ liệu công nợ ra file Excel thành công!\n\nĐường dẫn: {saveFileDialog.FileName}",
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

        private void LoadDebts()
        {
            var debts = _debtService.GetAllDebts();
            Debts.Clear();
            foreach (var debt in debts)
            {
                Debts.Add(debt);
            }
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            OnPropertyChanged(nameof(TotalDebt));
            OnPropertyChanged(nameof(TotalAllDebt));
        }

        private void ClearForm()
        {
            CustomerName = string.Empty;
            ItemName = string.Empty;
            DebtAmount = 0;
            PurchaseDate = DateTime.Now;
        }
    }
}
