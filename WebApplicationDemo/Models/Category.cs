using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationDemo.Models
{
    public class Category
    {

        public Category()
        {
            Jobs = new HashSet<Job>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name="Category Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category Description")]
        public string Description { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
