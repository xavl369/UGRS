using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;

namespace UGRS.Application.Test
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            DIApplication.DIConnect();
            InitializeComponent();
            tmrWorkerOne.Start();
            tmrWorkerTwo.Start();
            tmrWorkerThree.Start();
            tmrWorkerFour.Start();
        }

        private void tmrWorkerOne_Tick(object sender, EventArgs e)
        {
            tmrWorkerOne.Enabled = false;
            Recordset lObjRecordset = null;
            try
            {
                lObjRecordset = DIApplication.GetRecordset();
                lObjRecordset.DoQuery("SELECT ItemCode FROM OITM");

                lstThreadOne.Items.Clear();
                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lstThreadOne.Items.Add(lObjRecordset.Fields.Item(0).Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                lstThreadOne.Items.Add(ex.ToString());
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
                tmrWorkerOne.Enabled = true;
            }
        }

        private void tmrWorkerTwo_Tick(object sender, EventArgs e)
        {
            tmrWorkerTwo.Enabled = false;
            Recordset lObjRecordset = null;
            try
            {
                lObjRecordset = DIApplication.GetRecordset();
                lObjRecordset.DoQuery("SELECT CardCode FROM OCRD");

                lstThreadTwo.Items.Clear();
                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lstThreadTwo.Items.Add(lObjRecordset.Fields.Item(0).Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                lstThreadTwo.Items.Add(ex.ToString());
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
                tmrWorkerTwo.Enabled = true;
            }
        }

        private void tmrWorkerThree_Tick(object sender, EventArgs e)
        {
            tmrWorkerThree.Enabled = false;
            Recordset lObjRecordset = null;
            try
            {
                lObjRecordset = DIApplication.GetRecordset();
                lObjRecordset.DoQuery("SELECT ItemCode FROM OITM");

                lstThreadThree.Items.Clear();
                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lstThreadThree.Items.Add(lObjRecordset.Fields.Item(0).Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                lstThreadThree.Items.Add(ex.ToString());
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
                tmrWorkerThree.Enabled = true;
            }
        }

        private void tmrWorkerFour_Tick(object sender, EventArgs e)
        {
            tmrWorkerFour.Enabled = false;
            Recordset lObjRecordset = null;
            try
            {
                lObjRecordset = DIApplication.GetRecordset();
                lObjRecordset.DoQuery("SELECT CardCode FROM OCRD");

                lstThreadFour.Items.Clear();
                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lstThreadFour.Items.Add(lObjRecordset.Fields.Item(0).Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                lstThreadFour.Items.Add(ex.ToString());
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
                tmrWorkerFour.Enabled = true;
            }
        }

        private void frmTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrWorkerOne.Stop();
            tmrWorkerTwo.Stop();
            tmrWorkerThree.Stop();
            tmrWorkerFour.Stop();

            //this.Close();
        }
    }
}
