package com.gss.adm.scd;

import java.util.List;

import com.google.common.collect.Lists;

public class Lambda {
	
	
	public static void main(String args[]) {
		
		MathOperation addition = (int a, int b) -> sum(a, b);
		
		MathOperation subtraction = (a, b) -> sum(a, b);
		
		MathOperation multiplication = (int a, int b) -> {
			Math math = new Math();
			return math.sum(number, number);
		};
		
		List<Integer> numbers = Lists.newArrayList();
		
		numbers.stream().forEach(number -> {
			Math math = new Math();
			math.sum(number, number);
		});
		
		numbers.stream().forEach(number -> sum(number, number));
		
	}
	
	private static int sum(int a, int b){
		return a + b;
	}
}