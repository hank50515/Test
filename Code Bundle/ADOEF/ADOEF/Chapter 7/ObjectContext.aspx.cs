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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace ADOEF.Chapter_7
{
    public partial class ObjectContext : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        private void AddRecord()
        {
            PayrollModel.PayrollEntities ctx = new PayrollModel.PayrollEntities();
            PayrollModel.Employee employee = new PayrollModel.Employee();
            employee.EmployeeID = 16;
            employee.FirstName = "Debanjan";
            employee.LastName = "Banerjee";
            employee.Address = "New Delhi";
            employee.JoiningDate = DateTime.Now;
            employee.Department = ctx.Department.
            Where(d => d.DepartmentID == 3).First();
            ctx.AddObject("Employee", employee);
            ctx.SaveChanges();

        }

        private void UpdateRecord()
        {
            PayrollModel.PayrollEntities ctx = new PayrollModel.PayrollEntities();
            PayrollModel.Employee employee =
            ctx.Employee.Where(e => e.EmployeeID == 16).First();
            ctx.DeleteObject(employee);

            PayrollModel.Employee newRecord = new PayrollModel.Employee();
            newRecord.EmployeeID = 16;
            newRecord.FirstName = "Debanjan";
            newRecord.LastName = "Banerjee";
            newRecord.Address = "New Delhi";
            newRecord.JoiningDate = DateTime.Now;
            newRecord.Department = ctx.Department.
            Where(d => d.DepartmentID == 4).First();
            ctx.AddObject("Employee", newRecord);
            ctx.SaveChanges();
        }

        private void DeleteRecord()
          {
            PayrollModel.PayrollEntities ctx = new PayrollModel.PayrollEntities();
            PayrollModel.Employee employee = 
            ctx.Employee.Where(e => e.EmployeeID == 16).First();
            ctx.DeleteObject(employee);
            ctx.SaveChanges();
          }

        private void Serialize(String fileName, Object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(fileName, FileMode.Create);

            try
            {
                binaryFormatter.Serialize(fileStream, obj);
            }

            catch (SerializationException ex)
            {
                throw new ApplicationException("The object graph could not be serialized", ex);
            }
            finally
            {
                fileStream.Close();
            }
        }

        public Object DeSerialize(String fileName)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
			PayrollModel.Employee ct = new PayrollModel.Employee();

            try
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(fileStream);
            }

            catch (SerializationException ex)
            {
                throw new ApplicationException("Serialization Exception: " + ex.Message);
            }

            finally
            {
                fileStream.Close();
            }
         }
		 
		 public Object Test(String fileName)
        {
			
			PayrollModel.Employee ctx = new PayrollModel.Employee();
			foreach (int ct in ctx)
			{
				result.Add(ct);
			}
            
        }
     }
}
