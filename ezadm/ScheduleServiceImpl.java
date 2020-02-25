package com.gss.adm.core.prod.service.provider;

import java.util.Collection;
import java.util.Date;
import java.util.List;
import java.util.Optional;

import javax.ws.rs.BadRequestException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.google.common.collect.Lists;
import com.gss.adm.api.constant.ModelDefinitionIds;
import com.gss.adm.api.enums.ScheduleTypeEnum;
import com.gss.adm.api.enums.SchedulingScanStatusEnum;
import com.gss.adm.api.enums.SchedulingStatusEnum;
import com.gss.adm.api.lang.TimeUtils2;
import com.gss.adm.api.spring.security.AuthenticationUtils;
import com.gss.adm.api.utils.CollectionUtils2;
import com.gss.adm.api.validation.ValidatedMessage;
import com.gss.adm.api.validation.ValidationException;
import com.gss.adm.core.model.LogSchedule;
import com.gss.adm.core.model.Project;
import com.gss.adm.core.model.ProjectModel;
import com.gss.adm.core.model.ScheduleInfo;
import com.gss.adm.core.prod.schedule.AdmScheduleInfoBuilder;
import com.gss.adm.core.prod.schedule.CiDiscoveryServiceFacade;
import com.gss.adm.core.prod.schedule.CrossProjectDiscoveryFacade;
import com.gss.adm.core.prod.schedule.MatrixServiceFacade;
import com.gss.adm.core.service.LogScheduleService;
import com.gss.adm.core.service.ProjectModelService;
import com.gss.adm.core.service.ProjectService;
import com.gss.adm.core.service.ScheduleService;

@Service
public class ScheduleServiceImpl implements ScheduleService, ModelDefinitionIds {

	private static final int UNIT_MILLISECONDS = 1000;

	private static final String BUILD_MATRIX = "建立矩陣";

	@Autowired
	private LogScheduleService scheduleLogService;

	@Autowired
	private ProjectModelService projectModelService;

	@Autowired
	private ProjectService projectService;

	@Override
	public ScheduleInfo executeSchedule(Long projectId) {

		if (projectId == null) {
			throw new IllegalArgumentException("project can not be empty.");
		}

		ProjectModel scheduleProjectModel = projectModelService
				.findProjectModelByProjectIdAndModelDefinitionId(projectId, PROJECT_MODEL_SCHEDULE_ID).get(0);
		if (scheduleProjectModel == null) {
			throw new IllegalArgumentException("Schedule model can not be empty.");
		}

		return this.executeSchedule(scheduleProjectModel);
	}

	@Override
	public ScheduleInfo executeCrossProjectScheduleImmediately(Long projectId) {
		ProjectModel scheduleProjectModel = projectModelService
				.findProjectModelByProjectIdAndModelDefinitionId(projectId, PROJECT_MODEL_CROSS_PROJECT_SCHEDULE_ID)
				.get(0);
		if (scheduleProjectModel == null) {
			throw new IllegalArgumentException("Schedule model can not be empty.");
		}

		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		String scheduleId = AdmScheduleInfoBuilder.getScheduleId(scheduleProjectModel);
		String scheduleName = AdmScheduleInfoBuilder.getScheduleName(scheduleProjectModel);

		CrossProjectDiscoveryFacade crossProjectDiscoveryService = new CrossProjectDiscoveryFacade();
		crossProjectDiscoveryService.setProjectId(projectId);
		crossProjectDiscoveryService.setUsername(username);
		crossProjectDiscoveryService.setScheduleId(scheduleId);
		crossProjectDiscoveryService.setScheduleName(scheduleName);

		try {
			crossProjectDiscoveryService.excuteCrossImmediately();
		} catch (ValidationException e) {
			ValidatedMessage validateMessage = e.getValidateMessage();
			if (validateMessage.getMessage().equals(CrossProjectDiscoveryFacade.CURRENT_SCHEDULE_IS_CORSS_PROJECT)) {
				throw e;
			} else if (validateMessage.getMessage()
					.equals(CrossProjectDiscoveryFacade.NEXT_SCHEDULE_IS_CORSS_PROJECT)) {
				Optional<LogSchedule> logScheduleOption = getNextCrossProjectScheduleInfoByProjectIdAndScheduleId(
						projectId, scheduleId);
				if (logScheduleOption.isPresent()) {
					// 1. calculate estimated start time.
					Date estimatedStartTime = logScheduleOption.get().getEstimatedStartTime();

					// 2. calculate estimated timespent.
					Long estimatedTimespent = scheduleLogService.getPreviousProjectEstimatedTimespent(projectId);

					// 3. calculate estimated complete time.
					Date estimatedCompleteTime = TimeUtils2.calculateCompleteTime(estimatedStartTime,
							estimatedTimespent);

					return getScheduleInfo(logScheduleOption.get(), estimatedCompleteTime, estimatedTimespent);
				} else {
					throw new BadRequestException(e);
				}
			}
		}

		try {
			Thread.sleep(1000);
		} catch (InterruptedException e) {
			Thread.currentThread().interrupt();
		}

		return getCrossProjectScheduleInfoByProjectIdAndScheduleId(projectId, scheduleId);
	}

