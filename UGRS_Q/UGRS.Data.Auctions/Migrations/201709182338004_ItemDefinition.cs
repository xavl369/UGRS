namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemDefinition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "INVENTORY.ItemDefinitions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        ItemId = c.Long(nullable: false),
                        ItemTypeId = c.Long(nullable: false),
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
            DropTable("INVENTORY.ItemDefinitions");
        }
    }
}
