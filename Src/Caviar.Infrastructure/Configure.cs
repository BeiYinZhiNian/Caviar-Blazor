using Caviar.Core.Interface;
using Caviar.Infrastructure.Identity;
using Caviar.Infrastructure.Persistence;
using Caviar.Infrastructure.Persistence.Sys;
using Caviar.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure
{
    public static class Configure
    {
        public static void ConfigureServices(this IServiceCollection services)
        {

            

        }
        /// <summary>
        /// 自定义用户表和角色表
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="services"></param>
        public static void AddCaviarIdentity<TUser, TRole>(this IServiceCollection services)
            where TUser : IdentityUser<int>, IBaseEntity
            where TRole : IdentityRole<int>, IBaseEntity
        {
            services.AddIdentity<TUser, TRole>()
                    .AddEntityFrameworkStores<SysDbContext<TUser, TRole, int>>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

            services.AddTransient<IDbContext, SysDbContext<TUser, TRole, int>>();
        }



    }
}
