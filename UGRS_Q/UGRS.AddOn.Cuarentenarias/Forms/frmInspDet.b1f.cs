using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;

namespace UGRS.AddOn.Cuarentenarias.Forms
{
    [FormAttribute("UGRS.AddOn.Cuarentenarias.Forms.frmInspDet", "Forms/frmInspDet.b1f")]
    class frmInspDet : UserFormBase
    {
        public frmInspDet()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Matrix0 = ((SAPbouiCOM.Matrix)(this.GetItem("MtxInspD").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("btnOKInsD").Specific));
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("btnNoInsD").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_4").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("txtREDet").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("txtNPDet").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("txtTypeDet").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_8").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("txtQuaDet").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_10").Specific));
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("txtComDet").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_13").Specific));
            this.Button2 = ((SAPbouiCOM.Button)(this.GetItem("btnAddCom").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private SAPbouiCOM.Matrix Matrix0;

        private void OnCustomInitialize()
        {

        }

        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Button Button1;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.Button Button2;
    }
}
