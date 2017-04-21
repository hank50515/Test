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
using System.Data.EntityClient;

namespace ADOEF.Chapter_4
{
    public partial class StoredProcedures : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

        }

        private void InsertDepartment()
        {
            using (EntityConnection conn = new EntityConnection("Name=PayrollEntities"))
            {
                try
                {
                    conn.Open();
                    EntityCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "PayrollEntities.InsertDepartment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue
                    ("DepartmentName", "Finance");
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }
            }
        }
    }
}
