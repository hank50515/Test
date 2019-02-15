package com.gss.adm.antlr.java.listener;

import java.util.List;
import java.util.Objects;

import com.google.common.collect.Lists;
import com.gss.adm.antlr.java.Math;

public class MethodReference {
	public static void main(String args[]) {

		List<Integer> numbers = Lists.newArrayList();

		numbers.stream().forEach(System.out::println);
		numbers.stream().forEach(Objects::nonNull);
		numbers.stream().forEach(Math::addOne);
	}
}
