namespace tower_defense;

public static class GlobalProgress
{
    public static int Gems { get; set; }
    public static int DamageUpgradeLevel { get; set; }     // +5% к урону всех башен за уровень
    public static int GeneratorUpgradeLevel { get; set; }  // +10% к выработке генератора за уровень
    public static int BaseHPUpgradeLevel { get; set; }     // +2 к стартовому HP базы за уровень

    // Стоимость улучшения: 50, 100, 200, 400, 800...
    public static int GetUpgradeCost(int currentLevel)
        => 50 * (int)Math.Pow(2, currentLevel);

    public static bool TryUpgradeDamage()
    {
        int cost = GetUpgradeCost(DamageUpgradeLevel);
        if (Gems < cost) return false;
        Gems -= cost;
        DamageUpgradeLevel++;
        return true;
    }

    public static bool TryUpgradeGenerator()
    {
        int cost = GetUpgradeCost(GeneratorUpgradeLevel);
        if (Gems < cost) return false;
        Gems -= cost;
        GeneratorUpgradeLevel++;
        return true;
    }

    public static bool TryUpgradeBaseHP()
    {
        int cost = GetUpgradeCost(BaseHPUpgradeLevel);
        if (Gems < cost) return false;
        Gems -= cost;
        BaseHPUpgradeLevel++;
        return true;
    }
}
