using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherapi.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public DateTime? UpdatedAt { get; set; }
    }
}
