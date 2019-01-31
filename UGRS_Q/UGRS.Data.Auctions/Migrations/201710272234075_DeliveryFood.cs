namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeliveryFood : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FINANCIALS.DeliveriesFood",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DocType = c.String(maxLength: 1, unicode: false),
                        DocNum = c.Int(nullable: false),
                        DocEntry = c.Int(nullable: false),
                        CardCode = c.String(maxLength: 50, unicode: false),
                        LineNum = c.Int(nullable: false),
                        WhsCode = c.String(maxLength: 50, unicode: false),
                        ItemCode = c.String(maxLength: 50, unicode: false),
                        Quantity = c.Double(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Opened = c.Boolean(nullable: false),
                        Canceled = c.Boolean(nullable: false),
                        Processed = c.Boolean(nullable: false),
                        ProcessedDate = c.DateTime(),
                        Exported = c.Boolean(nullable: false),
                        ExportDate = c.DateTime(),
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
            DropTable("FINANCIALS.DeliveriesFood");
        }
    }
}
