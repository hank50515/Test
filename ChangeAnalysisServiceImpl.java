package com.gss.adm.core.prod.service.provider;

import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_SVN_PASSWORD_ID;
import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_SVN_URL_ID;
import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_SVN_USER_NAME_ID;
import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_TFS_COLLECTION_ID;
import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_TFS_PASSWORD_ID;
import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_TFS_PROJECT_ID;
import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_TFS_TEAM_PROJECT_ID;
import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_TFS_URL_ID;
import static com.gss.adm.api.enums.ModelDefinitionIdEnum.PROJECT_MODEL_ATTRIBUTE_TFS_USER_NAME_ID;
import static com.gss.adm.api.enums.ProjectDefinitions.SVN_PASSWORD;
import static com.gss.adm.api.enums.ProjectDefinitions.SVN_URL;
import static com.gss.adm.api.enums.ProjectDefinitions.SVN_USERNAME;
import static com.gss.adm.api.enums.ProjectDefinitions.TFS_COLLECTION;
import static com.gss.adm.api.enums.ProjectDefinitions.TFS_PASSWORD;
import static com.gss.adm.api.enums.ProjectDefinitions.TFS_PROJECT;
import static com.gss.adm.api.enums.ProjectDefinitions.TFS_TEAM_PROJECT;
import static com.gss.adm.api.enums.ProjectDefinitions.TFS_URL;
import static com.gss.adm.api.enums.ProjectDefinitions.TFS_USERNAME;
import static com.gss.adm.api.enums.ProjectDefinitionsEnum.REPOSITORY_TYPE_GIT;
import static com.gss.adm.api.enums.ProjectDefinitionsEnum.REPOSITORY_TYPE_SVN;
import static com.gss.adm.api.enums.ProjectDefinitionsEnum.REPOSITORY_TYPE_TFS;
import static com.gss.adm.api.enums.ProjectDefinitionsEnum.SOURCE_CODE;
import static com.gss.adm.api.enums.ProjectDefinitionsEnum.WORKSPACE;
import static com.gss.adm.api.enums.ProjectModelDefinitionIdEnum.GIT_ID;
import static com.gss.adm.core.prod.utils.ChangeAnalysisUtils.compareTree;
import static com.gss.adm.core.prod.utils.ChangeAnalysisUtils.getDiffByDiffLab;
import static com.gss.adm.core.prod.utils.ChangeAnalysisUtils.getDiffEntryByGit;
import static com.gss.adm.core.prod.utils.ChangeAnalysisUtils.getHistorycalEntityContentFromGit;
import static com.gss.adm.core.prod.utils.ChangeAnalysisUtils.getHistorycalEntityContentFromSVN;
import static com.gss.adm.core.prod.utils.ChangeAnalysisUtils.getRevCommit;
import static com.gss.adm.core.prod.utils.ChangeAnalysisUtils.getSVNRepository;
import static com.gss.adm.core.prod.utils.ChangeAnalysisUtils.getTargetParentIndex;

import java.io.File;
import java.nio.charset.StandardCharsets;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import javax.annotation.PostConstruct;

import org.apache.commons.collections.MapUtils;
import org.apache.commons.lang.StringUtils;
import org.eclipse.jgit.api.Git;
import org.eclipse.jgit.diff.DiffEntry;
import org.eclipse.jgit.lib.ObjectId;
import org.eclipse.jgit.lib.ObjectReader;
import org.eclipse.jgit.lib.Repository;
import org.eclipse.jgit.revwalk.RevCommit;
import org.eclipse.jgit.treewalk.TreeWalk;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.util.CollectionUtils;
import org.tmatesoft.svn.core.SVNException;
import org.tmatesoft.svn.core.SVNURL;
import org.tmatesoft.svn.core.io.SVNRepository;

