package com.gss.adm.core.prod.service.provider;

import static com.gss.adm.api.enums.SchedulingStatusEnum.Completed;
import static com.gss.adm.api.enums.SchedulingStatusEnum.Started;

import java.util.Collection;
import java.util.Date;
import java.util.List;
import java.util.Optional;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang.Validate;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.data.domain.Sort.Direction;
import org.springframework.data.domain.Sort.Order;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.google.common.collect.Iterables;
import com.google.common.collect.Lists;
import com.gss.adm.api.enums.DiscoveryStepEnum;
import com.gss.adm.api.enums.ModelDefinitionIdEnum;
import com.gss.adm.api.enums.ScheduleTypeEnum;
import com.gss.adm.api.enums.SchedulingScanStatusEnum;
import com.gss.adm.api.enums.SchedulingStatusEnum;
import com.gss.adm.api.lang.DateUtils2;
import com.gss.adm.api.lang.TimeUtils2;
import com.gss.adm.api.spring.data.Orders;
import com.gss.adm.core.model.LogSchedule;
import com.gss.adm.core.model.LogScheduleStep;
import com.gss.adm.core.model.ProjectModelAttribute;
import com.gss.adm.core.model.vo.LogscheduleStatusVo;
import com.gss.adm.core.prod.dao.LogScheduleDao;
import com.gss.adm.core.prod.dao.LogScheduleStepDao;
import com.gss.adm.core.prod.dao.ProjectModelAttributeDao;
import com.gss.adm.core.prod.dao.condition.LogScheuleCondition;
import com.gss.adm.core.service.LogScheduleService;
import com.gss.adm.core.service.ProjectService;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
@Transactional
public class LogScheduleServiceImpl implements LogScheduleService {

	@Autowired
	private LogScheduleDao scheduleLogDao;

	@Autowired
	private LogScheduleStepDao scheduleLogStepDao;

	@Autowired
	private ProjectModelAttributeDao projectModelAttributeDao;

	@Autowired
	private ProjectService projectService;

	public static final long DEFAULT_FULL_EXECUTION_TIMESPENT = 86400L;

	public static final long DEFAULT_INCREMENTAL_EXECUTION_TIMESPENT = 10800L;

	public static final long DEFAULT_MATIRX_EXECUTION_TIMESPENT = 3600L;

	public static final String COMPLETETIME = "completeTime";

	@Override
	public Collection<SchedulingScanStatusEnum> getScanStatusByScheduleType(Long projectId, String scheduleId,
			ScheduleTypeEnum scheduleType) {
		if (ScheduleTypeEnum.MATRIX.equals(scheduleType)) {
			return Lists.newArrayList(SchedulingScanStatusEnum.Full, SchedulingScanStatusEnum.OnlyMatrix);
		} else if (ScheduleTypeEnum.DISCOVERY.equals(scheduleType)) {
			// 找scheduleType為Discovery的ScanStatus是完整分析 還是 差異分析
			return Lists.newArrayList(getDiscoveryScanStatus(projectId, scheduleId));
		} else {
			throw new IllegalArgumentException("ScheduleType not implemented " + scheduleType.toString() + " !");
		}
	}

	@Override
	public Long getEstimatedTimespentByScheduleType(Long projectId, String scheduleId,
			Collection<SchedulingScanStatusEnum> scanStatus, ScheduleTypeEnum scheduleType) {
		if (ScheduleTypeEnum.MATRIX.equals(scheduleType)) {
			// 取得Matrix的預估時間
			return getBuildMatrixEstimatedTimespent(projectId, scanStatus);
		} else if (ScheduleTypeEnum.DISCOVERY.equals(scheduleType)) {
			// 取得Discovery的預估時間
			return getExecuteDiscoveryEstimatedTimespent(projectId, scheduleId, scheduleType, scanStatus);
		} else {
			throw new IllegalArgumentException("ScheduleType not implemented " + scheduleType.toString() + " !");
		}

	}

	@Override
	public void deletePendingOrStartedStstusLog() {
		log.debug("[Schedule] Delete the pending and started status of schedule log.");
		List<SchedulingStatusEnum> statuses = Lists.newArrayList(SchedulingStatusEnum.Started,
				SchedulingStatusEnum.Pending);
		List<LogSchedule> logSchedules = scheduleLogDao.findByStatuses(statuses);
		scheduleLogDao.delete(logSchedules);
		scheduleLogDao.flush();
	}

