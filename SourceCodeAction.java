package com.gss.adm.web.jsf.actions;

import static com.gss.adm.api.enums.ProjectDefinitions.REPOSITORY_TYPE_GIT;
import static com.gss.adm.api.enums.ProjectDefinitions.REPOSITORY_TYPE_SVN;
import static com.gss.adm.api.enums.ProjectDefinitions.REPOSITORY_TYPE_TFS;

import java.io.IOException;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.Set;
import java.util.regex.Pattern;

import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import com.google.common.collect.Lists;
import com.google.common.collect.Sets;
import com.gss.adm.api.constant.ModelDefinitions;
import com.gss.adm.api.enums.ModelAttributeNameEnum;
import com.gss.adm.api.vo.RelationDetail;
import com.gss.adm.core.model.EntityRelationship;
import com.gss.adm.core.model.Enttity;
import com.gss.adm.core.model.Project;
import com.gss.adm.core.model.SourceCodeDiffText;
import com.gss.adm.core.model.vo.SourceCodeMethodDto;
import com.gss.adm.core.service.ChangeAnalysisService;
import com.gss.adm.core.service.CrossProjectEntityRelationService;
import com.gss.adm.core.service.EntityRelationService;
import com.gss.adm.core.service.EntityService;
import com.gss.adm.core.service.ProjectService;
import com.gss.adm.web.view.RelationDetailView;
import com.gss.adm.web.view.SourceCodeDiffTextView;
import com.gss.adm.web.view.SourceCodePureTextView;
import com.gss.adm.web.view.SourceCodeTextView;
import com.gss.adm.web.view.SourceCodeTreeView;
import com.gss.adm.web.view.convert.RelationDetailViewConverter;
import com.gss.adm.web.view.convert.SourceCodeDiffTextViewConverter;
import com.gss.adm.web.view.convert.SourceCodePureTextViewConverter;
import com.gss.adm.web.view.convert.SourceCodeTextViewConverter;
import com.gss.adm.web.view.convert.SourceCodeTreeViewConverter;
import com.gss.adm.web.view.convert.SourceCodeTreeViewForEntityConverter;
import com.gss.tds.common.converter.GsonConverter;

import lombok.extern.slf4j.Slf4j;

/**
 * @author Taoyu_Wu
 */
@Slf4j
@Component
@Path("sourceCode")
public class SourceCodeAction implements ModelDefinitions {
	
	private static final String BACKWARD = "Backward";
	
	private static final String FORWARD = "Forward";
	
	@Autowired
	private ProjectService projectService;
	
	@Autowired
	private EntityService entityService;

	@Autowired
	private EntityRelationService entityRelationService;

	@Autowired
	private ChangeAnalysisService changeAnalysisService;
	
	@Autowired
	private CrossProjectEntityRelationService crossProjectEntityRelationService;

	@Autowired
	private SourceCodeTextViewConverter sourceCodeTextViewConverter;

	@Autowired
	private SourceCodePureTextViewConverter sourceCodePureTextViewConverter;

	@Autowired
	private SourceCodeTreeViewConverter sourceCodeTreeViewConverter;

	@Autowired
	private SourceCodeTreeViewForEntityConverter sourceCodeTreeViewForEntityConverter;

	@Autowired
	private SourceCodeDiffTextViewConverter sourceCodeDiffTextViewConverter;

	@Autowired
	private RelationDetailViewConverter relationDetailViewConverter;

	@GET
	@Path("project/{projectId}/entity/{entityId}")
	public SourceCodeTextView showSourceCodeText(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId) {

		log.debug("projectId: {}, EntityId: {}", projectId, entityId);

		Enttity entity = entityService.getEntityByProjectIdAndEntityId(projectId, entityId);
		if (entity == null) {
			throw new IllegalArgumentException("The entity can not be null");
		}

		try {
			return sourceCodeTextViewConverter.convert(entity);
		} catch (Exception e) {
			log.debug("get source code {} exception {}", entityId, e);
			throw new IllegalArgumentException("load source failed.");
		}
	}

