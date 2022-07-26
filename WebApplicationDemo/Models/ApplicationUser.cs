using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationDemo.Models
{
    public class ApplicationUser:IdentityUser
    {
        public ApplicationUser()
        {
            Jobs = new HashSet<Job>();
        }

        public string UserType { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
