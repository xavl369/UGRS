namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OptionalForeignName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("BUSINESS.Partners", "ForeignName", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("BUSINESS.Partners", "ForeignName", c => c.String(nullable: false, maxLength: 100, unicode: false));
        }
    }
}
