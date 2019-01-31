namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerClassifications : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "BUSINESS.PartnerCategories", newName: "PartnerClassifications");
            AddColumn("BUSINESS.PartnerClassifications", "CustomerId", c => c.Long(nullable: false));
            CreateIndex("BUSINESS.PartnerClassifications", "CustomerId");
            AddForeignKey("BUSINESS.PartnerClassifications", "CustomerId", "BUSINESS.Partners", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("BUSINESS.PartnerClassifications", "CustomerId", "BUSINESS.Partners");
            DropIndex("BUSINESS.PartnerClassifications", new[] { "CustomerId" });
            DropColumn("BUSINESS.PartnerClassifications", "CustomerId");
            RenameTable(name: "BUSINESS.PartnerClassifications", newName: "PartnerCategories");
        }
    }
}
