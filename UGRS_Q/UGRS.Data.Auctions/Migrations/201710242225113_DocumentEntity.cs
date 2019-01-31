namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Auctions", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("AUCTIONS.Auctions", "Processed", c => c.Boolean(nullable: false));
            AddColumn("AUCTIONS.Auctions", "ProcessedDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "Opened", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "Processed", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "ProcessedDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "Opened", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "Processed", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "ProcessedDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "Opened", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "Processed", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "ProcessedDate", c => c.DateTime(nullable: false));
            AddColumn("FINANCIALS.Invoices", "Opened", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.Invoices", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.Invoices", "Processed", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.Invoices", "ProcessedDate", c => c.DateTime(nullable: false));
            AddColumn("FINANCIALS.JournalEntries", "Opened", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.JournalEntries", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.JournalEntries", "Processed", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.JournalEntries", "ProcessedDate", c => c.DateTime(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "Opened", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "Processed", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "ProcessedDate", c => c.DateTime(nullable: false));
            AddColumn("FINANCIALS.GuideCharges", "Opened", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.GuideCharges", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.GuideCharges", "Processed", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.GuideCharges", "ProcessedDate", c => c.DateTime(nullable: false));
            DropColumn("INVENTORY.Stocks", "Temporary");
            DropColumn("INVENTORY.Stocks", "SyncDate");
        }
        
        public override void Down()
        {
            AddColumn("INVENTORY.Stocks", "SyncDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.Stocks", "Temporary", c => c.Boolean(nullable: false));
            DropColumn("FINANCIALS.GuideCharges", "ProcessedDate");
            DropColumn("FINANCIALS.GuideCharges", "Processed");
            DropColumn("FINANCIALS.GuideCharges", "Canceled");
            DropColumn("FINANCIALS.GuideCharges", "Opened");
            DropColumn("FINANCIALS.FoodCharges", "ProcessedDate");
            DropColumn("FINANCIALS.FoodCharges", "Processed");
            DropColumn("FINANCIALS.FoodCharges", "Canceled");
            DropColumn("FINANCIALS.FoodCharges", "Opened");
            DropColumn("FINANCIALS.JournalEntries", "ProcessedDate");
            DropColumn("FINANCIALS.JournalEntries", "Processed");
            DropColumn("FINANCIALS.JournalEntries", "Canceled");
            DropColumn("FINANCIALS.JournalEntries", "Opened");
            DropColumn("FINANCIALS.Invoices", "ProcessedDate");
            DropColumn("FINANCIALS.Invoices", "Processed");
            DropColumn("FINANCIALS.Invoices", "Canceled");
            DropColumn("FINANCIALS.Invoices", "Opened");
            DropColumn("INVENTORY.GoodsReturns", "ProcessedDate");
            DropColumn("INVENTORY.GoodsReturns", "Processed");
            DropColumn("INVENTORY.GoodsReturns", "Canceled");
            DropColumn("INVENTORY.GoodsReturns", "Opened");
            DropColumn("INVENTORY.GoodsReceipts", "ProcessedDate");
            DropColumn("INVENTORY.GoodsReceipts", "Processed");
            DropColumn("INVENTORY.GoodsReceipts", "Canceled");
            DropColumn("INVENTORY.GoodsReceipts", "Opened");
            DropColumn("INVENTORY.GoodsIssues", "ProcessedDate");
            DropColumn("INVENTORY.GoodsIssues", "Processed");
            DropColumn("INVENTORY.GoodsIssues", "Canceled");
            DropColumn("INVENTORY.GoodsIssues", "Opened");
            DropColumn("AUCTIONS.Auctions", "ProcessedDate");
            DropColumn("AUCTIONS.Auctions", "Processed");
            DropColumn("AUCTIONS.Auctions", "Canceled");
        }
    }
}
