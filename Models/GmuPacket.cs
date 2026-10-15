using System.Text.Json.Serialization;

namespace ExcavatorAngle.App.Models;

/// <summary>
/// Gói JSON mà GMU gửi qua UDP.
/// Ví dụ:
/// {
///   "timestamp": 1729000000123,
///   "frames": [
///     { "id": "18FF1001", "dlc": 8, "data": "A0 7D 10 6C 00 7D 00 7D" }
///   ]
/// }
/// </summary>
public class GmuPacket
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("frames")]
    public List<CanFrameJson> Frames { get; set; } = new();
}

/// <summary>Một CAN frame dạng JSON (id và data ở dạng chuỗi hex).</summary>
public class CanFrameJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("dlc")]
    public int Dlc { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; } = "";
}
