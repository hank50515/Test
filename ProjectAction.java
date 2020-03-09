package com.gss.adm.web.jsf.actions;

import static org.apache.cxf.transport.http.AbstractHTTPDestination.HTTP_REQUEST;

import java.io.File;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;
import javax.ws.rs.BeanParam;
import javax.ws.rs.Consumes;
import javax.ws.rs.DELETE;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.PUT;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang.StringUtils;
import org.apache.cxf.message.Message;
import org.apache.cxf.phase.PhaseInterceptorChain;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import com.google.common.base.Stopwatch;
import com.google.common.collect.Lists;
import com.gss.adm.api.constant.ModelDefinitionIds;
import com.gss.adm.api.lang.MapUtils2;
import com.gss.adm.api.lang.StringUtils2;
import com.gss.adm.api.spring.security.AuthenticationUtils;
import com.gss.adm.api.utils.LogUtils;
import com.gss.adm.core.dto.SimpleProject;
import com.gss.adm.core.form.GitVersionControlSetting;
import com.gss.adm.core.form.SvnVersionControlSetting;
import com.gss.adm.core.form.TfsVersionControlSetting;
import com.gss.adm.core.model.Entitydefinition;
import com.gss.adm.core.model.Project;
import com.gss.adm.core.model.ProjectModel;
import com.gss.adm.core.model.ProjectModelAttribute;
import com.gss.adm.core.model.ProjectModelDefinition;
import com.gss.adm.core.model.ProjectStatistics;
import com.gss.adm.core.model.UserProject;
import com.gss.adm.core.model.vo.EntityRelationshipStatisticsVo;
import com.gss.adm.core.model.vo.EntityStatistics;
import com.gss.adm.core.service.EntityDefinitionService;
import com.gss.adm.core.service.ProjectModelDefinitionService;
import com.gss.adm.core.service.ProjectModelService;
import com.gss.adm.core.service.ProjectService;
import com.gss.adm.core.service.ProjectStatisticsService;
import com.gss.adm.core.service.UserProjectService;
import com.gss.adm.core.service.VersionControlService;
import com.gss.adm.web.view.BackupLogView;
import com.gss.adm.web.view.EntityDefinitionCountView;
import com.gss.adm.web.view.FavoriteProjectView;
import com.gss.adm.web.view.ProjectModelView;
import com.gss.adm.web.view.ProjectProfileLinkView;
import com.gss.adm.web.view.ProjectProfileView;
import com.gss.adm.web.view.ProjectView;
import com.gss.adm.web.view.UserProjectView;
import com.gss.adm.web.view.UserSourceView;
import com.gss.adm.web.view.convert.BackupLogViewConverter;
import com.gss.adm.web.view.convert.EntityDefinitionCountViewConverter;
import com.gss.adm.web.view.convert.FavoriteProjectViewConverter;
import com.gss.adm.web.view.convert.GitVersionControlModelConverter;
import com.gss.adm.web.view.convert.ProjectProfileLinkViewConverter;
import com.gss.adm.web.view.convert.ProjectViewConverter;
import com.gss.adm.web.view.convert.SvnVersionControlModelConverter;
import com.gss.adm.web.view.convert.TfsVersionControlModelConverter;
import com.gss.adm.web.view.convert.UserProjectViewConverter;
import com.gss.adm.web.view.convert.UserSourceViewConverter;
import com.gss.adm.web.view.form.ProjectForm;
import com.gss.adm.web.view.form.ProjectModelAttributePureForm;
import com.gss.adm.web.view.form.ProjectModelIdForm;
import com.gss.adm.web.view.form.convert.ProjectFormConverter;
import com.gss.adm.web.view.form.convert.ProjectModelAttributePureFormConverter;

import lombok.extern.slf4j.Slf4j;

/**
 * 
 * @author Kuma_Wu
 */
@Slf4j
@Component
@Path("project")
public class ProjectAction implements ModelDefinitionIds {

	@Autowired
	private ProjectService projectService;

	@Autowired
	private ProjectStatisticsService projectStatisticsService;

	@Autowired
	private ProjectModelService projectModelService;

	@Autowired
	private ProjectModelDefinitionService projectModelDefitionService;

	@Autowired
	private EntityDefinitionService entityDefinitionService;

	@Autowired
	private UserProjectService userProjectService;

	@Autowired
	private VersionControlService versionControlService;

	@Autowired
	private ProjectModelAttributePureFormConverter projectModelAttributePureFormConverter;

	@Autowired
	private ProjectFormConverter projectFormConverter;

	@Autowired
	private ProjectViewConverter projectViewConverter;

	@Autowired
	private ProjectProfileLinkViewConverter projectProfileLinkViewConverter;

	@Autowired
	private EntityDefinitionCountViewConverter entityDefinitionCountViewConverter;

