using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models
{
	public class Solution
	{
		[Key]
		public int ID { get; set; }

		[StringLength(500)]
		public string Description { get; set; }

		public DateTime Added { get; set; }

        public int EmployeeID { get; set; }

        public int QueryID { get; set; }

        public Query Query { get; set; }
		public Artifact Artifact { get; set; }
		public Employee Employee { get; set; }

	}
}
