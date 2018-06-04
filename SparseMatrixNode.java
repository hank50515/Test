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
	
	private String code;

	
	private String code;
	
<<<<<<< HEAD
	private String name;
	
	private String fileName;
=======
	private Long projectId;
	
	private String code;
>>>>>>> origin/master

}
