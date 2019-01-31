using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Connection;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.Models;
using UGRS.Core.Utility;
using System.Linq;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please wait...");
            DIApplication.DIConnect();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Connection successful");

            SAPbobsCOM.Documents lObjDocument = null;
            lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);

            var a = lObjDocument.GetByKey(185);
            var b = lObjDocument.GroupNumber;
            var c = lObjDocument.PaymentMethod;
            var d = lObjDocument.ExtraDays;
            var e = lObjDocument.PaymentGroupCode;

            ////SAPbobsCOM.JournalEntries lObjJournalEntries = null;
            //SAPbobsCOM.Documents lObjDocument = null;
            ////int lIntResult = -1;

            //try
            //{
            //    Console.WriteLine("Please wait...");
            //    DIApplication.DIConnect();
            //    Console.ForegroundColor = ConsoleColor.Yellow;
            //    Console.WriteLine("Connection successful");

            //    lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);
            //    lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
            //    lObjDocument.Series = 297;
            //    lObjDocument.DocDate = DateTime.Now;
            //    lObjDocument.DocDueDate = DateTime.Now;
            //    lObjDocument.Comments = "Salida de mercancías";
            //    lObjDocument.JournalMemo = "Salida de mercancías";

            //    lObjDocument.Lines.ItemCode = "A00000468";
            //    lObjDocument.Lines.WarehouseCode = "SUHEG";
            //    lObjDocument.Lines.Quantity = 20;

            //    lObjDocument.Lines.BatchNumbers.Quantity = 20;
            //    lObjDocument.Lines.BatchNumbers.BatchNumber = "Prueba161017_2";
            //    lObjDocument.Lines.BatchNumbers.Add();

            //    lObjDocument.Lines.Add();

            //    if (lObjDocument.Add() != 0)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine(DIApplication.Company.GetLastErrorDescription());
            //    }


            //    //lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
            //    //lObjDocument.GetByKey(53);

            //    //if (lObjDocument != null)
            //    //{
            //    //    for (int i = 0; i < lObjDocument.Lines.Count; i++)
            //    //    {
            //    //        lObjDocument.Lines.SetCurrentLine(i);

            //    //        var a = lObjDocument.Lines.CostingCode;
            //    //        var b = lObjDocument.Lines.CostingCode2;
            //    //        var c = lObjDocument.Lines.CostingCode3;
            //    //        var d = lObjDocument.Lines.CostingCode4;
            //    //        var f = lObjDocument.Lines.CostingCode5;
            //    //    }
            //    //}


            //    //lObjJournalEntries = (SAPbobsCOM.JournalEntries)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
            //    //lObjJournalEntries.DueDate = DateTime.Today;
            //    //lObjJournalEntries.TaxDate = DateTime.Today;
            //    //lObjJournalEntries.AutoVAT = SAPbobsCOM.BoYesNoEnum.tYES;

            //    //lObjJournalEntries.Lines.AccountCode = "1070050001000";
            //    //lObjJournalEntries.Lines.ContraAccount = "2040010004000";
            //    //lObjJournalEntries.Lines.Credit = 0;
            //    //lObjJournalEntries.Lines.Debit = 100;
            //    //lObjJournalEntries.Lines.CostingCode = "SU_HERMO";
            //    //lObjJournalEntries.Lines.UserFields.Fields.Item("U_GLO_Auxiliary").Value = "CL00000001";
            //    //lObjJournalEntries.Lines.UserFields.Fields.Item("U_GLO_AuxType").Value = "1";
            //    //lObjJournalEntries.Lines.UserFields.Fields.Item("U_SU_Folio").Value = "SU-HE-170001";
            //    //lObjJournalEntries.Lines.Add();

            //    //lObjJournalEntries.Lines.AccountCode = "2040010004000";
            //    //lObjJournalEntries.Lines.ContraAccount = "1070050001000";
            //    //lObjJournalEntries.Lines.Credit = 100;
            //    //lObjJournalEntries.Lines.Debit = 0;
            //    //lObjJournalEntries.Lines.CostingCode = "SU_HERMO";
            //    //lObjJournalEntries.Lines.UserFields.Fields.Item("U_GLO_Auxiliary").Value = "CL00000002";
            //    //lObjJournalEntries.Lines.UserFields.Fields.Item("U_GLO_AuxType").Value = "1";
            //    //lObjJournalEntries.Lines.UserFields.Fields.Item("U_SU_Folio").Value = "SU-HE-170001";
            //    //lObjJournalEntries.Lines.Add();

            //    //lIntResult = lObjJournalEntries.Add();

            //    //if (lIntResult != 0)
            //    //{
            //    //    Console.WriteLine(DIApplication.Company.GetLastErrorDescription());
            //    //}

            //    //DIApplication.Company.StartTransaction();
            //    //CreateDocument("62");
            //    //CreateDocument("63");
            //    //DIApplication.Company.EndTransaction(BoWfTransOpt.wf_RollBack);
            //}
            //catch (Exception lObjException)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(lObjException.ToString());
            //}
            //finally
            //{
            //    //MemoryUtility.ReleaseComObject(lObjJournalEntries);
            //    MemoryUtility.ReleaseComObject(lObjDocument);
            //}

            Console.WriteLine("Please wait...");

            EntityFrameworkTransactionTest mObjTest = new EntityFrameworkTransactionTest();
            mObjTest.DoTest();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
        }

        //private static void CreateInvoice()
        //{
        //    SAPbobsCOM.Documents lObjDocument = null;
        //    int lIntResult = -1;

        //    try
        //    {
        //        DIApplication.DIConnect();

        //        int lIntSeries = new QueryManager().GetSeriesByName(((int)SAPbobsCOM.BoObjectTypes.oInvoices).ToString(), "SUHE");

        //        lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
        //        lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
        //        lObjDocument.CardCode = "Alb";
        //        lObjDocument.Series = lIntSeries;
        //        lObjDocument.DocDate = DateTime.Today;
        //        lObjDocument.DocDueDate = DateTime.Today;
        //        lObjDocument.Comments = "Cobro de alimento desde Subastas.";
        //        lObjDocument.JournalMemo = "Cobro de alimento.";

        //        lObjDocument.Lines.ItemCode = "A00000001";
        //        lObjDocument.Lines.WarehouseCode = "SUHE";
        //        lObjDocument.Lines.TaxCode = "v16";
        //        lObjDocument.Lines.Quantity = 10;

        //        lObjDocument.Lines.Add();
        //        lIntResult = lObjDocument.Add();

        //        if (lIntResult != 0)
        //        {
        //            Console.WriteLine(DIApplication.Company.GetLastErrorDescription());
        //        }

        //        //DIApplication.Company.StartTransaction();
        //        //CreateDocument("62");
        //        //CreateDocument("63");
        //        //DIApplication.Company.EndTransaction(BoWfTransOpt.wf_RollBack);
        //    }
        //    catch (Exception lObjException)
        //    {
        //        Console.WriteLine(lObjException.ToString());
        //    }
        //}

        //private static void CreateDocument(string pStrFolio)
        //{
        //    SAPbobsCOM.Documents lObjDocument = null;
        //    int lIntResult = -1;

        //    try
        //    {
        //        int lIntSeries = new QueryManager().GetSeriesByName(((int)SAPbobsCOM.BoObjectTypes.oInventoryGenEntry).ToString(), "SUHE");
        //        //SAPbobsCOM.
        //        lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
        //        lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
        //        lObjDocument.Series = lIntSeries;
        //        //lObjDocument.SeriesString = "SUHE";
        //        lObjDocument.DocDate = DateTime.Today;
        //        lObjDocument.DocDueDate = DateTime.Today;
        //        lObjDocument.UserFields.Fields.Item("U_GLO_Ticket").Value = pStrFolio;
        //        lObjDocument.Comments = "Importado desde entrada de mercancía temporal de Subastas.";
        //        lObjDocument.JournalMemo = "Entrada de mercancías";

        //        //lObjDocument.Lines.COGSCostingCode
        //        lObjDocument.Lines.CostingCode = "CR_CORRA";
        //        lObjDocument.Lines.ItemCode = "Vaquilla";
        //        lObjDocument.Lines.ItemDescription = "Vaquilla";
        //        lObjDocument.Lines.WarehouseCode = "SUHEG";
        //        //lObjDocument.Lines.Price = i.Price;
        //        lObjDocument.Lines.Quantity = 10;
        //        lObjDocument.Lines.AccountCode = "1150030003000";
        //        //lObjDocument.Lines.DiscountPercent = 0.0;

        //        lObjDocument.Lines.BatchNumbers.Quantity = 10;
        //        lObjDocument.Lines.BatchNumbers.BatchNumber = string.Format("S_RAUL FRANC{0}", DateTime.Today.ToString("ddMMyy"));
        //        lObjDocument.Lines.BatchNumbers.ExpiryDate = DateTime.Today.AddDays(1);
        //        lObjDocument.Lines.BatchNumbers.Notes = "Importado desde entrada de mercancía temporal de Subastas.";
        //        lObjDocument.Lines.BatchNumbers.ManufacturerSerialNumber = "CL00002800";
        //        lObjDocument.Lines.BatchNumbers.Quantity = 10;

        //        lObjDocument.Lines.BatchNumbers.Add();
        //        lObjDocument.Lines.Add();
        //        lIntResult = lObjDocument.Add();

        //        if (lIntResult != 0)
        //        {
        //            Console.Write(DIApplication.Company.GetLastErrorDescription());
        //        }
        //    }
        //    catch (Exception lObjException)
        //    {
        //        Console.WriteLine(lObjException.ToString());
        //    }
        //    finally
        //    {
        //        MemoryUtility.ReleaseComObject(lObjDocument);
        //    }
        //}
    }
}