	@Autowired
	private BackupLogViewConverter backupLogViewConverter;

	@Autowired
	private UserProjectViewConverter userProjectViewConverter;

	@Autowired
	private UserSourceViewConverter userSourceViewConverter;

	@Autowired
	private GitVersionControlModelConverter gitVersionControlModelConverter;

	@Autowired
	private SvnVersionControlModelConverter svnVersionControlModelConverter;

	@Autowired
	private TfsVersionControlModelConverter tfsVersionControlModelConverter;

	@Autowired
	private FavoriteProjectViewConverter favoriteProjectViewConverter;

	@GET
	@Path("/listAllForDefaultTemplate/test")
	public List<ProjectView> finAllProjects() {
		List<Project> projects = projectService.findAllProjects();
		return projectViewConverter.convert(projects).stream().sorted((o1, o2) -> {
			return o1.getCode().compareTo(o2.getCode());
		}).collect(Collectors.toList());
	}

	@GET
	@Path("/list")
	public List<ProjectView> getUserProjects() {
		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		List<Project> projects = projectService.findProjectsByUsername(username);
		return projectViewConverter.convert(projects);
	}

	@GET
	@Path("/simple/list")
	public List<ProjectView> getUserSimpleProjects() {
		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		List<SimpleProject> simpleProjects = userProjectService.findSimpleProjectsByUsername(username);
		return projectViewConverter.convert(simpleProjects);
	}

	@GET
	@Path("byCode/{projectCode}")
	@Produces(MediaType.APPLICATION_JSON)
	public ProjectView getProjectByCode(@PathParam("projectCode") String projectCode) {
		if (StringUtils.isBlank(projectCode)) {
			throw new IllegalArgumentException("Project code is null or empty.");
		}

		Project project = projectService.getProjectByCode(projectCode);
		if (project == null) {
			throw new IllegalArgumentException("Can't get project by project code: " + projectCode);
		}

		String username = AuthenticationUtils.getUsernameFromSecurityContext();

		UserProject userProject = userProjectService.getByUsernameAndProjectCode(username, projectCode);
		if (userProject == null) {
			throw new IllegalArgumentException("User: " + username + " can't access project: " + projectCode);
		}

		return projectViewConverter.convert(project);
	}

	@GET
	@Path("/{projectId}/members")
	@Produces(MediaType.APPLICATION_JSON)
	public List<UserSourceView> getMembers(@PathParam("projectId") long projectId) {
		Project project = projectService.getProjectById(projectId);
		List<UserProject> userProjects = userProjectService.findUserProjectsByProjectCode(project.getCode());
		List<UserProjectView> userProjectViews = userProjectViewConverter.convert(userProjects);

		return userSourceViewConverter.convert(userProjectViews);
	}

	@PUT
	@Path("/")
	@Consumes(MediaType.APPLICATION_JSON)
	@Produces(MediaType.APPLICATION_JSON)
	public ProjectView save(ProjectForm projectForm) {
		validateProjectForm(projectForm);
		Project existedProject = projectService.getProjectByCode(projectForm.getCode());
		validateUpdateProject(projectForm, existedProject);

		Project project = projectFormConverter.convert(projectForm);
		project = projectService.saveProject(project);

		return projectViewConverter.convert(project);
	}

	@POST
	@Path("/")
	@Consumes(MediaType.APPLICATION_JSON)
	@Produces(MediaType.APPLICATION_JSON)
	public ProjectView addProject(ProjectForm projectForm) {
		validateProjectForm(projectForm);

		Project existedProject = projectService.getProjectByCode(projectForm.getCode());
		validateAddProject(existedProject);

		Project project = projectFormConverter.convert(projectForm);
		project = projectService.saveProject(project);

		return projectViewConverter.convert(project);
	}

	// 之後改抽到 ProjectConfigurationAction.
	@POST
	@Path("{projectId}/removeProjectModel")
	@Consumes(MediaType.APPLICATION_FORM_URLENCODED)
	public boolean removeProjectModel(@PathParam("projectId") long projectId, @BeanParam ProjectModelIdForm form) {
		projectModelService.deleteProjectModelById(form.getProjectModelId());
		return true;
	}

	@DELETE
	@Path("removeProject/{projectId}")
	@Consumes(MediaType.APPLICATION_FORM_URLENCODED)
	@Produces(MediaType.APPLICATION_JSON)
	public void removeProject(@PathParam("projectId") long projectId) throws IllegalAccessException {

		Project project = projectService.getProjectById(projectId);
		if (project == null) {
			throw new IllegalAccessException("Can't find project by id: " + projectId);
		}

		String projectCode = project.getCode();
		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		UserProject userProject = userProjectService.getByUsernameAndProjectCode(username, projectCode);
		if (userProject == null) {
			throw new IllegalAccessException("Username: " + username + " doesn't belong to project: " + projectCode);
		}

		projectService.deleteProjectById(projectId);

		Message message = PhaseInterceptorChain.getCurrentMessage();
		HttpServletRequest request = (HttpServletRequest) message.get(HTTP_REQUEST);
		HttpSession session = request.getSession(true);
		session.removeAttribute("projectId");
	}

