using System.Collections.Immutable;

namespace Fluxor.Extensions
{
    public class EntityAdapter<TKey, TEntity>
        where TKey : notnull
        where TEntity : class
    {
        #region Ctors
        
        public EntityAdapter(Func<TEntity, TKey> selectId)
        {
            SelectId = selectId;
        }

        #endregion

        #region Public

        public Func<TEntity, TKey> SelectId { get; }

        #endregion

        #region Collection methods

        /// <summary>
        /// Add one entity to the collection.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        public TState Add<TState>(TState state, TEntity entity)
            where TState : EntityState<TKey, TEntity>
        {
            var entityKey = SelectId(entity);

            return state.Entities.ContainsKey(entityKey)
                ? state
                : state with
                {
                    Entities = state.Entities.Add(entityKey, entity),
                };
        }

        /// <summary>
        /// Add multiple entities to the collection.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        public TState AddRange<TState>(TState state, IEnumerable<TEntity> entities)
             where TState : EntityState<TKey, TEntity>
        {
            var notAddedEntities = new Dictionary<TKey, TEntity>();

            foreach (var entity in entities)
            {
                var entityKey = SelectId(entity);
                if (state.Entities.ContainsKey(entityKey))
                    continue;

                notAddedEntities.Add(entityKey, entity);
            }

            return state with
            {
                Entities = state.Entities.AddRange(notAddedEntities),
            };
        }

        /// <summary>
        /// Replace current collection with provided collection.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        public TState SetAll<TState>(TState state, IEnumerable<TEntity> entities)
            where TState : EntityState<TKey, TEntity>
            => state with
            {
                Entities = entities.ToImmutableDictionary(entity => SelectId(entity), entity => entity),
            };

        /// <summary>
        /// Add or Replace one entity in the collection.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        public TState SetOne<TState>(TState state, TEntity entity)
            where TState : EntityState<TKey, TEntity>
            => state with
            {
                Entities = state.Entities.SetItem(SelectId(entity), entity),
            };

        /// <summary>
        /// Add or Replace multiple entities in the collection.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        public TState SetMany<TState>(TState state, IEnumerable<TEntity> entities)
            where TState : EntityState<TKey, TEntity>
            => state with
            {
                Entities = state.Entities
                    .SetItems(entities.ToImmutableDictionary(entity => SelectId(entity), entity => entity)),
            };


        /// <summary>
        /// Remove one entity from the collection.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public TState Remove<TState>(TState state, TKey id)
            where TState : EntityState<TKey, TEntity>
            => state with
            {
                Entities = state.Entities.Remove(id),
            };

        /// <summary>
        /// Remove multiple entities from the collection, by ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="state"></param>
        public TState RemoveRange<TState>(TState state, IEnumerable<TKey> ids)
            where TState : EntityState<TKey, TEntity>
            => state with
            {
                Entities = state.Entities.RemoveRange(ids),
            };

        /// <summary>
        /// Remove multiple entities from the collection, by predicate.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="state"></param>
        public TState RemoveRange<TState>(TState state, Predicate<TEntity> predicate)
             where TState : EntityState<TKey, TEntity>
             => state with
             {
                 Entities = state.Entities
                    .RemoveRange(state.Entities
                        .Where(x => predicate(x.Value))
                        .Select(x => x.Key)),
             };

        /// <summary>
        /// Clear entity collection.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="state"></param>
        public TState RemoveAll<TState>(TState state)
             where TState : EntityState<TKey, TEntity>
            => state with
            {
                Entities = state.Entities.Clear(),
            };



        /// <summary>
        /// Update one entity in the collection.
        /// </summary>
        /// <param name="updatedEntity"></param>
        /// <param name="state"></param>
        public TState Update<TState>(TState state, TEntity updatedEntity)
             where TState : EntityState<TKey, TEntity>
        {
            var entityKey = SelectId(updatedEntity);

            return !state.Entities.ContainsKey(entityKey)
                ? state
                : state with
                {
                    Entities = state.Entities.SetItem(entityKey, updatedEntity),
                };
        }


        /// <summary>
        /// Update multiple entities in the collection. Supports partial updates.
        /// </summary>
        /// <param name="updatedEntities"></param>
        /// <param name="state"></param>
        public TState UpdateRange<TState>(TState state, IEnumerable<TEntity> updatedEntities)
             where TState : EntityState<TKey, TEntity>
        {
            var targetEntities = new Dictionary<TKey, TEntity>();

            foreach (var entity in updatedEntities)
            {
                var entityKey = SelectId(entity);
                if (!state.Entities.ContainsKey(entityKey))
                    continue;

                targetEntities.Add(entityKey, entity);
            }

            return state with
            {
                Entities = state.Entities.SetItems(targetEntities),
            };
        }

        /// <summary>
        /// Add or Update one entity in the collection.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        public TState Upsert<TState>(TState state, TEntity entity)
             where TState : EntityState<TKey, TEntity>
             => state with
             {
                 Entities = state.Entities.SetItem(SelectId(entity), entity),
             };

        /// <summary>
        /// Add or Update multiple entities in the collection.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        public TState UpsertRange<TState>(TState state, IEnumerable<TEntity> entities)
            where TState : EntityState<TKey, TEntity>
            => state with
            {
                Entities = entities.ToImmutableDictionary(entity => SelectId(entity), entity => entity),
            };

        public TState Map<TState>(TState state, TKey id, Func<TEntity, TEntity> updateFunc)
             where TState : EntityState<TKey, TEntity>
             => state with
             {
                 Entities = state.Entities.SetItem(id, updateFunc(state.Entities[id])),
             };

        public TState MapRange<TState>(TState state, IEnumerable<TKey> ids, Func<TEntity, TEntity> updateFunc)
             where TState : EntityState<TKey, TEntity>
             => state with
             {
                 Entities = state.Entities
                    .SetItems(state.Entities
                        .Where(x => ids.Contains(x.Key))
                        .ToImmutableDictionary(x => x.Key, x => updateFunc(x.Value))),
             };

        #endregion
    }
}
