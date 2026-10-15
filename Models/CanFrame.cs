namespace ExcavatorAngle.App.Models;

/// <summary>CAN frame đã chuyển từ chuỗi hex sang số, dễ xử lý.</summary>
public record CanFrame(uint Id, byte[] Data);
