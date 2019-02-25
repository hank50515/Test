package com.gss.adm.antlr.java.listener;

import java.util.List;
import java.util.Objects;

import com.google.common.base.Predicate;
import com.google.common.collect.Lists;
import com.gss.adm.antlr.java.RelationShip;
import com.gss.adm.antlr.java.Math;
import com.gss.adm.antlr.java.Project;

public class MethodReference {
	public static void main(String args[]) {

		List<Integer> numbers = Lists.newArrayList();

		// Java api
		numbers.stream().forEach(System.out::println);
		numbers.stream().forEach(Objects::nonNull);
		// Class::MethodName
		numbers.stream().forEach(Math::addOne);
		numbers.stream().forEach(com.gss.adm.antlr.java.Math::addOne);
		numbers.sort(Math::comparator);
		
		// Class.Field::MethodName
		numbers.sort(Math.sum::sum);
		numbers.sort(Math.sum.aggregate::aggregateAll);
		numbers.sort(Math.sum.aggregate.account::aggregateAccount);
		
		// Class<Generic>::MethodName
		Predicate<Project<String>> predicates = com.gss.adm.antlr.java.Project<String>::isEmpty;
		Predicate<Project<String>> predicates2 = Project<String>::isEmpty;
		Predicate<RelationShip<List<String>>> predicates3 = RelationShip<List<String>>::isCollections;
		
	}
}
