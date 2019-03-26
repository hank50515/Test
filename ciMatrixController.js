admApp.run(function($rootScope) {
	$rootScope.$on('triggerSetEntities', function(event, type) {
		$rootScope.$broadcast('setEntities', type);
	});
});

admApp.controller('ciMatrixCtrl', function($q, $scope, $modal, $interval, $http, matrixEntityService, projectService) {
    AngularIntro.setPageCode();

    $scope.isSideWrapOpen = true;
    
    $scope.isMatrixShown = false;
    $scope.isShowEditButton = false;
    $scope.isSmallMatrixShown = false;
    
    $scope.isDirty = false;
    $scope.currentQueryCondition = "Search";
    
	var MAX_NUM = 1000;
	var NUMBER_EXCEEDED = CI_QUERY_CONDITION_COUNT_EXCEED + MAX_NUM;
	
	$scope.SELECT_QUERY_CONDITION = SELECT_QUERY_CONDITION;
	
	$scope.queryConditions = [];
	$scope.queryCondition = {};
	
	$.blockUI();
	projectService.getCurrentProject().then(function(data) {
		$scope.project = data;

		var queryConditions = $http.get(getBaseUrl() + "/actions/matrixQueryCondition/project/" + $scope.project.id);

		$q.all([queryConditions]).then(function(values) {
			$scope.queryConditions = values[0].data;
		}, function error(response) {
			MessageBox.showErrMsg(response);
		}).finally(function(){
			$.unblockUI();
		});
	},function(data) {
		$.unblockUI();
		MessageBox.error(PROJECT_FAIL);
	});
		
	
	$scope.targetType = matrixEntityService.MODAL_TYPE.Target;
	$scope.sourceType = matrixEntityService.MODAL_TYPE.Source;
	
	$scope.disableSearch = true;
	$scope.disableTargetSearch = true;
	$scope.disableSourceSearch = true;
	$scope.isReduced = true;
	$scope.isIndirect = false;
	$scope.targetCiIds = [];
	$scope.sourceCiIds = [];
	$scope.targetCiNames = [];
	$scope.sourceCiNames = [];
	$scope.cellSize = 25;
	
	$scope.selectedDependencyTypes = [];
	$scope.selectedEntities = [];
	$scope.targetEntities = [];
	$scope.sourceEntities = [];
	$scope.targetEntitiesStr = '';
	$scope.sourceEntitiesStr = '';
	
	$scope.definitions = {
			placeholder: CHOOSE_CI_TYPE,
			dataTextField: "name",
			dataValueField: "definitionId",
			itemTemplate: '<span class="qu-narrow-legend-span" style="background-color: #:data.icon#">#:data.narrowName#</span><span class="left-padding">#:data.name#<span>',
			tagTemplate:  '<span class="qu-narrow-legend-span" style="background-color: #:data.icon#">#:data.narrowName#</span>',
			dataSource: {
				type: "jsonp",
				transport: {
					read: {
						url: getBaseUrl() + "/actions/entityDefinition/"
					}
				}
			}
	};
	
	$scope.dependencyTypeOptions = {
			dataTextField: "name",
			dataValueField: "relationTypeID",
			placeholder: SELECT_DEPENDENCY_TYPE,
			dataSource: {
				transport: {
					read: {
						type: "GET",
						url: getBaseUrl() + "/actions/relationType/"
					}
				}
			}
	    }

	$scope.$on('setEntities', function(event, type) {
		if (type == matrixEntityService.MODAL_TYPE.Target) {
			$scope.targetEntities = matrixEntityService.getTargetSelectedEntities();
			
			$scope.targetCiIds = [];
			$scope.targetCiNames = [];
			$scope.targetEntitiesStr = '';

			$($scope.targetEntities).each(function() {
				if ($.inArray(this.id, $scope.targetCiIds) < 0) {
					$scope.targetCiIds.push(this.id);
					$scope.targetCiNames.push(this.name);
				}
			});
			
			$scope.targetEntitiesStr = $scope.targetCiNames.toString();
		}

		if (type == matrixEntityService.MODAL_TYPE.Source) {
			$scope.sourceEntities = matrixEntityService.getSourceSelectedEntities();
			
			$scope.sourceCiIds = [];
			$scope.sourceCiNames = [];
			$scope.sourceEntitiesStr = '';

			$($scope.sourceEntities).each(function() {
				if ($.inArray(this.id, $scope.sourceCiIds) < 0) {
					$scope.sourceCiIds.push(this.id);
					$scope.sourceCiNames.push(this.name);
				}
			});
			
			$scope.sourceEntitiesStr = $scope.sourceCiNames.toString();
		}
	});
	
	$scope.openModal = function(type) {
		var controller = {};
		var definitionIds = [], definitions = [];
		if (type == matrixEntityService.MODAL_TYPE.Target) {
			controller = 'TargetModalInstanceCtrl';
			definitions = $scope.targetDefinitions;
		}

		if (type == matrixEntityService.MODAL_TYPE.Source) {
			controller = 'SourceModalInstanceCtrl';
			definitions = $scope.sourceDefinitions;
		}
		
		$(definitions).each(function() {
			definitionIds.push(this.definitionId);
		});
		
		$modal.open({
			animation : true,
			templateUrl : getBaseUrl() + '/browse/' + $scope.project.code + '/matrixEntitySelectingModalTemplate.xhtml',
			controller : controller,
			size : 'lg',
			resolve: {
				info: function() {
					return {
						'project' : $scope.project,
						'definitionIds' : definitionIds,
						'definitions' : definitions,
						'type' : type
					}
				}
			}
		});
		
		$scope.$emit('triggerSetEntities', type);
	};

	$scope.openSavingQueryConditionModal = function() {
		openQueryConditionModal("save");
	};
	
	$scope.openRenameQueryConditionModal = function() {
		openQueryConditionModal("rename");
	};


	$scope.deleteQueryCondition = function() {
		var conditionId = $scope.queryCondition.id;
		$.blockUI();
		$http({
			method : "DELETE",
			url : getBaseUrl() + "/actions/matrixQueryCondition/project/" + $scope.project.id + "/id/" + conditionId,
		}).then(function successCallback(response) {
			$scope.removeCondition(conditionId);
			$scope.clearConditions();
			
			MessageBox.success(DELETE_SUCCESS);
			$.unblockUI();
		}, function errorCallback(response) {
			MessageBox.showErrMsg(response);
		}).finally(function() {
			$.unblockUI();
	    });
	}
	
	$scope.updateQueryCondition = function() {
		var condition = $scope.queryCondition;
		condition.sourceDefinitions = $scope.sourceDefinitions;
		condition.sourceEntities = $scope.sourceEntities;
		condition.targetDefinitions = $scope.targetDefinitions;
		condition.targetEntities = $scope.targetEntities;
		condition.relationTypes = $scope.selectedDependencyTypes;
		condition.isReduce = $scope.isReduced;
		condition.isIndirect = $scope.isIndirect;
		
		$.blockUI();
		$http({
			method : "POST",
			url : getBaseUrl() + "/actions/matrixQueryCondition/project/" + $scope.project.id,
			data : condition
		}).then(function successCallback(response) {
			MessageBox.success(UPDATE_SUCCESS);
			$.unblockUI();
		}, function errorCallback(response) {
			MessageBox.showErrMsg(response);
			$.unblockUI();
		});
	}

	$scope.generateMatrix = function() {
		var targetLength = $scope.targetCiIds.length;
		var sourceLength = $scope.sourceCiIds.length;
		var totalLength = targetLength + sourceLength;

		if(totalLength > MAX_NUM) {
			MessageBox.error(NUMBER_EXCEEDED);			
			return;			
		}

		var matrixForm = $('#ciIdsForm').serialize();
		var projectId = $scope.project.id;
		
		$scope.showOrHideSmallMatrix(false);
		
		$.blockUI();
		$http({
			method : "POST",
			url : getBaseUrl() + '/actions/matrixRelation/' + projectId + '/relations/',
			headers : {
				'Content-Type' : 'application/x-www-form-urlencoded; charset=utf-8'
			},
			data : matrixForm
		}).success(function(data, status, headers, config) {
			if (data.matrixRelationViews.length == 0) {
				MessageBox.error(COULD_NOT_DISPLAY_MATRIX);
				$.unblockUI();
				return;
			}

			//先關閉檢視縮圖功能！
			$scope.isMatrixShown = false;
			$scope.matrixData = data;

			var containerWidth = $('#matrixLayout').width();
			var containerHeight = $('#matrixLayout').innerHeight();
			var containerId = "matrixContainer";
			
			$scope.ciMatrix = new CiMatrix(containerId, containerWidth, containerHeight, $scope.matrixData);
			$scope.ciMatrix.buildMatrix();
			$scope.ciMatrix.resetSmallSticky();
//			$scope.ciMatrix.showSmallMatrix($scope.matrixData);
//			$scope.showOrHideSmallMatrix(false);
		}).error(function(data, status, headers, config) {
			MessageBox.showErrMsg(data, status);
		}).finally(function(){	
			toggeleFilterDiv();
			$.unblockUI();
		});
	}
	
	$scope.selectSearchCondition = function(event, currentQueryCondition) {
		if(currentQueryCondition.id && currentQueryCondition.id != "") {
			$scope.queryCondition = currentQueryCondition;
			$scope.isShowEditButton = true;
			
			$scope.currentQueryCondition = currentQueryCondition.name;
			
			$scope.sourceDefinitions = currentQueryCondition.sourceDefinitions;
			$scope.sourceEntities = currentQueryCondition.sourceEntities;
			
			$scope.targetDefinitions = currentQueryCondition.targetDefinitions;
			$scope.targetEntities = currentQueryCondition.targetEntities;
			$scope.selectedDependencyTypes = currentQueryCondition.relationTypes;
			
			$scope.isReduced = currentQueryCondition.isReduce;
			$scope.isIndirect = currentQueryCondition.isIndirect;

			matrixEntityService.setSourceSelectedEntities($scope.sourceEntities);
			$scope.$emit('triggerSetEntities', matrixEntityService.MODAL_TYPE.Source);
			
			matrixEntityService.setTargetSelectedEntities($scope.targetEntities);
			$scope.$emit('triggerSetEntities', matrixEntityService.MODAL_TYPE.Target);
		} else {
			$scope.clearConditions();
		}
	}
	
	$scope.searchConditionHoverIn = function(event){
		var iElements = angular.element(event.currentTarget).find("i");
		
		if (iElements.length > 0){
			var iElement = iElements[0];
			iElement.classList.remove("qu-matrix-hidden");
			iElement.classList.toggle("qu-matrix-show");
		}
	};

	$scope.searchConditionHoverOut = function(event){
		var iElements = angular.element(event.currentTarget).find("i");
		
		if (iElements.length > 0){
			var iElement = iElements[0];
			iElement.classList.remove("qu-matrix-show");
			iElement.classList.toggle("qu-matrix-hidden");
		}
	};
	
	$scope.showOrHideSmallMatrix = function(isSmallMatrixShown) {
		if(isSmallMatrixShown !== undefined){
			$scope.isSmallMatrixShown = isSmallMatrixShown;
		} else {
			$scope.isSmallMatrixShown = !$scope.isSmallMatrixShown;
		}
		
		if ($scope.isSmallMatrixShown) {
			$("#smallMatrixSticky").show();
		} else {
			$("#smallMatrixSticky").hide();
		}
	}
	
	$scope.isSearchEnabled = function() {
		$scope.disableSourceSearch = (!$scope.sourceDefinitions || $scope.sourceDefinitions.length == 0);
		$scope.disableTargetSearch = (!$scope.targetDefinitions || $scope.targetDefinitions.length == 0);
		$scope.disableSearch = $scope.disableSourceSearch || $scope.disableTargetSearch;
		
		$scope.isDirty = !$scope.disableSearch;
	}
	
	$scope.clearConditions = function() {
		$scope.sourceDefinitions = [];
		$scope.sourceEntities = [];
		
		$scope.targetDefinitions = [];
		$scope.targetEntities = [];
		$scope.selectedDependencyTypes = [];
		
		$scope.isReduced = true;
		$scope.isIndirect = false;
		$scope.currentQueryCondition = "Search";
		
		matrixEntityService.setSourceSelectedEntities($scope.sourceEntities);
		$scope.$emit('triggerSetEntities', matrixEntityService.MODAL_TYPE.Source);

		matrixEntityService.setTargetSelectedEntities($scope.targetEntities);
		$scope.$emit('triggerSetEntities', matrixEntityService.MODAL_TYPE.Target);
	}

	$scope.$watch('sourceDefinitions', function(newValue, oldValue){
		if(oldValue && (!newValue || newValue.length < oldValue.length)) {
			matrixEntityService.setSourceSelectedEntities([]);
			$scope.sourceEntities = [];
			$scope.sourceCiNames = [];
			$scope.sourceCiIds = [];
			$scope.sourceEntitiesStr = [];
		}

		$scope.isSearchEnabled();
	});

	$scope.$watch('targetDefinitions', function(newValue, oldValue){
		if(oldValue && (!newValue || newValue.length < oldValue.length)) {
			matrixEntityService.setTargetSelectedEntities([]);
			$scope.targetEntities = [];
			$scope.targetCiNames = [];
			$scope.targetCiIds = [];
			$scope.targetEntitiesStr = [];		
		}
		$scope.isSearchEnabled();
	});
	
	$scope.removeCondition = function(conditionId) {
		var index = -1;
		for(var i =0 ; i< $scope.queryConditions.length; i++) {
			if($scope.queryConditions[i].id == conditionId) {
				index = i;
				break;
			}
		}
		
		if(index >= 0) {
			$scope.queryConditions.splice(index, 1);
			$scope.clearConditions();
			$scope.queryCondition = {};
			
			$scope.isShowEditButton = false;
		}
	}
	
	function openQueryConditionModal(type) {
		var newId = null;
		if (type == "rename") {
			newId = $scope.queryCondition.id;
		}
		var queryConditionModal = $modal.open({
			animation : true,
			templateUrl : getBaseUrl() + '/browse/' + $scope.project.code + '/matrixQueryConditionSavingModalTemplate.xhtml',
			controller : 'MatrixQueryConditionSavingModalCtrl',
			resolve: {
				condition: function() {
					return {
						'id'				: newId,
						'titleName'			: type,
						'name'			: $scope.queryCondition.name,
						'sourceDefinitions' : $scope.sourceDefinitions,
						'sourceEntities' : $scope.sourceEntities,
						'targetDefinitions' : $scope.targetDefinitions,
						'targetEntities' : $scope.targetEntities,
						'relationTypes' : $scope.selectedDependencyTypes,
						'isReduce' : $scope.isReduced,
						'isIndirect' : $scope.isIndirect
					}
				},
				project: function() {
					return $scope.project;
				}
			}
		});
		
		queryConditionModal.result.then(function (condition) {
			$scope.queryCondition = condition;
			if (type == "rename") {
				for (var i=0; i< $scope.queryConditions.length; i++){
					if ($scope.queryConditions[i].id == $scope.queryCondition.id){
						$scope.queryConditions[i] = $scope.queryCondition;
						$scope.currentQueryCondition = $scope.queryCondition.name;
					}
				}
			} else {
				$scope.queryConditions.push($scope.queryCondition);
			}
	    });
	}
	
	function toggeleFilterDiv() {
		$("li.head-action-item button.is-chevron-toggle").trigger("click");
	}
});