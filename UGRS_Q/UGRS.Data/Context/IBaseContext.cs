using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading;
using System.Threading.Tasks;
using UGRS.Core.Entities.Security;
using UGRS.Core.Entities.System;
using UGRS.Core.Entities.Users;

namespace UGRS.Data.Context
{
    public interface IBaseContext : IObjectContextAdapter
    {
        #region Attributes

        //USERS
        DbSet<User> User { get; set; }
        DbSet<UserType> UserType { get; set; }

        //SYSTEM
        DbSet<Core.Entities.System.Module> Module { get; set; }
        DbSet<Section> Section { get; set; }
        DbSet<Change> Change { get; set; }

        //SECURITY
        DbSet<Permission> Permission { get; set; }

        #endregion

        #region Methods

        DbChangeTracker ChangeTracker { get; }
        DbContextConfiguration Configuration { get; }
        Database Database { get; }
        void Dispose();
        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Type GetType();
        IEnumerable<DbEntityValidationResult> GetValidationErrors();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet Set(Type entityType);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        #endregion
    }
}
