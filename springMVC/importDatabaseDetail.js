$(function() {
	$.blockUI();

	$("#requestDate").kendoDateTimePicker({format : "{0:yyyy-MM-dd HH:mm}"});
	$("#plannedOnBoardDate").kendoDateTimePicker({format : "{0:yyyy-MM-dd}"});
	
	var checkinTimeDisplayName = $("#checkinTimeDisplayName").kendoDateTimePicker({
		format : "{0:yyyy-MM-dd HH:mm}",
		min: $("#requestDate").val(),
		change : function(){
			$('#checkinTime').val(this.value().toJSON());
	}});

	var applicationNo = $.url('?applicationNo');

	if (applicationNo == '') {
		return;
	}

	$("#fowardNext").kendoButton({
		enable : true,
		click : function(e) {

			var submitData = {
				"opinion" : $('#approvalRecordView\\.opinion').val(),
				"applicationNo" : $('#applicationNo').val(),
				"checkinTime" : $('#checkinTime').val()
			};

			$.blockUI();
			$.ajax({
				url : getBaseUrl() + '/deployApplication/repository',
				type : 'POST',
				dataType : 'json',
				contentType : 'application/json',
				data : JSON.stringify(submitData),
				error : function(xhr) {
					bindErrorMessage(xhr)
					MessageBox.error("入庫成功請填寫執行情形");
					$.unblockUI();
				},
				success : function(data) {
					if (data.length > 0) {

						var message;

						if (data[0] == "requiredReportNotExist") {
							message = '缺少必要報告：<br/>';

							for (var i = 1; i < data.length; i++) {
								message += data[i];
							}

						} else {
							message = data[0];
						}

						MessageBox.error(message);
						$.unblockUI();
						return;
					}

					MessageBox.success("入庫成功！");
					setTimeout(function() {
						$.unblockUI();
						location.href = getBaseUrl() + '/importDatabase/importDatabaseViewGrid';
					}, 1500);
				}
			});
		}
	});

	$("#rejectToApplicant").kendoButton({
		click : function(e) {

			var submitData = {
				"opinion" : $('#approvalRecordView\\.opinion').val(),
				"applicationNo" : $('#applicationNo').val()
			};

			$.blockUI();
			$.ajax({
				url : getBaseUrl() + '/deployApplication/rejectToApplicant',
				type : 'POST',
				dataType : 'json',
				contentType : 'application/json',
				data : JSON.stringify(submitData),
				error : function(xhr) {
					bindErrorMessage(xhr);
					MessageBox.error("退回申請人請填寫退回原因");
					$.unblockUI();
				},
				success : function(data) {
					if (data === false) {
						$('span[id^="error_"]').text('');
						$('#error_opinion').text('不可為空字串.');
						MessageBox.error("執行情形或退回原因需填寫");
						$.unblockUI();
					} else {
						MessageBox.success("本單已退回至申請人！");
						setTimeout(function() {
							$.unblockUI();
							location.href = getBaseUrl() + '/importDatabase/importDatabaseViewGrid';
						}, 1500);
					}
				}
			});
		}
	});

	$("#exit").kendoButton({
		click : function(e) {
			window.location.href = getBaseUrl() + '/importDatabase/importDatabaseViewGrid';
		}
	});

	$("#opinion").kendoMaskedTextBox();

	$("#changeItems").kendoGrid({
		dataSource : {
			transport : {
				read : {
					url : getBaseUrl() + '/changeItem/application/' + applicationNo,
					dataType : 'json'
				}
			},
		},
		scrollable : true,
		resizable : true,
		dataBound : function() {
			// 讓底下的報告跟程式能夠自動展開
			this.expandRow(this.tbody.find("tr.k-master-row"));
		},
		columns : [ {
			field : "autoConstructed",
			title : "自動建構",
			width : 80,
			template : "<input type='checkbox' class='checkbox' #if(autoConstructed) {#= checked = \'checked\' #}# disabled />"
		}, {
			field : "projectCode",
			title : "專案代碼"
		}, {
			field : "previousVersion",
			title : "前版次",
			width : 80
		}, {
			field : "developmentVersion",
			title : "入庫版次",
			width : 80
		} ],
		detailTemplate : kendo.template("<div class=\"reports changeNo#=changeNo#\" />"),
		detailInit : initReports
	});

	$("#approvalRecords").kendoGrid({
		dataSource : {
			transport : {
				read : {
					url : getBaseUrl() + '/approvalRecord/application/' + applicationNo,
					dataType : 'json'
				}
			},
			schema : {
				model : {
					fields : {
						approverFullName : {
							type : "string"
						},
						approvalDate : {
							type : "date"
						},
						action : {
							type : "string"
						},
						opinion : {
							type : "string"
						}
					}
				}
			},
		},
		height : 250,
		scrollable : true,
		resizable : true,
		columns : [ {
			field : "approverFullName",
			title : "簽核者",
			width : 200
		}, {
			field : "approvalDate",
			title : "簽核日期時間",
			width : 200,
			format : "{0:yyyy-MM-dd HH:mm}"
		}, {
			field : "action",
			title : "核准/退回",
			width : 100
		}, {
			field : "opinion",
			title : "備註欄"
		} ]
	});

	$("#relatedApplications").kendoGrid({
		dataSource : {
			transport : {
				read : {
					url : getBaseUrl() + '/deployApplication/related/' + applicationNo,
					dataType : 'json'
				}
			},
			schema : {
				model : {
					fields : {
						applicationNo : {
							type : "string"
						},
						status : {
							type : "string"
						},
						applicantFullName : {
							type : "string"
						},
						approval : {
							type : "string"
						}
					}
				}
			},
		},
		scrollable : true,
		columns : [ {
			field : "applicationNo",
			title : "入庫單號",
			width : "130px"
		}, {
			field : "status",
			title : "狀態",
			width : "130px"
		}, {
			field : "applicantFullName",
			title : "申請人",
			width : "130px"
		}, {
			field : "approval",
			title : "待簽核人",
			width : "130px"
		} ]
	});

	$.unblockUI();
});

