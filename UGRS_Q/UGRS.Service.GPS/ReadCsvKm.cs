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
    public class ReadCsvKm
    {

        ///<summary>    Verify files. </summary>
        ///<remarks>    Amartinez, 22/05/2017. </remarks>
        ///<param name="plLstfiles">    The pl lstfiles. </param>
        ///<param name="pstrPath">      Full pathname of the pstr file. </param>

        public static void VerifyFilesKM(List<string> plLstfiles, string pstrPath)
        {
            ImportedReportService lObjImortedReportService = new ImportedReportService();
            Console.WriteLine("Verificando archivos...");

            // int iIntTotalRegistros = 0;
            int lIntTotalArchivos = 0;
            foreach (string lStrFile in plLstfiles)
            {
                bool lBolExistFile = lObjImortedReportService.Exist(lStrFile);
                if (!lBolExistFile)
                {
                    List<KilometersTraveled> lLstKilometers = ReadKilometers(pstrPath + "\\" + lStrFile);
                    if (lLstKilometers.Count > 0)
                    {
                        if (SaveData(lLstKilometers, lStrFile))
                        {
                            if (ImportFiles.InsertImportedReport(lStrFile))
                            {
                                //iIntTotalRegistros += lLstKilometers.Count;
                                lIntTotalArchivos++;
                            }
                        }
                    }
                }
            }
            LogUtility.Write("Total archivos \"Distancia recorrida\" guardados: " + lIntTotalArchivos);
            Console.WriteLine("Total archivos \"Distancia recorrida\" guardados: " + lIntTotalArchivos);
        }


        ///<summary>    Reads the kilometers. </summary>
        ///<remarks>    Amartinez, 19/05/2017. </remarks>
        ///<param name="pstrPath">  Full pathname of the pstr file. </param>

        public static List<KilometersTraveled> ReadKilometers(string pstrPath)
        {
            string lStrline;
            List<KilometersTraveled> lListKilometersTraveled = new List<KilometersTraveled>();
            using (var lObjfile = File.OpenRead(pstrPath))
            using (var lObjreader = new StreamReader(lObjfile))
            {
                lStrline = lObjreader.ReadLine();
                try
                {
                    Console.Write("Verificando archivo KMrecorridos: " + Path.GetFileName(pstrPath));
                    while (!lObjreader.EndOfStream)
                    {
                        lStrline = lObjreader.ReadLine();
                        KilometersTraveled lobjKilometesTraveled = new KilometersTraveled();
                        var lArrValues = lStrline.Split(',');

                        if (string.IsNullOrEmpty(lArrValues[1].Trim())) //Verifica salto de linea
                        {
                            //lArrValues[0] = lArrValues[0].Replace("\"", "");
                            //lArrValues[0] = lArrValues[0].Replace("\\", "");
                            //lobjKilometesTraveled.Name = lArrValues[0];
                            lStrline = lObjreader.ReadLine();// Salto de linea de archivo
                            //lStrline = lStrline.Replace("\"", "");
                            //lStrline = lStrline.Replace("\\", "");
                            lStrline = lStrline.Replace("<br>", "");
                            lArrValues = lStrline.Split(new[] { "\",\"" }, StringSplitOptions.None);
                        }
                        var lArrValuesNew = lStrline.Split(new[] { "," }, StringSplitOptions.None);

                        string lStrDir1 = string.Empty;
                        List<string> lListString = new List<string>();
                        int lIntInicio = 0;
                        for (int i = 0; i < lArrValuesNew.Length; i++)
                        {
                            if (lIntInicio == 0)
                            {
                                if (lArrValuesNew[i].Contains("\""))
                                {
                                    lStrDir1 += lArrValuesNew[i];
                                    lIntInicio++;
                                }
                                else
                                {
                                    lListString.Add(lArrValuesNew[i].Replace("\"", ""));
                                }
                            }
                            else
                            {
                                if (lIntInicio == 1)
                                {
                                    if (lArrValuesNew[i].Contains("\""))
                                    {
                                        lStrDir1 += lArrValuesNew[i];
                                        lIntInicio=0;
                                        lListString.Add(lStrDir1.Replace("\"",""));
                                        lStrDir1 = string.Empty;
                                    }
                                    else
                                    {
                                        lStrDir1 += ", "+lArrValuesNew[i];
                                    }
                                }
                                
                            }
                        }
                        if (!string.IsNullOrEmpty(lStrDir1))
                            lListString.Add(lStrDir1.Replace("\"", ""));

                        var lArrDir = lStrline.Split(new[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
                        lArrValues[0] = lArrValues[0].Replace("\"", "");
                        lobjKilometesTraveled.Name = lListString[0];
                        var fecha = DateTime.Parse(lListString[1]);
                        lobjKilometesTraveled.FromDate = DateTime.Parse(lListString[1]);
                        lobjKilometesTraveled.FromAddress = lListString[2];
                        lobjKilometesTraveled.ToDate = DateTime.Parse(lListString[3]);
                        lobjKilometesTraveled.ToAddress = lListString[4];
                        lobjKilometesTraveled.Distance = float.Parse(lListString[5]);
                        lobjKilometesTraveled.Duration = lListString[6];
                        lobjKilometesTraveled.MaxVelocity = lListString[7];
                        lobjKilometesTraveled.OdometerStart = float.Parse(lListString[8]);
                        lobjKilometesTraveled.OdometerEnd = float.Parse(lListString[9]);
                        if (lListString[10] == "-")
                        {
                            lobjKilometesTraveled.MotorHours = 0;//Convert.ToInt32(lArrValues[10]);
                        }
                        else
                        {
                            lobjKilometesTraveled.MotorHours = Convert.ToInt32(lListString[10]);
                        }
                        lListKilometersTraveled.Add(lobjKilometesTraveled);

                    }
                    Console.WriteLine("OK");

                }
                catch (Exception e)
                {

                    LogUtility.Write(e.Message + " En archivo: " + Path.GetFileName(pstrPath));
                    Console.WriteLine(e.Message);
                }
                return lListKilometersTraveled;
            }
        }

        public static bool SaveData(List<KilometersTraveled> pLstKilometersTraveled, string pStrPath)
        {
            Console.Write("Guardando archivo: ");
            KilometersTraveledService lObjKilometersTraveledService = new KilometersTraveledService();

            foreach (KilometersTraveled lobjKilometerstraveled in pLstKilometersTraveled)
            {
                int lIntErrorCode = lObjKilometersTraveledService.Add(lobjKilometerstraveled);
                if (lIntErrorCode != 0)
                {

                    Console.WriteLine(DIApplication.Company.GetLastErrorDescription());
                    LogUtility.Write(DIApplication.Company.GetLastErrorDescription() + " En archivo: " + Path.GetFileName(pStrPath));
                    return false;
                }
            }
            Console.WriteLine("OK");
            LogUtility.Write("Archivo: " + Path.GetFileName(pStrPath) + " Guardado correctamente");
            Console.WriteLine("Archivo: " + Path.GetFileName(pStrPath) + " Guardado correctamente");
            return true;
        }
    }
}
