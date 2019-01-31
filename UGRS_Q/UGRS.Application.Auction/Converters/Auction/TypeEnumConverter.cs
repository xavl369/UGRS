using System;
using System.Globalization;
using System.Windows.Data;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension.Enum;

namespace UGRS.Application.Auctions.Converters.Auction
{
    public class TypeEnumConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return ((AuctionTypeEnum)pObjValue).GetDescription();
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return EnumExtension.GetValueFromDescription<AuctionTypeEnum>(pObjValue.ToString());
        }
    }
}
