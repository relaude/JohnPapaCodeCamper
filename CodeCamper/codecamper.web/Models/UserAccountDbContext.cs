using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace codecamper.web.Models
{
    public class UserAccountDbContext : DbContext
    {
        public UserAccountDbContext()
            : base(nameOrConnectionString: "Authentication") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Use singular table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            Database.SetInitializer<UserAccountDbContext>(null);
        }

        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }
    }
}