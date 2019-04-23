package com.gss.adm.web.jsf.actions;

import java.util.Collections;
import java.util.List;

import javax.ws.rs.BadRequestException;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.QueryParam;
import javax.ws.rs.core.MediaType;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Component;

import com.gss.adm.api.constant.ModelDefinitionIds;
import com.gss.adm.api.utils.LogUtils;
import com.gss.adm.api.validation.ValidationException;
import com.gss.adm.core.model.LogSchedule;
import com.gss.adm.core.model.ProjectModel;
import com.gss.adm.core.model.ProjectModelType;
import com.gss.adm.core.model.ScheduleInfo;
import com.gss.adm.core.service.LogScheduleService;
import com.gss.adm.core.service.ProjectModelService;
import com.gss.adm.core.service.ScheduleService;
import com.gss.adm.web.view.ScheduleInfoView;
import com.gss.adm.web.view.ScheduleView;
import com.gss.adm.web.view.convert.ScheduleInfoViewConverter;
import com.gss.adm.web.view.convert.ScheduleViewConverter;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
@Path("/schedule")
public class ScheduleAction implements ModelDefinitionIds {

	@Autowired
	private ScheduleService scheduleService;

	@Autowired
	private LogScheduleService scheduleLogService;

	@Autowired
	private ProjectModelService projectModelService;

	@Autowired
	private ScheduleInfoViewConverter scheduleInfoViewConverter;

	@Autowired
	private ScheduleViewConverter scheduleViewConverter;

	@POST
	@Path("/buildMatrix/{projectId}/{configurationId}")
	public ScheduleInfoView executeMatrixSchedule(@PathParam("projectId") Long projectId,
			@PathParam("configurationId") Long configurationId) {

		ProjectModel configuration = projectModelService.getProjectModelById(configurationId);

		// Check Model Type is Schedule.
		ProjectModelType modelType = configuration.getModelDefinition().getModelType();
		if (!PROJECT_MODEL_TYPE_SCHEDULE_ID.equals(modelType.getId())) {
			throw new BadRequestException();
		}

		ScheduleInfo result = scheduleService.executeBuildMatrixSchedule(configuration);
		
		return scheduleInfoViewConverter.convert(result);
	}

	@GET
	@Path("/project/{projectId}/execute")
	public ScheduleInfoView executeSchedule(@PathParam("projectId") Long projectId) {

		ScheduleInfo result = null;

		try {
			result = scheduleService.executeSchedule(projectId);
		} catch (IllegalArgumentException e) {
			log.debug(e.toString());
			throw new BadRequestException(e);
		}

		return scheduleInfoViewConverter.convert(result);
	}

	@GET
	@Path("/project/{projectId}/execute/crossProject")
	public ScheduleInfoView executeCrossProjectSchedule(@PathParam("projectId") Long projectId) {
		ScheduleInfo result = null;

		try {
			result = scheduleService.executeCrossProjectScheduleImmediately(projectId);
		} catch (IllegalArgumentException e) {
			LogUtils.logThrowable(e);
			throw new BadRequestException(e);
		} catch (ValidationException e) {
			LogUtils.logThrowable(e);
			throw e;
		}

		return scheduleInfoViewConverter.convert(result);
	}

	// FIXME 以後改用 search 方法來做.
	@GET
	@Path("/log/{projectId}")
	@Produces(MediaType.APPLICATION_JSON)
	public List<ScheduleView> findAllScheduling(@PathParam("projectId") Long projectId) {

		if (projectId == null) {
			throw new IllegalArgumentException("The project id is empty.");
		}

		List<LogSchedule> schedulings = scheduleLogService.findScheduleLog(projectId);
		return scheduleViewConverter.convert(schedulings);
	}

	// FIXME 以後改用 search 方法來做.
	@GET
	@Path("/log/{projectId}/custom")
	@Produces(MediaType.APPLICATION_JSON)
	public List<ScheduleView> findCurrentSchedulingByCount(@PathParam("projectId") Long projectId,
			@QueryParam("maxResults") int maxResults) {

		if (projectId == null) {
			throw new IllegalArgumentException("The project id is empty.");
		}
		PageRequest pageRequest = new PageRequest(0, maxResults, Sort.Direction.DESC, "addingTime");
		Page<LogSchedule> schedulings = scheduleLogService.findLatestLogSchedule(projectId, pageRequest);

		return scheduleViewConverter.convert(schedulings.getContent());
	}

	@GET
	@Path("/scheduleInfo/all")
	@Produces(MediaType.APPLICATION_JSON)
	public List<ScheduleInfoView> findCurrentSchedulingByCount() {

		List<ScheduleInfo> schedulings = scheduleService.getAllScheduleInfo();
		Collections.sort(schedulings, (s1, s2) -> s2.getEstimatedStartTime().compareTo(s1.getEstimatedStartTime()));
		
		return scheduleInfoViewConverter.convert(schedulings);
	}

}
