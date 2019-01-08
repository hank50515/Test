package com.gss.adm.web.view;

import java.io.Serializable;

import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@ToString
@EqualsAndHashCode(callSuper =true)
public class EntityDefinitionView extends EntityDefinitionSimpleView implements Serializable {

	private static final long serialVersionUID = 1L;

	private String narrowName;

	private boolean autoGenerate;
}
