using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("Invoices", Schema = "FINANCIALS")]
    public class Invoice : DocumentEntity
    {
        #region Entity properties

        [Column(TypeName = "varchar"), StringLength(1)]
        public string DocType { get; set; }

        public int DocNum { get; set; }

        public int DocEntry { get; set; }

        public bool CreditPayment { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string CardCode { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string Series { get; set; }

        public DateTime Date { get; set; }

        public DateTime DueDate { get; set; }

        [Column(TypeName = "text")]
        public string Comments { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string NumAtCard { get; set; }

        public bool Payed { get; set; }

        public bool Payment { get; set; }

        public int PaymentCondition { get; set; }

        public string PayMethod { get; set; }

        public string MainUsage { get; set; }

        public DateTime? PayedDate { get; set; }

        #endregion

        #region External properties

        public virtual IList<InvoiceLine> Lines { get; set; }

        public long AuctionId { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        #endregion
    }
}
