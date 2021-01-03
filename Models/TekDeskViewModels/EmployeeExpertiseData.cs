using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Models.TekDeskViewModels
{
    public class EmployeeExpertiseData
    {
        public Employee Employee { get; set; }
        public ICollection<EmployeeSubject> EmployeeSubjects { get; set; }
    }
}
