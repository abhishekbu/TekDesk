using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TekDesk.Data;
using TekDesk.Models;

namespace TekDesk.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly TekDeskContext _context;
        private readonly IWebHostEnvironment _env;

        public SolutionsController(TekDeskContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Solutions
        public async Task<IActionResult> Index(int? queryID)
        {
            var solutions = _context.Solutions.Include(s => s.Employee).Include(s => s.Query);
            if (queryID != null)
            {
                return View(await solutions.Where(s => s.QueryID == queryID).ToListAsync());
            }
            
            return View(await solutions.ToListAsync());
        }

        // GET: Solutions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _context.Solutions
                .Include(s => s.Employee)
                .Include(s => s.Query)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (solution == null)
            {
                return NotFound();
            }
            
            return View(solution);
        }

        // GET: Solutions/Create
        public IActionResult Create(int? queryID)
        {
            //ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName");
            //ViewData["QueryID"] = new SelectList(_context.Queries, "QueryID", "QueryID");

            if (HttpContext.Session.GetString("EmployeeId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (queryID == null)
            {
                return RedirectToAction("Index", "Queries");
            }

            var query = _context.Queries
                .Include(q => q.Employee)
                .Where(q => q.QueryID == queryID)
                .SingleOrDefault();

            if (query == null)
            {
                return RedirectToAction("Index", "Queries");
            }

            TempData["queryID"] = queryID;
            ViewData["EmployeeName"] = query.Employee.FullName;
            ViewData["queryDesc"] = query.Description;
            return View();
        }

        // POST: Solutions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Solution solution, IFormFile file)
        {
            if (ModelState.IsValid)
            {

                solution.Added = DateTime.Now;

                var employeeID = int.Parse(HttpContext.Session.GetString("EmployeeId"));

                solution.EmployeeID = employeeID;

                if (TempData.ContainsKey("queryID"))
                {
                    solution.QueryID = (int)TempData["queryID"];
                    TempData.Remove("queryID");
                }
                else
                {
                    return RedirectToAction("Index", "Queries");
                }

                if (file != null)
                {
                    var dir = _env.ContentRootPath + "\\FileUploads";

                    using (var filestream = new FileStream(Path.Combine(dir, file.FileName), FileMode.Create, FileAccess.Write))
                    {
                        await file.CopyToAsync(filestream);
                    }

                    var artifact = new Artifact();
                    artifact.Name = file.FileName;
                    artifact.file = Path.Combine(dir, file.FileName);
                    artifact.SolutionID = solution.ID;
                    artifact.Type = file.FileName.Split(".").Last();

                    _context.Artifacts.Add(artifact);
                }

                _context.Solutions.Add(solution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName", solution.EmployeeID);
            ViewData["QueryID"] = new SelectList(_context.Queries, "QueryID", "QueryID", solution.QueryID);
            return View(solution);
        }

        // GET: Solutions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _context.Solutions.FindAsync(id);
            if (solution == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName", solution.EmployeeID);
            ViewData["QueryID"] = new SelectList(_context.Queries, "QueryID", "QueryID", solution.QueryID);
            return View(solution);
        }

        // POST: Solutions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Description,Added,EmployeeID,QueryID")] Solution solution)
        {
            if (id != solution.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolutionExists(solution.ID))
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
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName", solution.EmployeeID);
            ViewData["QueryID"] = new SelectList(_context.Queries, "QueryID", "QueryID", solution.QueryID);
            return View(solution);
        }

        // GET: Solutions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _context.Solutions
                .Include(s => s.Employee)
                .Include(s => s.Query)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (solution == null)
            {
                return NotFound();
            }

            return View(solution);
        }

        // POST: Solutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solution = await _context.Solutions.FindAsync(id);
            _context.Solutions.Remove(solution);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SolutionExists(int id)
        {
            return _context.Solutions.Any(e => e.ID == id);
        }
    }
}
