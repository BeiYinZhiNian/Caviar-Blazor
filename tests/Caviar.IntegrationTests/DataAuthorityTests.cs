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
using System.Text;
using System.Threading.Tasks;

namespace Caviar.IntegrationTests
{
    [TestClass]
    public class DataAuthorityTests
    {
        SysDbContext<ApplicationUser, ApplicationRole, int> SysDbContext { get; set; }
        int _randomRange = 10;
        Random _random = new Random();
        readonly ILanguageService _languageService = new InAssemblyLanguageService();
        
        public DataAuthorityTests()
        {
            CaviarConfig CaviarConfig = new CaviarConfig();
            var builder = new DbContextOptionsBuilder<IdentityDbContext<ApplicationUser, ApplicationRole, int>>()
               .UseInMemoryDatabase("ApplicationDbContext");
            SysDbContext = new SysDbContext<ApplicationUser, ApplicationRole, int>(builder.Options, CaviarConfig);
            SetTestData();
        }

        /// <summary>
        /// 设置初始数据
        /// </summary>
        /// <returns></returns>
        private void SetTestData()
        {
            var menuList = new List<SysMenu>();
            for (int t = 0; t < 200; t++)
            {
                menuList.Add(new SysMenu()
                {
                    MenuName = "test",
                    DataId = _random.Next(_randomRange),
                });
            }
            var userGroupList = new List<SysUserGroup>();
            for (int i = 0; i < 100; i++)
            {
                userGroupList.Add(new SysUserGroup()
                {
                    Name = "test",
                    ParentId = _random.Next(_randomRange),
                });
            }
            var menuSet = SysDbContext.Set<SysMenu>();
            menuSet.AddRange(menuList);
            var userGroupSet = SysDbContext.Set<SysUserGroup>();
            userGroupSet.AddRange(userGroupList);
            SysDbContext.SaveChanges();
        }

        /// <summary>
        /// 读取本级测试
        /// </summary>
        [TestMethod]
        public void LevelTest()
        {
            var testData = SysDbContext.Set<SysMenu>().ToList();
            var userGroupId = _random.Next(_randomRange); //所在用户组
            Interactor interactor = new Interactor();
            interactor.ApplicationRoles = new List<ApplicationRole>() 
            { 
                new ApplicationRole() { DataRange = DataRange.Level },
            };
            interactor.UserInfo = new ApplicationUser()
            {
                UserGroupId = userGroupId
            };
            var appDbContext = new ApplicationDbContext(SysDbContext, interactor, _languageService);
            var menus = appDbContext.GetAllAsync<SysMenu>().ToList();
            var userGroupList = new List<int>() { 0,userGroupId};
            var userGroupData = testData.Where(u => userGroupList.Contains(u.DataId)).ToList();//获取本级数据
            Assert.AreEqual(menus.Count, userGroupData.Count); // 判断读取数量是否相等
            var exceptList = menus.Except(userGroupData).ToList();
            Assert.AreEqual(exceptList.Count, 0); // 判断读取数据是否有差别
        }

        /// <summary>
        /// 读取本级和下级测试
        /// </summary>
        [TestMethod]
        public void SubordinateTest()
        {
            var testData = SysDbContext.Set<SysMenu>().ToList();
            var userGroupId = _random.Next(_randomRange); //所在用户组
            Interactor interactor = new Interactor();
            interactor.ApplicationRoles = new List<ApplicationRole>()
            {
                new ApplicationRole() { DataRange = DataRange.Subordinate },
            };
            interactor.UserInfo = new ApplicationUser()
            {
                UserGroupId = userGroupId
            };
            var appDbContext = new ApplicationDbContext(SysDbContext, interactor, _languageService);
            var menus = appDbContext.GetAllAsync<SysMenu>().ToList();
            var childrenSet = SysDbContext.Set<SysUserGroup>();
            var childrenIds = childrenSet.Where(u => u.ParentId == userGroupId).Select(u=>u.Id).ToList();
            var userGroupList = new List<int>() { 0, userGroupId };
            userGroupList.AddRange(childrenIds);
            var userGroupData = testData.Where(u => userGroupList.Contains(u.DataId)).ToList();//获取本级和下级数据
            Assert.AreEqual(menus.Count, userGroupData.Count); // 判断读取数量是否相等
            var exceptList = menus.Except(userGroupData).ToList();
            Assert.AreEqual(exceptList.Count, 0); // 判断读取数据是否有差别
        }

        /// <summary>
        /// 自定义数据测试
        /// </summary>
        [TestMethod]
        public void CustomTest()
        {
            var testData = SysDbContext.Set<SysMenu>().ToList();
            var userGroupId = _random.Next(_randomRange); //所在用户组
            Interactor interactor = new Interactor();
            var randomList = new List<int>();
            var dataList = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                var id = _random.Next(_randomRange);
                randomList.Add(id);
                dataList.Append($"{id}{CurrencyConstant.CustomDataSeparator}");
            }
            interactor.ApplicationRoles = new List<ApplicationRole>()
            {
                new ApplicationRole() { DataRange = DataRange.Custom,DataList=dataList.ToString() },
            };
            interactor.UserInfo = new ApplicationUser()
            {
                UserGroupId = userGroupId
            };
            var appDbContext = new ApplicationDbContext(SysDbContext, interactor, _languageService);
            var menus = appDbContext.GetAllAsync<SysMenu>().ToList();
            var childrenSet = SysDbContext.Set<SysUserGroup>();
            var userGroupList = new List<int>() { 0 };
            userGroupList.AddRange(randomList);
            var userGroupData = testData.Where(u => userGroupList.Contains(u.DataId)).ToList();//自定义数据获取
            Assert.AreEqual(menus.Count, userGroupData.Count); // 判断读取数量是否相等
            var exceptList = menus.Except(userGroupData).ToList();
            Assert.AreEqual(exceptList.Count, 0); // 判断读取数据是否有差别
        }

        /// <summary>
        /// 所有数据测试
        /// </summary>
        [TestMethod]
        public void AllTest()
        {
            var testData = SysDbContext.Set<SysMenu>().ToList();
            var userGroupId = _random.Next(_randomRange); //所在用户组
            Interactor interactor = new Interactor();
            var randomList = new List<int>();
            var dataList = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                var id = _random.Next(_randomRange);
                randomList.Add(id);
                dataList.Append($"{id}{CurrencyConstant.CustomDataSeparator}");
            }
            interactor.ApplicationRoles = new List<ApplicationRole>()
            {
                new ApplicationRole() { DataRange = DataRange.All },
            };
            interactor.UserInfo = new ApplicationUser()
            {
                UserGroupId = userGroupId
            };
            var appDbContext = new ApplicationDbContext(SysDbContext, interactor, _languageService);
            var menus = appDbContext.GetAllAsync<SysMenu>().ToList();
            var userGroupData = testData;//自定义数据获取
            Assert.AreEqual(menus.Count, userGroupData.Count); // 判断读取数量是否相等
            var exceptList = menus.Except(userGroupData).ToList();
            Assert.AreEqual(exceptList.Count, 0); // 判断读取数据是否有差别
        }
    }
}
