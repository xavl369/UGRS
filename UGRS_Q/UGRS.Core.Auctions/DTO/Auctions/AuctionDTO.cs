using System;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension.Enum;

namespace UGRS.Core.Auctions.DTO.Auctions
{
    public class AuctionDTO
    {
        public long Id { get; set; }
        public string Folio { get; set; }
        public AuctionTypeEnum Type { get; set; }
        public string TypeText { get; set; }
        public AuctionCategoryEnum Category { get; set; }
        public string CategoryText { get; set; }
        public LocationEnum Location { get; set; }
        public string LocationText { get; set; }
        public float Commission { get; set; }
        public DateTime Date { get; set; }

        public AuctionDTO(Entities.Auctions.Auction pObjAuction)
        {
            Id = pObjAuction.Id;
            Folio = pObjAuction.Folio;
            Type = pObjAuction.Type;
            TypeText = pObjAuction.Type.GetDescription();
            Category = pObjAuction.Category;
            CategoryText = pObjAuction.Category.GetDescription();
            Location = pObjAuction.Location;
            LocationText = pObjAuction.Location.GetDescription();
            Commission = pObjAuction.Commission;
            Date = pObjAuction.Date;
        }

        public Entities.Auctions.Auction GetAuction()
        {
            return new Entities.Auctions.Auction()
            {
                Id = this.Id,
                Folio = this.Folio,
                Type = this.Type,
                Category = this.Category,
                Location = this.Location,
                Commission = this.Commission,
                Date = this.Date
            };
        }
    }
}
