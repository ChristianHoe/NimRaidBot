namespace EventBot.DataAccess.Queries
{
    public interface IQuery<T, R>
    {
        R Execute(T t);
    }
}
