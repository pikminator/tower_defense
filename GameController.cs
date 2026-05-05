using tower_defense.Models;
using tower_defense.Stats;

namespace tower_defense
{
    public class GameController
    {
        public int CellSize { get; set; } = GameConstants.CellSize;
        public int Gold { get; set; } = GameConstants.StartingGold;
        public int BaseHP { get; set; } = 20;
        public int Wave { get; set; } = 1;
        public int EnergyProduction { get; set; }
        public int EnergyConsumption { get; set; }
        public int Score { get; set; }
        public int GemsEarnedThisRun { get; set; }
        public int EnemiesRemainingToSpawn => _enemiesRemaining;

        public List<Tower> Towers { get; } = new();
        public List<Enemy> Enemies { get; } = new();
        public List<Projectile> Projectiles { get; } = new();
        public List<HitEffect> ActiveEffects { get; } = new();
        public List<Point> Path { get; } = new();

        private Random _random = new();
        private double _spawnTimer;
        private int _enemiesRemaining;
        private int _enemiesSpawnedThisWave;
        private double _passiveIncomeTimer = GameConstants.PassiveIncomeInterval;

        public GameController()
        {
            InitializeLevel(1);
        }

        public void InitializeLevel(int level = 1)
        {
            Gold = GameConstants.StartingGold;
            BaseHP = 20 + (GlobalProgress.BaseHPUpgradeLevel * 2);
            Wave = 1;
            EnergyProduction = 0;
            EnergyConsumption = 0;
            Score = 0;
            GemsEarnedThisRun = 0;

            Towers.Clear();
            Enemies.Clear();
            Projectiles.Clear();
            ActiveEffects.Clear();

            Path.Clear();
            if (level == 1)
            {
                // Горизонтальный зигзаг
                Path.Add(new Point(-1, 8));
                Path.Add(new Point(12, 8));
                Path.Add(new Point(12, 15));
                Path.Add(new Point(25, 15));
                Path.Add(new Point(25, 8));
                Path.Add(new Point(35, 8));
            }
            else if (level == 2)
            {
                // Спираль
                Path.Add(new Point(-1, 3));
                Path.Add(new Point(6, 3));
                Path.Add(new Point(6, 8));
                Path.Add(new Point(2, 8));
                Path.Add(new Point(2, 13));
                Path.Add(new Point(12, 13));
                Path.Add(new Point(12, 4));
                Path.Add(new Point(18, 4));
                Path.Add(new Point(18, 14));
                Path.Add(new Point(26, 14));
                Path.Add(new Point(26, 8));
                Path.Add(new Point(35, 8));
            }
            else if (level == 3)
            {
                // Сложный маршрут
                Path.Add(new Point(-1, 18));
                Path.Add(new Point(28, 18));
                Path.Add(new Point(28, 3));
                Path.Add(new Point(4, 3));
                Path.Add(new Point(4, 13));
                Path.Add(new Point(22, 13));
                Path.Add(new Point(22, 8));
                Path.Add(new Point(10, 8));
                Path.Add(new Point(10, -1));
            }

            _enemiesRemaining = 0;
            _enemiesSpawnedThisWave = 0;
            _spawnTimer = 0;
            _passiveIncomeTimer = GameConstants.PassiveIncomeInterval;
        }

        public void Update(double deltaTime)
        {
            UpdatePassiveIncome(deltaTime);
            ProcessSpawning(deltaTime);
            UpdateEnemyEffects(deltaTime);
            MoveEnemies(deltaTime);
            ProcessTowers(deltaTime);
            MoveProjectiles(deltaTime);
            UpdateDyingEnemies(deltaTime);
            UpdateHitEffects(deltaTime);
            DistributeEnergy(deltaTime);
        }

        private void UpdatePassiveIncome(double deltaTime)
        {
            if (Wave < GameConstants.PassiveIncomeStartWave) return;

            bool enemiesActive = _enemiesRemaining > 0 || Enemies.Count > 0;
            if (!enemiesActive) return;

            _passiveIncomeTimer -= deltaTime;
            if (_passiveIncomeTimer <= 0)
            {
                Gold += GameConstants.PassiveIncomeAmount;
                _passiveIncomeTimer = GameConstants.PassiveIncomeInterval;
            }
        }

