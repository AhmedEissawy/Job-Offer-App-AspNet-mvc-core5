using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationDemo.Data;
using WebApplicationDemo.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApplicationDemo.Controllers
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
    
        private readonly UserManager<ApplicationUser> _userManager;

        private  IHostingEnvironment _host;

        public JobsController(ApplicationDbContext context,IHostingEnvironment host,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _host = host;
            _userManager = userManager;
        }

        // GET: Jobs
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Job.Include(j => j.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Jobs/Details/5
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.Include(j => j.Category).FirstOrDefaultAsync(m => m.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {

            ViewBag.result = _context.Categories.ToList();

            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Job job)
        {
            if (ModelState.IsValid)
            {
                 UploadPhoto(job);
                 job.UserId = _userManager.GetUserId(User);
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.result = _context.Categories.ToList();

            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.Include(j=>j.Category).FirstOrDefaultAsync(j=>j.Id==id);

            if (job == null)
            {
                return NotFound();
            }

            ViewBag.result = _context.Categories.ToList();

            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }
          
            if (ModelState.IsValid)
            {
                try
                {
                    string oldPath = job.Image;
                    if (job.File != null)
                    {
                        System.IO.File.Delete(oldPath);
                        UploadPhoto(job);
                    } 
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.result = _context.Categories.ToList();

            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Job.FindAsync(id);
            _context.Job.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return _context.Job.Any(e => e.Id == id);
        }

        void UploadPhoto(Job model)
        {
            if (model.File != null)
            {
                string uploadFolder = Path.Combine(_host.WebRootPath, "Images/Jobs");
                string uniqueFileName = Guid.NewGuid() + ".jpg";
                string filePath = Path.Combine(uploadFolder,uniqueFileName);
                using (var fileStream = new FileStream(filePath,FileMode.Create))
                {
                    model.File.CopyTo(fileStream);
                }
                model.Image = uniqueFileName;
            }
        }
    }
}
