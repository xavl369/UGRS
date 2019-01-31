using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.FoodProduction.UI.Matriz
{
    public class Validations
    {
        QueryManager mObjQueryManager = new QueryManager();
        /// <summary>
        /// Varifica que todas las lines tengan segundo peso si tienen un primer peso
        /// </summary>
        public bool VerificarSegundoPeso(SAPbouiCOM.IMatrix mObjMatrix)
        {
            for (int i = 1; i <= mObjMatrix.RowCount; i++)
            {
                string lStrPeso1 = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso1").Cells.Item(i).Specific).Value;
                string lStrPeso2 = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso2").Cells.Item(i).Specific).Value;

                if (lStrPeso1 != "0.0" && lStrPeso2 == "0.0")
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsLastWeight(SAPbouiCOM.IMatrix mObjMatrix)
        {
            bool lBolLast = true;
            try
            {
                for (int i = 1; i <= mObjMatrix.RowCount; i++)
                {
                    //UPDATE RCordova 
                    if (i == mObjMatrix.Columns.Item(i).Cells.Count && ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value == ""
                         && (((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value == "0.0"))
                    {
                        mObjMatrix.DeleteRow(i);
                        i--;
                    }
                    if (((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value == "0.0")
                    {
                        lBolLast = false;
                    }
                    if (((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value == "0.0"
                       && ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value == "")
                    {
                        lBolLast = true;
                    }


                }
            }
            catch (Exception)
            {
                return false;

            }
            
            return lBolLast;
        }

        public bool VerificarCheck(SAPbouiCOM.IMatrix mObjMatrix)
        {
            SAPbouiCOM.CommonSetting lObjRowCtrl;
            lObjRowCtrl = mObjMatrix.CommonSetting;
            bool lBolActivateButtons = false;
            for (int i = 1; i <= mObjMatrix.RowCount; i++)
            {
                if ((mObjMatrix.Columns.Item("Check").Cells.Item(i).Specific as CheckBox).Checked)
                {
                    lBolActivateButtons = true;
                }
            }
            return lBolActivateButtons;
        }


        public bool ValidateWeight(string pStrTypeTicket, double pDblPesoNeto, double pDblPeso2, SAPbouiCOM.IMatrix pObjMatrix, int pIntRow)
        {
            bool lBolPesoIncorrecto = true;
            if (pStrTypeTicket == "Venta" && pDblPesoNeto < 0 && pDblPeso2 != 0)
            {
                pDblPesoNeto = 0;
                UIApplication.ShowMessageBox(string.Format("Error al verificar los datos: Peso neto incorrecto para venta"));
                lBolPesoIncorrecto = false;
                (pObjMatrix.Columns.Item("Peso2").Cells.Item(pIntRow).Specific as EditText).Value = "0.0";
            }
            if (pStrTypeTicket == "Compra" && pDblPesoNeto > 0 && pDblPeso2 != 0)
            {
                UIApplication.ShowMessageBox(string.Format("Error al verificar los datos: Peso neto incorrecto para compra"));
                lBolPesoIncorrecto = false;
                (pObjMatrix.Columns.Item("Peso2").Cells.Item(pIntRow).Specific as EditText).Value = "0.0";
            }
            return lBolPesoIncorrecto;
        }

        public bool VerifyBagBales(int pIntBag, string pStrItemCode)
        {
            bool lBolcorrect = true;
            string lChaBag = "N";

            lChaBag = mObjQueryManager.GetValue("QryGroup29", "ItemCode", pStrItemCode, "OITM");
            if (lChaBag == "Y" && pIntBag == 0)
            {
                return false;
            }
            return lBolcorrect;
        }
    }
}
