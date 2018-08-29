//-----------------------------------------------------------------------
// <copyright file="HandleService.cs" company="Galaxy Software Service">
// Copyright (c) Galaxy Software Service. All rights reserved.
// </copyright>
// <Summery>
//  模組: 事故管理模組
//  功能: 後台-退回轉派
// </summery>
// <history>
// [Date]           [Action]    [Coding]        [Description]
// 2017/01/11       New       Darren               Incident
// </history>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSS.ITSM.Core.Production.WorkFlow;
using GSS.ITSM.Model.DTO;
using GSS.ITSM.Model.Entities;
using GSS.ITSM.Model.Entities.Incident;
using GSS.ITSM.ToolKit;
using GSS.ITSM.WorkFlow.IncidentServices;
using GSS.ITSM.Dal.Interfaces;
using KendoGridBinder;
using GSS.ITSM.Domain.Incident.Interface;
using GSS.ITSM.Globalization;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
　
namespace GSS.ITSM.Domain.Incident.Service
{
    public class IncidentAlertMaintainService : BaseService, IIncidentAlertMaintainService
    {
		public AbstractBaseRepository BaseRepo { get; set; }
		
        #region IoC Service
        public AbstractItsmCcMRepository ItsmCcMRepo { get; set; }
        #endregion
　
        /// <summary>
        /// 初始化查詢畫面
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IncidentAlertMaintainDTO LoadQueryPartial(IncidentAlertMaintainDTO dto)
        {
            return dto;
        }
　
        /// <summary>
        /// 撈取結果畫面GridData
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public KendoGrid<IncidentAlertMaintainGridDTO> LoadQueryGridData(IncidentAlertMaintainGridDTO dto)
        {
            return this.ItsmCcMRepo.LoadQueryGridData(dto);
        }
　
        /// <summary>
        /// 警訊通知存檔
        /// </summary>
        /// <param name="dto"></param>
        public void SaveAlertNotice(IncidentAlertMaintainDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.SchSerilNo))
            {
                //// 先將SchSerilNo用逗號做分隔成string[]，取出個別的SchSerilNo
                var schSerilNoList = dto.SchSerilNo.Split(',');
                var CcTypeList = dto.CcType.Split(',');
　
                for (int i = 0; i < schSerilNoList.Count(); i++)
                {
                    //// 檢查有沒有存檔過
                    var itsmCcM = base.BaseRepo.LoadSingle<ItsmCcM>(base.GetCondition(new ItsmCcM { CcUsrId = dto.CcUsrId, SchSerilNo = schSerilNoList[i], TenantId = dto.TenantId }));
　
                    //// 有存檔過則更新，否則新增
                    if (itsmCcM != null)
                    {
                        itsmCcM.CcType = CcTypeList[i];
                        itsmCcM.ModDte = dto.ModDte.Value;
                        itsmCcM.ModUsr = dto.ModUsr;
　
                    }
                    else
                    {
                        var data = new ItsmCcM()
                        {
                            CcSerilNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 19),//TODO CHECK GUID
                            CcType = CcTypeList[i],
                            CcUsrType = "P",
                            CcUsrId = dto.CcUsrId,
                            CcOuId = string.Empty,
                            CcRolId = string.Empty,
                            SchSerilNo = schSerilNoList[i],
                            CreDte = dto.CreDte.Value,
                            CreUsr = dto.CreUsr,
                            ModDte = dto.ModDte.Value,
                            ModUsr = dto.ModUsr,
                            TenantId = dto.TenantId
                        };
　
                        base.BaseRepo.Add(data);
                    }
                }
            }
        }
　
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="dto"></param>
        public void DeleteData(IncidentAlertMaintainDTO dto)
        {
            var itsmCcMs = base.BaseRepo.Load<ItsmCcM>(base.GetCondition(new ItsmCcM { CcUsrId = dto.CcUsrId}));
　
            if (itsmCcMs.Count == 0)
            {
                throw new Exception("ItsmCcM is not exist");
            }
            else
            {
                base.BaseRepo.Delete<ItsmCcM>(new Dictionary<string, object>() {
                                                              {"CcUsrId",dto.CcUsrId}
                                                          });            
            }
        }
　
        /// <summary>
        /// 撈取編輯畫面GridData
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public KendoGrid<IncidentAlertMaintainGridDTO> LoadEditGridData(IncidentAlertMaintainGridDTO dto)
        {
            return this.ItsmCcMRepo.LoadEditGridData(dto);
        }
　
        /// <summary>
        /// 新增(編輯)畫面
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IncidentAlertMaintainDTO LoadEditPartial(IncidentAlertMaintainDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.CcUsrId)) 
            {
                var usrData = base.BaseRepo.LoadSingle<Scuserm>(base.GetCondition(new Scuserm
                {
                    UsrId = dto.CcUsrId,
                    UsrCompId = dto.TenantId
                }));
　
                dto.CcUsrIdName = string.Format("{0} ({1})", usrData.UsrName, usrData.UsrId);
            }
            return dto;
        }
　
        #region PrivateFunction
　
        #endregion
    }
}