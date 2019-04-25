admApp.controller('projectMemberSettingCtrl', function($http, $scope, $timeout, userService, modalService) {
    $scope.removeUser = function(dataItem) {
    	modalService.getDeleteModal().then(function(isDelete) {
    		if(isDelete){
    			$.blockUI();
    			$http({
    				method : "DELETE",
    				url : 'SSO/Login',
    				headers: {
    					'Content-Type': 'application/json'
    				},
    				data : dataItem
    			}).success(function(data, status, headers, config) {
    				$scope.projectMembers.dataSource.read();
    				MessageBox.success(DELETE_SUCCESS);
    			}).error(function(data, status, headers, config) {
    				MessageBox.showErrMsg(data, status);
    			}).finally(function() {			
    				$.unblockUI();
    			});
    		}
    	},function(response){
    		MessageBox.showErrMsg(response);
    	}).finally(function(){
    		$.unblockUI();
    	});
    }
});