namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Auctions_CostingCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Auctions", "CostingCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Auctions", "CostingCode");
        }
    }
}
