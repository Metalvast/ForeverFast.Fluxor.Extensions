using System.ComponentModel;

namespace Fluxor.Extensions
{
    public class SelectorSubscription<T> : ISelectorSubscription<T>
    {
        #region Fields

        private readonly IStore _store;
        private readonly ISelector<T> _selector;
        private readonly Action<T>? _valueChangedHandler;
        private readonly List<IFeature> _features;
        private bool _isPaused = false;
        private T _lastValue;

        #endregion

        #region Ctors

        public SelectorSubscription(IStore store, ISelector<T> selector)
            : this(store, selector, null)
        {
        }

        public SelectorSubscription(IStore store, ISelector<T> selector, Action<T>? valueChangedHandler)
        {
            _store = store;
            _selector = selector;
            _valueChangedHandler = valueChangedHandler;

            // IStore has no event, so subscribe to all feature changes.
            // Hold a list of all features we subscribed to.
            _features = store.Features.Select(kvp => kvp.Value).ToList();
            _features.ForEach(f => f.StateChanged += OnFeatureStateChanged);

            // create a wrapper selector to save the last value. the projector function will be called only if the value changed.
            _selector = SelectorFactory.CreateSelector(selector, newVal =>
            {
                OnValueChanged(newVal);
                return newVal;
            });

            _lastValue = _selector.Select(_store);
        }

        #endregion

        #region Props

        public T Value => _lastValue;

        public event EventHandler? StateChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region External events

        private void OnFeatureStateChanged(object? _, EventArgs e)
            => OnStateChanged();

        #endregion

        #region Public methods

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
            OnStateChanged();
        }

        public void Dispose()
        {
            _features.ForEach(f => f.StateChanged -= OnFeatureStateChanged);
        }

        #endregion

        #region Private methods

        private void OnStateChanged()
        {
            if (_isPaused)
            {
                return;
            }

            _ = _selector.Select(_store);
        }

        private void OnValueChanged(T newVal)
        {
            _lastValue = newVal;
            StateChanged?.Invoke(this, EventArgs.Empty);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            _valueChangedHandler?.Invoke(newVal);
        }

        #endregion
    }
}