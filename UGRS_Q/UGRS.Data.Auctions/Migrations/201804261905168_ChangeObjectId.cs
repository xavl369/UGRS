namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeObjectId : DbMigration
    {
        public override void Up()
        {
            AddColumn("SYSTEM.Changes", "ObjectId", c => c.Long(nullable: false));
            DropColumn("SYSTEM.Changes", "ObjectTypeId");
        }
        
        public override void Down()
        {
            AddColumn("SYSTEM.Changes", "ObjectTypeId", c => c.Long(nullable: false));
            DropColumn("SYSTEM.Changes", "ObjectId");
        }
    }
}
