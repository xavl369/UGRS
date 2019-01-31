using System;
using System.IO;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.GPS.Services;
using UGRS.Core.SDK.DI.GPS.Tables;
using UGRS.Core.Utility;

namespace UGRS.Service.GPS
{
    public class InsertReport
    {
        ///<summary>    Inserts an imorted report described by pstrPath. </summary>
        ///<remarks>    Amartinez, 22/05/2017. </remarks>
        ///<param name="pstrPath">  Full pathname of the pstr file. </param>

        public static void InsertImportedReport(string pstrPath)
        {
            ImportedReportService lObjImortedReportService = new ImportedReportService();
            ImportedReport lObjImportedReport = new ImportedReport();

            lObjImportedReport.FileName = Path.GetFileName(pstrPath);
            lObjImportedReport.Date = DateTime.Now;
            int lIntErrorCode = lObjImortedReportService.Add(lObjImportedReport);
            if (lIntErrorCode != 0)
            {
                Console.WriteLine(DIApplication.Company.GetLastErrorDescription());
                LogUtility.Write(DIApplication.Company.GetLastErrorDescription() + "ImportedReport  En archivo: " + Path.GetFileName(pstrPath));
            }
        }
    }
}
