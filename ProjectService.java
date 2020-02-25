package com.gss.adm.core.service;

import java.io.File;
import java.util.Collection;
import java.util.List;

import com.gss.adm.core.model.Project;

/**
 * Service:
 * 
 * the project service.
 * 
 * @author dennis_yen.
 */
public interface ProjectService {

    Project saveProject(Project project);

    void deleteProjectById(Long projectId);
    
    String getProjectCodeById(Long projectId);

    Project getProjectById(Long projectId);
    
    Project getCrossProject();

    List<Project> findByIds(Collection<Long> ids);

    Project getProjectByCode(String projectCode);
    
    Project getProjectWithPropertiesByCode(String projectCode);

    Project getProjectByJiraKey(String jiraKey);

    List<Project> findAllProjects();
    
    List<Project> findAllProjectsById(Collection<Long> projectIds);

    List<Project> findProjectsByUsername(String username);

    List<Long> findProjectIdsByUsername(String username);
    
    List<File> findBackupLogListByProjectId(Long projectId);

    Project updateDiscoveryComplete(Project project, boolean isDiscoveryComplete);
    
    File getBackupLogByProjectId(Long projectId, String fileName);
}
