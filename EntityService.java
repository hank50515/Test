package com.gss.adm.scd.webservice;

import java.util.HashMap;
import java.util.List;

import com.gss.adm.scd.model.Enttity;

public interface EntityService {

	public Enttity saveEntity(Enttity entity);

	public void deleteByProjectIdAndDefinitionId(Long projectId, Integer definitionId);

	public List<Enttity> findByProjectId(Long projectId);

	public List<Enttity> findEntitiesExcludedRelationship(Long projectId);

	public Integer getCurrentVersion(Long projectId);
	
	public HashMap<String, Enttity> getEntityCodeMap(Long projectId);
	
	public void setEntitiesDeletedByVersion(Long projectId, Integer currentVersion);
}
