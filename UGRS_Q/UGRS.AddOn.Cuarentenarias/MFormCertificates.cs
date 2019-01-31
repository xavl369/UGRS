using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Cuarentenarias.Models;
using UGRS.Core.Utility;

namespace UGRS.AddOn.Cuarentenarias
{
    public class MFormCertificates
    {
        SAPbouiCOM.Form lObjModalForm;
        SAPbouiCOM.Matrix lObjMatrix = null;
        SAPbouiCOM.DataTable oDTCert;
        SAPbouiCOM.Button lObjBtnOk = null;
        SAPbouiCOM.Button lObjBtnCan = null;

        List<Inspeccion> lstInspeccion = new List<Inspeccion>();

        public int count = 0;
        string lStrRow = "";
        int Row = 0;

        Utils.utils lObjUtils = new Utils.utils();

        public MFormCertificates(string pStrFileName, string pStrFormName, List<Inspeccion> LstInspeccion)
        {
            lstInspeccion = LstInspeccion;
            LoadFromXml(pStrFileName, pStrFormName);
            LoadMatrix();


        }


        private void LoadFromXml(string FileName, string FormName)
        {
            System.Xml.XmlDocument oXmlDoc = new System.Xml.XmlDocument();
            //string sPath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]).ToString();
            string sPath = PathUtilities.GetCurrent("XmlForms");

            oXmlDoc.Load(sPath + "\\" + FileName);

            SAPbouiCOM.FormCreationParams creationPackage = (SAPbouiCOM.FormCreationParams)Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);

            creationPackage.XmlData = oXmlDoc.InnerXml;

            if (FormName.Equals("frmModCert"))
            {
                //UGRS.AddOn.Cuarentenarias.Utils.utils.FormExists(FormName);
                if (!lObjUtils.FormExists(FormName))
                {
                    creationPackage.UniqueID = FormName;
                    creationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
                    creationPackage.Modality = SAPbouiCOM.BoFormModality.fm_Modal;
                    creationPackage.FormType = "frmModCert";
                    //creationPackage.

                    lObjModalForm = Application.SBO_Application.Forms.AddEx(creationPackage);
                    lObjModalForm.Title = "Asignar Certificados";
                    lObjModalForm.Left = 400;
                    lObjModalForm.Top = 100;
                    lObjModalForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    lObjModalForm.Visible = true;
                    initFormXml();

                }
                else
                {
                    //lObjModalForm.Select();
                }
            }
            
        }

        private void initFormXml()
        {
            lObjModalForm.Freeze(true);
            setItems();
            lObjModalForm.Freeze(false);
        }

        private void setItems()
        {
            lObjMatrix = ((SAPbouiCOM.Matrix)lObjModalForm.Items.Item("MtxCert").Specific);
            lObjBtnOk = ((SAPbouiCOM.Button)lObjModalForm.Items.Item("btnOk").Specific);
            lObjBtnCan = ((SAPbouiCOM.Button)lObjModalForm.Items.Item("btnCan").Specific);
            lObjMatrix.AutoResizeColumns();
            initMatrix();
        }

        private void initMatrix()
        {
            #region Matrix Certificado

            lObjModalForm.DataSources.DataTables.Add("DtCertificate");
            oDTCert = lObjModalForm.DataSources.DataTables.Item("DtCertificate");

            oDTCert.Columns.Add("Col_0", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            //oDTCert.Columns.Add("Col_1", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            oDTCert.Columns.Add("Col_2", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

            if (oDTCert.Rows.Count == 0)
            {
                oDTCert.Rows.Add();
            }

            #endregion


        }

        private void LoadMatrix()
        {
            //for (int i = 0; i < 12; i++)
            //{
            //oDTCert.Rows.Add();
            //lObjMatrix.AddRow();


            oDTCert.Columns.Item("Col_0").Cells.Item(0).Value = "";
            oDTCert.Columns.Item("Col_2").Cells.Item(0).Value = "";

                lObjMatrix.Columns.Item("Rank1").DataBind.Bind("DtCertificate", "Col_0");
                //lObjMatrix.Columns.Item("Rank2").DataBind.Bind("DtCertificate", "Col_1");
                lObjMatrix.Columns.Item("Cert").DataBind.Bind("DtCertificate", "Col_2");

                

            //}

            lObjMatrix.LoadFromDataSource();

        }

        internal void AddRow(int lIntRow)
        {
            //string lStrActualRow = ((SAPbouiCOM.EditText)(lObjMatrix.Columns.Item("Rank1").Cells.Item(lIntRow).Specific)).Value;
            //int ActualRow = lObjMatrix.GetCellFocus().rowIndex;

     
                if (((SAPbouiCOM.EditText)(lObjMatrix.Columns.Item("Rank1").Cells.Item(lIntRow).Specific)).Value != "")
                {
                    if (lStrRow == "")
                    {
                        lObjMatrix.AddRow();
                    }

                }
            

        }

        internal void ActualRow(int lIntRow)
        {
            lStrRow = ((SAPbouiCOM.EditText)(lObjMatrix.Columns.Item("Rank1").Cells.Item(lIntRow).Specific)).Value;
            //Row = lObjMatrix.GetNextSelectedRow(1, SAPbouiCOM.BoOrderType.ot_SelectionOrder);
        }

        //internal void SaveRanks()
        //{
            
        //}
    }
}
