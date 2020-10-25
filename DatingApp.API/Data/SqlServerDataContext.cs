using DatingApp.API.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DatingApp.API.Data
{
    public class SqlServerDataContext : DataContext
    {
        public SqlServerDataContext(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLazyLoadingProxies();
            options.UseSqlServer(Configuration.GetConnectionString(ConfigurationName.DefaultConnection));
        }
    }
}