import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.gss.adm.antlr.utils.FileUtils2;
import com.gss.adm.api.encrypt.AesEncrypter;
import com.gss.adm.api.enums.ProjectDefinitions;
import com.gss.adm.api.enums.ProjectModelDefinitionIdEnum;
import com.gss.adm.api.tsf.TfsConnectionAdvisor;
import com.gss.adm.core.form.ChangeAnalysisQueryCondition;
import com.gss.adm.core.model.ChangeAnalysis;
import com.gss.adm.core.model.ChangeRecord;
import com.gss.adm.core.model.Project;
import com.gss.adm.core.model.ProjectModel;
import com.gss.adm.core.model.ProjectModelAttribute;
import com.gss.adm.core.model.SourceCodeDiffText;
import com.gss.adm.core.prod.dao.ChangeAnalysisDao;
import com.gss.adm.core.prod.dao.condition.ChangeAnalysisCondition;
import com.gss.adm.core.prod.utils.ChangeAnalysisUtils;
import com.gss.adm.core.service.ChangeAnalysisService;
import com.gss.adm.core.service.ProjectModelService;
import com.gss.adm.core.service.ProjectService;
import com.gss.adm.core.service.dicovery.CiFactory;
import com.microsoft.tfs.core.TFSTeamProjectCollection;
import com.microsoft.tfs.core.clients.versioncontrol.VersionControlClient;
import com.microsoft.tfs.core.clients.versioncontrol.soapextensions.DeletedState;
import com.microsoft.tfs.core.clients.versioncontrol.soapextensions.Item;
import com.microsoft.tfs.core.clients.versioncontrol.specs.version.ChangesetVersionSpec;
import com.microsoft.tfs.core.httpclient.Credentials;
import com.microsoft.tfs.core.httpclient.UsernamePasswordCredentials;
import com.microsoft.tfs.core.util.URIUtils;

import difflib.Delta;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
@Transactional
public class ChangeAnalysisServiceImpl implements ChangeAnalysisService {

	@Autowired
	private ProjectModelService projectModelService;

	@Autowired
	private ProjectService projectService;

	@Autowired
	private ChangeAnalysisDao changeAnalysisDao;

	@Autowired
	private CiFactory cifactory;

	@Value("${adm.encrypt.key}")
	protected String encryptKey;

	private Properties properties;

	@PostConstruct
	private void init() {
		this.properties = cifactory.getProperties();
	}

	@Override
	public ChangeAnalysis saveChangeAnalysis(ChangeAnalysis changeAnalysis) {
		return changeAnalysisDao.save(changeAnalysis);
	}

	@Override
	public List<ChangeAnalysis> findChangeAnalysisesByProjectId(long projectId) {
		return changeAnalysisDao.findByProjectId(projectId);
	}

	@Override
	public List<ChangeRecord> findRecordsByProjectIdAndAnalysisId(long projectId, long analysisId) {
		ChangeAnalysis analysis = changeAnalysisDao.getByProjectIdAndAnalysisId(projectId, analysisId);

		return analysis.getChangeRecords();
	}

	@Override
	public int getTotalSizeByProjectId(long projectId) {
		return changeAnalysisDao.getTotalSizeByProjectId(projectId);
	}

	@Override
	public boolean isChangeAnalysisExisted(long projectId, String revision, String commiter) {
		ChangeAnalysis changeAnalysis = changeAnalysisDao.getByProjectIdAndRevisionAndCommiter(projectId, revision,
				commiter);

		return changeAnalysis != null;
	}

