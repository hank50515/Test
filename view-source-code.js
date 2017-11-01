admApp.controller('sourceCodeControl', 
    function($modal, $scope, $http, $timeout, $anchorScroll, $window, $location, projectService) {
	var NOT_CORRECT_RANGE = "選取範圍不正確，請重新選取。";
	var WAIT_TO_LOAD_RELATIONSHIPS = "撈取程式碼關聯需要一些時間，請稍候。";

	$scope.showInsertButton = false;
    $scope.selectedInfo = {};
    
    $.blockUI();
    projectService.getCurrentProject().then(function(project) {
        if ( ! project ) {
        	$.unblockUI();
            return ;
        }
        
        $scope.project = project;
        
        var entityId = $.url("?nodeId"),
            gotoLineNumber = $.url("?gotoline");
        if ( ! entityId) {
        	$.unblockUI();
            return;
        }
        
        // load source code
        $scope.lineIDPrefix = 'line-';
        $scope.showCodeFlag = false;
        var projectId = project.id;
        var projectCode = project.code;
        
        //initialize the add relatio button
        var $addRelationButton = $('.addRelationButton');
        $addRelationButton.on('click', function(){
        	$addRelationButton.hide();
        	
        	$modal.open({
    			animation : true,
    			backdrop:'static',
    			templateUrl : 'addRelation-template',
    			controller : 'ViewSourceCodeModalCtrl',
    			size : 'lg',
    			resolve: {
    				selectedInfo: function () {
    		          return $scope.selectedInfo;
    		        }
    		    }
    		});
        });
        
        var $sourceCodeMethodTree = $("#sourceCodeMethodTree");
        
        $timeout(function() {
            
            // load method, forward, backward info
            $sourceCodeMethodTree.block();
            $http({
                url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId + "/method/tree",
                method : "GET",
            }).success(function(data, status, headers, config) {
            	$scope.entityName = data[0].name;
            	
                // initial method tree view
                var $sourceCodeMethodTree = $("#sourceCodeMethodTree");
                var $sourceCodeMethodTreeView = $sourceCodeMethodTree.kendoTreeView({
                    template: kendo.template($("#treeview-template").html()),
                    select: function(e) {
                        var selectItem = $sourceCodeMethodTreeView.dataItem(e.node);
                        _gotoSelectedMethod(selectItem);
                    },
                    dataSource: new kendo.data.HierarchicalDataSource({
                        data: data
                    })
                }).data("kendoTreeView");
                
                
            }).error(function(data, status, headers, config){
            	 $sourceCodeMethodTree.unblock();
            	 MessageBox.error(data);
            });
            
            // load source code text
            $(".sourceCodeContentMainContent").block();
            $http({
                url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId + "/all",
                method : "GET"
            }).success(function(data, status, headers, config) {
            	$scope.syntaxHighlightExtensionName = data.syntaxHighlightExtensionName;
            	$scope.sourceCodeContent = data.sourceCode;

            	$timeout(function() {
            		
                    SyntaxHighlighter.defaults['quick-code'] = false;
                    SyntaxHighlighter.defaults['toolbar'] = false;
                    SyntaxHighlighter.highlight();
                    
                    $scope.showCodeFlag = true;
                    _bindIconSpace();
                    
                    MessageBox.info(WAIT_TO_LOAD_RELATIONSHIPS);
                    // load forward, backward line number
                    $(".triangleContent").block();
                    $http({
                        url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId,
                        method : "GET"
                    }).success(function(data, status, headers, config) {
                        _bindIcon(data);
                        
                        if ( gotoLineNumber ) { 
                    		_gotoLineNumber(gotoLineNumber);
                    	}
                    	
                    	var word = $.url("?q");
                		if (word){
                			_gotoHightLightKeyword(word);
                		}
                    }).error(function(data, status, headers, config) {
                        MessageBox.error(FILE_NOT_FOUND);
                        
                    }).finally(function(e) {
                    	$(".triangleContent").unblock();
                    });
                    
                    $(".line").mouseup(function(event){
                    	$('.addRelationButton').hide();
                    	$timeout(function(){
                    		var selection = window.getSelection();
                    		if(selection.type == "Range") {
                    			var y = event.pageY - $("#sourceCodeContent").offset().top;
                    			var x = event.pageX - $("#sourceCodeContent").offset().left;
                    			$scope.selectedInfo = _prepareSelectedInfo();
                    			
                    			if($scope.selectedInfo != undefined) {
                    				$('.addRelationButton').css("left", (x + 10)  + "px").css("top", (y + 10) + "px");
                    				$('.addRelationButton').show();
                    			} else {
                    				MessageBox.error(NOT_CORRECT_RANGE);
                    			}
                    		} else {
                    			$('.addRelationButton').hide();
                    		}
                    	}, 100);
                    });
                }, 100);
            }).error(function(data, status, headers, config) {
                MessageBox.error(FILE_NOT_FOUND);
            }).finally(function(e) {
            	$(".sourceCodeContentMainContent").unblock();            	
            });
        }, 500);
        
        function _getLineNumberFromNode($node){
        	try{
        		var classes = $node.attr("class").split(" ");
        		for(var i = 0; i< classes.length; i++) {
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
        
        function _prepareSelectedInfo() {
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
			selectedInfo.projectId = projectId;
			selectedInfo.sourceEntityId = entityId;
			selectedInfo.selectedText = selectionString;
			selectedInfo.startLine = startLine;
			selectedInfo.startIndex = startIndex;
			selectedInfo.stopLine = stopLine;
			selectedInfo.stopIndex = stopIndex;
			
			return selectedInfo;
		}
        
        function _gotoSelectedMethod(entity) {
            var line = (entity.startLine || 0),
                redirect = entity.redirect;
            
            if ( ! line && ! redirect ) {
                return ;
            } else if ( redirect ) {
                var nId = entity.id;
                $window.open(getBaseUrl() + "/browse/" + projectCode + "/view_source_code.xhtml?nodeId=" + nId, '_blank');
            }
            
            _gotoLineNumber(line);
        };
        
        function _gotoLineNumber(line) {
            var selectLineClassName = "selectLine";
            $("." + selectLineClassName).toggleClass(selectLineClassName);
            $(".number" + line).addClass(selectLineClassName);
            
            $location.hash(_getLineId(line));
            $anchorScroll();
        }
        
        function _bindIconSpace() {
        	var $tr = $("#sourceCodeContent").find("tr"),
            $backwardTd = $("<td class='triangleContent' />"),
            $forwardTd  = $("<td class='triangleContent' />");
        	
        	$tr.prepend($forwardTd);
        	$tr.prepend($backwardTd);
        }
        
        function _bindIcon(data) {
        	$("#sourceCodeContent .triangleContent").remove();
        	
            var $tr = $("#sourceCodeContent").find("tr"),
                $backwardTd = $("<td class='triangleContent' />"),
                $forwardTd  = $("<td class='triangleContent' />"),
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
                var $icon = $("<div class='triangle-left'>◀</div>");
                $backwardTd.append($icon);
                
//                if ( backwardEntityInfo.length == 1 ) {
//                    $icon.click(function() {
//                        PageUtils.openNewPage(_generateRelationEntityUrl(backwardEntityInfo[0]));
//                    });
//                } else {
                    _initialSourceCodeBackwardTooltip($icon, backwardEntityInfo);
//                }
            } else {
                $backwardTd.append("<div class='triangle-empty'>◀</div>");
            }
        }
        
        function _appendForwardIcon($forwardTd, forwardEntityInfo) {
            if ( forwardEntityInfo ) {
                var $icon = $("<div class='triangle-right'>▶</>");
                $forwardTd.append($icon);
                
//                if ( forwardEntityInfo.length == 1 ) {
//                    $icon.click(function() {
//                        PageUtils.openNewPage(_generateRelationEntityUrl(forwardEntityInfo[0]));
//                    });
//                } else {
                    _initialSourceCodeForwardTooltip($icon, forwardEntityInfo);
//                }
            } else {
                $forwardTd.append("<div class='triangle-empty'>▶</>");
            }
        }
        
        function _generateRelationEntityUrl(entityInfo) {
            var newUrl =
                getBaseUrl() + "/browse/" + projectCode + "/view_source_code.xhtml?nodeId=" + entityInfo.entityId + "&gotoline=" + entityInfo.sourceLineNumber;
            return newUrl;
        }
        
        function _appendHashDiv($line, line) {
            var $hashId = $("<div id='" + _getLineId(line) + "' />");
            $line.before($hashId);
        }
        
        function _initialMehodForwardTooltip($sourceCodeMethodTreeView) {
            $sourceCodeMethodTree.on("click", ".tree-triangle-right", function(e) {
                e.stopPropagation();
                
                var $triangleRightIcon = $(this);
                var loaded = $triangleRightIcon.attr("loaded");
                if ( String(loaded) == "true" ) {
                    return ;
                }
                
                var dataItem = $sourceCodeMethodTreeView.dataItem($triangleRightIcon.closest(".k-item"));
                var forwardEntityId = dataItem.id;
                
                $sourceCodeMethodTree.block();
                $http({
                    url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId + "/forward/" + forwardEntityId + "/realtionDetail/",
                    method : "GET"
                }).success(function(data, status, headers, config) {
                    
                    if ( typeof data[0] === 'undefined' ) {
                        // empty data
                        $triangleRightIcon.remove();
                        MessageBox.warn(NO_DATA);
                        return ;
                    }
                    
                    var template = kendo.template($("#methodForwardLink-template").html());
                    var $tooltip = $triangleRightIcon.kendoTooltip({
                        autoHide: false,
                        animation: false,
                        position: "right",
                        show: function(e) {
                        	var $popup = $(this.popup.element);
                        	_bindGoToLineEvents($popup);
                        	
                        	$popup.addClass("tooltipShift");
                        },
                        content: template({
                        	projectCode: projectCode,
                            entities: data,
                            baseUrl: getBaseUrl()
                        })
                    }).data("kendoTooltip");
                    $tooltip.show();
                    
                    $(document).on("mouseover", "a.forward", function(e) {
                    	var lineNum = $(this).attr("line");
                    	_gotoLineNumber(lineNum);
                    });
                    
                    $triangleRightIcon.attr("loaded", true);
                    
                }).error(function(data, status, headers, config){
                	MessageBox.error(data);
                }).finally(function(e) {
                    $sourceCodeMethodTree.unblock();
                });
            });
        }
        
        function _initialMethodBackwardTooltip($sourceCodeMethodTreeView) {
            $sourceCodeMethodTree.on("click", ".tree-triangle-left", function(e) {
                e.stopPropagation();
                
                var $triangleLeftIcon = $(this);
                var loaded = $triangleLeftIcon.attr("loaded");
                if ( String(loaded) == "true" ) {
                    return ;
                }
                
                var dataItem = $sourceCodeMethodTreeView.dataItem($triangleLeftIcon.closest(".k-item"));
                var backwardEntityId = dataItem.id;
                
                $sourceCodeMethodTree.block();
                $http({
                    url : getBaseUrl() + "/actions/sourceCode/project/" + projectId + "/entity/" + entityId + "/backward/" + backwardEntityId + "/realtionDetail/",
                    method : "GET"
                }).success(function(data, status, headers, config) {
                    
                    if ( typeof data[0] === 'undefined' ) {
                        // empty data
                        $triangleLeftIcon.remove();
                        MessageBox.warn(NO_DATA);
                        return ;
                    }
                    
                    var template = kendo.template($("#methodBackwardLink-template").html());
                    var $tooltip = $triangleLeftIcon.kendoTooltip({
                        autoHide: false,
                        animation: false,
                        position: "right",
                        show: function(e) {
                        	var $popup = $(this.popup.element);
                        	_bindGoToLineEvents($popup);
                        	
                        	$popup.addClass("tooltipShift");
                        },
                        content: template({
                        	projectCode: projectCode,
                            entities: data,
                            baseUrl: getBaseUrl()
                        })
                    }).data("kendoTooltip");
                    $tooltip.show();
                    
                    $(document).on("mouseover", "a.backward", function(e) {
                    	var lineNum = $(this).attr("line");
                    	_gotoLineNumber(lineNum);
                    });
                    
                    $triangleLeftIcon.attr("loaded", true);
                    
                }).error(function(data, status, headers, config){
                	MessageBox.error(data);
                }).finally(function(e) {
                    $sourceCodeMethodTree.unblock();
                });
            });
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

        	var entityId = parseInt($.url("?nodeId"));
        	$icon.kendoTooltip({
    			autoHide: false,
    			animation: false,
    			callout: true,
    			showOn: "click",
    			position: "bottom",
    			show: function(e) {
    				var $popup = $(this.popup.element);
    				_bindGoToLineEvents($popup);
    			},
    			content: template({
    				projectCode: projectCode,
    				entities: entities,
    				baseUrl: getBaseUrl()
    			})
    		});
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
            
            $popup.find('a[entityId=' + entityId + ']').mouseover(function() {
            	var lineNum = $(this).attr("line");
            	_gotoLineNumber(lineNum);
            });

            $popup.addClass("tooltipBackgroundDetail");
        }
        
        function _gotoHightLightKeyword(word){
    		//decoding
    		word = decodeURIComponent(word);
    		//replace + to space
    		word = word.replace(/\+/g, " ");
    			
    		//set multi highlight
    		var splitWords = [];
    		splitWords = word.split(/\s+/);
    		$(".highlightData").highlight(splitWords);
            var hightLineData = $(".highlight").first().closest(".line");
            if (hightLineData){
            	var className = hightLineData.attr('class');
            	var splitClassName = [];
            	var splitClassName = className.split(/\s+/);
            	if (splitClassName[0] == "line"){
            		var lineNumber = splitClassName[1].replace("number", "");
            		_gotoLineNumber(lineNumber);
            	}
            }
        }
    }, function(data) {
		$.unblockUI();
    	MessageBox.error(PROJECT_FAIL);
    });
    
    $scope.addIntoCart = function() {
    	var entityId = $.url("?nodeId");
    	if(!entityId) {
    		return;
    	}
    	
    	entityId = Number(entityId);
    	$(document).trigger("updateEntity", {id: entityId, name: $scope.entityName});
    }
    
});