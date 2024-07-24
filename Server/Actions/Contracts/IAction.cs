namespace Server.Actions.Contracts;

public interface IAction<T, U>
{
    Task<U> PerformAsync(T actionParams);
}
