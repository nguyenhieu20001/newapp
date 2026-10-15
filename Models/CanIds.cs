namespace ExcavatorAngle.App.Models;

/// <summary>
/// "CAN map" giả định cho dự án mẫu (dạng J1939 29-bit, PGN 0xFF10 / 0xFF11).
///
/// 0x18FF1001: byte0-1 Boom, byte2-3 Arm, byte4-5 Bucket, byte6-7 Swing
/// 0x18FF1101: byte0-1 Pitch, byte2-3 Roll
///
/// Mỗi giá trị: uint16 little-endian, độ = raw * 0.0078125 - 250  (kiểu J1939)
/// </summary>
public static class CanIds
{
    public const uint JointAngles = 0x18FF1001;
    public const uint BodyTilt    = 0x18FF1101;

    public const double Resolution = 0.0078125; // 1/128 độ mỗi bit
    public const double Offset     = -250.0;
}
