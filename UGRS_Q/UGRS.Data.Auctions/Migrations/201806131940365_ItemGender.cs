namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemGender : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.Items", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.Items", "Gender");
        }
    }
}
