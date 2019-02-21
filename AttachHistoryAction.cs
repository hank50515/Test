using GSS.SPEED.Dao.Model;
using GSS.SPEED.OD30.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSS.SPEED.OD30.Model.ModelGlobal;
using GSS.SPEED.SQLTool;
using GSS.SPEED.OD30.Model.Entity.Attachment;
using GSS.SPEED.SQLTool.Model;

namespace GSS.SPEED.OD30.DAL.Action
{
    public class AttachHistoryAction : IAttachHistoryAction
    {
        /// <summary>
        /// 查詢公文附件歷程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compId">公司/機關代碼</param>
        /// <param name="docNumber">公司/機關代碼</param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string compId, string docNumber)
        {
            return Select.Columns<AttachmentHistory, T>()
                         .From("ODMATTCHG")
                         .Where(@"COMP_ID = @compId AND CNO_CODE = @docNumber")
                         .Query<T>(new { compId, docNumber });
        }

        /// <summary>
        /// 查詢公文附件歷程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compId">公司/機關代碼</param>
        /// <param name="docNumber">公司/機關代碼</param>
        /// <param name="pageRow">分頁數</param>
        /// <param name="page">分頁-頁碼</param>
        /// <param name="orderBy">排序依據</param>
        /// <param name="seq">排序方式</param>
        /// <returns></returns>
        public Paging<T> QueryDraftAttach<T>(string compId, string docNumber, int pageRow, int page, string orderBy, GlobalEnum.Sequence seq)
        {
            //mod by ye 20170317 GSSSPEED-1135 公文編輯-缺附件修改歷程 加入ODMED2DOC，用來查詢函附件
            string cte = "";
            #region 稿附件
            SqlSet cteSqlsetODMED = Select.Columns<DraftAttachHistory, T>()
                            .Columns("ED_SEQ_NAME : [DraftSeqName]")
                            .From("ODMATTCHG", "CHG")
                            .LeftJoin("ODMED", "ED", @"CHG.COMP_ID = ED.COMP_ID 
                                     AND CHG.CNO_CODE = ED.CNO_CODE
                                     AND CHG.RECV_NO = ED.COMP_CODE")
                            .Where(@"CHG.COMP_ID = @compId 
                                  AND CHG.CNO_CODE = @docNumber 
                                  AND CHG.ATT_KIND NOT IN ('E0', 'E1', 'P0', 'P1') 
                                  AND CHG.RECV_KIND=3")
                                 .Output();
            cte += $@";WITH CTEODMED AS({cteSqlsetODMED.Sql})";
            #endregion

            #region 函附件 函的Table裡面不會存名稱，所以先取出稿的名稱，再將"稿"替換成"函"
            SqlSet cteSqlsetODMED2DOC = Select.Columns<DraftAttachHistory, T>()
                             .Columns("REPLACE (ED_SEQ_NAME, '稿', '函') : [DraftSeqName]")
                             .From("ODMATTCHG", "CHG")
                             .InnerJoin("ODMED2DOC", "E2D", @"CHG.COMP_ID = E2D.COMP_ID 
                                     AND CHG.CNO_CODE = E2D.CNO_CODE
                                     AND CHG.RECV_NO = E2D.COMP_CODE")
                             .InnerJoin("ODMED", "ED", @"E2D.COMP_ID = ED.COMP_ID 
                                     AND E2D.CNO_CODE = ED.CNO_CODE
                                     AND E2D.SCOMP_CODE = ED.COMP_CODE ")
                             .Where(@"CHG.COMP_ID = @compId 
                                  AND CHG.CNO_CODE = @docNumber 
                                  AND CHG.ATT_KIND NOT IN ('E0', 'E1', 'P0', 'P1') 
                                  AND CHG.RECV_KIND=3")
                                  .Output();
            cte += $@", CTEODMED2DOC AS({cteSqlsetODMED2DOC.Sql})";
            #endregion

            //Union以上兩個cte
            cte += $@", CTERESULT AS(SELECT * FROM CTEODMED UNION SELECT * FROM CTEODMED2DOC WITH (NOLOCK) )";

            return Select.Columns()
                               .Cte(cte)
                               .From("CTERESULT")
                               .OrderBy(orderBy, "[ChangeDate]", seq.ToString())
                               .Paging(pageRow, page)
                                .PagingQuery<T, Paging<T>>(new
                                {
                                    compId,
                                    docNumber
                                },
                                i => new Paging<T>(i.Datas, i.Total));            
        }

        /// <summary>
        /// 新增附件檔歷史資訊
        /// </summary>
        /// <param name="insAtts">新增資訊</param>
        /// <param name="currentUser">新增資料的人員資訊</param>
        /// <returns></returns>
        public IEnumerable<DaoSqlSetting> Insert(IEnumerable<AttachmentHistory> insAtts, UserInfo currentUser)
        {
            if (!insAtts.Any()) return new DaoSqlSetting[] { };

            UserEditInfo user = currentUser.ToEditType();
            //修改為寫ODMATTCHG時，ADD_XXX及 CRT_XXX都沿用原ODMATT的資料
            return insAtts.Select(i => SQLTool.Insert.Table("ODMATTCHG")
                                                     .Columns(i)
                                                     .MatcheColumns(user, "CHG")
                                                     .ColumnsFrom("ODMATT",
                                                                    "ADD_COMP", "ADD_COMP_ID", "ADD_DEPT_IDENT", "ADD_DEPT", "ADD_USR_IDENT", "ADD_USR", "ADD_DT",
                                                                    "CRT_COMP", "CRT_COMP_ID", "CRT_DEPT_IDENT", "CRT_DEPT", "CRT_USR_IDENT", "CRT_USR", "CRT_DT"
                                                                    )
                                                     .Where("COMP_ID = @CompId AND ATT_SEQ = @SequenceNumber")
                                                     .Output<DaoSqlSetting>(new
                                                     {
                                                         i.CompId,
                                                         i.SequenceNumber
                                                     }));
        }

        /// <summary>
        /// 刪除公文附件歷程
        /// </summary>
        /// <param name="compId">公司/機關代碼</param>
        /// <param name="docNumber">公司/機關代碼</param>
        /// <returns></returns>
        public DaoSqlSetting Delete(string compId, string docNumber)
        {
            return SQLTool.Delete.Table("ODMATTCHG")
                                 .Where("COMP_ID = @compId AND CNO_CODE = @docNumber")
                                 .Output<DaoSqlSetting>(new { compId, docNumber });
        }

       
    }
}