	// 之後改抽到 ProjectConfigurationAction.
	@POST
	@Path("{projectId}/validVersionControlSetting")
	@Consumes(MediaType.APPLICATION_JSON)
	public boolean validVersionControlSetting(@PathParam("projectId") long projectId,
			ProjectModelView projectModelView) {

		Long projectModelDefinitionId = projectModelView.getProjectModelDefinitionId();
		boolean success = false;
		if (PROJECT_MODEL_SUBVERSION_ID.equals(projectModelDefinitionId)) {
			SvnVersionControlSetting svnSetting = svnVersionControlModelConverter.convert(projectModelView);
			success = versionControlService.isValidSvn(svnSetting);
		} else if (PROJECT_MODEL_GIT_ID.equals(projectModelDefinitionId)) {
			GitVersionControlSetting gitSetting = gitVersionControlModelConverter.convert(projectModelView);
			success = versionControlService.isValidGit(gitSetting);
		} else if (PROJECT_MODEL_TFS_ID.equals(projectModelDefinitionId)) {
			TfsVersionControlSetting tfsSetting = tfsVersionControlModelConverter.convert(projectModelView);
			success = versionControlService.isValidTfs(tfsSetting);
		}

		return success;
	}

	// 之後改抽到 ProjectConfigurationAction.
	@POST
	@Path("/{projectId}/sourceCodeSetting/")
	public boolean saveSourceCodeSetting(@PathParam("projectId") Long projectId, ProjectModelView projectModelView) {

		Long projectModelDefinitionId = projectModelView.getProjectModelDefinitionId();
		ProjectModel projectModel = getAndCreateSourceCodeProjectModel(projectId, projectModelDefinitionId);
		List<ProjectModelAttribute> modelAttributes = createSourceCodeProjectModelAttribute(projectModelView);

		projectModel.setModelAttribute(modelAttributes);
		projectModelService.saveProjectModelAndAdjustSetting(projectModel);

		return true;
	}

	// return backup log list
	@GET
	@Path("/{projectId}/backupList/")
	@Produces(MediaType.APPLICATION_JSON)
	public List<BackupLogView> getBackupList(@PathParam("projectId") Long projectId) {

		log.debug("Get project backup log list: {}", projectId);
		List<File> backupFiles = projectService.findBackupLogListByProjectId(projectId);
		if (CollectionUtils.isEmpty(backupFiles)) {
			return Lists.newArrayList();
		}
		return backupLogViewConverter.convert(backupFiles);
	}

	// return backup log file
	@GET
	@Path("/{projectId}/backupList/{fileName}/download")
	@Produces("application/zip")
	public File getBackupLogFile(@PathParam("projectId") Long projectId, @PathParam("fileName") String fileName) {

		log.debug("Get project backup log: {}", projectId);
		File backupFile = projectService.getBackupLogByProjectId(projectId, fileName);
		if (backupFile == null) {
			return null;
		}
		return backupFile;
	}

	@POST
	@Path("/{projectId}/favorite/{isFavorite}")
	public FavoriteProjectView updateFavoriteProject(@PathParam("projectId") Long projectId,
			@PathParam("isFavorite") boolean isFavorite) {

		String username = AuthenticationUtils.getUsernameFromSecurityContext();

		UserProject userProject = userProjectService.updateFavoriteProject(projectId, username, isFavorite);

		return favoriteProjectViewConverter.convert(userProject);
	}

	@GET
	@Path("/favorite")
	@Produces(MediaType.APPLICATION_JSON)
	public List<FavoriteProjectView> findFavoriteProjects() {

		String username = AuthenticationUtils.getUsernameFromSecurityContext();

		Stopwatch watch = LogUtils.logStart("Find {}'s favorite projects.", username);

		List<UserProject> favoriteProjects = userProjectService.findFavoriteUserProjectsByUsername(username);
		List<FavoriteProjectView> favoriteProjectViews = favoriteProjectViewConverter.convert(favoriteProjects);
		Collections.sort(favoriteProjectViews, (FavoriteProjectView o1, FavoriteProjectView o2) -> o1.getProject()
				.getCode().compareTo(o2.getProject().getCode()));

		LogUtils.logStop(watch, "Find {} favorite projects from {}.", favoriteProjectViews.size(), username);

		return favoriteProjectViews;
	}

