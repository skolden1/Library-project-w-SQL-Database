using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboration_SQL_up1
{
    public class UserContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public UserContext(): base("name=UserDatabaseConnectionString")
        {

        }
    }
}
