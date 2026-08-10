using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public abstract class ModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private Dictionary<string, PropertyChangedEventArgs> _eventArgsCache = new();

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (!_eventArgsCache.TryGetValue(propertyName, out var eventArgs))
        {
            eventArgs = new PropertyChangedEventArgs(propertyName);
            _eventArgsCache[propertyName] = eventArgs;
        }

        PropertyChanged?.Invoke(this, eventArgs);
    }

    public abstract void InitializeOnce();
}
