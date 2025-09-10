using AddressBookOneNZ.Models;
using AddressBookOneNZ.Repositories.Interfaces;
using AddressBookOneNZ.Services.Interfaces;

namespace AddressBookOneNZ.Services
{
    /// <summary>
    /// // Implementation of ContactService for managing Contacts in the address book.
    /// // IEnumerable-ReadOnlyList-Abstraction-Async Await-Pagination-Validations-Clean Architecture
    /// </summary>
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IGroupRepository _groupRepository;

        public ContactService(IContactRepository contactRepository, IGroupRepository groupRepository)
        {
            _contactRepository = contactRepository;
            _groupRepository = groupRepository;
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));

            return await _contactRepository.GetAllContactsAsync(pageNumber, pageSize);
        }

        public async Task<Contact?> GetContactByNameAsync(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Contact FirstName and LastName cannot be null or empty.");

            var existingContact = await _contactRepository.GetContactByNameAsync(firstName, lastName);
            return existingContact ?? throw new KeyNotFoundException($"Contact '{firstName} {lastName}' not found.");
        }

        public async Task<Contact> AddContactAsync(Contact contact)
        {
            var existingContact = await ValidateContactAsync(contact);
            if (existingContact != null)
                throw new InvalidOperationException($"Contact '{contact.FirstName} {contact.LastName}' already exists.");

            // Map GroupNames into Groups - Add contact to groups.
            await MapContactToGroupsAsync(contact);

            return await _contactRepository.AddContactAsync(contact);
        }

        public async Task<Contact> UpdateContactAsync(Contact contact)
        {
            var existingContact = await ValidateContactAsync(contact);
            if (existingContact == null)
                throw new InvalidOperationException($"Contact '{contact.FirstName} {contact.LastName}' does not exist.");

            // Update contact fields
            existingContact.Email = contact.Email;
            existingContact.PhoneNumber = contact.PhoneNumber;

            await MapContactToGroupsAsync(contact);
            existingContact.GroupNames = contact.GroupNames;

            return await _contactRepository.UpdateContactAsync(existingContact);
        }

        public async Task<Contact> DeleteContactAsync(Contact contact)
        {
            var existingContact = await ValidateContactAsync(contact);
            if (existingContact == null)
                throw new InvalidOperationException($"Contact '{contact.FirstName} {contact.LastName}' does not exist.");

            return await _contactRepository.DeleteContactAsync(existingContact);
        }

        private async Task<Contact?> ValidateContactAsync(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.FirstName) || string.IsNullOrWhiteSpace(contact.LastName))
                throw new ArgumentException("Contact FirstName or Lastname cannot be null or empty.");

            if (contact.FirstName == "string" || contact.LastName == "string")
                throw new ArgumentException("Enter a valid Contact details.");

            if (contact.FirstName.Length < 2 || contact.LastName.Length < 2)
                throw new ArgumentException("Contact FirstName and Lastname must be at least 2 characters long.");

            if (contact.FirstName.Length > 50 || contact.LastName.Length > 50)
                throw new ArgumentException("Contact FirstName and Lastname must not exceed 50 characters.", nameof(contact));

            if (!contact.FirstName.All(c => char.IsLetter(c) || c == ' ') || 
                !contact.LastName.All(c => char.IsLetter(c) || c == ' '))
                throw new ArgumentException("Contact FirstName and Lastname can only contain letters and spaces.", nameof(contact));

            if (contact.PhoneNumber == null || string.IsNullOrWhiteSpace(contact.PhoneNumber))
                throw new ArgumentException("Contact PhoneNumber cannot be null or empty.", nameof(contact.PhoneNumber));

            if (contact.PhoneNumber.Length < 10)
                throw new ArgumentException("Contact PhoneNumber must be at least 10 characters long.");

            if (contact.PhoneNumber.Length > 15)
                throw new ArgumentException("Contact PhoneNumber must not exceed 15 characters.", nameof(contact.PhoneNumber));

            if (contact.PhoneNumber.Any(c => !char.IsDigit(c)))
                throw new ArgumentException("Contact PhoneNumber must contain only digits.");

            if (contact.Email == null || string.IsNullOrWhiteSpace(contact.Email))
                throw new ArgumentException("Contact Email cannot be null or empty.");

            if (!contact.Email.Contains("@") || !contact.Email.Contains('.'))
                throw new ArgumentException("Contact Email must be a valid email address.");

            if (!contact.Email.All(c => char.IsLetterOrDigit(c) || c == '@' || c == '.' || c == '_'))
                throw new ArgumentException("Contact Email can only contain letters, digits, '@', '.', and '_'.", nameof(contact.Email));

            if (contact.Email.Length < 5)
                throw new ArgumentException("Contact Email must be at least 5 characters long.", nameof(contact.Email));

            if (contact.Email.Length > 100)
                throw new ArgumentException("Contact Email must not exceed 100 characters.", nameof(contact.Email));

            // Check if Contact exists. Returns null if not found.
            return await _contactRepository.GetContactByNameAsync(contact.FirstName, contact.LastName);
        }

        private async Task MapContactToGroupsAsync(Contact contact)
        {
            if (contact.GroupNames == null || contact.GroupNames.Count == 0)
                return;

            foreach (var groupName in contact.GroupNames.Distinct())
            {
                var group = await _groupRepository.GetGroupByNameAsync(groupName);
                if (group != null)
                {
                    if (!contact.Groups.Contains(group))
                        contact.Groups.Add(group);
                }
                else
                {
                    throw new InvalidOperationException($"Group: {groupName} does not exist.");
                }
            }
        }
    }
}
