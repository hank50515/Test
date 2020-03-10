package com.gss.cia.test.gssqueyecia1277;

import lombok.Getter;

@Getter
public class PropertiesUtils {
	
	public PropertiesUtils(String propFileName) {
		this.propFileName = propFileName;
	}

	private String propFileName;
}