	@Override
	public LogSchedule logCrossProjectPending(Long projectId, String scheduleId, String scheduleName) {

		Validate.notNull(projectId);
		Validate.notNull(scheduleId);
		Validate.notNull(scheduleName);

		List<LogSchedule> scheduleLogs = scheduleLogDao.findByStatus(projectId, scheduleId,
				Lists.newArrayList(SchedulingStatusEnum.Pending));
		LogSchedule scheduleLog;
		if (CollectionUtils.isNotEmpty(scheduleLogs)) {
			scheduleLog = Iterables.getFirst(scheduleLogs, null);
		} else {
			scheduleLog = new LogSchedule();
			scheduleLog.setProjectId(projectId);
			scheduleLog.setScheduleId(scheduleId);
			scheduleLog.setScheduleName(scheduleName);
			scheduleLog.setScheduleType(ScheduleTypeEnum.CROSS_PROJECT);
			scheduleLog.setAddingTime(new Date());

			Optional<LogSchedule> currentStartScheduleOption = scheduleLogDao.getCurrentStartingScheduleLog().stream()
					.findFirst();
			if (currentStartScheduleOption.isPresent()) {
				LogSchedule currentStartSchedule = currentStartScheduleOption.get();
				Long previousTimeSpented = getPreviousProjectEstimatedTimespent(currentStartSchedule.getProjectId());
				Date estimatedStartTime = TimeUtils2.calculateCompleteTime(new Date(), previousTimeSpented);
				scheduleLog.setEstimatedStartTime(estimatedStartTime);
			} else {
				scheduleLog.setEstimatedStartTime(new Date());
			}

			scheduleLog.setStatus(SchedulingStatusEnum.Pending);
		}

		return scheduleLogDao.saveAndFlush(scheduleLog);
	}

	@Override
	public LogSchedule logPending(Long projectId, String scheduleId, String scheduleName,
			ScheduleTypeEnum scheduleType) {

		Validate.notNull(projectId);
		Validate.notNull(scheduleId);
		Validate.notNull(scheduleName);

		if (ScheduleTypeEnum.CROSS_PROJECT.equals(scheduleType)) {
			return logCrossProjectPending(projectId, scheduleId, scheduleName);
		} else {
			LogSchedule scheduleLog = findPendingScheduleLog(projectId, scheduleId);
			if (scheduleLog == null) {
				scheduleLog = createScheduleLogInstance(projectId, scheduleId, scheduleName, scheduleType);
			}

			return scheduleLogDao.saveAndFlush(scheduleLog);
		}
	}

	@Override
	public LogSchedule logStarted(Long projectId, String scheduleId, String scheduleName,
			ScheduleTypeEnum scheduleType) {

		Validate.notNull(projectId);
		Validate.notNull(scheduleId);
		Validate.notNull(scheduleName);

		LogSchedule scheduleLog = findPendingScheduleLog(projectId, scheduleId);

		Validate.notNull(scheduleLog);

		scheduleLog.setStatus(SchedulingStatusEnum.Started);
		scheduleLog.setStartTime(new Date());

		scheduleLogDao.saveAndFlush(scheduleLog);
		return getLastExecutingLog(projectId, scheduleId, scheduleType);
	}

	@Override
	public LogSchedule logComplete(Long projectId, String scheduleId, long scheduleLogId, Boolean isFullScan) {

		LogSchedule scheduleLog = findScheduleByStatusAndScheduleLogId(projectId, scheduleLogId, Started);

		if (scheduleLog == null) {
			log.debug("Schedule log is null, cause schedule log id can not find in database.");
			return null;
		}

		Date now = new Date();
		Long timespent = DateUtils2.getTimespent(scheduleLog.getStartTime(), now);

		scheduleLog.setCompleteTime(now);
		scheduleLog.setTimespent(timespent / 1000);
		scheduleLog.setStatus(Completed);

		if (isFullScan == null) {
			scheduleLog.setScanStatus(SchedulingScanStatusEnum.Unknown);
		} else if (isFullScan) {
			scheduleLog.setScanStatus(SchedulingScanStatusEnum.Full);
		} else {
			scheduleLog.setScanStatus(SchedulingScanStatusEnum.Incremental);
		}

		return scheduleLogDao.saveAndFlush(scheduleLog);
	}