	@GET
	@Path("project/{projectId}/entity/{entityId}/all")
	public SourceCodePureTextView showAllSourceCodeText(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId) {

		log.debug("projectId: {}, EntityId: {}", projectId, entityId);

		Enttity entity = entityService.getEntityByProjectIdAndEntityId(projectId, entityId);
		if (entity == null) {
			throw new IllegalArgumentException("The entity can not be null");
		}

		SourceCodePureTextView convertAll = sourceCodePureTextViewConverter.convertAll(entity);
		if(convertAll == null){
			throw new IllegalArgumentException("The sourceCodePureTextView can not be null");
		}
		
		return convertAll;
	}

	@GET
	@Path("project/{projectId}/entity/{entityId}/firstLine/{firstLineNumber}/interval/{intervalLine}/fewSourceCode")
	public SourceCodePureTextView showFewSourceCodeText(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId, @PathParam("firstLineNumber") Integer firstLineNumber,
			@PathParam("intervalLine") Integer intervalLine) {

		log.debug("projectId: {}, EntityId: {}, firstLine: {}, intervalLine: {}", projectId, entityId, firstLineNumber,
				intervalLine);

		Enttity entity = entityService.getEntityByProjectIdAndEntityId(projectId, entityId);
		if (entity == null) {
			throw new IllegalArgumentException("The entity can not be null");
		}

		try {
			return sourceCodePureTextViewConverter.convert(entity, firstLineNumber,
					intervalLine);

		} catch (Exception e) {
			log.debug("get source code {} exception {}", entityId, e);
			throw new IllegalArgumentException("load source failed.");
		}
	}

	@GET
	@Path("project/{projectId}/parent/{parentId}/parentLineNumber/{parentLineNumber}/self/{selfId}/selfLineNumber/{selfLineNumber}/highlight")
	public List<Integer> findParentHighlightLineNumbers(@PathParam("projectId") Long projectId,
			@PathParam("parentId") Integer parentId, @PathParam("parentLineNumber") Integer parentLineNumber,
			@PathParam("selfId") Integer selfId, @PathParam("selfLineNumber") Integer selfLineNumber) {

		if (selfLineNumber < 0) {
			return Lists.newArrayList(parentLineNumber);
		}

		Enttity parent = entityService.getEntityByProjectIdAndEntityId(projectId, parentId);
		if (parent == null) {
			throw new IllegalArgumentException("Parent can not be null");
		}

		Enttity self = entityService.getEntityByProjectIdAndEntityId(projectId, selfId);
		if (self == null) {
			throw new IllegalArgumentException("Self can not be null");
		}

		List<Integer> highlightLineNumbers = Lists.newArrayList();

		List<EntityRelationship> relations = entityRelationService.findRelationByTargetIdAndSourceId(projectId, selfId,
				parentId);

		boolean isImplement = CollectionUtils.isEmpty(relations);

		if (!isImplement) {
			for (EntityRelationship relation : relations) {
				String relationDetailValue = relation.getAttributeValue(RELATION_DETAIL);
				@SuppressWarnings("unchecked")
				List<Object> detailObjects = GsonConverter.toObject(relationDetailValue, List.class);
				List<RelationDetail> details = GsonConverter.convert(detailObjects, RelationDetail.class);

				for (RelationDetail detail : details) {
					if (detail.getTargetLineNumber() == selfLineNumber) {
						if (detail.getSourceMethodLineNumber() == parentLineNumber) {
							highlightLineNumbers.add(detail.getSourceLineNumber());
						}
					}
				}
			}

			if (CollectionUtils.isEmpty(highlightLineNumbers)) {
				if (parentId.equals(selfId)) {
					highlightLineNumbers.add(parentLineNumber);
				}
			}
		} else {
			highlightLineNumbers.add(parentLineNumber);
		}

		return highlightLineNumbers;
	}

