namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "BUSINESS.PartnerCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Name = c.String(maxLength: 100, unicode: false),
                        Description = c.String(unicode: false, storeType: "text"),
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
            DropTable("BUSINESS.PartnerCategories");
        }
    }
}
