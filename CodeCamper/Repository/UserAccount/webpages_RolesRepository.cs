using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;
using DataModel;
using System.Data.Entity;

namespace Repository.UserAccount
{
    public class webpages_RolesRepository : Iwebpages_RolesRepository
    {
        private readonly IUsersAccountDbContext _context;

        public webpages_RolesRepository(IUnitOfWork uow)
        {
            _context = uow.Context as IUsersAccountDbContext;
        }

        public IQueryable<webpages_Roles> All
        {
            get { return _context.webpages_Roles; }
        }

        public IQueryable<webpages_Roles> AllIncluding(params System.Linq.Expressions.Expression<Func<webpages_Roles, object>>[] includeProperties)
        {
            IQueryable<webpages_Roles> query = _context.webpages_Roles;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public webpages_Roles Find(int id)
        {
            return _context.webpages_Roles.Find(id);
        }

        public void InsertOrUpdate(webpages_Roles entity)
        {
            if (entity.RoleId == default(int))
            {
                _context.SetAdd(entity);
            }
            else
            {
                _context.SetModified(entity);
            }
        }

        public void Delete(int id)
        {
            var entity = _context.webpages_Roles.Find(id);
            _context.webpages_Roles.Remove(entity);
        }

        public void Save()
        {
            _context.SaveModifications();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
