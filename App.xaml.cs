using System.Windows;
using ExcavatorAngle.App.CalibrationSetting;
using ExcavatorAngle.App.Controls;
using ExcavatorAngle.App.Facades;
using ExcavatorAngle.App.Services;
using ExcavatorAngle.App.States;
using ExcavatorAngle.App.Views;

namespace ExcavatorAngle.App;

/// <summary>
/// ĐIỂM KHỞI ĐỘNG của app WPF.
/// Ở đây "lắp ráp" toàn bộ các tầng: services → facade → controller → view.
/// </summary>
public partial class App : Application
{
    private MainController? _controller;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Cấu hình hiệu chỉnh (đọc từ file json)
        var calibrationRepo = new CalibrationRepository("CalibrationSetting/calibration.json");
        var calibration = calibrationRepo.Load();

        // 2. Các service (không biết gì về giao diện)
        var udp = new UdpReceiverService(port: 50000);
        var parser = new GmuJsonParserService();
        var calculator = new AngleCalculationService(calibration);

        // 3. Facade gom các service lại
        var facade = new ExcavatorFacade(udp, parser, calculator);

        // 4. State dùng chung cho toàn app
        var state = new AppState();

        // 5. Controller điều phối giữa view ↔ facade ↔ state
        _controller = new MainController(facade, state, calibration, calibrationRepo);

        // 6. Mở cửa sổ chính
        var window = new MainWindow(_controller, state);
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _controller?.Dispose();
        base.OnExit(e);
    }
}