	@Override
	public ScheduleInfo executeSchedule(ProjectModel configuration) {
		Long projectId = configuration.getProject().getId();
		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		String scheduleId = AdmScheduleInfoBuilder.getScheduleId(configuration);
		String scheduleName = AdmScheduleInfoBuilder.getScheduleName(configuration);

		CiDiscoveryServiceFacade discoveryService = new CiDiscoveryServiceFacade();
		discoveryService.setProjectId(projectId);
		discoveryService.setUsername(username);
		discoveryService.setScheduleId(scheduleId);
		discoveryService.setScheduleName(scheduleName);
		discoveryService.execute();

		try {
			Thread.sleep(1000);
		} catch (InterruptedException e) {
			Thread.currentThread().interrupt();
		}

		return getScheduleInfoByProjectIdAndScheduleIdAndScheduleType(projectId, scheduleId,
				ScheduleTypeEnum.DISCOVERY);
	}

	@Override
	public ScheduleInfo executeBuildMatrixSchedule(ProjectModel configuration) {
		Long projectId = configuration.getProject().getId();
		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		String scheduleId = AdmScheduleInfoBuilder.getMatrixScheduleId(configuration);

		MatrixServiceFacade matrixService = new MatrixServiceFacade();
		matrixService.setProjectId(projectId);
		matrixService.setUsername(username);
		matrixService.setScheduleId(scheduleId);
		matrixService.setScheduleName(BUILD_MATRIX);
		matrixService.execute();

		try {
			Thread.sleep(1000);
		} catch (InterruptedException e) {
			Thread.currentThread().interrupt();
		}

		return getScheduleInfoByProjectIdAndScheduleIdAndScheduleType(projectId, scheduleId, ScheduleTypeEnum.MATRIX);
	}

	@Override
	public List<ScheduleInfo> getAllScheduleInfo() {
		return getAllPendingAndStartedScheduleInfosByScheduleType(ScheduleTypeEnum.DISCOVERY);
	}

	@Override
	public List<ScheduleInfo> getAllMatrixInfo() {
		return getAllPendingAndStartedScheduleInfosByScheduleType(ScheduleTypeEnum.MATRIX);
	}

	@Override
	public ScheduleInfo getScheduleInfoByProjectIdAndScheduleIdAndScheduleType(Long projectId, String scheduleId,
			ScheduleTypeEnum scheduleType) {
		LogSchedule scheduleLog = scheduleLogService.getLastExecutingLog(projectId, scheduleId, scheduleType);
		if (scheduleLog == null) {
			return null;
		}
		// 1. calculate estimated start time.
		Date estimatedStartTime = scheduleLog.getEstimatedStartTime();

		// 2. calculate estimated timespent.
		Long estimatedTimespent = getPreviousEstimatedTimespent(scheduleLog, scheduleType);

		// 3. calculate estimated complete time.
		Date estimatedCompleteTime = TimeUtils2.calculateCompleteTime(estimatedStartTime, estimatedTimespent);

		return getScheduleInfo(scheduleLog, estimatedCompleteTime, estimatedTimespent);
	}

	private ScheduleInfo getCrossProjectScheduleInfoByProjectIdAndScheduleId(Long projectId, String scheduleId) {
		LogSchedule scheduleLog = scheduleLogService.getLastExecutingLog(projectId, scheduleId,
				ScheduleTypeEnum.CROSS_PROJECT);
		if (scheduleLog == null) {
			return null;
		}
		// 1. calculate estimated start time.
		Date estimatedStartTime = scheduleLog.getEstimatedStartTime();

		// 2. calculate estimated timespent.
		Long estimatedTimespent = scheduleLogService.getPreviousProjectEstimatedTimespent(projectId);

		// 3. calculate estimated complete time.
		Date estimatedCompleteTime = TimeUtils2.calculateCompleteTime(estimatedStartTime, estimatedTimespent);

		return getScheduleInfo(scheduleLog, estimatedCompleteTime, estimatedTimespent);
	}