        private void ProcessSpawning(double deltaTime)
        {
            if (_enemiesRemaining <= 0) return;

            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0)
            {
                SpawnNextEnemy();
                _spawnTimer = GameConstants.SpawnInterval;
                _enemiesRemaining--;
            }
        }

        private void UpdateEnemyEffects(double deltaTime)
        {
            foreach (var enemy in Enemies)
            {
                if (enemy.SlowTimer > 0)
                {
                    enemy.SlowTimer -= deltaTime;
                    if (enemy.SlowTimer <= 0)
                        enemy.SlowFactor = 1.0;
                }
            }
        }

        // Идём задом наперёд, потому что удаляем врагов, дошедших до базы
        private void MoveEnemies(double deltaTime)
        {
            for (var i = Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = Enemies[i];
                if (enemy.IsDying) continue;
                if (enemy.PathIndex >= Path.Count - 1) continue;

                var targetPoint = Path[enemy.PathIndex + 1];
                var targetX = targetPoint.X * CellSize;
                var targetY = targetPoint.Y * CellSize;

                var dx = targetX - enemy.X;
                var dy = targetY - enemy.Y;
                var dist = Math.Sqrt(dx * dx + dy * dy);
                var moveAmount = enemy.EffectiveSpeed * deltaTime * CellSize;

                if (dist <= moveAmount)
                {
                    enemy.X = targetX;
                    enemy.Y = targetY;
                    enemy.PathIndex++;
                    if (enemy.PathIndex >= Path.Count - 1)
                    {
                        BaseHP -= (enemy.Type == EnemyType.Boss ? 5 : 1);
                        Enemies.RemoveAt(i);
                    }
                }
                else
                {
                    enemy.X += (dx / dist) * moveAmount;
                    enemy.Y += (dy / dist) * moveAmount;
                }
            }
        }

        // Генераторы не атакуют, только дают энергию
        private void ProcessTowers(double deltaTime)
        {
            foreach (var tower in Towers)
            {
                if (!tower.IsPowered || tower.Type == TowerType.Generator) continue;

                tower.Cooldown -= deltaTime;
                if (tower.Cooldown <= 0)
                {
                    var target = FindTarget(tower);
                    if (target != null)
                    {
                        Shoot(tower, target);
                        tower.Cooldown = TowerStats.GetCooldown(tower.Type, tower.Level);
                    }
                }
            }
        }

        private void MoveProjectiles(double deltaTime)
        {
            for (var i = Projectiles.Count - 1; i >= 0; i--)
            {
                var proj = Projectiles[i];
                if (proj.Target == null || !Enemies.Contains(proj.Target) || proj.Target.IsDying)
                {
                    Projectiles.RemoveAt(i);
                    continue;
                }

                proj.PrevX = proj.X;
                proj.PrevY = proj.Y;

                var dx = proj.Target.X - proj.X;
                var dy = proj.Target.Y - proj.Y;
                var dist = Math.Sqrt(dx * dx + dy * dy);
                var moveSpeed = proj.Speed * deltaTime;

                if (dist <= moveSpeed)
                {
                    ApplyDamage(proj, proj.Target);
                    ActiveEffects.Add(new HitEffect
                    {
                        X = proj.Target.X,
                        Y = proj.Target.Y,
                        Timer = GameConstants.HitEffectDuration,
                        MaxTimer = GameConstants.HitEffectDuration,
                        MaxRadius = proj.IsAoE ? GameConstants.CannonAoERadius : GameConstants.HitEffectMaxRadius
                    });
                    Projectiles.RemoveAt(i);
                }
                else
                {
                    proj.X += (dx / dist) * moveSpeed;
                    proj.Y += (dy / dist) * moveSpeed;
                }
            }
        }

