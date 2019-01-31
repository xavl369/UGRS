using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;

namespace UGRS.AddOn.FoodProduction.UI.Menu
{
    public class Section
    {
        public string UniqueID { get; set; }
        public string String { get; set; }
        public BoMenuType Type { get; set; }
        public int Position { get; set; }
    }
}
