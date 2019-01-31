namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateNulls : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "SYSTEM.Configurations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Key = c.Int(nullable: false),
                        Value = c.String(unicode: false, storeType: "text"),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("INVENTORY.Stocks", "CurrentWarehouse", c => c.String(maxLength: 50, unicode: false));
            AddColumn("INVENTORY.Stocks", "InitialWarehouse", c => c.String(maxLength: 50, unicode: false));
            AddColumn("INVENTORY.Stocks", "ChargeFood", c => c.Boolean(nullable: false));
            AlterColumn("AUCTIONS.Auctions", "ProcessedDate", c => c.DateTime());
            AlterColumn("AUCTIONS.Auctions", "ExportDate", c => c.DateTime());
            AlterColumn("AUCTIONS.Batches", "ExportDate", c => c.DateTime());
            AlterColumn("BUSINESS.Partners", "SyncDate", c => c.DateTime());
            AlterColumn("INVENTORY.GoodsIssues", "ProcessedDate", c => c.DateTime());
            AlterColumn("INVENTORY.GoodsIssues", "ExportDate", c => c.DateTime());
            AlterColumn("AUCTIONS.BatchLines", "ExportDate", c => c.DateTime());
            AlterColumn("INVENTORY.GoodsReceipts", "ProcessedDate", c => c.DateTime());
            AlterColumn("INVENTORY.GoodsReceipts", "ExportDate", c => c.DateTime());
            AlterColumn("INVENTORY.GoodsReturns", "ProcessedDate", c => c.DateTime());
            AlterColumn("INVENTORY.GoodsReturns", "ExportDate", c => c.DateTime());
            AlterColumn("FINANCIALS.Invoices", "ProcessedDate", c => c.DateTime());
            AlterColumn("FINANCIALS.Invoices", "ExportDate", c => c.DateTime());
            AlterColumn("FINANCIALS.InvoiceLines", "ExportDate", c => c.DateTime());
            AlterColumn("FINANCIALS.JournalEntries", "ProcessedDate", c => c.DateTime());
            AlterColumn("FINANCIALS.JournalEntries", "ExportDate", c => c.DateTime());
            AlterColumn("FINANCIALS.JournalEntryLines", "ExportDate", c => c.DateTime());
            AlterColumn("FINANCIALS.FoodCharges", "ProcessedDate", c => c.DateTime());
            AlterColumn("FINANCIALS.FoodCharges", "ExportDate", c => c.DateTime());
            AlterColumn("FINANCIALS.GuideCharges", "ProcessedDate", c => c.DateTime());
            AlterColumn("FINANCIALS.GuideCharges", "ExportDate", c => c.DateTime());
            AlterColumn("BUSINESS.PartnerMappings", "ExportDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("BUSINESS.PartnerMappings", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.GuideCharges", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.GuideCharges", "ProcessedDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.FoodCharges", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.FoodCharges", "ProcessedDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.JournalEntryLines", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.JournalEntries", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.JournalEntries", "ProcessedDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.InvoiceLines", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.Invoices", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("FINANCIALS.Invoices", "ProcessedDate", c => c.DateTime(nullable: false));
            AlterColumn("INVENTORY.GoodsReturns", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("INVENTORY.GoodsReturns", "ProcessedDate", c => c.DateTime(nullable: false));
            AlterColumn("INVENTORY.GoodsReceipts", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("INVENTORY.GoodsReceipts", "ProcessedDate", c => c.DateTime(nullable: false));
            AlterColumn("AUCTIONS.BatchLines", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("INVENTORY.GoodsIssues", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("INVENTORY.GoodsIssues", "ProcessedDate", c => c.DateTime(nullable: false));
            AlterColumn("BUSINESS.Partners", "SyncDate", c => c.DateTime(nullable: false));
            AlterColumn("AUCTIONS.Batches", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("AUCTIONS.Auctions", "ExportDate", c => c.DateTime(nullable: false));
            AlterColumn("AUCTIONS.Auctions", "ProcessedDate", c => c.DateTime(nullable: false));
            DropColumn("INVENTORY.Stocks", "ChargeFood");
            DropColumn("INVENTORY.Stocks", "InitialWarehouse");
            DropColumn("INVENTORY.Stocks", "CurrentWarehouse");
            DropTable("SYSTEM.Configurations");
        }
    }
}
