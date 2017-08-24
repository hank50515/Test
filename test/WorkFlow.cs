///需求單號：201607040315-00
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BusinessLogicLayer.Service.Interface;
using BusinessLogicLayer;
using DomainObject.DomainObject;
using System.Xml.Serialization;
using DomainObject.DomainObject.WorkFlowObject;
using System.Web.Script.Services;
　
/// <summary>
/// WorkFlow 的摘要描述
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
[System.Web.Script.Services.ScriptService]
public class WorkFlow : System.Web.Services.WebService {
　
    #region Property
　
    private static IWorkFlowService _WFService;
    public IWorkFlowService WFService
    {
        get
        {
            if (_WFService == null)
            {
                _WFService = (IWorkFlowService)(new RepositoryFactory()).Service("WorkFlowService");
            }
　
            return _WFService;
        }
    }
　
    #endregion
　
    #region Process Definition Methods
    /// <summary>
    /// 取得所有流程樣版資料
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/20  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetProcDefs(ReceiveObject obj)
    {
        return this.WFService.GetProcDefs(obj);
    }
　
    /// <summary>
    /// 取得流程樣版ID
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/20  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetBaseProcDefID(ReceiveObject obj)
    {
        return this.WFService.GetBaseProcDefID(obj);
    }
　
    /// <summary>
    /// 取得Release流程樣版ID
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/27  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetReleasedProcDefs(ReceiveObject obj)
    {
        return this.WFService.GetReleasedProcDefs(obj);
    }
　
    /// <summary>
    /// 刪除所有Process Definition
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/17  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject DeleteAllDefinition(ReceiveObject obj)
    {
        return this.WFService.DeleteAllDefinition(obj);
    }
    #endregion
　
    #region Process Instances Method
    /// <summary>
    /// 新增Process Instance
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/20  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CreateProcInst(ReceiveObject obj)
    {
        return this.WFService.CreateProcInst(obj);
    }
　
    /// <summary>
    /// 中止Process Instance
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/25  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject SuspendProcInst(ReceiveObject obj)
    {
        return this.WFService.SuspendProcInst(obj);
    }
　
    /// <summary>
    /// 取消Process Instance
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/10/12  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CancelProcInst(ReceiveObject obj)
    {
        return this.WFService.CancelProcInst(obj);
    }
　
    /// <summary>
    /// 取得Process Instance資料
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/25  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetProcInst(ReceiveObject obj)
    {
        return this.WFService.GetProcInst(obj);
    }
　
    /// <summary>
    /// 取得Process Instance Attributes
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/25  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetProcInstAttrs(ReceiveObject obj)
    {
        return this.WFService.GetProcInstAttrs(obj);
    }
　
    /// <summary>
    /// 重啟Process Instance
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/25  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject ResumeProcInst(ReceiveObject obj)
    {
        return this.WFService.ResumeProcInst(obj);
    }
　
    /// <summary>
    /// Rollback Process
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/08/19  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject RollbackProcInst(ReceiveObject obj)
    {
        return this.WFService.RollbackProcInst(obj);
    }
　
    /// <summary>
    /// 刪除Process Instance
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/29  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject DeleteProcInst(ReceiveObject obj)
    {
        return this.WFService.DeleteProcInst(obj);
    }
　
    /// <summary>
    /// 刪除所有Process Instance
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/29  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject DeleteAllProcInst(ReceiveObject obj)
    {
        return this.WFService.DeleteAllProcInst(obj);
    }
    #endregion
　
    #region Activity Instance Method
    /// <summary>
    /// 取得Active Instances資料 by Process Instance ID
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/26  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetActivityInstsByPIID(ReceiveObject obj)
    {
        return this.WFService.GetActivityInstsByPIID(obj);
    }
    #endregion
　
    #region Manual Work Items Method
    /// <summary>
    /// 取得Manual Work Item資料
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/26  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetWorkItem(ReceiveObject obj)
    {
        return this.WFService.GetWorkItem(obj);
    }
　
    /// <summary>
    /// Complete WorkItem
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/26  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject OriginalCompleteWorkItem(ReceiveObject obj)
    {
        return this.WFService.OriginalCompleteWorkItem(obj);
    }
　
    /// <summary>
    /// Cancel WorkItem
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/26  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CancelWorkItem(ReceiveObject obj)
    {
        return this.WFService.CancelWorkItem(obj);
    }
　
    /// <summary>
    /// Assign WorkItem
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/27  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject OriginalAssignWorkItem(ReceiveObject obj)
    {
        return this.WFService.OriginalAssignWorkItem(obj);
    }
　
    /// <summary>
    /// 解除 Assign WorkItem
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/27  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject OriginalUndoAssignWorkItem(ReceiveObject obj)
    {
        return this.WFService.OriginalUndoAssignWorkItem(obj);
    }
　
    /// <summary>
    /// 重新指派WorkItem
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/27  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject ReassignWorkItem(ReceiveObject obj)
    {
        return this.WFService.ReassignWorkItem(obj);
    }
　
    /// <summary>
    /// 取得WorkList by UserID
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/27  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetWorkListByUserID(ReceiveObject obj)
    {
        return this.WFService.GetWorkListByUserID(obj);
    }
　
    /// <summary>
    /// 新增 Linked Work Item (並簽)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/28  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject OriginalCreateLinkedWorkItem(ReceiveObject obj)
    {
        return this.WFService.OriginalCreateLinkedWorkItem(obj);
    }
　
    /// <summary>
    /// 新增 Work Item (串簽)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/28  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CreateWorkItem(ReceiveObject obj)
    {
        return this.WFService.CreateWorkItem(obj);
    }
　
    /// <summary>
    /// 重指派WorkItem(可更新OriginalUserID)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/28  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject ReassignUpdateWorkItem(ReceiveObject obj)
    {
        return this.WFService.ReassignUpdateWorkItem(obj);
    }
　
    /// <summary>
    /// 更新WorkItem
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/28  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject UpdateWorkItem(ReceiveObject obj)
    {
        return this.WFService.UpdateWorkItem(obj);
    }
    #endregion 
　
    #region Custom Attributes Method
    /// <summary>
    /// 設定CustomerAttr
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/28  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject SetCustomAttr(ReceiveObject obj)
    {
        return this.WFService.SetCustomAttr(obj);
    }
　
    /// <summary>
    /// 取得CustomerAttr
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/29  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetCustomAttr(ReceiveObject obj)
    {
        return this.WFService.GetCustomAttr(obj);
    }
　
    /// <summary>
    /// 設定CustomerAttrs(多筆)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/29  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject SetCustomAttrs(ReceiveObject obj)
    {
        return this.WFService.SetCustomAttrs(obj);
    }
　
    /// <summary>
    /// 取得CustomerAttrs(多筆)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/29  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetCustomAttrs(ReceiveObject obj)
    {
        return this.WFService.GetCustomAttrs(obj);
    }
　
    /// <summary>
    /// 移除CustomerAttrs(多筆)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/29  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject RemoveCustomAttrs(ReceiveObject obj)
    {
        return this.WFService.RemoveCustomAttrs(obj);
    }
    #endregion
　
    #region Archiving And Retoring Process Method
    /// <summary>
    /// 流程歸檔
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/04/29  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject OriginalArchiveProcInst(ReceiveObject obj)
    {
        return this.WFService.OriginalArchiveProcInst(obj);
    }
    #endregion
　
    #region Customize Method
    /// <summary>
    /// 啟動母案件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/13  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CreateCase(ReceiveObject obj)
    {
        return this.WFService.CreateCase(obj);
    }
　
    /// <summary>
    /// 啟動掃瞄進件母案件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/13  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CreateCaseByScan(ReceiveObject obj)
    {
        return this.WFService.CreateCaseByScan(obj);
    }
　
    /// <summary>
    /// 啟動子案件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/15  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CreateSubCase(ReceiveObject obj)
    {
        return this.WFService.CreateSubCase(obj);
    }
　
    /// <summary>
    /// 取消案件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/14  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CancelCase(ReceiveObject obj)
    {
        return this.WFService.CancelCase(obj);
    }
　
    /// <summary>
    /// 暫停案件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/15  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject SuspendCase(ReceiveObject obj)
    {
        return this.WFService.SuspendCase(obj);
    }
　
    /// <summary>
    /// 重啟案件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/14  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject ResumeCase(ReceiveObject obj)
    {
        return this.WFService.ResumeCase(obj);
    }
　
    /// <summary>
    /// 更新案件參數
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/16  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject SetCasePara(ReceiveObject obj)
    {
        return this.WFService.SetCasePara(obj);
    }
　
    /// <summary>
    /// 取得案件參數
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/16  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetCasePara(ReceiveObject obj)
    {
        return this.WFService.GetCasePara(obj);
    }
　
    /// <summary>
    /// 封存案件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/18  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject ArchiveCase(ReceiveObject obj)
    {
        return this.WFService.ArchiveCase(obj);
    }
　
    /// <summary>
    /// 新增並簽作業
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/18  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject CreateLinkedWorkItem(ReceiveObject obj)
    {
        return this.WFService.CreateLinkedWorkItem(obj);
    }
　
    /// <summary>
    /// 指派作業人員
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/16  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject AssignWorkItem(ReceiveObject obj)
    {
        return this.WFService.AssignWorkItem(obj);
    }
　
    /// <summary>
    /// 取消指派作業人員
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/16  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject UndoAssignWorkItem(ReceiveObject obj)
    {
        return this.WFService.UndoAssignWorkItem(obj);
    }
　
    /// <summary>
    /// 重分案
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/05/18  Steven_Chen Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject ReAssignWorkItem(ReceiveObject obj)
    {
        return this.WFService.ReAssignWorkItem(obj);
    }
　
    /// <summary>
    /// 進行處理步驟前檢核警告
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/06/02  Daniel Lee Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject DoCaseProcPreAlertA1(ReceiveObject obj)
    {
        return this.WFService.DoCaseProcPreAlertA1(obj);
    }
　
    /// <summary>
    /// 進行處理步驟前檢核警告
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/06/02  Daniel Lee Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject DoCaseProcPreAlert(ReceiveObject obj)
    {
        return this.WFService.DoCaseProcPreAlert(obj);
    }
　
    /// <summary>
    /// 進行處理步驟前檢核確認
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/06/02  Daniel Lee Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject DoCaseProcPreConfirm(ReceiveObject obj)
    {
        return this.WFService.DoCaseProcPreConfirm(obj);
    }
　
    /// <summary>
    /// 進行處理步驟前須執行事項
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/06/02  Daniel Lee Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject DoCaseProcPre(ReceiveObject obj)
    {
        return this.WFService.DoCaseProcPre(obj);
    }
　
    /// <summary>
    /// 進行處理步驟後須執行事項
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/06/02  Daniel Lee Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject DoCaseProcAft(ReceiveObject obj)
    {
        return this.WFService.DoCaseProcAft(obj);
    }
　
    /// <summary>
    /// 進行處理步驟後須執行事項
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/06/02  Daniel Lee Create
    /// </history>
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public DeliverObject SendWorkItem(ReceiveObject obj)
    {
        return this.WFService.SendWorkItem(obj);
    }
　
    /// <summary>
    /// 取得OPN資料
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/06/12  Daniel Lee Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject ReadCaseOPN(ReceiveObject obj)
    {
        return this.WFService.ReadCaseOPN(obj);
    }
　
    /// <summary>
    /// 更新OPN資料
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2015/06/17  Daniel Lee Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject UpdateCaseOPN(ReceiveObject obj)
    {
        return this.WFService.UpdateCaseOPN(obj);
    }
　
    /// <summary>
    /// 取得案件歷程關卡資料OPN資料
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2016/03/21  Derek Chou  Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject ReadCaseOpnStep(ReceiveObject obj)
    {
        return this.WFService.ReadCaseOpnStep(obj);
    }
　
    /// <summary>
    /// 取得案件指定關卡WorkItem
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2016/07/17  Derek Chou  Create
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject GetCaseStepWorkItem(ReceiveObject obj)
    {
        return this.WFService.GetCaseStepWorkItem(obj);
    }
　
    /// <summary>
    /// 更新案件流程
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2016/12/06  Derek Chou Create CR-20161124-作業-客戶基本資料上傳 新增流程升級功能 需求單號:201701250391-00
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject MigrateProcessInstances(ReceiveObject obj)
    {
        return this.WFService.MigrateProcessInstances(obj);
    }
　
        /// <summary>
    /// 更新流程
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2016/12/06  Derek Chou Create CR-20161124-作業-客戶基本資料上傳 新增流程升級功能 需求單號:201701250391-00
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject MigrateProcess(ReceiveObject obj)
    {
        return this.WFService.MigrateProcess(obj);
    }
　
    /// <summary>
    /// 成作業--先寫入OPN,WorkItem事後再補
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <history>
    /// 2017/03/21  Derek Chou  Create TFBNCLM-14790 需求單號：201703280561-00
    /// </history>
    [WebMethod(EnableSession = true)]
    public DeliverObject PredictSendWorkItem(ReceiveObject obj)
    {
        return this.WFService.PredictSendWorkItem(obj);
    }
　
    #endregion
}