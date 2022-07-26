using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplicationDemo.Models
{
    public class Job
    {
        public int Id { get; set; }

        public string Title { get; set; }

        
        public string Content { get; set; }

        public string Image { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
