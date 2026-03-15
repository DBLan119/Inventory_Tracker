using System;

namespace QuanLyKho.Models
{
    public class Debt
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string ItemName { get; set; }
        public decimal DebtAmount { get; set; }
        public bool IsPaid { get; set; }

        public Debt()
        {
            Id = Guid.NewGuid();
            PurchaseDate = DateTime.Now;
            CustomerName = string.Empty;
            ItemName = string.Empty;
            IsPaid = false;
        }
    }
}
