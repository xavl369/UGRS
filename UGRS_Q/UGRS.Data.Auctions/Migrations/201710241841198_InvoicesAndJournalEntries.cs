namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoicesAndJournalEntries : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "AUCTIONS.FoodCharge", newName: "FoodCharges");
            MoveTable(name: "AUCTIONS.FoodCharges", newSchema: "FINANCIALS");
            CreateTable(
                "FINANCIALS.Invoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DocType = c.String(maxLength: 1, unicode: false),
                        DocNum = c.Int(nullable: false),
                        DocEntry = c.Int(nullable: false),
                        CardCode = c.String(maxLength: 50, unicode: false),
                        Series = c.String(maxLength: 50, unicode: false),
                        Date = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Comments = c.String(unicode: false, storeType: "text"),
                        NumAtCard = c.String(maxLength: 50, unicode: false),
                        AuctionId = c.Long(nullable: false),
                        Exported = c.Boolean(nullable: false),
                        ExportDate = c.DateTime(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Auctions", t => t.AuctionId, cascadeDelete: true)
                .Index(t => t.AuctionId);
            
            CreateTable(
                "FINANCIALS.InvoiceLines",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LineNum = c.Int(nullable: false),
                        ItemCode = c.String(maxLength: 50, unicode: false),
                        TaxCode = c.String(maxLength: 50, unicode: false),
                        WarehouseCode = c.String(maxLength: 50, unicode: false),
                        Quantity = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        CostingCode = c.String(maxLength: 50, unicode: false),
                        InvoiceId = c.Long(nullable: false),
                        Exported = c.Boolean(nullable: false),
                        ExportDate = c.DateTime(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("FINANCIALS.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "FINANCIALS.JournalEntries",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Series = c.String(maxLength: 50, unicode: false),
                        Remarks = c.String(unicode: false, storeType: "text"),
                        Reference = c.String(maxLength: 50, unicode: false),
                        AuctionId = c.Long(nullable: false),
                        Exported = c.Boolean(nullable: false),
                        ExportDate = c.DateTime(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Auctions", t => t.AuctionId, cascadeDelete: true)
                .Index(t => t.AuctionId);
            
            CreateTable(
                "FINANCIALS.JournalEntryLines",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LineNum = c.Int(nullable: false),
                        AccountCode = c.String(maxLength: 50, unicode: false),
                        AccountName = c.String(maxLength: 100, unicode: false),
                        Debit = c.Double(nullable: false),
                        Credit = c.Double(nullable: false),
                        Remarks = c.String(unicode: false, storeType: "text"),
                        Reference = c.String(maxLength: 50, unicode: false),
                        JournalEntryId = c.Long(nullable: false),
                        Exported = c.Boolean(nullable: false),
                        ExportDate = c.DateTime(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("FINANCIALS.JournalEntries", t => t.JournalEntryId, cascadeDelete: true)
                .Index(t => t.JournalEntryId);
            
            AddColumn("AUCTIONS.Auctions", "Exported", c => c.Boolean(nullable: false));
            AddColumn("AUCTIONS.Auctions", "ExportDate", c => c.DateTime(nullable: false));
            AddColumn("AUCTIONS.Batches", "Exported", c => c.Boolean(nullable: false));
            AddColumn("AUCTIONS.Batches", "ExportDate", c => c.DateTime(nullable: false));
            AddColumn("BUSINESS.Partners", "SyncDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "Exported", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "ExportDate", c => c.DateTime(nullable: false));
            AddColumn("AUCTIONS.BatchLines", "Exported", c => c.Boolean(nullable: false));
            AddColumn("AUCTIONS.BatchLines", "ExportDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "Exported", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "ExportDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "Exported", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "ExportDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.Stocks", "SyncDate", c => c.DateTime(nullable: false));
            DropColumn("INVENTORY.GoodsIssues", "Temporary");
            DropColumn("INVENTORY.Items", "Temporary");
            DropColumn("INVENTORY.GoodsReceipts", "Temporary");
            DropColumn("INVENTORY.GoodsReturns", "Temporary");
        }
        
        public override void Down()
        {
            AddColumn("INVENTORY.GoodsReturns", "Temporary", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "Temporary", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.Items", "Temporary", c => c.Boolean(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "Temporary", c => c.Boolean(nullable: false));
            DropForeignKey("FINANCIALS.JournalEntryLines", "JournalEntryId", "FINANCIALS.JournalEntries");
            DropForeignKey("FINANCIALS.JournalEntries", "AuctionId", "AUCTIONS.Auctions");
            DropForeignKey("FINANCIALS.InvoiceLines", "InvoiceId", "FINANCIALS.Invoices");
            DropForeignKey("FINANCIALS.Invoices", "AuctionId", "AUCTIONS.Auctions");
            DropIndex("FINANCIALS.JournalEntryLines", new[] { "JournalEntryId" });
            DropIndex("FINANCIALS.JournalEntries", new[] { "AuctionId" });
            DropIndex("FINANCIALS.InvoiceLines", new[] { "InvoiceId" });
            DropIndex("FINANCIALS.Invoices", new[] { "AuctionId" });
            DropColumn("INVENTORY.Stocks", "SyncDate");
            DropColumn("INVENTORY.GoodsReturns", "ExportDate");
            DropColumn("INVENTORY.GoodsReturns", "Exported");
            DropColumn("INVENTORY.GoodsReceipts", "ExportDate");
            DropColumn("INVENTORY.GoodsReceipts", "Exported");
            DropColumn("AUCTIONS.BatchLines", "ExportDate");
            DropColumn("AUCTIONS.BatchLines", "Exported");
            DropColumn("INVENTORY.GoodsIssues", "ExportDate");
            DropColumn("INVENTORY.GoodsIssues", "Exported");
            DropColumn("BUSINESS.Partners", "SyncDate");
            DropColumn("AUCTIONS.Batches", "ExportDate");
            DropColumn("AUCTIONS.Batches", "Exported");
            DropColumn("AUCTIONS.Auctions", "ExportDate");
            DropColumn("AUCTIONS.Auctions", "Exported");
            DropTable("FINANCIALS.JournalEntryLines");
            DropTable("FINANCIALS.JournalEntries");
            DropTable("FINANCIALS.InvoiceLines");
            DropTable("FINANCIALS.Invoices");
            MoveTable(name: "FINANCIALS.FoodCharges", newSchema: "AUCTIONS");
            RenameTable(name: "AUCTIONS.FoodCharges", newName: "FoodCharge");
        }
    }
}
