namespace Fluxor.Extensions
{
    public static class IStoreExtensions
    {
        public static ISelectorSubscription<TResult> SubscribeSelector<TResult>(
            this IStore store,
            ISelector<TResult> selector)
            => new SelectorSubscription<TResult>(store, selector);

        public static ISelectorSubscription<TResult> SubscribeSelector<TResult>(
            this IStore store,
            ISelector<TResult> selector,
            Action<TResult> handler)
            => new SelectorSubscription<TResult>(store, selector, handler);

        public static TResult Select<TResult>(this IStore store, ISelector<TResult> selector)
            => selector.Select(store);
    }
}