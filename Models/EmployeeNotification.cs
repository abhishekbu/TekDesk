using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models
{
	public class EmployeeNotification
	{
        public int EmployeeID { get; set; }
        public int QueryID { get; set; }

        [StringLength(60)]
        public string Notification { get; set; }

        public Employee Employee { get; set; }
        public Query Query { get; set; }
    }
}
