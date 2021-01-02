using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models
{
	public enum States
	{
		pending, 
		closed
	}

	public class Query
	{
		[Key]
		public int QueryID { get; set; }

		[StringLength(500)]
		public string Description { get; set; }

		[Display(Name = "Current State")]
		public States QState { get; set; }

		// .HasDefaultValueSql("getdate()");
		[Display(Name = "Date & Time")]
		public DateTime Added { get; set; }

		public int EmployeeID { get; set; }

		[Display(Name = "Type")]
		public Expertise Tag { get; set; }

		public Employee Employee { get; set; }
		public ICollection<Solution> Solutions { get; set; }
        public ICollection<EmployeeNotification> EmployeeNotifications { get; set; }
    }
}
