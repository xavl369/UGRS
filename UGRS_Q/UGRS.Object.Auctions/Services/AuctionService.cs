using System;
using System.Linq;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.DAO.Base;
using UGRS.Core.Extension.Enum;
using System.Data.Entity;

namespace UGRS.Object.Auctions.Services
{
    public class AuctionService
    {
        #region Attributes

        UGRS.Core.SDK.DI.Auctions.Services.AuctionService mObjSapAuctionService;
        UGRS.Core.Auctions.Services.Auctions.AuctionService mObjLocalAuctionService;
        SAPbobsCOM.UserTable mObjsboTable;

        #endregion

        #region Properties

        public UGRS.Core.SDK.DI.Auctions.Services.AuctionService SapAuctionService
        {
            get { return mObjSapAuctionService; }
            set { mObjSapAuctionService = value; }
        }

        public UGRS.Core.Auctions.Services.Auctions.AuctionService LocalAuctionService
        {
            get { return mObjLocalAuctionService; }
            set { mObjLocalAuctionService = value; }
        }

        #endregion

        #region Contructor

        public AuctionService()
        {
            SapAuctionService = new UGRS.Core.SDK.DI.Auctions.Services.AuctionService();
            LocalAuctionService = new UGRS.Core.Auctions.Services.Auctions.AuctionService(new BaseDAO<UGRS.Core.Auctions.Entities.Auctions.Auction>());
        }

        #endregion

        #region Methods

        public void ExportAuctions(string pStrLocation)
        {
            DateTime lDtmLastCreationDate = GetLastCreationDate(pStrLocation);
            var ded = LocalAuctionService.GetListFilteredByCC().ToList();
            foreach (UGRS.Core.Auctions.Entities.Auctions.Auction lObjAcution in LocalAuctionService.GetListFilteredByCC().Where(x => x.CreationDate >= lDtmLastCreationDate).ToList())
            {
                if (!SapAuctionService.HasBeenImported(lObjAcution.Folio))
                {
                    ExportAuction(lObjAcution);
                }
            }
        }

        public void UpdateAuctions(string pStrLocation)
        {
            
            DateTime lDtmLastModificationDate = GetLastModificationDate(pStrLocation);
            foreach (UGRS.Core.Auctions.Entities.Auctions.Auction lObjAcution in LocalAuctionService.GetListFilteredByCC().Where(x => x.ModificationDate >= lDtmLastModificationDate).ToList())
            {
                if (SapAuctionService.HasBeenUpdated(lObjAcution.Folio, lObjAcution.ModificationDate))
                {
                    UpdateAuction(lObjAcution);
                }
            }
        }

        private void ExportAuction(UGRS.Core.Auctions.Entities.Auctions.Auction pObjAuction)
        {
            try
            {
                if (SapAuctionService.Add(GetSAPAuction(pObjAuction)) != 0)
                {
                    LogUtility.Write(string.Format("[ERROR] {0}", DIApplication.Company.GetLastErrorDescription()));
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private void UpdateAuction(UGRS.Core.Auctions.Entities.Auctions.Auction pObjAuction)
        {
            try
            {
                if (SapAuctionService.Update(GetSAPAuction(pObjAuction, true)) != 0)
                {
                    LogUtility.Write(string.Format("[ERROR] {0}", DIApplication.Company.GetLastErrorDescription()));
                }
                else
                {
                    //Modify Modification Date for current auction
                    pObjAuction.ModificationDate = DateTime.Now;
                    LocalAuctionService.SaveOrUpdate(pObjAuction);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private UGRS.Core.SDK.DI.Auctions.Tables.Auction GetSAPAuction(UGRS.Core.Auctions.Entities.Auctions.Auction pObjAuction, bool pBoolToUpdate = false)
        {

            if (pBoolToUpdate)
            {
                mObjsboTable = null;
                mObjsboTable = (SAPbobsCOM.UserTable)DIApplication.Company.UserTables.Item("UG_SU_AUTN");
                mObjsboTable.GetByKey(GetKeyByFolio(pObjAuction.Folio));
            }

           
            return new UGRS.Core.SDK.DI.Auctions.Tables.Auction()
            {
                //Id = pObjAuction.Id,
                Id = !pBoolToUpdate ? GetNextId() : GetAuctionId(pObjAuction.Folio),
                LocationId = (int)pObjAuction.Location,
                Location = pObjAuction.CostingCode,
                Folio = pObjAuction.Folio,
                TypeId = (int)pObjAuction.Type,
                Type = pObjAuction.Type.GetDescription(),
                CategoryId = (int)pObjAuction.Category,
                Category = pObjAuction.Category.GetDescription(),
                Commission = pObjAuction.Commission,
                Date = pObjAuction.Date,
                Opened = pObjAuction.Opened,
                Protected = pObjAuction.Protected,
                Removed = pObjAuction.Removed,
                Active = pObjAuction.Active,
                CreationDate = pObjAuction.CreationDate,
                CreationTime = pObjAuction.CreationDate,
                ModificationDate = DateTime.Now,
                ModificationTime = DateTime.Now,
                AutAuction = !pBoolToUpdate ? false : mObjsboTable.UserFields.Fields.Item("U_AutAuction").Value.ToString() == "Y" ? true : false,
                AutCorral = !pBoolToUpdate ? false : mObjsboTable.UserFields.Fields.Item("U_AutCorral").Value.ToString() == "Y" ? true : false,
                AutCyC = !pBoolToUpdate ? false : mObjsboTable.UserFields.Fields.Item("U_AutCyC").Value.ToString() == "Y" ? true : false,
                AutFz = !pBoolToUpdate ? false : mObjsboTable.UserFields.Fields.Item("U_AutFz").Value.ToString() == "Y" ? true : false,
                AutTransp = !pBoolToUpdate ? false : mObjsboTable.UserFields.Fields.Item("U_AutTransp").Value.ToString() == "Y" ? true : false,

            };


        }

        private string GetKeyByFolio(string pStrFolio)
        {
            return mObjSapAuctionService.GetKeyByFolio(pStrFolio).ToString();
        }

        private long GetAuctionId(string pStrFolio)
        {
            try
            {
                return SapAuctionService.GetAuctionId(pStrFolio);
            }
            catch
            {
                return 0;
            }
        }

        private long GetNextId()
        {
            try
            {
                return SapAuctionService.GetNextId();
            }
            catch
            {
                return 0;
            }
        }

        private DateTime GetLastCreationDate(string pStrLocation)
        {
            try
            {
                return SapAuctionService.GetLastCreationDate(pStrLocation);
            }
            catch
            {
                return DateTime.Today.AddYears(-10);
            }

        }

        private DateTime GetLastModificationDate(string pStrLocation)
        {
            try
            {
                return SapAuctionService.GetLastModificationDate(pStrLocation);
            }
            catch
            {
                return DateTime.Today.AddYears(-10);
            }
        }

        #endregion
    }
}
