using System;
using System.Globalization;
using System.Windows.Data;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.Extension.Enum;

namespace UGRS.Application.Auctions.Converters
{
    public class ItemStatusConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return ((ItemStatusEnum)pObjValue).GetDescription();
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return EnumExtension.GetValueFromDescription<ItemStatusEnum>(pObjValue.ToString());
        }
    }
}
