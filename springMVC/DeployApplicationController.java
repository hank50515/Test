package com.gss.casip.web.controller.deployapplication;

import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.validation.Valid;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang3.StringUtils;
import org.joda.time.format.ISODateTimeFormat;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;

import com.google.common.base.Joiner;
import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.google.common.collect.Sets;
import com.gss.casip.core.api.condition.ClosedListSearchCondition;
import com.gss.casip.core.api.enums.ApprovalStatusEnum;
import com.gss.casip.core.api.enums.FlowStatusEnum;
import com.gss.casip.core.api.spring.security.AuthenticationUtils;
import com.gss.casip.core.service.AnalysisReportService;
import com.gss.casip.core.service.ApprovalRecordService;
import com.gss.casip.core.service.ChangeItemService;
import com.gss.casip.core.service.DepartmentService;
import com.gss.casip.core.service.DeployApplicationService;
import com.gss.casip.core.service.JobAssociationService;
import com.gss.casip.core.service.MergeRecordService;
import com.gss.casip.core.service.THCBChangedProgramService;
import com.gss.casip.core.service.UserService;
import com.gss.casip.model.domain.AnalysisReport;
import com.gss.casip.model.domain.ApprovalRecord;
import com.gss.casip.model.domain.ChangeItem;
import com.gss.casip.model.domain.Department;
import com.gss.casip.model.domain.DepartmentId;
import com.gss.casip.model.domain.DeployApplication;
import com.gss.casip.model.domain.DeployApplicationSearch;
import com.gss.casip.model.domain.JobAssociation;
import com.gss.casip.model.domain.JobMapping;
import com.gss.casip.model.domain.RequirementApplicationRelation;
import com.gss.casip.model.domain.THCBChangedProgram;
import com.gss.casip.model.domain.Users;
import com.gss.casip.model.domain.dto.DeployApplicationDto;
import com.gss.casip.web.domain.ApprovalRecordView;
import com.gss.casip.web.domain.CC23DeployApplicationView;
import com.gss.casip.web.domain.DeployApplicationView;
import com.gss.casip.web.domain.converter.ApprovalRecordViewConverter;
import com.gss.casip.web.domain.converter.CC23DeployApplicationViewConverter;
import com.gss.casip.web.domain.converter.DeployApplicationDtoViewConverter;
import com.gss.casip.web.domain.converter.DeployApplicationSearchViewConverter;
import com.gss.casip.web.domain.converter.DeployApplicationViewConverter;

import lombok.extern.log4j.Log4j;

@Controller
@Log4j
@RequestMapping("deployApplication/")
public class DeployApplicationController {

	@Autowired
	DeployApplicationService deployApplicationService;

	@Autowired
	ApprovalRecordService approvalRecordService;

	@Autowired
	UserService userService;
	
	@Autowired
	THCBChangedProgramService thcbChangedProgramService;

	@Autowired
	DeployApplicationSearchViewConverter deployApplicationSearchViewConverter;

	@Autowired
	DeployApplicationViewConverter deployApplicationViewConverter;

	@Autowired
	CC23DeployApplicationViewConverter cc23DeployApplicationViewConverter;

	@Autowired
	DeployApplicationDtoViewConverter deployApplicationDtoViewConverter;

	@Autowired
	ApprovalRecordViewConverter approvalRecordViewConverter;

	@Autowired
	private ChangeItemService changeItemService;

	@Autowired
	private DepartmentService departmentService;

	@Autowired
	private AnalysisReportService analysisReportService;

	@Autowired
	private JobAssociationService jobAssociationService;
	
	@Autowired
	private MergeRecordService mergeRecordService;

	@ResponseBody
	@RequestMapping(value = "{applicationNo}", method = RequestMethod.GET)
	public DeployApplicationView getByApplicationNo(@PathVariable String applicationNo) {
		DeployApplication deployApplication = deployApplicationService.getByApplicationNo(applicationNo);

		return deployApplicationViewConverter.toView(deployApplication);
	}

