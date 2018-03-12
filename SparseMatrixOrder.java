package com.gss.adm.scd.model.sparsematrix.dto;

import java.io.Serializable;
import java.util.Collection;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang.StringUtils;

import com.google.common.collect.HashBiMap;
import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.google.common.collect.Sets;
import com.gss.adm.api.utils.CollectionUtils2;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class SparseMatrixOrder implements Serializable {
	
		private Long projectId;
	
	private String code;

	private static final long serialVersionUID = 1L;
	
	private static final int MAP_SIZE = 300000;

	public Integer getIndexById(String id) {
		if (StringUtils.isBlank(id)) {
			throw new IllegalArgumentException("Id is null.");
		}

		return idIndexMap.get(id);
	}

	public String getIdByIndex(Integer index) {
		if (index == null) {
			throw new IllegalArgumentException("Index is null.");
		}

		return idIndexMap.inverse().get(index);
	}

	public SparseMatrixNode getNodeById(String id) {
		if (StringUtils.isBlank(id)) {
			throw new IllegalArgumentException("Id is null.");
		}

		return nodeMap.get(id);
	}

	public SparseMatrixNode getNodeByIndex(Integer index) {
		if (index == null) {
			throw new IllegalArgumentException("Index is null.");
		}

		String id = getIdByIndex(index);

		return getNodeById(id);
	}

	public List<SparseMatrixNode> findNodesByEntityId(Integer entityId) {
		if (entityId == null) {
			throw new IllegalArgumentException("Entity id is null.");
		}

		List<SparseMatrixNode> nodes = Lists.newArrayList();
		Collection<String> nodeIds = getEntityIdMap().get(entityId);
		for (String nodeId : nodeIds) {
			SparseMatrixNode node = getNodeById(nodeId);
			nodes.add(node);
		}

		return nodes;
	}

	public void putIntoMaps(SparseMatrixNode node) {
		String id = node.getId();
		Integer entityId = node.getEntityId();
		Integer index = getIndexById(id);
		SparseMatrixNode nodeFromMap = getNodeById(id);

		if (index == null && nodeFromMap == null) {
			idIndexMap.put(id, idIndexMap.size());
			nodeMap.put(id, node);
			
			if(entityIdMap.get(entityId) == null) {
				entityIdMap.put(entityId, Sets.newHashSet(id));
			} else {
				entityIdMap.get(entityId).add(id);
			}
		}
	}
	
	
	public void removeByEntityId(Integer entityId) {
		Set<String> nodeIds = entityIdMap.get(entityId);
		
		if(CollectionUtils.isEmpty(nodeIds)) {
			return;
		}

		for (String nodeId : nodeIds) {
			idIndexMap.remove(nodeId);
			nodeMap.remove(nodeId);
		}

		entityIdMap.remove(entityId);
	}
	
	public void removeByEntityIds(Collection<Integer> entityIds) {
		for (Integer entityId : entityIds) {
			removeByEntityId(entityId);
		}
	}
	
	public Set<Integer> findIndexesByEntityId(Integer entityId) {
		Set<Integer> indexes = Sets.newHashSet();
		Set<String> nodeIds = entityIdMap.get(entityId);

		if (CollectionUtils.isEmpty(nodeIds)) {
			return indexes;
		}

		for (String nodeId : nodeIds) {
			indexes.add(idIndexMap.get(nodeId));
		}

		return indexes;
	}
	
	
	private Integer findCurrentMaxIndex() {
		Set<Integer> unsortedIndexes = idIndexMap.values();

		if (CollectionUtils.isEmpty(unsortedIndexes)) {
			return 0;
		}

		List<Integer> indexes = Lists.newArrayList(unsortedIndexes);
		Collections.sort(indexes);

		return indexes.get(indexes.size() - 1);
	}
}
