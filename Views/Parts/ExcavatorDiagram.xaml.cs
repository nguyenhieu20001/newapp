using System.Windows;
using System.Windows.Controls;

namespace ExcavatorAngle.App.Views.Parts;

/// <summary>
/// CONTROL TỰ VẼ hình máy xúc.
/// Có các DependencyProperty (BoomAngle, ArmAngle...) để bên ngoài binding vào,
/// khi giá trị đổi thì cập nhật góc của RotateTransform tương ứng.
///
/// Quy ước: góc dương = hướng lên (ngược chiều kim đồng hồ),
/// còn WPF xoay dương = cùng chiều kim đồng hồ → phải đổi dấu.
/// </summary>
public partial class ExcavatorDiagram : UserControl
{
    public ExcavatorDiagram() => InitializeComponent();

    public static readonly DependencyProperty BoomAngleProperty =
        Register(nameof(BoomAngle), (d, v) => d.BoomRotate.Angle = -v);
    public static readonly DependencyProperty ArmAngleProperty =
        Register(nameof(ArmAngle), (d, v) => d.ArmRotate.Angle = -v);
    public static readonly DependencyProperty BucketAngleProperty =
        Register(nameof(BucketAngle), (d, v) => d.BucketRotate.Angle = -v);
    public static readonly DependencyProperty SwingAngleProperty =
        Register(nameof(SwingAngle), (d, v) => d.SwingRotate.Angle = v);
    public static readonly DependencyProperty PitchAngleProperty =
        Register(nameof(PitchAngle), (d, v) => d.BodyRotate.Angle = -v);

    public double BoomAngle   { get => (double)GetValue(BoomAngleProperty);   set => SetValue(BoomAngleProperty, value); }
    public double ArmAngle    { get => (double)GetValue(ArmAngleProperty);    set => SetValue(ArmAngleProperty, value); }
    public double BucketAngle { get => (double)GetValue(BucketAngleProperty); set => SetValue(BucketAngleProperty, value); }
    public double SwingAngle  { get => (double)GetValue(SwingAngleProperty);  set => SetValue(SwingAngleProperty, value); }
    public double PitchAngle  { get => (double)GetValue(PitchAngleProperty);  set => SetValue(PitchAngleProperty, value); }

    private static DependencyProperty Register(string name, Action<ExcavatorDiagram, double> apply) =>
        DependencyProperty.Register(name, typeof(double), typeof(ExcavatorDiagram),
            new PropertyMetadata(0.0, (d, e) => apply((ExcavatorDiagram)d, (double)e.NewValue)));
}
