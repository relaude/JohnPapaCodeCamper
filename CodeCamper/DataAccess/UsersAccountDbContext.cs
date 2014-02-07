using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DataModel;
using Breeze.WebApi.EF;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;

namespace DataAccess
{
    public interface IUsersAccountDbContext : IContext
    {
        IDbSet<UserProfile> UserProfile { get; }
        IDbSet<webpages_Roles> webpages_Roles { get; }
        IDbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; }
    }

    public class UsersAccountDbContext : BaseContext<UsersAccountDbContext>, IUsersAccountDbContext
    {
        private readonly EFContextProvider<UsersAccountDbContext> _context = new EFContextProvider<UsersAccountDbContext>();

        //Entity
        public IDbSet<UserProfile> UserProfile { get; set; }
        public IDbSet<webpages_Roles> webpages_Roles { get; set; }
        public IDbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }

        public int SaveModifications()
        {
            return this.SaveModifications();
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = System.Data.EntityState.Modified;
        }

        public void SetAdd(object entity)
        {
            Entry(entity).State = System.Data.EntityState.Added;
        }

        public string Metadata()
        {
            return _context.Metadata();
        }

        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _context.SaveChanges(saveBundle);
        }
    }
}