	private List<ScheduleInfo> getAllPendingAndStartedScheduleInfosByScheduleType(ScheduleTypeEnum scheduleType) {
		List<ScheduleInfo> scheduleInfoList = Lists.newArrayList();
		List<SchedulingStatusEnum> status = Lists.newArrayList(SchedulingStatusEnum.Started,
				SchedulingStatusEnum.Pending);
		// 根據scheduleType拿出全部pending和started的logSchedule
		List<LogSchedule> pendingAndStartedLogSchedulesByDiscovery = scheduleLogService
				.getAllPendingAndStartedScheduleLogByStatusesAndScheduleType(status, scheduleType);

		if (CollectionUtils2.isEmpty(pendingAndStartedLogSchedulesByDiscovery)) {
			return scheduleInfoList;
		}

		// 跑for loop，後面一筆去減前面一筆 可以得到估計花費時間
		for (int i = 0; i < pendingAndStartedLogSchedulesByDiscovery.size(); i++) {
			LogSchedule currentLogSchedule = pendingAndStartedLogSchedulesByDiscovery.get(i);
			Date currentEstimatedStartTime = currentLogSchedule.getEstimatedStartTime();

			Long estimatedTimespent;
			if (i == pendingAndStartedLogSchedulesByDiscovery.size() - 1) {
				// 計算list中最後一個logSchedule的估計花費時間
				estimatedTimespent = getPreviousEstimatedTimespent(currentLogSchedule, scheduleType);
			} else {
				LogSchedule nextLogSchedule = pendingAndStartedLogSchedulesByDiscovery.get(i + 1);
				Date nextEstimatedStartTime = nextLogSchedule.getEstimatedStartTime();
				estimatedTimespent = (nextEstimatedStartTime.getTime() - currentEstimatedStartTime.getTime())
						/ UNIT_MILLISECONDS;
			}
			Date estimatedCompleteTime = TimeUtils2.calculateCompleteTime(currentEstimatedStartTime,
					estimatedTimespent);

			ScheduleInfo scheduleInfo = getScheduleInfo(currentLogSchedule, estimatedCompleteTime, estimatedTimespent);
			scheduleInfoList.add(scheduleInfo);
		}

		return scheduleInfoList;
	}

	private ScheduleInfo getScheduleInfo(LogSchedule logSchedule, Date estimatedCompleteTime, Long estimatedTimespent) {

		Project project = projectService.getProjectById(logSchedule.getProjectId());

		Long projectId = project.getId();
		String projectName = project.getName();
		String scheduleId = logSchedule.getScheduleId();
		String scheduleName = logSchedule.getScheduleName();
		Date estimatedStartTime = logSchedule.getEstimatedStartTime();
		Date startTime = logSchedule.getStartTime();

		ScheduleInfo scheduleInfo = new ScheduleInfo();
		scheduleInfo.setProjectId(projectId);
		scheduleInfo.setProjectName(projectName);
		scheduleInfo.setScheduleId(scheduleId);
		scheduleInfo.setScheduleName(scheduleName);
		scheduleInfo.setEstimatedStartTime(estimatedStartTime);
		scheduleInfo.setEstimatedCompleteTime(estimatedCompleteTime);
		scheduleInfo.setStartTime(startTime);
		scheduleInfo.setEstimatedTimespent(estimatedTimespent);
		return scheduleInfo;
	}

	private Long getPreviousEstimatedTimespent(LogSchedule logSchedule, ScheduleTypeEnum scheduleType) {
		Long projectId = logSchedule.getProjectId();
		String scheduleId = logSchedule.getScheduleId();
		Collection<SchedulingScanStatusEnum> scanStatus = scheduleLogService.getScanStatusByScheduleType(projectId,
				scheduleId, scheduleType);
		return scheduleLogService.getEstimatedTimespentByScheduleType(projectId, scheduleId, scanStatus, scheduleType);
	}

	private Optional<LogSchedule> getNextCrossProjectScheduleInfoByProjectIdAndScheduleId(Long projectId,
			String scheduleId) {
		List<LogSchedule> logSchedules = scheduleLogService.findScheduleLogByProjectIdAndScheduleIdAndScheduleStatus(
				projectId, scheduleId, Lists.newArrayList(SchedulingStatusEnum.Pending));

		return logSchedules.stream()
				.sorted((old, current) -> old.getEstimatedStartTime().compareTo(current.getEstimatedStartTime()))
				.findFirst();
	}
}
