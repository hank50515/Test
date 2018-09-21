// Function Declaration
function helloFunction() {
	alert("Hello");
}

helloFunction();

// Function Constructor
var multiplicationFunction = new Function("a", "b", "return a * b");

multiplicationFunction(5, 10);

// Arrow Function 
var additionFunction = (x,y) => {
	return x + y;
}

additionFunction(1, 2);

// Object Method
var person = {
	firstName : "John",
	lastName : "Doe",
	fullName : function() {
		return this.firstName + " " + this.lastName;
	}
}

person.fullName();

// Function prototype
employee.prototype.display = function(){
	alert("Hello World");
}

employee.display();

// jQuery FN Extend
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

$("#checkBtn").check();