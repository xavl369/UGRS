using System;

namespace UGRS.Core.Application.Utility
{
    public static class ExceptionUtility
    {
        public static void CheckArgumentNotNull(object pObjArgument, string pStrName)
        {
            if (pObjArgument == null)
            {
                throw new ArgumentNullException(pStrName);
            }
        }
    }
}
