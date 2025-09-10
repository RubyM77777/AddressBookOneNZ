using AddressBookOneNZ.Models;
using AddressBookOneNZ.Repositories.Interfaces;
using AddressBookOneNZ.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookOneNZ.Services
{
    /// <summary>
    /// // Implementation of GroupService for managing groups in the address book.
    /// // IEnumerable-ReadOnlyList-Abstraction-Async Await-Pagination-Validations-Clean Architecture
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger<GroupService> _logger;
        public GroupService(IGroupRepository groupRepository, ILogger<GroupService> logger)
        {
            _groupRepository = groupRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                _logger.LogWarning("Page number less than 1: {PageNumber}", pageNumber);
                throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));
            }

            return await _groupRepository.GetAllGroupsAsync(pageNumber, pageSize);
        }

        public async Task<Group?> GetGroupByNameAsync(string name)
        {
            var existingGroup = await ValidateGroupAsync(name);
            return existingGroup ?? throw new KeyNotFoundException($"Group: {name} not found.");
        }

        public async Task<Group> AddGroupAsync(Group group)
        {
            var existingGroup = await ValidateGroupAsync(group.Name);
            if (existingGroup != null)
                throw new InvalidOperationException($"Group '{group.Name}' already exists. Enter new Group Name to add.");

            return await _groupRepository.AddGroupAsync(group);
        }

        public async Task<Group> UpdateGroupAsync(string currentName, [FromBody] Group updateGroup)
        {
            var existingGroup = await ValidateGroupAsync(currentName);
            if (existingGroup == null)
                throw new InvalidOperationException($"Group: {currentName} does not exist. Enter existing Group Name to update.");

            // Update the group
            await ValidateGroupAsync(updateGroup.Name);
            existingGroup.Name = updateGroup.Name;
            
            return await _groupRepository.UpdateGroupAsync(existingGroup);
        }

        public async Task<Group?> DeleteGroupAsync(string name)
        {
            var existingGroup = await ValidateGroupAsync(name);
            if (existingGroup == null)
                throw new InvalidOperationException($"Group: {name} does not exist. Enter existing Group Name to delete.");
            
            return await _groupRepository.DeleteGroupAsync(existingGroup);
        }

        private async Task<Group?> ValidateGroupAsync(string groupName)
        {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrWhiteSpace(groupName))
                throw new ArgumentException("Group name cannot be null or empty.", nameof(groupName));

            if (groupName == "string")
                throw new ArgumentException("Enter a new valid Group name.", nameof(groupName));

            if (!groupName.All(c => char.IsLetterOrDigit(c) || c == ' '))
                throw new ArgumentException("Group name can only contain letters, digits, and spaces.", nameof(groupName));

            if (groupName.Length < 2)
                throw new ArgumentException("Group name must be at least 2 characters long.", nameof(groupName));

            if (groupName.Length > 50)
                throw new ArgumentException("Group name must not exceed 50 characters.", nameof(groupName));    

            // Check if group exists. Returns null if not found.
            return await _groupRepository.GetGroupByNameAsync(groupName);
        }
    }
}
