namespace ExcavatorAngle.App.States;

public enum AppPage
{
    Monitor,      // màn hình hiển thị góc
    Calibration   // màn hình hiệu chỉnh
}

public enum ConnectionStatus
{
    Stopped,      // chưa bấm Start
    Waiting,      // đã mở port, chưa có dữ liệu / mất dữ liệu
    Receiving     // đang nhận dữ liệu đều
}

/// <summary>
/// TRẠNG THÁI DÙNG CHUNG của app: trang hiện tại, trạng thái kết nối, các góc đang hiển thị...
/// Mọi view đều binding vào đây → luôn hiển thị nhất quán.
/// </summary>
public class AppState : ObservableObject
{
    private AppPage _currentPage = AppPage.Monitor;
    public AppPage CurrentPage
    {
        get => _currentPage;
        set => Set(ref _currentPage, value);
    }

    private ConnectionStatus _connection = ConnectionStatus.Stopped;
    public ConnectionStatus Connection
    {
        get => _connection;
        set
        {
            if (Set(ref _connection, value))
            {
                Raise(nameof(IsRunning));
                Raise(nameof(ConnectionText));
            }
        }
    }

    public bool IsRunning => Connection != ConnectionStatus.Stopped;

    public string ConnectionText => Connection switch
    {
        ConnectionStatus.Stopped   => "Đã dừng",
        ConnectionStatus.Waiting   => "Đang chờ dữ liệu GMU...",
        ConnectionStatus.Receiving => "Đang nhận dữ liệu",
        _ => ""
    };

    // ---- Các góc hiển thị ----
    private double _boom, _arm, _bucket, _swing, _pitch, _roll;
    public double Boom   { get => _boom;   set => Set(ref _boom, value); }
    public double Arm    { get => _arm;    set => Set(ref _arm, value); }
    public double Bucket { get => _bucket; set => Set(ref _bucket, value); }
    public double Swing  { get => _swing;  set => Set(ref _swing, value); }
    public double Pitch  { get => _pitch;  set => Set(ref _pitch, value); }
    public double Roll   { get => _roll;   set => Set(ref _roll, value); }

    private DateTime? _lastReceived;
    public DateTime? LastReceived { get => _lastReceived; set => Set(ref _lastReceived, value); }

    private int _packetCount;
    public int PacketCount { get => _packetCount; set => Set(ref _packetCount, value); }

    private string _message = "";
    public string Message { get => _message; set => Set(ref _message, value); }
}
