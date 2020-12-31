using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models
{
	public class Employee
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ID { get; set; }

		[Required]
		[Column("FirstName")]
		[Display(Name = "First Name")]
		[StringLength(50)]
		public string FName { get; set; }

		[Required]
		[Column("LastName")]
		[Display(Name = "Last Name")]
		[StringLength(50)]
		public string LName { get; set; }

		[Display(Name = "Employee")]
		public string FullName {
			get
			{
				return FName + " " + LName;
			} 
		}

		public ICollection<Query> Queries { get; set; }

        public ICollection<EmployeeSubject> EmployeeSubjects { get; set; }
		public ICollection<EmployeeNotification> EmployeeNotifications { get; set; }
    }
}
