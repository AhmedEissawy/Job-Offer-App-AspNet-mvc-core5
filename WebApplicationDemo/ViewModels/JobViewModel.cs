using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationDemo.Models;

namespace WebApplicationDemo.ViewModels
{
    public class JobViewModel
    {
        public string JobTitle { get; set; }

        public IEnumerable<ApplyForJob> items { get; set; }
    }
}
