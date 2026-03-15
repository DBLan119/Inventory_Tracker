using System;

namespace QuanLyKho.Models
{
    public enum TransactionType
    {
        Import,  // Nhập kho
        Export   // Xuất kho
    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public string DealerName { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }

        public Transaction()
        {
            Id = Guid.NewGuid();
            Date = DateTime.Now;
        }
    }
}
