package com.gss.adm.web.jsf.actions;


import lombok.extern.slf4j.Slf4j;

/**
 * @author Taoyu_Wu
 */
@Slf4j
@Component
@Path("sourceCode")
public class SourceCodeAction implements ModelDefinitions {


	@GET
	@Path("project/{projectId}/entity/{entityId}")
	public SourceCodeTextView showSourceCodeText(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId) {

		log.debug("projectId: {}, EntityId: {}", projectId, entityId);

	}

	@GET
	@Path("project/{projectId}/entity/{entityId}/all")
	public SourceCodePureTextView showAllSourceCodeText(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId) {

		log.debug("projectId: {}, EntityId: {}", projectId, entityId);

	}

	@GET
	@Path("project/{projectId}/entity/{entityId}/firstLine/{firstLineNumber}/interval/{intervalLine}/fewSourceCode")
	public SourceCodePureTextView showFewSourceCodeText(@PathParam("projectId") Long projectId,
			@PathParam("entityId") Integer entityId, @PathParam("firstLineNumber") Integer firstLineNumber,
			@PathParam("intervalLine") Integer intervalLine) {

		log.debug("projectId: {}, EntityId: {}, firstLine: {}, intervalLine: {}", projectId, entityId, firstLineNumber,
				intervalLine);
	}

	@GET
	@Path("project/{projectId}/parent/{parentId}/parentLineNumber/{parentLineNumber}/self/{selfId}/selfLineNumber/{selfLineNumber}/highlight")
	public List<Integer> findParentHighlightLineNumbers(@PathParam("projectId") Long projectId,
			@PathParam("parentId") Integer parentId, @PathParam("parentLineNumber") Integer parentLineNumber,
			@PathParam("selfId") Integer selfId, @PathParam("selfLineNumber") Integer selfLineNumber) {

		
		return null;
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

			SourceCodeTreeView forwardRootNode = prepareForwardRootNode(entity);
			CollectionUtils.addIgnoreNull(resultTree, forwardRootNode);

			SourceCodeTreeView backwardRootNode = prepareBackwardRootNode(entity);
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

		List<EntityRelationship> relationships = entityRelationService.findRelationshipBySourceId(projectId, entityId);

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

		List<EntityRelationship> relationships = entityRelationService.findRelationshipByTargetId(projectId, entityId);
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
			@PathParam(value = "revision") String revision) {

		List<SourceCodeDiffTextView> results = Lists.newArrayList();
		Enttity entity = entityService.getEntityByEntityId(entityId);
		String location = entity.getAttributeValue(ModelAttributeNameEnum.location.name());

		String repositoryType = getRepositoryType(location);

		try {
			List<List<SourceCodeDiffTextView>> sourceCodeDiffTextViews = Lists.newArrayList();
			List<SourceCodeDiffText> sourceCodeDiffTexts = Lists.newArrayList();
			if (StringUtils.equals(repositoryType, ProjectDefinitions.REPOSITORY_TYPE_SVN.getName())) {
				sourceCodeDiffTexts = changeAnalysisService.getDiffFromSVN(projectId, location, Long.valueOf(revision),
						entity.getAttributeValue(ModelAttributeNameEnum.extension.name()));
			} else if (StringUtils.equals(repositoryType, ProjectDefinitions.REPOSITORY_TYPE_GIT.getName())) {
				sourceCodeDiffTexts = changeAnalysisService.getDiffFromGIT(projectId, changeAnalysisId, location,
						revision, entity.getAttributeValue(ModelAttributeNameEnum.extension.name()));
			} else if (StringUtils.equals(repositoryType, ProjectDefinitions.REPOSITORY_TYPE_TFS.getName())){
				sourceCodeDiffTexts = changeAnalysisService.getDiffFromTFS(projectId, location, Integer.valueOf(revision),
						entity.getAttributeValue(ModelAttributeNameEnum.extension.name()));
			}
			
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
			SourceCodePureTextView sourceCodePureTextView = sourceCodePureTextViewConverter
					.covertFromRepository(dependedEntity, historicalEntityContent);

			return sourceCodePureTextView;
		} catch (Exception e) {
			log.debug("Get diff of entity : {} error, cause {}", dependedEntityId, e);
		}

		return new SourceCodePureTextView();
	}
	
	@GET
	@Path("project/{projectId}/changeAnalysisId/{changeAnalysisId}/methodName/{methodName}")
	@Produces(MediaType.APPLICATION_JSON)
	public Integer findMethodStartLine(@PathParam("projectId") Long projectId,
			@PathParam("changeAnalysisId") Long changeAnalysisId, @PathParam("methodName") String methodName) {
		
		Enttity entity = entityService.getEntityByEntityId(changeAnalysisId.intValue());
		if(entity != null){
			try {
				List<SourceCodeMethodDto> methods = findEntityMethods(entity);
				for(SourceCodeMethodDto method : methods){
					if(StringUtils.equals(method.getMethodName(), methodName)){
						return method.getStartLine();
					}
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

		SourceCodeTreeView methodRootNode = sourceCodeTreeViewConverter.convert(rootEntity);
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

	private SourceCodeTreeView prepareForwardRootNode(Enttity entity) {

		List<EntityRelationship> targetRelationships = entity.getTargetEntityRelationships();
		if (CollectionUtils.isEmpty(targetRelationships)) {
			return null;
		}

		String FORWARD = "Forward";

		Set<SourceCodeTreeView> nodes = Sets.newHashSet();
		for (EntityRelationship relationship : targetRelationships) {

			Enttity target = relationship.getTarget();
			SourceCodeTreeView node = sourceCodeTreeViewConverter.convert(target);
			if (node != null) {
				node.setRedirect(true);
				node.setForward(true);

				nodes.add(node);
			}
		}

		return convertTreeNode(FORWARD, nodes);
	}

	private SourceCodeTreeView prepareBackwardRootNode(Enttity entity) {

		List<EntityRelationship> sourceRelationships = entity.getSourceEntityRelationships();
		if (CollectionUtils.isEmpty(sourceRelationships)) {
			return null;
		}

		String BACKWARD = "Backward";

		Set<SourceCodeTreeView> nodes = Sets.newHashSet();
		for (EntityRelationship relationship : sourceRelationships) {

			Enttity source = relationship.getSource();
			SourceCodeTreeView node = sourceCodeTreeViewConverter.convert(source);
			if (node != null) {
				node.setRedirect(true);
				node.setBackward(true);

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
