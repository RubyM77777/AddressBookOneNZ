using AddressBookOneNZ.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressBookOneNZ.SQLiteDbContext
{
    /// <summary>
    /// /// EF Core DB context for the Address Book application using SQLite.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Group> Groups { get; set; }


    }
}