	@GET
	@Path("project/{projectId}/entity/{entityId}/method/tree")
	public List<SourceCodeTreeView> findEntityMethodTree(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId) throws IOException {
		log.debug("projectId: {}, EntityId: {}", projectId, entityId);

		Enttity entity = entityService.getEntityByProjectIdAndEntityId(projectId, entityId);
		try {
			SourceCodeTreeView methodRootNode = prepareMethodRootNode(entity);
			if (methodRootNode == null) {
				return Lists.newLinkedList();
			}

			List<SourceCodeTreeView> resultTree = Lists.newLinkedList();
			CollectionUtils.addIgnoreNull(resultTree, methodRootNode);

			SourceCodeTreeView forwardRootNode = prepareForwardRootNode(projectId, entity);
			CollectionUtils.addIgnoreNull(resultTree, forwardRootNode);

			SourceCodeTreeView backwardRootNode = prepareBackwardRootNode(projectId, entity);
			CollectionUtils.addIgnoreNull(resultTree, backwardRootNode);

			return resultTree;

		} catch (Exception e) {
			log.debug("get source code {} exception {}", entityId, e);
			throw new IllegalArgumentException("load method tree failed.");
		}
	}

	@GET
	@Path("project/{projectId}/entity/{entityId}/forward/{forwardEntityId}/realtionDetail/")
	public List<RelationDetailView> findForwardRelationDetail(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId, @PathParam("forwardEntityId") Integer forwardEntityId) {

		log.debug("projectId: {}, EntityId: {}, forwardEntityId: {}", projectId, entityId, forwardEntityId);

		List<EntityRelationship> relationships = crossProjectEntityRelationService.findRelationshipsBySourceId(entityId);

		if (CollectionUtils.isEmpty(relationships)) {
			return Lists.newArrayList();
		}

		List<RelationDetailView> views = relationDetailViewConverter.convertForwardAndFilterByEntity(relationships,
				forwardEntityId);
		sortBySourceLineNumber(views);

		return views;
	}

	@GET
	@Path("project/{projectId}/entity/{entityId}/backward/{backwardEntityId}/realtionDetail/")
	public List<RelationDetailView> findBackwardRelationDetail(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId, @PathParam("backwardEntityId") Integer backwardEntityId) {

		log.debug("projectId: {}, EntityId: {}, backwardEntityId: {}", projectId, entityId, backwardEntityId);

		List<EntityRelationship> relationships = crossProjectEntityRelationService.findRelationshipsByTargetId(entityId);
		if (CollectionUtils.isEmpty(relationships)) {
			return Lists.newArrayList();
		}

		List<RelationDetailView> views = relationDetailViewConverter.convertBackwardAndFilterByEntity(relationships,
				backwardEntityId);
		sortByLineNumbers(views);

		return views;
	}

	@GET
	@Path("project/{projectId}/changeAnalysisId/{changeAnalysisId}/entityId/{entityId}/Revision/{revision}")
	@Produces(MediaType.APPLICATION_JSON)
	public List<SourceCodeDiffTextView> findDiffByProjectAndEntityIdAndRevision(
			@PathParam(value = "projectId") Long projectId,
			@PathParam(value = "changeAnalysisId") Long changeAnalysisId, @PathParam(value = "entityId") int entityId,
			@PathParam(value = "revision") String revision) throws IOException {

		List<SourceCodeDiffTextView> results = Lists.newArrayList();
		Enttity entity = entityService.getEntityByEntityId(entityId);
		String location = entity.getAttributeValue(ModelAttributeNameEnum.location.name());

		String repositoryType = getRepositoryType(location);

		
		List<List<SourceCodeDiffTextView>> sourceCodeDiffTextViews = Lists.newArrayList();
		List<SourceCodeDiffText> sourceCodeDiffTexts = changeAnalysisService.getDiff(projectId, changeAnalysisId,
				location, revision, entity.getAttributeValue(ModelAttributeNameEnum.extension.name()), repositoryType);
		try {
			if (CollectionUtils.isNotEmpty(sourceCodeDiffTexts)) {
				for (SourceCodeDiffText sourceCodeDiffText : sourceCodeDiffTexts) {
					sourceCodeDiffTextViews.add(sourceCodeDiffTextViewConverter.convert(sourceCodeDiffText));
				}
				results = sourceCodeDiffTextViewConverter.merge(sourceCodeDiffTextViews);
			}
			return results;
		} catch (Exception e) {
			log.debug("Get diff of entity : {} error, cause {}", entityId, e);
		}
		
		return results;
	}