function initReports(e) {
	var detailRow = e.detailRow;
	var changeNo = e.data.changeNo;

	if ($('#changeNo').length == 0) {
		var changeNoField = document.createElement('input');
		$(changeNoField).val(changeNo).attr('id', 'changeNo').attr('type', 'hidden');
		$('#content').append(changeNoField);
	}

	detailRow.find(".reports").kendoGrid(
			{
				dataSource : {
					transport : {
						read : {
							url : getBaseUrl() + '/analysisReport/changeItem/' + changeNo,
							dataType : 'json'
						}
					},
					schema : {
						model : {
							fields : {
								changeNo : {
									type : "number"
								},
								reportNo : {
									type : "number"
								},
								fileName : {
									type : "string"
								},
								dependReport : {
									type : "string"
								},
								associationStatus : {
									type : "string"
								},
								jobName : {
									type : "string"
								},
								retriggerable : {
									type : "string"
								},
								triggerTime : {
									type : "date"
								},
								status : {
									type : "string"
								},
								reportType : {
									type : "string"
								}
							}
						}
					}
				},
				scrollable : true,
				resizable: true,
				sortable : true,
				pageable : false,
				dataBound : adjustReportInput,
				columns : [ {
					field : "fileName",
					title : "報告名稱",
					template : '<a class="hyperlink" href=\"' + getBaseUrl() + '/analysisReport/download/#=reportNo#\" target="_blank">#=fileName#</a>',
					width : 120
				}, {
					field : "jobName",
					title : "任務名稱",
					width : 150
				}, {
					field : "dependReport",
					title : "前置任務",
					width : 150
				}, {
					field : "associationStatus",
					title : "前置任務狀態",
					width : 120
				}, {
					field : "triggerTime",
					title : "啟動時間",
					template : '<input id="triggerTime#=reportNo#" class="triggerTimePicker" value="#=triggerTime#" />',
					width : 240
				}, {
					field : "status",
					title : "狀態",
					width : 100
				}, {
					field : "reportType",
					title : "報告類型",
					width : 100
				}, {
					width : 260,
					command : [ {
						name : "刪除任務",
						click : function(e) {
							$.blockUI();
							var tr = $(e.target).closest("tr");
							var data = this.dataItem(tr);

							$.ajax({
								method : "POST",
								url : getBaseUrl() + "/analysisReport/remove",
								data : {
									"changeNo" : data.changeNo,
									"reportNo" : data.reportNo
								},
								success : function(result) {
									var childGrid = $(e.target).closest(".k-grid").data("kendoGrid");
									childGrid.dataSource.read();

									$.unblockUI();

									MessageBox.success("任務已刪除");
								},
								error : function(result, status) {

									$.unblockUI();

									MessageBox.showErrMsg(result, status);
									bindErrorMessage(result, status);
								}
							});

						}

					}, {
						// 排入 Queue，等排程執行 trigger Jenkins的啟動按鈕
						name : "啟動",
						click : function(e) {

							$.blockUI();

							var tr = $(e.target).closest("tr");
							var data = this.dataItem(tr);

							$.ajax({
								method : "POST",
								dataType : 'json',
								contentType : 'application/json',
								url : getBaseUrl() + "/analysisReport/build/" + data.reportNo,
								success : function(result) {
									var childGrid = $(e.target).closest(".k-grid").data("kendoGrid");
									childGrid.dataSource.read();

									$.unblockUI();
									MessageBox.success("已排程準備啟動");
								},
								error : function(xhr) {
									$.unblockUI();
									bindErrorMessage(xhr);
								}
							});
						}
					}, {
						name : "前置任務",
						click : function(e) {

							var tr = $(e.target).closest("tr");
							var data = this.dataItem(tr);
							var reportNo = data.reportNo;
							var dependReport = data.dependReport;
							openDependReportWindow(changeNo, reportNo, dependReport);
						}
					}, {
						name : "編輯",
						click : function(e) {

							var tr = $(e.target).closest("tr");
							var data = this.dataItem(tr);
							var reportNo = data.reportNo;
							var jobName = data.jobName;
							openAnalysisReportWindow(reportNo, jobName);
						}
					}, {
						name : "取消前置任務",
						click : function(e) {

							var tr = $(e.target).closest("tr");
							var data = this.dataItem(tr);
							var reportNo = data.reportNo;

							$.blockUI();
							$.ajax({
								method : "POST",
								url : getBaseUrl() + "/deployApplication/deleteDependReport",
								data : {
									"reportNo" : reportNo
								},
								success : function(result) {

									$.unblockUI();
									var childGrid = $(e.target).closest(".k-grid").data("kendoGrid");
									childGrid.dataSource.read();
									MessageBox.success("取消成功");
								},
								error : function(xhr) {
									$.unblockUI();
									MessageBox.error("取消失敗")
								}
							});
						}
					} ]
				} ],
				toolbar : kendo.template("<div class=\"toolbar\">" + "<button class=\"k-button k-i-button\" onclick=\"return openAddReportWindow(" + changeNo
						+ ");\">" + "<span class=\"k-icon k-i-plus\" />" + "<span>新增任務</span>" + "</button>" 
						+ "<button class=\"k-button k-i-button\" onclick=\"return refreshAnalysisReport(" + changeNo + ");\">"
							+ "<span class=\"k-icon k-i-refresh\" />"
							+ "<span>重新整理</span>"
						+ "</button>"
						+ "</div>")
			});

};
function checkReleaseVersionIsBlank() {
	var result = 'false';

	var changeItemGrid = $('#changeItems').data('kendoGrid');
	var changeItemData = changeItemGrid.dataSource.view();

	$.each(changeItemData, function(index, value) {

		var releaseVersion = $('#releaseVersion' + value.changeNo).val();
		if (releaseVersion == '') {

			$('#error_releaseVersion' + value.changeNo).text('不得為空');

			result = 'true';
		}
	});

	return result;
};

