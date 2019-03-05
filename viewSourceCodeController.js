admApp.controller('sourceCodeControl', function($q, $modal, $scope, $http, $timeout, $interval, $anchorScroll, $window, $location, projectService, entityService) {
	AngularIntro.setPageCode();
	
	$scope.buildRelationship = function() {
		$modal.open({
			animation : true,
			backdrop:'static',
			templateUrl : "view_source_code_modal.xhtml",
			controller : 'ViewSourceCodeModalCtrl',
			size : 'lg',
			resolve: {
				selectedInfo: function () {
		          return $scope.selectedInfo;
		        }
		    }
		});
	}
	
	$scope.prepareSelectedInfo = function() {		
		var selection = window.getSelection();
		
		var $startNode = $(selection.anchorNode.parentElement).closest('.line');
		var $stopNode = $(selection.extentNode.parentElement).closest('.line');
		
		var startLine = _getLineNumberFromNode($startNode);
		var stopLine = _getLineNumberFromNode($stopNode);
		
		if(startLine < 0 || stopLine < 0) {
			return;
		}
		
		if(stopLine < startLine) {
			var temp = $startNode;
			$startNode = $stopNode;
			$stopNode = temp;
			
			temp = startLine;
			startLine = stopLine;
			stopLine = temp;
		}
		
		var startFullText = $startNode.text();
		var stopFullText = $stopNode.text();
		
		var selectionString = selection.toString();
		var selectionArr = selectionString.split("\n");
		
		var selectedStartText = selectionArr[0];
		var selectedStopText = selectionArr[selectionArr.length - 1];
		
		var startIndex = startFullText.indexOf(selectedStartText);
		var stopIndex = stopFullText.indexOf(selectedStopText) + selectedStopText.length;
		
		var selectedInfo = {};
		selectedInfo.projectId = $scope.project.id;
		selectedInfo.sourceEntityId = $scope.entity.id;
		selectedInfo.selectedText = selectionString;
		selectedInfo.startLine = startLine;
		selectedInfo.startIndex = startIndex;
		selectedInfo.stopLine = stopLine;
		selectedInfo.stopIndex = stopIndex;
		
		return selectedInfo;
	}
	
	$scope.addIntoCart = function() {
    	var entityId = $.url("?nodeId");
    	if(!entityId) {
    		return;
    	}
    	
    	entityId = Number(entityId);
    	$(document).trigger("updateEntity", {id: entityId, name: $scope.entity.name});
    }
	
	$scope.gotoSelectedMethod = function(entity) {
		var line = (entity.startLine || 0),
		redirect = entity.redirect;
		if ( ! line && ! redirect ) {
			return ;
		} else if ( redirect ) {
			var projectCode = $scope.project.code;
			var nId = entity.id;
			$window.open(getBaseUrl() + "/browse/" + projectCode + "/view_source_code.xhtml?nodeId=" + nId, '_blank');
		}
		
		_gotoLineNumber(line);
	}
	
	var selectLineClassName = "qu-line-select";
	
	$scope.lineIDPrefix = 'line-';
	$scope.enableCreateRelationship = false;
	
	var entityId = $.url("?nodeId");
	if(!entityId) {
		MessageBox.error("CI ID 為空!");
		return;
	}
	
	var gotoLineNumber = $.url("?gotoline");
	
	$scope.showTooltip = function(item, $event) {
		if(item.loaded) {
			return;
		}
		
		var projectCode = $scope.project.code;
		var projectId = $scope.project.id;
		var entityId = $scope.entity.id;
		var direction = item.forward? "forward" : "backward";
		var targetEntityId = item.id;
		var isExternal = item.isExternal;
		
		$(".main-splitter-sidebar").block();
		$http({
			url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId + "/" + direction + "/" + targetEntityId + "/realtionDetail/",
            method : "GET",
        }).success(function(data, status, headers, config) {
        	if ( typeof data[0] === 'undefined' ) {
        		// empty data
        		item.notHasEntities = true;
        		MessageBox.warn(NO_DATA);
        		return ;
        	}
        	
        	item.notHasEntities = false;
        	
        	var templateId = item.forward? "methodForwardLink-template" : "methodBackwardLink-template";
			
			var template = kendo.template($("#" + templateId).html());		
			
			var $tooltip = $($event.target).closest("button").kendoTooltip({
                autoHide: false,
                animation: false,
                position: "right",
                showOn: "click",
                show: function(e) {
            		var $popup = $(this.popup.element);
            		_bindGoToLineEvents($popup);
            		
            		$popup.addClass("qu-tooltip-shift");
            		if(isExternal){
            			$popup.addClass("qu-external-background-color");
            		}
                },
                content: template({
                	projectCode: projectCode,
                    entities: data,
                    baseUrl: getBaseUrl()
                })
            }).data("kendoTooltip");
            $tooltip.show();
        }).error(function(data, status, headers, config){
        	MessageBox.error(data);
        }).finally(function(e) {
        	item.loaded = true;
        	$(".main-splitter-sidebar").unblock();
        });
	}
	
	$.blockUI();
	projectService.getCurrentProject().then(function(project) {
		$scope.project = project;
		
		var projectId = $scope.project.id;
		var projectCode = $scope.project.code;
		
		var getEntityById = entityService.getById(projectId, entityId)
		
		var getMethodTree = $http({
	        url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId + "/method/tree",
	        method : "GET"
	    });
		
		var getSourceCode = $http({
            url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId + "/all",
            method : "GET"
        });
		
		var getDirection = $http({
            url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId,
            method : "GET"
        });
		
		$q.all([getEntityById, getMethodTree, getSourceCode, getDirection]).then(function(datas) {
			$scope.entity = datas[0];
			$scope.relationships = datas[1].data;
			$scope.syntaxHighlightExtensionName = datas[2].data.syntaxHighlightExtensionName;
			$scope.sourceCodeContent = datas[2].data.sourceCode;
			$scope.direction = datas[3].data;
			$timeout(function() {        		
				if($scope.entity.blacklist){
					$("#blacklistIconWarning").kendoTooltip({
				        content: BLACKLIST_ERROR_MESSAGE + $scope.entity.blacklist.cause
				    });
				}
				
                SyntaxHighlighter.defaults['quick-code'] = false;
                SyntaxHighlighter.defaults['toolbar'] = false;
                SyntaxHighlighter.highlight();
                
                $scope.showCodeFlag = true;
                _bindIconSpace();
                
                MessageBox.info(WAIT_TO_LOAD_RELATIONSHIPS);
                // load forward, backward line number
                _bindIcon($scope.direction);
                
                if ( gotoLineNumber ) { 
            		_gotoLineNumber(gotoLineNumber);
            	}
            	
            	var word = $.url("?q");
        		if (word){
        			_gotoHightLightKeyword(word);
        		}
                
                $(".line").mouseup(function(event){
                	$timeout(function(){
                		var selection = window.getSelection();
                		if(selection.type == "Range") {
                			var y = event.pageY - $("#sourceCodeContainer").offset().top;
                			var x = event.pageX - $("#sourceCodeContainer").offset().left;
                			$scope.selectedInfo = $scope.prepareSelectedInfo();
                			
                			if($scope.selectedInfo != undefined) {
                				$scope.enableCreateRelationship = true;
                				
                				$('.qu-source-code-add-relation-container').css("left", (x + 10)  + "px").css("top", (y + 10) + "px");
                			} else {
                				$scope.enableCreateRelationship = false;
                				MessageBox.error(NOT_CORRECT_RANGE);
                			}
                		} else {
                			$scope.enableCreateRelationship = false;
                		}
                	}, 100);
                });
                
            	function _bindIconSpace() {
                	var $tr = $("#sourceCodeContent").find("tr"),
                    $backwardTd = $("<td class='qu-triangle-content' />"),
                    $forwardTd  = $("<td class='qu-triangle-content' />");
                	
                	$tr.prepend($forwardTd);
                	$tr.prepend($backwardTd);
                }
            	
                function _bindIcon(data) {
                	$("#sourceCodeContent .qu-triangle-content").remove();
                	
                    var $tr = $("#sourceCodeContent").find("tr"),
                        $backwardTd = $("<td class='qu-triangle-content' />"),
                        $forwardTd  = $("<td class='qu-triangle-content' />"),
                        backwardMap = data.backwardEntityMap || {},
                        forwardMap  = data.forwardEntityMap || {};

                    angular.forEach($(".gutter > div"), function(value, key) {
                        var line = key + 1;
                        
                        _appendBackwardIcon($backwardTd, backwardMap[line]);
                        _appendForwardIcon ($forwardTd,  forwardMap[line]);
                        _appendHashDiv($(value), line);
                    });
                    
                    $tr.prepend($forwardTd);
                	$tr.prepend($backwardTd);
                }
                
                function _appendBackwardIcon($backwardTd, backwardEntityInfo) {
                    if ( backwardEntityInfo ) {
                        var $icon = $("<div class='qu-triangle-left'>◀</div>");
                        $backwardTd.append($icon);
                        
                        _initialSourceCodeBackwardTooltip($icon, backwardEntityInfo);
                    } else {
                        $backwardTd.append("<div class='qu-triangle-empty'>◀</div>");
                    }
                }
                
                function _appendForwardIcon($forwardTd, forwardEntityInfo) {
                    if ( forwardEntityInfo ) {
                        var $icon = $("<div class='qu-triangle-right'>▶</>");
                        $forwardTd.append($icon);
                        
                        _initialSourceCodeForwardTooltip($icon, forwardEntityInfo);
                    } else {
                        $forwardTd.append("<div class='qu-triangle-empty'>▶</>");
                    }
                }
                
                function _appendHashDiv($line, line) {
                    var $hashId = $("<div id='" + _getLineId(line) + "' />");
                    $line.before($hashId);
                }
                
                function _initialSourceCodeForwardTooltip($icon, entities) {
                    var template = kendo.template($("#sourceCodeForwardLink-template").html());
                    $icon.kendoTooltip({
                        autoHide: false,
                        animation: false,
                        callout: true,
                        showOn: "click",
                        position: "bottom",
                        show: function(e) {
                        	var $popup = $(this.popup.element);
                        	_bindGoToLineEvents($popup);
                        	
                        	$.each(entities, function (index, entity) {
          					  if(entity.isExternal){
                        			$popup.addClass("qu-external-background-color");
                        		}
                        	});
                        },
                        content: template({
                        	projectCode: projectCode,
                            entities: entities,
                            baseUrl: getBaseUrl()
                        })
                    });
                }
                
                function _initialSourceCodeBackwardTooltip($icon, entities) {
                	var template = kendo.template($("#sourceCodeBackwardLink-template").html());
                	$icon.kendoTooltip({
            			autoHide: false,
            			animation: false,
            			callout: true,
            			showOn: "click",
            			position: "bottom",
            			show: function(e) {
            				var $popup = $(this.popup.element);
            				_bindGoToLineEvents($popup);
            				
            				$.each(entities, function (index, entity) {
            					  if(entity.isExternal){
                          			$popup.addClass("qu-external-background-color");
                          		}
        					});
            			},
            			content: template({
            				projectCode: projectCode,
            				entities: entities,
            				baseUrl: getBaseUrl()
            			})
            		});
                }
            }, 100);
			
			$.unblockUI();
		}, function(error) {
			MessageBox.showErrMsg(error);
			$.unblockUI();
		});
	}, function(error) {
    	MessageBox.error(PROJECT_FAIL);
    	$.unblockUI();
    });
	
    function _gotoLineNumber(line) {
        $("." + selectLineClassName).toggleClass(selectLineClassName);
        $(".number" + line).addClass(selectLineClassName);
        
        $location.hash(_getLineId(line));
        $anchorScroll();
    }
    
    function _getLineId(index) {
        return ($scope.lineIDPrefix + index);
    }
    
    function _bindGoToLineEvents($popup) {
    	var entityId = parseInt($.url("?nodeId"));
    	$popup.find('a').off("click");
		$popup.find('a').click(function() {
			var tooltipEntityId = parseInt($(this).attr('entityId'));
			var lineNumber = parseInt($(this).attr('targetline'));
			if (! tooltipEntityId){
				return;
			}
			
			if(entityId == tooltipEntityId) {
				_gotoLineNumber(lineNumber);
				$popup.hide();
			} else {
				var projectCode = $scope.project.code;
				
				var sourceCodeUrl = getBaseUrl() + "/browse/" + projectCode
				+ "/view_source_code.xhtml?nodeId=" + tooltipEntityId
				+ "&gotoline=" + lineNumber;
				
				window.open(sourceCodeUrl, '_blank');
			}
		});
    	
        $popup.find('a.qu-entity-link').mouseover(function() {
        	var lineNum = $(this).attr("line");
        	_gotoLineNumber(lineNum);
        });
        
        $popup.find('div.qu-external-entity-link').mouseover(function() {
        	var lineNum = $(this).attr("line");
        	_gotoLineNumber(lineNum);
        });

        $popup.addClass("qu-source-code-tooltip-background");
    }
    
    function _gotoHightLightKeyword(word){
		// decoding
		word = decodeURIComponent(word);
		// replace + to space
		word = word.replace(/\+/g, " ");
			
		// set multi highlight
		var splitWords = [];
		splitWords = word.split(/\s+/);
		$(".highlightData").highlight(splitWords);
        var hightLineData = $(".highlight").first().closest(".line");
        if (hightLineData){
        	var className = hightLineData.attr('class');
        	var splitClassName = className.split(/\s+/);
        	if (splitClassName[0] == "line"){
        		var lineNumber = splitClassName[1].replace("number", "");
        		_gotoLineNumber(lineNumber);
        	}
        }
    }
    
    function _getLineNumberFromNode($node){
    	try{
    		var classes = $node.attr("class").split(" ");
    		for(var i = 0; i < classes.length; i++) {
    			var classValue = classes[i];
    			if(classValue.indexOf("index") >= 0) {
    				return Number(classValue.replace("index", ""));
    			}
    		}

    		return -1
    	} catch (err) {
    		return -1;
    	}
    }
});