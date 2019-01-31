using System;
using System.Globalization;
using System.Windows.Data;

namespace UGRS.Application.Auctions.Converters
{
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return (bool)pObjValue ? "Si" : "No";
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return pObjValue.Equals("Si");
        }
    }
}
