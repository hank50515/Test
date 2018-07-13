public interface GenericClassInterface<T, U>
        where U : GenericClass2
{
    IEnumerable<T> List()where T : GenericClass1;
	
    IEnumerable<GenericClass1> Get(U id);
	
	void post<GenericClass2>();
	
	void genericParm(IList<GenericClass2> gc2);
}