function openAddReportWindow(changeNo) {
	var dialog = document.createElement("div");
	var content = kendo.template($("#addReportWindow").html());
	var changeNoField = document.createElement("input");

	$(changeNoField).val(changeNo).attr('id', 'changeNo').attr('type', 'hidden');

	$(dialog).attr('id', 'reportWindow').append(content).append(changeNoField);

	$(dialog).kendoWindow({
		title : "新增任務",
		width : "600px",
		modal : true,
		animation : {
			open : {
				effects : "fade:in"
			},
			close : {
				effects : "fade:out"
			}
		},
		close : function(e) {
			$(dialog).data("kendoWindow").destroy();
		}
	});

	var addReportDialog = $(dialog).data("kendoWindow");

	initReportType();

	initUploadReport();
	$("#reportWindow .k-dropzone .k-button.k-upload-button").after("&nbsp;&nbsp;", $("#autoconstructButton").kendoButton({
		click : function(e) {

			$.blockUI();
			// blockui遮不住 kendowindow
			$('#autoconstructButton').data('kendoButton').enable(false);
			$('#uploadReport').data('kendoUpload').disable();
			$("#reportType").data("kendoDropDownList").enable(false);

			var reportType = $("#reportType").data("kendoDropDownList").value();

			$.ajax({
				method : "POST",
				dataType : 'json',
				contentType : 'application/json',
				url : getBaseUrl() + '/analysisReport/create/UNREPOSITORY/' + reportType + '/' + changeNo,
				success : function(result) {

					$.unblockUI();

					if (result == 'fail') {
						MessageBox.error('此項報告類型無自動建構相關設定');

					} else if (result == 'success') {
						MessageBox.success("任務已新增");
						setTimeout(function() {
							var changeNo = $('#reportWindow #changeNo').val();
							var childGrid = $(".reports.changeNo" + changeNo).data("kendoGrid");
							childGrid.dataSource.read();

							var dialog = $("#reportWindow").data("kendoWindow");
							dialog.destroy();
						}, 1000);

					} else {
						MessageBox.error('controller 返回結果無法分辨');

					}

					$('#uploadReport').data('kendoUpload').enable();
					$("#reportType").data("kendoDropDownList").enable(true);
					$('#autoconstructButton').data('kendoButton').enable(true);

				},
				error : function(xhr) {
					MessageBox.error('發生錯誤，請稍後再試');
				}
			});
		}
	}));
	addReportDialog.open().center();
	return false;
};

