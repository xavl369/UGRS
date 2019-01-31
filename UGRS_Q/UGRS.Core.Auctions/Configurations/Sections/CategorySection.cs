using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using UGRS.Core.Auctions.Configurations.Models;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension;

namespace UGRS.Core.Auctions.Configurations.Sections
{
    public class CategorySection : IConfigurationSectionHandler
    {
        public object Create(object pObjParent, object pObjConfigContext, XmlNode pObjSection)
        {
            List<CategoryModel> lLstObjCategories = new List<CategoryModel>();
            foreach (XmlNode lObjChildNode in pObjSection.ChildNodes)
            {
                lLstObjCategories.Add(new CategoryModel()
                {
                    Category = lObjChildNode.Attributes["category"].Value.GetValue<AuctionCategoryEnum>(),
                    Abbreviation = lObjChildNode.Attributes["abbreviation"].Value.GetValue<string>()
                });
            }
            return lLstObjCategories;
        }
    }
}
