using System;
using System.Globalization;
using System.Windows.Data;

namespace UGRS.Application.Auctions.Converters
{
    public class LongDateConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return ((DateTime)pObjValue).ToString("D", pObjCulture);
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return DateTime.ParseExact(pObjValue.ToString(), "D", pObjCulture);
        }
    }
}
