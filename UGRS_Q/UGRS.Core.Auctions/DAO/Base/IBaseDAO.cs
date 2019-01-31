using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.DAO.Base
{
    public interface IBaseDAO<T> : IDisposable where T : BaseEntity
    {
        void AddEntity(T pObjEntity);

        void AddEntitiesList(IList<T> pLstObjEntity);

        void SaveChanges();

        void SaveOrUpdateEntity(T pObjEntity);

        void SaveOrUpdateEntitiesList(IList<T> pLstObjEntity);

        T GetEntity(long pLonId);

        IQueryable<T> GetEntitiesList();

        void RemoveEntity(long pLonId);

        T SingleEntity(Func<T, bool> pBolPredicate);

        T UpdateEntity(T pObjEntity, long pLonId);
    }
}
