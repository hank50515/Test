$(document).ready(function () {
    init();
    
    /*左方menu 點選顏色鎖定*/  
    $("div#submenu > div > a:contains('要保')").addClass("link hassub on");
    $("div#submenu > div > div > a:contains('要保輸入')").addClass("link sub2 on");
    $("div#submenu > div > div > a:contains('團體輸入投保')").attr("href", "#");
    $("div#submenu > div > div > a:contains('團體輸入投保')").click(function() {
        var urlMain = location.href;
        var ii = urlMain.search("action/prdt/GTL/");
        var urlRoot = urlMain.substring(0,ii);
        var r=confirm("確定要轉至[團體輸入投保]?!");
        if(r==true){//改變表單的前往位址...並且繳交表單
            url = urlRoot + "action/prdt/GTL/page_change_invite_grp";
            $("#busyWindow").show();
            self.btnSubmitLock(true);
            $('#inviteForm').attr("action", url);
            $('#inviteForm').submit();
        }
    });
    
    /*datebox植入*/
    $('#txtCustBthAAD').datepicker({
        maxDate: '-1',
        changeYear : true,
        changeMonth: true,
        yearRange: "-100:-20",
        defaultDate : (new Date(new Date().getFullYear() - 20 + "/01/01") - new Date()) / (1000 * 60 * 60 * 24),
        onSelect: function(dateText, inst) {
            alert(dateText);
            //alert(inst);
            //dateText = dateText-19110000;
            $('#txtCustBthAAD').val(dateText);
        }
    });
    
    $('#txtCustBthIAD').datepicker({
        maxDate: '-1',
        changeYear : true,
        changeMonth: true,
        yearRange: "-100:+0",
        defaultDate : (new Date(new Date().getFullYear() - 20 + "/01/01") - new Date()) / (1000 * 60 * 60 * 24)
    });
    
    $("#nPageSize").spinner({ min: 10 });
    $("#nPageIndex").spinner({ min: 1 });

    //針對jquery UI spinner 的漏洞補強
    $(".ui-spinner-button").click(function() {
        $(this).siblings("input").change();
    });

    /*投保方案  畫面聯動   Start*/
    $("#rateTypeInforView").hide();
    
    /*投保方案  畫面聯動   End*/
    /*在頁面完全Load結束後所清除的站存資料*/
    function afterLoadAction() {
        //確認已LOAD結束的註記
        $("#hidAfterLoadMark").attr("value", "Y");
        $("#hidTourAreaCD").attr("value", "");
        //報價轉要保帶回的暫存參數
        $("#projectListLoad").attr("value","[]");
    }
    
    
    function InviteInputViewModel() {
        var riskIDs = ["09", "10", "11"];
        //控制是否允許清除專案
        var urlMain = location.href;
        var ii = urlMain.search("action/prdt/GTL/");
        var urlRoot = urlMain.substring(0,ii);
        var chaValue = "0.0"; //劫機慰問 
        var othValue = "0.0"; //額外費用 
        var cfpValue = "0.0"; //竊盜損失
        var clmValue = "0.0"; //食物中毒
        //var limitTourDaysMax = 0; //旅遊天數上限
        //var limitAmtMax = 0; //ADD上限保額
        
        // Data
        
	}

    var vm = new InviteInputViewModel();
    
    ko.applyBindings(vm, document.getElementById('content'));
        
    $('#backCpc').click(function() {
        var urlMain = location.href;
        var ii = urlMain.search("action/prdt/GTL/");
        var urlRoot = urlMain.substring(0,ii);
        url = urlRoot+"/action/prdt/GTL/inviteCpcBack";
        var r=confirm("確定要轉回CPC登打?!");
        if(r==true){
            $("#busyWindow").show();
            self.btnSubmitLock(true);
            $('#inviteForm').attr("action", url);
            $('#inviteForm').submit();
        }
    });
    
});