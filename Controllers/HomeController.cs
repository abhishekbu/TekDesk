using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TekDesk.Models;
using Microsoft.AspNetCore.Http;
using TekDesk.Data;

namespace TekDesk.Controllers
{

	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly TekDeskContext _tekDeskContext;

		public HomeController(ILogger<HomeController> logger, TekDeskContext tekDeskContext)
		{
			_logger = logger;
			_tekDeskContext = tekDeskContext;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
        public IActionResult Store(string empId)
        {
			try
            {
				var employee = _tekDeskContext.Employees.Where(e => e.ID == int.Parse(empId)).SingleOrDefault();


				if (employee != null)
				{
					TempData["EmployeeExists"] = true;
					HttpContext.Session.SetString("EmployeeId", empId);
					return RedirectToAction("Index", "Queries");
				}

				TempData["EmployeeExists"] = false;
				TempData["Message"] = "Login Failed! Employee doesn't Exist";
				return RedirectToAction(nameof(Index));
			}
			catch
            {
				TempData["EmployeeExists"] = false;
				TempData["Message"] = "Login Failed! Employee doesn't Exist";
				return RedirectToAction(nameof(Index));
			}
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