	@Override
	public void updateEstimatedStartTimeAllPendingStatusLogs(LogSchedule currentExecutingLogSchedule) {
		if (currentExecutingLogSchedule == null) {
			return;
		}

		ScheduleTypeEnum scheduleType = currentExecutingLogSchedule.getScheduleType();
		// 找pending以及跟自己相同的scheduleType 的logSchedules
		List<LogSchedule> pendingLogSchedules = scheduleLogDao
				.findPendingOrStartedByStatusAndScheduleType(SchedulingStatusEnum.Pending, scheduleType);

		// 第一個人正在Peding的人 預估開始時間 更正成logSchedule的完成時間
		Date newEstimatedStartTime = currentExecutingLogSchedule.getCompleteTime();
		for (LogSchedule pendingLogSchedule : pendingLogSchedules) {
			pendingLogSchedule.setEstimatedStartTime(newEstimatedStartTime);

			Long projectId = pendingLogSchedule.getProjectId();
			String scheduleId = pendingLogSchedule.getScheduleId();
			Collection<SchedulingScanStatusEnum> scanStatus = getScanStatusByScheduleType(projectId, scheduleId,
					scheduleType);

			long logScheduleLatestTimeSpent = getEstimatedTimespentByScheduleType(projectId, scheduleId, scanStatus,
					scheduleType);

			// 下一筆logSchedule估計開始時間 = 上一個logSchedule的完成時間(newEstimatedStartTime +
			// 該logSchedule最近一筆的歷史花費時間)
			newEstimatedStartTime = TimeUtils2.calculateCompleteTime(newEstimatedStartTime, logScheduleLatestTimeSpent);
		}

		scheduleLogDao.save(pendingLogSchedules);
		scheduleLogDao.flush();
	}

	@Override
	public LogSchedule logFailure(Long projectId, String scheduleId, long scheduleLogId, String exception) {

		LogSchedule scheduleLog = findScheduleByStatusAndScheduleLogId(projectId, scheduleLogId, Started);

		if (scheduleLog == null) {
			log.debug("Schedule log is null, cause schedule log id can not find in database.");
			return null;
		}

		Date now = new Date();
		Long timespent = DateUtils2.getTimespent(scheduleLog.getStartTime(), now);

		scheduleLog.setCompleteTime(now);
		scheduleLog.setTimespent(timespent / 1000);
		scheduleLog.setStatus(SchedulingStatusEnum.Failure);
		scheduleLog.setComment(exception);

		return scheduleLogDao.saveAndFlush(scheduleLog);
	}

	@Override
	public List<LogSchedule> findScheduleLog(Long projectId) {
		return scheduleLogDao.findByProjectId(projectId);
	}

	@Override
	public LogSchedule getLastExecutingLog(final long projectId, final String scheduleId,
			ScheduleTypeEnum scheduleType) {
		List<SchedulingStatusEnum> statuses = Lists.newArrayList(SchedulingStatusEnum.Pending,
				SchedulingStatusEnum.Started);
		Sort sort = Orders.orderBy("estimatedStartTime", Direction.DESC);
		PageRequest pageable = new PageRequest(0, 1, sort);

		Page<LogSchedule> result = scheduleLogDao.findLogSchedulesByProjectIdAndScheduleIdAndStatusAndScheduleType(
				projectId, scheduleId, statuses, scheduleType, pageable);
		return Iterables.getFirst(result, null);
	}

	@Override
	public List<LogSchedule> getAllPendingAndStartedScheduleLogByStatusesAndScheduleType(
			Collection<SchedulingStatusEnum> statuses, ScheduleTypeEnum scheduleType) {
		return scheduleLogDao.findScheduleLogByStatusesAndScheduleType(statuses, scheduleType);
	}

	@Override
	public LogSchedule getScheduleLogById(Long scheduleLogId) {
		return scheduleLogDao.getOne(scheduleLogId);
	}

	@Override
	public void updateLogSchedule(LogSchedule scheduleLog) {
		scheduleLogDao.saveAndFlush(scheduleLog);
	}

