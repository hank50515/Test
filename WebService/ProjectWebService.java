package com.gss.adm.scd.webservice;

import java.util.List;

import com.gss.adm.core.wsclient.domian.ProjectDomain;
import com.gss.adm.core.wsclient.domian.simple.ProjectSimpleDomain;

public interface ProjectWebService {

	public List<ProjectDomain> findAllProjects();

	public void updateDiscoveryComplete(long projectId, boolean isDiscoveryComplete);

	public ProjectDomain getProject(long projectId);

	public ProjectDomain getProjectWithPropertiesByCode(String projectCode);

	public ProjectSimpleDomain getCrossProject();
}
