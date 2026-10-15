using System.Net.Sockets;
using System.Text;

namespace ExcavatorAngle.App.Services;

/// <summary>
/// NHẬN DỮ LIỆU TỪ GMU qua UDP.
/// Chạy trên thread nền, mỗi gói nhận được sẽ bắn event JsonReceived.
/// Service này không biết gì về JSON hay giao diện, chỉ lo mạng.
/// </summary>
public class UdpReceiverService : IDisposable
{
    private readonly int _port;
    private UdpClient? _client;
    private CancellationTokenSource? _cts;

    /// <summary>Có gói tin mới (chuỗi JSON thô).</summary>
    public event Action<string>? JsonReceived;

    /// <summary>Lỗi mạng.</summary>
    public event Action<Exception>? Error;

    public UdpReceiverService(int port) => _port = port;

    public int Port => _port;
    public bool IsRunning => _client != null;

    public void Start()
    {
        if (IsRunning) return;

        _client = new UdpClient(_port);           // lắng nghe trên mọi IP của máy, port _port
        _cts = new CancellationTokenSource();
        _ = Task.Run(() => ReceiveLoopAsync(_cts.Token));  // chạy vòng lặp nhận ở thread nền
    }

    public void Stop()
    {
        _cts?.Cancel();
        _client?.Dispose();
        _client = null;
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && _client != null)
        {
            try
            {
                var result = await _client.ReceiveAsync(token);   // chờ gói tin
                var json = Encoding.UTF8.GetString(result.Buffer);
                JsonReceived?.Invoke(json);
            }
            catch (OperationCanceledException) { break; }
            catch (ObjectDisposedException) { break; }
            catch (Exception ex) { Error?.Invoke(ex); }
        }
    }

    public void Dispose() => Stop();
}
