using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.FoodProduction.Tables;

namespace UGRS.Core.SDK.DI.FoodProduction.Services
{
    public class SetupService
    {
        private TableDAO<Ticket> mObjTicketDAO;
        private TableDAO<TicketDetail> mObjTicketDetailDAO;

        public SetupService()
        {
            mObjTicketDAO = new TableDAO<Ticket>();
            mObjTicketDetailDAO = new TableDAO<TicketDetail>();
        }

        public void InitializeTables()
        {
            mObjTicketDAO.Initialize();
            mObjTicketDetailDAO.Initialize();
        }


    }
}
