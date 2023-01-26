using weatherapi.Entities.Enum;
using System.ComponentModel.DataAnnotations;

namespace weatherapi.Models.Account
{
    public class UpdateUserDto
    {
        [StringLength(maximumLength: 30)]
        public string FirstName { get; set; }

        [StringLength(maximumLength: 30)]
        public string LastName { get; set; }

        public Gender Gender { get; set; }
    }
}
