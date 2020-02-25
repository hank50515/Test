admApp.factory('projectScheduleService', ['$q', '$http', '$modal', 'projectModelService', function($q, $http, $modal, projectModelService) {
	var scheduleType = {
		buildMatrix: 0,
		executeSchedule: 1
	};

	return {
		buildMatrix: buildMatrix,
		executeSchedule: executeSchedule,
		executeCrossProjectSchedule: executeCrossProjectSchedule,
	};

	function openInfoModel(response, type) {
		response.data.currentType = type;
		$modal.open({
			templateUrl: 'schedule-info-modal.xhtml',
			controller: 'scheduleInfoModalCtrl',
			size: 'sm',
			resolve: {
				scheduleInfo: function () {
					return {
						scheduleType: scheduleType,
						data: response.data
					};
				}
			}
		});
	}

	function buildMatrix(projectId, configurationId) {
		var deferred = $q.defer();

		if (!configurationId) {
			projectModelService.getProjectModel(projectId, projectModelService.PROJECT_MODEL_TYPE.Schedule)
				.then(function(data) {
					if(data.length === 0) {
						MessageBox.warn(SCHEDULE_NO_EXIST_CANT_BUILD_MATRIX);
						return;
					}
					
					buildMatrix(projectId, data[0].projectModelId)
						.then(deferred.resolve, deferred.reject);
				}, function(response, status) {
					deferred.reject(response, status);
				});
		} else {
			$http.post(getBaseUrl() + '/actions/schedule/buildMatrix/' + projectId + "/" + configurationId)
				.then(function(response) {
					openInfoModel(response, scheduleType.buildMatrix);
					deferred.resolve(response.data);
				}, function(response) {
					deferred.reject(response, response.status);
				});
		}
		
		return deferred.promise;
	}

	function executeSchedule(projectId) {
		var deferred = $q.defer();

		$http.get(
			getBaseUrl() + "/actions/schedule/project/" + projectId + "/execute"
		).then(function(response) {
			openInfoModel(response, scheduleType.executeSchedule);
			deferred.resolve(response.data);
		}, function(response) {
			deferred.reject(response, response.status);
		});

		return deferred.promise;
	}
	
	function executeCrossProjectSchedule(projectId) {
		var deferred = $q.defer();

		$http.get(
			getBaseUrl() + "/actions/schedule/project/" + projectId + "/execute/crossProject"
		).then(function(response) {
			openInfoModel(response, scheduleType.executeSchedule);
			deferred.resolve(response.data);
		}, function(response) {
			deferred.reject(response, response.status);
		});

		return deferred.promise;
	}
}]);