	@GET
	@Path("/userProjects")
	@Produces(MediaType.APPLICATION_JSON)
	public List<UserProjectView> findUserProjects() {

		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		Stopwatch watch = LogUtils.logStart("Find {}'s user projects.", username);
		List<UserProject> userProjects = userProjectService.findUserProjectsByUsername(username);
		List<UserProjectView> userProjectViews = userProjectViewConverter.convert(userProjects);
		Collections.sort(userProjectViews, (UserProjectView o1, UserProjectView o2) -> {
			if (o1.isFavorite() != o2.isFavorite()) {
				return o1.isFavorite() ? -1 : 1;
			} else {
				return o1.getProjectView().getCode().compareTo(o2.getProjectView().getCode());
			}
		});

		LogUtils.logStop(watch, "Find {} user projects from {}.", userProjectViews.size(), username);

		return userProjectViews;
	}

	@GET
	@Path("/{projectId}/profile")
	@Produces(MediaType.APPLICATION_JSON)
	public ProjectProfileView findByProjectId(@PathParam("projectId") Long projectId) {

		ProjectStatistics lastProjectStatistics = projectStatisticsService
				.findLatestProjectStatisticsByProjectId(projectId);

		if (lastProjectStatistics == null) {
			return new ProjectProfileView();
		}

		// nodes data
		List<EntityStatistics> entityStatistics = lastProjectStatistics.getEntityStatistics();
		Map<String, EntityStatistics> entityStatisticsMap = MapUtils2.convertToMap(entityStatistics, "name");

		List<Entitydefinition> entityDefinitions = entityDefinitionService.findAllEntityDefinitionsOrderBySortAndName();
		List<EntityDefinitionCountView> entityDefinitionCountViews = entityDefinitionCountViewConverter
				.convert(entityDefinitions, entityStatisticsMap);

		// links data
		List<EntityRelationshipStatisticsVo> entityRelationshipStatistics = lastProjectStatistics
				.getEntityRelationshipStatistics();
		List<ProjectProfileLinkView> projectProfileLinksViews = projectProfileLinkViewConverter
				.convert(entityDefinitionCountViews, entityRelationshipStatistics);

		return new ProjectProfileView(entityDefinitionCountViews, projectProfileLinksViews);
	}

	@GET
	@Path("/getCrossProjectId")
	@Produces(MediaType.APPLICATION_JSON)
	public Long getCrossProjectId() {
		Project crossProject = projectService.getCrossProject();
		return crossProject.getId();
	}

	@GET
	@Path("/{projectId}/code")
	@Produces(MediaType.TEXT_PLAIN)
	public String getProjectCodeByProjectId(@PathParam("projectId") Long projectId) {
		return projectService.getProjectCodeById(projectId);
	}

	//////////////////////////////////////////////////////////////////////////////////////
	// Private Methods.
	private void validateUpdateProject(ProjectForm projectData, Project project) {
		if (project != null) {
			if (!projectData.getId().equals(project.getId())) {
				throw new IllegalArgumentException("請勿隨意修改專案代號!");
			} else if (StringUtils2.isNotBlank(project.getSource())
					&& !project.getName().equals(projectData.getName())) {
				throw new IllegalArgumentException("請勿隨意修改專案名稱!");
			}
		} else {
			throw new IllegalArgumentException("請勿隨意修改專案代號!");
		}
	}

	private void validateAddProject(Project project) {
		if (project != null) {
			throw new IllegalArgumentException("該專案代號已被使用，請重新輸入新的專案代號!");
		}
	}

	private ProjectModel getAndCreateSourceCodeProjectModel(Long projectId, Long modelDefinitionId) {
		Project project = projectService.getProjectById(projectId);
		ProjectModelDefinition modelDefinition = projectModelDefitionService.findModelDefinitionById(modelDefinitionId);
		ProjectModel projectModel = new ProjectModel();

		projectModel.setName(modelDefinition.getName());
		projectModel.setProject(project);
		projectModel.setModelDefinition(modelDefinition);
		projectModel.setDisabled(false);

		return projectModelService.saveProjectModelAndAdjustSetting(projectModel);
	}

	private List<ProjectModelAttribute> createSourceCodeProjectModelAttribute(ProjectModelView projectModelView) {

		List<ProjectModelAttributePureForm> projectModelAttributeViews = projectModelView
				.getProjectModelAttributeViews();

		if (CollectionUtils.isEmpty(projectModelAttributeViews)) {
			return Lists.newArrayList();
		}

		return projectModelAttributePureFormConverter.convert(projectModelAttributeViews);
	}

	private void validateProjectForm(ProjectForm form) {
		if (StringUtils2.isEmpty(form.getCode())) {
			throw new IllegalArgumentException("專案代號不可為空!");
		}

		if (StringUtils2.isEmpty(form.getName())) {
			throw new IllegalArgumentException("專案名稱不可為空!");
		}
	}

}
