using System;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Object.Auctions.Services
{
    public class FinancialsService
    {
        #region Attributes

        UGRS.Core.SDK.DI.Auctions.Services.FinancialsService mObjSapFinancialsService;
        //FinancialsServicesFactory mObjLocalFoodChargeService;
        

        #endregion

        #region Properties

        public UGRS.Core.SDK.DI.Auctions.Services.FinancialsService SapFinancialsService
        {
            get { return mObjSapFinancialsService; }
            set { mObjSapFinancialsService = value; }
        }


        //public FinancialsServicesFactory LocalFoodChargeService
        //{
        //    get { return mObjLocalFoodChargeService; }
        //    set { mObjLocalFoodChargeService = value; }
        //}

        #endregion

        #region Contructor

        public FinancialsService()
        {
            SapFinancialsService = new UGRS.Core.SDK.DI.Auctions.Services.FinancialsService();
            //LocalFoodChargeService = new FinancialsServicesFactory();
        }

        #endregion

        #region Methods

        public int CreateJournalEntry(string pStrFolio, string pStrSellerCardCode, string pStrBuyerCardCode, double pDblAmount)
        {
            SAPbobsCOM.JournalEntries lObjJournalEntry = null;
            int lIntResult = -1;

            try
            {
                //Get Header
                lObjJournalEntry = (SAPbobsCOM.JournalEntries)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lObjJournalEntry.DueDate = DateTime.Today;
                lObjJournalEntry.TaxDate = DateTime.Today;
                lObjJournalEntry.AutoVAT = SAPbobsCOM.BoYesNoEnum.tYES;
                lObjJournalEntry.Reference = pStrFolio;
                lObjJournalEntry.Series = GetSeries();
                lObjJournalEntry.Memo = "Cierre de subasta " + DateTime.Now.ToShortDateString();
                
                //Add debit to seller
                lObjJournalEntry = AddDebitToSeller(lObjJournalEntry, pStrFolio, pStrSellerCardCode, pDblAmount);

                //Add credit to the buyer
                lObjJournalEntry = AddCreditToBuyer(lObjJournalEntry, pStrFolio, pStrBuyerCardCode, pDblAmount);

                //Save
                lIntResult = lObjJournalEntry.Add();
            }
            catch (Exception lObjException)
            {
                Console.WriteLine(lObjException.ToString());
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjJournalEntry);
            }

            return lIntResult;
        }

        public string GetDeliveriesFood(string pStrWhsCode, string pStrCardCode)
        {
            return SapFinancialsService.GetDeliveriesFood(pStrWhsCode, pStrCardCode).JsonSerialize();
        }

        public string GetPrice(string pStrWhsCode, string pStrItemCode)
        {
            return SapFinancialsService.GetPrice(pStrWhsCode, pStrItemCode);
        }

        private SAPbobsCOM.JournalEntries AddDebitToSeller(SAPbobsCOM.JournalEntries pObjJournalEntry, string pStrFolio, string pStrSellerCardCode, double pDblAmount)
        {
            pObjJournalEntry.Lines.AccountCode = GetDebitAccount();
            pObjJournalEntry.Lines.ContraAccount = GetCreditAccount();
            pObjJournalEntry.Lines.CostingCode = GetCostCenter();
            pObjJournalEntry.Lines.Credit = 0;
            pObjJournalEntry.Lines.Debit = pDblAmount;
            pObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliary").Value = pStrSellerCardCode;
            pObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_AuxType").Value = "1";
            pObjJournalEntry.Lines.UserFields.Fields.Item("U_SU_Folio").Value = pStrFolio;
            pObjJournalEntry.Lines.Add();

            return pObjJournalEntry;
        }

        private SAPbobsCOM.JournalEntries AddCreditToBuyer(SAPbobsCOM.JournalEntries pObjJournalEntry, string pStrFolio, string pStrBuyerCardCode, double pDblAmount)
        {
            pObjJournalEntry.Lines.AccountCode = GetCreditAccount();
            pObjJournalEntry.Lines.ContraAccount = GetDebitAccount();
            pObjJournalEntry.Lines.CostingCode = GetCostCenter();
            pObjJournalEntry.Lines.Credit = pDblAmount;
            pObjJournalEntry.Lines.Debit = 0;
            pObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliary").Value = pStrBuyerCardCode;
            pObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_AuxType").Value = "1";
            pObjJournalEntry.Lines.UserFields.Fields.Item("U_SU_Folio").Value = pStrFolio;
            pObjJournalEntry.Lines.Add();

            return pObjJournalEntry;
        }

        private int GetSeries()
        {
            QueryManager lObjQueryManager = new QueryManager();
            try
            {
                return Convert.ToInt16(lObjQueryManager.GetValue("U_Value", "Name", "SU_HE_SERIE", "[@UG_CONFIG]"));
            }
            catch (Exception)
            {
                return 0;
            }
            
        }

        private static string GetCostCenter()
        {
            return ConfigurationUtility.GetValue<string>("CostCenter");
        }

        private static string GetDebitAccount()
        {
            // 1070050001000 Otros deudores diversos (Activo)
            return ConfigurationUtility.GetValue<string>("DebitAccount");
        }

        private static string GetCreditAccount()
        {
            // 2040010004000 Otros acreedores diversos (Pasivo)
            return ConfigurationUtility.GetValue<string>("CreditAccount");
        }

        #endregion

        //public void SetFoodCHeckList()
        //{
            
        //}
    }
}
