namespace tower_defense.Models;

public class Enemy
{
    public double X { get; set; }
    public double Y { get; set; }
    public double HP { get; set; }
    public double MaxHP { get; set; }
    public double Speed { get; set; }
    public EnemyType Type { get; set; }
    public int Reward { get; set; }
    public int PathIndex { get; set; }
    public double SlowTimer { get; set; }
    public double SlowFactor { get; set; } = 1.0;
    // При активном замедлении скорость умножается на SlowFactor (0.3)
    public double EffectiveSpeed => SlowTimer > 0 ? Speed * SlowFactor : Speed;

    public bool IsDying { get; set; }
    public double DeathTimer { get; set; }
}
