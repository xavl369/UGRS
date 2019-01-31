using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.FoodProduction.DAO;

namespace UGRS.AddOn.FoodProduction.UI.ChooseFromlist
{
    public class ConditionsUI
    {
        public  ChooseFromList SetConditionCFLWareHouse()
        {
            TicketDAO lObjTicketDAO = new TicketDAO();
            string lStrWareHouse = lObjTicketDAO.GetWareHouse(DIApplication.Company.UserSignature.ToString());
            List<string> lLstWareHouse = lObjTicketDAO.GetWareHousePather(lStrWareHouse).ToList();

            SAPbouiCOM.ChooseFromListCollection lObjCFLs = null;
            ChooseFromList lObjCFL = null;
            SAPbouiCOM.Conditions lObjCons = new Conditions();
            SAPbouiCOM.Condition lObjCon = null;

            lObjCFL = lObjCFLs.Item("CFL_Ware");

            int i = 1;
            foreach (string lStrWareHousePather in lLstWareHouse)
            {
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "WhsCode";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = lStrWareHousePather;

                if (lLstWareHouse.Count() > i)
                {
                    lObjCon.Relationship = BoConditionRelationship.cr_OR;
                }
                i++;

            }

            lObjCFL.SetConditions(lObjCons);

            return lObjCFL;
        }


        ///<summary>    Initializes the chooseFromlist. 
        /// </summary>
        public SAPbouiCOM.ChooseFromList initChooseFromListBussinesPartner(string pStrTipoDoc, SAPbouiCOM.ChooseFromList pObjCFLSocio)
        {
            string lStrType = string.Empty;
            if (pStrTipoDoc == "CFL_Venta")
            {
                lStrType = "C";
            }

            if (pStrTipoDoc == "CFL_Compra")
            {
                lStrType = "S";
            }

            try
            {

                SAPbouiCOM.Conditions lObjCons = null;
                SAPbouiCOM.Condition lObjCon = null;
                //  Adding Conditions to CFLPO
                lObjCons = pObjCFLSocio.GetConditions();

                bool lBolNewCond = true;
                foreach (SAPbouiCOM.Condition lObjCond in lObjCons)
                {
                    if (lObjCond.Alias == "CardType")
                    {
                        lObjCond.CondVal = lStrType;
                        lBolNewCond = false;
                        break;
                    }
                }

                if (lBolNewCond)
                {
                    lObjCon = lObjCons.Add();
                    lObjCon.Alias = "CardType";
                    lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    lObjCon.CondVal = lStrType;
                }

                pObjCFLSocio.SetConditions(lObjCons);

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitChooseFromListException: {0}", ex.Message));
            }

            return pObjCFLSocio;
         
        }


    }


}
