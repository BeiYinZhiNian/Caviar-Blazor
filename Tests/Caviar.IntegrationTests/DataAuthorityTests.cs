using Caviar.Core.Interface;
using Caviar.Infrastructure.Persistence;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.IntegrationTests
{
    [TestClass]
    public class DataAuthorityTests
    {

        public DataAuthorityTests()
        {
            
        }

        /// <summary>
        /// 设置初始数据
        /// </summary>
        /// <returns></returns>
        private List<SysMenu> SetLeveTestData(out IAppDbContext appDbContext)
        {
            var builder = new DbContextOptionsBuilder<IdentityDbContext<ApplicationUser, ApplicationRole, int>>()
               .UseInMemoryDatabase("ApplicationDbContext");
            var dbContext = new SysDbContext<ApplicationUser, ApplicationRole, int>(builder.Options);
            ILanguageService languageService = new InAssemblyLanguageService();
            Interactor interactor = new Interactor();
            Random random = new Random();
            var list = new List<SysMenu>();
            var applicationDb = new ApplicationDbContext(dbContext, interactor, languageService);
            for (int i = 0; i < 100; i++)
            {
                //设置当前角色
                interactor.ApplicationRoles = new[]
                {
                new ApplicationRole()
                {
                    DataRange = DataRange.Level
                }
                };
                interactor.UserInfo = new ApplicationUser()
                {
                    UserGroupId = random.Next(5),
                };
                
                
                List<SysMenu> sysMenu = new List<SysMenu>();
                for (int t = 0; t < 10; t++)
                {
                    sysMenu.Add(new SysMenu()
                    {
                        Key = "test",
                    });
                }
                applicationDb.AddEntityAsync(sysMenu).Wait();
                list.AddRange(sysMenu);
            }
            appDbContext = applicationDb;
            return list;
        }

        [TestMethod]
        public void LevelTest()
        {
            var testData = SetLeveTestData(out IAppDbContext appDbContext);
        }
    }
}
