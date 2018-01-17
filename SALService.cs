using BusinessLogicLayer.Service.Interface;
using DataAccessLayer.Dao.Interface;
using DomainObject.DomainObject;
using DomainObject.DomainObject.DefinedDomainObject;
using DomainObject.DomainObject.TableDomainObject;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
　
namespace BusinessLogicLayer.Service.Implement.SAL
{
    public partial class SALService : BaseService, ISALService
    {
        #region Public Method
        /// <summary>
        /// 授變更取號開單-取得客戶名稱
        /// </summary>
        /// <param name="strParameters">參數</param>
        /// <returns></returns>
        /// <history>
        /// 2015/11/09  Nancy Chen    Create
        /// </history>
        public DeliverObject GetChgCustname(ReceiveObject objPara)
        {
            DeliverObject objDeliver = new DeliverObject();
            ArrayList arylstResult = new ArrayList();
　
            //將objParameters轉成所需的格式
            SAL41100 objParameters = JsonConvert.DeserializeObject<SAL41100>(JsonConvert.SerializeObject(objPara.Parameters));
　
            //客戶資料交易：032151-00：呼叫共用程式規格SendHTItem取得發查序號
            SendHTItemObject objSendHtItem = new SendHTItemObject();
            objSendHtItem.QCOND = new SendHTItemObject.OBJ_QCOND();
            objSendHtItem.QINFO = new SendHTItemObject.OBJ_QINFO();
            DeliverObject objDeliverSendHt = new DeliverObject();
　
            //傳入參數設定
            string strUserUnitID = objParameters.UNIT_ID,
                strUserID = objParameters.USER_ID,
                strUserName = objParameters.USER_NM,
                strCASE_NO = objParameters.CASE_NO,
                strCUST_ID = objParameters.CUST_ID;
　
            //發查03215100
            objSendHtItem.CASE_NO = strCASE_NO;
            objSendHtItem.QITEM = "03215100";
　
            //取得 QINFO
            objSendHtItem.QINFO.ORG = "B";                     //發查部門單位
            objSendHtItem.QINFO.EXEC_TYPE = "R1";              //執行類別
            objSendHtItem.QINFO.QBRANCH_ID = strUserUnitID;    //查詢者單位代號
            objSendHtItem.QINFO.QUSR_ID = strUserID;           //發查人員員編
            objSendHtItem.QINFO.QUSR_NM = strUserName;         //發查人員姓名
　
            //取得 QCOND
            objSendHtItem.QCOND.QFUNC = "0";                   //功能
            objSendHtItem.QCOND.QCUST_ID = strCUST_ID;         //客戶ID
　
            objPara.Parameters = objSendHtItem;
　
            //呼叫 SendHTItem
            objDeliverSendHt = OTHService.SendHTItem(objPara);
　
            //取得 outQSendNO
            PropertyInfo sendHtItemProp = objDeliverSendHt.Data.GetType().GetProperty("QSEND_NO");
            string outQSendNO = sendHtItemProp.GetValue(objDeliverSendHt.Data, null).ToString();
            //string outQSendNO = "HT1505290000030";
　
            //利用QSEND_NO取得TB_HT_03215100_M的CUST_NAME
            string outCustName = Convert.ToString(DynaGenDao.SelectFromDual(objPara.LogInfoObject, "(SELECT CUST_NAME FROM TB_HT_03215100_M WHERE QSEND_NO = '" + outQSendNO + "')"));
            objDeliver.Data = outCustName;
　
            return objDeliver;
        }
　
        #endregion
    }
}