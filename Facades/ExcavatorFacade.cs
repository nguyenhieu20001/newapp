using ExcavatorAngle.App.Models;
using ExcavatorAngle.App.Services;

namespace ExcavatorAngle.App.Facades;

/// <summary>
/// FACADE: che giấu chuỗi xử lý UDP → parse JSON → tính góc.
/// Bên ngoài (controller) chỉ cần Start/Stop và nghe event AnglesUpdated.
///
/// Lưu ý: các event bắn ra từ THREAD NỀN (thread nhận UDP),
/// nên bên nhận phải chuyển sang UI thread trước khi đụng tới giao diện.
/// </summary>
public class ExcavatorFacade : IDisposable
{
    private readonly UdpReceiverService _udp;
    private readonly GmuJsonParserService _parser;
    private readonly AngleCalculationService _calculator;

    public event Action<ExcavatorAngles>? AnglesUpdated;
    public event Action<string>? ErrorOccurred;

    public ExcavatorFacade(UdpReceiverService udp,
                           GmuJsonParserService parser,
                           AngleCalculationService calculator)
    {
        _udp = udp;
        _parser = parser;
        _calculator = calculator;

        _udp.JsonReceived += OnJsonReceived;
        _udp.Error += ex => ErrorOccurred?.Invoke(ex.Message);
    }

    public int Port => _udp.Port;
    public ExcavatorAngles? LastRawAngles => _calculator.LastRaw;

    public void Start() => _udp.Start();
    public void Stop() => _udp.Stop();

    private void OnJsonReceived(string json)
    {
        try
        {
            var frames = _parser.Parse(json);
            var angles = _calculator.Update(frames);
            AnglesUpdated?.Invoke(angles);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Lỗi dữ liệu: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _udp.JsonReceived -= OnJsonReceived;
        _udp.Dispose();
    }
}
