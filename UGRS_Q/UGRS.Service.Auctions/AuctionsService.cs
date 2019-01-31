using QualisysConfig;
using QualisysLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;
using UGRS.Core.Auctions.DTO.Session;
using UGRS.Core.Services;
using UGRS.Core.Utility;
using UGRS.Object.Auctions;

namespace UGRS.Service.Auctions
{
    partial class AuctionsService : ServiceBase
    {
        private Timer mTmrService;
        private AuctionsServicesFactory mObjFactory;
        private System.Threading.Thread mObjWorker;
        private bool mBolInitialized;
        private DateTime mAuctionDate;

        public AuctionsServicesFactory Factory
        {
            get { return mObjFactory; }
            set { mObjFactory = value; }
        }

        public AuctionsService()
        {
            LogService.WriteInfo("Constructor");
            InitializeComponent();
            LogService.WriteInfo("inicializado");
            Factory = new AuctionsServicesFactory();

            mTmrService = new Timer
            {
                AutoReset = true,
                Interval = GetIntervalTime()
            };

            mTmrService.Elapsed += Timer_Elapsed;


            mAuctionDate = mObjFactory.GetAuctionService().LocalAuctionService.GetActiveAuction() != null
                ? mObjFactory.GetAuctionService().LocalAuctionService.GetActiveAuction().Date : DateTime.MinValue;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                LogService.WriteInfo("Servicio iniciado");

                mObjWorker = new System.Threading.Thread(() => StartAllProcess());
                mObjWorker.Start();

                mTmrService.Start();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                mTmrService.Stop();
                LogService.WriteInfo("Servicio detenido");
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
        }

        private void StartAllProcess()
        {
            LogService.WriteInfo("Iniciando procesos de importación...");
            mBolInitialized = false;

            StaticSessionUtility.mObjSeccion = new SessionDTO()
            {
                Id = 0,
                UserName = "AuctionsService"
            };

            InitTablesAndFields();
            InitConfigurationsProcess();
            InitBusinnessPartnerProcess();
            InitItemProcess();
            InitBuissnesPartnerConciliation();
            InitStockProcess(GetWhsCode());
            InitAuctionProcess();
            InitStockConciliations();
            InitBatchesProcess(mAuctionDate);
            InitBatchLinesProcess();
            InitFoodDeliveriesProcess(GetFoodWhsCode());
            InitOperationsProcess(GetWhsCode());

            LogService.WriteInfo("Procesos de importación finalizados.");
            mBolInitialized = true;
        }

        private void InitTablesAndFields()
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Validando tablas definidas por usuario...");
                Factory.GetSetupService().InitializeTablesAndFields();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void InitBusinnessPartnerProcess()
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Importando socios de negocio...");
                Factory.GetBusinessPartnerService().ImportCustomers();

                LogService.WriteInfo("Actualizando socios de negocio...");
                Factory.GetBusinessPartnerService().UpdateCustomers();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void InitItemProcess(string pStrWarehouse)
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Importando artículos...");
                Factory.GetItemService().ImportItems();

                LogService.WriteInfo("Actualizando artículos...");
                Factory.GetItemService().UpdateItems();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void InitStockProcess(string pStrWarehouse)
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Importando stock...");
                Factory.GetStockService().ImportStocks(pStrWarehouse, mAuctionDate);

                LogService.WriteInfo("Actualizando stock...");
                Factory.GetStockService().UpdateStocks(pStrWarehouse);
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void InitAuctionProcess()
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Importando subastas...");
                Factory.GetAuctionService().ExportAuctions(GetLocation());

                LogService.WriteInfo("Actualizando subastas...");
                Factory.GetAuctionService().UpdateAuctions(GetLocation());
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void InitBatchesProcess(DateTime pDtmAuctionDate)
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Importando lotes...");
                Factory.GetBatchService().ExportBatches(pDtmAuctionDate);

                LogService.WriteInfo("Actualizando lotes...");
                Factory.GetBatchService().UpdateBatches(pDtmAuctionDate);
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void InitBatchLinesProcess()
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Importando lineas...");
                Factory.GetBatchLineService().ExportBatchLines(GetLocation());

                LogService.WriteInfo("Actualizando lineas...");
                Factory.GetBatchLineService().UpdateBatchLines(GetLocation());
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void InitItemProcess()
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Importando Articulos...");
                Factory.GetItemService().ImportItems();
                LogService.WriteInfo("Actualizando Articulos...");
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

        private void InitBuissnesPartnerConciliation()
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Procesando clientes temporales...");
                Factory.GetOperationsService().ConciliatePartners();
                LogService.WriteInfo("clientes temporales procesados.");
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

        private void InitStockConciliations()
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Procesando entradas temporales...");
                Factory.GetOperationsService().ConciliateStock(mAuctionDate);
                LogService.WriteInfo("Entradas temporales procesadas.");
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

        private void InitFoodDeliveriesProcess(string pStrWarehouse)
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Importando entregas de alimento...");
                Factory.GetFoodDeliveryService().ImportFoodDeliveries(pStrWarehouse);
                LogService.WriteSuccess("Entregas de alimento importadadas");

                LogService.WriteInfo("Actualizando entregas de alimento...");
                Factory.GetFoodDeliveryService().UpdateFoodDeliveries(pStrWarehouse);
                LogService.WriteSuccess("Actualizacion de entregas de alimento completadas");
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

        private void InitOperationsProcess(string pStrWarehouse)
        {
            try
            {
                Factory.Reconnection();
                LogService.WriteInfo("Procesando entradas temporales..");
                Factory.GetOperationsService().ConciliateStock(mAuctionDate);
                LogService.WriteSuccess("Entradas temporales procesadas");

                Factory.Reconnection();
                LogService.WriteInfo("Procesando subastas cerradas...");
                Factory.GetOperationsService().ProcessClosedAuctions();
                LogService.WriteSuccess("Subastas procesadas");

                Factory.Reconnection();
                LogService.WriteInfo("Procesando pagos de facturas generadas");
                Factory.GetOperationsService().ProcessPayments();
                LogService.WriteSuccess("Pagos generados");

                Factory.Reconnection();
                LogService.WriteInfo("Procesando subastas Re-abiertas...");
                Factory.GetOperationsService().ProcessReOpenedAuctions();
                LogService.WriteSuccess("Subastas procesadas");

                //InitStockProcess(pStrWarehouse);
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void InitConfigurationsProcess()
        {
            try
            {
                LogService.WriteInfo("Procesando configuraciones");
                Factory.GetConfigurationService().ProcessConfigurations();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
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

        public static string GetFoodWhsCode()
        {
            return QsConfig.GetValue<string>("FoodWarehouse");
        }

        public static string GetLocation()
        {
            return QsConfig.GetValue<string>("CostCenter");
        }

        private void Timer_Elapsed(object pObjSender, ElapsedEventArgs pObjEventArgs)
        {
            mTmrService.Enabled = false;

            try
            {
                if (mBolInitialized)
                {
                    StartAllProcess();
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
            }
            finally
            {
                mTmrService.Enabled = true;
            }
        }
    }
}
