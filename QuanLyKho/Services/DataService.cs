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
    public class DataService
    {
        private readonly string _dataFolder;
        private readonly JsonSerializerOptions _jsonOptions;

        public DataService()
        {
            _dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(_dataFolder))
            {
                Directory.CreateDirectory(_dataFolder);
            }

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private string GetFileName(int year, int month)
        {
            return Path.Combine(_dataFolder, $"{year}_{month:D2}.json");
        }

        private string GetExcelFileName(int year, int month)
        {
            return Path.Combine(_dataFolder, $"{year}_{month:D2}.xlsx");
        }

        public MonthlyData LoadMonthData(int year, int month)
        {
            string fileName = GetFileName(year, month);
            
            if (!File.Exists(fileName))
            {
                return new MonthlyData
                {
                    Year = year,
                    Month = month,
                    Transactions = new List<Transaction>()
                };
            }

            try
            {
                string json = File.ReadAllText(fileName);
                var data = JsonSerializer.Deserialize<MonthlyData>(json, _jsonOptions);
                return data ?? new MonthlyData { Year = year, Month = month };
            }
            catch
            {
                return new MonthlyData { Year = year, Month = month };
            }
        }

        public void SaveMonthData(MonthlyData data)
        {
            string fileName = GetFileName(data.Year, data.Month);
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(fileName, json);
            
            // Tự động lưu vào Excel ở thư mục Data
            string excelPath = GetExcelFileName(data.Year, data.Month);
            ExportToExcel(data.Year, data.Month, excelPath);
        }

        public void AddTransaction(Transaction transaction)
        {
            int year = transaction.Date.Year;
            int month = transaction.Date.Month;
            
            var monthData = LoadMonthData(year, month);
            monthData.Transactions.Add(transaction);
            SaveMonthData(monthData);
        }

        public void UpdateTransaction(Transaction transaction)
        {
            int year = transaction.Date.Year;
            int month = transaction.Date.Month;
            
            var monthData = LoadMonthData(year, month);
            var existingTransaction = monthData.Transactions.FirstOrDefault(t => t.Id == transaction.Id);
            
            if (existingTransaction != null)
            {
                monthData.Transactions.Remove(existingTransaction);
                monthData.Transactions.Add(transaction);
                SaveMonthData(monthData);
            }
        }

        public void DeleteTransaction(Transaction transaction)
        {
            int year = transaction.Date.Year;
            int month = transaction.Date.Month;
            
            var monthData = LoadMonthData(year, month);
            monthData.Transactions.RemoveAll(t => t.Id == transaction.Id);
            SaveMonthData(monthData);
        }

        public List<Transaction> GetTransactionsByMonth(int year, int month)
        {
            var monthData = LoadMonthData(year, month);
            return monthData.Transactions.OrderByDescending(t => t.Date).ToList();
        }

        public void ExportToExcel(int year, int month, string filePath)
        {
            var transactions = GetTransactionsByMonth(year, month);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"Tháng {month}/{year}");

                // Header
                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Loại";
                worksheet.Cells[1, 3].Value = "Đại lý";
                worksheet.Cells[1, 4].Value = "Mặt hàng";
                worksheet.Cells[1, 5].Value = "Số lượng";
                worksheet.Cells[1, 6].Value = "Số tiền (VNĐ)";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(33, 150, 243));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Data
                int row = 2;
                decimal totalImport = 0;
                decimal totalExport = 0;

                foreach (var transaction in transactions.OrderBy(t => t.Date))
                {
                    worksheet.Cells[row, 1].Value = transaction.Date.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 2].Value = transaction.Type == TransactionType.Import ? "Nhập" : "Xuất";
                    worksheet.Cells[row, 3].Value = transaction.DealerName;
                    worksheet.Cells[row, 4].Value = transaction.ItemName;
                    worksheet.Cells[row, 5].Value = transaction.Quantity;
                    worksheet.Cells[row, 6].Value = transaction.Amount;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";

                    if (transaction.Type == TransactionType.Import)
                    {
                        totalImport += transaction.Amount;
                        worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(220, 255, 220));
                    }
                    else
                    {
                        totalExport += transaction.Amount;
                        worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 240, 220));
                    }

                    row++;
                }

                // Summary
                row++;
                worksheet.Cells[row, 1].Value = "Tổng nhập kho:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = totalImport;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Color.SetColor(System.Drawing.Color.Green);

                row++;
                worksheet.Cells[row, 1].Value = "Tổng xuất kho:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = totalExport;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Color.SetColor(System.Drawing.Color.Orange);

                row++;
                worksheet.Cells[row, 1].Value = "Chênh lệch:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = totalImport - totalExport;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Save file
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            }
        }
    }
}
