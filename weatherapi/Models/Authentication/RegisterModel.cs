using System.ComponentModel.DataAnnotations;

namespace weatherapi.Models.Authentication
{
    public class RegisterModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and comfirm password do not match")]
        public string ComfirmPassword { get; set; }
    }
}
