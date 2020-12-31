using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models.TekDeskViewModels
{
    public class EmployeeQueries
    {
        public Employee Employee { get; set; }
        public ICollection<Query> Queries { get; set; }
    }
}
