package com.gss.adm.web.view;

import java.io.Serializable;

import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@ToString
@EqualsAndHashCode(of = { "definitionId" })
public class EntityDefinitionSimpleView implements Serializable {

	private static final long serialVersionUID = 1L;

	private Integer definitionId;
	
	private String name;

	private String icon;

	private boolean haveSource;
}
