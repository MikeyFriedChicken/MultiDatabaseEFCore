# Support Multiple Database in Entity Framework Core

 [Medium Post](https://medium.com/@michaelceber/how-to-support-multiple-databases-in-entity-framework-core-1ccd24896829)
 
Sample code for easily switching between different database types whislt allowing for database specific settings such as column types to be configured database specifically.

In this example I show how to configure the following databases ...

  - Microsoft SQL Server
  - Postgres SQL

... and show how to use dependency injection and subclasses to achieve this.

Firstly, depending on the configuration create the appropriate instance of DBContext:

```csharp
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
```
'ConfigureSQLServer'
```csharp
        private void ConfigureSQLServer(DbContextOptionsBuilder options)
        {
            string sqlServerConnectionString = "user id=sa;password=YourStrong!Passw0rd;server=localhost,1433;database=PeopleDatabase;Trusted_Connection=no";

            options.UseSqlServer(sqlServerConnectionString);
            options.EnableSensitiveDataLogging();
        }
```

and 'ConfigurePostgresSQL'

```csharp
        private void ConfigurePostgresSQL(DbContextOptionsBuilder options)
        {
            string postgresSqlConnectionString = "Host=localhost;Database=PeopleDatabase;Username=postgres;Password=example";

            options.UseNpgsql(postgresSqlConnectionString);
            options.EnableSensitiveDataLogging();
        }
```

In this sample we have a Person entity as shown below.  For FirstName and LastName we can set the typename as an attribute as all databases support this.  However, we leave MetaInfo for now as we would like to have different types depending on whether SQL Server or Postgres SQL is used.

Our 'Person' model class:

```csharp
    public class Person
    {

        [Key]
        public long PersonId { get; set; }

        [Column("FirstName", TypeName = "varchar(50)")]
        public string FirstName { get; set; }

        [Column("LastName", TypeName = "varchar(50)")]
        public string LastName { get; set; }

        // As attributes can only reference constants
        // we must set the typename in OnModelCreating in the database context
        // as we want this to vary depending on the type of context used, i.e sql server, or postgressql
        [Column("MetaInfo")]
        public string MetaInfo { get; set; }
    }
```

In SQL Server we want to set the Meta column type to 'varchar(max)', however, in Postgres SQL we would like to use a type of 'json'.  

To do this we define all such differences in the OnModelCreating method of DBContext.  Hence, we create a subclass for each database as follows:

Define a sub class for SQL Server

```csharp
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

```



Define a subclass for Postgres SQL
```csharp
    public class PeopleDBContext : DbContext
    {

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
```

And that is it!

Why do this?  Well my main use case is that I have an application which might be using SQL Server in development, however, I know at a later stage or perhaps a different customer will want to use another database.   I want to keep duplication to a minimum and allow for seamless swapping of the database.  So I consolidtate all such differens to the the 'OnModelCreating' method of each database subclass keeping the code simple and maintainable.
