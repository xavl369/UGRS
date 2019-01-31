using System;
using System.Collections.Generic;
using System.IO;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.GPS.Services;
using UGRS.Core.SDK.DI.GPS.Tables;
using UGRS.Core.Utility;

namespace UGRS.Service.GPS
{
    public class ImportFiles
    {
        ///<summary>    Inserts an imorted report described by pstrPath. </summary>
        ///<remarks>    Amartinez, 22/05/2017. </remarks>
        ///<param name="pStrPath">  Full pathname of the pstr file. </param>

        public static bool InsertImportedReport(string pStrPath)
        {
            try
            {
                ImportedReportService lObjImortedReportService = new ImportedReportService();
                ImportedReport lObjImportedReport = new ImportedReport();

                lObjImportedReport.FileName = Path.GetFileName(pStrPath);
                lObjImportedReport.Date = DateTime.Now;
                int lIntErrorCode = lObjImortedReportService.Add(lObjImportedReport);
                if (lIntErrorCode != 0)
                {
                    Console.WriteLine(DIApplication.Company.GetLastErrorDescription());
                    LogUtility.Write(DIApplication.Company.GetLastErrorDescription() + "ImportedReport  En archivo: " + Path.GetFileName(pStrPath));
                }
            }
            catch (Exception ex)
            {
                LogUtility.Write("Archivo: " + Path.GetFileName(pStrPath) + " Guardado correctamente");
                return false;
            }
            return true;
        }

        ///<summary>    Searches for the first files. </summary>
        ///<remarks>    Amartinez, 23/05/2017. </remarks>
        ///<param name="pstrPath">  Full pathname of the pstr file. </param>
        ///<returns>    The found files. </returns>
        public static List<string> FindFiles(string pstrPath)
        {
            List<string> lLstFiles = new List<string>();
            DirectoryInfo lDtIDirectorio = new DirectoryInfo(pstrPath);
            FileInfo[] lFlIArchivos = lDtIDirectorio.GetFiles("*.csv");
            foreach (FileInfo lFliIarchivo in lFlIArchivos)
            {
                lLstFiles.Add(lFliIarchivo.Name);
            }
            return lLstFiles;
        }
    }
}
