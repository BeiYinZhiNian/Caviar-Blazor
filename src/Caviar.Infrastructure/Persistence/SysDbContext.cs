﻿// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caviar.Core.Interface;
using Caviar.Core.Services;
using Caviar.SharedKernel.Common;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Caviar.Infrastructure.Persistence
{
    public class SysDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>, IDbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly CaviarConfig _caviarConfig;
        private readonly IInteractor _interactor;
        private readonly ILanguageService _languageService;
        public SysDbContext(DbContextOptions options,
            CaviarConfig caviarConfig,
            IInteractor interactor,
            ILanguageService languageService) : base(options)
        {
            _caviarConfig = caviarConfig;
            _interactor = interactor;
            _languageService = languageService;
        }

        public SysDbContext() : base()
        {

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //检测是否为初始化
            if (Configure.HasDataInit) { return base.SaveChangesAsync(cancellationToken); }
            if (_caviarConfig.DemonstrationMode)
            {
                throw new ResultException(new ResultMsg()
                {
                    Title = "当前处于演示模式，该功能无法使用，更多精彩功能，请下载源代码后体验",
                    Url = "https://gitee.com/Cherryblossoms/caviar",
                    Status = System.Net.HttpStatusCode.BadRequest
                });
            }
            ChangeTracker.DetectChanges(); // Important!
            var entries = ChangeTracker.Entries();
            foreach (var item in entries)
            {
                IUseEntity baseEntity;
                var entity = item.Entity;
                if (entity == null) continue;
                if (entity is not IUseEntity) continue;
                baseEntity = entity as IUseEntity;
                switch (item.State)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    case EntityState.Modified:
                        baseEntity.OperatorUp = _interactor.UserInfo?.UserName;
                        baseEntity.UpdateTime = CommonHelper.GetSysDateTimeNow();
                        var entityType = entity.GetType();
                        var baseType = typeof(IUseEntity);
                        var fields = FieldScannerServices.GetClassFields(baseType, _languageService);
                        foreach (var fieldItem in fields)
                        {
                            switch (fieldItem.Entity.FieldName.ToLower())
                            {
                                //不可更新字段
                                case "id":
                                case "uid":
                                case "creattime": // 创建时间
                                case "operatorcare": // 创建者
                                case "dataid": // 数据权限
                                case "isdelete": // 是否删除
                                    item.Property(fieldItem.Entity.FieldName).IsModified = false;
                                    continue;
                                //系统更新字段
                                case "updatetime":
                                case "operatorup":
                                    item.Property(fieldItem.Entity.FieldName).IsModified = true;
                                    continue;
                                default:
                                    break;
                            }
                        }
                        break;
                    case EntityState.Added:
                        baseEntity.CreatTime = CommonHelper.GetSysDateTimeNow();
                        baseEntity.OperatorCare = _interactor.UserInfo?.UserName;
                        if (baseEntity.DataId != CurrencyConstant.PublicData) //当不为公共数据时，必须为当前用户组
                        {
                            baseEntity.DataId = _interactor.UserInfo?.UserGroupId ?? CurrencyConstant.PublicData;
                        }
                        break;
                    default:
                        break;
                }
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