        // Выбирает цель в радиусе башни по приоритету. Меньше score, выше приоритет.
        private Enemy? FindTarget(Tower tower)
        {
            var maxRange = TowerStats.GetRange(tower.Type, tower.Level) * CellSize;
            var tCenterX = tower.GridX * CellSize + CellSize / 2.0;
            var tCenterY = tower.GridY * CellSize + CellSize / 2.0;

            Enemy? bestTarget = null;
            var bestScore = double.MaxValue;

            foreach (var enemy in Enemies)
            {
                if (enemy.IsDying) continue;

                var dx = enemy.X - tCenterX;
                var dy = enemy.Y - tCenterY;
                var dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist <= maxRange)
                {
                    var score = CalculatePriorityScore(tower.Priority, enemy, dist);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestTarget = enemy;
                    }
                }
            }
            return bestTarget;
        }

        // First: чем дальше враг продвинулся по пути, тем меньше score (выше приоритет).
        private double CalculatePriorityScore(TargetPriority priority, Enemy enemy, double distance)
        {
            return priority switch
            {
                TargetPriority.First =>
                    -(enemy.PathIndex * 1000 -
                      Math.Sqrt(Math.Pow(Path[enemy.PathIndex + 1].X * CellSize - enemy.X, 2) +
                                Math.Pow(Path[enemy.PathIndex + 1].Y * CellSize - enemy.Y, 2))),

                TargetPriority.Strongest =>
                    -enemy.HP,

                TargetPriority.Closest =>
                    distance,

                _ => distance
            };
        }

        private void Shoot(Tower tower, Enemy target)
        {
            var startX = tower.GridX * CellSize + CellSize / 2.0;
            var startY = tower.GridY * CellSize + CellSize / 2.0;

            if (tower.Type == TowerType.Tesla)
            {
                target.HP -= TowerStats.GetDamage(tower.Type, tower.Level);
                ActiveEffects.Add(new HitEffect
                {
                    X = target.X,
                    Y = target.Y,
                    Timer = GameConstants.HitEffectDuration,
                    MaxTimer = GameConstants.HitEffectDuration,
                    MaxRadius = GameConstants.HitEffectMaxRadius
                });
                CheckEnemyDeath(target);
            }
            else
            {
                Projectiles.Add(new Projectile
                {
                    X = startX,
                    Y = startY,
                    PrevX = startX,
                    PrevY = startY,
                    Target = target,
                    Speed = GameConstants.ProjectileSpeed,
                    Damage = TowerStats.GetDamage(tower.Type, tower.Level),
                    IsAoE = tower.Type == TowerType.Cannon,
                    AoERadius = tower.Type == TowerType.Cannon ? GameConstants.CannonAoERadius : 0,
                    SourceType = tower.Type
                });
            }
        }

        private void ApplyDamage(Projectile proj, Enemy target)
        {
            if (proj.IsAoE)
            {
                for (var i = Enemies.Count - 1; i >= 0; i--)
                {
                    var e = Enemies[i];
                    var dx = e.X - target.X;
                    var dy = e.Y - target.Y;
                    if (Math.Sqrt(dx * dx + dy * dy) <= proj.AoERadius)
                    {
                        e.HP -= proj.Damage;
                        CheckEnemyDeath(e);
                    }
                }
            }
            else
            {
                target.HP -= proj.Damage;

                if (proj.SourceType == TowerType.Slow)
                {
                    target.SlowFactor = GameConstants.SlowFactor;
                    target.SlowTimer = GameConstants.SlowDuration;
                }

                CheckEnemyDeath(target);
            }
        }

        private void CheckEnemyDeath(Enemy enemy)
        {
            if (enemy.HP <= 0 && !enemy.IsDying && Enemies.Contains(enemy))
            {
                enemy.IsDying = true;
                enemy.DeathTimer = GameConstants.DeathAnimDuration;
                Gold += enemy.Reward;
                Score += enemy.Reward;
            }
        }

        private void UpdateDyingEnemies(double deltaTime)
        {
            for (var i = Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = Enemies[i];
                if (!enemy.IsDying) continue;

                enemy.DeathTimer -= deltaTime;
                if (enemy.DeathTimer <= 0)
                    Enemies.RemoveAt(i);
            }
        }

        private void UpdateHitEffects(double deltaTime)
        {
            for (var i = ActiveEffects.Count - 1; i >= 0; i--)
            {
                var effect = ActiveEffects[i];
                effect.Timer -= deltaTime;
                if (effect.Timer <= 0)
                    ActiveEffects.RemoveAt(i);
            }
        }

        private void SpawnNextEnemy()
        {
            if (Path.Count == 0) return;

            var type = EnemyType.Normal;

            // Каждый 10-й враг в волне: босс. Остальные: случайный тип.
            if (_enemiesSpawnedThisWave % GameConstants.BossSpawnEveryNth == 0 && _enemiesSpawnedThisWave > 0)
                type = EnemyType.Boss;
            else if (_random.NextDouble() < GameConstants.FastEnemyChance)
                type = EnemyType.Fast;
            else if (_random.NextDouble() < GameConstants.ArmoredEnemyChance)
                type = EnemyType.Armored;

            // HP и скорость растут на 20% за каждые 5 волн
            var scaling = 1.0 + ((Wave / 5) * GameConstants.HpSpeedScalingPer5Waves);

            var enemy = new Enemy
            {
                Type = type,
                Speed = EnemyStats.GetBaseSpeed(type) * scaling,
                MaxHP = EnemyStats.GetBaseHealth(type, Wave) * scaling,
                Reward = EnemyStats.GetReward(type),
                PathIndex = 0,
                SlowFactor = 1.0
            };
            enemy.HP = enemy.MaxHP;
            enemy.X = Path[0].X * CellSize;
            enemy.Y = Path[0].Y * CellSize;

            Enemies.Add(enemy);
            _enemiesSpawnedThisWave++;
        }

        // Генераторы производят энергию, башни потребляют. Башня получает питание, только если энергии хватает И она в радиусе генератора.
        private void DistributeEnergy(double deltaTime)
        {
            EnergyProduction = 0;
            EnergyConsumption = 0;

            var generators = Towers.Where(t => t.Type == TowerType.Generator).ToList();
            foreach (var gen in generators)
            {
                EnergyProduction += TowerStats.GetEnergy(gen.Type, gen.Level);
            }

            var availableEnergy = EnergyProduction;
            var combatTowers = Towers.Where(t => t.Type != TowerType.Generator).ToList();

            foreach (var tower in combatTowers)
            {
                var reqEnergy = -TowerStats.GetEnergy(tower.Type, tower.Level);

                if (IsInRangeOfAnyGenerator(tower, generators, availableEnergy, reqEnergy))
                {
                    availableEnergy -= reqEnergy;
                    EnergyConsumption += reqEnergy;
                    tower.IsPowered = true;
                }
                else
                {
                    tower.IsPowered = false;
                }
            }
        }

        private static bool IsInRangeOfAnyGenerator(Tower tower, List<Tower> generators, int availableEnergy, int required)
        {
            if (availableEnergy < required) return false;

            foreach (var gen in generators)
            {
                var dist = Math.Sqrt(Math.Pow(tower.GridX - gen.GridX, 2) +
                                     Math.Pow(tower.GridY - gen.GridY, 2));
                if (dist <= TowerStats.GetRange(TowerType.Generator, gen.Level))
                    return true;
            }
            return false;
        }

        public bool IsCellPowered(int gridX, int gridY)
        {
            foreach (var gen in Towers)
            {
                if (gen.Type != TowerType.Generator) continue;
                var dist = Math.Sqrt(Math.Pow(gridX - gen.GridX, 2) + Math.Pow(gridY - gen.GridY, 2));
                if (dist <= TowerStats.GetRange(TowerType.Generator, gen.Level))
                    return true;
            }
            return false;
        }

        public bool BuildTower(TowerType type, int x, int y)
        {
            var cost = TowerStats.GetCost(type, 1);
            if (Gold < cost) return false;

            Gold -= cost;
            Towers.Add(new Tower { Type = type, GridX = x, GridY = y, Level = 1 });
            DistributeEnergy(0);
            return true;
        }

        public void SpawnWave()
        {
            if (Wave > GameConstants.GemFormulaWaveThreshold)
            {
                var gems = 1 + ((Wave - GameConstants.GemFormulaWaveThreshold) / GameConstants.GemFormulaDivisor);
                GemsEarnedThisRun += gems;
                GlobalProgress.Gems += gems;
            }

            _enemiesRemaining += 5 * Wave + _random.Next(0, Wave * 2);
            _enemiesSpawnedThisWave = 0;
            Wave++;
        }

        public bool SellTower(Tower t)
        {
            if (!Towers.Contains(t)) return false;

            var cost = TowerStats.GetCost(t.Type, t.Level);
            Gold += (int)(cost * GameConstants.SellRefundRate);
            Towers.Remove(t);
            DistributeEnergy(0);
            return true;
        }

        public bool UpgradeTower(Tower t)
        {
            if (t.Level >= GameConstants.MaxTowerLevel) return false;

            var cost = TowerStats.GetCost(t.Type, t.Level + 1);
            if (Gold < cost) return false;

            Gold -= cost;
            t.Level++;
            DistributeEnergy(0);
            return true;
        }
    }
}
