using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models
{
	public class Artifact
	{
		[Key]
		public int ID { get; set; }

		[StringLength(10)]
		public string Type { get; set; }

		[StringLength(30)]
		public string Name { get; set; }

		public string file { get; set; }

        public int SolutionID { get; set; }

		public Solution Solution { get; set; }
	}
}