	@ResponseBody
	@RequestMapping(value = "requirementNos", method = RequestMethod.GET)
	public Set<String> findAllRequirementNos() {

		Set<String> requirementNos = Sets.newLinkedHashSet();

		List<DeployApplication> deployApplications = deployApplicationService.findAll();

		for (DeployApplication deployApplication : deployApplications) {
			requirementNos.add(deployApplication.getRequirementApplicationRelation().getRequirementNo());
		}

		return requirementNos;
	}
	
	
	@ResponseBody
	@RequestMapping(value = "departments", method = RequestMethod.GET)
	public Set<HashMap<String, String>> findAllDepartments() {

		Set<HashMap<String, String>> departments = Sets.newLinkedHashSet();

		List<Department> dept = departmentService.findITDepartment();
		for (Department department : dept) {
			HashMap<String, String> dep = Maps.newHashMap();
			DepartmentId deptId = department.getKey();
			dep.put("departmentId", deptId.getDepartmentId());
			dep.put("departmentName", department.getDepartmentName());
			departments.add(dep);
		}

		return departments;
	}
	
		@ResponseBody
	@RequestMapping(value = "applicationNos", method = RequestMethod.GET)
	public List<String> findAllApplicationNos() {

		List<String> applicationNos = Lists.newArrayList();

		List<DeployApplication> deployApplications = deployApplicationService.findAll();

		for (DeployApplication deployApplication : deployApplications) {
			applicationNos.add(deployApplication.getApplicationNo());
		}

		return applicationNos;
	}

	
	@ResponseBody
	@RequestMapping(value = "report/departments", method = RequestMethod.GET)
	public Set<String> findReportDepartments() {

		Set<String> departments = Sets.newLinkedHashSet();

		String username = AuthenticationUtils.getUsername();

		// TODO 不應限制申請人，暫時先讀取所有資料，會有效能問題，後續修改
		List<Department> dept = departmentService.findAll();
		for (Department department : dept) {
			DepartmentId deptId = department.getKey();
			departments.add(String.format("%s %s", deptId.getDepartmentId(), department.getDepartmentName()));
		}

		return departments;
	}

	@ResponseBody
	@RequestMapping(value = "applicants", method = RequestMethod.GET)
	public Set<HashMap<String, String>> findAllApplicants() {
		List<Users> users = userService.findAll();

		Set<HashMap<String, String>> applicants = Sets.newLinkedHashSet();

		List<DeployApplication> deployApplications = deployApplicationService.findAll();

		for (DeployApplication deployApplication : deployApplications) {
			Users user = getUserById(deployApplication.getApplicant(), users);

			if (user != null) {
				HashMap<String, String> applicant = Maps.newHashMap();
				applicant.put("applicantFullName", user.getUsername() + " " + user.getName());
				applicant.put("applicant", user.getUsername());
				applicants.add(applicant);
			}
		}

		return applicants;
	}

	@ResponseBody
	@RequestMapping(value = "systemKind", method = RequestMethod.GET)
	public Set<String> findAllSystemKinds() {

		Set<String> systemKinds = Sets.newLinkedHashSet();
		
		List<DeployApplication> deployApplications = deployApplicationService.findAll();

		for (DeployApplication deployApplication : deployApplications) {
			
			String systemKind = deployApplication.getSystemKind();
			if (StringUtils.isEmpty(systemKind)) {
				systemKind = StringUtils.EMPTY;
			}
			systemKinds.add(systemKind);
		}

		return systemKinds;
	}
	
	@ResponseBody
	@RequestMapping(value = "description", method = RequestMethod.GET)
	public Set<String> findAllDescriptions() {

		Set<String> descriptions = Sets.newLinkedHashSet();

		List<DeployApplication> deployApplications = deployApplicationService.findAll();

		for (DeployApplication deployApplication : deployApplications) {
			String description = deployApplication.getDescription();
			if (StringUtils.isEmpty(description)) {
				description = StringUtils.EMPTY;
			}
			descriptions.add(description);
		}

		return descriptions;
	}
	
	
	@ResponseBody
	@RequestMapping(value = "condition", method = RequestMethod.POST)
	public List<DeployApplicationView> findByCondition(@RequestBody ClosedListSearchCondition searchCondition) {
		
		String requirementType = searchCondition.getRequirementType();
		
		List<DeployApplicationSearch> serches = Lists.newArrayList();
		List<DeployApplicationSearch> deployApplicationSearches = deployApplicationService.searchDeployApplication(searchCondition);
		if (StringUtils.isNotEmpty(requirementType)) {
			for (DeployApplicationSearch deployApplicationSearch : deployApplicationSearches) {
				String applicationNo = deployApplicationSearch.getApplicationNo();
				if (StringUtils.equals(requirementType, "OPEN")) {
					String regex = ".*CC-35.*";
					if (applicationNo.matches(regex)) {
						serches.add(deployApplicationSearch);
					}
				} else if (StringUtils.equals(requirementType, "TW")){
					String regex = ".*CC-23.*";
					if (applicationNo.matches(regex)) {
						serches.add(deployApplicationSearch);
					}
				}
			}
		} else {
			serches = deployApplicationSearches;
		}
		
		List<DeployApplicationView> views = deployApplicationSearchViewConverter.toViews(serches);

		return views;
	}

