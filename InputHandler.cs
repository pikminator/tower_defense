using tower_defense.Models;

namespace tower_defense;

public class InputHandler
{
    public bool InMainMenu { get; set; } = true;
    public bool ShowingLevels { get; set; }
    public bool ShowingTechs { get; set; }
    public bool ShowingRecords { get; set; }
    public bool IsPaused { get; set; }
    public int TimeScale { get; set; } = 1;
    public int CurrentLevel { get; set; } = 1;
    public TowerType SelectedTower { get; set; } = TowerType.Generator;
    public Tower? SelectedTowerObj { get; set; }

    public void HandleMouseClick(Point location, Size clientSize, GameController game,
        int topHudHeight, int bottomPanelY, int offsetX, int offsetY, int cellSize)
    {
        if (InMainMenu)
        {
            HandleMenuClick(location, clientSize, game);
            return;
        }

        // Конец игры
        if (game.BaseHP <= 0 || (game.Wave > GameConstants.FinalWave && game.Enemies.Count == 0))
        {
            if (GameRenderer.GetGameOverRestartBtn(clientSize).Contains(location))
            {
                HighScores.Add(game.Wave - 1, game.Score);
                SaveManager.Save();
                InMainMenu = true;
            }
            return;
        }

        // Кнопки HUD
        if (GameRenderer.GetExitToMenuBtn(clientSize, topHudHeight, cellSize).Contains(location))
        {
            SaveManager.Save();
            InMainMenu = true;
            return;
        }

        if (GameRenderer.GetSpeedBtn(clientSize, topHudHeight, cellSize).Contains(location))
        {
            TimeScale = TimeScale switch { 1 => 2, 2 => 5, _ => 1 };
            return;
        }

        if (GameRenderer.GetPauseBtn(clientSize, topHudHeight, cellSize).Contains(location))
        {
            IsPaused = !IsPaused;
            return;
        }

        if (GameRenderer.GetRestartBtn(clientSize, topHudHeight, cellSize).Contains(location))
        {
            game.InitializeLevel(CurrentLevel);
            SelectedTowerObj = null;
            IsPaused = false;
            return;
        }

        if (GameRenderer.GetStartWaveBtn(clientSize, topHudHeight, cellSize).Contains(location))
        {
            game.SpawnWave();
            return;
        }

        // Панель информации о башне (работает даже на паузе)
        if (SelectedTowerObj != null)
        {
            if (GameRenderer.GetUpgradeBtn(clientSize, topHudHeight).Contains(location))
            {
                game.UpgradeTower(SelectedTowerObj);
                return;
            }
            if (GameRenderer.GetSellBtn(clientSize, topHudHeight).Contains(location))
            {
                game.SellTower(SelectedTowerObj);
                SelectedTowerObj = null;
                return;
            }
            if (SelectedTowerObj.Type != TowerType.Generator &&
                GameRenderer.GetPriorityBtn(clientSize, topHudHeight).Contains(location))
            {
                // Цикл: Первый -> Сильнейший -> Ближайший -> Первый
                SelectedTowerObj.Priority = SelectedTowerObj.Priority switch
                {
                    TargetPriority.First => TargetPriority.Strongest,
                    TargetPriority.Strongest => TargetPriority.Closest,
                    _ => TargetPriority.First
                };
                return;
            }
        }

        // На паузе: только кнопки и панель башни, нельзя ставить/выбирать
        if (IsPaused) return;

        // Нижняя панель выбора башни
        int panelH = cellSize * 3 + 15;
        var bottomCard = new Rectangle(0, bottomPanelY, clientSize.Width, panelH);
        if (bottomCard.Contains(location))
        {
            TowerType[] types = (TowerType[])Enum.GetValues(typeof(TowerType));
            for (int i = 0; i < types.Length; i++)
            {
                if (GameRenderer.GetTowerBtn(clientSize, i, bottomPanelY, cellSize).Contains(location))
                {
                    SelectedTower = types[i];
                    return;
                }
            }
        }
        else
        {
            // Игровое поле
            int gridX = (location.X - offsetX) / cellSize;
            int gridY = (location.Y - offsetY) / cellSize;

            if (location.Y <= topHudHeight || gridX < 0 || gridX >= GameConstants.TargetCols ||
                gridY < 0 || gridY >= GameConstants.TargetRows)
                return;

            Tower? clicked = game.Towers.FirstOrDefault(t => t.GridX == gridX && t.GridY == gridY);
            if (clicked != null)
            {
                SelectedTowerObj = clicked;
            }
            else
            {
                SelectedTowerObj = null;
                bool blocked = game.Towers.Any(t => t.GridX == gridX && t.GridY == gridY) ||
                               IsOnPath(game, gridX, gridY) || !IsAdjacentToPath(game, gridX, gridY);
                if (!blocked)
                    game.BuildTower(SelectedTower, gridX, gridY);
            }
        }
    }

