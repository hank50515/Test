class GenericClass<U> : MyGenericClass<U> where U: GenericClass1
{ 
    public static int AddNumber<T>(int val1, int val2) where T: GenericClass2{
        return val1 + val2;
    }
	
	public static int AddNumber2<GenericClass1>(int val1, int val2){
        return val1 + val2;
    }
	
	public static IList<GenericClass2> AddList(GenericClass2 gc2){
		List<GenericClass2> myIntLists = new List<GenericClass2>();
		myIntLists.add(gc2);
		
		return myIntLists;
	}
	
	public static void genericParm(IList<GenericClass2> gc2){
		
	}
	
	
	private static async Task<GenericClass1> ShowTodaysInfo(){
		
	}
}