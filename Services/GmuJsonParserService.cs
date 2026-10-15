using System.Globalization;
using System.Text.Json;
using ExcavatorAngle.App.Models;

namespace ExcavatorAngle.App.Services;

/// <summary>
/// Chuyển chuỗi JSON từ GMU → danh sách CanFrame.
/// </summary>
public class GmuJsonParserService
{
    public IReadOnlyList<CanFrame> Parse(string json)
    {
        var packet = JsonSerializer.Deserialize<GmuPacket>(json)
                     ?? throw new FormatException("JSON rỗng");

        var frames = new List<CanFrame>();
        foreach (var f in packet.Frames)
        {
            uint id = uint.Parse(f.Id, NumberStyles.HexNumber);
            byte[] data = f.Data
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(b => byte.Parse(b, NumberStyles.HexNumber))
                .ToArray();
            frames.Add(new CanFrame(id, data));
        }
        return frames;
    }
}
