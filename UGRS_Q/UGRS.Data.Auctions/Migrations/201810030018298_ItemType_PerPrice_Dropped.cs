namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemType_PerPrice_Dropped : DbMigration
    {
        public override void Up()
        {
            DropColumn("INVENTORY.ItemTypes", "PerPrice");
        }
        
        public override void Down()
        {
            AddColumn("INVENTORY.ItemTypes", "PerPrice", c => c.Boolean(nullable: false));
        }
    }
}
