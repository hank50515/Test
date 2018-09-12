using GSS.Radar.Domain.Models.Mail;
using GSS.Radar.Domain.Models.Payroll;
using GSS.Radar.Domain.Models.TaxReturn;
using GSS.Radar.Infrastructure.Arguments;
using GSS.Radar.Infrastructure.Data;
using GSS.Radar.Infrastructure.Data.Impl;
using GSS.Radar.Infrastructure.Data.SqlHelper;
using System;
using System.Collections.Generic;
　
namespace GSS.Radar.Core.Services.Domain.Persistence.Impl
{
    public class MailInformationDao : EntityDao<MailInformation>, IMailInformationDao
    {
        public virtual IList<DbLinkTable> GetDbLinkTable(SQLQueryParameter filter)
        {
　
            SqlBuilder sb = new SqlBuilder(@"
                        select *
                        from adm..dao.PROJECT "
                , SqlBuilder.ParamType.NHibernate)
                .WhereScope(where =>
                {
                    where
                        .AddWhereClause(filter.FilterGroup)
                    ;
                });
　
            ExecuteSQL<DbLinkTable>(sb);
        }
		
		public virtual IList<DbLinkTable> GetDbLinkTableByOpenQuery(SQLQueryParameter filter)
        {
　
            SqlBuilder sb = new SqlBuilder(@"
                        select *
                        from OPENQUERY(adm, 'SELECT id FROM TEST.PROJECT') "
                , SqlBuilder.ParamType.NHibernate)
                .WhereScope(where =>
                {
                    where
                        .AddWhereClause(filter.FilterGroup)
                    ;
                });
　
            ExecuteSQL<DbLinkTable>(sb);
        }
　
        public virtual IList<DbLinkTable> GetDbLinkTableByInformix(SQLQueryParameter filter)
        {
            SqlBuilder sb = new SqlBuilder(@"
                        select *
                        from adm@remoteoffice:project "
                , SqlBuilder.ParamType.NHibernate)
                .WhereScope(where =>
                {
                    where
                        .AddWhereClause(filter.FilterGroup)
                    ;
                });
　
            ExecuteSQL<DbLinkTable>(sb);
        }
		
		public virtual IList<DbLinkTable> GetDbLinkTableByInformixAlias(SQLQueryParameter filter)
        {
            SqlBuilder sb = new SqlBuilder(@"
                        select *
                        from adm@remoteoffice:informix.project "
                , SqlBuilder.ParamType.NHibernate)
                .WhereScope(where =>
                {
                    where
                        .AddWhereClause(filter.FilterGroup)
                    ;
                });
　
            ExecuteSQL<DbLinkTable>(sb);
        }
	}
}