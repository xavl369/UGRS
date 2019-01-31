namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Items_level : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.Items", "Level", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.Items", "Level");
        }
    }
}
