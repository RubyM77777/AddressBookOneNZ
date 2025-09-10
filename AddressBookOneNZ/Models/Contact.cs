using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AddressBookOneNZ.Models
{
    /// <summary>
    /// // Properties of a Contact in the address book. List of groups this contact belongs to
    /// </summary>
    public class Contact
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters.")]
        public required string LastName { get; set; }

        [Phone]
        [Required(ErrorMessage = "Phone Number is required.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone Number must be between 10 and 15 characters.")]
        [RegularExpression(@"^[0-9\s\-\(\)]+$", ErrorMessage = "Phone Number can only contain digits, spaces, dashes, and parentheses.")]
        public required string PhoneNumber { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(254, ErrorMessage = "Email must not exceed 254 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not in a valid format.")]
        public required string Email { get; set; }

        [JsonIgnore]
        public List<Group> Groups { get; set; } = new();

        public List<string> GroupNames { get; set; } = new List<string>();
    }
}
