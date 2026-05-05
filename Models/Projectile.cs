namespace tower_defense.Models;

public class Projectile
{
    public double X { get; set; }
    public double Y { get; set; }
    public double PrevX { get; set; }
    public double PrevY { get; set; }
    public Enemy Target { get; set; } = null!;
    public double Speed { get; set; }
    public double Damage { get; set; }
    public bool IsAoE { get; set; }
    public double AoERadius { get; set; }
    public TowerType SourceType { get; set; }
}
