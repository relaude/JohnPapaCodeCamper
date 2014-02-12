using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;
using DataModel;
using System.Data.Entity;

namespace Repository.UserAccount
{
    public class webpages_UsersInRolesRepository : Iwebpages_UsersInRolesRepository
    {
        private readonly IUsersAccountDbContext _context;

        public webpages_UsersInRolesRepository(IUnitOfWork uow)
        {
            _context = uow.Context as IUsersAccountDbContext;
        }

        public IQueryable<UserRoles> UserRoles()
        {
            var list = (from userinrole in _context.webpages_UsersInRoles
                        select new UserRoles
                        {
                            RoleId = userinrole.RoleId,
                            RoleName = userinrole.webpages_Roles.RoleName,
                            UserId = userinrole.UserId,
                            UserName = userinrole.UserProfile.UserName
                        }).AsQueryable();

            return list;
        }

        public List<UserRoles> GetUserRolesByProfileId(int id)
        {
            var list = (from userinrole in _context.webpages_UsersInRoles
                        where userinrole.UserProfile.UserId == id
                        select new UserRoles
                        {
                            RoleId = userinrole.RoleId,
                            RoleName = userinrole.webpages_Roles.RoleName,
                            UserId = userinrole.UserId,
                            UserName = userinrole.UserProfile.UserName
                        }).ToList();

            return list;
        }

        public IQueryable<webpages_UsersInRoles> All
        {
            get { return _context.webpages_UsersInRoles; }
        }

        public IQueryable<webpages_UsersInRoles> AllIncluding(params System.Linq.Expressions.Expression<Func<webpages_UsersInRoles, object>>[] includeProperties)
        {
            IQueryable<webpages_UsersInRoles> query = _context.webpages_UsersInRoles;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public webpages_UsersInRoles Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(webpages_UsersInRoles entity)
        {
            if (entity.UserId == default(int))
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
            var entity = _context.webpages_UsersInRoles.Find(id);
            _context.webpages_UsersInRoles.Remove(entity);
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
