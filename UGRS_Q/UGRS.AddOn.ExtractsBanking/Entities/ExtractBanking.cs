using System;

namespace UGRS.AddOn.ExtractsBanking.Entities
{
    public class ExtractBanking
    {
        public string AccountCode { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string Detail { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
    }
}
