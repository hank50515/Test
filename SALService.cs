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
    public partial class SALService
    {
        
            string outCustName = Convert.ToString(DynaGenDao.SelectFromDual(objPara.LogInfoObject, "(SELECT CUST_NAME FROM TB_HT_03215100_M WHERE QSEND_NO = '" + outQSendNO + "')"));
　
            return outCustName;
        }
　
        #endregion
    }
}