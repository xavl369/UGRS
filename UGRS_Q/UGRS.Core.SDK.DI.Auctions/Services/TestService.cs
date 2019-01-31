using System.Collections.Generic;
using UGRS.Core.SDK.DI.Auctions.UDO;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    public class TestService
    {
        #region Attributes

        private IUDODAO<Test> mObjTestDAO;

        #endregion

        #region Properties

        private IUDODAO<Test> TestDAO
        {
            get { return mObjTestDAO; }
            set { mObjTestDAO = value; }
        }

        #endregion

        #region Construct

        public TestService()
        {
            TestDAO = new UDODAO<Test>();
        }

        #endregion

        #region Methods

        public int Initialize()
        {
            return TestDAO.Initialize();
        }

        public IList<Test> GetList()
        {
            return null;
        }

        #endregion
    }
}
