using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;

namespace UGRS.AddOn.Cuarentenarias
{
    [FormAttribute("UGRS.AddOn.Cuarentenarias.frmModIns", "Forms/frmModIns.b1f")]
    class frmModIns : UserFormBase
    {
        public frmModIns()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("btnModOk").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("txtTotkg").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("lblTotkg").Specific));
            this.Matrix0 = ((SAPbouiCOM.Matrix)(this.GetItem("MtxInsp").Specific));
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("btnModNo").Specific));
            this.Button2 = ((SAPbouiCOM.Button)(this.GetItem("btnVal").Specific));
            //     this.Button2.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button2_ClickBefore);
            this.Matrix2 = ((SAPbouiCOM.Matrix)(this.GetItem("MtxCert").Specific));
            //    this.Matrix2.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.Matrix2_ClickBefore);
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("txtRE").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_4").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_5").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("txtNP").Specific));
            this.CheckBox0 = ((SAPbouiCOM.CheckBox)(this.GetItem("chkAdu").Specific));
            this.CheckBox1 = ((SAPbouiCOM.CheckBox)(this.GetItem("chkInspE").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private SAPbouiCOM.Button Button0;

        private void OnCustomInitialize()
        {

        }
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.Matrix Matrix0;
        private SAPbouiCOM.Button Button1;
        private SAPbouiCOM.Button Button2;
        private SAPbouiCOM.Matrix Matrix2;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.CheckBox CheckBox0;
        private SAPbouiCOM.CheckBox CheckBox1;


        /*
        private void Button2_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            throw new System.NotImplementedException();

        }
        */

    }
}
