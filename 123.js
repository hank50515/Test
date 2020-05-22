$(document).ready(function () {

    
    $("div#submenu > div > a:contains('要')").addClass("link hassub on");
    $("div#submenu > div > div > a:contains('團')").click(function() {
        var urlMain = location.href;
        if(r==true){
            self.btnSubmitLock(true);
            $('#inviteForm').attr("action", url);
            $('#inviteForm').submit();
        }
    });
    
    $(".ui-spinner-button").click(function() {
        $(this).siblings("input").change();
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
	console.log(vm);
    
});