using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MikeyFriedChicken.MultiDatabaseEFCore
{

    public class DatabaseSettings
    {
        public string DatabaseType { get; set; }
    }

    public class Startup
    {


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var databaseSection = Configuration.GetSection("Database");
            services.Configure<DatabaseSettings>(Configuration.GetSection("Database"));
            var databaseSettings = databaseSection.Get<DatabaseSettings>();

            if (databaseSettings.DatabaseType == "SQLServer")
            {
                 services.AddDbContext<PeopleDBContext,PeopleDBContextSQLServer>(ConfigureSQLServer);
            }
            else
            {
                services.AddDbContext<PeopleDBContext,PeopleDBContextPostGresSQL>(ConfigurePostgresSQL);
            }
        }

        private void ConfigurePostgresSQL(DbContextOptionsBuilder options)
        {
            string postgresSqlConnectionString = "Host=localhost;Database=PeopleDatabase;Username=postgres;Password=example";

            options.UseNpgsql(postgresSqlConnectionString);
            options.EnableSensitiveDataLogging();
        }

        private void ConfigureSQLServer(DbContextOptionsBuilder options)
        {
            string sqlServerConnectionString = "user id=sa;password=YourStrong!Passw0rd;server=localhost,1433;database=PeopleDatabase;Trusted_Connection=no";

            options.UseSqlServer(sqlServerConnectionString);
            options.EnableSensitiveDataLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
