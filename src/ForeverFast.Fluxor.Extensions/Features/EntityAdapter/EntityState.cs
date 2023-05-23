using System.Collections.Immutable;

namespace Fluxor.Extensions
{
    public abstract record EntityState<TKey, TEntity>
        where TKey : notnull
        where TEntity : class
    {
        public IImmutableDictionary<TKey, TEntity> Entities { get; internal set; } = ImmutableDictionary<TKey, TEntity>.Empty;
    }
}