	@GET
	@Path("project/{projectId}/changeAnalysisId/{changeAnalysisId}/dependedEntityId/{dependedEntityId}/Revision/{revision}")
	@Produces(MediaType.APPLICATION_JSON)
	public SourceCodePureTextView findByProjectAndDependedEntityIdAndRevision(
			@PathParam(value = "projectId") Long projectId,
			@PathParam(value = "changeAnalysisId") Long changeAnalysisId,
			@PathParam(value = "dependedEntityId") int dependedEntityId,
			@PathParam(value = "revision") String revision) {
		Enttity dependedEntity = entityService.getEntityByEntityId(dependedEntityId);
		String location = dependedEntity.getAttributeValue(ModelAttributeNameEnum.location.name());
		String repositoryType = getRepositoryType(location);

		try {
			String historicalEntityContent = changeAnalysisService.getHistorycalEntityContent(projectId, changeAnalysisId, repositoryType,
					revision, location);

			return sourceCodePureTextViewConverter.covertFromRepository(dependedEntity, historicalEntityContent);
		} catch (Exception e) {
			log.debug("Get diff of entity : {} error, cause {}", dependedEntityId, e);
		}

		return new SourceCodePureTextView();
	}
	
	@GET
	@Path("project/{projectId}/changeAnalysisId/{changeAnalysisId}/methodName/{methodName}/methodLine/{methodStartLine}")
	@Produces(MediaType.APPLICATION_JSON)
	public Integer findMethodStartLine(@PathParam("projectId") Long projectId,@PathParam("changeAnalysisId") Long changeAnalysisId,
			 @PathParam("methodName") String methodName, @PathParam("methodStartLine") Integer methodStartLine) {
		
		//TODO: 目前先這樣寫，後續需要重構
		//先找有沒有對應的 method line number 
		//如果沒有再找對應的 method name
		//最後找是不是 className
		
		Enttity entity = entityService.getEntityByEntityId(changeAnalysisId.intValue());
		if(entity != null){
			try {
				List<SourceCodeMethodDto> methods = findEntityMethods(entity);
				for(SourceCodeMethodDto method : methods){
					if(method.getStartLine() == methodStartLine){
						return methodStartLine;
					}
				}
				
				for(SourceCodeMethodDto method : methods){
					if(StringUtils.equals(method.getMethodName(), methodName)){
						return method.getStartLine();
					}
				}
				
				if(StringUtils.equals(entity.getName(), methodName)){
					return methodStartLine;
				}
			} catch (Exception e) {
				log.debug("Can't find entity methods, maybe entity is deleted.");
				return null;
			}
		}

		return null;
	}

	private String getRepositoryType(String location) {
		Pattern svnPattern = Pattern.compile("(?i)\\W" + REPOSITORY_TYPE_SVN.getName() + "\\W");
		Pattern gitPattern = Pattern.compile("(?i)\\W" + REPOSITORY_TYPE_GIT.getName() + "\\W");
		Pattern tfsPattern = Pattern.compile("(?i)\\W" + REPOSITORY_TYPE_TFS.getName() + "\\W");

		if (svnPattern.matcher(location).find()) {
			return REPOSITORY_TYPE_SVN.getName();
		} else if (gitPattern.matcher(location).find()) {
			return REPOSITORY_TYPE_GIT.getName();
		} else if (tfsPattern.matcher(location).find()){
			return REPOSITORY_TYPE_TFS.getName();
		}

		return StringUtils.EMPTY;
	}

	private SourceCodeTreeView prepareMethodRootNode(Enttity rootEntity) throws Exception {

		SourceCodeTreeView methodRootNode = sourceCodeTreeViewForEntityConverter.convert(rootEntity);
		if (methodRootNode == null) {
			return null;
		}

		List<SourceCodeMethodDto> entityMethods = findEntityMethods(rootEntity);
		List<SourceCodeTreeView> methodNodes = sourceCodeTreeViewConverter.convert(entityMethods);
		//根據開始行號做排序
		Collections.sort(methodNodes, new Comparator<SourceCodeTreeView>() {
			public int compare(SourceCodeTreeView o1, SourceCodeTreeView o2) {
				return o1.getStartLine() - o2.getStartLine();
			}
		});
		methodRootNode.setItems(methodNodes);

		return methodRootNode;
	}