	@Override
	public List<LogSchedule> findLastTwoSchedulesByScheduleingScanStatuses(long projectId,
			Collection<SchedulingScanStatusEnum> schedulingScanStatuses) {
		Order order = new Order(Direction.DESC, "startTime");
		Sort sort = new Sort(order);
		PageRequest page = new PageRequest(0, 2, sort);

		Page<LogSchedule> pagedSchedules = scheduleLogDao.findLastTwoSchedulesByScheduleingScanStatus(projectId,
				schedulingScanStatuses, page);
		return pagedSchedules.getContent();
	}

	@Override
	public LogSchedule getLastScheduleBeforeLogScheduleId(long projectId, long logScheduleId) {
		Order order = new Order(Direction.DESC, "startTime");
		Sort sort = new Sort(order);
		PageRequest page = new PageRequest(0, 2, sort);

		Page<LogSchedule> pagedSchedules = scheduleLogDao.findLastTwoSchedules(projectId, page);
		List<LogSchedule> lastTwoSchedules = pagedSchedules.getContent();
		if (lastTwoSchedules.size() < 2) {
			return null;
		} else {
			LogSchedule currentSchedule = lastTwoSchedules.get(0);
			if (currentSchedule.getId().equals(logScheduleId)) {
				return lastTwoSchedules.get(1);
			}

			return null;
		}
	}

	@Override
	public Long getPreviousProjectEstimatedTimespent(Long projectId) {
		LogSchedule targetLogSchedule = findLatestCompletedByProjectIdAndScheduleIdAndScanStatuses(projectId,
				Lists.newArrayList(SchedulingScanStatusEnum.Full, SchedulingScanStatusEnum.Incremental));

		if (targetLogSchedule != null) {
			return targetLogSchedule.getTimespent();
		} else {
			return DEFAULT_FULL_EXECUTION_TIMESPENT;
		}
	}

	@Override
	public List<LogSchedule> findScheduleLogByProjectIdAndScheduleIdAndScheduleStatus(Long projectId, String scheduleId,
			Collection<SchedulingStatusEnum> schedulingStatus) {
		return scheduleLogDao.findByStatus(projectId, scheduleId, schedulingStatus);
	}
	
	@Override
	public Page<LogSchedule> findLatestLogSchedule(long projectId, Pageable pageable) {

		LogScheuleCondition logScheuleCondition = new LogScheuleCondition();
		logScheuleCondition.setProjectId(projectId);

		Specification<LogSchedule> specification = logScheuleCondition.getSpecification();

		return scheduleLogDao.findAll(specification, pageable);
	}
	
	@Override
	public List<LogscheduleStatusVo> findLogSchedulesStatusVoWithoutPendingAndStarted(List<Long> projectIds){
		List<SchedulingStatusEnum> schedulingStatus = Lists.newArrayList(SchedulingStatusEnum.Completed, SchedulingStatusEnum.Failure);
		
		return scheduleLogDao.findLogscheduleStatusVoByProjectIdsAndStatus(projectIds, schedulingStatus);
	}
	
	private SchedulingScanStatusEnum getDiscoveryScanStatus(Long projectId, String scheduleId) {
		// 畫面上的差異分析是否有打勾
		Long arrtibuteScheduleScanStatusId = ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_SCHEDULE_SCAN_STATUS_ID
				.getValue();
		ProjectModelAttribute projectModelAttribute = projectModelAttributeDao.getByAttributeDefinitionId(projectId,
				arrtibuteScheduleScanStatusId);
		Boolean isIncrementalScan = Boolean.valueOf(projectModelAttribute.getAttributeValue());

		// 分析工具是否有異動
		boolean isDiscoveryComplete = projectService.getProjectById(projectId).getProperties().isDiscoveryComplete();

		List<SchedulingStatusEnum> statuses = Lists.newArrayList(SchedulingStatusEnum.Completed,
				SchedulingStatusEnum.Failure);
		Sort sort = Orders.orderBy(COMPLETETIME, Direction.DESC);
		PageRequest pageable = new PageRequest(0, 1, sort);
		// 找排程狀態是完成或失敗(scheduleType為DISCOVERY)
		Page<LogSchedule> previousLogSchedules = scheduleLogDao
				.findLogSchedulesByProjectIdAndScheduleIdAndStatusAndScheduleType(projectId, scheduleId, statuses,
						ScheduleTypeEnum.DISCOVERY, pageable);
		LogSchedule previousLogSchedule = Iterables.getFirst(previousLogSchedules, null);

		// 之前沒有跑過排程 或 之前排程是失敗的 或 網頁上沒有勾選差異分析 或 分析工具有異動 皆需要執行完整分析
		boolean isFullScan = previousLogSchedule == null
				|| previousLogSchedule.getStatus() == SchedulingStatusEnum.Failure || !isIncrementalScan
				|| !isDiscoveryComplete;
		if (isFullScan) {
			return SchedulingScanStatusEnum.Full;
		}
		return SchedulingScanStatusEnum.Incremental;
	}

