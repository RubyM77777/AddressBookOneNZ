using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AddressBookOneNZ.Models
{
    /// <summary>
    /// // Properties of a Group in the address book. List of contacts added in this group
    /// </summary>
    public class Group
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [Required(ErrorMessage = "Group Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Group Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Group Name can only contain letters, digits, and spaces.")]
        public required string Name { get; set; }

        [JsonIgnore]
        public List<Contact> Contacts { get; set; } = new();
    }
}