	@ResponseBody
	@RequestMapping(value = "status/{status}", method = RequestMethod.GET)
	public List<DeployApplicationView> findByEmployeeIdAndStatus(@PathVariable String status) {

		String username = AuthenticationUtils.getUsername();

		FlowStatusEnum statusEnum = FlowStatusEnum.valueOf(status);
		if (statusEnum == null) {
			throw new IllegalArgumentException("Can't find the enum with value: " + status);
		}

		List<DeployApplicationView> applicationViews = Lists.newArrayList();
		if (statusEnum.equals(FlowStatusEnum.UNSIGN)) {
			List<DeployApplication> deployApplications = deployApplicationService.findByApproverAndApprovalStatus(username, statusEnum);

			applicationViews = deployApplicationViewConverter.toViews(deployApplications);

		} else if (statusEnum.equals(FlowStatusEnum.UNREPOSITORY) || statusEnum.equals(FlowStatusEnum.UNCLOSED)) {

			String curSignLevel = "0";
			username = "%%";

			List<DeployApplicationDto> deployApplication = deployApplicationService.findByStatusAndTranformApprovalStatus(statusEnum,
					curSignLevel, username);

			applicationViews = deployApplicationDtoViewConverter.toViews(deployApplication);

		} else {
			List<DeployApplication> deployApplications = deployApplicationService.findByEmployeeIdAndStatus(username, statusEnum);

			applicationViews = deployApplicationViewConverter.toViews(deployApplications);

		}

		return applicationViews;
	}

	@ResponseBody
	@RequestMapping(value = "related/{applicationNo}", method = RequestMethod.GET)
	public List<DeployApplicationView> findRelatedByApplicationNo(@PathVariable String applicationNo) {

		List<DeployApplication> relatedApplications = deployApplicationService.findRelatedApplications(applicationNo);

		return deployApplicationViewConverter.toViews(relatedApplications);
	}

	@ResponseBody
	@RequestMapping(value = "rejectToApplicant", method = RequestMethod.POST)
	public boolean doRejectToApplicant(@Valid @RequestBody ApprovalRecordView approvalView) {
		
		String opinion = approvalView.getOpinion();
		if (StringUtils.isEmpty(opinion)) {
			return false;
		}
		ApprovalRecord approvalRecord = approvalRecordViewConverter.toDto(approvalView);

		String applicationNo = approvalView.getApplicationNo();
		DeployApplication deployApplication = deployApplicationService.getByApplicationNo(applicationNo);

		deployApplicationService.rejectToApplicant(deployApplication, approvalRecord);

		return true;
	}

	@ResponseBody
	@RequestMapping(value = "backwardPrevious", method = RequestMethod.POST)
	public boolean doBackwardPrevious(@Valid @RequestBody ApprovalRecordView approvalView) {
		
		String opinion = approvalView.getOpinion();
		if (StringUtils.isEmpty(opinion)) {
			return false;
		}
		ApprovalRecord approvalRecord = approvalRecordViewConverter.toDto(approvalView);

		String applicationNo = approvalView.getApplicationNo();
		DeployApplication deployApplication = deployApplicationService.getByApplicationNo(applicationNo);

		deployApplicationService.backwardPreviousStep(deployApplication, approvalRecord);

		return true;
	}

	@ResponseBody
	@RequestMapping(value = "fowardNext", method = RequestMethod.POST)
	public boolean doFowardNext(@Valid @RequestBody ApprovalRecordView approvalView) {
		ApprovalRecord approvalRecord = approvalRecordViewConverter.toDto(approvalView);

		String applicationNo = approvalView.getApplicationNo();
		DeployApplication deployApplication = deployApplicationService.getByApplicationNo(applicationNo);

		deployApplicationService.fowardNextStep(deployApplication, approvalRecord);

		return true;
	}

	@ResponseBody
	@RequestMapping(value = "underStatusNos", method = RequestMethod.GET)
	public List<String> findUnderStatusNos(@RequestParam(value = "applicationNo") String applicationNo) {

		List<String> unfinishedApplicationNos = deployApplicationService.findUnfinishedApplicationNos(applicationNo);

		return unfinishedApplicationNos;
	}

