using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Services;

namespace UGRS.Core.Auctions.DAO
{
    public class QueryManager
    {
        public List<Stock> GetStockList()
        {
            SqlCommand lObjCommand;
            SqlConnection lobjConnection = null;
            SqlDataReader lObjSqlReader;
            List<Stock> lLstStock = new List<Stock>();

            try
            {
                using (lobjConnection = new SqlConnection(GetConnection()))
                {
                    using(lObjCommand = new SqlCommand())
                    {
                        lObjCommand.CommandText = "SELECT * FROM "+GetDbName()+".[INVENTORY].[Stocks]";
                        lObjCommand.CommandType = CommandType.Text;
                        lObjCommand.Connection = lobjConnection;
                        lObjCommand.CommandTimeout = 0;

                        lobjConnection.Open();

                        lObjSqlReader = lObjCommand.ExecuteReader();

                        if(lObjSqlReader.HasRows)
                        {
                            DataTable lobjDt = new DataTable();
                            lobjDt.Load(lObjSqlReader);

                            lLstStock = (from DataRow lObjRow in lobjDt.Rows

                                         select new Stock
                                         {
                                             Id = Convert.ToInt64(lObjRow["Id"]),
                                             BatchNumber = lObjRow["BatchNumber"].ToString(),
                                             Quantity = Convert.ToInt32(lObjRow["Quantity"]),
                                             CustomerId = Convert.ToInt64(lObjRow["CustomerId"]),
                                             ItemId = Convert.ToInt64(lObjRow["ItemId"]),
                                             Protected = (bool)lObjRow["Protected"],
                                             Removed = (bool)lObjRow["Removed"],
                                             Active = (bool)lObjRow["Active"],
                                             CreationDate = Convert.ToDateTime(lObjRow["CreationDate"]),
                                             ModificationDate = Convert.ToDateTime(lObjRow["ModificationDate"]),
                                             CurrentWarehouse = lObjRow["CurrentWarehouse"].ToString(),
                                             InitialWarehouse = lObjRow["InitialWarehouse"].ToString(),
                                             ChargeFood = (bool)lObjRow["ChargeFood"],
                                             ExpirationDate = Convert.ToDateTime(lObjRow["ExpirationDate"]),
                                             Payment = (bool)lObjRow["Payment"],
                                             EntryFolio = lObjRow["EntryFolio"].ToString()

                                         }).ToList();
                            
                        }

                    }

                }
            }
            catch(Exception lO)
            {
                LogService.WriteError(lO.Message);
            }
            finally
            {
                try
                {
                    lobjConnection.Close();
                    lobjConnection.Dispose();
                }
                catch
                {

                }
            }
            return lLstStock;
        }

        private string GetConnection()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["UGRS_Subastas_Com"].ConnectionString;
        }

        private string GetDbName()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder lObjBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(GetConnection());
            return lObjBuilder["Initial Catalog"] as string;
        }
    }
}
