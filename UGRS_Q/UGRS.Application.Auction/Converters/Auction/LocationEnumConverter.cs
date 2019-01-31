using System;
using System.Globalization;
using System.Windows.Data;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension.Enum;

namespace UGRS.Application.Auctions.Converters.Auction
{
    public class LocationEnumConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return ((LocationEnum)pObjValue).GetDescription();
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return EnumExtension.GetValueFromDescription<LocationEnum>(pObjValue.ToString());
        }
    }
}
