using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using UGRS.Core.DAO.Base;
using UGRS.Core.DTO.Session;
using UGRS.Core.Entities.Base;
using UGRS.Core.Entities.System;
using UGRS.Core.Enums.System;
using UGRS.Core.Utility;
using UGRS.Data.Context;

namespace UGRS.Data.DAO.Base
{
    public class BaseDAO<T> : IBaseDAO<T> where T : BaseEntity
    {
        public IBaseContext mObjContext;
        public IObjectSet<T> mLstObjSet;

        public BaseDAO(IBaseContext pObjContext)
        {
            mObjContext = pObjContext;
            SetObjectContext();
        }

        private void SetObjectContext()
        {
            mLstObjSet = ((IObjectContextAdapter)mObjContext).ObjectContext.CreateObjectSet<T>();
        }

        public void AddEntity(T pObjEntity)
        {
            pObjEntity.Status = GetStatus(pObjEntity);
            pObjEntity.ChangeStatus = ChangeStatusEnum.AUTHORIZED;
            pObjEntity.CreationDate = DateTime.Now;
            pObjEntity.ModificationDate = DateTime.Now;

            mLstObjSet.AddObject(pObjEntity);

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
                        lObjEntity.Status = GetStatus(lObjEntity);
                        lObjEntity.ChangeStatus = ChangeStatusEnum.AUTHORIZED;
                        lObjEntity.CreationDate = DateTime.Now;
                        lObjEntity.ModificationDate = DateTime.Now;

                        mLstObjSet.AddObject(lObjEntity);

                        SaveChange(ChangeTypeEnum.INSERT, lObjEntity);
                    }
                }
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            mObjContext.SaveChanges();
        }

        public void saveChanges(T pObjEntity)
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
                        lObjEntity.Status = GetStatus(lObjEntity);
                        lObjEntity.ChangeStatus = ChangeStatusEnum.AUTHORIZED;
                        lObjEntity.CreationDate = DateTime.Now;
                        lObjEntity.ModificationDate = DateTime.Now;

                        mLstObjSet.AddObject(lObjEntity);

                        SaveChange(ChangeTypeEnum.INSERT, lObjEntity);
                    }
                    else
                    {
                        T lObjCurrentEntity = GetEntity(lObjEntity.Id);

                        if (lObjCurrentEntity != null)
                        {
                            if (lObjCurrentEntity.Protected == false)
                            {
                                lObjEntity.Status = GetStatus(lObjEntity);
                                lObjEntity.ChangeStatus = ChangeStatusEnum.AUTHORIZED;
                                lObjEntity.CreationDate = lObjCurrentEntity.CreationDate;
                                lObjEntity.ModificationDate = DateTime.Now;

                                mObjContext.Entry(lObjCurrentEntity).CurrentValues.SetValues(lObjEntity);

                                SaveChange(ChangeTypeEnum.UPDATE, lObjEntity);
                            }
                            else
                            {
                                throw new Exception("Registro protegido contra escritura.");
                            }
                        }
                    }
                }

                SaveChanges();
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
            T lObjCurrentEntity = GetEntity(pLonId);

            if (lObjCurrentEntity != null)
            {
                if (lObjCurrentEntity.Protected == false)
                {
                    lObjCurrentEntity.ModificationDate = DateTime.Now;
                    lObjCurrentEntity.Status = EntityStatusEnum.INACTIVE;
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
                    pObjNewEntity.Status = GetStatus(pObjNewEntity);
                    pObjNewEntity.ChangeStatus = ChangeStatusEnum.AUTHORIZED;
                    pObjNewEntity.CreationDate = lObjCurrentEntity.CreationDate;
                    pObjNewEntity.ModificationDate = DateTime.Now;

                    mObjContext.Entry(lObjCurrentEntity).CurrentValues.SetValues(pObjNewEntity);

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

        private EntityStatusEnum GetStatus(T pObjEntity)
        {
            return pObjEntity.Status = (int)pObjEntity.Status == 0 ? EntityStatusEnum.ACTIVE : pObjEntity.Status;
        }

        #region Change

        private void SaveChange(ChangeTypeEnum pIntChangeType, T pObjEntity)
        {
            mObjContext.Change.Add(new Change()
            {
                Id = 0,
                ChangeType = pIntChangeType,
                ObjectType = typeof(T).Name,
                Object = GetSerializedObject(pObjEntity),
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
                        Object = GetSerializedObject(lObjEntity),
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

        private long GetCurrentUserId()
        {
            return ((SessionDTO)StaticSessionUtility.GetCurrentSession()).Id;
        }

        private string GetSerializedObject(T pObjEntity)
        {
            return JsonConvert.SerializeObject(pObjEntity, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