function initReportType() {

	var dataSource = new kendo.data.DataSource({
		transport : {
			read : {
				url : getBaseUrl() + '/code/analysisReportType/UNREPOSITORY/O',
				dataType : 'json'
			}
		}
	})
	$("#reportType").kendoDropDownList({
		dataTextField : "name",
		dataValueField : "value",
		dataSource : dataSource,
		index : 0
	});

};

function initUploadReport(e) {
	$("#uploadReport").kendoUpload({
		localization : {
			select : "請選擇報告",
			uploadSelectedFiles : "上傳報告"
		},
		multiple : false,
		async : {
			saveUrl : getBaseUrl() + "/analysisReport/upload",
			autoUpload : false
		},
		upload : function(e) {
			var changeNo = $('#reportWindow #changeNo').val();
			var reportType = $("#reportType").data("kendoDropDownList").value();

			e.data = {
				changeNo : changeNo,
				reportType : reportType,
				flowStatus : 'UNREPOSITORY'
			};
		},
		success : function(e) {
			MessageBox.success("報告已新增");
			setTimeout(function() {
				var changeNo = $('#reportWindow #changeNo').val();
				var childGrid = $(".reports.changeNo" + changeNo).data("kendoGrid");
				childGrid.dataSource.read();

				var dialog = $("#reportWindow").data("kendoWindow");
				dialog.destroy();
			}, 1000);

		}
	});
};

