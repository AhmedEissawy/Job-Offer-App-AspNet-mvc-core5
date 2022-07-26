using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplicationDemo.Models;
using WebApplicationDemo.ViewModel;
namespace WebApplicationDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Job> Job { get; set; }

        public DbSet<ApplyForJob> ApplyForJobs { get; set; }

        public DbSet<Contact> Contacts { get; set; }

    }
}
 