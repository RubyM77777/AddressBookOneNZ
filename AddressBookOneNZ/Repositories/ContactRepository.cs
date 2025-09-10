using AddressBookOneNZ.Models;
using AddressBookOneNZ.Repositories.Interfaces;
using AddressBookOneNZ.SQLiteDbContext;
using Microsoft.EntityFrameworkCore;

namespace AddressBookOneNZ.Repositories
{
    /// <summary>
    /// // Implementation of ContactRepository for managing contacts in the address book.
    /// // CRUD Operations-List-Concrete List-Direct EFCore DB access-Async Await-Paginated
    /// </summary>
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _dbContext;
        public ContactRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Contact>> GetAllContactsAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Contacts.OrderBy(g => g.FirstName)
                                            .AsNoTracking()
                                            .Include(g => g.Groups)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();
        }
        public async Task<Contact?> GetContactByNameAsync(string firstName, string lastName)
        {
            return await _dbContext.Contacts.Include(g => g.Groups).FirstOrDefaultAsync(c => c.FirstName == firstName && c.LastName == lastName);
        }
        public async Task<Contact> AddContactAsync(Contact Contact)
        {
            _dbContext.Contacts.Add(Contact);
            await _dbContext.SaveChangesAsync();
            return Contact;
        }
        public async Task<Contact> UpdateContactAsync(Contact Contact)
        {
            _dbContext.Contacts.Update(Contact);
            await _dbContext.SaveChangesAsync();
            return Contact;
        }
        public async Task<Contact> DeleteContactAsync(Contact Contact)
        {
            _dbContext.Contacts.Remove(Contact);
            await _dbContext.SaveChangesAsync();
            return Contact;
        }
    }
}