	private LogSchedule createScheduleLogInstance(Long projectId, String scheduleId, String scheduleName,
			ScheduleTypeEnum scheduleType) {

		LogSchedule logSchedule = new LogSchedule();
		logSchedule.setProjectId(projectId);
		logSchedule.setScheduleId(scheduleId);
		logSchedule.setScheduleName(scheduleName);
		logSchedule.setScheduleType(scheduleType);
		logSchedule.setAddingTime(new Date());

		Date estimatedStartTime = getEestimatedStartTime(scheduleType);
		logSchedule.setEstimatedStartTime(estimatedStartTime);

		logSchedule.setStatus(SchedulingStatusEnum.Pending);
		return logSchedule;
	}

	private Date getEestimatedStartTime(ScheduleTypeEnum scheduleType) {
		// 根據scheduleType，找整個資料庫中是 Pending 或 Started的排程
		List<SchedulingStatusEnum> statuses = Lists.newArrayList(SchedulingStatusEnum.Pending,
				SchedulingStatusEnum.Started);
		Sort sort = Orders.orderBy("estimatedStartTime", Direction.DESC);
		PageRequest pageable = new PageRequest(0, 1, sort);

		Page<LogSchedule> pendingOrStartedLogSchedulesByScheduleType = scheduleLogDao
				.findPendingOrStartedByStatusAndScheduleType(statuses, scheduleType, pageable);
		LogSchedule pendingOrStartedLogScheduleByScheduleType = Iterables
				.getFirst(pendingOrStartedLogSchedulesByScheduleType, null);

		Date currentEstimatedStartTime = new Date(); // 沒有的話 估計時間先用現在的時間
		Long currentEstimatedTimespent;
		// 有排程的話 當前專案估計開始時間 = 正在運行排程的專案最新一筆 Completed 的花費時間
		if (pendingOrStartedLogScheduleByScheduleType != null) {
			Long projectId = pendingOrStartedLogScheduleByScheduleType.getProjectId();
			String scheduleId = pendingOrStartedLogScheduleByScheduleType.getScheduleId();
			Collection<SchedulingScanStatusEnum> scanStatus = getScanStatusByScheduleType(projectId, scheduleId,
					scheduleType);

			currentEstimatedTimespent = getEstimatedTimespentByScheduleType(projectId, scheduleId, scanStatus,
					scheduleType);

			// 當前logSchedule估計開始時間 = 上一筆logSchedule的估計完成時間(previousLogSchedule
			// 的開始時間＋上面求出來的估計花費時間)
			Date previousEstimatedStartTime = pendingOrStartedLogScheduleByScheduleType.getEstimatedStartTime();
			currentEstimatedStartTime = TimeUtils2.calculateCompleteTime(previousEstimatedStartTime,
					currentEstimatedTimespent);
		}
		return currentEstimatedStartTime;
	}

	private Long getDiscoveryDefaultEstimatedTimespent(Collection<SchedulingScanStatusEnum> scanStatuses) {
		SchedulingScanStatusEnum scanStatus = Iterables.getFirst(scanStatuses, null);
		if (scanStatus == SchedulingScanStatusEnum.Incremental) {
			return DEFAULT_INCREMENTAL_EXECUTION_TIMESPENT;
		}
		return DEFAULT_FULL_EXECUTION_TIMESPENT;
	}

