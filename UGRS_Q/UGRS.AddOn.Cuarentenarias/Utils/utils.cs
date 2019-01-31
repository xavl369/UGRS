using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.Cuarentenarias.Utils
{
    public class utils
    {
     

        /// <summary>
        /// FormExists
        /// </summary>
        /// <param name="formUid"></param>
        /// <returns></returns>
        public bool FormExists(string formUid)
        {
            bool exist = false;
            try
            {
                Application.SBO_Application.Forms.Item(formUid);

                exist = true;
            }
            catch (Exception e)
            {
                exist = false;
            }

            return exist;
        }
        public bool FormExists(string lStrTypex, int lIntTypeCount)
        {
            bool exist = false;
            try
            {
                var a = Application.SBO_Application.Forms.GetForm(lStrTypex, lIntTypeCount);
                exist = true;
            }
            catch (Exception e)
            {
                exist = false;
            }

            return exist;
        }

    }
}
