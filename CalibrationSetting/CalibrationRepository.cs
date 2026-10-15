using System.IO;
using System.Text.Json;

namespace ExcavatorAngle.App.CalibrationSetting;

/// <summary>Đọc / ghi file calibration.json để giữ hiệu chỉnh sau khi tắt app.</summary>
public class CalibrationRepository
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private readonly string _path;

    public CalibrationRepository(string relativePath)
        => _path = Path.Combine(AppContext.BaseDirectory, relativePath);

    public CalibrationSettings Load()
    {
        if (!File.Exists(_path)) return new CalibrationSettings();
        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<CalibrationSettings>(json) ?? new CalibrationSettings();
    }

    public void Save(CalibrationSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(settings, Options));
    }
}
