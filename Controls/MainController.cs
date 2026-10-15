using System.Windows;
using System.Windows.Threading;
using ExcavatorAngle.App.CalibrationSetting;
using ExcavatorAngle.App.Facades;
using ExcavatorAngle.App.Models;
using ExcavatorAngle.App.States;

namespace ExcavatorAngle.App.Controls;

/// <summary>
/// CONTROLLER: nhận thao tác từ view (bấm nút), gọi facade, cập nhật AppState.
/// Đây cũng là nơi CHUYỂN TỪ THREAD NỀN SANG UI THREAD (Dispatcher).
/// </summary>
public class MainController : IDisposable
{
    private readonly ExcavatorFacade _facade;
    private readonly AppState _state;
    private readonly CalibrationSettings _calibration;
    private readonly CalibrationRepository _calibrationRepo;
    private readonly Dispatcher _ui;
    private readonly DispatcherTimer _watchdog;

    public MainController(ExcavatorFacade facade, AppState state,
                          CalibrationSettings calibration, CalibrationRepository calibrationRepo)
    {
        _facade = facade;
        _state = state;
        _calibration = calibration;
        _calibrationRepo = calibrationRepo;
        _ui = Application.Current.Dispatcher;

        _facade.AnglesUpdated += OnAnglesUpdated;
        _facade.ErrorOccurred += msg => _ui.InvokeAsync(() => _state.Message = msg);

        // Mất dữ liệu > 1 giây → chuyển trạng thái về "Đang chờ"
        _watchdog = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        _watchdog.Tick += (_, _) =>
        {
            if (_state.Connection == ConnectionStatus.Receiving &&
                _state.LastReceived is { } t && DateTime.Now - t > TimeSpan.FromSeconds(1))
            {
                _state.Connection = ConnectionStatus.Waiting;
            }
        };
    }

    public CalibrationSettings Calibration => _calibration;

    // ---------- Các thao tác từ giao diện ----------

    public void StartReceiving()
    {
        try
        {
            _facade.Start();
            _state.Connection = ConnectionStatus.Waiting;
            _state.Message = $"Đang lắng nghe UDP port {_facade.Port}";
            _watchdog.Start();
        }
        catch (Exception ex)
        {
            _state.Message = $"Không mở được port {_facade.Port}: {ex.Message}";
        }
    }

    public void StopReceiving()
    {
        _facade.Stop();
        _watchdog.Stop();
        _state.Connection = ConnectionStatus.Stopped;
        _state.Message = "Đã dừng nhận dữ liệu";
    }

    public void NavigateTo(AppPage page) => _state.CurrentPage = page;

    /// <summary>Lấy góc hiện tại làm góc 0 cho tất cả các khớp.</summary>
    public void SetCurrentAsZero()
    {
        if (_facade.LastRawAngles is not { } raw)
        {
            _state.Message = "Chưa có dữ liệu để hiệu chỉnh";
            return;
        }
        _calibration.BoomOffset = raw.Boom;
        _calibration.ArmOffset = raw.Arm;
        _calibration.BucketOffset = raw.Bucket;
        _calibration.SwingOffset = raw.Swing;
        _calibration.PitchOffset = raw.Pitch;
        _calibration.RollOffset = raw.Roll;
        SaveCalibration();
    }

    public void ResetCalibration()
    {
        _calibration.BoomOffset = _calibration.ArmOffset = _calibration.BucketOffset = 0;
        _calibration.SwingOffset = _calibration.PitchOffset = _calibration.RollOffset = 0;
        SaveCalibration();
    }

    public void SaveCalibration()
    {
        _calibrationRepo.Save(_calibration);
        _state.Message = "Đã lưu hiệu chỉnh";
    }

    // ---------- Dữ liệu đến từ facade (THREAD NỀN) ----------

    private void OnAnglesUpdated(ExcavatorAngles a)
    {
        // Không được sửa giao diện / state đang binding từ thread nền → chuyển về UI thread
        _ui.InvokeAsync(() =>
        {
            _state.Boom = a.Boom;
            _state.Arm = a.Arm;
            _state.Bucket = a.Bucket;
            _state.Swing = a.Swing;
            _state.Pitch = a.Pitch;
            _state.Roll = a.Roll;
            _state.LastReceived = a.ReceivedAt;
            _state.PacketCount++;
            if (_state.Connection != ConnectionStatus.Stopped)
                _state.Connection = ConnectionStatus.Receiving;
        });
    }

    public void Dispose()
    {
        _watchdog.Stop();
        _facade.AnglesUpdated -= OnAnglesUpdated;
        _facade.Dispose();
    }
}
