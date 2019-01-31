namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabaseVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "AUCTIONS.Auctions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Folio = c.String(nullable: false, maxLength: 20, unicode: false),
                        Commission = c.Single(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Opened = c.Boolean(nullable: false),
                        LocationId = c.Long(nullable: false),
                        TypeId = c.Long(nullable: false),
                        CategoryId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("AUCTIONS.Locations", t => t.LocationId, cascadeDelete: true)
                .ForeignKey("AUCTIONS.Types", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.LocationId)
                .Index(t => t.TypeId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "AUCTIONS.Batches",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Weight = c.Single(nullable: false),
                        AverageWeight = c.Single(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unsold = c.Boolean(nullable: false),
                        UnsoldMotive = c.Int(nullable: false),
                        SellerId = c.Long(nullable: false),
                        BuyerId = c.Long(),
                        ItemId = c.Long(nullable: false),
                        ItemTypeId = c.Long(nullable: false),
                        AuctionId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Auctions", t => t.AuctionId, cascadeDelete: true)
                .ForeignKey("INVENTORY.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("BUSINESS.Partners", t => t.BuyerId)
                .ForeignKey("INVENTORY.ItemTypes", t => t.ItemTypeId, cascadeDelete: true)
                .ForeignKey("BUSINESS.Partners", t => t.SellerId)
                .Index(t => t.SellerId)
                .Index(t => t.BuyerId)
                .Index(t => t.ItemId)
                .Index(t => t.ItemTypeId)
                .Index(t => t.AuctionId);
            
            CreateTable(
                "BUSINESS.Partners",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 15, unicode: false),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        TaxCode = c.String(maxLength: 32, unicode: false),
                        PartnerStatus = c.Int(nullable: false),
                        Temporary = c.Boolean(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "INVENTORY.GoodsIssues",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Temporary = c.Boolean(nullable: false),
                        CustomerId = c.Long(nullable: false),
                        ItemId = c.Long(nullable: false),
                        BatchId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Batches", t => t.BatchId)
                .ForeignKey("BUSINESS.Partners", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("INVENTORY.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.ItemId)
                .Index(t => t.BatchId);
            
            CreateTable(
                "INVENTORY.Items",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50, unicode: false),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        ItemStatus = c.Int(nullable: false),
                        Temporary = c.Boolean(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "INVENTORY.GoodsReceipts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Folio = c.String(maxLength: 50, unicode: false),
                        Quantity = c.Int(nullable: false),
                        Temporary = c.Boolean(nullable: false),
                        CustomerId = c.Long(nullable: false),
                        ItemId = c.Long(nullable: false),
                        BatchId = c.Long(),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Batches", t => t.BatchId)
                .ForeignKey("BUSINESS.Partners", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("INVENTORY.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.ItemId)
                .Index(t => t.BatchId);
            
            CreateTable(
                "INVENTORY.GoodsReturns",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Temporary = c.Boolean(nullable: false),
                        CustomerId = c.Long(nullable: false),
                        ItemId = c.Long(nullable: false),
                        BatchId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Batches", t => t.BatchId)
                .ForeignKey("BUSINESS.Partners", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("INVENTORY.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.ItemId)
                .Index(t => t.BatchId);
            
            CreateTable(
                "INVENTORY.ItemTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50, unicode: false),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        PerPrice = c.Boolean(nullable: false),
                        Level = c.Int(nullable: false),
                        Gender = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Parent = c.Int(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ParentName_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("INVENTORY.ItemTypes", t => t.ParentName_Id)
                .Index(t => t.ParentName_Id);
            
            CreateTable(
                "AUCTIONS.Categories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, unicode: false),
                        Description = c.String(maxLength: 100, unicode: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "AUCTIONS.Locations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Abbreviation = c.String(maxLength: 5, unicode: false),
                        Name = c.String(maxLength: 100, unicode: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "AUCTIONS.Types",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, unicode: false),
                        Description = c.String(maxLength: 250, unicode: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "SYSTEM.Changes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ChangeType = c.Int(nullable: false),
                        ObjectType = c.String(nullable: false, unicode: false, storeType: "text"),
                        Object = c.String(nullable: false, unicode: false, storeType: "text"),
                        Date = c.DateTime(nullable: false),
                        UserId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "SYSTEM.Modules",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Icon = c.String(maxLength: 50, unicode: false),
                        Path = c.String(maxLength: 100, unicode: false),
                        Name = c.String(maxLength: 100, unicode: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "SYSTEM.Sections",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Path = c.String(maxLength: 100, unicode: false),
                        ModuleId = c.Long(nullable: false),
                        Name = c.String(maxLength: 100, unicode: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("SYSTEM.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "SECURITY.Permissions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PermissionType = c.Int(nullable: false),
                        PermissionId = c.Long(nullable: false),
                        AccessType = c.Int(nullable: false),
                        AccessId = c.Long(nullable: false),
                        AllowAccess = c.Boolean(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "INVENTORY.Stocks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BatchNumber = c.String(nullable: false, maxLength: 100, unicode: false),
                        Quantity = c.Int(nullable: false),
                        Temporary = c.Boolean(nullable: false),
                        CustomerId = c.Long(nullable: false),
                        ItemId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BUSINESS.Partners", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("INVENTORY.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.ItemId);
            
            CreateTable(
                "USERS.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50, unicode: false),
                        EmailAddress = c.String(nullable: false, maxLength: 50, unicode: false),
                        Password = c.String(unicode: false, storeType: "text"),
                        FirstName = c.String(nullable: false, maxLength: 50, unicode: false),
                        LastName = c.String(nullable: false, maxLength: 50, unicode: false),
                        UserTypeId = c.Long(nullable: false),
                        Image = c.String(unicode: false, storeType: "text"),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("USERS.UserTypes", t => t.UserTypeId, cascadeDelete: true)
                .Index(t => t.UserTypeId);
            
            CreateTable(
                "USERS.UserTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 250, unicode: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("USERS.Users", "UserTypeId", "USERS.UserTypes");
            DropForeignKey("INVENTORY.Stocks", "ItemId", "INVENTORY.Items");
            DropForeignKey("INVENTORY.Stocks", "CustomerId", "BUSINESS.Partners");
            DropForeignKey("SYSTEM.Sections", "ModuleId", "SYSTEM.Modules");
            DropForeignKey("AUCTIONS.Auctions", "TypeId", "AUCTIONS.Types");
            DropForeignKey("AUCTIONS.Auctions", "LocationId", "AUCTIONS.Locations");
            DropForeignKey("AUCTIONS.Auctions", "CategoryId", "AUCTIONS.Categories");
            DropForeignKey("AUCTIONS.Batches", "SellerId", "BUSINESS.Partners");
            DropForeignKey("AUCTIONS.Batches", "ItemTypeId", "INVENTORY.ItemTypes");
            DropForeignKey("INVENTORY.ItemTypes", "ParentName_Id", "INVENTORY.ItemTypes");
            DropForeignKey("AUCTIONS.Batches", "BuyerId", "BUSINESS.Partners");
            DropForeignKey("INVENTORY.GoodsReturns", "ItemId", "INVENTORY.Items");
            DropForeignKey("INVENTORY.GoodsReturns", "CustomerId", "BUSINESS.Partners");
            DropForeignKey("INVENTORY.GoodsReturns", "BatchId", "AUCTIONS.Batches");
            DropForeignKey("INVENTORY.GoodsReceipts", "ItemId", "INVENTORY.Items");
            DropForeignKey("INVENTORY.GoodsReceipts", "CustomerId", "BUSINESS.Partners");
            DropForeignKey("INVENTORY.GoodsReceipts", "BatchId", "AUCTIONS.Batches");
            DropForeignKey("INVENTORY.GoodsIssues", "ItemId", "INVENTORY.Items");
            DropForeignKey("AUCTIONS.Batches", "ItemId", "INVENTORY.Items");
            DropForeignKey("INVENTORY.GoodsIssues", "CustomerId", "BUSINESS.Partners");
            DropForeignKey("INVENTORY.GoodsIssues", "BatchId", "AUCTIONS.Batches");
            DropForeignKey("AUCTIONS.Batches", "AuctionId", "AUCTIONS.Auctions");
            DropIndex("USERS.Users", new[] { "UserTypeId" });
            DropIndex("INVENTORY.Stocks", new[] { "ItemId" });
            DropIndex("INVENTORY.Stocks", new[] { "CustomerId" });
            DropIndex("SYSTEM.Sections", new[] { "ModuleId" });
            DropIndex("INVENTORY.ItemTypes", new[] { "ParentName_Id" });
            DropIndex("INVENTORY.GoodsReturns", new[] { "BatchId" });
            DropIndex("INVENTORY.GoodsReturns", new[] { "ItemId" });
            DropIndex("INVENTORY.GoodsReturns", new[] { "CustomerId" });
            DropIndex("INVENTORY.GoodsReceipts", new[] { "BatchId" });
            DropIndex("INVENTORY.GoodsReceipts", new[] { "ItemId" });
            DropIndex("INVENTORY.GoodsReceipts", new[] { "CustomerId" });
            DropIndex("INVENTORY.GoodsIssues", new[] { "BatchId" });
            DropIndex("INVENTORY.GoodsIssues", new[] { "ItemId" });
            DropIndex("INVENTORY.GoodsIssues", new[] { "CustomerId" });
            DropIndex("AUCTIONS.Batches", new[] { "AuctionId" });
            DropIndex("AUCTIONS.Batches", new[] { "ItemTypeId" });
            DropIndex("AUCTIONS.Batches", new[] { "ItemId" });
            DropIndex("AUCTIONS.Batches", new[] { "BuyerId" });
            DropIndex("AUCTIONS.Batches", new[] { "SellerId" });
            DropIndex("AUCTIONS.Auctions", new[] { "CategoryId" });
            DropIndex("AUCTIONS.Auctions", new[] { "TypeId" });
            DropIndex("AUCTIONS.Auctions", new[] { "LocationId" });
            DropTable("USERS.UserTypes");
            DropTable("USERS.Users");
            DropTable("INVENTORY.Stocks");
            DropTable("SECURITY.Permissions");
            DropTable("SYSTEM.Sections");
            DropTable("SYSTEM.Modules");
            DropTable("SYSTEM.Changes");
            DropTable("AUCTIONS.Types");
            DropTable("AUCTIONS.Locations");
            DropTable("AUCTIONS.Categories");
            DropTable("INVENTORY.ItemTypes");
            DropTable("INVENTORY.GoodsReturns");
            DropTable("INVENTORY.GoodsReceipts");
            DropTable("INVENTORY.Items");
            DropTable("INVENTORY.GoodsIssues");
            DropTable("BUSINESS.Partners");
            DropTable("AUCTIONS.Batches");
            DropTable("AUCTIONS.Auctions");
        }
    }
}
