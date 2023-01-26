using weatherapi.Entities.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherapi.Entities
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public DateTime Created { get; set; } = DateTime.Now;

        [StringLength(maximumLength: 30)]
        public string FirstName { get; set; }

        [StringLength(maximumLength: 30)]
        public string LastName { get; set; }

        public Gender Gender { get; set; }
    }
}
