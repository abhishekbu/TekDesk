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
                .Include(s => s.Artifact)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (solution == null)
            {
                return NotFound();
            }

            //ViewData["ArtifactName"] = solution.Artifact.Name;

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
                }
                else
                {
                    return RedirectToAction("Index", "Queries");
                }

                _context.Solutions.Add(solution);
                await _context.SaveChangesAsync();

                if (file != null)
                {
                    var dir = _env.ContentRootPath + "\\FileUploads";

                    using (var filestream = new FileStream(Path.Combine(dir, file.FileName), FileMode.Create, FileAccess.Write))
                    {
                        await file.CopyToAsync(filestream);
                    }

                    //var solID = _context.Solutions
                    //    .Where(sol => (sol.EmployeeID == employeeID) && (sol.QueryID == (int)TempData["queryID"]))
                    //    .OrderByDescending(sol => sol.Added).ToList().FirstOrDefault();

                    var artifact = new Artifact();
                    artifact.Name = file.FileName;
                    artifact.file = Path.Combine(dir, file.FileName);
                    artifact.SolutionID = solution.ID;
                    artifact.Type = file.FileName.Split(".").Last();

                    _context.Artifacts.Add(artifact);
                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index), new { queryID = (int)TempData["queryID"] });
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

            var solution = await _context.Solutions.Include(s => s.Artifact).Where(s => s.ID == id).FirstOrDefaultAsync();

            var employeeId = HttpContext.Session.GetString("EmployeeId");

            if ((employeeId == null) || (int.Parse(employeeId) != solution.EmployeeID))
            {
                return RedirectToAction("Index", "Home");
            }

            if (solution == null)
            {
                return NotFound();
            }

            TempData["QueryID"] = solution.QueryID;

            return View(solution);
        }

        // POST: Solutions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Solution solution, IFormFile file)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    solution.Added = DateTime.Now;
                    if (TempData.ContainsKey("QueryID"))
                        solution.QueryID = (int)TempData["QueryID"];
                    else
                        return NotFound();
                    solution.EmployeeID = int.Parse(HttpContext.Session.GetString("EmployeeId"));
                    _context.Update(solution);

                    if (file != null)
                    {
                        var currentArtifact = await _context.Artifacts.Where(a => a.SolutionID == solution.ID).FirstOrDefaultAsync();

                        if (currentArtifact != null)
                        {
                            _context.Artifacts.Remove(currentArtifact);
                        }

                        var dir = _env.ContentRootPath + "\\FileUploads";

                        using (var filestream = new FileStream(Path.Combine(dir, file.FileName), FileMode.Create, FileAccess.Write))
                        {
                            await file.CopyToAsync(filestream);
                        }

                        var artifact = new Artifact
                        {
                            Name = file.FileName,
                            file = Path.Combine(dir, file.FileName),
                            SolutionID = solution.ID,
                            Type = file.FileName.Split(".").Last()
                        };
                        _context.Artifacts.Add(artifact);
                    }
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
                return RedirectToAction(nameof(Index), new { queryID = solution.QueryID });
            }

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

            var employeeId = HttpContext.Session.GetString("EmployeeId");

            if ((employeeId == null) || (int.Parse(employeeId) != solution.EmployeeID))
            {
                return RedirectToAction("Index", "Home");
            }

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


            var artifact = _context.Artifacts.Where(a => a.SolutionID == solution.ID).FirstOrDefault();

            if (artifact != null)
            {
                var fileName = artifact.file;

                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
            }

            _context.Solutions.Remove(solution);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Solutions", new { queryID = solution.QueryID });
        }

        private bool SolutionExists(int id)
        {
            return _context.Solutions.Any(e => e.ID == id);
        }
    }
}
