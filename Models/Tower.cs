namespace tower_defense.Models;

public class Tower
{
    public TowerType Type { get; set; }
    public int Level { get; set; } = 1;
    public int GridX { get; set; }
    public int GridY { get; set; }
    public double Cooldown { get; set; }
    public double MaxCooldown { get; set; }
    public TargetPriority Priority { get; set; } = TargetPriority.First;
    public bool IsPowered { get; set; } = true;
}
