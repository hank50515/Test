package com.gss.adm.scd.model.sparsematrix.dto;

import java.io.Serializable;

import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@ToString
@EqualsAndHashCode(of = { "projectId", "id" })
public class SparseMatrixNode implements Serializable {

	private static final long serialVersionUID = 1L;

	private Long projectId;

	private String id;

	private String name;
	
	private String code;

	private Integer entityId;

	private String entityName;
	
	private String identifier;

	private String entityCode;

	private String entityDefinitionName;

	private Integer startLineNumber;

	private Integer endLineNumber;

	private boolean haveSource;
}
