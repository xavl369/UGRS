using System;
using System.Globalization;
using System.Windows.Data;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension.Enum;

namespace UGRS.Application.Auctions.Converters.Auction
{
    public class CategoryEnumConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return ((AuctionCategoryEnum)pObjValue).GetDescription();
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return EnumExtension.GetValueFromDescription<AuctionCategoryEnum>(pObjValue.ToString());
        }
    }
}
