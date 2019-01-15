package com.gss.adm.web.jsf.actions;

import java.util.List;

import javax.ws.rs.Consumes;
import javax.ws.rs.DELETE;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import com.gss.adm.api.spring.security.AuthenticationUtils;
import com.gss.adm.core.model.MatrixQueryCondition;
import com.gss.adm.core.service.MatrixQueryConditionService;
import com.gss.adm.web.view.MatrixQueryConditionView;
import com.gss.adm.web.view.convert.MatrixQueryConditionViewConverter;
import com.gss.adm.web.view.form.convert.MatrixQueryConditionConverter;

/**
 * @author Kuma_Wu
 */
@Component
@Path("matrixQueryCondition")
public class MatrixQueryConditionAction {

	@Autowired
	private MatrixQueryConditionViewConverter matrixQueryConditionViewConverter;

	@Autowired
	private MatrixQueryConditionConverter matrixQueryConditionConverter;

	@Autowired
	private MatrixQueryConditionService matrixQueryConditionService;

	@GET
	@Path("project/{projectId}/name/{conditionName}")
	public boolean isNameDuplicated(@PathParam(value = "projectId") long projectId,
			@PathParam(value = "conditionName") String conditionName) {
		String username = AuthenticationUtils.getUsernameFromSecurityContext();

		return matrixQueryConditionService.isConditionNameDuplicated(projectId, username, conditionName);
	}

	@GET
	@Path("project/{projectId}")
	@Produces(MediaType.APPLICATION_JSON)
	public List<MatrixQueryConditionView> findByProjectIdAndUsername(@PathParam(value = "projectId") long projectId) {

		String username = AuthenticationUtils.getUsernameFromSecurityContext();

		List<MatrixQueryCondition> conditions = matrixQueryConditionService
				.findQueryConditionsByProjectIdAndUsername(projectId, username);

		return matrixQueryConditionViewConverter.convert(conditions);
	}

	@POST
	@Path("project/{projectId}")
	@Consumes(MediaType.APPLICATION_JSON)
	@Produces(MediaType.APPLICATION_JSON)
	public MatrixQueryConditionView saveQueryCondition(@PathParam(value = "projectId") long projectId,
			MatrixQueryConditionView conditionView) {

		String username = AuthenticationUtils.getUsernameFromSecurityContext();
		conditionView.setProjectId(projectId);

		MatrixQueryCondition condition = matrixQueryConditionConverter.convert(conditionView);
		condition = matrixQueryConditionService.saveQueryCondition(projectId, username, condition);

		return matrixQueryConditionViewConverter.convert(condition);

	}

	@DELETE
	@Path("project/{projectId}/id/{id}")
	public void deleteQueryCondition(@PathParam(value = "projectId") long projectId, @PathParam(value = "id") long id) {
		MatrixQueryCondition condition = matrixQueryConditionService.getQueryConditionById(projectId, id);
		matrixQueryConditionService.deleteQueryCondition(condition);
	}
}
