using AddressBookOneNZ.Models;
using AddressBookOneNZ.Repositories.Interfaces;
using AddressBookOneNZ.SQLiteDbContext;
using Microsoft.EntityFrameworkCore;

namespace AddressBookOneNZ.Repositories
{
    /// <summary>
    /// // Implementation of GroupRepository for managing groups in the address book.
    /// // CRUD Operations-List-Concrete List-Direct EFCore DB access-Async Await-Paginated
    /// </summary>
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _dbContext;
        public GroupRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Group>> GetAllGroupsAsync(int pageNumber, int pageSize)
        {
            // As large Data is Paginated (many groups with many contacts), Select is preferred for Groups (Lazy Loading) 
            return await _dbContext.Groups.OrderBy(g => g.Name)
                                          .AsNoTracking()
                                          .Include(g => g.Contacts)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync();
        }
        public async Task<Group?> GetGroupByNameAsync(string name)
        {
            // Only one Group with all contacts, Include is preferred (Eager Loading)
            return await _dbContext.Groups.Include(g => g.Contacts).FirstOrDefaultAsync(g => g.Name == name);
        }
        public async Task<Group> AddGroupAsync(Group group)
        {
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();
            return group;
        }
        public async Task<Group> UpdateGroupAsync(Group group)
        {
            _dbContext.Groups.Update(group);
            await _dbContext.SaveChangesAsync();
            return group;
        }
        public async Task<Group> DeleteGroupAsync(Group group)
        {
            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();
            return group;
        }
    }
}
