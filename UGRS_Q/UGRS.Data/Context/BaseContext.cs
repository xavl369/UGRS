using System.Data.Entity;
using UGRS.Core.Entities.Security;
using UGRS.Core.Entities.System;
using UGRS.Core.Entities.Users;

namespace UGRS.Data.Context
{
    public class BaseContext : DbContext, IBaseContext
    {
        //USERS
        public DbSet<User> User { get; set; }
        public DbSet<UserType> UserType { get; set; }

        //SYSTEM
        public DbSet<Core.Entities.System.Module> Module { get; set; }
        public DbSet<Section> Section { get; set; }
        public DbSet<Change> Change { get; set; }

        //SECURITY
        public DbSet<Permission> Permission { get; set; }

        public BaseContext(string pStrConnection):
            base(pStrConnection) 
        { }
    }
}
