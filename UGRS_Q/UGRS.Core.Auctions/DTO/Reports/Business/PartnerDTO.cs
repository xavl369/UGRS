using UGRS.Core.Auctions.Enums.Business;
using UGRS.Core.Extension.Enum;

namespace UGRS.Core.Auctions.DTO.Reports.Business
{
    public class PartnerDTO
    {
        #region Attributes

        private int mIntPartnerStatusId;
        private string mStrPartnerStatus;

        #endregion

        #region Properties

        public long PartnerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }


        public int PartnerStatusId
        {
            get
            {
                return mIntPartnerStatusId;
            }
            set
            {
                mIntPartnerStatusId = value;
                mStrPartnerStatus = ((PartnerStatusEnum)mIntPartnerStatusId).GetDescription();
            }
        }

        public string PartnerStatus
        {
            get
            {
                return mStrPartnerStatus;
            }
        }

        public bool Temporary { get; set; }

        #endregion

        #region Constructor

        public PartnerDTO()
        {
            //Default constructor
        }

        public PartnerDTO(dynamic pUnkObject)
        {
            SetEntityFields(pUnkObject);
        }

        #endregion

        #region Methods

        private void SetEntityFields(dynamic pUnkObject)
        {
            PartnerId = pUnkObject.Id;
            Code = pUnkObject.Code;
            Name = pUnkObject.Name;
            PartnerStatusId = (int)pUnkObject.PartnerStatus;
            Temporary = pUnkObject.Temporary;
        }

        #endregion
    }
}
