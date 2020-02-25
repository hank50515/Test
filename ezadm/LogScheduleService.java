package com.gss.adm.core.service;

import java.util.Collection;
import java.util.List;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import com.gss.adm.api.enums.ScheduleTypeEnum;
import com.gss.adm.api.enums.SchedulingScanStatusEnum;
import com.gss.adm.api.enums.SchedulingStatusEnum;
import com.gss.adm.core.model.LogSchedule;
import com.gss.adm.core.model.vo.LogscheduleStatusVo;

public interface LogScheduleService {

	void deletePendingOrStartedStstusLog();

	LogSchedule getScheduleLogById(Long scheduleLogId);
	
	void updateLogSchedule(LogSchedule scheduleLog);

	/**
	 * 新增跨專案排程紀錄為 Pending 並且更新相關時間資訊。若系統因重開而已存在相關排程紀錄，則更新此排程紀錄。
	 * 
	 * @param projectId
	 * @param scheduleId
	 * @param scheduleName
	 * @return LogSchedule
	 */
	LogSchedule logCrossProjectPending(Long projectId, String scheduleId, String scheduleName);
	
	LogSchedule logPending(Long projectId, String scheduleId, String scheduleName, ScheduleTypeEnum scheduleType);

	LogSchedule logStarted(Long projectId, String scheduleId, String scheduleName, ScheduleTypeEnum scheduleType);

	LogSchedule logComplete(Long projectId, String scheduleId, long scheduleLogId, Boolean isFullScan);

	void updateEstimatedStartTimeAllPendingStatusLogs(LogSchedule scheduleLog);
	
	LogSchedule logFailure(Long projectId, String scheduleId, long scheduleLogId, String exception);

	List<LogSchedule> findScheduleLog(Long projectId);

	LogSchedule getLastExecutingLog(long projectId, String scheduleId, ScheduleTypeEnum scheduleType);
	
	List<LogSchedule> getAllPendingAndStartedScheduleLogByStatusesAndScheduleType(Collection<SchedulingStatusEnum> statuses, ScheduleTypeEnum scheduleType);

    Collection<SchedulingScanStatusEnum> getScanStatusByScheduleType(Long projectId, String scheduleId, ScheduleTypeEnum scheduleType);
    
    Long getEstimatedTimespentByScheduleType(Long projectId, String scheduleId, Collection<SchedulingScanStatusEnum> scanStatus, ScheduleTypeEnum scheduleType);

	public List<LogSchedule> findLastTwoSchedulesByScheduleingScanStatuses(long projectId,
			Collection<SchedulingScanStatusEnum> schedulingScanStatuses);
	
	public LogSchedule getLastScheduleBeforeLogScheduleId(long projectId, long logScheduleId);

	/**
	 * 找出上一次成功解析完成的跨專案排程所花費的時間長度
	 * @param projectId
	 * @param scheduleId
	 * @return Long
	 */
	Long getPreviousProjectEstimatedTimespent(Long projectId);

	List<LogSchedule> findScheduleLogByProjectIdAndScheduleIdAndScheduleStatus(Long projectId, String scheduleId,
			Collection<SchedulingStatusEnum> schedulingStatus);
	
	// FIXME: 改 return Optional<LogSchedule>
	public Page<LogSchedule> findLatestLogSchedule(long projectId, Pageable pageable);
	
	/**
	 * 根據 projectIds 找出所有的完成或失敗的 schedule，並只拿出它們的狀態跟 id 還有 projectId
	 * @param projectId
	 * @return LogscheduleStatusVo
	 */
	List<LogscheduleStatusVo> findLogSchedulesStatusVoWithoutPendingAndStarted(List<Long> projectId);
}
