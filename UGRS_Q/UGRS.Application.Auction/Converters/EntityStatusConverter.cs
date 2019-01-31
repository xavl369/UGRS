using System;
using System.Globalization;
using System.Windows.Data;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Core.Extension.Enum;

namespace UGRS.Application.Auctions.Converters
{
    public class EntityStatusConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return ((EntityStatusEnum)pObjValue).GetDescription();
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return EnumExtension.GetValueFromDescription<EntityStatusEnum>(pObjValue.ToString());
        }
    }
}
