namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemTypeOptimization : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "INVENTORY.ItemTypes", name: "ParentName_Id", newName: "ParentId");
            RenameIndex(table: "INVENTORY.ItemTypes", name: "IX_ParentName_Id", newName: "IX_ParentId");
            DropColumn("INVENTORY.ItemTypes", "Type");
            DropColumn("INVENTORY.ItemTypes", "Parent");
        }
        
        public override void Down()
        {
            AddColumn("INVENTORY.ItemTypes", "Parent", c => c.Int(nullable: false));
            AddColumn("INVENTORY.ItemTypes", "Type", c => c.Int(nullable: false));
            RenameIndex(table: "INVENTORY.ItemTypes", name: "IX_ParentId", newName: "IX_ParentName_Id");
            RenameColumn(table: "INVENTORY.ItemTypes", name: "ParentId", newName: "ParentName_Id");
        }
    }
}
