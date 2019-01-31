using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.Auctions.DAO;
using UGRS.Core.SDK.DI.Auctions.DTO;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    [Serializable]
    public class BusinessPartnerSevice
    {
        private BusinessPartnerDAO mObjBusinessPartnerDAO;

        public BusinessPartnerSevice()
        {
            mObjBusinessPartnerDAO = new BusinessPartnerDAO();
        }

        public IList<string> GetCardCodesList()
        {
            return mObjBusinessPartnerDAO.GetCardCodesList();
        }

        public IList<CustomerDTO> GetUpdatedCardCodesList()
        {
            return mObjBusinessPartnerDAO.GetUpdatedCardCodesList();
        }

        public CustomerDTO GetCustomerByCardCode(string pStrCardCode)
        {
            return mObjBusinessPartnerDAO.GetCustomerByCode(pStrCardCode);
        }

        public SAPbobsCOM.BusinessPartners GetBusinessPartnerObject()
        {
            return (SAPbobsCOM.BusinessPartners)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
        }

        public IList<CustomerDTO> SearchBusinessPartner(string pStrFilter)
        {
            return mObjBusinessPartnerDAO.SearchBusinessPartner(pStrFilter);
        }
    }
}
