package com.gss.adm.core.prod.service.provider;

import java.io.File;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.filefilter.DirectoryFileFilter;
import org.apache.commons.io.filefilter.TrueFileFilter;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.gss.adm.api.constant.ProjectDefinitions;
import com.gss.adm.api.lang.MapUtils2;
import com.gss.adm.api.spring.security.AuthenticationUtils;
import com.gss.adm.core.model.Project;
import com.gss.adm.core.model.ProjectProperties;
import com.gss.adm.core.model.User;
import com.gss.adm.core.model.UserProject;
import com.gss.adm.core.prod.dao.ProjectDao;
import com.gss.adm.core.service.ProjectService;
import com.gss.adm.core.service.UserProjectService;
import com.gss.adm.core.service.UserService;
import com.gss.adm.core.service.dicovery.CiFactory;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
@Transactional
public class ProjectServiceImpl implements ProjectService, ProjectDefinitions {

	@Autowired
	private CiFactory ciFactory;

	@Autowired
	private ProjectDao projectDao;

	@Autowired
	private UserService userService;

	@Autowired
	private UserProjectService userProjectService;

	@Override
	public Project saveProject(Project project) {

		// 1. 先儲存專案資料.
		project = projectDao.saveAndFlush(project);

		// 2. 再儲存使用者跟專案的關連.
		this.saveUserAndProjectRelationship(project);

		return project;
	}

	@Override
	public void deleteProjectById(Long projectId) {

		Project project = projectDao.findOne(projectId);

		// 刪除 userProejct 因為 FK 是 projectCode 無法使用 cascade
		userProjectService.deleteByProjectId(projectId);

		// 刪除 project
		projectDao.delete(project);

		deleteWorkSpace(project.getCode());
	}

	@Override
	public String getProjectCodeById(Long projectId) {
		return projectDao.getCodeById(projectId);
	}

	@Override
	public Project getProjectById(Long projectId) {
		if (projectId == null) {
			return null;
		}

		return projectDao.findOne(projectId);
	}

	@Override
	public Project getCrossProject() {
		return projectDao.getCorssProject();
	}

	@Override
	public List<Project> findByIds(Collection<Long> ids) {
		return projectDao.findAll(ids);
	}

	@Override
	public Project getProjectByCode(String projectCode) {
		return projectDao.findByCode(projectCode);
	}

	@Override
	public Project getProjectWithPropertiesByCode(String projectCode) {
		Project project = projectDao.findByCode(projectCode);
		project.getProperties();
		return project;
	}

	@Override
	public Project getProjectByJiraKey(String jiraKey) {
		return projectDao.findByPropertiesJiraKey(jiraKey);
	}

	@Override
	public List<Project> findAllProjects() {
		return projectDao.findAll();
	}

	@Override
	public List<Project> findProjectsByUsername(String username) {
		return userProjectService.findProjectsByUsername(username);
	}

	@Override
	public List<Long> findProjectIdsByUsername(String username) {

		List<Long> projectIds = Lists.newLinkedList();
		List<Project> projects = findProjectsByUsername(username);

		for (Project project : projects) {
			projectIds.add(project.getId());
		}

		return projectIds;
	}

	@Override
	public Project updateDiscoveryComplete(Project project, boolean isDiscoveryComplete) {

		String projectCode = project.getCode();
		project = getProjectByCode(projectCode);

		ProjectProperties properties = project.getProperties();

		if (properties == null) {
			properties = new ProjectProperties();
			properties.setProject(project);
		}

		properties.setDiscoveryComplete(isDiscoveryComplete);

		project.setProperties(properties);

		Project savedProject = projectDao.save(project);
		log.debug("[Scan Status] Project {} disvoery complete: {}", project.getCode(), isDiscoveryComplete);

		return savedProject;
	}

	@Override
	public List<File> findBackupLogListByProjectId(Long projectId) {
		Map<String, Object> configuration = getConfiguration();
		String workspace = MapUtils2.getValue(configuration, SYSTEM_PROPERTIES_WORKSPACE);
		String projectCode = projectDao.getCodeById(projectId);
		Path path = Paths.get(workspace, projectCode, "BackupDisocveryLog");
		File file = path.toFile();

		if (file.exists()) {
			log.info("Delete workspace...");
			Collection<File> listFiles = FileUtils.listFiles(file, TrueFileFilter.INSTANCE,
					DirectoryFileFilter.INSTANCE);
			return new ArrayList<File>(listFiles);
		}
		return Lists.newArrayList();
	}

	@Override
	public File getBackupLogByProjectId(Long projectId, String fileName) {
		Map<String, Object> configuration = getConfiguration();
		String workspace = MapUtils2.getValue(configuration, SYSTEM_PROPERTIES_WORKSPACE);
		String projectCode = projectDao.getCodeById(projectId);
		return Paths.get(workspace, projectCode, "BackupDisocveryLog", fileName).toFile();
	}

	@Override
	public List<Project> findAllProjectsById(Collection<Long> projectIds) {
		return projectDao.findAll(projectIds);
	}

	/**
	 * 儲存使用者跟專案的關連.
	 * 
	 * Tip. 使用者這邊是抓登入者的使用者資料.
	 * 
	 * @param project
	 *            - 專案資料.
	 */
	private void saveUserAndProjectRelationship(Project project) {

		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		User user = userService.getUserByUsername(username);

		String projectCode = project.getCode();
		UserProject relationship = userProjectService.getByUsernameAndProjectCode(username, projectCode);

		if (relationship == null) {
			relationship = new UserProject();
		}

		relationship.setUser(user);
		relationship.setProject(project);

		userProjectService.saveRelationship(relationship);
	}

	private void deleteWorkSpace(String projectCode) {
		Map<String, Object> configuration = getConfiguration();
		String workspace = MapUtils2.getValue(configuration, SYSTEM_PROPERTIES_WORKSPACE);

		Path path = Paths.get(workspace, projectCode);
		File file = path.toFile();

		if (file.exists()) {
			log.info("Delete workspace...");
			FileUtils.deleteQuietly(path.toFile());
		}
	}

	private Map<String, Object> getConfiguration() {
		Map<String, Object> configuration = Maps.newHashMap();
		Properties systemProperties = ciFactory.getProperties();
		configuration.put(SYSTEM_PROPERTIES, Maps.newLinkedHashMap(systemProperties));

		return configuration;
	}
}
