using QualisysConfig;
using QualisysLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.ServiceProcess;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.DTO.Session;
using UGRS.Core.Services;
using UGRS.Core.Utility;
using UGRS.Object.Auctions;

namespace UGRS.Service.Auctions
{
    public class Program
    {
        public static AuctionsServicesFactory mObjFactory;
        private static DateTime mObjAuctionDate;
        public static AuctionsServicesFactory Factory
        {
            get { return mObjFactory; }
            set { mObjFactory = value; }
        }

        static void Main(string[] args)
        {

            Factory = new AuctionsServicesFactory();


            mObjAuctionDate = mObjFactory.GetAuctionService().LocalAuctionService.GetActiveAuction() != null ? mObjFactory.GetAuctionService().LocalAuctionService.GetActiveAuction().Date : DateTime.MinValue;
#if DEBUG

            try
            {
                StaticSessionUtility.mObjSeccion = new SessionDTO()
                {
                    Id = 0,
                    UserName = "AuctionsService"
                };
                
                //RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);

                //Console.WriteLine("El servicio se esta ejecutando...");
                //mObjAuctions = (AuctionsServerObject)Activator.GetObject(typeof(AuctionsServerObject), "http://localhost:8830/Auctions");
                //InitTablesAndFields();
                //InitConfigurationsProcess();
                //InitBusinnessPartnerProcess();
                //InitItemProcess();
                //InitBuissnesPartnerConciliation();
                //InitStockProcess(GetWhsCode());
                //InitAuctionProcess();
                //InitStockConciliations();
                //InitBatchesProcess(mObjAuctionDate);
                //InitFoodDeliveriesProcess(FoodWarehouse());
                InitOperationsProcess(GetWhsCode());

                Console.WriteLine("El servicio se ha detenido.");
                //IChannel lObjChannel = (IChannel)ChannelServices.GetChannel(QsConfig.GetValue<string>("ChannelName"));
                //ChannelServices.UnregisterChannel(lObjChannel);
            }
            catch (Exception e)
            {
                LogService.WriteError(e.ToString());
                Console.WriteLine(e.ToString());
            }

            Console.ReadLine();

#else

            ServiceBase[] ServicesToRun = new ServiceBase[] 
            {
                new AuctionsService()
            };
            ServiceBase.Run(ServicesToRun);

#endif
        }



        private static void InitTablesAndFields()
        {
            try
            {
                Factory.Reconnection();
                Factory.GetSetupService().InitializeTablesAndFields();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitConfigurationsProcess()
        {
            try
            {
                Factory.Reconnection();
                Factory.GetConfigurationService().ProcessConfigurations();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitBusinnessPartnerProcess()
        {
            try
            {
                Factory.Reconnection();
                Factory.GetBusinessPartnerService().ImportCustomers();
                Factory.GetBusinessPartnerService().UpdateCustomers();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitItemProcess()
        {
            try
            {
                Factory.Reconnection();
                Factory.GetItemService().ImportItems();
                Factory.GetItemService().UpdateItems();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitStockProcess(string pStrWarehouse)
        {
            try
            {
                Factory.Reconnection();
                Factory.GetStockService().ImportStocks(pStrWarehouse, mObjAuctionDate);
                Factory.GetStockService().UpdateStocks(pStrWarehouse);

            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitAuctionProcess()
        {
            try
            {
                Factory.Reconnection();
                Factory.GetAuctionService().ExportAuctions(GetLocation());
                Factory.GetAuctionService().UpdateAuctions(GetLocation());
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitBatchesProcess(DateTime pDtAuctionDate)
        {
            try
            {
                Factory.Reconnection();
                Factory.GetBatchService().ExportBatches(pDtAuctionDate);
                Factory.GetBatchService().UpdateBatches(pDtAuctionDate);
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitBatchLinesProcess()
        {
            try
            {
                Factory.GetBatchLineService().ExportBatchLines(GetLocation());
                Factory.GetBatchLineService().UpdateBatchLines(GetLocation());
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
        }

        private static void InitFoodDeliveriesProcess(string pStrWarehouse)
        {
            try
            {
                Factory.GetFoodDeliveryService().ImportFoodDeliveries(pStrWarehouse);
                Factory.GetFoodDeliveryService().UpdateFoodDeliveries(pStrWarehouse);
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitFoodCheckProcess()
        {
            try
            {
                Factory.GetFoodChargeService().SetFoodChargesChecks(mObjAuctionDate);
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitOperationsProcess()
        {
            try
            {
                Factory.GetOperationsService().ProcessReOpenedAuctions();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
        }

        private static void InitBuissnesPartnerConciliation()
        {
            try
            {
                QsLog.WriteProcess("Procesando clientes temporales...");
                Factory.GetOperationsService().ConciliatePartners();
                QsLog.WriteProcess("clientes temporales procesados.");
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }
    
        private static void InitStockConciliations()
        {
            try
            {
                QsLog.WriteProcess("Procesando entradas temporales...");
                Factory.GetOperationsService().ConciliateStock(mObjAuctionDate);
                QsLog.WriteProcess("Entradas temporales procesadas.");
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void InitOperationsProcess(string pStrWarehouse)
        {
            try
            {

                QsLog.WriteProcess("Procesando subastas cerradas...");
                Factory.GetOperationsService().ProcessClosedAuctions();
                QsLog.WriteProcess("Subastas procesadas.");

                QsLog.WriteProcess("Procesando Pagos de facturas generadas");
                Factory.GetOperationsService().ProcessPayments();

                QsLog.WriteProcess("Procesando subastas Re abiertas...");
                Factory.GetOperationsService().ProcessReOpenedAuctions();
                QsLog.WriteProcess("Subastas procesadas.");

                InitStockProcess(pStrWarehouse);

            }
            catch (Exception lObjException)
            {
                QsLog.WriteException(lObjException);
            }
            finally
            {
                GC.Collect();
            }
        }

        private int GetIntervalTime()
        {
            return ConfigurationManager.AppSettings["TimeInterval"] != null ?
                Convert.ToInt32(ConfigurationManager.AppSettings["TimeInterval"].ToString()) : 60000;
        }

        public static string GetWhsCode()
        {
            return QsConfig.GetValue<string>("AuctionsWarehouse");
        }

        public static string FoodWarehouse()
        {
            return QsConfig.GetValue<string>("FoodWarehouse");
        }

        public static string GetLocation()
        {
            return QsConfig.GetValue<string>("CostCenter");
        }

    }
}
