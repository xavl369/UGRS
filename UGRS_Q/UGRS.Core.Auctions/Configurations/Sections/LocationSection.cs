using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using UGRS.Core.Auctions.Configurations.Models;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension;

namespace UGRS.Core.Auctions.Configurations.Sections
{
    public class LocationSection : IConfigurationSectionHandler
    {
        public object Create(object pObjParent, object pObjConfigContext, XmlNode pObjSection)
        {
            List<LocationModel> lLstObjLocations = new List<LocationModel>();
            foreach (XmlNode lObjChildNode in pObjSection.ChildNodes)
            {
                lLstObjLocations.Add(new LocationModel()
                {
                    Location = lObjChildNode.Attributes["location"].Value.GetValue<LocationEnum>(),
                    Abbreviation = lObjChildNode.Attributes["abbreviation"].Value.GetValue<string>()
                });
            }
            return lLstObjLocations;
        }
    }
}
