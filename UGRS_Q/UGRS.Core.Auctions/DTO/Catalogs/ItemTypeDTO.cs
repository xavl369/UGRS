using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.Extension.Enum;

namespace UGRS.Core.Auctions.DTO.Catalogs
{
    public class ItemTypeDTO
    {
        #region Attributes

        private int mIntGenderId;
        private int mIntSellTypeId;
        private string mStrGender;
        private string mStrSellType;
        

        #endregion

        #region Properties

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool PerPrice { get; set; }
        public int Level { get; set; }
        public long CategoryId { get; set; }
        public string Category { get; set; }
        public long SubCategoryId { get; set; }
        public string SubCategory { get; set; }

        public int GenderId
        {
            get
            {
                return mIntGenderId;
            }
            set
            {
                mIntGenderId = value;
                mStrGender = ((ItemTypeGenderEnum)mIntGenderId).GetDescription();
            }
        }

        public string Gender
        {
            get
            {
                return mStrGender;
            }
        }

        public int SellTypeid
        {
            get
            {
                return mIntSellTypeId;
            }
            set
            {
                mIntSellTypeId = value;
                mStrSellType = ((SellTypeEnum)mIntSellTypeId).GetDescription();
            }
        }

        public string SellType
        {
            get
            {
                return mStrSellType;
            }
        }
        #endregion

        #region Constructor

        public ItemTypeDTO()
        {
            //Default constructor
        }

        public ItemTypeDTO(dynamic pUnkObject)
        {
            SetEntityFields(pUnkObject);
        }

        #endregion

        #region Methods

        private void SetEntityFields(dynamic pUnkObject)
        {
            Id = pUnkObject.Id;
            Code = pUnkObject.Code;
            Name = pUnkObject.Name;
            PerPrice = pUnkObject.PerPrice;
            Level = pUnkObject.Level;
            GenderId = (int)pUnkObject.Gender;
            CategoryId = pUnkObject.ParentId != null ? pUnkObject.ParentId : 0;
            Category = pUnkObject.ParentId != null && pUnkObject.Parent ? pUnkObject.Parent.Name : "";
            SubCategoryId = pUnkObject.ParentId != null && pUnkObject.Parent && pUnkObject.Parent.ParentId != null ? pUnkObject.Parent.ParentId : 0;
            SubCategory = pUnkObject.ParentId != null && pUnkObject.Parent && pUnkObject.Parent.ParentId != null && pUnkObject.Parent.Parent != null ? pUnkObject.Parent.Parent.Name : "";
        }

        #endregion
    }
}
