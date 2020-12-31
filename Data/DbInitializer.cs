using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TekDesk.Models;

namespace TekDesk.Data
{
    public class DbInitializer
    {
        public static void Initialize(TekDeskContext context)
        {
            context.Database.EnsureCreated();

            // Look for any employees
            if (context.Employees.Any())
            {
                return; // DB has been seeded
            }

            var employees = new Employee[]
            {
                new Employee{ID=1000, FName="KING", LName="FIRST"},
                new Employee{ID=1001, FName="BLAKE", LName="LAST"},
                new Employee{ID=1002, FName="Carson", LName="Alexander"},
                new Employee{ID=1003, FName="Meredith", LName="Alonso"},
                new Employee{ID=1004, FName="Arturo", LName="Anand"},
                new Employee{ID=1005, FName="Gytis", LName="Barzdukas"},
                new Employee{ID=1006, FName="Yan", LName="Li"},
                new Employee{ID=1007, FName="Peggy", LName="Justice"},
                new Employee{ID=1008, FName="Laura", LName="Norman"},
                new Employee{ID=1009, FName="Nino", LName="Olivetto"},
            };

            foreach (Employee e in employees)
            {
                context.Employees.Add(e);
            }
            context.SaveChanges();

            var queries = new Query[]
            {
                new Query{Description="Network Device not connecting", 
                    EmployeeID=1003, QState=States.pending, Tag=Expertise.Network, 
                    Added=DateTime.Now},
                new Query{Description="OS showing not activated", 
                    EmployeeID=1005, QState=States.pending, Tag=Expertise.Software, 
                    Added=DateTime.Now},
                new Query{Description="MS office Not Installing", 
                    EmployeeID=1004, QState=States.pending, Tag=Expertise.Software, 
                    Added=DateTime.Now},
                new Query{Description="Server crashed", 
                    EmployeeID=1002, QState=States.pending, Tag=Expertise.Network, 
                    Added=DateTime.Now},
                new Query{Description="Keyborad not functioning correctly", 
                    EmployeeID=1002, QState=States.pending, Tag=Expertise.Hardware, 
                    Added=DateTime.Now},
            };

            foreach (Query q in queries)
            {
                context.Queries.Add(q);
            }
            context.SaveChanges();

            var subjects = new Subject[]
            {
                new Subject{ID=2, Name=Expertise.Network},
                new Subject{ID=1, Name=Expertise.Software},
                new Subject{ID=0, Name=Expertise.Hardware}
            };

            foreach (Subject s in subjects)
            {
                context.Subjects.Add(s);
            }
            context.SaveChanges();

            var solutions = new Solution[]
            {
                new Solution{Description="Check the network cables", EmployeeID=1000, QueryID=1},
                new Solution{Description="Renew the Activation", EmployeeID=1002, QueryID=2},
                new Solution{Description="Check System Requirements", EmployeeID=1003, QueryID=3},
                new Solution{Description="Contact data recovery", EmployeeID=1001, QueryID=4},
                new Solution{Description="Buy a new one", EmployeeID=1005, QueryID=5}
            };

            foreach (Solution s in solutions)
            {
                context.Solutions.Add(s);
            }
            context.SaveChanges();

            var employeeSubjects = new EmployeeSubject[]
            {
                new EmployeeSubject{EmployeeID=1000, SubjectID=0},
                new EmployeeSubject{EmployeeID=1001, SubjectID=0},
                new EmployeeSubject{EmployeeID=1002, SubjectID=1},
                new EmployeeSubject{EmployeeID=1003, SubjectID=1},
                new EmployeeSubject{EmployeeID=1004, SubjectID=2},
                new EmployeeSubject{EmployeeID=1005, SubjectID=2},
                new EmployeeSubject{EmployeeID=1006, SubjectID=0},
                new EmployeeSubject{EmployeeID=1007, SubjectID=0},
                new EmployeeSubject{EmployeeID=1008, SubjectID=1},
                new EmployeeSubject{EmployeeID=1009, SubjectID=2},
            };

            foreach (EmployeeSubject es in employeeSubjects)
            {
                context.EmployeeSubjects.Add(es);
            }
            context.SaveChanges();

            var employeeNotifications = new EmployeeNotification[]
            {
                new EmployeeNotification
                {
                    EmployeeID=1004, 
                    Notification="Network Device not connecting", 
                    QueryID=1
                },
                new EmployeeNotification
                {
                    EmployeeID=1005, 
                    Notification="Network Device not connecting", 
                    QueryID=1
                },
                new EmployeeNotification
                {
                    EmployeeID=1009, 
                    Notification="Network Device not connecting", 
                    QueryID=1
                },
               
                new EmployeeNotification
                {
                    EmployeeID=1002, 
                    Notification="OS showing not activated", 
                    QueryID=2
                },
                new EmployeeNotification
                {
                    EmployeeID=1003, 
                    Notification="OS showing not activated", 
                    QueryID=2
                },
                new EmployeeNotification
                {
                    EmployeeID=1008, 
                    Notification="OS showing not activated", 
                    QueryID=2
                },

                new EmployeeNotification
                {
                    EmployeeID=1002, 
                    Notification="MS office Not Installing", 
                    QueryID=3
                },
                new EmployeeNotification
                {
                    EmployeeID=1003, 
                    Notification="MS office Not Installing", 
                    QueryID=3
                },
                new EmployeeNotification
                {
                    EmployeeID=1008, 
                    Notification="MS office Not Installing", 
                    QueryID=3
                },
                new EmployeeNotification
                {
                    EmployeeID=1004, 
                    Notification="Server crashed", 
                    QueryID=4
                },
                new EmployeeNotification
                {
                    EmployeeID=1005, 
                    Notification="Server crashed", 
                    QueryID=4
                },
                new EmployeeNotification
                {
                    EmployeeID=1009, 
                    Notification="Server crashed", 
                    QueryID=4
                },
                  
                new EmployeeNotification
                {
                    EmployeeID=1000, 
                    Notification="Keyborad not functioning correctly", 
                    QueryID=5
                },
                new EmployeeNotification
                {
                    EmployeeID=1001, 
                    Notification="Keyborad not functioning correctly", 
                    QueryID=5
                },
                new EmployeeNotification
                {
                    EmployeeID=1006, 
                    Notification="Keyborad not functioning correctly", 
                    QueryID=5
                },
                new EmployeeNotification
                {
                    EmployeeID=1007, 
                    Notification="Keyborad not functioning correctly", 
                    QueryID=5
                },
            };

            foreach (EmployeeNotification ef in employeeNotifications)
            {
                context.EmployeeNotifications.Add(ef);
            }
            context.SaveChanges();
        }
    }
}
