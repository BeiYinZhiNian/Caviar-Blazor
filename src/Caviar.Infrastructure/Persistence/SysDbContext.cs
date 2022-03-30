using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.Persistence
{
    public class SysDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>, IDbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly CaviarConfig _caviarConfig;
        public SysDbContext(DbContextOptions options,CaviarConfig caviarConfig) : base(options)
        {
            _caviarConfig = caviarConfig;
        }

        public SysDbContext() : base()
        {

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_caviarConfig.DemonstrationMode)
            {
                throw new ResultException(new ResultMsg()
                {
                    Title = "当前处于演示模式，该功能无法使用，更多精彩功能，请下载源代码后体验",
                    Url = "https://gitee.com/Cherryblossoms/caviar",
                    Status = System.Net.HttpStatusCode.BadRequest
                });
            }
            return base.SaveChangesAsync(cancellationToken);
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
            //和设置的表主键对应，在mysql主键不能过长，因此修改主键长度
            modelBuilder.Entity<IdentityUserLogin<TKey>>(e => e.Property(p => p.LoginProvider).HasMaxLength(128));
            modelBuilder.Entity<IdentityUserLogin<TKey>>(e => e.Property(p => p.ProviderKey).HasMaxLength(128));

            modelBuilder.Entity<IdentityUserToken<TKey>>(e => e.Property(p => p.LoginProvider).HasMaxLength(128));
            modelBuilder.Entity<IdentityUserToken<TKey>>(e => e.Property(p => p.Name).HasMaxLength(128));
            modelBuilder.Entity<SysPermission>(e => e.HasKey(e => new { e.Entity, e.Permission, e.PermissionType }));

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
