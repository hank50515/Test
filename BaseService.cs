using GSS.ITSM.Core.Production.Account;
using GSS.ITSM.Dal;
using GSS.ITSM.Domain;
using GSS.ITSM.WorkFlow.IncidentServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
　
namespace GSS.ITSM.API.Service.Base
{
    public abstract class BaseService
    {
        public AbstractBaseRepository BaseRepo { get; set; }
        public IItsmSeqService ItsmSeqService { get; set; }
        public IItsmSeqCiidService ItsmSeqCiidService { get; set; }
　
        public IWorkFlowServices WorkFlowServices { get; set; }
        public IUserService UserService { get; set; }
　
    }
}