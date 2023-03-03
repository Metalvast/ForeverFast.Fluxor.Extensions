using System.Reflection;

namespace Fluxor.Extensions
{
    public static class Adapters
    {
        internal static Dictionary<Type, object> s_cachedAdapters { get; set; } = new();

        public static EntityAdapter<TKey, TEntity> Get<TKey, TEntity>()
            where TEntity : class
            where TKey : notnull
        {
            if (!s_cachedAdapters.TryGetValue(typeof(TEntity), out var adapter))
                throw new Exception();

            return (EntityAdapter<TKey, TEntity>)adapter;
        }

        public static void Scan(params Assembly[] assemblies)
        {
            var types = assemblies
               .SelectMany(x => x.GetTypes())
               .Where(x
                   => !x.IsAbstract
                   && (x.BaseType?.IsGenericType ?? false)
                   && x.BaseType?.GetGenericTypeDefinition() == (typeof(EntityAdapter<,>)))
               .ToList();

            types.ForEach(x =>
            {
                var entityType = x.BaseType!.GenericTypeArguments[1];
                var adapterInstance = Activator.CreateInstance(x);

                if (adapterInstance == null)
                    throw new($"Can't create instance of {x.Name}");

                s_cachedAdapters.TryAdd(entityType, adapterInstance);
            });
        }
    }
}
