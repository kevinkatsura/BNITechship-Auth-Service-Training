using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlphaAuthService.Models
{
    public class IdentityModel : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
