using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataModel;

namespace Repository
{
    public interface IUserProfileRepository : Interface<UserProfile> { }
    public interface Iwebpages_RolesRepository : Interface<webpages_Roles> { }
    public interface Iwebpages_UsersInRolesRepository : Interface<webpages_UsersInRoles> { }
    
    public interface Interface<T> : IDisposable
    {
        IQueryable<T> All { get; }
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        T Find(int id);
        void InsertOrUpdate(T entity);
        void Delete(int id);
        void Save();
    }
}
