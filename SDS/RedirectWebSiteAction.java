package com.gss.cia.test.gssqueyecia1277;

public class RedirectWebSiteAction {

	protected static final String SDS_MODEL_EWEB_URL = SDSModelResource.SDS_MODEL_EWEB_URL.toString();
	protected static final String SDS_MODEL_EPAY_URL = SDSModelResource.SDS_MODEL_EPAY_URL.toString();
	protected static final String SDS_MODEL_EWEB_EXPORT_URL = SDSModelResource.SDS_MODEL_EWEB_EXPORT_URL.toString();
	protected static final String SDS_MODEL_GROUP_URL = SDSModelResource.SDS_MODEL_GROUP_URL.toString();

	private static final String SUCCESS = "SUCCESS200";

	/**
	 * Mocked method
	 * 
	 * @return
	 * @throws Exception
	 */
	public String redirectToEPay() throws Exception {
		// Some logics
		String outJson = "";
		String inJson = sendJson(outJson, SDS_MODEL_EPAY_URL);

		boolean result = parseJson(inJson, true);
		if (result) {
			return SUCCESS;
		} else {
			return "ERROR401";
		}
	}

	/**
	 * Dummy method
	 * 
	 * @param jsonValue
	 * @param uri
	 * @return
	 */
	private String sendJson(String jsonValue, String uri) {
		return null;
	}

	/**
	 * Dummy method
	 * 
	 * @param jsonValue
	 * @param defaultValue
	 * @return
	 */
	private boolean parseJson(String jsonValue, boolean defaultValue) {
		return defaultValue;
	}

}
