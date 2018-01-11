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
            employee.FirstName = "SELECT balance FROM adm..TEST.PROJECT WHERE id = 12345";
            employee.LastName = "SELECT * FROM OPENQUERY(adm, 'SELECT id FROM TEST.PROJECT WHERE id = 12345')";
			employee.LastName = "SELECT * FROM adm@remoteoffice:project";
			employee.LastName = "SELECT * FROM adm@remoteoffice:informix.project";
            ctx.SaveChanges();

        }
    }
}
