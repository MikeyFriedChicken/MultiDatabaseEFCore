using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MikeyFriedChicken.MultiDatabaseEFCore
{
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
}
