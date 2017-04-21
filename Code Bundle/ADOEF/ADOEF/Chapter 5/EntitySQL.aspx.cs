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
using System.Data.SqlClient;
using System.Data.Common;

namespace ADOEF.Chapter_5
{
    public partial class EntitySQL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void SelectEmployees()
        {
            String connectionString = String.Empty;
            using (EntityConnection entityConnection = new EntityConnection(connectionString))
            {
                entityConnection.Open();
                String queryString = "Select value e from PayrollEntities.Employee as e";
                using (EntityCommand entityCommand = new EntityCommand(queryString, entityConnection))
                {
                    using (DbDataReader dataReader = entityCommand.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (dataReader.Read())
                        {
                            Response.Write(dataReader.GetValue(0));
                        }
                    }
                }
            }
        }

        private void EntitySQLSample()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            sqlConnectionStringBuilder.DataSource = ".";
            sqlConnectionStringBuilder.InitialCatalog = "test";
            sqlConnectionStringBuilder.IntegratedSecurity = false;
            sqlConnectionStringBuilder.UserID = "sa";
            sqlConnectionStringBuilder.Password = "sa";
            EntityConnectionStringBuilder entityConnectionStringBuilder =
            new EntityConnectionStringBuilder();
            entityConnectionStringBuilder.Metadata = ".";
            entityConnectionStringBuilder.Provider = "System.Data.SqlClient";
            entityConnectionStringBuilder.ProviderConnectionString = sqlConnectionStringBuilder.ToString();
            entityConnectionStringBuilder.Metadata = ".";
            
            using (EntityConnection entityConnection =
            new EntityConnection(entityConnectionStringBuilder.ToString()))
            {
                entityConnection.Open();
                String sqlString = "SELECT FirstName, LastName, Address, PhoneNo FROM HREmployees";
                EntityCommand entityCommand = new EntityCommand(sqlString, entityConnection);
                EntityDataReader entityDataReader = entityCommand.ExecuteReader
                (CommandBehavior.SequentialAccess);
                while (entityDataReader.Read())
                    Response.Write(entityDataReader[0].ToString() + "\t" +
                    entityDataReader[1].ToString());
                if (entityConnection.State == ConnectionState.Open)
                    entityConnection.Close();
            }
        }
    }
}
