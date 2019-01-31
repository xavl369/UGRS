namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemType_SellType : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.ItemTypes", "SellType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.ItemTypes", "SellType");
        }
    }
}