	@ResponseBody
	@RequestMapping(value = "repository", method = RequestMethod.POST)
	public List<String> doSaveToRepository(@Valid @RequestBody ApprovalRecordView approvalView) {

		String applicationNo = approvalView.getApplicationNo();
		List<String> errorMessage = Lists.newArrayList();

		// 檢查必要報表有沒有存在
		FlowStatusEnum phaseEnum = FlowStatusEnum.valueOf("UNREPOSITORY");
		Map<ChangeItem, List<JobMapping>> missedJobMappings = analysisReportService.checkRequiredReport(phaseEnum, applicationNo);

		errorMessage = composeMissingReportsErrorMessage(missedJobMappings);
		if (CollectionUtils.isNotEmpty(errorMessage)) {
			errorMessage.add(0, "requiredReportNotExist");

			return errorMessage;
		}
		
		DeployApplication deployApplication = deployApplicationService.getByApplicationNo(applicationNo);
		RequirementApplicationRelation requirement = deployApplication.getRequirementApplicationRelation();
		String requirementNo = requirement.getRequirementNo();
		String requirementSequence = requirement.getRequirementSequence();
		
		if (StringUtils.isNoneEmpty(approvalView.getCheckinTime())) {
			deployApplication.setCheckinTime(ISODateTimeFormat.dateTime().parseDateTime(approvalView.getCheckinTime()).toDate());
		} else {
			errorMessage.add(0, "請填寫入館日期");
			return errorMessage;
		}

		int numberOfUpdate = mergeRecordService.closeDeployForm(requirementNo, requirementSequence);
		
		if (numberOfUpdate == 0) {
			errorMessage.add(0, "執行DeployForm_Close失敗");
			return errorMessage;
		}
		
		ApprovalRecord approvalRecord = approvalRecordViewConverter.toDto(approvalView);
		
		deployApplicationService.fowardNextStep(deployApplication, approvalRecord);
		
		return errorMessage;
	}

	@ResponseBody
	@RequestMapping(value = "CC23submitViewDetail", method = RequestMethod.POST)
	public List<String> doCC23submitDetail(@Valid @RequestBody CC23DeployApplicationView applicationView) {

		String applicationNo = applicationView.getApplicationNo();
		List<String> errorMessage = Lists.newArrayList();
		// 檢查是否有ONLINE類型的程式 
		List<ChangeItem> twChangeItems = changeItemService.findByApplicationNo(applicationNo);
		// 台幣主機只會有一個changeItem
		List<THCBChangedProgram> programs = thcbChangedProgramService.findByChangeItem(twChangeItems.get(0));
		
		// 檢查程式清單是否有一個以上ONLINE類型的程式(SLONLINE、SLHPLONLINE、XALONLINE、XALHPLONLINE)
		boolean isOnline = false;
		boolean isOnlineLink = false;
		for (THCBChangedProgram program : programs) {
			String programType = program.getProgramType();
			// 檢查是否有....ONLINE
			String regex = ".*ONLINE";
			if (programType.matches(regex)) {
				isOnline = true;
			}
			
			// 檢查是否有ONLINELINK
			String linkRegex = "ONLINELINK.*";
			if (programType.matches(linkRegex)) {
				isOnlineLink = true;
			}
		}
		
		// 若有必須有一個ONLINELINK的項目(ONLINELINK_TPA、ONLINELINK_TPR、ONLINELINK_TPS、ONLINELINK_XALLMAIN) 才能送出
		if (isOnline) {
			if (!isOnlineLink) {
				// 沒有ONLINELINK要提示錯誤訊息
				errorMessage.add(0, "缺少ONLINELINK項目，請補充程式清單。");
				return errorMessage;
			}
		}
		
		
		// 檢查必要報表有沒有存在
		FlowStatusEnum phaseEnum = FlowStatusEnum.valueOf("UNDEPLOYED");
		Map<ChangeItem, List<JobMapping>> missedJobMappings = analysisReportService.checkRequiredReport(phaseEnum, applicationNo);

		errorMessage = composeMissingReportsErrorMessage(missedJobMappings);
		if (CollectionUtils.isNotEmpty(errorMessage)) {
			errorMessage.add(0, "requiredReportNotExist");

		} else {
			DeployApplication dto = cc23DeployApplicationViewConverter.toDto(applicationView);
			// 同步入庫版次及入館版次
			List<ChangeItem> changeItems = dto.getChangeItems();
			for (ChangeItem changeItem : changeItems) {
				changeItem.setReleaseVersion(changeItem.getDevelopmentVersion());
				changeItemService.saveChangeItem(changeItem);
			}
			ApprovalRecord approvalRecord = new ApprovalRecord();
			approvalRecord.setDeployApplication(dto);
			approvalRecord.setStatus(dto.getStatus());
			approvalRecord.setCurSignLevel(null);
			approvalRecord.setAction(ApprovalStatusEnum.approval.name());
			approvalRecord.setCreateDate(new Date());
			approvalRecord.setApprovalDate(new Date());
			approvalRecord.setApprover(applicationView.getApplicant());
			approvalRecord.setCreator(applicationView.getApplicant());

			approvalRecordService.saveApprovalRecord(approvalRecord);
			deployApplicationService.fowardNextStep(dto);
		}

		return errorMessage;
	}

