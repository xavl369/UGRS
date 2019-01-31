using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;

namespace UGRS.Core.SDK.DI.Auctions.UDO
{
    [ObjectAttribute(Name = "QS_TEST_A", Description = "Table for testing A")]
    public class Test : Models.UDO
    {
        [FieldAttribute(Name = "Name", Description = "Name", Type = BoFieldTypes.db_Alpha, SubType = BoFldSubTypes.st_None, Size = 20)]
        public string Name { get; set; }

        [FieldAttribute(Name = "Description", Description = "Description", Type = BoFieldTypes.db_Alpha, SubType = BoFldSubTypes.st_None, Size = 100)]
        public string Description { get; set; }
    }
}
