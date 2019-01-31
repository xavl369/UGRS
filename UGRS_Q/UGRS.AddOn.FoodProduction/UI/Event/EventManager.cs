using System.Collections.Generic;
using System.Linq;
using UGRS.AddOn.FoodProduction.UI.Menu;

namespace UGRS.AddOn.FoodProduction.UI .Event
{
    public class EventManager
    {
        public SAPbouiCOM.EventFilters GetItemEventFiltersByMenu(IList<Module> pLstObjMenu)
        {
            SAPbouiCOM.EventFilters lObjResult = new SAPbouiCOM.EventFilters();

            if (pLstObjMenu != null && pLstObjMenu.Count > 0)
            {
                lObjResult = GetItemEventFilters(pLstObjMenu.SelectMany(m => m.Sections).Select(s => string.Format("UGRS.PlantaAlimentos.Forms.{0}", s.UniqueID)).ToList());
            }

            return lObjResult;
        }

        public SAPbouiCOM.EventFilters GetItemEventFilters(IList<string> pLstStrFormList)
        {
            SAPbouiCOM.EventFilters lObjFilters;
            SAPbouiCOM.EventFilter lObjFilter;

            lObjFilters = new SAPbouiCOM.EventFilters();

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD);
            foreach (string lStrForm in pLstStrFormList) { lObjFilter.AddEx(lStrForm); }

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE);
            foreach (string lStrForm in pLstStrFormList) { lObjFilter.AddEx(lStrForm); }

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_CLICK);
            foreach (string lStrForm in pLstStrFormList) { lObjFilter.AddEx(lStrForm); }

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED);
            foreach (string lStrForm in pLstStrFormList) { lObjFilter.AddEx(lStrForm); }

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_MENU_CLICK);
            foreach (string lStrForm in pLstStrFormList) { lObjFilter.AddEx(lStrForm); }

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_KEY_DOWN);
            foreach (string lStrForm in pLstStrFormList) { lObjFilter.AddEx(lStrForm); }

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_COMBO_SELECT);
            foreach (string lStrForm in pLstStrFormList) { lObjFilter.AddEx(lStrForm); }

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST);
            foreach (string lStrForm in pLstStrFormList) { lObjFilter.AddEx(lStrForm); }

            return lObjFilters;
        }

        private SAPbouiCOM.EventFilters GetItemEventFilters(string pStrTypeEx)
        {
            SAPbouiCOM.EventFilters lObjFilters;
            SAPbouiCOM.EventFilter lObjFilter;

            lObjFilters = new SAPbouiCOM.EventFilters();

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD);
            lObjFilter.AddEx(pStrTypeEx);

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE);
            lObjFilter.AddEx(pStrTypeEx);

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_CLICK);
            lObjFilter.AddEx(pStrTypeEx);

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED);
            lObjFilter.AddEx(pStrTypeEx);

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_MENU_CLICK);
            lObjFilter.AddEx(pStrTypeEx);

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_KEY_DOWN);
            lObjFilter.AddEx(pStrTypeEx);

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_COMBO_SELECT);
            lObjFilter.AddEx(pStrTypeEx);

            lObjFilter = lObjFilters.Add(SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST);
            lObjFilter.AddEx(pStrTypeEx);

            return lObjFilters;
        }
    }
}
