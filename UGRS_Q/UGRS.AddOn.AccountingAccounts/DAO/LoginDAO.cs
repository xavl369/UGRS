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
using UGRS.Core.Extension;
using UGRS.Core.Exceptions;

namespace UGRS.AddOn.AccountingAccounts.DAO
{
    public class LoginDAO
    {
        public IList<LoginDTO> GetLoginServer()
        {
            Recordset lObjRecordset = null;
            LoginDTO lObjLoginDTO = null;
            IList<LoginDTO> lListObjResult = null;

            try
            {
                lListObjResult = new List<LoginDTO>();

                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                //(Recordset)((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany()).GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = this.GetSQL("SetupLoginNew");
                lObjRecordset.DoQuery(lStrQuery);
                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lObjLoginDTO = new LoginDTO();
                        lObjLoginDTO.Code = int.Parse(lObjRecordset.Fields.Item("Code").Value.ToString());
                        lObjLoginDTO.IdBD = int.Parse(lObjRecordset.Fields.Item("U_IdBD").Value.ToString());
                        //lObjLoginDTO.Name = lObjRecordset.Fields.Item("U_Name").Value.ToString();
                        lObjLoginDTO.NameServer = lObjRecordset.Fields.Item("U_NameServer").Value.ToString();
                        lObjLoginDTO.NameDB = lObjRecordset.Fields.Item("U_NameDB").Value.ToString();
                        lObjLoginDTO.Login = lObjRecordset.Fields.Item("U_Login").Value.ToString();
                        lObjLoginDTO.Password = lObjRecordset.Fields.Item("U_Password").Value.ToString();
                        lObjLoginDTO.AccountingAccount = lObjRecordset.Fields.Item("U_AccountingAccount").Value.ToString();
                        lObjLoginDTO.Activo = lObjRecordset.Fields.Item("U_Activo").Value.ToString();
                        lObjLoginDTO.Descripcion = lObjRecordset.Fields.Item("U_Descripcion").Value.ToString();

                        lListObjResult.Add(lObjLoginDTO);
                        
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

        private bool ExistTable(string tableName)
        {
            Recordset lObjRecordset = null;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            
                lObjRecordset.DoQuery("SELECT * FROM OUTB");
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

            return false;
        }


        internal int GetUserSerie(int pIntUserSign)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);


                string lStrQuery = this.GetSQL("GetSerie").InjectSingleValue("UsrSign", pIntUserSign);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (int)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }
    }
}
