using System.ComponentModel;

namespace Fluxor.Extensions
{
    public interface ISelectorSubscription<T> : IState<T>, INotifyPropertyChanged, IDisposable
    {
        void Pause();

        void Resume();
    }
}
