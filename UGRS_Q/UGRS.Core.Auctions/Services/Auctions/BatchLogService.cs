using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Auction;
using UGRS.Core.Auctions.DTO.System;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Auctions.Enums.System;

namespace UGRS.Core.Auctions.Services.Auctions
{
    public class BatchLogService
    {
        private IBaseDAO<Batch> mObjBatchDAO;
        private IBaseDAO<Change> mObjChangeDAO;
        private IBaseDAO<User> mObjUserDAO;

        public BatchLogService(IBaseDAO<Batch> pObjBatchDAO, IBaseDAO<Change> pObjChangeDAO, IBaseDAO<User> pObjUserDAO)
        {
            mObjBatchDAO = pObjBatchDAO;
            mObjChangeDAO = pObjChangeDAO;
            mObjUserDAO = pObjUserDAO;
        }

        public List<BatchLogDTO> GetBatchLogList(long pLonBatchId)
        {
            return InternalGetBatchLogList
            (
                mObjBatchDAO.GetEntity(pLonBatchId), 
                GetBatchChanges(pLonBatchId)
            );
        }

        private List<BatchLogDTO> InternalGetBatchLogList(Batch pObjBatch, List<Change> pLstObjChanges)
        {
            return ConvertToBatchLog
            (
                pObjBatch, 
                GetLogList
                (
                    pLstObjChanges, 
                    GetUsers
                    (
                        GetUsersId(pLstObjChanges)
                    )
                )
            );
        }

        private List<BatchLogDTO> ConvertToBatchLog(Batch pObjBatch, List<LogDTO> pLstObjLog)
        {
            var lObjAuction = pObjBatch.Auction;

            var lObjLog = pLstObjLog
                .Where(y=> y.ChangeType == ChangeTypeEnum.INSERT)
                .Select(y=> new {UserId = y.UserId, User = y.User, Date = y.Date})
                .FirstOrDefault();

            return pLstObjLog.Select(x => new BatchLogDTO()
                {
                    Id = x.Id,
                    AuctionId = lObjAuction.Id,
                    Auction = lObjAuction.Folio,
                    Number = pLstObjLog.IndexOf(x) + 1,
                    BatchId = pObjBatch.Id,
                    BatchObject = x.Object,
                    BatchNumber = pObjBatch.Number,
                    ModificationUserId = x.UserId,
                    ModificationUser = x.User,
                    ModificationDate = x.Date,
                    CreationUserId = lObjLog.UserId,
                    CreationUser = lObjLog.User,
                    CreationDate = lObjLog.Date
                })
                .ToList();
        }

        private List<LogDTO> GetLogList(List<Change> pLstObjChanges, List<User> pLstObjUsers)
        {
            return (from C in pLstObjChanges
                    join U in pLstObjUsers
                    on C.UserId equals U.Id
                    select new LogDTO
                    {
                        Id = C.Id,
                        ChangeType = C.ChangeType,
                        ObjectId = C.ObjectId,
                        Object = C.Object,
                        UserId = C.UserId,
                        User = U.UserName,
                        Date = C.CreationDate
                    })
                    .ToList();
        }

        private List<Change> GetBatchChanges(long pLonBatchId)
        {
            string lStrObjectType = typeof(Batch).Name;
            return mObjChangeDAO.GetEntitiesList().Where(x => x.ObjectType.StartsWith(lStrObjectType) && x.ObjectType.EndsWith(lStrObjectType) && x.ObjectId == pLonBatchId).ToList();
        }

        private List<long> GetUsersId(List<Change> pLstObjChanges)
        {
            return pLstObjChanges.Select(x=> x.UserId).ToList();
        }

        private List<User> GetUsers(List<long> pLstObjUsersId)
        {
            return mObjUserDAO.GetEntitiesList().Where(x => pLstObjUsersId.Contains(x.Id)).ToList();
        }
    }
}

