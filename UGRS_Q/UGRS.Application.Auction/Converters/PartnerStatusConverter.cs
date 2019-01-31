using System;
using System.Globalization;
using System.Windows.Data;
using UGRS.Core.Auctions.Enums.Business;
using UGRS.Core.Extension.Enum;

namespace UGRS.Application.Auctions.Converters
{
    public class PartnerStatusConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return ((PartnerStatusEnum)pObjValue).GetDescription();
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return EnumExtension.GetValueFromDescription<PartnerStatusEnum>(pObjValue.ToString());
        }
    }
}
