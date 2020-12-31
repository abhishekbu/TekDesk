using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TekDesk.Data;
using TekDesk.Models;

namespace TekDesk.Controllers
{
    public class QueriesController : Controller
    {
        private readonly TekDeskContext _context;

        public QueriesController(TekDeskContext context)
        {
            _context = context;
        }

        // GET: Queries
        public async Task<IActionResult> Index()
        {
            var queries = _context.Queries.Include(q => q.Employee);
           
            return View(await queries.ToListAsync());
        }

        public async Task<IActionResult> MyQueries()
        {
            var employeeId = HttpContext.Session.GetString("EmployeeId");
            if (employeeId != null)
            {
                var queries = _context.Queries.Include(q => q.Employee).Where(q => q.Employee.ID == int.Parse(employeeId)).OrderByDescending(q => q.Added);
                return View("Index", await queries.ToListAsync());
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Queries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Queries
                .Include(q => q.Employee)
                .FirstOrDefaultAsync(m => m.QueryID == id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // GET: Queries/Create
        public IActionResult Create()
        {
            var employeeId = HttpContext.Session.GetString("EmployeeId");
            if (employeeId != null)
            {
                ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName");
                return View();
            }
            return RedirectToAction("Index", "Home");

        }

        // POST: Queries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,QState,Added,Tag")] Query query)
        {
            if (ModelState.IsValid)
            {
                query.Added = DateTime.Now;
                query.EmployeeID = int.Parse(HttpContext.Session.GetString("EmployeeId"));
                query.QState = States.pending;
                _context.Add(query);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyQueries));
            }
            //ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName", query.EmployeeID);
            return View(query);
        }

        // GET: Queries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Queries.FindAsync(id);
            if (query == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName", query.EmployeeID);
            return View(query);
        }

        // POST: Queries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QueryID,Description,QState,Added,EmployeeID,Tag")] Query query)
        {
            if (id != query.QueryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(query);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QueryExists(query.QueryID))
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
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName", query.EmployeeID);
            return View(query);
        }

        // GET: Queries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Queries
                .Include(q => q.Employee)
                .FirstOrDefaultAsync(m => m.QueryID == id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // POST: Queries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var query = await _context.Queries.FindAsync(id);
            _context.Queries.Remove(query);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QueryExists(int id)
        {
            return _context.Queries.Any(e => e.QueryID == id);
        }
    }
}
