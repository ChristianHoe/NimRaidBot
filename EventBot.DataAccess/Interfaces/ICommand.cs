namespace EventBot.DataAccess.Commands
{
    public interface ICommand<T>
    {
        void Execute(T t);
    }
}
