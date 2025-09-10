using AddressBookOneNZ.Controllers;
using AddressBookOneNZ.Models;
using AddressBookOneNZ.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AddressBookOneNZ.UnitTests
{
    public class AddressBookOneNZTests
    {
        private readonly Mock<IGroupService> _mockGroupService;
        private readonly Mock<IContactService> _mockContactService;
        private readonly ContactsController _mockContactsController;
        private readonly GroupsController _mockGroupsController;

        public AddressBookOneNZTests()
        {
            _mockGroupService = new Mock<IGroupService>();
            _mockGroupsController = new GroupsController(_mockGroupService.Object);
            _mockContactService = new Mock<IContactService>();
            _mockContactsController = new ContactsController(_mockContactService.Object);
        }

        #region GroupsController Tests
        [Fact]
        public async Task GetAllGroups_ReturnsOkResult_WithGroups()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { Name = "Family" },
                new Group { Name = "Friends" }
            };
            _mockGroupService.Setup(service => service.GetAllGroupsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(groups);
            // Act
            var result = await _mockGroupsController.GetAllGroups();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnGroups = Assert.IsAssignableFrom<IEnumerable<Group>>(okResult.Value);
            Assert.Equal(2, returnGroups.Count());
        }

        [Fact]
        public async Task GetGroupByName_ReturnsOkResult_WithGroup()
        {
            // Arrange
            var groupName = "Family";
            var group = new Group { Name = groupName };
            _mockGroupService.Setup(service => service.GetGroupByNameAsync(groupName))
                .ReturnsAsync(group);
            // Act
            var result = await _mockGroupsController.GetGroupByName(groupName);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnGroup = Assert.IsType<Group>(okResult.Value);
            Assert.Equal(groupName, returnGroup.Name);
        }

        [Fact]
        public async Task AddNewGroup_ReturnsCreatedAtAction_WithGroup()
        {
            // Arrange
            var newGroup = new Group { Name = "Work" };
            _mockGroupService.Setup(service => service.AddGroupAsync(newGroup))
                .ReturnsAsync(newGroup);
            // Act
            var result = await _mockGroupsController.AddNewGroup(newGroup);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnGroup = Assert.IsType<Group>(createdResult.Value);
            Assert.Equal(newGroup.Name, returnGroup.Name);
        }

        [Fact]
        public async Task UpdateExistingGroup_ReturnsOkResult_WithUpdatedGroup()
        {
            // Arrange
            var groupName = "Family";
            var updatedGroup = new Group { Name = "Close Family" };
            _mockGroupService.Setup(service => service.UpdateGroupAsync(groupName, updatedGroup))
                .ReturnsAsync(updatedGroup);
            // Act
            var result = await _mockGroupsController.UpdateExistingGroup(groupName, updatedGroup);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnGroup = Assert.IsType<Group>(okResult.Value);
            Assert.Equal(updatedGroup.Name, returnGroup.Name);
        }

        [Fact]
        public async Task DeleteGroup_ReturnsOkResult_WithDeletedGroup()
        {
            // Arrange
            var groupName = "Friends";
            var deletedGroup = new Group { Name = groupName };
            _mockGroupService.Setup(service => service.DeleteGroupAsync(groupName))
                .ReturnsAsync(deletedGroup);
            // Act
            var result = await _mockGroupsController.DeleteGroup(groupName);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnGroup = Assert.IsType<Group>(okResult.Value);
            Assert.Equal(deletedGroup.Name, returnGroup.Name);
        }

        [Theory]
        [InlineData("Family", true)]
        [InlineData("UnknownGroup", false)]
        public async Task GetGroupByName_ReturnsExpectedResult(string groupName, bool exists)
        {
            // Arrange
            Group? group = exists ? new Group { Name = groupName } : null;
            _mockGroupService.Setup(service => service.GetGroupByNameAsync(groupName))
                .ReturnsAsync(group);

            // Act
            var result = await _mockGroupsController.GetGroupByName(groupName);

            // Assert
            if (exists)
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnGroup = Assert.IsType<Group>(okResult.Value);
                Assert.Equal(groupName, returnGroup.Name);
            }
            else
            {
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("Friends", true)]
        [InlineData("NonExistent", false)]
        public async Task DeleteGroup_ReturnsExpectedResult(string groupName, bool exists)
        {
            // Arrange
            Group? group = exists ? new Group { Name = groupName } : null;
            _mockGroupService.Setup(service => service.DeleteGroupAsync(groupName))
                .ReturnsAsync(group);

            // Act
            var result = await _mockGroupsController.DeleteGroup(groupName);

            // Assert
            if (exists)
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnGroup = Assert.IsType<Group>(okResult.Value);
                Assert.Equal(groupName, returnGroup.Name);
            }
            else
            {
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("Work", true)]
        public async Task AddNewGroup_ReturnsExpectedResult(string groupName, bool exists)
        {
            // Arrange
            Group? group = exists ? new Group { Name = groupName } : null;
            _mockGroupService.Setup(service => service.AddGroupAsync(group))
                .ReturnsAsync(group);
            // Act
            var result = await _mockGroupsController.AddNewGroup(group);
            // Assert
            if (exists)
            {
                var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var returnGroup = Assert.IsType<Group>(createdResult.Value);
                Assert.Equal(groupName, returnGroup.Name);
            }
            else
            {
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("Family", "Close Family", true)]
        [InlineData("NonExistentGroup", "Updated Group", false)]
        public async Task UpdateExistingGroup_ReturnsExpectedResult(string currentName, string newName, bool exists)
        {
            // Arrange
            Group? group = exists ? new Group { Name = newName } : null;
            _mockGroupService.Setup(service => service.UpdateGroupAsync(currentName, It.IsAny<Group>()))
                .ReturnsAsync(group);
            // Act
            var result = await _mockGroupsController.UpdateExistingGroup(currentName, new Group { Name = newName });
            // Assert
            if (exists)
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnGroup = Assert.IsType<Group>(okResult.Value);
                Assert.Equal(newName, returnGroup.Name);
            }
            else
            {
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("Family", true)]
        [InlineData("NonExistentGroup", false)]
        public async Task GetAllGroups_ReturnsExpectedResult(string groupName, bool exists)
        {
            // Arrange
            IEnumerable<Group> groups = exists ? new List<Group> { new Group { Name = groupName } } : new List<Group>();
            _mockGroupService.Setup(service => service.GetAllGroupsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(groups);
            // Act
            var result = await _mockGroupsController.GetAllGroups();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnGroups = Assert.IsAssignableFrom<IEnumerable<Group>>(okResult.Value);
            if (exists)
            {
                Assert.Single(returnGroups);
                Assert.Equal(groupName, returnGroups.First().Name);
            }
            else
            {
                Assert.Empty(returnGroups);
            }
        }

        [Theory]
        [InlineData("Family", "Friends", true)]
        [InlineData("NonExistentGroup", "AnotherGroup", false)]
        public async Task GetAllGroups_ReturnsExpectedResultWithMultipleGroups(string groupName1, string groupName2, bool exists)
        {
            // Arrange
            var groups = exists ? new List<Group>
            {
                new Group { Name = groupName1 },
                new Group { Name = groupName2 }
            } : new List<Group>();
            _mockGroupService.Setup(service => service.GetAllGroupsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(groups);
            // Act
            var result = await _mockGroupsController.GetAllGroups();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnGroups = Assert.IsAssignableFrom<IEnumerable<Group>>(okResult.Value);
            Assert.Equal(exists ? 2 : 0, returnGroups.Count());
        }

        #endregion

        #region ContactsController Tests
        [Fact]
        public async Task GetAllContacts_ReturnsOkResult_WithContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { FirstName = "John", LastName = "Doe", PhoneNumber = "1234567890", Email = "john@example.com" },
                new Contact { FirstName = "Jane", LastName = "Smith", PhoneNumber = "0987654321", Email = "jane@example.com" }
            };
            _mockContactService.Setup(service => service.GetAllContactsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(contacts);

            // Act
            var result = await _mockContactsController.GetAllContacts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnContacts = Assert.IsAssignableFrom<IEnumerable<Contact>>(okResult.Value);
            Assert.Equal(2, returnContacts.Count());
        }

        [Fact]
        public async Task GetContactByName_ReturnsOkResult_WithContact()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var contact = new Contact { FirstName = firstName, LastName = lastName, PhoneNumber = "1234567890", Email = "john@gmail.com" };
            _mockContactService.Setup(service => service.GetContactByNameAsync(firstName, lastName))
                .ReturnsAsync(contact);

            // Act
            var result = await _mockContactsController.GetContactByName(firstName, lastName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnContact = Assert.IsType<Contact>(okResult.Value);
            Assert.Equal(firstName, returnContact.FirstName);
            Assert.Equal(lastName, returnContact.LastName);
            Assert.Equal("1234567890", returnContact.PhoneNumber);
            Assert.Equal("john@gmail.com", returnContact.Email);
        }

        [Fact]
        public async Task AddNewContact_ReturnsCreatedAtAction_WithContact()
        {
            // Arrange
            var newContact = new Contact { FirstName = "Alice", LastName = "Johnson", PhoneNumber = "5551234567", Email = "john@gmail.com" };
            _mockContactService.Setup(service => service.AddContactAsync(newContact))
                .ReturnsAsync(newContact);

            // Act
            var result = await _mockContactsController.AddNewContact(newContact);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnContact = Assert.IsType<Contact>(createdResult.Value);
            Assert.Equal(newContact.FirstName, returnContact.FirstName);
            Assert.Equal(newContact.LastName, returnContact.LastName);
            Assert.Equal(newContact.PhoneNumber, returnContact.PhoneNumber);
            Assert.Equal(newContact.Email, returnContact.Email);
        }

        [Fact]
        public async Task UpdateExistingContact_ReturnsOkResult_WithUpdatedContact()
        {
            // Arrange
            var updateContact = new Contact { FirstName = "Bob", LastName = "Brown", PhoneNumber = "5559876543", Email = "john@gmail.com" };
            _mockContactService.Setup(service => service.UpdateContactAsync(updateContact))
                .ReturnsAsync(updateContact);

            // Act
            var result = await _mockContactsController.UpdateExistingContact(updateContact);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnContact = Assert.IsType<Contact>(okResult.Value);
            Assert.Equal(updateContact.FirstName, returnContact.FirstName);
            Assert.Equal(updateContact.LastName, returnContact.LastName);
            Assert.Equal(updateContact.PhoneNumber, returnContact.PhoneNumber);
            Assert.Equal(updateContact.Email, returnContact.Email);
        }

        [Fact]
        public async Task DeleteContact_ReturnsOkResult_WithDeletedContact()
        {
            // Arrange
            var contact = new Contact { FirstName = "Charlie", LastName = "Davis", PhoneNumber = "5551112222", Email = "john@gmail.com" };
            _mockContactService.Setup(service => service.DeleteContactAsync(contact))
                .ReturnsAsync(contact);

            // Act
            var result = await _mockContactsController.DeleteContact(contact);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnContact = Assert.IsType<Contact>(okResult.Value);
            Assert.Equal(contact.FirstName, returnContact.FirstName);
            Assert.Equal(contact.LastName, returnContact.LastName);
            Assert.Equal(contact.PhoneNumber, returnContact.PhoneNumber);
            Assert.Equal(contact.Email, returnContact.Email);
        }

        // Fix for CS9035: Set required members 'PhoneNumber' and 'Email' in Contact object initializers

        [Theory]
        [InlineData("John", "Doe", true)]
        [InlineData("Unknown", "Person", false)]
        public async Task GetContactByName_ReturnsExpectedResult(string firstName, string lastName, bool exists)
        {
            // Arrange
            Contact? contact = exists
                ? new Contact
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = "0000000000",
                    Email = "test@example.com"
                }
                : null;
            _mockContactService.Setup(service => service.GetContactByNameAsync(firstName, lastName))
                .ReturnsAsync(contact);
            // Act
            var result = await _mockContactsController.GetContactByName(firstName, lastName);
            // Assert
            if (exists)
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnContact = Assert.IsType<Contact>(okResult.Value);
                Assert.Equal(firstName, returnContact.FirstName);
                Assert.Equal(lastName, returnContact.LastName);
            }
            else
            {
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("Alice", "Johnson", true)]
        [InlineData("NonExistent", "Person", false)]
        public async Task DeleteContact_ReturnsExpectedResult(string firstName, string lastName, bool exists)
        {
            // Arrange
            Contact? contact = exists
                ? new Contact
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = "0000000000",
                    Email = "test@example.com"
                }
                : null;
            _mockContactService.Setup(service => service.DeleteContactAsync(contact))
                .ReturnsAsync(contact);
            // Act
            var result = await _mockContactsController.DeleteContact(contact);
            // Assert
            if (exists)
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnContact = Assert.IsType<Contact>(okResult.Value);
                Assert.Equal(firstName, returnContact.FirstName);
                Assert.Equal(lastName, returnContact.LastName);
            }
            else
            {
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("Bob", "Brown", true)]
        public async Task AddNewContact_ReturnsExpectedResult(string firstName, string lastName, bool exists)
        {
            // Arrange
            Contact? contact = exists
                ? new Contact
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = "0000000000",
                    Email = "test@example.com"
                }
                : null;
            _mockContactService.Setup(service => service.AddContactAsync(contact))
                .ReturnsAsync(contact);

            // Act
            var result = await _mockContactsController.AddNewContact(contact);

            // Assert
            if (exists)
            {
                var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var returnContact = Assert.IsType<Contact>(createdResult.Value);
                Assert.Equal(firstName, returnContact.FirstName);
                Assert.Equal(lastName, returnContact.LastName);
            }
            else
            {
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("Charlie", "Davis", true)]
        [InlineData("NonExistent", "Person", false)]
        public async Task UpdateExistingContact_ReturnsExpectedResult(string firstName, string lastName, bool exists)
        {
            // Arrange
            Contact? contact = exists
                ? new Contact
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = "0000000000",
                    Email = "john@gmail.com"
                }
                : null;
            _mockContactService.Setup(service => service.UpdateContactAsync(contact))
                .ReturnsAsync(contact);

            // Act
            var result = await _mockContactsController.UpdateExistingContact(contact);

            // Assert
            if (exists)
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnContact = Assert.IsType<Contact>(okResult.Value);
                Assert.Equal(firstName, returnContact.FirstName);
                Assert.Equal(lastName, returnContact.LastName);
            }
            else
            {
                Assert.IsType<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("John", "Doe", true)]
        [InlineData("NonExistent", "Person", false)]
        public async Task GetAllContacts_ReturnsExpectedResult(string firstName, string lastName, bool exists)
        {
            // Arrange
            IEnumerable<Contact> contacts = exists
                ? new List<Contact>
                {
                    new Contact { FirstName = firstName, LastName = lastName, PhoneNumber = "0000000000", Email = "john@gmail.com" }
                }
                : new List<Contact>();
            _mockContactService.Setup(service => service.GetAllContactsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(contacts);

            // Act
            var result = await _mockContactsController.GetAllContacts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnContacts = Assert.IsAssignableFrom<IEnumerable<Contact>>(okResult.Value);
            if (exists)
            {
                Assert.Single(returnContacts);
                Assert.Equal(firstName, returnContacts.First().FirstName);
                Assert.Equal(lastName, returnContacts.First().LastName);
            }
            else
            {
                Assert.Empty(returnContacts);
            }
        }

        [Theory]

        [InlineData("John", "Doe", "Jane", "Smith", true)]
        [InlineData("NonExistent", "Person", "Another", "Person", false)]
        public async Task GetAllContacts_ReturnsExpectedResultWithMultipleContacts(string firstName1, string lastName1, string firstName2, string lastName2, bool exists)
        {
            // Arrange
            var contacts = exists ? new List<Contact>
            {
                new Contact { FirstName = firstName1, LastName = lastName1, PhoneNumber = "0000000000", Email = "john@gmail.com" },
                new Contact { FirstName = firstName2, LastName = lastName2, PhoneNumber = "1111111111", Email = "john@gmail.com" }
                } : new List<Contact>();
            _mockContactService.Setup(service => service.GetAllContactsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(contacts);

            // Act
            var result = await _mockContactsController.GetAllContacts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnContacts = Assert.IsAssignableFrom<IEnumerable<Contact>>(okResult.Value);
            if (exists)
            {
                Assert.Equal(2, returnContacts.Count());
                Assert.Contains(returnContacts, c => c.FirstName == firstName1 && c.LastName == lastName1);
                Assert.Contains(returnContacts, c => c.FirstName == firstName2 && c.LastName == lastName2);
            }
            else
            {
                Assert.Empty(returnContacts);
            }
        }

        [Fact]
        public async Task GetAllContacts_ReturnsEmptyList_WhenNoContactsExist()
        {
            // Arrange
            _mockContactService.Setup(service => service.GetAllContactsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Contact>());
            // Act
            var result = await _mockContactsController.GetAllContacts();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnContacts = Assert.IsAssignableFrom<IEnumerable<Contact>>(okResult.Value);
            Assert.Empty(returnContacts);
        }

        [Fact]
        public async Task GetContactByName_ReturnsNotFound_WhenContactDoesNotExist()
        {
            // Arrange
            _mockContactService.Setup(service => service.GetContactByNameAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Contact?)null);
            // Act
            var result = await _mockContactsController.GetContactByName("NonExistent", "Person");
            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateExistingContact_ReturnsNotFound_WhenContactDoesNotExist()
        {
            // Arrange
            var updateContact = new Contact { FirstName = "NonExistent", LastName = "Person", PhoneNumber = "0000000000", Email = "john@gmail.com" };
            _mockContactService.Setup(service => service.UpdateContactAsync(updateContact))
                .ReturnsAsync((Contact?)null);

            // Act
            var result = await _mockContactsController.UpdateExistingContact(updateContact);

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteContact_ReturnsNotFound_WhenContactDoesNotExist()
        {
            // Arrange
            var contact = new Contact { FirstName = "NonExistent", LastName = "Person", PhoneNumber = "0000000000", Email = "john@gmail.com" };
            _mockContactService.Setup(service => service.DeleteContactAsync(contact))
                .ReturnsAsync((Contact?)null);

            // Act
            var result = await _mockContactsController.DeleteContact(contact);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
        #endregion
    }
    }