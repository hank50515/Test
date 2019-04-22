using GSS.Radar.Domain.Models.Sys;
using GSS.Radar.Infrastructure.Arguments;
using GSS.Radar.Infrastructure.Data.Impl;
using GSS.Radar.Infrastructure.Data.SqlHelper;
using System.Collections.Generic;

namespace GSS.Radar.Domain.Persistence.Production.Impl
{
    public class TaskDao : EntityDao<Task>, ITaskDao
    {
        public virtual IList<ViewTask> GetTaskList(IBasicArgs args)
        {
            return ExecuteSQL<ViewTask>(sb);
        }
    }
}