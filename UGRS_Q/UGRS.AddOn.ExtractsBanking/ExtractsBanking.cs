using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data;
using System.IO;
using UGRS.AddOn.ExtractsBanking.Entities;
using UGRS.Core.Utility;
using UGRS.Core.SDK.UI.ProgressBar;
using System.Globalization;
using UGRS.AddOn.ExtractsBanking.Services;



namespace UGRS.AddOn.ExtractsBanking
{
    public class ExtractsBanking
    {
        private SAPbouiCOM.Form mObjForm = null;
        private SAPbobsCOM.Company mObjCompany = null;
        private SAPbobsCOM.BankPages mObjBankPage = null;
        private ProgressBarManager mObjProgressBar = null;
        string lStrAccountCode = "";
        int mIntCountCarga = 0;
        int mIntTotalCarga = 0;

        public ExtractsBanking(SAPbouiCOM.Company pObjCompany)
        {
            mObjCompany = (SAPbobsCOM.Company)pObjCompany.GetDICompany();
            if (mObjCompany.Connected)
            {
                Application.SBO_Application.StatusBar.SetText("Add-on Conectado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                Application.SBO_Application.StatusBar.SetText("Add-on disponible.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
                Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent_btnImpo);
            }
        }

        private void SBO_Application_ItemEvent_btnImpo(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.ItemUID == "btnImpo" && pVal.EventType == SAPbouiCOM.BoEventTypes.et_CLICK && pVal.Before_Action)
                {
                    if (((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value != "")
                    {
                        lStrAccountCode = ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value;
                        switch (((SAPbouiCOM.ComboBox)mObjForm.Items.Item("cmbBanco").Specific).Value)
                        {
                            case "BANAMEX":
                                showOpenFileDialog();
                                break;
                            case "BANCOMER":
                                showOpenFileDialog();
                                break;
                            case "BANORTE":
                                showOpenFileDialog();
                                break;
                            case "SANTANDER":
                                showOpenFileDialog();
                                break;
                            case "SCOTIABANK":
                                showOpenFileDialog();
                                break;
                            case "SELECCIONE":
                                Application.SBO_Application.MessageBox("Hizo falta capturar el Nombre del Banco del que se va a realizar el extracto bancario");
                                break;
                        }
                    }
                    else
                    {
                        Application.SBO_Application.MessageBox("Hizo falta capturar la Cuenta de Mayor antes de importar");
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.Message);
            }
        }

        private void ReadFileTxt_Banamex(string pStrPath)
        {
            mObjForm.Freeze(true);
            string[] ArrStrWords;
            mIntCountCarga = 0;
            mIntTotalCarga = 0;

            try
            {
                ArrStrWords = System.IO.File.ReadAllLines(pStrPath);
                mObjProgressBar = new ProgressBarManager(Application.SBO_Application, "Cargando extractos bancarios", ArrStrWords.Length);
                foreach (ExtractBanking lObjlObjExtractBanking in GetBanamexExtractBanking(ArrStrWords))
                {
                    mObjBankPage = PopulateBankPages(lObjlObjExtractBanking);
                    int result = mObjBankPage.Add();
                    mIntTotalCarga++;
                    if (result == 0)
                        mIntCountCarga++;
                    if (result != 0)
                    {
                        string r = mObjCompany.GetLastErrorDescription();
                        //Application.SBO_Application.MessageBox("No se cargo el movimiento número: " + mIntTotalCarga);
                        Application.SBO_Application.StatusBar.SetText("No se cargo el movimiento número: " + mIntTotalCarga, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    mObjProgressBar.NextPosition();
                }
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = "";
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = lStrAccountCode;
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                BankStatementsLogService.WriteError(ex.Message);

            }
            finally
            {
                mObjForm.Freeze(false);
                MemoryUtility.ReleaseComObject(mObjBankPage);
            }
            Application.SBO_Application.MessageBox("Proceso Terminado. Se cargaron " + mIntCountCarga + " de " + mIntTotalCarga + " movimientos bancarios.");
        }

        private IList<ExtractBanking> GetBanamexExtractBanking(string[] pArrStrLines)
        {
            IList<ExtractBanking> lLstObjResult = new List<ExtractBanking>();
            for (int i = 13; i < pArrStrLines.Length; i++)
            {
               
                    //@"\,([^\, \" ])"
                string[] lArrStrColumns = Regex.Split(pArrStrLines[i], "\\,([^\\,0-9])",RegexOptions.IgnorePatternWhitespace);

                if(lArrStrColumns.Length == 9)
                {
                    ExtractBanking lObjExtractBanking = new ExtractBanking();
                    lObjExtractBanking.AccountCode = ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value;
                    lObjExtractBanking.Date = Convert.ToDateTime(lArrStrColumns[0]);
                    //lObjExtractBanking.Reference = lArrStrColumns[1].Substring(0, 16).Trim();

                    lObjExtractBanking.Detail = string.Format("{0}{1}", lArrStrColumns[1].Trim(), lArrStrColumns[2].Trim());

                    if (lArrStrColumns[3] != "" && lArrStrColumns[3] != "-")
                    {

                        lObjExtractBanking.DebitAmount = Convert.ToDouble(lArrStrColumns[4].Replace("\"", ""));
                    }

                    if (lArrStrColumns[5] != "" && lArrStrColumns[5] != "-")
                    {
                        lObjExtractBanking.CreditAmount = Convert.ToDouble(lArrStrColumns[6].Replace("\"", ""));
                    }

                    if (!(lObjExtractBanking.DebitAmount == 0 && lObjExtractBanking.CreditAmount == 0))
                    {
                        lLstObjResult.Add(lObjExtractBanking);
                    }
                }
            }
            return lLstObjResult;
        }

        private void ReadFileTxt_Banorte(string pStrPath)
        {
            mObjForm.Freeze(true);
            string[] lArrStrLines;
            mIntCountCarga = 0;
            mIntTotalCarga = 0;

            try
            {
                lArrStrLines = System.IO.File.ReadAllLines(pStrPath);
                mObjProgressBar = new ProgressBarManager(Application.SBO_Application, "Cargando extractos bancarios", lArrStrLines.Length);
                foreach (ExtractBanking lObjlObjExtractBanking in GetBanorteExtractBanking(lArrStrLines))
                {
                    mObjBankPage = PopulateBankPages(lObjlObjExtractBanking);
                    int result = mObjBankPage.Add();
                    mIntTotalCarga++;
                    if (result == 0)
                        mIntCountCarga++;
                    if (result != 0)
                    {
                        //Application.SBO_Application.MessageBox("No se cargo el movimiento número: " + mIntTotalCarga);
                        Application.SBO_Application.StatusBar.SetText("No se cargo el movimiento número: " + mIntTotalCarga, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    mObjProgressBar.NextPosition();
                }
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = "";
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = lStrAccountCode;
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                BankStatementsLogService.WriteError(ex.Message);
            }
            finally
            {
                mObjForm.Freeze(false);
                MemoryUtility.ReleaseComObject(mObjBankPage);
            }
            Application.SBO_Application.MessageBox("Proceso Terminado. Se cargaron " + mIntCountCarga + " de " + mIntTotalCarga + " movimientos bancarios.");
        }

        private IList<ExtractBanking> GetBanorteExtractBanking(string[] pArrStrLines)
        {
            DateTime dateValue;
            IList<ExtractBanking> lLstObjResult = new List<ExtractBanking>();
            for (int i = 0; i < pArrStrLines.Length; i++)
            {
                string[] lArrStrColumns = pArrStrLines[i].Split('|');
                if (DateTime.TryParse(lArrStrColumns[1], out dateValue))   //valide
                {
                    ExtractBanking lObjExtractBanking = new ExtractBanking();

                    lObjExtractBanking.AccountCode = ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value;
                    lObjExtractBanking.Date = Convert.ToDateTime(lArrStrColumns[1]);
                    lObjExtractBanking.Reference = lArrStrColumns[3];
                    lObjExtractBanking.Detail = lArrStrColumns[4];
                    lObjExtractBanking.DebitAmount = Convert.ToDouble(Regex.Replace(lArrStrColumns[8], @"[$\,]", ""));
                    lObjExtractBanking.CreditAmount = Convert.ToDouble(Regex.Replace(lArrStrColumns[7], @"[$\,]", ""));

                    lLstObjResult.Add(lObjExtractBanking);
                }
            }
            return lLstObjResult;
        }

        private void ReadFileXlsx_Bancomer(string pStrPath)
        {
            mObjForm.Freeze(true);
            string[] ArrStrWords;
            mIntCountCarga = 0;
            mIntTotalCarga = 0;

            //DataTable lDtbFileBancomer = new DataTable();
            try
            {
                ArrStrWords = System.IO.File.ReadAllLines(pStrPath);
                //lDtbFileBancomer = GetDtbFileBancomer(pStrPath);
                mObjProgressBar = new ProgressBarManager(Application.SBO_Application, "Cargando extractos bancarios", ArrStrWords.Length);
                foreach (ExtractBanking lObjExtractBanking in GetBancomerExtractBanking(ArrStrWords))
                {
                    mObjBankPage = PopulateBankPages(lObjExtractBanking);
                    int result = mObjBankPage.Add();
                    mIntTotalCarga++;
                    if (result == 0)
                        mIntCountCarga++;
                    if (result != 0)
                    {
                        //Application.SBO_Application.MessageBox("No se cargo el movimiento número: " + mIntTotalCarga);
                        Application.SBO_Application.StatusBar.SetText("No se cargo el movimiento número: " + mIntTotalCarga, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    mObjProgressBar.NextPosition();
                }
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = "";
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = lStrAccountCode;

            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                BankStatementsLogService.WriteError(ex.Message);
            }
            finally
            {
                mObjForm.Freeze(false);
                MemoryUtility.ReleaseComObject(mObjBankPage);
            }
            Application.SBO_Application.MessageBox("Proceso Terminado. Se cargaron " + mIntCountCarga + " de " + mIntTotalCarga + " movimientos bancarios.");
            BankStatementsLogService.WriteSuccess("Proceso Terminado. Se cargaron " + mIntCountCarga + " de " + mIntTotalCarga + " movimientos bancarios.");
        }

        private IList<ExtractBanking> GetBancomerExtractBanking(string[] pArrStrWords)
        {
            DateTime dateValue;
            IList<ExtractBanking> lLstObjResult = new List<ExtractBanking>();

            for (int i = 1; i < pArrStrWords.Length; i++)
            {
                string[] lArrStrColumns = pArrStrWords[i].Split('\t');

                ExtractBanking lObjExtractBanking = new ExtractBanking();

                string lStrFecha = lArrStrColumns[0].ToString();
                string lStrConcepto = lArrStrColumns[1].ToString();
                
                string lStrCargo = lArrStrColumns[2].ToString();
                string lStrAbono = lArrStrColumns[3].ToString();

                lObjExtractBanking.AccountCode = ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value;
                lObjExtractBanking.Date = Convert.ToDateTime(lStrFecha);
                lObjExtractBanking.Detail = lStrConcepto;
                if (lStrCargo != "")
                    lObjExtractBanking.DebitAmount = Convert.ToDouble(Regex.Replace(lStrCargo, @"[$\,]", ""));
                if (lStrCargo == "")
                    lObjExtractBanking.DebitAmount = 0;
                if (lStrAbono != "")
                    lObjExtractBanking.CreditAmount = Convert.ToDouble(Regex.Replace(lStrAbono, @"[$\,]", ""));
                if (lStrAbono == "")
                    lObjExtractBanking.CreditAmount = 0;

                lLstObjResult.Add(lObjExtractBanking);
            }
            //for (int i = 0; i < pArrStrWords.Count(); i++)
            //{
            //    if (DateTime.TryParse(pArrStrWords.Rows[i].ItemArray[0].ToString(), out dateValue))   //valide
            //    {
            //ExtractBanking lObjExtractBanking = new ExtractBanking();

            //        string lStrFecha = pArrStrWords.Rows[i].ItemArray[0].ToString();
            ////        string lStrConcepto = pArrStrWords.Rows[i].ItemArray[1].ToString();
            ////        string lStrReferencia = pArrStrWords.Rows[i].ItemArray[2].ToString();
            ////        string lStrCargo = pArrStrWords.Rows[i].ItemArray[4].ToString();
            ////        string lStrAbono = pArrStrWords.Rows[i].ItemArray[5].ToString();

            //lObjExtractBanking.AccountCode = ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value;
            //lObjExtractBanking.Date = Convert.ToDateTime(lStrFecha);
            //lObjExtractBanking.Reference = lStrReferencia;
            //lObjExtractBanking.Detail = lStrConcepto;
            //if (lStrCargo != "")
            //    lObjExtractBanking.DebitAmount = Convert.ToDouble(Regex.Replace(lStrCargo, @"[$\,]", ""));
            //if (lStrCargo == "")
            //    lObjExtractBanking.DebitAmount = 0;
            //if (lStrAbono != "")
            //    lObjExtractBanking.CreditAmount = Convert.ToDouble(Regex.Replace(lStrAbono, @"[$\,]", ""));
            //if (lStrAbono == "")
            //    lObjExtractBanking.CreditAmount = 0;

            //        lLstObjResult.Add(lObjExtractBanking);
            //    }
            //}
            return lLstObjResult;
        }

        private DataTable GetDtbFileBancomer(string pStrPath)
        {
            string lStrConexion = "", lStrSheetName = "";
            int lIntCountSheets = 0;
            OleDbConnection lOleConnection = new OleDbConnection();
            OleDbCommand lOleCommand = new OleDbCommand();
            DataTable lDtbSheet = new DataTable();
            DataTable lDtbFile = new DataTable();
            OleDbDataAdapter lOleDataAdapter = new OleDbDataAdapter();

            if (System.IO.Path.GetExtension(pStrPath) == ".xlsx")
                lStrConexion = "Provider=Microsoft.ACE.OLEDB.12.0; Extended Properties=Excel 12.0 XML; Data Source=" + pStrPath + ";";
            using (lOleConnection = new OleDbConnection(lStrConexion))
            {
                lOleConnection.Open();
                lOleCommand.Connection = lOleConnection;
                lDtbSheet = lOleConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);   // Get all Sheets in Excel File                    
                lIntCountSheets = 1;
                foreach (DataRow dr in lDtbSheet.Rows) // Loop through all Sheets to get data
                {
                    if (lIntCountSheets == 1)
                    {
                        lStrSheetName = dr["TABLE_NAME"].ToString();
                        if (!lStrSheetName.EndsWith("$"))
                            continue;
                        lOleCommand.CommandText = "SELECT * FROM [" + lStrSheetName + "]";  // Get all rows from the Sheet
                        lDtbFile.TableName = lStrSheetName;
                        lOleDataAdapter = new OleDbDataAdapter(lOleCommand);
                        lOleDataAdapter.Fill(lDtbFile);
                    }
                    lIntCountSheets++;
                }
            }
            return lDtbFile;
        }

        private void ReadFileTxt_ScotiaBank(string pStrPath)
        {
            mObjForm.Freeze(true);
            string[] lArrStrLines;
            mIntCountCarga = 0;
            mIntTotalCarga = 0;

            try
            {
                lArrStrLines = System.IO.File.ReadAllLines(pStrPath);
                mObjProgressBar = new ProgressBarManager(Application.SBO_Application, "Cargando extractos bancarios", lArrStrLines.Length);
                foreach (ExtractBanking lObjExtractBanking in GetScotiaBankExtractBanking(lArrStrLines))
                {
                    mObjBankPage = PopulateBankPages(lObjExtractBanking);
                    int result = mObjBankPage.Add();
                    mIntTotalCarga++;
                    if (result == 0)
                        mIntCountCarga++;
                    if (result != 0)
                    {
                        //Application.SBO_Application.MessageBox("No se cargo el movimiento número: " + mIntTotalCarga);
                        Application.SBO_Application.StatusBar.SetText("No se cargo el movimiento número: " + mIntTotalCarga, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    mObjProgressBar.NextPosition();
                }
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = "";
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = lStrAccountCode;
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                BankStatementsLogService.WriteError(ex.Message);
            }
            finally
            {
                mObjForm.Freeze(false);
                MemoryUtility.ReleaseComObject(mObjBankPage);
            }
            Application.SBO_Application.MessageBox("Proceso Terminado. Se cargaron " + mIntCountCarga + " de " + mIntTotalCarga + " movimientos bancarios.");
        }

        private IList<ExtractBanking> GetScotiaBankExtractBanking(string[] pArrStrLines)
        {
            IList<ExtractBanking> lLstObjResult = new List<ExtractBanking>();
            for (int i = 0; i < pArrStrLines.Length; i++)
            {
                ExtractBanking lObjExtractBanking = new ExtractBanking();

                string lStrFecha = pArrStrLines[i].Substring(26, 10);
                string lStrReferencia = pArrStrLines[i].Substring(36, 10);
                string lStrImporte = pArrStrLines[i].Substring(46, 17);
                string lStrCargoAbono = pArrStrLines[i].Substring(63, 5);
                string lStrSaldo = pArrStrLines[i].Substring(68, 17);
                string lStrTransaccion = pArrStrLines[i].Substring(134, 56);

                lObjExtractBanking.AccountCode = ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value;
                lObjExtractBanking.Date = Convert.ToDateTime(lStrFecha);
                lObjExtractBanking.Reference = lStrReferencia;
                lObjExtractBanking.Detail = lStrTransaccion;
                if (lStrCargoAbono.ToUpper() == "CARGO")
                {
                    lObjExtractBanking.DebitAmount = Convert.ToDouble(lStrImporte);
                }
                else
                {
                    lObjExtractBanking.CreditAmount = Convert.ToDouble(lStrImporte);
                }
                lLstObjResult.Add(lObjExtractBanking);
            }
            return lLstObjResult;
        }

        private void ReadFileCsv_Santander(string pStrPath)
        {
            mObjForm.Freeze(true);
            string[] lArrStrLines;
            mIntCountCarga = 0;
            mIntTotalCarga = 0;

            try
            {
                lArrStrLines = System.IO.File.ReadAllLines(pStrPath);
                mObjProgressBar = new ProgressBarManager(Application.SBO_Application, "Cargando extractos bancarios", lArrStrLines.Length);
                foreach (ExtractBanking lObjlObjExtractBanking in GetSantanderExtractBanking(lArrStrLines))
                {
                    mObjBankPage = PopulateBankPages(lObjlObjExtractBanking);
                    int result = mObjBankPage.Add();
                    mIntTotalCarga++;
                    if (result == 0)
                        mIntCountCarga++;
                    if (result != 0)
                    {
                        //Application.SBO_Application.MessageBox("No se cargo el movimiento número: " + mIntTotalCarga);
                        Application.SBO_Application.StatusBar.SetText("No se cargo el movimiento número: " + mIntTotalCarga, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    mObjProgressBar.NextPosition();
                }
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = "";
                ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value = lStrAccountCode;
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                BankStatementsLogService.WriteError(ex.Message);

            }
            finally
            {
                mObjForm.Freeze(false);
                MemoryUtility.ReleaseComObject(mObjBankPage);
            }

            Application.SBO_Application.MessageBox("Proceso Terminado. Se cargaron " + mIntCountCarga + " de " + mIntTotalCarga + " movimientos bancarios.");
        }

        private IList<ExtractBanking> GetSantanderExtractBanking(string[] pArrStrLines)
        {
            int dateValue;
            IList<ExtractBanking> lLstObjResult = new List<ExtractBanking>();
            for (int i = 0; i < pArrStrLines.Length; i++)
            {
                string[] lArrStrColumns = pArrStrLines[i].Split(',');
                if (int.TryParse(Regex.Replace(lArrStrColumns[1].ToString(), @"[^\w]", ""), out dateValue))   //valide
                {
                    ExtractBanking lObjExtractBanking = new ExtractBanking();

                    string lStrConvertFecha = Regex.Replace(lArrStrColumns[1].ToString(), @"[^\w]", "");
                    string lStrCargoAbono = lArrStrColumns[5].ToString();
                    string lStrImporte = lArrStrColumns[6].Replace(",", "");

                    lObjExtractBanking.AccountCode = ((SAPbouiCOM.EditText)mObjForm.Items.Item("18").Specific).Value;
                    lObjExtractBanking.Date = DateTime.ParseExact(lStrConvertFecha, "ddMMyyyy", CultureInfo.InvariantCulture);
                    lObjExtractBanking.Reference = Regex.Replace(lArrStrColumns[8].ToString(), @"[^\w]", "");
                    lObjExtractBanking.Detail = Regex.Replace(lArrStrColumns[4].ToString(), @"[^\w]", " ").Trim();

                    lStrCargoAbono = lStrCargoAbono.ToArray().Contains('+') ? "ABONO" : "CARGO";

                    if (lStrCargoAbono.ToUpper() == "CARGO")
                    {
                        lObjExtractBanking.DebitAmount = Convert.ToDouble(lStrImporte);
                    }
                    else
                    {
                        lObjExtractBanking.CreditAmount = Convert.ToDouble(lStrImporte);
                    }

                    lLstObjResult.Add(lObjExtractBanking);
                }
            }
            return lLstObjResult;
        }

        private SAPbobsCOM.BankPages PopulateBankPages(ExtractBanking pObjExtractBanking)
        {
            mObjBankPage = (SAPbobsCOM.BankPages)mObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBankPages);

            mObjBankPage.AccountCode = pObjExtractBanking.AccountCode;
            mObjBankPage.DueDate = pObjExtractBanking.Date;
            mObjBankPage.Reference = pObjExtractBanking.Reference;
            mObjBankPage.Memo = pObjExtractBanking.Detail;

            if (pObjExtractBanking.DebitAmount == 0 && pObjExtractBanking.CreditAmount == 0)
            {

            }

            if (pObjExtractBanking.DebitAmount > 0)
            {
                mObjBankPage.DebitAmount = pObjExtractBanking.DebitAmount;
            }

            if (pObjExtractBanking.CreditAmount > 0)
            {
                mObjBankPage.CreditAmount = pObjExtractBanking.CreditAmount;
            }

            return mObjBankPage;
        }

        private void showOpenFileDialog()
        {
            try
            {
                Thread ShowFolderBroserThread = new Thread(ShowFolderBrowser);
                if (ShowFolderBroserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBroserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBroserThread.Start();
                }
                else
                {
                    ShowFolderBroserThread.Start();
                    ShowFolderBroserThread.Join();

                }
                while (ShowFolderBroserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.Message);
            }
        }

        /*
        private void ShowFolderBroser()
        {
            string lStrPath = null;
            string lStrBanking = "";
            using (System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog())
            {
                lStrBanking = ((SAPbouiCOM.ComboBox)mObjForm.Items.Item("cmbBanco").Specific).Value;
                OpenFileDialogFilterAndTitle(lStrBanking, openFileDialog1);
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lStrPath = openFileDialog1.FileName;
                    switch (((SAPbouiCOM.ComboBox)mObjForm.Items.Item("cmbBanco").Specific).Value)
                    {
                        case "BANAMEX":
                            if (System.IO.Path.GetExtension(lStrPath) == ".txt")
                            {
                                ReadFileTxt_Banamex(lStrPath);
                            }
                            else
                            {
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(txt)");
                            }
                            break;
                        case "BANCOMER":
                            if (System.IO.Path.GetExtension(lStrPath) == ".xlsx")
                            {
                                ReadFileXlsx_Bancomer(lStrPath);
                            }
                            else
                            {
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(xlsx)");
                            }
                            break;
                        case "BANORTE":
                            if (System.IO.Path.GetExtension(lStrPath) == ".txt")
                            {
                                ReadFileTxt_Banorte(lStrPath);
                            }
                            else
                            {
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(txt)");
                            }
                            break;
                        case "SANTANDER":
                            if (System.IO.Path.GetExtension(lStrPath) == ".csv")
                            {
                                ReadFileCsv_Santander(lStrPath);
                            }
                            else
                            {
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(csv)");
                            }
                            break;
                        case "SCOTIABANK":
                            if (System.IO.Path.GetExtension(lStrPath) == ".txt")
                            {
                                ReadFileTxt_ScotiaBank(lStrPath);
                            }
                            else
                            {
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(txt)");
                            }
                            break;
                    }
                }
            }
        }

         */


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 
        /// </summary>
        private void ShowFolderBrowser()
        {
            using (System.Windows.Forms.OpenFileDialog oFile = new System.Windows.Forms.OpenFileDialog())
            {
                string fileName = "";
                string lStrBank = "";
                try
                {
                    IntPtr sapProc = GetForegroundWindow();
                    WindowWrapper MyWindow = null;

                    MyWindow = new WindowWrapper(sapProc);

                    oFile.Multiselect = false;
                    lStrBank = ((SAPbouiCOM.ComboBox)mObjForm.Items.Item("cmbBanco").Specific).Value;
                    GetFileDialogFilter(lStrBank, oFile);
                    //oFile.Filter = "Archivos Excel(*.xls)|*.xls|Archivos TXT(*.txt)|*.txt|Archivos CSV(*.csv)|*.csv";
                    oFile.FilterIndex = 0;
                    oFile.RestoreDirectory = true;
                    var dialogResult = oFile.ShowDialog(MyWindow);

                    if (dialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        fileName = oFile.FileName;

                        string lStrExtencion = Path.GetExtension(fileName);
                        switch (((SAPbouiCOM.ComboBox)mObjForm.Items.Item("cmbBanco").Specific).Value)
                        {
                            case "BANAMEX":
                                if (lStrExtencion == ".csv") { ReadFileTxt_Banamex(fileName); }
                                else { SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(txt)"); }
                                break;

                            case "BANCOMER":
                                if (lStrExtencion == ".txt") { ReadFileXlsx_Bancomer(fileName); }
                                else { SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(xlsx)"); }
                                break;

                            case "BANORTE":
                                if (lStrExtencion == ".txt") { ReadFileTxt_Banorte(fileName); }
                                else { SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(txt)"); }
                                break;

                            case "SANTANDER":
                                if (lStrExtencion == ".csv") { ReadFileCsv_Santander(fileName); }
                                else { SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(csv)"); }
                                break;

                            case "SCOTIABANK":
                                if (lStrExtencion == ".txt") { ReadFileTxt_ScotiaBank(fileName); }
                                else { SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(txt)"); }
                                break;
                        }

                    }
                    else

                        System.Windows.Forms.Application.ExitThread();


                }
                catch (Exception e)
                {
                    fileName = "";
                    SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Archivo Excel se encuentra abierto o el archivo tiene un formato incorrecto");
                    BankStatementsLogService.WriteError(e.Message);
                    System.Windows.Forms.Application.ExitThread();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pStrBank"></param>
        /// <param name="pOFile"></param>
        private void GetFileDialogFilter(string pStrBank, System.Windows.Forms.OpenFileDialog pOFile)
        {
            switch (pStrBank)
            {
                case "BANAMEX":
                    pOFile.Filter = "Text Files|*.csv";
                    pOFile.Title = "Selecciona el archivo de Banamex";
                    break;
                case "BANCOMER":
                    pOFile.Filter = "Text Files|*.txt";
                    pOFile.Title = "Selecciona el archivo de Bancomer";
                    break;
                case "BANORTE":
                    pOFile.Filter = "Text Files|*.txt";
                    pOFile.Title = "Selecciona el archivo de Banorte";
                    break;
                case "SANTANDER":
                    pOFile.Filter = "Text Files|*.csv";
                    pOFile.Title = "Selecciona el archivo de Santander";
                    break;
                case "SCOTIABANK":
                    pOFile.Filter = "Text Files|*.txt";
                    pOFile.Title = "Selecciona el archivo de ScotiaBank";
                    break;
            }

        }

        /*
        private void OpenFileDialogFilterAndTitle(string pStrBanking, System.Windows.Forms.OpenFileDialog pBoxDialog)
        {
            switch (pStrBanking)
            {
                case "BANAMEX":
                    pBoxDialog.Filter = "Text Files|*.txt";
                    pBoxDialog.Title = "Selecciona el archivo de Banamex";
                    break;
                case "BANCOMER":
                    pBoxDialog.Filter = "Text Files|*.xlsx";
                    pBoxDialog.Title = "Selecciona el archivo de Bancomer";
                    break;
                case "BANORTE":
                    pBoxDialog.Filter = "Text Files|*.txt";
                    pBoxDialog.Title = "Selecciona el archivo de Banorte";
                    break;
                case "SANTANDER":
                    pBoxDialog.Filter = "Text Files|*.csv";
                    pBoxDialog.Title = "Selecciona el archivo de Santander";
                    break;
                case "SCOTIABANK":
                    pBoxDialog.Filter = "Text Files|*.txt";
                    pBoxDialog.Title = "Selecciona el archivo de ScotiaBank";
                    break;
            }
        }
        */

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.FormType == 385 && pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD && pVal.Before_Action)
                {
                    SAPbouiCOM.Item lItmLblBanco = null;
                    SAPbouiCOM.Item lItmBtnImportar = null;
                    SAPbouiCOM.Item lItmCmbBanco = null;

                    mObjForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);

                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD && pVal.Before_Action)
                    {
                        lItmLblBanco = mObjForm.Items.Add("lblBanco", SAPbouiCOM.BoFormItemTypes.it_STATIC); //Item("lblBanco");
                        lItmBtnImportar = mObjForm.Items.Add("btnImpo", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                        lItmCmbBanco = mObjForm.Items.Add("cmbBanco", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);

                        //lItmCmbBanco.Top = mObjForm.Items.Item("540002004").Top;
                        //lItmCmbBanco.Left = mObjForm.Items.Item("540002004").Left + 45;

                        lItmLblBanco.Top = mObjForm.Items.Item("6").Top;
                        lItmLblBanco.Left = mObjForm.Items.Item("6").Left + 40;

                        lItmCmbBanco.Top = lItmLblBanco.Top + 15;
                        lItmCmbBanco.Left = lItmLblBanco.Left;

                        //lItmLblBanco.Top = lItmCmbBanco.Top -15;
                        //lItmLblBanco.Left = lItmCmbBanco.Left - 25;
                        (lItmLblBanco.Specific as SAPbouiCOM.StaticText).Caption = "Nombre Banco:";

                        lItmBtnImportar.Top = lItmCmbBanco.Top;
                        lItmBtnImportar.Left = lItmCmbBanco.Left + lItmCmbBanco.Width + 1;
                        (lItmBtnImportar.Specific as SAPbouiCOM.Button).Caption = "Importar";
                        (lItmCmbBanco.Specific as SAPbouiCOM.ComboBox).ValidValues.Add("SELECCIONE", "SELECCIONE");
                        (lItmCmbBanco.Specific as SAPbouiCOM.ComboBox).ValidValues.Add("BANAMEX", "BANAMEX");
                        (lItmCmbBanco.Specific as SAPbouiCOM.ComboBox).ValidValues.Add("BANCOMER", "BANCOMER");
                        (lItmCmbBanco.Specific as SAPbouiCOM.ComboBox).ValidValues.Add("BANORTE", "BANORTE");
                        (lItmCmbBanco.Specific as SAPbouiCOM.ComboBox).ValidValues.Add("SANTANDER", "SANTANDER");
                        (lItmCmbBanco.Specific as SAPbouiCOM.ComboBox).ValidValues.Add("SCOTIABANK", "SCOTIABANK");
                        (lItmCmbBanco.Specific as SAPbouiCOM.ComboBox).Select("SELECCIONE");
                        lItmCmbBanco.DisplayDesc = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.Message);
            }
        }
    }
}
