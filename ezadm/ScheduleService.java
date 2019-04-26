package com.gss.adm.core.service;

import java.util.List;

import com.gss.adm.api.enums.ScheduleTypeEnum;
import com.gss.adm.core.model.ProjectModel;
import com.gss.adm.core.model.ScheduleInfo;

public interface ScheduleService {

	ScheduleInfo executeSchedule(Long projectId);
	
	ScheduleInfo executeCrossProjectScheduleImmediately(Long projectId);

	ScheduleInfo executeSchedule(ProjectModel configuration);

	ScheduleInfo executeBuildMatrixSchedule(ProjectModel configuration);

	List<ScheduleInfo> getAllScheduleInfo();

	List<ScheduleInfo> getAllMatrixInfo();

	ScheduleInfo getScheduleInfoByProjectIdAndScheduleIdAndScheduleType(Long projectId, String scheduleId,
			ScheduleTypeEnum scheduleType);
}
