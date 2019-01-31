using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension.Enum;

namespace UGRS.Application.Auctions.Converters
{
    public class UnsoldMotiveConverter : IValueConverter
    {
        public object Convert(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
           // return (condicion ? x : y);
            string lStrDescription = ((UnsoldMotiveEnum)pObjValue).GetDescription();
            return (lStrDescription == "0" ? "" : lStrDescription);
        }

        public object ConvertBack(object pObjValue, Type pObjTargetType, object pObjParameter, CultureInfo pObjCulture)
        {
            return EnumExtension.GetValueFromDescription<UnsoldMotiveEnum>(pObjValue.ToString());
        }
    }
}