// 報告初始化要做的動作
function adjustReportInput(e) {
	initTriggerTimePicker();
	changeReportGridButton(e);
};

// 將啟動時間的input轉為dateTimePickers
function initTriggerTimePicker() {
	// 找到class name 為 triggerTimePicker 的啟動時間input
	var triggerTimePickers = $("input[class='triggerTimePicker']");

	//
	function convertDayTime(datetime) {

		if (datetime == "null") {

			return " ";

		} else {

			return new Date(datetime);
		}

	}
	$(triggerTimePickers).each(function(e) {

		var datetime = $(this).val();

		$(this).kendoDateTimePicker({
			value : convertDayTime(datetime),
			format : "{0:yyyy-MM-dd HH:mm}",
			change : function(e) {
				// 選擇的 datepicker 日期時間
				var triggerTime = this.value();
				// function event 中找到 input id 為 triggerTime+reportNo
				var idReportNo = e.sender.element[0].id;
				// 去除 input id 前面的 triggerTime字串得到 reportNo
				var reportNo = idReportNo.slice(11);

				var myObject = new Object();
				myObject.reportNo = reportNo;
				myObject.triggerTime = triggerTime;

				$.blockUI();
				$.ajax({
					method : "POST",
					url : getBaseUrl() + "/analysisReport/triggerTime/update",
					contentType : 'application/json',
					data : JSON.stringify(myObject),
					success : function(result) {
						$.unblockUI();
						MessageBox.success("成功");
						setTimeout(function() {

						}, 1000);

					},
					error : function(xhr, result, status) {
						$.unblockUI();
						MessageBox.error("失敗");

					}
				});
			}
		});
	});

};

function changeReportGridButton(e) {
	
    $(".k-grid-啟動").each(function () {
    	var curGrid = $(this).closest('.reports.k-grid').data("kendoGrid");
    	var curDataItem = curGrid.dataItem($(this).closest("tr"));

        if (curDataItem.retriggerable === 'false') {
        	$('#triggerTime' + curDataItem.reportNo).data('kendoDateTimePicker').enable(false);
            $(this).remove();
        }
        
        if (curDataItem.isHidden === 'true') {
        	$(this).remove();
        }
 
    });
    
    $(".k-grid-刪除任務").each(function () {
    	var curGrid = $(this).closest('.reports.k-grid').data("kendoGrid");
    	var curDataItem = curGrid.dataItem($(this).closest("tr"));
    	 if (curDataItem.isHidden === 'true') {
    		 $(this).remove();
    	 }
    });
    
    $(".k-grid-前置任務").each(function () {
    	var curGrid = $(this).closest('.reports.k-grid').data("kendoGrid");
    	var curDataItem = curGrid.dataItem($(this).closest("tr"));
    	 if (curDataItem.isHidden === 'true') {
    		 $(this).remove();
    	 }
    });
    
    $(".k-grid-編輯").each(function () {
    	var curGrid = $(this).closest('.reports.k-grid').data("kendoGrid");
    	var curDataItem = curGrid.dataItem($(this).closest("tr"));
    	 if (curDataItem.isHidden === 'true') {
    		 $(this).remove();
    	 }
    });
    
    $(".k-grid-取消前置任務").each(function () {
    	var curGrid = $(this).closest('.reports.k-grid').data("kendoGrid");
    	var curDataItem = curGrid.dataItem($(this).closest("tr"));
    	 if (curDataItem.isHidden === 'true') {
    		 $(this).remove();
    	 }
    });
};

function refreshAnalysisReport(changeNo) {

	var childGrid = $(".reports.changeNo" + changeNo).data("kendoGrid");
	childGrid.dataSource.read();
	MessageBox.success("報告清單重新整理完成");
	
	return false;
}
