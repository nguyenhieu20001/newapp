namespace ExcavatorAngle.App.Models;

/// <summary>
/// Kết quả cuối cùng: các góc của máy xúc (đơn vị độ).
/// </summary>
public record ExcavatorAngles(
    double Boom,     // góc cần (boom) so với phương ngang
    double Arm,      // góc tay gầu (arm) so với boom
    double Bucket,   // góc gầu (bucket) so với arm
    double Swing,    // góc quay thân trên
    double Pitch,    // độ nghiêng thân máy trước/sau
    double Roll,     // độ nghiêng thân máy trái/phải
    DateTime ReceivedAt);
