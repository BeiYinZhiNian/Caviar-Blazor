using Caviar.Infrastructure.Identity;
using Caviar.Infrastructure.Persistence;
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

            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //        .AddEntityFrameworkStores<ApplicationDbContext>()
            //        .AddDefaultUI()
            //        .AddDefaultTokenProviders();

        }

    }
}
