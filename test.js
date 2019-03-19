function initFrm() {
	$('#123++').css("display", "");
	$("#123++").val(TestInnerJSMethod());
	$("#456").val(TestInnerJSMethod());
	$.ajax({
		url : "welcome/test",
		context : document.body
	}).done(function() {
		$(this).addClass("done");
	});
}