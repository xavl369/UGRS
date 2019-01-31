using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.GPS.Services;
using UGRS.Core.SDK.DI.GPS.Tables;
using UGRS.Core.Utility;

namespace UGRS.Service.GPS
{
    public class ReadCsvTime
    {
        public static void VerifyFilesTime(List<string> plLstfiles, string pstrPath)
        {
            ImportedReportService lObjImportedReportService = new ImportedReportService();
            Console.WriteLine("Verificando archivos...");
            // var lLstFiles = ReadCSV.ReadFiles(pstrPath);
            int lIntTotalArchivos = 0;
            foreach (string lStrFile in plLstfiles)
            {
                bool lBolExistFile = lObjImportedReportService.Exist(lStrFile);
                if (!lBolExistFile)
                {
                    List<TimeEngine> lListTimeEngine = ReadTimeEngine(pstrPath + "\\" + lStrFile);
                    if (lListTimeEngine.Count > 0)
                    {
                        if (SaveData(lListTimeEngine, lStrFile))
                        {
                            if (ImportFiles.InsertImportedReport(lStrFile))
                            {
                                lIntTotalArchivos++;
                            }
                        }
                    }

                }
            }
            LogUtility.Write("Total archivos de \"Tiempo de motor\" guardados: " + lIntTotalArchivos);
            Console.WriteLine("Total archivos \"Tiempo de motor\" guardados: " + lIntTotalArchivos);
        }

        ///<summary>    Reads time engine. </summary>
        ///<remarks>    Amartinez, 22/05/2017. </remarks>
        ///<param name="pstrPath">  Full pathname of the pstr file. </param>
        public static List<TimeEngine> ReadTimeEngine(string pstrPath)
        {
            string lStrline;
            List<TimeEngine> lListTimeEngine = new List<TimeEngine>();
            using (var lObjfile = File.OpenRead(pstrPath))
            using (var lObjreader = new StreamReader(lObjfile))
            {
                lStrline = lObjreader.ReadLine();
              
                try
                {
                    Console.Write("Verificando archivo Horas de motor: "+Path.GetFileName(pstrPath));
                    while (!lObjreader.EndOfStream)
                    {
                        lStrline = lObjreader.ReadLine();
                        TimeEngine lobjTimeEngine = new TimeEngine();
                        var lArrValues = lStrline.Split(',');

                        lobjTimeEngine.AccountName = lArrValues[0];
                        lobjTimeEngine.AccountNumber = Convert.ToInt32(lArrValues[1]);
                        lobjTimeEngine.DateStart = DateTime.ParseExact(lArrValues[2], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        lobjTimeEngine.DateEnd = DateTime.ParseExact(lArrValues[3], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        //
                        lobjTimeEngine.MachineName = lArrValues[4];
                        lobjTimeEngine.PinOrVin = lArrValues[5];
                        lobjTimeEngine.TerminalId = lArrValues[6];
                        lobjTimeEngine.Brand = lArrValues[7];
                        lobjTimeEngine.Model = lArrValues[8];
                        lobjTimeEngine.MachineType = lArrValues[9];
                        lobjTimeEngine.MachineGroup = lArrValues[10];
                        lobjTimeEngine.OtherOwner = lArrValues[11];

                        lobjTimeEngine.LastCall = DateTime.ParseExact(lArrValues[12], "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture);//verificar si es correcto o yyyy/MM/dd
                        lobjTimeEngine.Location = lArrValues[13];
                        lobjTimeEngine.AreasControl = lArrValues[14];
                        lobjTimeEngine.Period = float.Parse(lArrValues[15]);
                        lobjTimeEngine.LastPeriod = float.Parse(lArrValues[16]);
                        if (lArrValues[17] != "")
                            lobjTimeEngine.Week1 = float.Parse(lArrValues[17]);
                        if (lArrValues[18] != "")
                        lobjTimeEngine.Week2 = float.Parse(lArrValues[18]);
                        if (lArrValues[19] != "")
                        lobjTimeEngine.Week3 = float.Parse(lArrValues[19]);
                        if (lArrValues[20] != "")
                        lobjTimeEngine.Week4 = float.Parse(lArrValues[20]);
                        if (lArrValues[21] != "")
                        lobjTimeEngine.Week5 = float.Parse(lArrValues[21]);

                        lListTimeEngine.Add(lobjTimeEngine);

                        //Lectura automatica de cualquier csv o clase
                        // faltó la validacion para cada tipo de dato 
                        //int i = 0;
                        //foreach (var lpropiedades in lobjTimeEngine.GetType().GetProperties())// revisa las propiedades
                        //{
                        //    lpropiedades.SetValue(lobjTimeEngine,lArrValues[i]);
                        //    i++;
                        //}
                    }
                    Console.WriteLine(" OK ");
                   
                }
                catch (Exception e)
                {
                    LogUtility.Write(e.Message + " En archivo: " + Path.GetFileName(pstrPath));
                    Console.WriteLine(e.Message);
                }
                return lListTimeEngine;
                
            }
           
        }

        public static bool SaveData(List<TimeEngine> pListTimeEngine, string pstrPath)
        {
            Console.Write("Guardando archivo: ");
            TimeEngineService lObjTimeEngineService = new TimeEngineService();
            foreach (TimeEngine lObjTimeEngine in pListTimeEngine)
            {
                int lIntErrorCode = lObjTimeEngineService.Add(lObjTimeEngine);
                if (lIntErrorCode != 0)
                {
                   
                    Console.WriteLine(DIApplication.Company.GetLastErrorDescription());
                    LogUtility.Write(DIApplication.Company.GetLastErrorDescription() + " En archivo: " + Path.GetFileName(pstrPath));
                    return false;
                }
            }
            Console.WriteLine(" OK ");
            ImportFiles.InsertImportedReport(pstrPath);
            Console.WriteLine("Archivo: " + Path.GetFileName(pstrPath) + " Guardado correctamente");
            LogUtility.Write("Archivo: " + Path.GetFileName(pstrPath) + " Guardado correctamente");
            return true;
        }
      
  
    }
}
