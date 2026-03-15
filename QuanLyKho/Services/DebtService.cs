using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using QuanLyKho.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace QuanLyKho.Services
{
    public class DebtService
    {
        private readonly string _dataFolder;
        private readonly string _debtFile;
        private readonly JsonSerializerOptions _jsonOptions;

        public DebtService()
        {
            _dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(_dataFolder))
            {
                Directory.CreateDirectory(_dataFolder);
            }

            _debtFile = Path.Combine(_dataFolder, "debts.json");

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
        }

        private DebtData LoadDebtData()
        {
            if (!File.Exists(_debtFile))
            {
                return new DebtData { Debts = new List<Debt>() };
            }

            try
            {
                string json = File.ReadAllText(_debtFile);
                var data = JsonSerializer.Deserialize<DebtData>(json, _jsonOptions);
                return data ?? new DebtData();
            }
            catch
            {
                return new DebtData();
            }
        }

        private void SaveDebtData(DebtData data)
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(_debtFile, json);
        }

        public void AddDebt(Debt debt)
        {
            var data = LoadDebtData();
            data.Debts.Add(debt);
            SaveDebtData(data);
        }

        public void UpdateDebt(Debt debt)
        {
            var data = LoadDebtData();
            var existingDebt = data.Debts.FirstOrDefault(d => d.Id == debt.Id);
            
            if (existingDebt != null)
            {
                data.Debts.Remove(existingDebt);
                data.Debts.Add(debt);
                SaveDebtData(data);
            }
        }

        public void DeleteDebt(Debt debt)
        {
            var data = LoadDebtData();
            data.Debts.RemoveAll(d => d.Id == debt.Id);
            SaveDebtData(data);
        }

        public List<Debt> GetAllDebts()
        {
            var data = LoadDebtData();
            return data.Debts.OrderByDescending(d => d.PurchaseDate).ToList();
        }

        public List<Debt> GetDebtsByCustomer(string customerName)
        {
            var data = LoadDebtData();
            return data.Debts
                .Where(d => d.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(d => d.PurchaseDate)
                .ToList();
        }

        public void ExportDebtsToExcel(string filePath)
        {
            var debts = GetAllDebts();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Công Nợ");

                // Header
                worksheet.Cells[1, 1].Value = "Ngày mua";
                worksheet.Cells[1, 2].Value = "Tên khách hàng";
                worksheet.Cells[1, 3].Value = "Mặt hàng";
                worksheet.Cells[1, 4].Value = "Số tiền nợ (VNĐ)";
                worksheet.Cells[1, 5].Value = "Trạng thái";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(220, 53, 69));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Data
                int row = 2;
                decimal totalDebt = 0;

                foreach (var debt in debts.Where(d => !d.IsPaid))
                {
                    worksheet.Cells[row, 1].Value = debt.PurchaseDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 2].Value = debt.CustomerName;
                    worksheet.Cells[row, 3].Value = string.IsNullOrWhiteSpace(debt.ItemName) ? "" : debt.ItemName;
                    worksheet.Cells[row, 4].Value = debt.DebtAmount;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 5].Value = debt.IsPaid ? "Đã trả" : "Chưa trả";

                    if (!debt.IsPaid)
                    {
                        totalDebt += debt.DebtAmount;
                        worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 230, 230));
                    }

                    row++;
                }

                // Summary
                row++;
                worksheet.Cells[row, 1].Value = "Tổng công nợ:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = totalDebt;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Save file
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            }
        }
    }
}
