using Caviar.Infrastructure.Identity;
using Caviar.Infrastructure.Persistence;
using Caviar.SharedKernel.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Caviar.IntegrationTests.Persistence
{
    [TestClass]
    public class DbContextTests
    {
        BaseEfRepoTestFixture efCore = new BaseEfRepoTestFixture();
        EasyDbContext<SysMenu> DbContext;
        public DbContextTests()
        {
            DbContext = efCore.GetDbContext<SysMenu>();
        }
        public SysMenu CreateEntity()
        {
            var menu = new SysMenu() { MenuName = "test" };
            var id = DbContext.AddEntityAsync(menu).Result;
            Assert.IsTrue(id == menu.Id);
            return menu;
        }
        public List<SysMenu> CreateEntityList()
        {
            List<SysMenu> menus = new List<SysMenu>();
            for (int i = 0; i < 50; i++)
            {
                menus.Add(CreateEntity());
            }
            return menus;
        }
        public SysMenu GetEntityTest()
        {
            var menu = CreateEntity();
            var entity = DbContext.SingleOrDefaultAsync(u => u.Id == menu.Id).Result;
            Assert.IsTrue(entity!=null);
            return menu;
        }

        public List<SysMenu> GetEntityListTest()
        {
            var menu = CreateEntityList();
            var entity = DbContext.GetAllAsync().Result;
            Assert.IsTrue(entity.Count == menu.Count);
            return menu;
        }

        [TestMethod]
        public void UpdateEntityTest()
        {
            var menu = GetEntityTest();
            var testName = "testUpdate";
            menu.MenuName = testName;
            menu = DbContext.UpdateEntityAsync(menu,false).Result;
            DbContext.SaveChangesAsync(false).Wait();
            Assert.AreEqual(menu.MenuName, testName);
        }




        [TestMethod]
        public void DetleteEntityTest()
        {
            var menu = GetEntityTest();
            DbContext.DeleteEntityAsync(menu).Wait();
            var entity = DbContext.SingleOrDefaultAsync(u => u.Id == menu.Id,isRecycleBin:true).Result;
            Assert.IsTrue(entity != null);
            DbContext.DeleteEntityAsync(menu).Wait();
            entity = DbContext.SingleOrDefaultAsync(u => u.Id == menu.Id, isRecycleBin: true).Result;
            Assert.IsTrue(entity == null);
        }
        [TestMethod]
        public void UpdateEntityListTest()
        {
            var menus = GetEntityListTest().OrderBy(u=>u.Id).ToList();
            for (int i = 0; i < menus.Count; i++)
            {
                menus[i].MenuName =  i + "";
            }
            DbContext.UpdateEntityAsync(menus, false).Wait();
            DbContext.SaveChangesAsync(false).Wait();
            var entity = DbContext.GetAllAsync().Result.OrderBy(u=>u.Id).ToList();
            for (int i = 0; i < entity.Count; i++)
            {
                Assert.AreEqual("" + i, entity[i].MenuName);
            }
        }

        [TestMethod]
        public void DetleteEntityListTest()
        {
            var menu = GetEntityListTest();
            DbContext.DeleteEntityAsync(menu).Wait();
            var entity = DbContext.GetEntityAsync(u => u.IsDelete == true, isRecycleBin: true).Result;
            Assert.IsTrue(entity.Count == menu.Count);
            DbContext.DeleteEntityAsync(menu).Wait();
            entity = DbContext.GetEntityAsync(u => u.IsDelete == true, isRecycleBin: true).Result;
            Assert.IsTrue(entity.Count == 0);
        }
    }
}
