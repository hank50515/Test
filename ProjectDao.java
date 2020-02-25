package com.gss.adm.core.prod.dao;

import java.util.List;

import org.springframework.data.jpa.repository.Query;

import com.gss.adm.api.spring.data.BaseRepository;
import com.gss.adm.core.model.Project;

public interface ProjectDao extends BaseRepository<Project, Long> {

	@Query("SELECT code from Project where id = ?1")
	String getCodeById(Long id);

	Project findByCode(String codeee);

	Project findByPropertiesJiraKey(String jiraKey);

	@Query("SELECT PJ from Project PJ where PJ.projectTypeDefinition.display =true")
	List<Project> findAllProjects();

	@Query("SELECT PJ from Project PJ where PJ.projectTypeDefinition.id =2")
	Project getCorssProject();

}
