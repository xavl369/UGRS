namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerMapping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "BUSINESS.PartnerMappings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Exists = c.Boolean(nullable: false),
                        PartnerId = c.Long(nullable: false),
                        NewPartnerId = c.Long(),
                        Autorized = c.Boolean(nullable: false),
                        AutorizedByUserId = c.Long(nullable: false),
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
            DropTable("BUSINESS.PartnerMappings");
        }
    }
}
