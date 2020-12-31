using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models
{
	public enum Expertise
	{
		Hardware,
		Software,
		Network
	}

	public class Subject
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ID { get; set; }

		[Required]
		public Expertise Name { get; set; }

		public ICollection<EmployeeSubject> EmployeeSubjects { get; set; }
	}
}
