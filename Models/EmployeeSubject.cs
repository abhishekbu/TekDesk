using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models
{
	public class EmployeeSubject
	{
		public int EmployeeID { get; set; }
		public int SubjectID { get; set; }
		public Employee Employee { get; set; }
		public Subject Subject { get; set; }
	}
}
