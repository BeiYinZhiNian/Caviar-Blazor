using Caviar.Core;
using Caviar.Core.Interface;
using Caviar.Infrastructure.Identity;
using Caviar.Infrastructure.Persistence;
using Caviar.Infrastructure.Persistence.Sys;
using Caviar.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        public static void AddCaviar(this IServiceCollection services)
        {
            services.AddScoped<ILanguageService, InAssemblyLanguageService>();
            services.AddScoped<Interactor>();
            services.AddTransient<IAppDbContext, ApplicationDbContext>();
            services.AddTransient(typeof(IEasyDbContext<>),typeof(EasyDbContext<>));
            AutomaticInjection injection = new AutomaticInjection();
            injection.AddIBaseModel(services);
            injection.AddInject(services);

        }
        /// <summary>
        /// 自定义用户表和角色表
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="services"></param>
        public static void AddCaviarIdentity<TUser, TRole>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TUser : IdentityUser<int>, IBaseEntity
            where TRole : IdentityRole<int>, IBaseEntity
        {
            services.AddDbContext<SysDbContext<TUser, TRole, int>>(optionsAction, contextLifetime, optionsLifetime);
            services.AddIdentity<TUser, TRole>()
                    .AddEntityFrameworkStores<SysDbContext<TUser, TRole, int>>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

            services.AddTransient<IDbContext, SysDbContext<TUser, TRole, int>>();
        }

        /// <summary>
        /// 自定义用户表和角色表
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="services"></param>
        public static void AddCaviarIdentity(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            services.AddCaviarIdentity<ApplicationUser, ApplicationRole>(optionsAction, contextLifetime, optionsLifetime);
        }



    }

    class AutomaticInjection
    {

        /// <summary>
        /// 遍历所有的类，筛选实现IService接口的类，并过判断是否是类,并按照注解方式自动注入类
        /// 自动注入所有继承IDIinjectAtteribute接口的类
        /// </summary>
        public void AddIBaseModel(IServiceCollection services)
        {
            CommonHelper.GetAssembly()
                //遍历查找
                .ForEach((t =>
                {
                        //获取所有对象
                        t.GetTypes()
                        //查找是否包含IDIinjectAtteribute接口的类
                        .Where(u => u.GetInterfaces().Contains(typeof(IDIinjectAtteribute)))
                        //判断是否是类
                        .Where(u => u.IsClass)
                        //转换成list
                        .ToList()
                        //循环,并添注入
                        .ForEach(t =>
                        {
                            services.AddTransient(t);
                        });
                }));

        }
        /// <summary>
        /// 自动注入所有带有Inject特性类
        /// </summary>
        /// <param name="services"></param>
        public void AddInject(IServiceCollection services)
        {
            CommonHelper.GetAssembly()
               //遍历查找
               .ForEach((t =>
               {
                       //获取所有对象
                       t.GetTypes()
                       //查找是否包含DI特性并且查看是否是抽象类
                       .Where(a => a.GetCustomAttributes(true).Select(t => t.GetType()).Contains(typeof(DIInjectAttribute)) && !a.IsAbstract)
                       //判断是否是类
                       .Where(u => u.IsClass)
                       //转换成list
                       .ToList()
                       //循环,并添注入
                       .ForEach(t =>
                       {
                           var inject = (DIInjectAttribute)t.GetCustomAttributes(true).FirstOrDefault(d => d.GetType() == typeof(DIInjectAttribute));
                           switch (inject.InjectType)
                           {
                               case InjectType.SINGLETON:
                                   services.AddSingleton(t);
                                   break;
                               case InjectType.SCOPED:
                                   services.AddScoped(t);
                                   break;
                               case InjectType.TRANSIENT:
                                   services.AddTransient(t);
                                   break;
                           }
                       });
               }));
        }
    }
}
