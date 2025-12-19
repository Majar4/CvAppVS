using Microsoft.EntityFrameworkCore;

namespace CvAppenVS.Models
{
    public class CvContext : DbContext
    {
        public CvContext(DbContextOptions<CvContext> options) : base(options) { }
        //public DbSet<User> Users { get; set; } <-- så ska propertysarna se ut
        //public DbSet<> av de entiteter vi har.
        // DbContext hanterar CRUD
        // DbSet länkar models till databasen
    }
}
