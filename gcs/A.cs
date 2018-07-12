class A<U> : MyGenericClass<U> where U: B
{ 
    public static int AddNumber<T>(int val1, int val2) where T: C{
        return val1 + val2;
    };
}