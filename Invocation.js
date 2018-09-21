function myFunction() {
	document.getElementById("demo").style.color = "red";
}

myFunction();

var myFunction2 = new Function("a", "b", "return a * b");

myFunction2();

var myFunction3 = (x,y) => {
	return x + y;
}

myFunction3();

var myObject = {
	firstName : "John",
	lastName : "Doe",
	fullName : function() {
		return this.firstName + " " + this.lastName;
	}
}

function myFunction4() {
	myObject.fullName();
}

myObject.fullName();

myFunction3.apply();

myFunction3.call();

employee.prototype.display = function(){
	var emp = new employee();
	emp.display();
}

employee.display();

jQuery.fn.extend({
	check : function(){
		return this.each(function(){
			this.checked = true;
		});
	},
	unCheck : function(){
		return this.each(function(){
			this.checked = false;
		});
	}
});

$("#abc").check();