	@Override
	public List<SourceCodeDiffText> getDiffFromSVN(long projectId, String location, long revision, String extentionName) {
		List<SourceCodeDiffText> sourceCodeDiffTexts = Lists.newArrayList();
		Project project = projectService.getProjectById(projectId);
		List<ProjectModel> projectModels = projectModelService.findProjectModelByProjectIdAndModelDefinitionId(
				projectId, ProjectModelDefinitionIdEnum.SUBVERSION_ID.getValue());
		for (ProjectModel projectModel : projectModels) {
			List<ProjectModelAttribute> modelAttributes = projectModel.getModelAttribute();
			
			Map<String, String> auth = null;
			try {
				auth = getAuthFromSVN(modelAttributes);
			} catch (Exception e) {
				log.warn("Can not get Auth from svn cause:{} ", e);
				return Lists.newArrayList();
			}

			if (MapUtils.isEmpty(auth)) {
				return Lists.newArrayList();
			}

			SVNRepository repository;
			try {
				repository = getSVNRepository(
						SVNURL.parseURIEncoded(auth.get(ProjectDefinitions.SVN_URL.getName())),
						auth.get(ProjectDefinitions.SVN_USERNAME.getName()),
						auth.get(ProjectDefinitions.SVN_PASSWORD.getName()));
				
				Long currentRevision = revision;
				Long oldRevision = revision - 1;

				String workSpacePath = this.properties.getProperty(WORKSPACE.getName());
				Path localPath = Paths.get(workSpacePath, project.getCode(), SOURCE_CODE.getName(),
						REPOSITORY_TYPE_SVN.getName(), projectModel.getId().toString());

				String fileLocation = ChangeAnalysisUtils.convertToSVNPath(repository, location, localPath.toString());

				String beforeFileContent = getHistorycalEntityContentFromSVN(repository, oldRevision, fileLocation);
				String afterFileContent = getHistorycalEntityContentFromSVN(repository, currentRevision, fileLocation);

				if (StringUtils.isBlank(beforeFileContent) && StringUtils.isBlank(afterFileContent)) {
					continue;
				}

				List<Delta> deltas = getDiffByDiffLab(beforeFileContent, afterFileContent, "\n");
				SourceCodeDiffText sourceCodeDiffText = new SourceCodeDiffText();
				sourceCodeDiffText.setAfterText(afterFileContent);
				sourceCodeDiffText.setBeforeText(beforeFileContent);
				sourceCodeDiffText.setDeltas(deltas);
				sourceCodeDiffText.setExtentionName(extentionName);
				sourceCodeDiffTexts.add(sourceCodeDiffText);
				
				return sourceCodeDiffTexts;
			} catch (SVNException e) {
				log.warn("Can not get svn information cause:{} ", e);
			} catch (Exception e) {
				log.warn("Svn do diff have exception cause:{} ", e);
			}
		}
		return sourceCodeDiffTexts;
	}

