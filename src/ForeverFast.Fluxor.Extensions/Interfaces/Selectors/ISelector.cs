namespace Fluxor.Extensions
{
    public interface ISelector<TResult>
    {
        TResult Select(IStore state);
    }
}