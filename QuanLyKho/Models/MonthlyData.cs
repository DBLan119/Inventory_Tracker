using System.Collections.Generic;

namespace QuanLyKho.Models
{
    public class MonthlyData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<Transaction> Transactions { get; set; }

        public MonthlyData()
        {
            Transactions = new List<Transaction>();
        }
    }
}
