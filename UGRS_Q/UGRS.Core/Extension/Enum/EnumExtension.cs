using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace UGRS.Core.Extension.Enum
{
    public class EnumItem
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public static class EnumExtension
    {
        public static string GetDescription(this System.Enum pObjElement)
        {
            Type lObjType = pObjElement.GetType();
            MemberInfo[] lArrObjMemberInfo = lObjType.GetMember(pObjElement.ToString());
            if (lArrObjMemberInfo != null && lArrObjMemberInfo.Length > 0)
            {
                object[] lArrObjAtributes = lArrObjMemberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (lArrObjAtributes != null && lArrObjAtributes.Length > 0)
                {
                    return ((DescriptionAttribute)lArrObjAtributes[0]).Description;
                }
            }

            return pObjElement.ToString();
        }

        public static List<EnumItem> GetEnumItemList<T>() where T : struct, IConvertible
        {
            List<EnumItem> lLstObjResult = new List<EnumItem>();

            int lIntValueItem = 0;
            string lStrTextItem = null;

            if (typeof(T).IsEnum)
            {
                foreach (System.Enum lObjItem in typeof(T).GetEnumValues())
                {
                    lIntValueItem = Convert.ToInt32(lObjItem);
                    lStrTextItem = lObjItem.GetDescription();
                    lLstObjResult.Add(new EnumItem() { Value = lIntValueItem, Text = lStrTextItem });
                }
                return lLstObjResult;
            }
            else
            {
                throw new Exception("La clase seleccionada no es del tipo enumerador.");
            }
        }

        public static T GetValueFromDescription<T>(string pStrDescription)
        {
            var lObjType = typeof(T);
            if (!lObjType.IsEnum) throw new InvalidOperationException();

            foreach (var lObjField in lObjType.GetFields())
            {
                var lObjAttribute = Attribute.GetCustomAttribute(lObjField, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (lObjAttribute != null)
                {
                    if (lObjAttribute.Description == pStrDescription)
                        return (T)lObjField.GetValue(null);
                }
                else
                {
                    if (lObjField.Name == pStrDescription)
                        return (T)lObjField.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "Description");
            // or return default(T);
        }
    }
}