	private Map<String, String> getAuthFromSVN(List<ProjectModelAttribute> modelAttributes) throws Exception {
		Map<String, String> auth = Maps.newHashMap();
		for (ProjectModelAttribute projectModelAttribute : modelAttributes) {
			if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_SVN_USER_NAME_ID
					.getValue()) {
				auth.put(SVN_USERNAME.getName(), projectModelAttribute.getAttributeValue());
			} else if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_SVN_PASSWORD_ID
					.getValue()) {
				String svnPassword = AesEncrypter.decrypt(projectModelAttribute.getAttributeValue(), encryptKey);
				auth.put(SVN_PASSWORD.getName(), svnPassword);
			} else if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_SVN_URL_ID
					.getValue()) {
				auth.put(SVN_URL.getName(), projectModelAttribute.getAttributeValue());
			}
		}
		return auth;
	}

	@Override
	public List<SourceCodeDiffText> getDiffFromGIT(long projectId, long changeAnalysisId, String location,
			String revision, String extentionName) {
		List<SourceCodeDiffText> sourceCodeDiffTexts = Lists.newArrayList();
		byte[] afterTextBytes = null;
		String afterText = StringUtils.EMPTY;
		TreeWalk afterTreeWalk = null;

		Project project = projectService.getProjectById(projectId);
		List<ProjectModel> projectModels = projectModelService
				.findProjectModelByProjectIdAndModelDefinitionId(projectId, GIT_ID.getValue());

		List<ChangeAnalysis> sameRevisionResults = findChangeAnalysisByProjectIdAndRevision(projectId, revision);

		Integer targetParentIndex = getTargetParentIndex(sameRevisionResults, changeAnalysisId);

		for (ProjectModel projectModel : projectModels) {
			String workSpacePath = this.properties.getProperty(WORKSPACE.getName());
			Path path = Paths.get(workSpacePath, project.getCode(), SOURCE_CODE.getName(),
					REPOSITORY_TYPE_GIT.getName(), projectModel.getId().toString());

			Git git;
			try {
				git = Git.open(path.toFile());
				Repository repository = git.getRepository();

				RevCommit currentCommit = getRevCommit(repository, revision);

				if (currentCommit == null) {
					return Lists.newArrayList();
				}

				RevCommit parentCommit = currentCommit.getParent(targetParentIndex);
				ObjectId afterObjectId = parentCommit.getId();
				RevCommit beforeCommit = getRevCommit(repository, afterObjectId.getName());
				ObjectReader reader = repository.newObjectReader();
				List<DiffEntry> listDiffs = compareTree(repository, reader, currentCommit.getTree(),
						beforeCommit.getTree());
				if (CollectionUtils.isEmpty(listDiffs)) {
					continue;
				}
				DiffEntry diffEntry = getDiffEntryByGit(listDiffs, path.toString(), location);

				if (diffEntry == null) {
					continue;
				}

				TreeWalk beforeTreeWalk = TreeWalk.forPath(repository, diffEntry.getOldPath(), beforeCommit.getTree());
				byte[] beforeTextBytes = repository.open(beforeTreeWalk.getObjectId(0)).getBytes();

				if (afterTreeWalk == null) {
					// 避免重複
					afterTreeWalk = TreeWalk.forPath(repository, diffEntry.getNewPath(), currentCommit.getTree());
					afterTextBytes = repository.open(afterTreeWalk.getObjectId(0)).getBytes();
					afterText = new String(afterTextBytes, StandardCharsets.UTF_8);
				}
				if (StringUtils.isNotBlank(afterText)) {
					String beforeText = new String(beforeTextBytes, StandardCharsets.UTF_8);
					List<Delta> deltas = getDiffByDiffLab(beforeText, afterText, "\n");
					SourceCodeDiffText sourceCodeDiffText = new SourceCodeDiffText();
					sourceCodeDiffText.setAfterText(afterText);
					sourceCodeDiffText.setBeforeText(beforeText);
					sourceCodeDiffText.setDeltas(deltas);
					sourceCodeDiffText.setExtentionName(extentionName);
					sourceCodeDiffTexts.add(sourceCodeDiffText);
				}
			} catch (Exception e) {
				log.warn("Git do diff have exception cause :{}", e);
			}
			
		}
		return sourceCodeDiffTexts;
	}

	@Override
	public String getHistorycalEntityContent(long projectId, long changeAnalysisId, String repositoryType,
			String revision, String location) throws Exception {
		Project project = projectService.getProjectById(projectId);
		if (StringUtils.equals(REPOSITORY_TYPE_GIT.getName(), repositoryType)) {
			List<ProjectModel> projectModels = projectModelService
					.findProjectModelByProjectIdAndModelDefinitionId(projectId, GIT_ID.getValue());
			List<ChangeAnalysis> sameRevisionResults = findChangeAnalysisByProjectIdAndRevision(projectId, revision);
			Integer targetParentIndex = getTargetParentIndex(sameRevisionResults, changeAnalysisId);

			for (ProjectModel projectModel : projectModels) {
				String workSpacePath = this.properties.getProperty(WORKSPACE.getName());
				Path path = Paths.get(workSpacePath, project.getCode(), SOURCE_CODE.getName(),
						REPOSITORY_TYPE_GIT.getName(), projectModel.getId().toString());

				location = FileUtils2.convertToSameSeparator(location, path.toString());
				location = location.replace(path.toString(), StringUtils.EMPTY);

				Git git = Git.open(path.toFile());
				Repository repository = git.getRepository();
				RevCommit currentCommit = getRevCommit(repository, revision);
				String result = getHistorycalEntityContentFromGit(currentCommit, repository, location,
						targetParentIndex);
				if (StringUtils.isBlank(result)) {
					continue;
				}
				return result;
			}
		} else if (StringUtils.equals(REPOSITORY_TYPE_SVN.getName(), repositoryType)) {
			List<ProjectModel> projectModels = projectModelService.findProjectModelByProjectIdAndModelDefinitionId(
					projectId, ProjectModelDefinitionIdEnum.SUBVERSION_ID.getValue());
			for (ProjectModel projectModel : projectModels) {
				List<ProjectModelAttribute> modelAttributes = projectModel.getModelAttribute();
				Map<String, String> auth = getAuthFromSVN(modelAttributes);

				if (MapUtils.isEmpty(auth)) {
					return StringUtils.EMPTY;
				}

				SVNRepository repository = getSVNRepository(
						SVNURL.parseURIEncoded(auth.get(ProjectDefinitions.SVN_URL.getName())),
						auth.get(ProjectDefinitions.SVN_USERNAME.getName()),
						auth.get(ProjectDefinitions.SVN_PASSWORD.getName()));

				Long oldRevision = Long.valueOf(revision) - 1;

				String workSpacePath = this.properties.getProperty(WORKSPACE.getName());
				Path path = Paths.get(workSpacePath, project.getCode(), SOURCE_CODE.getName(),
						REPOSITORY_TYPE_SVN.getName(), projectModel.getId().toString());

				String fileLocation = ChangeAnalysisUtils.convertToSVNPath(repository, location, path.toString());
				String result = getHistorycalEntityContentFromSVN(repository, oldRevision, fileLocation);
				if (StringUtils.isBlank(result)) {
					continue;
				}
				return result;
			}
		}
		return StringUtils.EMPTY;
	}

	@Override
	public Page<ChangeAnalysis> findChangeAnalysisByQueryConditions(long projectId,
			ChangeAnalysisQueryCondition changeAnalysisQueryCondition, Pageable pageable) {

		Specification<ChangeAnalysis> condition = getSpecificationCondition(projectId, changeAnalysisQueryCondition);
		
		return changeAnalysisDao.findAll(condition, pageable);
	}

	@Override
	public int getTotalSizeByProjectIdQueryCondition(long projectId,
			ChangeAnalysisQueryCondition changeAnalysisQueryCondition) {

		Specification<ChangeAnalysis> condition = getSpecificationCondition(projectId, changeAnalysisQueryCondition);

		// 取出總筆數
		long count = changeAnalysisDao.count(condition);
		int total = (int) count;

		return total;
	}

	@Override
	public List<String> searchAuthorBySearchKey(long projectId, String authorSearchKey) {

		List<String> searchAuthorBySearchKey = changeAnalysisDao.searchAuthorBySearchKey(projectId, authorSearchKey);

		return searchAuthorBySearchKey;
	}

	@Override
	public List<ChangeAnalysis> findChangeAnalysisByProjectIdAndRevision(long projectId, String revision) {
		List<ChangeAnalysis> results = changeAnalysisDao.findByProjectIdAndRevision(projectId, revision);

		return results;
	}

	private Specification<ChangeAnalysis> getSpecificationCondition(Long projectId,
			ChangeAnalysisQueryCondition changeAnalysisQueryCondition) {

		String comment = changeAnalysisQueryCondition.getComment();
		String commiter = changeAnalysisQueryCondition.getAuthor();
		String revision = changeAnalysisQueryCondition.getRevision();
		Date startDate = changeAnalysisQueryCondition.getStartDate();
		Date endDate = changeAnalysisQueryCondition.getEndDate();
		
		ChangeAnalysisCondition searchCondition = new ChangeAnalysisCondition();
		searchCondition.setProjectId(projectId);
		searchCondition.setStartDate(startDate);
		searchCondition.setEndDate(endDate);
		searchCondition.setCommiter(commiter);
		searchCondition.setComment(comment);
		searchCondition.setRevision(revision);
		
		return searchCondition.getSpecification();
	}

	@Override
	public List<SourceCodeDiffText> getDiffFromTFS(long projectId, String location, Integer revision ,String extentionName){
		
		List<SourceCodeDiffText> sourceCodeDiffTexts = Lists.newArrayList();
		Project project = projectService.getProjectById(projectId);
		
		List<ProjectModel> projectModels = projectModelService.findProjectModelByProjectIdAndModelDefinitionId(
				projectId, ProjectModelDefinitionIdEnum.TFS_ID.getValue());
		for (ProjectModel projectModel : projectModels) {
			List<ProjectModelAttribute> modelAttributes = projectModel.getModelAttribute();
			
			Map<String, String> auth = null;
			try {
				auth = getAuthFromTFS(modelAttributes);
			} catch (Exception e) {
				log.warn("Can not get Auth from tfs cause:{} ", e);
				return Lists.newArrayList();
			}
			
			if (MapUtils.isEmpty(auth)) {
				return Lists.newArrayList();
			}

			TFSTeamProjectCollection tfsTeamProjectCollection = getTFSTeamProjectCollection(auth);
			VersionControlClient versionControlClient = tfsTeamProjectCollection.getVersionControlClient();
			
			Integer currentRevision = revision;
			Integer oldRevision = revision - 1;

			String workSpacePath = this.properties.getProperty(WORKSPACE.getName());
			String localPath = Paths.get(workSpacePath, project.getCode(), SOURCE_CODE.getName(),
					REPOSITORY_TYPE_TFS.getName(), projectModel.getId().toString()).toString();
			String filePath = StringUtils.replace(location, localPath, "");
			
			String afterFileContent = getContent(versionControlClient, currentRevision, filePath, auth);
			String beforeFileContent = getContent(versionControlClient, oldRevision, filePath, auth);

			try{
				tfsTeamProjectCollection.close();
			} catch (Exception e) {
				log.warn("Can't close tfs connection cause: {}", e);
			}
			
			if (StringUtils.isBlank(beforeFileContent) && StringUtils.isBlank(afterFileContent)) {
				continue;
			}

			List<Delta> deltas = getDiffByDiffLab(beforeFileContent, afterFileContent, "\n");
			SourceCodeDiffText sourceCodeDiffText = new SourceCodeDiffText();
			sourceCodeDiffText.setAfterText(afterFileContent);
			sourceCodeDiffText.setBeforeText(beforeFileContent);
			sourceCodeDiffText.setDeltas(deltas);
			sourceCodeDiffText.setExtentionName(extentionName);
			sourceCodeDiffTexts.add(sourceCodeDiffText);
			
			return sourceCodeDiffTexts;
		}
		return sourceCodeDiffTexts;
	}
	
	private Map<String, String> getAuthFromTFS(List<ProjectModelAttribute> modelAttributes) throws Exception {
		Map<String, String> auth = Maps.newHashMap();
		for (ProjectModelAttribute projectModelAttribute : modelAttributes) {
			if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_TFS_USER_NAME_ID
					.getValue()) {
				auth.put(TFS_USERNAME.getName(), projectModelAttribute.getAttributeValue());
			} else if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_TFS_PASSWORD_ID
					.getValue()) {
				String svnPassword = AesEncrypter.decrypt(projectModelAttribute.getAttributeValue(), encryptKey);
				auth.put(TFS_PASSWORD.getName(), svnPassword);
			} else if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_TFS_URL_ID
					.getValue()) {
				auth.put(TFS_URL.getName(), projectModelAttribute.getAttributeValue());
			} else if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_TFS_COLLECTION_ID
					.getValue()) {
				auth.put(TFS_COLLECTION.getName(), projectModelAttribute.getAttributeValue());
			} else if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_TFS_TEAM_PROJECT_ID
					.getValue()) {
				auth.put(TFS_TEAM_PROJECT.getName(), projectModelAttribute.getAttributeValue());
			} else if (projectModelAttribute.getAttributeDefinition().getId() == PROJECT_MODEL_ATTRIBUTE_TFS_PROJECT_ID
					.getValue()) {
				auth.put(TFS_PROJECT.getName(), projectModelAttribute.getAttributeValue());
			} 
			
		}
		return auth;
	}
	
	private TFSTeamProjectCollection getTFSTeamProjectCollection(Map<String, String> auth){
		String url = auth.get(ProjectDefinitions.TFS_URL.getName());
		String username = auth.get(ProjectDefinitions.TFS_USERNAME.getName());
		String decryptedPassword = auth.get(ProjectDefinitions.TFS_PASSWORD.getName());
		String collection = auth.get(ProjectDefinitions.TFS_COLLECTION.getName());
		
		String collectionUrlFormat = "%s/%s";
		String collectionUrl = String.format(collectionUrlFormat, url, collection);
		Credentials credentials = new UsernamePasswordCredentials(username, decryptedPassword);
		TfsConnectionAdvisor connectionAdvisor = new TfsConnectionAdvisor();

		return new TFSTeamProjectCollection(URIUtils.newURI(collectionUrl), credentials, connectionAdvisor);
	}
	
	private String getContent(VersionControlClient versionControlClient, int version, String filePath, Map<String, String> auth){

		try {
			String serverPathPattern = "$/%s/%s";
			String tfsTeamProject = auth.get(ProjectDefinitions.TFS_TEAM_PROJECT.getName());
			String tfsProject = auth.get(ProjectDefinitions.TFS_PROJECT.getName());
			String serverPath = String.format(serverPathPattern, tfsTeamProject, tfsProject);
			filePath = StringUtils.replace(filePath, "\\", "/");
			
			Item item = versionControlClient.getItem(serverPath + filePath, new ChangesetVersionSpec(version), DeletedState.NON_DELETED);
			File downloadFileToTempLocation = item.downloadFileToTempLocation(versionControlClient, item.getServerItem());
			String contentByFile = FileUtils2.readFile(downloadFileToTempLocation);
			
			return contentByFile;
		} catch (Exception e){
			log.info("Can't find file");
		}
		
		return StringUtils.EMPTY;
	}
}
