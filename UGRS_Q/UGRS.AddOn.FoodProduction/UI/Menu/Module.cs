using SAPbouiCOM;
using System.Collections.Generic;

namespace UGRS.AddOn.FoodProduction.UI.Menu
{
    public class Module
    {
        public string UniqueID { get; set; }
        public string String { get; set; }
        public BoMenuType Type { get; set; }
        public bool Enable { get; set; }
        public string Image { get; set; }
        public int Position { get; set; }
        public IList<Section> Sections { get; set; }
    }
}
