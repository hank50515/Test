package com.gss.cia.test.gssqueyecia1277;

import lombok.Getter;

public enum SDSModelResource {
	SDS_MODEL_EWEB_URL("sdsModel.eweb.url"), 
	SDS_MODEL_EPAY_URL("sdsModel.epay.url"), 
	SDS_MODEL_EWEB_EXPORT_URL("sdsModel.eweb.export.url"), 
	SDS_MODEL_GROUP_URL("sdsModel.group.url");

	@Getter
	private String value;

	private final String SystemEnvironment = SystemEnvironmentResource.SYSTEM_ENVIRONMENT.toString();
	private final String PROP_FILE = "SDSModel_" + SystemEnvironment + ".properties";

	@Getter
	private PropertiesUtils props;

	private SDSModelResource(String value) {
		this.props = new PropertiesUtils(PROP_FILE);
		this.value = value;
	}
}