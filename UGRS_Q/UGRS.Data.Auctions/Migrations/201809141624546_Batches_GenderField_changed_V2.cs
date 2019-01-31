namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Batches_GenderField_changed_V2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Batches", "Gender", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Batches", "Gender");
        }
    }
}
