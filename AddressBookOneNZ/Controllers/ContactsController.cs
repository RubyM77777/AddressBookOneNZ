using AddressBookOneNZ.Models;
using AddressBookOneNZ.Services;
using AddressBookOneNZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookOneNZ.Controllers
{
    /// <summary>
    /// // ContactsController for managing Contacts in the address book.
    /// // IEnumerable-ReadOnlyList-Abstraction-Async Await-Pagination--Clean Architecture
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // GET api/Contacts?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAllContacts(int pageNumber = 1, int pageSize = 10)
        {
            var allContacts = await _contactService.GetAllContactsAsync(pageNumber, pageSize);
            return Ok(allContacts);
        }

        // GET api/contacts/{firstName}/{lastName}
        [HttpGet("{firstName}/{lastName}")]
        public async Task<ActionResult<Contact>> GetContactByName(string firstName, string lastName)
        {
            var contact = await _contactService.GetContactByNameAsync(firstName, lastName);
            return Ok(contact);
        }

        // POST api/Contacts
        [HttpPost]
        public async Task<ActionResult<Contact>> AddNewContact([FromBody] Contact newContact)
        {
            var addedContact = await _contactService.AddContactAsync(newContact);
            return CreatedAtAction(nameof(GetContactByName), 
                new { firstname = addedContact.FirstName,
                      lastname = addedContact.LastName,
                      phonenumber = addedContact.PhoneNumber, 
                      email = addedContact.Email}, addedContact);
        }

        // PUT api/Contacts/{name}
        [HttpPut]
        public async Task<ActionResult<Contact>> UpdateExistingContact([FromBody] Contact updateContact)
        {
            var updatedContact = await _contactService.UpdateContactAsync(updateContact);
            return Ok(updatedContact);
        }

        // DELETE api/Contacts/{name}
        [HttpDelete]
        public async Task<ActionResult<Contact>> DeleteContact([FromBody] Contact contact)
        {
            var deletedContact = await _contactService.DeleteContactAsync(contact);
            return Ok(deletedContact);
        }
    }
}
