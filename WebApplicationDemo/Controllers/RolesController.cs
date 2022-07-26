using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationDemo.Data;

namespace WebApplicationDemo.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: RolesController
        public ActionResult Index()
        {
            var roles = _context.Roles.ToList();

            return View(roles);
        }

        // GET: RolesController/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var role = _context.Roles.FirstOrDefault(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: RolesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RolesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(role);
            }
        }

        // GET: RolesController/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var role = _context.Roles.Find(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: RolesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, IdentityRole role)
        {
            if (id == null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var roleInDb = _context.Roles.SingleOrDefault(r => r.Id == role.Id);

                if (roleInDb == null)
                {
                    return NotFound();
                }

                roleInDb.Name = role.Name;

                _context.Roles.Update(roleInDb);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View(role);
            }
        }

        // GET: RolesController/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var role = _context.Roles.Find(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: RolesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var role = _context.Roles.Find(id);

            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);

            _context.SaveChanges();

            return RedirectToAction("Index");
          
        }
    }
}
