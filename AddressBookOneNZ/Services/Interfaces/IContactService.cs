using AddressBookOneNZ.Models;

namespace AddressBookOneNZ.Services.Interfaces
{
    /// <summary>
    /// // Definition of IContactService for managing groups in the address book.
    /// </summary>
    public interface IContactService
    {
        /// <summary>
        /// Gets all contacts.
        /// </summary>
        /// <returns>A list of contacts.</returns>
        Task<IEnumerable<Contact>> GetAllContactsAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Gets a contact by Name.
        /// </summary>
        /// <returns>The contact with the specified Name.</returns>
        Task<Contact?> GetContactByNameAsync(string firstName, string lastName);

        /// <summary>
        /// Adds a new contact.
        /// </summary>
        /// <returns>The added contact.</returns>
        Task<Contact> AddContactAsync(Contact contact);

        /// <summary>
        /// Updates an existing contact.
        /// </summary>
        /// <returns>The updated contact.</returns>
        Task<Contact> UpdateContactAsync(Contact contact);

        /// <summary>
        /// Deletes an existing contact.
        /// </summary>
        /// <returns>Removes the contact.</returns>
        Task<Contact> DeleteContactAsync(Contact contact);
    }
}
