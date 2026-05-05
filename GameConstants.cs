namespace tower_defense;


// Константы игры

public static class GameConstants
{
    // Сетка
    public const int CellSize = 32;
    public const int TargetCols = 32;
    public const int TargetRows = 24;

    // Экономика
    public const int StartingGold = 150;
    public const int PassiveIncomeAmount = 5;
    public const double PassiveIncomeInterval = 1.0;
    public const int PassiveIncomeStartWave = 5;
    public const double SellRefundRate = 0.7;
    public const int MaxTowerLevel = 3;

    // Спавн
    public const double SpawnInterval = 1.0;
    public const int BossSpawnEveryNth = 10;
    public const double FastEnemyChance = 0.2;
    public const double ArmoredEnemyChance = 0.2;
    public const double HpSpeedScalingPer5Waves = 0.20;

    // Бой
    public const double ProjectileSpeed = 400.0;
    public const double CannonAoERadius = 64.0;

    // Эффекты
    public const double SlowFactor = 0.3;
    public const double SlowDuration = 3.0;
    public const double DeathAnimDuration = 0.3;
    public const double HitEffectDuration = 0.35;
    public const double HitEffectMaxRadius = 24.0;

    // Волны
    public const int FinalWave = 50;
    public const int GemFormulaWaveThreshold = 10;
    public const int GemFormulaDivisor = 5;
}
