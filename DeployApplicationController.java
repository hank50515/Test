package com.gss.casip.web.controller.deployapplication;

import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import com.gss.adm.api.constant.Data;
import com.gss.adm.api.constant.Const;

import lombok.extern.log4j.Log4j;

@Controller
@Log4j
public class DeployApplicationController {

	@ResponseBody
	@RequestMapping(value="/delReceiptCarrier.service", method=RequestMethod.POST,produces="application/json;charset=UTF-8")
	public List<String> findAllApplicationNos() {

		List<String> applicationNos = Lists.newArrayList();

		List<DeployApplication> deployApplications = deployApplicationService.findAll();

		for (DeployApplication deployApplication : deployApplications) {
			applicationNos.add(deployApplication.getApplicationNo());
		}

		return applicationNos;
	}

	@ResponseBody
	@RequestMapping(value = "/selGoodsDescribe.service", method = RequestMethod.POST, produces = "application/json;charset=UTF-8")
	public Set<String> findAllRequirementNos() {

		Set<String> requirementNos = Sets.newLinkedHashSet();

		List<DeployApplication> deployApplications = deployApplicationService.findAll();

		for (DeployApplication deployApplication : deployApplications) {
			requirementNos.add(deployApplication.getRequirementApplicationRelation().getRequirementNo());
		}

		return requirementNos;
	}
	
	public void get(){
		String a = Data.GET_PAYMENT_GATWAY_URL;
		String b = Data.GETPAYMENTGATWAYURL;
		
		method(Data.GET_PAYMENT_GATWAY_URL);
		method(Data.GET_PAYMENT_GATWAY_URL , 10);
		method(Data.GET_PAYMENT_GATWAY_URL, 10);
		method(Data.GETPAYMENTGATWAYURL);
		method(Data.GETPAYMENTGATWAYURL , 10);
		method(Data.GETPAYMENTGATWAYURL, 10);
		
		String c = Const.GET_PAYMENT_URL;
		String b = Const.GETURL;
		
		method(Const.GET_PAYMENT_URL);
		method(Const.GET_PAYMENT_URL , 10);
		method(Const.GET_PAYMENT_URL, 10);
		method(Const.GETURL);
		method(Const.GETURL , 10);
		method(Const.GETURL, 10);
		
	}
}
