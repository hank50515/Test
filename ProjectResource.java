package com.gss.adm.rest.plugin.resource;

import java.util.List;

import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;

import org.apache.commons.collections.CollectionUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import com.gss.adm.core.model.Project;
import com.gss.adm.core.service.ProjectService;
import com.gss.adm.core.wsclient.domian.ProjectDomain;
import com.gss.adm.core.wsclient.domian.ResultView;
import com.gss.adm.core.wsclient.domian.simple.ProjectSimpleDomain;
import com.gss.adm.rest.plugin.domain.converter.ProjectDomainConverter;
import com.gss.adm.rest.plugin.domain.simple.converter.ProjectSimpleDomainConverter;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
@Path("/project")
public class ProjectResource {

	@Autowired
	private ProjectService projectService;

	@Autowired
	private ProjectDomainConverter projectDomainConverter;

	@Autowired
	private ProjectSimpleDomainConverter projectSimpleDomainConverter;
	
	private static final String SUCCESS = "success";
	private static final String FAIL = "fail";

	@GET
	@Path("/{projectId}/")
	public ProjectDomain getProject(@PathParam("projectId") long projectId) {
		log.debug("call api: /project/{}", projectId);

		Project project = projectService.getProjectById(projectId);
		if (project != null) {
			return projectDomainConverter.convert(project);
		} else {
			return null;
		}
	}

	@GET
	@Path("/code/{projectCode}")
	public ProjectDomain getProjectWithPropertiesByCode(@PathParam("projectCode") String projectCode) {
		log.debug("call api: /project/code/{}", projectCode);
		
		Project project = projectService.getProjectWithPropertiesByCode(projectCode);
		if (project != null) {
			return projectDomainConverter.convert(project);
		} else {
			return null;
		}
	}

	@GET
	@Path("all")
	public List<ProjectDomain> findAllProjects() {
		log.debug("call api: /project/all");

		List<Project> allProjects = projectService.findAllProjects();
		return projectDomainConverter.convert(allProjects);
	}

	@GET
	@Path("simple/all")
	public List<ProjectSimpleDomain> getAllSimpleProjects() {
		log.debug("call api: /project/simple/all");

		List<Project> allProjects = projectService.findAllProjects();
		return projectSimpleDomainConverter.convert(allProjects);
	}

	@GET
	@Path("{projectId}/isComplete/{isDiscoveryComplete}")
	public boolean updateDiscoveryComplete(@PathParam("projectId") long projectId,
			@PathParam("isDiscoveryComplete") Boolean isDiscoveryComplete) {

		log.debug("call api: /project/{}/isComplete/{}", projectId, isDiscoveryComplete);

		Project project = projectService.getProjectById(projectId);
		if (project == null) {
			return false;
		}

		projectService.updateDiscoveryComplete(project, isDiscoveryComplete);
		return true;
	}
	
	@GET
	@Path("updateAllPorject/FullScan/")
	public ResultView updateAllPorjectFullScan() {

		log.debug("call api: /updateAllPorject/FullScan/");
		ResultView result = new ResultView();
		List<Project> projects = projectService.findAllProjects();
		if (CollectionUtils.isEmpty(projects)){
			result.setMessage(FAIL);
			return result;
		}

		for (Project project : projects) {
			projectService.updateDiscoveryComplete(project, false);
		}
		result.setMessage(SUCCESS);
		return result;
	}
}
