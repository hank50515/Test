package com.gss.adm.antlr.java.listener;

public class Car implements FourWheeler {
	public void print() {
		FourWheeler.super.print();
		System.out.println("I am a car!");
	}
}
