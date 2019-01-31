using System;
using System.Globalization;
using System.Windows.Data;

namespace UGRS.Application.Auctions.Converters
{
    public class DocumentStatusConverter : IValueConverter
    {
        UGRS.Data.Auctions.Factories.AuctionsServicesFactory mObjAuctionFactory = new Data.Auctions.Factories.AuctionsServicesFactory();

        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            bool reopened = mObjAuctionFactory.GetAuctionService().GetReopenedActiveAuction() != null ? 
                mObjAuctionFactory.GetAuctionService().GetReopenedActiveAuction().ReOpened : false;

            return ((bool)pObjValue) && reopened ? "Re-Abierta" :
                ((bool)pObjValue) && !reopened ? "Abierta" :
                "Cerrada";

            //return ((bool)pObjValue) ? "Abierta" : "Cerrada";
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return pObjValue.ToString().Equals("Abierta");
        }
    }
}
