using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Caviar.Infrastructure.Persistence
{
    public class SysDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>, IDbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public SysDbContext(DbContextOptions options) : base(options)
        {

        }

        public SysDbContext() : base()
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var types = CommonHelper.GetEntityList();
            foreach (var item in types)
            {
                var method = modelBuilder.GetType().GetMethods().Where(x => x.Name == "Entity").FirstOrDefault();
                if (method != null)
                {
                    method = method.MakeGenericMethod(new Type[] { item });
                    method.Invoke(modelBuilder, null);
                }
            }
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().ToTable("SysUser");//不能创建SysUsers
            modelBuilder.Entity<ApplicationRole>().ToTable("SysRoles");
            modelBuilder.Entity<IdentityUserClaim<TKey>>().ToTable("SysUserClaims");
            modelBuilder.Entity<IdentityUserLogin<TKey>>().ToTable("SysUserLogins");
            modelBuilder.Entity<IdentityRoleClaim<TKey>>().ToTable("SysIdentityRoleClaims");
            modelBuilder.Entity<IdentityUserRole<TKey>>().ToTable("SysUserRoles");
            modelBuilder.Entity<IdentityUserToken<TKey>>().ToTable("SysUserTokens");
        }
    }
}
