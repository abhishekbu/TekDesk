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
using TekDesk.Models.TekDeskViewModels;

namespace TekDesk.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly TekDeskContext _context;

        public EmployeesController(TekDeskContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var viewModel = new EmployeeExpertiseData();
            viewModel.Employees = await _context.Employees.ToListAsync();

            viewModel.EmployeeSubjects = await _context.EmployeeSubjects
                .Include(es => es.Subject)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Notifications
        public async Task<IActionResult> Notification()
        {
            var employeeId = HttpContext.Session.GetString("EmployeeId");

            if (employeeId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var employeesNotifications = _context.EmployeeNotifications
                .Include(en => en.Query)
                    .ThenInclude(q => q.Employee)
                .Where(en => en.EmployeeID == int.Parse(employeeId));

            var notifications = await employeesNotifications.ToListAsync();

            return View(notifications);
        }
    }
}
