using System.ComponentModel.DataAnnotations;

namespace weatherapi.Models.Account
{
    public class UserDto
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
