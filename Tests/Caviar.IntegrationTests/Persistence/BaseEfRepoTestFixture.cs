using Caviar.Infrastructure;
using Caviar.Infrastructure.Identity;
using Caviar.Infrastructure.Persistence;
using Caviar.Infrastructure.Persistence.SysDbContext;
using Caviar.SharedKernel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Caviar.IntegrationTests.Persistence
{
    public class BaseEfRepoTestFixture
    {
        protected ApplicationDbContext<ApplicationUser, ApplicationRole, int> _dbContext;

        public BaseEfRepoTestFixture()
        {
            var builder = new DbContextOptionsBuilder<IdentityDbContext<ApplicationUser, ApplicationRole, int>>()
                .UseInMemoryDatabase("ApplicationDbContext");
            var dbContext = new SysDbContext<ApplicationUser, ApplicationRole, int>(builder.Options);
            Interactor interactor = new Interactor();
            ILanguageService languageService = new InAssemblyLanguageService();
            _dbContext = new ApplicationDbContext<ApplicationUser, ApplicationRole, int>(dbContext, interactor, languageService);
        }

        public ApplicationDbContext<ApplicationUser, ApplicationRole, int> GetDbContext()
        {
            return _dbContext;
        }
    }
}
