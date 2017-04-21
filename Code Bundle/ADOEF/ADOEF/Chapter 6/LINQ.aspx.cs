using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using PayrollModel;
using System.Data.Objects;

namespace ADOEF.Chapter_6
{
    public class Employee
    {
        public string EmpCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string DeptCode { get; set; }
        public DateTime JoiningDate { get; set; }
        public decimal Salary { get; set; }
    }

    public class EmployeeContact
    {
        private String name;
        private String address;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }
    }

    public partial class LINQ : System.Web.UI.Page
    {
        private static List<String> GenericEmployeeList = new List<String>()
            {
                "Joydip", "Douglas", "Jini", "Piku","Rama", "Amal", "Indronil"
            };

        protected void Page_Load(object sender, EventArgs e)
        {
            PayrollModel.PayrollEntities ctx = new PayrollModel.PayrollEntities();
            var query = from dept in ctx.Department
                        select dept;
            foreach (var department in query)
                Response.Write("<BR>" + department.DepartmentName);
        }

        private void QueryGenericList()
        {
            IEnumerable<String> employees = from emp in GenericEmployeeList
                                            select emp;
            foreach (string employee in employees)
            {
                Response.Write(employee);
            }


            IEnumerable<String> employeeList = from emp in GenericEmployeeList
                                               where emp.Length > 4
                                               select emp;
            foreach (string employee in employeeList)
            {
                Response.Write(employee);
            }

        }

        private void LINQSample()
        {
            var employees = new List<Employee> { new Employee { FirstName = "Joydip", LastName = "Kanjilal",Address = "Hyderabad" },
            new Employee { FirstName = "Douglas", LastName = "Paterson",Address = "Birmingham"},
            new Employee { FirstName = "Oindrilla", LastName = "RoyChowdhury", Address = "Kolkata" }};

            var query =
            from employee in employees
            orderby employee.FirstName
            select employee;
        }

        private void LinqToDataSet()
        {
            DataTable empDataTable = new DataTable();
            empDataTable.Columns.Add("EmpCode", typeof(String));
            empDataTable.Columns.Add("EmpName", typeof(String));
            empDataTable.Columns.Add("DeptCode", typeof(String));
            empDataTable.Columns.Add("Salary", typeof(Decimal));
            empDataTable.Rows.Add("E0001", "Joydip", "D0001", 23000);
            empDataTable.Rows.Add("E0002", "Douglas", "D0002", 45000);
            empDataTable.Rows.Add("E0003", "Jini", "D0001", 12000);
            empDataTable.Rows.Add("E0004", "Piku", "D0003", 13000);
            empDataTable.Rows.Add("E0005", "Rama", "D0003", 27500);
            empDataTable.Rows.Add("E0006", "Amal", "D0002", 19500);
            var empRecords = from row in empDataTable.AsEnumerable()
                             where row.Field<decimal>("Salary") > 15000
                             select row;

            foreach (var emp in empRecords)
                Response.Write("<BR>" + emp["EmpCode"].ToString() + "\t" + emp["EmpName"].ToString() + "\t" + emp["Salary"].ToString());
        }

        private void GetEmployeeData()
        {
            PayrollEntities payrollEntities = new PayrollModel.PayrollEntities();
            IQueryable<EmployeeContact> query = payrollEntities.Employee
            .Where(emp => emp.JoiningDate >= new DateTime(2004, 01, 01))
            .Select(emp => new EmployeeContact { Name = emp.FirstName, Address = emp.Address });
        }

        public static void ImmediateExecution()
        {
            int[] intArray = new int[] { 1, 2, 3, 4, 5};
            int index = 0;
            var query = from i in intArray select ++index;
            Console.WriteLine("Illustrating Immediate Execution\n");
            foreach (var number in query)
            Console.WriteLine("The value of number is: {0}. The value of index is: {1}", number, index);
        }

        public static void DeferredExecution()
        {
            int[] intArray = new int[] { 1, 2, 3, 4, 5 };
            int index = 0;
            var query = (from i in intArray select ++index).ToList();
            Console.WriteLine("\n\nIllustrating Deferred Execution\n");
            foreach (var number in query)
            Console.WriteLine("The value of number is: {0}. The value of index is: {1}", number, index);
        }

        private void CompiledQueries()
        {
            var employees = CompiledQuery.Compile((PayrollEntities entities, string address) =>
            from employee in entities.Employee where employee.Address.Equals(address)
            orderby employee.FirstName select employee);
            using (PayrollEntities payrollEntities = new PayrollEntities())
            {
                foreach (var employee in employees(payrollEntities, "Kolkata"))
                {
                    Response.Write("<BR>" + employee.FirstName);
                }
            }
        }
        
        private void QueryFromGenericList()
        {
            List<Employee> empList = new List<Employee>()
            {
            new Employee
            {
            EmpCode = "E0001", FirstName = "Joydip", DeptCode =
            "D0001", Salary = 23000
            },
            new Employee
            {
            EmpCode = "E0002", FirstName = "Douglas", DeptCode =
            "D0003", Salary = 45000
            },
            new Employee
            {
            EmpCode = "E0003", FirstName = "Jini", DeptCode = "D0002",
            Salary = 15000
            }
            };
            var empRecords = from row in empList.AsEnumerable()
                             where row.Salary > 15000
                             select row;

            foreach (var emp in empRecords)
                Response.Write("<BR>" + emp.EmpCode.ToString() + "\t" +
                emp.FirstName.ToString() + "\t" + emp.Salary.ToString());

        }
    }

}
