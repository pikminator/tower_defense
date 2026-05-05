using System.Text.Json;

namespace tower_defense;

public static class SaveManager
{
    private static readonly string SavePath = Path.Combine(AppContext.BaseDirectory, "save.json");

    public static void Save()
    {
        var data = new SaveData
        {
            Gems = GlobalProgress.Gems,
            DamageUpgradeLevel = GlobalProgress.DamageUpgradeLevel,
            GeneratorUpgradeLevel = GlobalProgress.GeneratorUpgradeLevel,
            BaseHPUpgradeLevel = GlobalProgress.BaseHPUpgradeLevel,
            Records = HighScores.Records
        };

        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(SavePath, json);
    }

    public static void Load()
    {
        if (!File.Exists(SavePath)) return;

        try
        {
            var json = File.ReadAllText(SavePath);
            var data = JsonSerializer.Deserialize<SaveData>(json);
            if (data == null) return;

            GlobalProgress.Gems = data.Gems;
            GlobalProgress.DamageUpgradeLevel = data.DamageUpgradeLevel;
            GlobalProgress.GeneratorUpgradeLevel = data.GeneratorUpgradeLevel;
            GlobalProgress.BaseHPUpgradeLevel = data.BaseHPUpgradeLevel;
            HighScores.Records = data.Records ?? new List<Record>();
        }
        catch
        {
            // Файл сохранения повреждён, пропускаем
        }
    }
}

public class SaveData
{
    public int Gems { get; set; }
    public int DamageUpgradeLevel { get; set; }
    public int GeneratorUpgradeLevel { get; set; }
    public int BaseHPUpgradeLevel { get; set; }
    public List<Record>? Records { get; set; }
}
