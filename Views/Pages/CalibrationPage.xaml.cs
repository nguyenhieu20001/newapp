using System.Windows;
using System.Windows.Controls;
using ExcavatorAngle.App.Controls;

namespace ExcavatorAngle.App.Views.Pages;

/// <summary>Trang hiệu chỉnh. Nút bấm → gọi controller.</summary>
public partial class CalibrationPage : UserControl
{
    private readonly MainController _controller;

    public CalibrationPage(MainController controller)
    {
        InitializeComponent();
        _controller = controller;
        OffsetGrid.DataContext = controller.Calibration;
    }

    private void SetZero_Click(object sender, RoutedEventArgs e)
    {
        _controller.SetCurrentAsZero();
        RefreshOffsets();
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _controller.ResetCalibration();
        RefreshOffsets();
    }

    private void Save_Click(object sender, RoutedEventArgs e) => _controller.SaveCalibration();

    // CalibrationSettings là class đơn giản (không có INotifyPropertyChanged)
    // nên khi đổi từ code phải gán lại DataContext để ô nhập cập nhật.
    private void RefreshOffsets()
    {
        OffsetGrid.DataContext = null;
        OffsetGrid.DataContext = _controller.Calibration;
    }
}
