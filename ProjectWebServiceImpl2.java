package com.gss.adm.scd.webservice.provider;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.google.gson.internal.StringMap;
import com.gss.adm.api.rest.template.RestTemplateGET;
import com.gss.adm.core.wsclient.domian.ProjectDomain;
import com.gss.adm.scd.webservice.ProjectWebService;

@Service
public class ProjectWebServiceImpl implements ProjectWebService {

	@Autowired
	private RestTemplateGET requestGET;

	@Override
	public List<ProjectDomain> findAllProjects() {
		String restUri = requestGET.combineBaseRestUri("project/all");
		return requestGET.httpGETMany(restUri, ProjectDomain.class);
	}

	@Override
	public void updateDiscoveryComplete(long projectId, boolean isDiscoveryComplete) {
		String restUri = requestGET.combineBaseRestUri("project/{projectId}/isComplete/{isDiscoveryComplete}");
		StringMap<Object> param = new StringMap<Object>();
		param.put("projectId", projectId);
		param.put("isDiscoveryComplete", isDiscoveryComplete);

		requestGET.httpGETOneByPath(restUri, boolean.class, param);
	}

	@Override
	public ProjectDomain getProject(long projectId) {
		String restUri = requestGET.combineBaseRestUri("project/{projectId}");
		StringMap<Object> param = new StringMap<Object>();
		param.put("projectId", projectId);

		return requestGET.httpGETOneByPath(restUri, ProjectDomain.class, param);
	}

	@Override
	public ProjectDomain getProjectWithPropertiesByCode(String projectCode) {
		String restUri = requestGET.combineBaseRestUri("project/code/{projectCode}");
		StringMap<Object> param = new StringMap<Object>();
		param.put("projectCode", projectCode);

		return requestGET.httpGETOneByPath(restUri, ProjectDomain.class, param);
	}
}