	private List<SourceCodeMethodDto> findEntityMethods(Enttity entity) throws Exception {

		String attributeValue = entity.getAttributeValue(METHOD);
		@SuppressWarnings("unchecked")
		List<Object> sourceCodeMethodLists = GsonConverter.toObject(attributeValue, List.class);

		return GsonConverter.convert(sourceCodeMethodLists, SourceCodeMethodDto.class);
	}

	private SourceCodeTreeView convertTreeNode(String name, Collection<SourceCodeTreeView> nodes) {
		List<SourceCodeTreeView> nodeList = Lists.newArrayList(nodes);
		SourceCodeTreeView mock = new SourceCodeTreeView();
		mock.setName(name);
		Collections.sort(nodeList);
		mock.setItems(nodeList);
		mock.setHasChildren(CollectionUtils.isNotEmpty(nodes));

		return mock;
	}

	private SourceCodeTreeView prepareForwardRootNode(Long projectId, Enttity entity) {

		List<EntityRelationship> targetRelationships = entity.getTargetEntityRelationships();
		if (CollectionUtils.isEmpty(targetRelationships)) {
			return null;
		}

		Set<SourceCodeTreeView> nodes = Sets.newHashSet();
		for (EntityRelationship relationship : targetRelationships) {

			Enttity target = relationship.getTarget();
			SourceCodeTreeView node = sourceCodeTreeViewForEntityConverter.convert(target);
			if (node != null) {
				node.setRedirect(true);
				node.setForward(true);
				node.setExternal(false);
				// 判斷是不是外部
				if(! projectId.equals(target.getProjectId())){
					Project targetProject = projectService.getProjectById(target.getProjectId());
					node.setExternalProjectName(targetProject.getName());
					node.setExternal(true);
				}

				nodes.add(node);
			}
		}

		return convertTreeNode(FORWARD, nodes);
	}

	private SourceCodeTreeView prepareBackwardRootNode(Long projectId, Enttity entity) {

		List<EntityRelationship> sourceRelationships = entity.getSourceEntityRelationships();
		if (CollectionUtils.isEmpty(sourceRelationships)) {
			return null;
		}

		Set<SourceCodeTreeView> nodes = Sets.newHashSet();
		for (EntityRelationship relationship : sourceRelationships) {

			Enttity source = relationship.getSource();
			SourceCodeTreeView node = sourceCodeTreeViewForEntityConverter.convert(source);
			if (node != null) {
				node.setRedirect(true);
				node.setBackward(true);
				// 判斷是不是外部
				if(! projectId.equals(source.getProjectId())){
					Project sourceProject = projectService.getProjectById(source.getProjectId());
					node.setExternalProjectName(sourceProject.getName());
					node.setExternal(true);
				}

				nodes.add(node);
			}
		}

		return convertTreeNode(BACKWARD, nodes);
	}

	private void sortBySourceLineNumber(List<RelationDetailView> views) {
		Collections.sort(views, new Comparator<RelationDetailView>() {
			@Override
			public int compare(RelationDetailView o1, RelationDetailView o2) {
				return Integer.compare(o1.getSourceEntityLineNumber(), o2.getSourceEntityLineNumber());
			}
		});
	}

	private void sortByLineNumbers(List<RelationDetailView> views) {
		Collections.sort(views, new Comparator<RelationDetailView>() {
			@Override
			public int compare(RelationDetailView o1, RelationDetailView o2) {
				int o1TargetLine = o1.getTargetEntityLineNumber();
				int o2TargetLine = o2.getTargetEntityLineNumber();

				if (Integer.compare(o1TargetLine, o2TargetLine) != 0) {
					return Integer.compare(o1TargetLine, o2TargetLine);
				}

				int o1SourceLine = o1.getSourceEntityLineNumber();
				int o2SourceLine = o2.getSourceEntityLineNumber();

				return Integer.compare(o1SourceLine, o2SourceLine);
			}
		});
	}

}
