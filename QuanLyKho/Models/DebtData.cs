using System.Collections.Generic;

namespace QuanLyKho.Models
{
    public class DebtData
    {
        public List<Debt> Debts { get; set; }

        public DebtData()
        {
            Debts = new List<Debt>();
        }
    }
}
