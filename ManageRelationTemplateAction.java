package com.gss.adm.web.jsf.actions;

import java.util.List;

import javax.ws.rs.DELETE;
import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import com.gss.adm.core.model.RelationTemplate;
import com.gss.adm.core.service.RelationTemplateService;
import com.gss.adm.web.view.ManageRelationTemplateView;
import com.gss.adm.web.view.convert.ManageRelationTemplateViewConverter;

@Component
@Path("manageRelationTemplate")
public class ManageRelationTemplateAction {

	@Autowired
	private RelationTemplateService relationTemplateService;

	@Autowired
	private ManageRelationTemplateViewConverter manageRelationTemplateViewConverter;

	@GET
	@Path("/project/{projectId}")
	@Produces(MediaType.APPLICATION_JSON)
	public List<ManageRelationTemplateView> findByProjectId(@PathParam(value = "projectId") Long projectId) {
		List<RelationTemplate> sources = relationTemplateService.findRelationTemplatesByProjecId(projectId);

		return manageRelationTemplateViewConverter.convert(sources);
	}

	@DELETE
	@Path("/relationTemplate/{relationTemplateId}")
	public void deleteRelationTemplate(@PathParam("relationTemplateId") Long relationTemplateId) {

		if (relationTemplateId == null) {
			throw new IllegalArgumentException("relationTemplateId is empty");
		}

		relationTemplateService.deleteByRelationTemplateId(relationTemplateId);
	}
}
