using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Session;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Core.Extension;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Context;

namespace UGRS.Data.Auctions.DAO.Base
{
    public class BaseDAO<T> : IBaseDAO<T> where T : BaseEntity
    {
        #region Attributes

        public AuctionsContext mObjContext = new AuctionsContext();
        public IObjectSet<T> mLstObjSet;

        #endregion

        #region Constructor

        public BaseDAO()
        {
            SetObjectContext();
        }

        #endregion

        #region Methods

        private void SetObjectContext()
        {
            mLstObjSet = ((IObjectContextAdapter)mObjContext).ObjectContext.CreateObjectSet<T>();
        }

        public void AddEntity(T pObjEntity)
        {
            pObjEntity.CreationDate = pObjEntity.CreationDate != DateTime.MinValue ? pObjEntity.CreationDate : DateTime.Now;
            pObjEntity.ModificationDate = pObjEntity.ModificationDate != DateTime.MinValue ? pObjEntity.ModificationDate : DateTime.Now;
            pObjEntity.Active = true;
            mLstObjSet.AddObject(pObjEntity);
            SaveChanges();

            SaveChange(ChangeTypeEnum.INSERT, pObjEntity);
            SaveChanges();
        }

        public void AddEntitiesList(IList<T> pLstObjEntity)
        {
            if (pLstObjEntity != null && pLstObjEntity.Count > 0)
            {
                foreach (var lObjEntity in pLstObjEntity)
                {
                    if (lObjEntity.Id == 0)
                    {
                      
                        lObjEntity.CreationDate = DateTime.Now;
                        lObjEntity.ModificationDate = DateTime.Now;
                        lObjEntity.Active = true;
                        mLstObjSet.AddObject(lObjEntity);
                        SaveChanges();

                        SaveChange(ChangeTypeEnum.INSERT, lObjEntity);
                        SaveChanges();
                    }
                }
            }
        }

        public void SaveChanges()
        {
            mObjContext.SaveChanges();
        }

        public void SaveChanges(T pObjEntity)
        {
            SaveChanges();
        }

        public void SaveOrUpdateEntity(T pObjEntity)
        {
            if (pObjEntity.Id == 0)
            {
                AddEntity(pObjEntity);
            }
            else
            {
                UpdateEntity(pObjEntity, pObjEntity.Id);
            }
        }

        public void SaveOrUpdateEntitiesList(IList<T> pLstObjEntity)
        {
            if (pLstObjEntity != null && pLstObjEntity.Count > 0)
            {
                foreach (var lObjEntity in pLstObjEntity)
                {
                    if (lObjEntity.Id == 0)
                    {

                        lObjEntity.CreationDate = lObjEntity.CreationDate != DateTime.MinValue ? lObjEntity.CreationDate : DateTime.Now;
                        lObjEntity.ModificationDate = lObjEntity.ModificationDate != DateTime.MinValue ? lObjEntity.ModificationDate : DateTime.Now;
                        lObjEntity.Active = true;
                        mLstObjSet.AddObject(lObjEntity);
                        SaveChanges();

                        SaveChange(ChangeTypeEnum.INSERT, lObjEntity);
                        SaveChanges();
                    }
                    else
                    {
                        T lObjCurrentEntity = GetEntity(lObjEntity.Id);

                        if (lObjCurrentEntity != null)
                        {
                            if (lObjCurrentEntity.Protected == false)
                            {
                             
                                lObjEntity.CreationDate = lObjCurrentEntity.CreationDate;
                                lObjEntity.ModificationDate = lObjEntity.ModificationDate != DateTime.MinValue ? lObjEntity.ModificationDate : DateTime.Now;
                                lObjEntity.Active = true;
                                mObjContext.Entry(lObjCurrentEntity).CurrentValues.SetValues(lObjEntity);
                                SaveChanges();

                                SaveChange(ChangeTypeEnum.UPDATE, lObjEntity);
                                SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Registro protegido contra escritura.");
                            }
                        }
                    }
                }
            }
        }

        public T GetEntity(long pLonId)
        {
            T lObjEntity = mObjContext.Set<T>().Where(x => x.Removed == false && x.Id == pLonId).FirstOrDefault();

            if (lObjEntity == null)
            {
                throw new Exception("No se encontró el registro.");
            }

            return lObjEntity;
        }

        public IQueryable<T> GetEntitiesList()
        {
            return mObjContext.Set<T>().Where(x => x.Removed == false);
        }

