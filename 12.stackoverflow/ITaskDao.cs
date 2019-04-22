using GSS.Radar.Domain.Models.Sys;
using GSS.Radar.Infrastructure.Arguments;
using GSS.Radar.Infrastructure.Data;
using System.Collections.Generic;

namespace GSS.Radar.Domain.Persistence.Production
{
    public interface ITaskDao : IEntityDao<Task>
    {
        IList<ViewTask> GetTaskList(IBasicArgs args);
    }
}
