using System.Collections.Immutable;

namespace Fluxor.Extensions
{
    public abstract class EntityAdapter<TKey, TEntity>
        where TKey : notnull
        where TEntity : class
    {
        public abstract Func<TEntity, TKey> SelectId { get; }

        public abstract EntityState<TKey, TEntity> GetInitialState();

        #region Collection methods

        /// <summary>
        /// Add one entity to the collection.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        public TState Add<TState>(TEntity entity, EntityState<TKey, TEntity> state)
            where TState : EntityState<TKey, TEntity>
        {
            var entityKey = SelectId(entity);

            return state.Entities.ContainsKey(entityKey)
                ? (TState)state
                : (TState)state with
                {
                    Entities = state.Entities.Add(entityKey, entity),
                };
        }

        /// <summary>
        /// Add multiple entities to the collection.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        public TState AddRange<TState>(IEnumerable<TEntity> entities, EntityState<TKey, TEntity> state)
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

            return (TState)state with
            {
                Entities = state.Entities.AddRange(notAddedEntities),
            };
        }

        /// <summary>
        /// Replace current collection with provided collection.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        public TState SetAll<TState>(IEnumerable<TEntity> entities, EntityState<TKey, TEntity> state)
            where TState : EntityState<TKey, TEntity>
            => (TState)state with
            {
                Entities = entities.ToImmutableDictionary(entity => SelectId(entity), entity => entity),
            };

        /// <summary>
        /// Add or Replace one entity in the collection.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        public TState SetOne<TState>(TEntity entity, EntityState<TKey, TEntity> state)
            where TState : EntityState<TKey, TEntity>
            => (TState)state with
            {
                Entities = state.Entities.SetItem(SelectId(entity), entity),
            };

        /// <summary>
        /// Add or Replace multiple entities in the collection.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        public TState SetMany<TState>(IEnumerable<TEntity> entities, EntityState<TKey, TEntity> state)
            where TState : EntityState<TKey, TEntity>
            => (TState)state with
            {
                Entities = state.Entities.SetItems(entities.ToImmutableDictionary(entity => SelectId(entity), entity => entity)),
            };


        /// <summary>
        /// Remove one entity from the collection.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public TState Remove<TState>(TKey id, EntityState<TKey, TEntity> state)
            where TState : EntityState<TKey, TEntity>
            => (TState)state with
            {
                Entities = state.Entities.Remove(id),
            };

        /// <summary>
        /// Remove multiple entities from the collection, by ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="state"></param>
        public TState RemoveRange<TState>(IEnumerable<TKey> ids, EntityState<TKey, TEntity> state)
            where TState : EntityState<TKey, TEntity>
            => (TState)state with
            {
                Entities = state.Entities.RemoveRange(ids),
            };

        /// <summary>
        /// Remove multiple entities from the collection, by predicate.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="state"></param>
        public TState RemoveRange<TState>(Predicate<TEntity> predicate, EntityState<TKey, TEntity> state)
             where TState : EntityState<TKey, TEntity>
             => (TState)state with
             {
                 Entities = state.Entities.RemoveRange(state.Entities.Where(x => predicate(x.Value)).Select(x => x.Key)),
             };

        /// <summary>
        /// Clear entity collection.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="state"></param>
        public TState RemoveAll<TState>(EntityState<TKey, TEntity> state)
             where TState : EntityState<TKey, TEntity>
            => (TState)state with
            {
                Entities = state.Entities.Clear(),
            };



        /// <summary>
        /// Update one entity in the collection.
        /// </summary>
        /// <param name="updatedEntity"></param>
        /// <param name="state"></param>
        public TState Update<TState>(TEntity updatedEntity, EntityState<TKey, TEntity> state)
             where TState : EntityState<TKey, TEntity>
        {
            var entityKey = SelectId(updatedEntity);

            return !state.Entities.ContainsKey(entityKey)
                ? (TState)state
                : (TState)state with
                {
                    Entities = state.Entities.SetItem(entityKey, updatedEntity),
                };
        }


        /// <summary>
        /// Update multiple entities in the collection. Supports partial updates.
        /// </summary>
        /// <param name="updatedEntities"></param>
        /// <param name="state"></param>
        public TState UpdateRange<TState>(IEnumerable<TEntity> updatedEntities, EntityState<TKey, TEntity> state)
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

            return (TState)state with
            {
                Entities = state.Entities.SetItems(targetEntities),
            };
        }

        /// <summary>
        /// Add or Update one entity in the collection.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        public TState Upsert<TState>(TEntity entity, EntityState<TKey, TEntity> state)
             where TState : EntityState<TKey, TEntity>
             => (TState)state with
             {
                 Entities = state.Entities.SetItem(SelectId(entity), entity),
             };

        /// <summary>
        /// Add or Update multiple entities in the collection.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        public TState UpsertRange<TState>(IEnumerable<TEntity> entities, EntityState<TKey, TEntity> state)
            where TState : EntityState<TKey, TEntity>
            => (TState)state with
            {
                Entities = entities.ToImmutableDictionary(entity => SelectId(entity), entity => entity),
            };

        #endregion
    }
}
