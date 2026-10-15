using ExcavatorAngle.App.CalibrationSetting;
using ExcavatorAngle.App.Models;

namespace ExcavatorAngle.App.Services;

/// <summary>
/// TÍNH GÓC: từ CAN frame → giá trị độ, rồi áp dụng hiệu chỉnh (offset).
/// Đây là phần quan trọng nhất về độ chính xác, nên viết unit test cho nó.
/// </summary>
public class AngleCalculationService
{
    private readonly CalibrationSettings _calibration;

    // Giữ giá trị mới nhất của từng tín hiệu (vì 2 frame đến riêng rẽ)
    private double _boom, _arm, _bucket, _swing, _pitch, _roll;

    // Giá trị chưa hiệu chỉnh mới nhất (dùng cho nút "Set 0")
    public ExcavatorAngles? LastRaw { get; private set; }

    public AngleCalculationService(CalibrationSettings calibration) => _calibration = calibration;

    public ExcavatorAngles Update(IEnumerable<CanFrame> frames)
    {
        foreach (var frame in frames)
        {
            switch (frame.Id)
            {
                case CanIds.JointAngles:
                    _boom   = Decode(frame.Data, 0);
                    _arm    = Decode(frame.Data, 2);
                    _bucket = Decode(frame.Data, 4);
                    _swing  = Decode(frame.Data, 6);
                    break;
                case CanIds.BodyTilt:
                    _pitch = Decode(frame.Data, 0);
                    _roll  = Decode(frame.Data, 2);
                    break;
                // ID khác: bỏ qua
            }
        }

        var now = DateTime.Now;
        LastRaw = new ExcavatorAngles(_boom, _arm, _bucket, _swing, _pitch, _roll, now);

        var c = _calibration;
        return new ExcavatorAngles(
            Boom:   _boom   - c.BoomOffset,
            Arm:    _arm    - c.ArmOffset,
            Bucket: _bucket - c.BucketOffset,
            Swing:  NormalizeSwing(_swing - c.SwingOffset),
            Pitch:  _pitch  - c.PitchOffset,
            Roll:   _roll   - c.RollOffset,
            ReceivedAt: now);
    }

    /// <summary>uint16 little-endian → độ (công thức kiểu J1939).</summary>
    public static double Decode(byte[] data, int index)
    {
        ushort raw = (ushort)(data[index] | (data[index + 1] << 8));
        return raw * CanIds.Resolution + CanIds.Offset;
    }

    private static double NormalizeSwing(double deg)
    {
        deg %= 360;
        if (deg > 180) deg -= 360;
        if (deg < -180) deg += 360;
        return deg;
    }
}
