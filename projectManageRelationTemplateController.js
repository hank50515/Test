admApp.controller('projectManageRelationTemplateCtrl', function($scope, $http, $modal, projectService, modalService) {
	AngularIntro.setPageCode();
	
	var closestTr;
	var closestTrData;
	
	//防止拿前面的 $scope.project 拿不到
	projectService.getCurrentProject().then(function(project) {
		
		var grid = $("#projectManageRelationTemplate").kendoGrid({
			dataSource:{
				transport:{
					read:getBaseUrl() + "/actions/manageRelationTemplate/project/" + project.id,
				},
				pageSize:10
			},
			sortable: true,
			pageable:true,
            resizable: true,
            height: '100%',
			columns:[{
					field:"name",
					title: TEMPLATE_NAME,
					template: '<div class="sg-ellipsis" title="#:name#">#:name#<div>',
					width: 120
				},{
					field:"targetDefinition.name",
					title: TARGET_CI,
					template: '<div class="sg-text--center">' +
								'<span class="qu-narrow-legend-span" title="#=targetDefinition.name#" ' +
			        				'style="background-color:#= targetDefinition.icon #;\">#= targetDefinition.narrowName #</span>' +
			        			'</div>',
					width: 100
				},{
					field:"relationTypeName",
					title: RELATION_TYPE,
					template: '<div class="sg-ellipsis" title="#:relationTypeName#">#:relationTypeName#<div>',
					width: 100
				},{
					title:RELATION_TEXT,
					template: function(dataItem) {
					   return getTemplate(dataItem.manageRelationCasesView);
					}
				},{ 
					title: FUNCTION,
					command: [{
						name:"delete",text:DELETE,
						click: function(e){
							closestTr = $(e.target).closest("tr");
							closestTrData = this.dataItem(closestTr);
	
							$("#confirmDelName").text(closestTrData.name);
							$scope.openDeleteModal()
						}
					}],
					width: 100
				}]
		}).data("kendoGrid");
		
		function deleteRelationTemplate(){
			$http({
				url: getBaseUrl() + "/actions/manageRelationTemplate/relationTemplate/" + closestTrData.id,
				method: "delete"
			}).success(function(data){
				grid.removeRow(closestTr);
				MessageBox.success(DELETE_SUCCESS);
			}).error(function(response){
				MessageBox.error(response);
			});
		};
		
		function getTemplate(manageRelationCasesView) {
			var template="";
			for(var i=0 ;i < manageRelationCasesView.length ; i++){
				template += '<div><span style="color:#3c763d;">'+ manageRelationCasesView[i].selectedText
					  +'</span><span class="all-padding">/</span>'
					  +'<span style="color:#2b4fea;">'+manageRelationCasesView[i].targetText +'</span></div>';
			}
			return template;
		}
		
		$scope.openDeleteModal = function(){
			modalService.getDeleteModal().then(function(isDelete) {
	    		if(isDelete){
	    			deleteRelationTemplate();
	    		}
	    	},function(response){
	    		MessageBox.showErrMsg(response);
	    	}).finally(function(){
	    		$.unblockUI();
	    	});
	    };
		
	}, function(data) {
        MessageBox.error(PROJECT_FAIL);
    });
   
});