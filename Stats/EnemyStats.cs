using tower_defense.Models;

namespace tower_defense.Stats;

// Статы врагов по типу и волне
public static class EnemyStats
{
    public static double GetBaseSpeed(EnemyType type) => type switch
    {
        EnemyType.Normal => 2.0,
        EnemyType.Fast => 4.0,
        EnemyType.Armored => 1.2,
        EnemyType.Boss => 1.8,
        _ => 2.0
    };

    public static int GetReward(EnemyType type) => type switch
    {
        EnemyType.Normal => 10,
        EnemyType.Fast => 15,
        EnemyType.Armored => 40,
        EnemyType.Boss => 500,
        _ => 10
    };

    // Три диапазона волн (1-10, 11-25, 26+) с разными значениями HP
    public static double GetBaseHealth(EnemyType type, int wave)
    {
        if (wave <= 10)
            return type switch { EnemyType.Normal => 50, EnemyType.Fast => 30, EnemyType.Armored => 150, EnemyType.Boss => 900, _ => 50 };
        if (wave <= 25)
            return type switch { EnemyType.Normal => 80, EnemyType.Fast => 50, EnemyType.Armored => 250, EnemyType.Boss => 1600, _ => 80 };
        return type switch { EnemyType.Normal => 120, EnemyType.Fast => 75, EnemyType.Armored => 400, EnemyType.Boss => 2800, _ => 120 };
    }
}
