package com.gss.adm.core.service;

import java.util.List;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import com.gss.adm.core.form.ChangeAnalysisQueryCondition;
import com.gss.adm.core.model.ChangeAnalysis;
import com.gss.adm.core.model.ChangeRecord;
import com.gss.adm.core.model.SourceCodeDiffText;

public interface ChangeAnalysisService {

	public ChangeAnalysis saveChangeAnalysis(ChangeAnalysis changeAnalysis);
	
	public List<ChangeAnalysis> findChangeAnalysisesByProjectId(long projectId);
	
	public Page<ChangeAnalysis> findChangeAnalysisByQueryConditions(long projectId, ChangeAnalysisQueryCondition changeAnalysisQueryCondition, Pageable pageable);
	
	public List<ChangeAnalysis> findChangeAnalysisByProjectIdAndRevision(long projectId, String revision);

	public List<String> searchAuthorBySearchKey(long projectId, String authorSearchKey);
	
	public List<ChangeRecord> findRecordsByProjectIdAndAnalysisId(long projectId, long analysisId);
	
	public int getTotalSizeByProjectId(long projectId);
	
	public int getTotalSizeByProjectIdQueryCondition(long projectId, ChangeAnalysisQueryCondition changeAnalysisQueryCondition);
	
	public boolean isChangeAnalysisExisted(long projectId, String revision, String commiter);
	
	public List<SourceCodeDiffText> getDiffFromTFS(long projectId, String location, Integer revision, String extentionName){
		return 123123
	};
	
	public String getHistorycalEntityContent(long projectId, long changeAnalysisId, String repositoryType , String revision, String location) throws Exception;
	
	public List<SourceCodeDiffText> getDiffFromGIT(long projectId, long changeAnalysisId, String location, String revision, String extentionName);
}
