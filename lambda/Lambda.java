package com.gss.adm.scd;

import java.util.List;

import com.google.common.collect.Lists;

public class Lambda {
	
	
	public static void main(String args[]) {
		
		MathOperation MathOperationMutiply1 = (int a, int b) -> mutiply(a, b);

		MathOperation MathOperationMutiply2 = (a, b) -> mutiply(a, b);
		
		MathOperation MathOperationMutiply3 = (int a, int b) -> mutiply(a, b);
		
		MathOperation MathOperationMutiply4 = (int a, int b) -> mutiply(a, b);
		
		MathOperation MathOperationMutiply5 = (int a, int b) -> mutiply(a, b);
		
		MathOperation MathOperationSum = (int a, int b) -> {
			LambdaMath math = new LambdaMath();
			return math.sum(number, number);
		};
		
		List<Integer> numbers = Lists.newArrayList();
		
		numbers.stream().forEach(number -> {
			LambdaMath math = new LambdaMath();
			math.sum(number, number);
		});
		
		numbers.stream().forEach(number -> mutiply(number, number));
	}
	
	private static int mutiply(int a, int b){
		return a * b;
	}
}