using System;
using System.Globalization;
using System.Windows.Data;

namespace UGRS.Application.Auctions.Converters
{
    public class ShortDateConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return ((DateTime)pObjValue).ToString("d", pObjCulture);
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return DateTime.ParseExact(pObjValue.ToString(), "d", pObjCulture);
        }
    }
}