        public void RemoveEntity(long pLonId)
        {
            T lObjCurrentEntity = mObjContext.Set<T>().Where(x => x.Id == pLonId).FirstOrDefault();

            if (lObjCurrentEntity != null)
            {
                if (lObjCurrentEntity.Protected == false)
                {
                    lObjCurrentEntity.ModificationDate = DateTime.Now;
                    lObjCurrentEntity.Active = false;
                    lObjCurrentEntity.Removed = true;

                    SaveChange(ChangeTypeEnum.DELETE, lObjCurrentEntity);
                    SaveChanges();
                }
                else
                {
                    throw new Exception("Registro protegido contra escritura.");
                }
            }
        }

        public T SingleEntity(Func<T, bool> pBolPredicate)
        {
            return mLstObjSet.Single<T>(pBolPredicate);
        }

        public T UpdateEntity(T pObjNewEntity, long pLonId)
        {
            T lObjCurrentEntity = GetEntity(pLonId);

            if (lObjCurrentEntity != null)
            {
                if (lObjCurrentEntity.Protected == false)
                {
                   
                    pObjNewEntity.CreationDate = lObjCurrentEntity.CreationDate;
                    pObjNewEntity.ModificationDate = pObjNewEntity.ModificationDate != DateTime.MinValue ? pObjNewEntity.ModificationDate : DateTime.Now;
                    pObjNewEntity.Active = true;
                    mObjContext.Entry(lObjCurrentEntity).CurrentValues.SetValues(pObjNewEntity);
                    SaveChanges();

                    SaveChange(ChangeTypeEnum.UPDATE, pObjNewEntity);
                    SaveChanges();
                }
                else
                {
                    throw new Exception("Registro protegido contra escritura.");
                }
            }

            return lObjCurrentEntity;
        }
        
        protected virtual void Dispose(bool pBolDisposing)
        {
            if (pBolDisposing)
            {
                if (mObjContext != null)
                {
                    mObjContext.Dispose();
                    mObjContext = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Change

        private void SaveChange(ChangeTypeEnum pIntChangeType, T pObjEntity)
        {
            #if DEBUG
            
            var a = typeof(T).Name;
            var b = pObjEntity.CopyWithoutVirtualProperties().JsonSerialize();
            var c = GetCurrentUserId();

            #endif

            mObjContext.Change.Add(new Change()
            {
                Id = 0,
                ChangeType = pIntChangeType,
                ObjectType = typeof(T).Name,
                ObjectId = pObjEntity.Id,
                Object = pObjEntity.CopyWithoutVirtualProperties().JsonSerialize(),
                Date = DateTime.Now,
                Protected = true,
                Removed = false,
                UserId = GetCurrentUserId(),
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now
            });
        }

        private void SaveChange(ChangeTypeEnum pIntChangeType, IList<T> pLstObjEntity)
        {
            if (pLstObjEntity != null && pLstObjEntity.Count > 0)
            {
                foreach (var lObjEntity in pLstObjEntity)
                {
                    mObjContext.Change.Add(new Change()
                    {
                        Id = 0,
                        ChangeType = pIntChangeType,
                        ObjectType = lObjEntity.GetType().Name,
                        ObjectId = lObjEntity.Id,
                        Object = lObjEntity.CopyWithoutVirtualProperties().JsonSerialize(),
                        Date = DateTime.Now,
                        Protected = true,
                        Removed = false,
                        UserId = GetCurrentUserId(),
                        CreationDate = DateTime.Now,
                        ModificationDate = DateTime.Now
                    });
                }
            }
        }

        private T GetOptimizedObjectForSerialize(T pObjEntity)
        {
            //Create a clone to not affect the saving
            T lObjClone = (T)Activator.CreateInstance(typeof(T));

            foreach (PropertyInfo lObjCloneProperty in lObjClone.GetType().GetProperties().Where(x=> !x.GetMethod.IsVirtual))
            {
                try
                {
                    lObjCloneProperty.SetValue
                    (
                        lObjClone, 
                        pObjEntity.GetType().GetProperties().Where(x => x.Name == lObjCloneProperty.Name).FirstOrDefault().GetValue(pObjEntity)
                    );
                }
                catch
                {
                    //Ignore
                }
            }

            return lObjClone;
        }

        private long GetCurrentUserId()
        {
            return ((SessionDTO)StaticSessionUtility.GetCurrentSession()).Id;
        }

        #endregion
    }
}
