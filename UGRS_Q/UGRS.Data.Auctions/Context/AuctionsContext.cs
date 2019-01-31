using System.Data.Entity;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Entities.Security;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Entities.Users;


namespace UGRS.Data.Auctions.Context
{
    public class AuctionsContext : DbContext
    {
        //USERS
        public DbSet<User> User { get; set; }
        public DbSet<UserType> UserType { get; set; }

        //SYSTEM
        public DbSet<Core.Auctions.Entities.System.Module> Module { get; set; }
        public DbSet<Section> Section { get; set; }
        public DbSet<Change> Change { get; set; }
        public DbSet<Configuration> Configuration { get; set; }

        //SECURITY
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Authorization> Authorization { get; set; }

        //AUCTIONS
        public DbSet<Auction> Auction { get; set; }
        public DbSet<Batch> Batch { get; set; }
        public DbSet<BatchLine> BatchLine { get; set; }
        
        //BUSINESS
        public DbSet<Partner> Partner { get; set; }
        public DbSet<PartnerClassification> PartnerCategory { get; set; }
        public DbSet<PartnerMapping> PartnerMapping { get; set; }

        //INVENTORY
        public DbSet<Stock> Stock { get; set; }
        public DbSet<GoodsIssue> GoodsIssue { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipt { get; set; }
        public DbSet<GoodsReturn> GoodsReturn { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<ItemType> ItemType { get; set; }
        public DbSet<ItemDefinition> ItemDefinition { get; set; }
        public DbSet<ItemTypeDefinition> ItemTypeDefinition { get; set; }

        //FINANCIALS
        public DbSet<FoodDelivery> FoodDelivery { get; set; }
        public DbSet<GuideCharge> GuideCharge { get; set; }
        public DbSet<FoodCharge> FoodCharge { get; set; }
        public DbSet<FoodChargeLine> FoodChargeLine { get; set; }
        public DbSet<FoodChargeCheck> FoodChargeCheck { get; set; }
        public DbSet<DeductionCheck> DeductionCheck { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceLine> InvoiceLine { get; set; }
        public DbSet<JournalEntry> JournalEntry { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLine { get; set; }

        //TRADES
        public DbSet<Trade> Trade { get; set; }

        public AuctionsContext() : 
            base("name=UGRS_Subastas_Com")
        { 
        
        }

        protected override void OnModelCreating(DbModelBuilder pObjModelBuilder)
        {
            pObjModelBuilder.Entity<Batch>()
                .HasOptional(b => b.Seller)
                .WithMany(s => s.SelledBatches)
                .HasForeignKey(f => f.SellerId)
                .WillCascadeOnDelete(false);

            pObjModelBuilder.Entity<Batch>()
                .HasOptional(b => b.Buyer)
                .WithMany(s => s.BuyedBatches)
                .HasForeignKey(f => f.BuyerId)
                .WillCascadeOnDelete(false);

            pObjModelBuilder.Entity<Batch>()
                .HasOptional(b => b.BuyerClassification);

            pObjModelBuilder.Entity<ItemType>()
                .HasOptional(x => x.Parent);

            pObjModelBuilder.Entity<GoodsIssue>()
                .HasRequired(b => b.Batch)
                .WithMany(s => s.GoodsIssues)
                .HasForeignKey(f => f.BatchId)
                .WillCascadeOnDelete(false);

            pObjModelBuilder.Entity<GoodsReturn>()
                .HasRequired(b => b.Batch)
                .WithMany(s => s.GoodsReturns)
                .HasForeignKey(f => f.BatchId)
                .WillCascadeOnDelete(false);

            pObjModelBuilder.Entity<PartnerClassification>()
                .HasRequired(b => b.Customer)
                .WithMany(s => s.Classifications)
                .HasForeignKey(f => f.CustomerId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(pObjModelBuilder);
        }
    }
}

