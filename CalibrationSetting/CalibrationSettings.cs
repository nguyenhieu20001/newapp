namespace ExcavatorAngle.App.CalibrationSetting;

/// <summary>
/// Giá trị hiệu chỉnh: góc hiển thị = góc đo được - offset.
/// "Set 0" nghĩa là lấy góc hiện tại làm offset.
/// </summary>
public class CalibrationSettings
{
    public double BoomOffset { get; set; }
    public double ArmOffset { get; set; }
    public double BucketOffset { get; set; }
    public double SwingOffset { get; set; }
    public double PitchOffset { get; set; }
    public double RollOffset { get; set; }
}