	@ResponseBody
	@RequestMapping(value = "submitViewDetail", method = RequestMethod.POST)
	public List<String> doSubmitDetail(@Valid @RequestBody DeployApplicationView applicationView) {

		String applicationNo = applicationView.getApplicationNo();
		List<String> errorMessage = Lists.newArrayList();

		// 檢查必要報表有沒有存在
		FlowStatusEnum phaseEnum = FlowStatusEnum.valueOf("UNDEPLOYED");
		Map<ChangeItem, List<JobMapping>> missedJobMappings = analysisReportService.checkRequiredReport(phaseEnum, applicationNo);

		errorMessage = composeMissingReportsErrorMessage(missedJobMappings);
		if (CollectionUtils.isNotEmpty(errorMessage)) {
			errorMessage.add(0, "requiredReportNotExist");

		} else {
			DeployApplication dto = deployApplicationViewConverter.toDto(applicationView);
			// 同步入庫版次及入館版次
			List<ChangeItem> changeItems = dto.getChangeItems();
			for (ChangeItem changeItem : changeItems) {
				changeItem.setReleaseVersion(changeItem.getDevelopmentVersion());
				changeItemService.saveChangeItem(changeItem);
			}
			ApprovalRecord approvalRecord = new ApprovalRecord();
			approvalRecord.setDeployApplication(dto);
			approvalRecord.setStatus(dto.getStatus());
			approvalRecord.setCurSignLevel(null);
			approvalRecord.setAction(ApprovalStatusEnum.approval.name());
			approvalRecord.setCreateDate(new Date());
			approvalRecord.setApprovalDate(new Date());
			approvalRecord.setApprover(applicationView.getApplicant());
			approvalRecord.setCreator(applicationView.getApplicant());

			approvalRecordService.saveApprovalRecord(approvalRecord);
			deployApplicationService.fowardNextStep(dto);
		}

		return errorMessage;
	}

	@ResponseBody
	@RequestMapping(value = "closeDeployApplication", method = RequestMethod.POST)
	public List<String> doCloseDeployApplication(@Valid @RequestBody ApprovalRecordView approvalView) {

		String applicationNo = approvalView.getApplicationNo();
		List<String> errorMessage = Lists.newArrayList();

		// 檢查必要報表有沒有存在
		FlowStatusEnum phaseEnum = FlowStatusEnum.valueOf("UNCLOSED");
		Map<ChangeItem, List<JobMapping>> missedJobMappings = analysisReportService.checkRequiredReport(phaseEnum, applicationNo);

		errorMessage = composeMissingReportsErrorMessage(missedJobMappings);
		if (CollectionUtils.isNotEmpty(errorMessage)) {
			errorMessage.add(0, "requiredReportNotExist");

			return errorMessage;
		}

		ApprovalRecord approvalRecord = approvalRecordViewConverter.toDto(approvalView);
		DeployApplication deployApplication = deployApplicationService.getByApplicationNo(applicationNo);
		deployApplicationService.fowardNextStep(deployApplication, approvalRecord);
		
		// 呼叫mergeRecordService.closeDeployForm(requirementNo, requirementSequence)改為
		// 在待入庫頁面，點擊"入庫"按鈕時動作
		
		return errorMessage;
	}

	// 提供同一changeItem內的任務List 作為前置任務的DropDownList
	@ResponseBody
	@RequestMapping(value = "dependReport/{changeNo}", method = RequestMethod.POST)
	public List<String> dependReportDropDownList(@PathVariable Long changeNo) {

		List<AnalysisReport> analysisReports = analysisReportService.findByChangeNo(changeNo);
		List<String> jobNames = Lists.newArrayList();

		for (AnalysisReport analysisReport : analysisReports) {
			String jobName = analysisReport.getJobName();
			if (StringUtils.isNotEmpty(jobName)) {
				jobNames.add(jobName);
			}
		}

		return jobNames;
	}

