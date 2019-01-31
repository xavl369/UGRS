using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.Attributes;

namespace UGRS.Core.SDK.DI.Models
{
    public interface ISAPObject
    {
        SAPObjectAttribute GetAttributes();

        IList<Field> GetFields();
    }
}