	private Long getBuildMatrixEstimatedTimespent(Long projectId, Collection<SchedulingScanStatusEnum> scanStatuses) {
		LogSchedule latestCompletedLogSchedule = findLatestCompletedByProjectIdAndScheduleIdAndScanStatuses(projectId,
				scanStatuses);

		Long estimatedTimespent = DEFAULT_MATIRX_EXECUTION_TIMESPENT;
		if (latestCompletedLogSchedule != null) {
			SchedulingScanStatusEnum scanStatus = latestCompletedLogSchedule.getScanStatus();
			if (scanStatus == SchedulingScanStatusEnum.OnlyMatrix) {
				// 若最近的是OnlyMatrix 則直接取time spent
				estimatedTimespent = latestCompletedLogSchedule.getTimespent();
			} else {
				// 若是full 則去logScheduleStep找 Generate Indirect Matrix 的花費時間
				String stepName = DiscoveryStepEnum.GENERATE_INDIRECT_MATRIX.getStepName();
				LogScheduleStep logScheduleStep = findLatestLogScheduleStepByLogScheduleAndStepName(
						latestCompletedLogSchedule, stepName);
				if (logScheduleStep != null) {
					estimatedTimespent = logScheduleStep.getTimespent();
				}
			}
		}
		return estimatedTimespent;
	}

	private Long getExecuteDiscoveryEstimatedTimespent(Long projectId, String scheduleId, ScheduleTypeEnum scheduleType,
			Collection<SchedulingScanStatusEnum> scanStatuses) {
		LogSchedule latestCompletedLogSchedule = findLatestCompletedByProjectIdAndScheduleIdAndScanStatusAndScheduleType(
				projectId, scheduleId, scanStatuses, scheduleType);

		Long estimatedTimespent = getDiscoveryDefaultEstimatedTimespent(scanStatuses);
		if (latestCompletedLogSchedule != null) {
			estimatedTimespent = latestCompletedLogSchedule.getTimespent();
		}
		return estimatedTimespent;
	}

	private LogScheduleStep findLatestLogScheduleStepByLogScheduleAndStepName(LogSchedule logSchedule,
			String stepName) {
		Long logId = logSchedule.getId();
		Sort sort = Orders.orderBy(COMPLETETIME, Direction.DESC);
		PageRequest pageable = new PageRequest(0, 1, sort);
		Page<LogScheduleStep> logScheduleStep = scheduleLogStepDao.findByLogIdAndStepName(logId, stepName, pageable);
		return Iterables.getFirst(logScheduleStep, null);
	}

	private LogSchedule findLatestCompletedByProjectIdAndScheduleIdAndScanStatuses(Long projectId,
			Collection<SchedulingScanStatusEnum> scanStatuses) {
		Sort sort = Orders.orderBy(COMPLETETIME, Direction.DESC);
		PageRequest pageable = new PageRequest(0, 1, sort);
		Page<LogSchedule> latestCompletedLogSchedules = scheduleLogDao
				.findLogScheduleByProjectIdAndScheduleIdAndScanStatus(projectId,
						Lists.newArrayList(SchedulingStatusEnum.Completed), scanStatuses, pageable);
		return Iterables.getFirst(latestCompletedLogSchedules, null);
	}

	private LogSchedule findLatestCompletedByProjectIdAndScheduleIdAndScanStatusAndScheduleType(Long projectId,
			String scheduleId, Collection<SchedulingScanStatusEnum> scanStatus, ScheduleTypeEnum scheduleType) {
		Sort sort = Orders.orderBy(COMPLETETIME, Direction.DESC);
		PageRequest pageable = new PageRequest(0, 1, sort);
		Page<LogSchedule> latestCompletedLogSchedules = scheduleLogDao
				.findByProjectIdAndScheduleIdAndStatusAndScanStatusAndScheduleType(projectId, scheduleId,
						SchedulingStatusEnum.Completed, scanStatus, scheduleType, pageable);
		return Iterables.getFirst(latestCompletedLogSchedules, null);
	}

	private LogSchedule findScheduleByStatusAndScheduleLogId(long projectId, long scheduleLogId,
			SchedulingStatusEnum status) {
		return scheduleLogDao.findByStatusAndScheduleLogId(projectId, scheduleLogId, status);
	}

	protected LogSchedule findPendingScheduleLog(long projectId, String scheduleId) {
		List<LogSchedule> scheduleLogs = scheduleLogDao.findByStatus(projectId, scheduleId,
				Lists.newArrayList(SchedulingStatusEnum.Pending));

		return Iterables.getFirst(scheduleLogs, null);
	}

	protected LogSchedule findScheduleLogByStatus(long projectId, String scheduleId, SchedulingStatusEnum status) {
		List<LogSchedule> scheduleLogs = scheduleLogDao.findByStatus(projectId, scheduleId, Lists.newArrayList(status));
		return Iterables.getFirst(scheduleLogs, null);
	}
}
