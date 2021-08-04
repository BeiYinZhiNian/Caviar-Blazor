using Caviar.SharedKernel.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.IntegrationTests.Persistence
{
    [TestClass]
    public class MenuEfActionTests
    {
        BaseEfRepoTestFixture efCore = new BaseEfRepoTestFixture();
        ViewMenu menu;
        [TestMethod]
        public void AddMenuTest()
        {
            var dbContext = efCore.GetDbContext();
            menu = new ViewMenu() { MenuName = "测试"};
            var id = dbContext.AddEntityAsync(menu).Result;
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void GetMenuTest()
        {
            var dbContext = efCore.GetDbContext();
            var entity = dbContext.GetEntityAsync<ViewMenu>(u => u.Id == menu.Id);
            Assert.IsTrue(entity.Id > 0);
        }
    }
}
