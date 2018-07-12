public interface D<T, U>
        where T : B
        where U : C
{
    IEnumerable<T> List();
    T Get(U id);
}