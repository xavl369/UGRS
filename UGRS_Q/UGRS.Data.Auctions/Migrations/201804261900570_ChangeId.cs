namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("SYSTEM.Changes", "ObjectTypeId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("SYSTEM.Changes", "ObjectTypeId");
        }
    }
}
