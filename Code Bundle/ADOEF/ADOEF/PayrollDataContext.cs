using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Data.EntityClient;
using System.Data.Objects;

public class PayrollDataContext : PayrollModel.PayrollEntities
{
    String connectionString;

    public PayrollDataContext()
    {
        connectionString = base.Connection.ConnectionString;
    }

    public String ConnectionString
    {
        get
        {
            return connectionString;
        }
    }

    public ObjectResult<PayrollModel.Employee> GetEmployeeRecords()
    {
        ObjectContext ctx = new ObjectContext("Name=PayrollEntities");
        ObjectQuery<PayrollModel.Employee> query =
        ctx.CreateQuery<PayrollModel.Employee>(@"SELECT VALUE e FROM PayrollEntities.Employee AS e");
        ObjectResult<PayrollModel.Employee> result = query.Execute(MergeOption.NoTracking);
        return result;
    }
}
