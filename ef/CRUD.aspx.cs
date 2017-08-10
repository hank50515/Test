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

namespace WebApplication3
{

	public partial class CRUD
    {
		private void AddRecord()
        {
            WebApplication3.Product ctx = new WebApplication3.Product();
            WebApplication3.TransactionHistory transactionHistory = new WebApplication3.TransactionHistory();
            transactionHistory.TransactionID = 16;
            ctx.AddObject("TransactionHistory", transactionHistory);
            ctx.SaveChanges();

        }

        private void DeleteRecord()
	    {
			WebApplication3.Product ctx = new WebApplication3.Product();
			WebApplication3.TransactionHistory transactionHistory = 
			ctx.TransactionHistory.Where(e => e.TransactionID == 16).First();
			ctx.DeleteObject(transactionHistory);
			ctx.SaveChanges();
		}
 
	}
}