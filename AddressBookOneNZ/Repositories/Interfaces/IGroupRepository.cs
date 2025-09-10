using AddressBookOneNZ.Models;

namespace AddressBookOneNZ.Repositories.Interfaces
{
    /// <summary>
    /// // Definition of IGroupRepository for managing groups in the address book.
    /// </summary>
    public interface IGroupRepository
    {
        /// <summary>
        /// Gets all groups.
        /// </summary>
        /// <returns>A list of groups.</returns>
        Task<List<Group>> GetAllGroupsAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Gets a group by Name.
        /// </summary>
        /// <returns>The group with the specified Name.</returns>
        Task<Group?> GetGroupByNameAsync(string name);

        /// <summary>
        /// Adds a new group.
        /// </summary>
        /// <returns>The added group.</returns>
        Task<Group> AddGroupAsync(Group group);

        /// <summary>
        /// Updates an existing group.
        /// </summary>
        /// <returns>The updated group.</returns>
        Task<Group> UpdateGroupAsync(Group group);

        /// <summary>
        /// Deletes an existing group.
        /// </summary>
        /// <returns>Removes the group.</returns>
        Task<Group> DeleteGroupAsync(Group group);
    }
}
