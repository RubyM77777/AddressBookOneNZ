using AddressBookOneNZ.Models;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookOneNZ.Services.Interfaces
{
    /// <summary>
    /// // Definition of IGroupService for managing groups in the address book.
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// Gets all groups.
        /// </summary>
        /// <returns>A list of groups.</returns>
        Task<IEnumerable<Group>> GetAllGroupsAsync(int pageNumber = 1, int pageSize = 10);

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
        Task<Group> UpdateGroupAsync(string CurrentName, [FromBody] Group updateGroup);

        /// <summary>
        /// Deletes a group by Name.
        /// </summary>
        /// <returns>Removes the group.</returns>
        Task<Group?> DeleteGroupAsync(string name);
    }
}
