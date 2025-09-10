using AddressBookOneNZ.Models;

namespace AddressBookOneNZ.Repositories.Interfaces
{
    /// <summary>
    /// ///// <summary>
    /// // Definition of IContactRepository for managing contacts in the address book.
    /// </summary>
    /// </summary>
    public interface IContactRepository
    {
        /// <summary>
        /// Gets all contacts.
        /// </summary>
        /// <returns>A list of contacts.</returns>
        Task<List<Contact>> GetAllContactsAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Gets a contact by Name.
        /// </summary>
        /// <param name="id">The Name of the contact.</param>
        /// <returns>The contact with the specified Name.</returns>
        Task<Contact?> GetContactByNameAsync(string firstName, string lastName);

        /// <summary>
        /// Adds a new contact.
        /// </summary>
        /// <param name="contact">The contact to add.</param>
        /// <returns>The added contact.</returns>
        Task<Contact> AddContactAsync(Contact contact);

        /// <summary>
        /// Updates an existing contact.
        /// </summary>
        /// <param name="contact">The contact to update.</param>
        /// <returns>The updated contact.</returns>
        Task<Contact> UpdateContactAsync(Contact contact);

        /// <summary>
        /// Deletes an existing contact.
        /// </summary>
        /// <returns>Removes the contact.</returns>
        Task<Contact> DeleteContactAsync(Contact contact);
    }
}
