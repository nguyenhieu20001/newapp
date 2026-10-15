using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExcavatorAngle.App.States;

/// <summary>
/// Lớp cơ sở cho data binding của WPF.
/// Khi gọi Set(...), WPF được báo "thuộc tính đã đổi" và tự cập nhật giao diện.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        return true;
    }

    protected void Raise([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
