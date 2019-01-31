namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Authorization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "SECURITY.Authorizations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Function = c.Int(nullable: false),
                        Comment = c.String(maxLength: 500, unicode: false),
                        BatchId = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Batches", t => t.BatchId, cascadeDelete: true)
                .ForeignKey("USERS.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.BatchId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("SECURITY.Authorizations", "UserId", "USERS.Users");
            DropForeignKey("SECURITY.Authorizations", "BatchId", "AUCTIONS.Batches");
            DropIndex("SECURITY.Authorizations", new[] { "UserId" });
            DropIndex("SECURITY.Authorizations", new[] { "BatchId" });
            DropTable("SECURITY.Authorizations");
        }
    }
}
