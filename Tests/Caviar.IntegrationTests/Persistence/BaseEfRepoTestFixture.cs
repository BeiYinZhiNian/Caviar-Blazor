using Caviar.Infrastructure;
using Caviar.Infrastructure.Identity;
using Caviar.Infrastructure.Persistence;
using Caviar.Infrastructure.Persistence.Sys;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Caviar.IntegrationTests.Persistence
{
    public class BaseEfRepoTestFixture
    {

        public EasyDbContext<T> GetDbContext<T>() where T:class,IBaseEntity,new()
        {
            var builder = new DbContextOptionsBuilder<IdentityDbContext<ApplicationUser, ApplicationRole, int>>()
                .UseInMemoryDatabase("ApplicationDbContext");
            var dbContext = new SysDbContext(builder.Options);
            Interactor interactor = new Interactor();
            ILanguageService languageService = new InAssemblyLanguageService();
            var _dbContext = new EasyDbContext<T>(dbContext, interactor, languageService);
            interactor.UserData.Fields = _dbContext.GetAllAsync<SysFields>().Result;
            return _dbContext;
        }
    }
}