	// 送出進行前置任務的產生
	@ResponseBody
	@RequestMapping(value = "sendDepentReport", method = RequestMethod.POST)
	public String reReportsendDepent(@RequestParam("changeNo") Long changeNo, @RequestParam("jobName") String jobName,
			@RequestParam("reportNo") Long reportNo, @RequestParam("associationStatus") String associationStatus) {

		AnalysisReport sourceReport = analysisReportService.findByReportNo(reportNo);

		// 新增新的前置任務
		List<AnalysisReport> analysisReports = analysisReportService.findByChangeNo(changeNo);
		for (AnalysisReport analysisReport : analysisReports) {
			if (StringUtils.equals(analysisReport.getJobName(), jobName)) {
				Long targetReportNo = analysisReport.getReportNo();
				if (StringUtils.equals(reportNo.toString(), targetReportNo.toString())) {
					return "fail";
				} else {
					// 若有舊的前置任務需要先刪除
					List<JobAssociation> jobAssociations = jobAssociationService.findBySourceAnalysisReportAndType(sourceReport, "DEPEND");
					if (CollectionUtils.isNotEmpty(jobAssociations)) {
						for (JobAssociation jobAssociation : jobAssociations) {
							jobAssociationService.deleteJobAssociation(jobAssociation);
						}
					}
					jobAssociationService.createJobAssociation("DEPEND", sourceReport, analysisReport, associationStatus);
				}
			}
		}

		return "success";
	}

	// 編輯AnalysisReport
	@ResponseBody
	@RequestMapping(value = "editAnalysisReport", method = RequestMethod.POST)
	public String edReportEdit(@RequestParam("reportNo") Long reportNo, @RequestParam("jobName") String jobName) {

		AnalysisReport analysisReport = analysisReportService.findByReportNo(reportNo);
		analysisReport.setJobName(jobName);
		analysisReportService.saveAnalysisReport(analysisReport);

		return "succes";
	}

	// 取消前置任務
	@ResponseBody
	@RequestMapping(value = "deleteDependReport", method = RequestMethod.POST)
	public String edReportDependdelete(@RequestParam("reportNo") Long reportNo) {

		AnalysisReport analysisReport = analysisReportService.findByReportNo(reportNo);
		List<JobAssociation> jobAssociations = jobAssociationService.findBySourceAnalysisReportAndType(analysisReport, "DEPEND");
		if (CollectionUtils.isNotEmpty(jobAssociations)) {
			for (JobAssociation jobAssociation : jobAssociations) {
				jobAssociationService.deleteJobAssociation(jobAssociation);
			}
		}

		return "success";
	}

	private List<String> composeMissingReportsErrorMessage(Map<ChangeItem, List<JobMapping>> missedJobMappings) {

		List<String> missingReportWarning = Lists.newArrayList();

		for (Map.Entry<ChangeItem, List<JobMapping>> missedJobMapping : missedJobMappings.entrySet()) {

			ChangeItem changeItem = missedJobMapping.getKey();

			List<JobMapping> jobMappings = missedJobMapping.getValue();
			if (!CollectionUtils.sizeIsEmpty(jobMappings)) {

				List<String> missingJobs = Lists.newArrayList();
				for (JobMapping jobMapping : jobMappings) {
					missingJobs.add(String.format("「%s」", jobMapping.getReportType()));
				}

				String errorMessage = String.format("異動項目「%s」- 尚缺 %s報告<br/>", changeItem.getProjectCode(), Joiner.on("、").join(missingJobs));

				missingReportWarning.add(errorMessage);
			}
		}

		return missingReportWarning;

	}

	private List<String> composeChangeItemVersionBlankErrorMessage(List<ChangeItem> changeItems) {

		List<String> errorMessage = Lists.newArrayList();

		for (ChangeItem changeItem : changeItems) {

			if (StringUtils.isBlank(changeItem.getReleaseVersion())) {
				errorMessage.add(String.format("異動項目「%s」<br/>", changeItem.getProjectCode()));
			}
		}

		return errorMessage;
	}

	private Users getUserById(String username, List<Users> users) {
		for (Users user : users) {
			if (user.getUsername().equals(username)) {
				return user;
			}
		}

		log.warn("Can't find a user with username: " + username);

		return null;
	}

}
