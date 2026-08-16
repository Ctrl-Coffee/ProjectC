using System;

public interface ContainerPropertyChanged<T>
{
    event Action<string, ContainerPropertyChangedEvent, T> ContainerPropertyChanged;
}
