using tower_defense.Models;

namespace tower_defense.Stats;

// Статы башен по типу и уровню (1-3)

public static class TowerStats
{
    // Урон умножается на глобальный модификатор (5% за уровень улучшения)
    public static double GetDamage(TowerType type, int level)
    {
        double baseDmg = type switch
        {
            TowerType.Crossbow => level == 1 ? 40 : level == 2 ? 70 : 120,
            TowerType.Tesla => level == 1 ? 15 : level == 2 ? 25 : 50,
            TowerType.Cannon => level == 1 ? 60 : level == 2 ? 120 : 220,
            _ => 0
        };
        return baseDmg * (1.0 + GlobalProgress.DamageUpgradeLevel * 0.05);
    }

    public static double GetCooldown(TowerType type, int level) => type switch
    {
        TowerType.Crossbow => 1.0,
        TowerType.Tesla => 0.25,
        TowerType.Cannon => 2.5,
        TowerType.Slow => 1.5,
        _ => 1.0
    };

    public static int GetCost(TowerType type, int level)
    {
        return type switch
        {
            TowerType.Tesla => level == 1 ? 60 : level == 2 ? 100 : 180,
            TowerType.Crossbow => level == 1 ? 45 : level == 2 ? 80 : 140,
            TowerType.Cannon => level == 1 ? 90 : level == 2 ? 160 : 280,
            TowerType.Slow => level == 1 ? 35 : level == 2 ? 60 : 110,
            TowerType.Generator => level == 1 ? 75 : level == 2 ? 150 : 300,
            _ => 0
        };
    }

    // Положительное для генераторов, отрицательное (потребление) для боевых башен.
    public static int GetEnergy(TowerType type, int level)
    {
        if (type == TowerType.Generator)
        {
            int baseEnergy = level == 1 ? 5 : level == 2 ? 10 : 20;
            return (int)(baseEnergy * (1.0 + GlobalProgress.GeneratorUpgradeLevel * 0.10));
        }

        int consumption = type switch
        {
            TowerType.Tesla => level == 1 ? 3 : level == 2 ? 5 : 8,
            TowerType.Crossbow => level == 1 ? 2 : level == 2 ? 3 : 5,
            TowerType.Cannon => level == 1 ? 6 : level == 2 ? 9 : 12,
            TowerType.Slow => level == 1 ? 1 : level == 2 ? 2 : 4,
            _ => 0
        };
        return -consumption;
    }

    // Tesla, Slow, Generator: общая прогрессия радиуса. Crossbow: самый дальний. Cannon: самый короткий.
    public static double GetRange(TowerType type, int level)
    {
        return type switch
        {
            TowerType.Tesla or TowerType.Slow or TowerType.Generator => level == 1 ? 2.0 : level == 2 ? 3.5 : 5.0,
            TowerType.Crossbow => level == 1 ? 2.5 : level == 2 ? 4.0 : 5.5,
            TowerType.Cannon => level == 1 ? 1.8 : level == 2 ? 3.0 : 4.5,
            _ => 0
        };
    }
}
