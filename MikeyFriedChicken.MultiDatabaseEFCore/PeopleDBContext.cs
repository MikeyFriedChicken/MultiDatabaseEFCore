using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MikeyFriedChicken.MultiDatabaseEFCore
{
    public class PeopleDBContext : DbContext
    {

        public PeopleDBContext(DbContextOptions<PeopleDBContext> options) : base(options)
        {

        }

        /// <summary>
        /// This allows a sub class to call the base class 'DbContext' non typed constructor
        /// This is need because instances of the subclasses will use a specifc typed DbContextOptions
        /// which can not be converted into the paramter in the above constructor
        /// </summary>
        /// <param name="options"></param>
        protected PeopleDBContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            throw new Exception("This method must be overriden as we have database typenames which need to be defined");
        }

    }

    public class PeopleDBContextSQLServer : PeopleDBContext
    {

        public PeopleDBContextSQLServer(DbContextOptions<PeopleDBContextSQLServer> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(a => a.MetaInfo).HasColumnType("varchar(max)");
        }

    }

    public class PeopleDBContextPostGresSQL: PeopleDBContext
    {
        /// <summary>
        /// Options will be of this specific type and we pass this to the protected not typed base class constructor
        /// </summary>
        /// <param name="options"></param>
        public PeopleDBContextPostGresSQL(DbContextOptions<PeopleDBContextPostGresSQL> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(a => a.MetaInfo).HasColumnType("json");
        }
    }
}
