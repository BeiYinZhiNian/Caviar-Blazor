using Caviar.Models.SystemData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Caviar.Control
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }

        public DataContext():base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder = optionsBuilder.UseLazyLoadingProxies();
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = CaviarConfig.SqlConfig.Connections;
                switch (CaviarConfig.SqlConfig.DBTypeEnum)
                {
                    case DBTypeEnum.SqlServer:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;
                    case DBTypeEnum.MySql:
                        optionsBuilder.UseMySql(connectionString, null, null);
                        break;
                    case DBTypeEnum.PgSql:
                        optionsBuilder.UseNpgsql(connectionString);
                        break;
                    case DBTypeEnum.Memory:
                        optionsBuilder.UseInMemoryDatabase(connectionString);
                        break;
                    case DBTypeEnum.SQLite:
                        optionsBuilder.UseSqlite(connectionString);
                        break;
                    case DBTypeEnum.Oracle:
                        optionsBuilder.UseOracle(connectionString);
                        break;
                    default:
                        break;
                }
                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var types = CommonHelper.GetModelList();
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

        }

    }


}
