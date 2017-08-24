///需求單號：51-SOURCE_CARD
/// <reference path="../../jQuery/jquery-1.11.1.min.js" />
/// <reference path="../Common/Extend.js" />
/// <reference path="../Common/SharedMethodObject.js" />
　
//功能: CtlCaseProcWithSaveBtn-案件儲存並傳送
//描述: 案件儲存並傳送
//歷程: 1. 2015/06/10   1.00   Daniel Lee   Create
//          2. 2016/10/14   1.01   Steven_Chen  TFBNCLMC-1033 新增options.guide_funcID和ProcLoadPage Method  需求單號:201610170229-00
　
(function ($) {
　
    var kendo = window.kendo,
        ui = kendo.ui,
        BestValidateButton = ui.BestValidateButton;
　
    /**
     * @classdesc 案件儲存並傳送
     * @class BestCtlCaseProcWithSaveBtn
     */
    var GSSCtlCaseProcWithSaveBtn = BestValidateButton.extend({
　
        init: function (element, options) {
　
            // cache a reference to this
            var that = this
                , initOptions = {}
                , defaultOptions = $.extend({}, that.options);
　
            //取得網頁上的設定值
            initOptions = kendo.parseOptions(element, that.options);
　
            //將傳入的options與網頁的上的options合併
            options = $.extend(initOptions, options);
            options = $.extend(defaultOptions, options);
　
            // make the base call to initialize this widget
            BestValidateButton.fn.init.call(this, element, options);
　
            // actually create the UI elements. this is a private method
            // that is below
            that._custCreate();
　
            //2015/09/01 加入RW 控管 TFBNCLM-812
            //處理readonly
            //Modify By Tony Lee http://gssjira.gss.com.tw/browse/TFBNCLM-3379 STEP_ID為999或998才可用PageRW控管
            if (that.options.readonly_status && (CaseObject.Flow.StepID == "999" || CaseObject.Flow.StepID == "998")) {
                that.Breadonly(true);
            } else {
                that.Breadonly(false);
            }
　
　
        },
　
        options: {
　
            /**
             * @property {string} name Widget的namespace
             * @default BestCtlCaseProcWithSaveBtn
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            name: "BestCtlCaseProcWithSaveBtn",
　
            /**
             * @property {string} window_url 視窗連結路徑
             * @default 空字串
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            window_url: "Scripts/GSS/CustomControl/Window/CasePhraseWindow.aspx",
　
            /**
             * @property 路徑
             * @default 空字串
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            currentPath: "",
　
            /**
             * @property {string} 案件編號
             * @default ""
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            case_no: "",
　
            /**
             * @property {string} 案件歷程序號
             * @default ""
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            opn_seq_no: "",
　
            /**
             * @property {string} 處理人員
             * @default ""
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            proc_taker_id: "",
　
            /**
             * @property {string} 處理方式
             * @default ""
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            proc_id: "",
　
            /**
             * @property {string} 處理人員
             * @default ""
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            recv_taker_id: "",
　
            /**
             * @property {string} btn的樣式
             * @default B
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            btn_kind: "B",
　
            /**
             * @property {string} btn的呈現文字
             * @default ""
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            btn_name: "",
　
            /**
             * @property {function} dataBoundFN 按鈕click時，前客製化執行的function
             * @default null
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            pre_delegate_function: null,
　
            /**
             * @property {function} dataBoundFN 按鈕click時，後客製化執行的function
             * @default null
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            aft_delegate_function: null,
　
            /**
             * @property {function} flow_type
             * @default ""
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            flow_type: "",
　
            /**
             * @property {function} proc_desc
             * @default ""
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            proc_desc: null,
　
            /**
            * @property {function} proc_kind
            * @default ""
            * @MemberOf BestCtlCaseProcWithSaveBtn
            */
            proc_kind: null,
　
            /**
            * @property {function} load_initPage 傳送結束後始否導頁,若為N則不導業
            * @default ""
            * @MemberOf BestCtlCaseProcWithSaveBtn
            */
            load_initPage: "",
　
            /**
             * @property {function} top_div_obj
             * @default $("<div/>")
             * @MemberOf BestCtlCaseProcWithSaveBtn
             */
            top_div_obj: $("<div/>"),
            /**
             * @property {string} readonly_status 控制項唯讀狀態
             * @default false
             * @MemberOf BestButton
             * @desc 供整頁唯讀, 區域唯讀, 動態唯讀使用
             */
            readonly_status: false,
            /**
             * @property {string} guide_funcID 客製導頁的FUNCID
             * @default string.empty
             * @MemberOf BestCtlCaseProcWithSaveBtn
             * @desc 客製導頁的FUNCID
             */
            guide_funcID: ""
　
        },
　
        _custCreate: function () {
            var that = this;
　
            //依照btn的樣式設置class
            if (that.options.btn_kind == "P") {
                that.element.find("i").attr("class", "fa fa-mail-forward fa-lg");
            } else if (that.options.btn_kind == "B") {
                that.element.find("i").attr("class", "fa fa-send");
            }
　
            //設置呈現文字
            that.element.find("i").after("<span>" + that.options.btn_name + "</span>");
　
            //綁定click事件
            that.element.click(function () {
                SharedMethodObject.Page.LockWindow();
                that.LockWindow();
                that._clickEvent();
            });
        },
　
        //處理click事件
        _clickEvent: function () {
            var that = this,
                procKind,
                nowOPN;
　
            SharedMethodObject.Ajax.Read({
                url: BaseObject.ServiceUrl + "/WorkFlow.asmx/ReadCaseOPN",
                data: { SEQ_NO: that.options.opn_seq_no },
                async: false,
                successAction: function (SearchResult) {
                    nowOPN = SearchResult[0];
                }
            });
　
　
            if (nowOPN.PROC_DT_STR !== "00000000000000") {
                alert("傳送失敗,案件已不在目前作業關卡");
　
                SharedMethodObject.Page.LockWindow(false);
                that.LockWindow(false);
                var objPage = SharedMethodObject.Page.FindNowPage();
                if (objPage) {
                    var objNowWindow = objPage.data("kendoBestWindow");
                    if (objNowWindow) {
                        objNowWindow._closeWindowEvent();
                    }
                }
　
                that.ProcLoadInitPage();
　
                return false;
            }
　
            //送件前執行function
            if (that.options.pre_delegate_function != null) {
                if (that.options.pre_delegate_function(that) == false) {
                    //TFBNCLM-1191 處理方式不選擇按傳送出現提示後出現一直lock畫面
                    SharedMethodObject.Page.LockWindow(false);
                    that.LockWindow(false);
                    return false;
                }
            }
　
            //若proc_desc不為空則更新proc_desc
            if ($.isEmptyObject(that.options.proc_desc) == false) {
                SharedMethodObject.Ajax.Read({
                    url: BaseObject.ServiceUrl + "/WorkFlow.asmx/UpdateCaseOPN",
                    data: { SEQ_NO: that.options.opn_seq_no, PROC_DESC: that.options.proc_desc.Bvalue() },
                    successAction: function (result) {
                    }
                });
            }
　
            //取得FLOW_TYPE
            SharedMethodObject.Ajax.Read({
                url: BaseObject.ServiceUrl + "/CRD.asmx/ReadCaseOpn_SeqNo",
                data: { SEQ_NO: that.options.opn_seq_no },
                successAction: function (result) {
　
                    that.options.case_no = result[0].CASE_NO;
                    that.options.flow_type = result[0].FLOW_TYPE;
　
                    var objData = {
                        OpnSeqNo: that.options.opn_seq_no, FlowType: that.options.flow_type, CaseNo: that.options.case_no, ProcId: that.options.proc_id,
                        ProcTakerId: BaseObject.UserInfo.UserID
                    },
                        objProcPreConfirm,
                        objGetProcKind;
　
                    //取得分案方式
                    objGetProcKind = function () {
                        SharedMethodObject.Ajax.Read({
                            url: BaseObject.ServiceUrl + "/OTH.asmx/GetProcKind",
                            data: { FLOW_TYPE: that.options.flow_type, PROC_ID: that.options.proc_id },
                            successAction: function (result) {
                                procKind = result;
                                that.options.proc_kind = result;
　
                                var objData;
　
                                objData = {
                                    CASE_NO: that.options.case_no, OPN_SEQ_NO: that.options.opn_seq_no,
                                    FLOW_TYPE: that.options.flow_type, PROC_ID: that.options.proc_id,
                                    PROC_TAKER_ID: that.options.proc_taker_id
                                };
　
                                //若recv_taker_id不為空值 則依照procKind決定取recv_taker_id的方式
                                //TFBNCLM-13611 循序分案(PROC_TAKER IN I.帳務循序分案, P.不分單位循序分案)放在前處理後再執行
                                //2016/11/25 Modify By Derek Chou (需求單號：201611290234-00)
                                if (that.options.recv_taker_id == "") {
                                    if (procKind == "A" || procKind == "B") {
                                        SharedMethodObject.Page.LockWindow(false);
                                        that.LockWindow(false);
                                        //開燈箱
                                        that.WindowOpen(objData);
                                    } else if (procKind == "Y" || procKind == "Z" || procKind == "I") {
                                        that.AferOpenWindow();
                                    } else {
                                        var procTaker = PublicMethodObject.GetCaseProcTaker(objData);
                                        procTaker = procTaker == null ? "" : procTaker;
　
                                        if (procTaker.length > 0) {
                                            that.options.recv_taker_id = procTaker[0].EMP_NO;
                                            that.AferOpenWindow();
                                        } else {
                                            if (that.options.proc_id == "111110P0") {
                                                that.AferOpenWindow();
                                            } else {
                                                alert('送件失敗，無收件人員。')
                                                SharedMethodObject.Page.LockWindow(false);
                                                that.LockWindow(false);
                                            }
                                        }
                                    }
                                } else {
                                    that.AferOpenWindow();
                                }
　
                            }
                        });
                    };
　
                    //處理步驟前檢核警告
                    objProcPreAlertA1 = function () {
                        if (!that.DoCaseProcPreAlertA1(objData, objProcPreConfirm)) {
                            SharedMethodObject.Page.LockWindow(false);
                            that.LockWindow(false);
                            return false
                        };
                    };
　
                    //處理步驟前檢核警告
                    objProcPreConfirm = function () {
                        var objConfirm = {
                            OpnSeqNo: that.options.opn_seq_no,
                            FlowType: that.options.flow_type,
                            CaseNo: that.options.case_no,
                            ProcId: that.options.proc_id,
                            ProcTakerId: BaseObject.UserInfo.UserID
                        };
　
                        //進行處理步驟前檢核確認
                        if (!that.DoCaseProcPreConfirm(objConfirm, objGetProcKind)) {
                            SharedMethodObject.Page.LockWindow(false);
                            that.LockWindow(false);
                            return false
                        };
                    };
　
                    if (!that.DoCaseProc("DoCaseProcPreAlert", objData, objProcPreAlertA1)) {
                        SharedMethodObject.Page.LockWindow(false);
                        that.LockWindow(false);
                        return false
                    };
　
                }
            });
　
        },
　
        //處理步驟
        DoCaseProc: function (method, objData, callback) {
            var that = this,
                rtn;
　
            SharedMethodObject.Ajax.Read({
                url: BaseObject.ServiceUrl + "/WorkFlow.asmx/" + method,
                data: objData,
                async: false,
                successAction: function (result) {
                    if (result.errMSG !== "") {
                        alert(result.errMSG);
                        rtn = false;
                    } else {
                        rtn = true;
                        if (callback !== undefined) {
                            callback();
                        }
                    }
                }
            });
            return rtn;
        },
　
　
        //處理步驟
        DoCaseProcPre: function (objData, callback) {
            var that = this;
　
            SharedMethodObject.Ajax.Read({
                url: BaseObject.ServiceUrl + "/WorkFlow.asmx/DoCaseProcPre",
                data: objData,
                successAction: function (result) {
                    if (result.errMSG !== "") {
                        alert(result.errMSG);
                        //前處理不過解鎖畫面
                        SharedMethodObject.Page.LockWindow(false);
                        that.LockWindow(false);
                    } else {
                        //TFBNCLM-13611 循序分案(PROC_TAKER IN I.帳務循序分案, P.不分單位循序分案)放在前處理後再執行
                        if (objData.ProcTaker == "I") {
                            var objProcData = {
                                CASE_NO: that.options.case_no, OPN_SEQ_NO: that.options.opn_seq_no,
                                FLOW_TYPE: that.options.flow_type, PROC_ID: that.options.proc_id,
                                PROC_TAKER_ID: that.options.proc_taker_id
                            };
　
                            var procTaker = PublicMethodObject.GetCaseProcTaker(objProcData);
                            procTaker = procTaker == null ? "" : procTaker;
　
                            if (procTaker.length > 0) {
                                that.options.recv_taker_id = procTaker[0].EMP_NO;
                                if (callback !== undefined) {
                                    callback();
                                }
                            } else {
                                alert('送件失敗，無收件人員。')
                                SharedMethodObject.Page.LockWindow(false);
                                that.LockWindow(false);
                            }
                        } else {
                            if (callback !== undefined) {
                                callback();
                            }
                        }
                    }
                }
            });
        },
　
        //處理步驟
        DoCaseProcAft: function (objData, callback) {
            var that = this;
　
            SharedMethodObject.Ajax.Read({
                url: BaseObject.ServiceUrl + "/WorkFlow.asmx/DoCaseProcAft",
                data: objData,
                successAction: function (result) {
                    if (result.errMSG !== "") {
　
                        //送件後執行function
                        if (that.options.aft_delegate_function != null) {
                            that.options.aft_delegate_function(that);
                        }
　
                        //Mdoify By Derek Chou 2016/10/04
                        //需求單號: 201610040276-00
                        //取消ajax檢查
                        //that.ProcUndoneAJAX(function () {
　
                        //Mdoify By Derek Chou
                        //需求單號: 201609290195-00
                        //避免USER在傳送Alert後沒按確認,導致後續處理沒執行,故將Alert訊息搬至所有處理結束
                        alert(objData.Msg);
                        alert(result.errMSG);
　
                        SharedMethodObject.Page.LockWindow(false);
                        that.LockWindow(false);
　
                        //後處理失敗因為案件已送走,所以還是要導頁
                        if (that.options.load_initPage != "N") {
                            that.ProcLoadInitPage();
                        } else if (that.options.guide_funcID != "") {
                            that.ProcLoadPage();
                        };
                        //});
　
                    } else {
　
                        //送件後執行function
                        if (that.options.aft_delegate_function != null) {
                            that.options.aft_delegate_function(that);
                        }
　
                        //that.ProcUndoneAJAX(function () {
　
                        //避免USER在傳送Alert後沒按確認,導致後續處理沒執行,故將Alert訊息搬至所有處理結束
                        alert(objData.Msg);
　
                        SharedMethodObject.Page.LockWindow(false);
                        that.LockWindow(false);
　
                        //載入初始頁
                        if (that.options.load_initPage != "N") {
                            that.ProcLoadInitPage();
                        } else if (that.options.guide_funcID != "") {
                            that.ProcLoadPage();
                        };
                        //});
                    }
                }
            });
        },
　
        //進行處理步驟前檢核確認
        DoCaseProcPreAlertA1: function (objData, callback) {
            var that = this,
                rtn;
　
            SharedMethodObject.Ajax.Read({
                url: BaseObject.ServiceUrl + "/WorkFlow.asmx/DoCaseProcPreAlertA1",
                data: objData,
                async: false,
                successAction: function (result) {
　
                    if (result.errMSG !== "") {
                        alert(result.errMSG);
                    }
                }
            });
　
            if (callback !== undefined) {
                callback();
            }
　
            return true;
        },
　
        //進行處理步驟前檢核確認
        DoCaseProcPreConfirm: function (objPara, callback) {
            var that = this,
                rtn;
　
            SharedMethodObject.Ajax.Read({
                url: BaseObject.ServiceUrl + "/WorkFlow.asmx/DoCaseProcPreConfirm",
                data: { OpnSeqNo: objPara.OpnSeqNo, FlowType: objPara.FlowType, CaseNo: objPara.CaseNo, ProcId: objPara.ProcId, ProgOrder: objPara.ProgOrder },
                async: false,
                successAction: function (result) {
                    if (result.errMSG !== "") {
                        if (confirm(result.errMSG + "，是否繼續傳送")) {
                            objPara.ProgOrder = result.ProgOrder;
                            rtn = that.DoCaseProcPreConfirm(objPara);
                        } else {
                            rtn = false;
                        };
                    } else {
                        rtn = true;
                    }
                }
            });
　
            if (rtn == true && callback !== undefined) {
                callback();
            }
            return rtn;
        },
　
        //開窗後須執行的事件
        AferOpenWindow: function () {
            var that = this,
                objData = {
                    OpnSeqNo: that.options.opn_seq_no, FlowType: that.options.flow_type, CaseNo: that.options.case_no, ProcId: that.options.proc_id,
                    ProcTakerId: BaseObject.UserInfo.UserID, ProcTaker: that.options.proc_kind
                },
                objOPN = {},
                objDoCaseProcAft;
　
            //後處理_Method
            objDoCaseProcAft = function (obj) {
　
                var objData = {
                    OpnSeqNo: that.options.opn_seq_no, FlowType: that.options.flow_type, CaseNo: that.options.case_no, ProcId: that.options.proc_id,
                    ProcTakerId: BaseObject.UserInfo.UserID, NextOpnSeqNo: obj.nextOPN, Msg: obj.Msg
                };
　
                that.DoCaseProcAft(objData)
            };
　
            SharedMethodObject.Page.LockWindow(true);
            that.LockWindow(true);
　
            //前處理->案件傳送->後處理
            that.DoCaseProcPre(objData, function () { that.SendWorkItem(objOPN, objDoCaseProcAft) });
        },
　
        //案件傳送
        SendWorkItem: function (objOPN, callback) {
            var that = this,
                rtn;
　
            SharedMethodObject.Ajax.Read({
                url: BaseObject.ServiceUrl + "/WorkFlow.asmx/SendWorkItem",
                data: {
                    OpnSeqNo: that.options.opn_seq_no, RecvTakerId: that.options.recv_taker_id, CaseNo: that.options.case_no,
                    ProcId: that.options.proc_id, ProcTakerId: that.options.proc_taker_id, UPD_USRID: BaseObject.UserInfo.UserID,
                    ProcTaker: that.options.proc_kind
                },
                successAction: function (result) {
                    if (result.length !== 0) {
                        var OPN, strMSG;
                        //考慮可能產生多個新關卡,畫面上只會提示使用者選擇傳送的關卡
                        for (i = 0; i < result.length; i++) {
                            SharedMethodObject.Ajax.Read({
                                url: BaseObject.ServiceUrl + "/WorkFlow.asmx/ReadCaseOpnStep",
                                data: { SEQ_NO: result[i].toString() },
                                async: false,
                                successAction: function (SearchResult) {
                                    OPN = SearchResult[0];
　
                                    if (OPN.STEP_ID == that.options.proc_id.substring(3, 6)) {
　
                                        //若STEP_MSG有值則顯示STEP_MSG
                                        if (OPN.STEP_MSG) {
                                            strMSG = "下一關：" + OPN.STEP_MSG;
                                        }
                                        else {
                                            strMSG = "下一關：" + OPN.STEP_NM;
　
                                            if (OPN.RECV_TAKER_ID) {
                                                //PROC_TAKER不為 Z-不指定處理人員且不為系統人員時，才顯示接收人
                                                if (that.options.proc_kind != "Z" && OPN.RECV_TAKER_ID.indexOf("SYS") < 0) {
                                                    strMSG += "\n接收人：" + OPN.RECV_TAKER_ID + "-" + OPN.RECV_TAKER_NM
                                                }
                                            }
                                        }
　
                                        objOPN.nextOPN = OPN.SEQ_NO;
                                    }
                                }
                            });
                        }
　
                        if (!objOPN.nextOPN) {
　
                            if (OPN.STEP_MSG) {
                                strMSG = "下一關：" + OPN.STEP_MSG;
                            }
                            else {
                                strMSG = "下一關：" + OPN.STEP_NM;
                                if (OPN.RECV_TAKER_ID) {
                                    if (OPN.RECV_TAKER_ID.indexOf("SYS") < 0) {
                                        strMSG += "\n接收人：" + OPN.RECV_TAKER_ID + "-" + OPN.RECV_TAKER_NM
                                    }
                                }
                            }
　
                            objOPN.nextOPN = OPN.SEQ_NO;
                        }
　
                        if (!strMSG) {
                            objOPN.Msg = MessageObject.SumitSuccess;
                        } else {
                            objOPN.Msg = strMSG;
                        }
　
                        rtn = true;
                        callback(objOPN);
                    }
                    else {
                        if (that.options.proc_id.indexOf("END") >= 0) {
                            objOPN.Msg = MessageObject.SumitSuccess;
                            rtn = true;
                            callback(objOPN);
                        } else {
                            alert(MessageObject.SumitFail);
                            rtn = false;
                        }
                    }
                }
            });
　
            return rtn;
        },
　
        WindowOpen: function (obj) {
            /// <summary>
            /// 開啟燈箱
            /// </summary>
            /// <param name="obj">要傳遞參數</param>
　
            var objOptions = {},
                that = this;
　
            objOptions.title = "接收人員";
            objOptions.content = "Window/OTH/RecvTaker.aspx";
            objOptions.width = $(window).width() * 0.8;
            objOptions.position = { top: 100, left: 100 };
            objOptions.fn = function (result) {
                that.options.recv_taker_id = result.RECV_TAKER_ID;
                that.AferOpenWindow();
            }
　
            //設定 Options
            objWindow = $.fn.BestWindow(objOptions);
            //設定 Parameter
            objWindow.parameter(obj);
            //open window
            objWindow.open();
　
        },
　
        LockWindow: function (isLock) {
            /// <summary>
            /// 開啟燈箱
            /// </summary>
            /// <param name="obj">要傳遞參數</param>
　
            var objPage = SharedMethodObject.Page.FindNowPage();
            if (objPage) {
                var objNowWindow = objPage.data("kendoBestWindow");
                if (objNowWindow) {
                    objPage.LockUI(isLock);
　
                    //TFBNCLM-11227 lockui僅針對視窗大小，scrollTop避免不知案件正在傳送
                    if (isLock) {
                        objPage.scrollTop(0);
　
                    }
                }
            }
        },
　
        ProcLoadInitPage: function () {
            /// <summary>
            /// 導頁至預設頁
            /// </summary>
            /// <param name="obj">要傳遞參數</param>
　
            var objPage = SharedMethodObject.Page.FindNowPage();
            if (objPage) {
                var objNowWindow = objPage.data("kendoBestWindow");
                if (objNowWindow) {
                    objNowWindow._closeWindowEvent();
                }
            }
　
            var objData = {}, objPage = $("#Template_Vertical");
　
            objData = SharedMethodObject.Page.GetFuncInfo("Init");
　
            $("#Template_Top").load(objData.top.URI);
            $("#Template_Bottom").load(objData.bottom.URI);
　
            SharedMethodObject.Page.LoadContent(objData);
        },
　
        ProcLoadPage: function () {
            /// <summary>
            /// 導頁
            /// </summary>
            /// <param name="obj">要傳遞參數</param>
　
            var objPage = SharedMethodObject.Page.FindNowPage();
            if (objPage) {
                var objNowWindow = objPage.data("kendoBestWindow");
                if (objNowWindow) {
                    objNowWindow._closeWindowEvent();
                }
            }
　
            var objData = {};
　
            objData = SharedMethodObject.Page.GetFuncInfo(this.options.guide_funcID);
　
            SharedMethodObject.Page.LoadContent(objData);
        },
　
        ProcUndoneAJAX: function (fn_success) {
            /// <summary>
            /// 導頁前處理AJAX未完成
            /// </summary>
            var that = this,
                isactive = $.active > 0 ? true : false;
　
            if (isactive) {
                setTimeout(function () { that.ProcUndoneAJAX(fn_success) }, 2000);
            } else {
                if (fn_success) {
                    fn_success();
                }
            }
　
        },
　
        /**
         * @desc 設定控制項的options
         * @memberof BestCtlCaseProcWithSaveBtn
         * @method BsetOptions
         * @example
         * 設定options: element.BestCtlCaseProcWithSaveBtn().BsetOptions({ readonly_mode: "2" });
         */
        BsetOptions: function (options) {
　
            var that = this;
　
            BestValidateButton.fn.setOptions.call(that, options);
　
        },
        /**
         * @desc 唯讀處理
         * @memberof BestButton
         * @method Breadonly
         * @param {boolean} readonly 是否唯讀
         * @example
         * element.BestButton().Breadonly();
         */
        Breadonly: function (readonly) {
　
            readonly = (readonly === undefined) ? true : readonly;
　
            if (readonly) {
                this.element.hide();
            }
　
            if (!readonly) {
                this.element.show();
            }
　
        },
    });
　
    ui.plugin(GSSCtlCaseProcWithSaveBtn);
　
　
})(jQuery);