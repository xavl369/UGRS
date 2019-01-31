using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.AccountingAccounts.DTO;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Utility;

namespace UGRS.AddOn.AccountingAccounts.DAO
{
    public class DBDAO
    {
        public IList<DBDTO> GetDBServer()
        {
            Recordset lObjRecordset = null;
            DBDTO lObjDBDTO = null;
            IList<DBDTO> lListObjResult = null;

            try
            {
                lObjDBDTO = new DBDTO();
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                //(Recordset)((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany()).GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = this.GetSQL("SetupDB");
                lObjRecordset.DoQuery(lStrQuery);
                if (lObjRecordset.RecordCount > 0)
                {
                    lListObjResult = new List<DBDTO>();
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lListObjResult.Add(GetItemDB(lObjRecordset));
                        lObjRecordset.MoveNext();                        
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                //throw;
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lListObjResult;
        }
        private DBDTO GetItemDB(Recordset pObjRecordset)
        {
            DBDTO lObjDBDTO = new DBDTO();            
            lObjDBDTO.Code = pObjRecordset.Fields.Item("Code").Value.ToString();
            lObjDBDTO.Name = pObjRecordset.Fields.Item("Name").Value.ToString();
            lObjDBDTO.Code_UG_AA_LOGIN = pObjRecordset.Fields.Item("U_Code_UG_AA_LOGIN").Value.ToString();
            lObjDBDTO.NameDB = pObjRecordset.Fields.Item("U_NameDB").Value.ToString();
            lObjDBDTO.Descripcion = pObjRecordset.Fields.Item("U_Descripcion").Value.ToString();
            lObjDBDTO.Status = pObjRecordset.Fields.Item("U_Status").Value.ToString();
            return lObjDBDTO;
        }

    }
}