    public void HandleRightClick() => SelectedTowerObj = null;

    public void HandleKeyDown(Keys key)
    {
        if (key == Keys.Space && !InMainMenu)
            IsPaused = !IsPaused;
        if (key == Keys.Escape && !InMainMenu)
        {
            SaveManager.Save();
            InMainMenu = true;
        }
    }

    private void HandleMenuClick(Point location, Size clientSize, GameController game)
    {
        if (ShowingLevels)
        {
            for (int i = 0; i < 3; i++)
            {
                if (GameRenderer.GetLevelBtn(clientSize, i).Contains(location))
                {
                    CurrentLevel = i + 1;
                    game.InitializeLevel(CurrentLevel);
                    SelectedTowerObj = null;
                    IsPaused = false;
                    InMainMenu = false;
                    ShowingLevels = false;
                    return;
                }
            }
            if (GameRenderer.GetLevelBackBtn(clientSize).Contains(location))
                ShowingLevels = false;
        }
        else if (ShowingTechs)
        {
            if (GameRenderer.GetTechDmgBtn(clientSize).Contains(location))
            { GlobalProgress.TryUpgradeDamage(); SaveManager.Save(); }
            else if (GameRenderer.GetTechGenBtn(clientSize).Contains(location))
            { GlobalProgress.TryUpgradeGenerator(); SaveManager.Save(); }
            else if (GameRenderer.GetTechHpBtn(clientSize).Contains(location))
            { GlobalProgress.TryUpgradeBaseHP(); SaveManager.Save(); }
            else if (GameRenderer.GetTechBackBtn(clientSize).Contains(location))
                ShowingTechs = false;
        }
        else if (ShowingRecords)
        {
            if (GameRenderer.GetRecordsBackBtn(clientSize).Contains(location))
                ShowingRecords = false;
        }
        else
        {
            if (GameRenderer.GetMainMenuStartBtn(clientSize).Contains(location))
                ShowingLevels = true;
            else if (GameRenderer.GetMainMenuTechBtn(clientSize).Contains(location))
                ShowingTechs = true;
            else if (GameRenderer.GetMainMenuRecordsBtn(clientSize).Contains(location))
                ShowingRecords = true;
            else if (GameRenderer.GetMainMenuExitBtn(clientSize).Contains(location))
            { SaveManager.Save(); Application.Exit(); }
        }
    }

    private static bool IsOnPath(GameController game, int gridX, int gridY)
    {
        if (game.Path.Count < 2) return false;
        for (int i = 0; i < game.Path.Count - 1; i++)
        {
            var p1 = game.Path[i];
            var p2 = game.Path[i + 1];
            if (p1.X == p2.X && gridX == p1.X && gridY >= Math.Min(p1.Y, p2.Y) && gridY <= Math.Max(p1.Y, p2.Y))
                return true;
            if (p1.Y == p2.Y && gridY == p1.Y && gridX >= Math.Min(p1.X, p2.X) && gridX <= Math.Max(p1.X, p2.X))
                return true;
        }
        return false;
    }

    // Башню можно ставить только вплотную к дорожке (1 клетка).
    private static bool IsAdjacentToPath(GameController game, int gridX, int gridY)
    {
        if (game.Path.Count < 2) return false;
        for (int i = 0; i < game.Path.Count - 1; i++)
        {
            var p1 = game.Path[i];
            var p2 = game.Path[i + 1];
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int cx = gridX + dx;
                    int cy = gridY + dy;
                    if (p1.X == p2.X && cx == p1.X && cy >= Math.Min(p1.Y, p2.Y) && cy <= Math.Max(p1.Y, p2.Y))
                        return true;
                    if (p1.Y == p2.Y && cy == p1.Y && cx >= Math.Min(p1.X, p2.X) && cx <= Math.Max(p1.X, p2.X))
                        return true;
                }
            }
        }
        return false;
    }
}
