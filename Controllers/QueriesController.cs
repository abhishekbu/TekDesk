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

        private IQueryable<Query> sorting(string sortOrder, IQueryable<Query> queries)
        {

            switch (sortOrder)
            {
                case "sort_pending":
                    queries = queries.OrderBy(q => (int)q.QState);
                    break;
                case "Date":
                    queries = queries.OrderBy(q => q.Added);
                    break;
                case "date_desc":
                    queries = queries.OrderByDescending(q => q.Added);
                    break;
                default:
                    queries = queries.OrderByDescending(q => (int)q.QState);
                    break;
            }

            return queries;
        }

        private IQueryable<Query> queryFilter(IQueryable<Query> queries, States? state)
        {
            queries = queries.Where(q => q.QState == state);
            return queries;
        }

        private IQueryable<Query> typesFilter(IQueryable<Query> queries, Expertise? type)
        {
            queries = queries.Where(q => q.Tag == type);
            return queries;
        }

        // GET: Queries
        public async Task<IActionResult> Index(States? stateFilter, 
            Expertise? typeFilter, 
            string sortOrder,
            string currentFilter,
            string searchTerm,
            int? pageNumber)
        {
            ViewData["StateSortParam"] = String.IsNullOrEmpty(sortOrder) ? "sort_pending" : "";
            ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["SearchTerm"] = searchTerm;
            ViewData["CurrentSort"] = sortOrder;

            if (searchTerm != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchTerm = currentFilter;
            }

            ViewData["CurrentFilter"] = searchTerm;

            var queries = _context.Queries.Include(q => q.Employee).Select(q => q);
             
            if (!String.IsNullOrEmpty(searchTerm))
            {
                queries = queries.Where(q => q.Employee.FName.Contains(searchTerm) 
                    || q.Employee.LName.Contains(searchTerm));
            }

            if (stateFilter != null)
            {
                queries = queryFilter(queries, stateFilter);
            }

            if (typeFilter != null)
            {
                queries = typesFilter(queries, typeFilter);
            }

            var Allqueries = sorting(sortOrder, queries);


            TempData["Title"] = "All Queries";

            int pageSize = 3;
            return View(await PaginatedList<Query>.CreateAsync(Allqueries.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> MyQueries(States? stateFilter, 
            Expertise? typeFilter, 
            string sortOrder, 
            int? pageNumber)
        {
            ViewData["StateSortParam"] = String.IsNullOrEmpty(sortOrder) ? "sort_pending" : "";
            ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentSort"] = sortOrder;

            var employeeId = HttpContext.Session.GetString("EmployeeId");

            if (employeeId != null)
            {
                var queries = _context.Queries.Include(q => q.Employee).Where(q => q.EmployeeID == int.Parse(employeeId));

                if (stateFilter != null)
                {
                    queries = queryFilter(queries, stateFilter);
                }

                if (typeFilter != null)
                {
                    queries = typesFilter(queries, typeFilter);
                }

                var myQueries = sorting(sortOrder, queries);

                TempData["Title"] = "My Queries";

                int pageSize = 3;
                return View("Index" ,await PaginatedList<Query>.CreateAsync(myQueries.AsNoTracking(), pageNumber ?? 1, pageSize));
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
            if (HttpContext.Session.GetString("EmployeeId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName");
            return View();
        }

        // POST: Queries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Query query)
        {
            if (ModelState.IsValid)
            {
                var employeeId = int.Parse(HttpContext.Session.GetString("EmployeeId"));
                query.Added = DateTime.Now;
                query.EmployeeID = employeeId;
                query.QState = States.pending;

                _context.Add(query);
                await _context.SaveChangesAsync();

                var employeeSubjects = _context.EmployeeSubjects
                    .Include(es => es.Subject)
                    .Include(es => es.Employee);

                var employees = employeeSubjects.Where(es => (int)es.Subject.Name == (int)query.Tag).ToList();

                foreach (var e in employees)
                {
                    if (e.EmployeeID != employeeId)
                        _context.EmployeeNotifications
                            .Add(new EmployeeNotification
                            {
                                EmployeeID = e.EmployeeID,
                                QueryID = query.QueryID,
                                Notification = query.Description
                            });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyQueries));
            }
            //ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName", query.EmployeeID);
            return View(query);
        }

        // GET: Queries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var employeeId = HttpContext.Session.GetString("EmployeeId");

                var query = await _context.Queries.FindAsync(id);

                if (query == null)
                {
                    return NotFound();
                }

                if (query.EmployeeID != int.Parse(employeeId) || employeeId == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FName", query.EmployeeID);
                return View(query);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Queries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Query query)
        {
            query.EmployeeID = int.Parse(HttpContext.Session.GetString("EmployeeId"));
            query.Added = DateTime.Now;
            query.QueryID = id;

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
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employeeID = HttpContext.Session.GetString("EmployeeId");


                var query = await _context.Queries
                    .Include(q => q.Employee)
                    .FirstOrDefaultAsync(m => m.QueryID == id);
                if (query == null)
                {
                    return NotFound();
                }

                if (employeeID == null || int.Parse(employeeID) != query.EmployeeID)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(query);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Queries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var query = await _context.Queries.FindAsync(id);
            var solutions = _context.Solutions.Include(s => s.Artifact).Where(s => s.QueryID == id).ToList();
            var notifications = _context.EmployeeNotifications.Where(n => n.QueryID == id).ToList();

            foreach (var n in notifications)
            {
                _context.EmployeeNotifications.Remove(n);
            }

            foreach (var s in solutions)
            {
                _context.Solutions.Remove(s);
            }

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
