$(document).ready(function () {

    	console.log("123");
    $("div#submenu > div > div > a:contains('團')").click(function() {
        var urlMain = location.href;
        if(r==true){
            self.btnSubmitLock(true);
            $('#inviteForm').attr("action", url);
            $('#inviteForm').submit();
        }
    });
    
    $(".ui-spinner-button").click(function() {
        console.log();
    });

    function afterLoadAction() {
        $("#hidAfterLoadMark").attr("value", "Y");
    }
    
    
    function InviteInputViewModel() {
        var urlMain = location.href;
        
        var self = this;
		console.log(self);
        //TODO

	}

    var vm = new InviteInputViewModel();
    $("div#submenu > div > a:contains('要')").addClass("link hassub on");
});