using System;
using System.Globalization;
using System.Windows.Data;

namespace UGRS.Application.Auctions.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return string.Format("{0}%", (((float)pObjValue) * 100).ToString(pObjCulture));
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return float.Parse(pObjValue.ToString().Replace("%", ""), pObjCulture);
        }
    }
}
