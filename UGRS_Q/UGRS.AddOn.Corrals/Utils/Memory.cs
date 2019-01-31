using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ContabilidadElectronicaAddOn.Utils
{
    public class Memory
    {
        public static void ReleaseComObject(object ComObject)
        {
            if (ComObject == null)
                return;
            Marshal.ReleaseComObject(ComObject);
            Marshal.FinalReleaseComObject(ComObject);
            ComObject = (object)null;
        }

        public static void ReleaseComObject(params object[] ComObject)
        {
            for (int index = 0; index < ComObject.Length; ++index)
            {
                if (ComObject[index] != null)
                {
                    Marshal.ReleaseComObject(ComObject[index]);
                    Marshal.FinalReleaseComObject(ComObject[index]);
                    ComObject[index] = (object)null;
                }
            }
        }
    }
}
