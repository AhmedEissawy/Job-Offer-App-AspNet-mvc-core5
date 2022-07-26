using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplicationDemo.Data;
using WebApplicationDemo.Models;
using WebApplicationDemo.ViewModels;

namespace WebApplicationDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var result = _context.Categories.Include(c=>c.Jobs).ToList();

            return View(result);
        }

        public IActionResult Details(int? id)
        {
            if (id==null)
            {
                return BadRequest();
            }
            var job = _context.Job.Include(j=>j.Category).FirstOrDefault(j=>j.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            TempData["id"] = id;

            return View(job);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Apply(string Message)
        {
            var userId = _userManager.GetUserId(User);

            var jobId = Convert.ToInt32(TempData["id"]);

            var check = _context.ApplyForJobs.Where(a => a.JobId == jobId && a.UserId == userId).ToList();

            if (check.Count<1)
            {
                ApplyForJob applyForJob = new ApplyForJob()
                {
                    UserId = userId,
                    JobId = jobId,
                    ApplyDate = DateTime.Now,
                    Message = Message,
                };
                _context.ApplyForJobs.Add(applyForJob);
                _context.SaveChanges();

                ViewBag.result = "Thanks For Applying Succeessfully !";
            }
            else
            {
                ViewBag.result = "You had been applied for that job before !";
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetJobsByUser()
        {
            var userId = _userManager.GetUserId(User);

            var jobs = _context.ApplyForJobs.Include(a=>a.Job).Where(a => a.UserId == userId).ToList();

            return View(jobs);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AppliedJobDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var appliedJob = _context.ApplyForJobs.Include(a=>a.Job).FirstOrDefault(a=>a.Id == id);

            if (appliedJob == null)
            {
                return NotFound();
            }

            return View(appliedJob);
        }
        [HttpGet]
        public IActionResult EditAppliedJob(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var appliedJob = _context.ApplyForJobs.Include(a => a.Job).FirstOrDefault(a => a.Id == id);

            if (appliedJob == null)
            {
                return NotFound();
            }

            return View(appliedJob);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAppliedJob(int id , ApplyForJob applyJob)
        {
            if (id != applyJob.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var jobInDb = _context.ApplyForJobs.Find(id);

                if (jobInDb == null)
                {
                    return NotFound();
                }

                jobInDb.Message = applyJob.Message;

                jobInDb.ApplyDate = DateTime.Now;

                _context.ApplyForJobs.Update(jobInDb);

                _context.SaveChanges();

                return RedirectToAction("GetJobsByUser");
            }


            return View(applyJob);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var job = _context.ApplyForJobs.Include(j=>j.Job).FirstOrDefault(j=>j.Id==id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id,ApplyForJob applyForJob)
        {
            if (id != applyForJob.Id)
            {
                return NotFound();
            }

            var job = _context.ApplyForJobs.Find(id);

            if (job == null)
            {
                return NotFound();
            }

            _context.ApplyForJobs.Remove(job);

            _context.SaveChanges();

            return RedirectToAction("GetJobsByUser");
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetJobsByPublisher()
        {
            var userId = _userManager.GetUserId(User);

            var publishedJobs = from app in _context.ApplyForJobs
                                join job in _context.Job
                                on app.JobId equals job.Id
                                where job.UserId == userId
                                select app;
                                


            var grouped = from j in publishedJobs
                          group j by j.Job.Title
                          into gr
                          select new JobViewModel
                          {
                              JobTitle = gr.Key,
                              items = gr
                          };


            

            return View(grouped.ToList());
        }
        [Authorize]
        [HttpGet]
        public IActionResult AppliedJobsBySearcher()
        {
            var userId = _userManager.GetUserId(User);

            var appliedJobs = _context.ApplyForJobs.Include(u=>u.Job).Where(u => u.UserId == userId).ToList();

            return View(appliedJobs);
        }

        [HttpPost]
        public IActionResult Search(string search)
        {
            var result = _context.Job.Where(j => j.Title.Contains(search) ||
                                           j.Content.Contains(search) ||
                                           j.Category.Name.Contains(search) ||
                                           j.Category.Description.Contains(search)).ToList();

            return View(result);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(Contact contact)
        {
            var mail = new MailMessage();

            var loginInfo = new NetworkCredential("ahmedeissawy2020@gmail.com","AhmedEissawy4417");

            mail.From = new MailAddress(contact.Email);

            mail.To.Add(new MailAddress("ahmedeissawy2020@gmail.com"));

            mail.Subject = contact.Subject;

            mail.IsBodyHtml = true;

            string body = "UserName:" + contact.Name + "<br>" +
                          "Email:" + contact.Email + "<br>" +
                          "Subject:" + contact.Subject + "<br>" +
                          "Message:" + contact.Message + "<br>";

            mail.Body = body;

            var smtpClient = new SmtpClient("smtp.gmail.com", 587);

            smtpClient.EnableSsl = true;

            smtpClient.Credentials = loginInfo;

            smtpClient.Send(mail);

            return RedirectToAction("Index");

            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
