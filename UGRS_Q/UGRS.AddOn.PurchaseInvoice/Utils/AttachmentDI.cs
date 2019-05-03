using System;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.AddOn.PurchaseInvoice.Utils
{
    public class AttachmentDI
    {
        private int AttachFileDI(string pStrFile)
        {
            int lIntAttEntry = -1;
            SAPbobsCOM.Attachments2 lObjAttachment = (SAPbobsCOM.Attachments2)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
            try
            {
                string lStrFileName = pStrFile;
                lObjAttachment.Lines.Add();
                lObjAttachment.Lines.FileName = System.IO.Path.GetFileNameWithoutExtension(lStrFileName);
                lObjAttachment.Lines.FileExtension = System.IO.Path.GetExtension(lStrFileName).Substring(1);
                lObjAttachment.Lines.SourcePath = System.IO.Path.GetDirectoryName(lStrFileName);
                lObjAttachment.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;

                if (lObjAttachment.Add() == 0)
                {

                    lIntAttEntry = int.Parse(DIApplication.Company.GetNewObjectKey());
                }
                else
                {
                    UIApplication.ShowMessageBox(DIApplication.Company.GetLastErrorDescription());
                    LogService.WriteError("AttachmentDI (AttachFile) " + DIApplication.Company.GetLastErrorDescription());
                }

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessage(ex.Message);
                LogService.WriteError("AttachmentDI (AttachFile) " + ex.Message);
                LogService.WriteError(ex);

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjAttachment);
            }
            return lIntAttEntry;
        }

        public void AttatchFile(string pStrFile, int pIntDocEntry)
        {
            int lIntAttachement = 0;
            string lStrAttach = string.Empty;
            string lStrAttachPath = GetAttachPath();
            if (!string.IsNullOrEmpty(pStrFile))
            {
                lIntAttachement = AttachFileDI(pStrFile);
                if (lIntAttachement > 0)
                {
                    lStrAttach = lStrAttachPath + System.IO.Path.GetFileName(pStrFile);
                }
                else
                {
                    LogService.WriteError("InvoiceDI (AttachDocument) " + DIApplication.Company.GetLastErrorDescription());
                    UIApplication.ShowError(string.Format("InvoiceDI (AttachDocument) : {0}", DIApplication.Company.GetLastErrorDescription()));
                    if (System.IO.File.Exists(pStrFile))
                    {
                        lStrAttach = pStrFile;
                    }
                    else
                    {
                        LogService.WriteError("InvoiceDI (AttachDocument) Archivo \n" + pStrFile + " no encontrado");
                        UIApplication.ShowError("InvoiceDI (AttachDocument) Archivo  \n" + pStrFile + " no encontrado");
                    }
                }
            }

            Update(pIntDocEntry, lStrAttach);
        }


        private string GetAttachPath()
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrAttachPath = string.Empty;

            try
            {
                string lStrQuery = "select AttachPath from OADP with (Nolock)"; //this.GetSQL("GetAttachPath");
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrAttachPath = lObjRecordset.Fields.Item("AttachPath").Value.ToString();
                }
            }
            catch (Exception ex)
            {
                //UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetAttachPath): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lStrAttachPath;
        }


        private bool Update(int pIntDocEntry, string pStrPath)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            try
            {
                string lStrQuery = string.Format("Update OPCH set U_ArchivoPDF = '{0}' where DocEntry = '{1}'", pStrPath, pIntDocEntry.ToString()); //this.GetSQL("GetAttachPath");
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetAttachPath): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return false;
        }
    }
}
