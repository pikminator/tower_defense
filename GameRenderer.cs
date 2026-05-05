using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using tower_defense.Models;
using tower_defense.Stats;

namespace tower_defense;

public class GameRenderer
{
    private readonly Dictionary<string, Bitmap> _sprites;

    private Font MainFont { get; }
    private Font BoldFont { get; }
    private Font SmallFont { get; }
    private Font TitleFont { get; }
    private Font HeaderFont { get; }
    private Font SubheaderFont { get; }
    private Font BtnFont { get; }
    private Font HugeFont { get; }

    private Brush AccentBrush { get; }
    private Brush SecondaryBrush { get; }
    private Brush DangerBrush { get; }
    private Brush SuccessBrush { get; }
    private Brush WarningBrush { get; }
    private Brush PanelBrush { get; }
    private Brush PathBrush { get; }
    private Brush HudBrush { get; }
    private Brush ShadowBrush { get; }
    private Brush OverlayBrush { get; }
    private Brush WhiteGhostBrush { get; }
    private Brush GreenGhostBrush { get; }
    private Brush YellowGhostBrush { get; }
    private Brush RedGhostBrush { get; }
    private Brush BossColorBrush { get; }
    private Brush EnemyColorBrush { get; }
    private Brush ProjBrush { get; }
    private Brush BlueBrush { get; }
    private Brush BlackBrush { get; }
    private Brush TomatoBrush { get; }
    private Brush LightGreenBrush { get; }

    private Pen GridPen { get; }
    private Pen PathPen { get; }
    private Pen RedPen { get; }
    private Pen WhitePen { get; }
    private Pen RangePen { get; }

    private StringFormat CenterAlign { get; }
    private StringFormat NearFarAlign { get; }
    private StringFormat CenterFarAlign { get; }

    private static readonly string[] TowerRuNames = { "Тесла", "Арбалет", "Пушка", "Замедление", "Генератор" };
    private static readonly string[] TowerSpriteKeys = { "tesla", "crossbow", "cannon", "slow", "generator" };

    // Для анимации звёзд в меню
    private readonly List<(float X, float Y, float Speed, float Size)> _stars = new();
    private readonly Random _starRng = new();

    public GameRenderer(
        Dictionary<string, Bitmap> sprites,
        Font mainFont, Font boldFont, Font smallFont, Font titleFont,
        Font headerFont, Font subheaderFont, Font btnFont, Font hugeFont,
        Brush accentBrush, Brush secondaryBrush, Brush dangerBrush, Brush successBrush,
        Brush warningBrush, Brush panelBrush, Brush pathBrush, Brush hudBrush,
        Brush shadowBrush, Brush overlayBrush, Brush whiteGhostBrush, Brush greenGhostBrush,
        Brush yellowGhostBrush, Brush redGhostBrush, Brush bossColorBrush, Brush enemyColorBrush,
        Brush projBrush, Brush blueBrush, Brush blackBrush, Brush tomatoBrush, Brush lightGreenBrush,
        Pen gridPen, Pen pathPen, Pen redPen, Pen whitePen, Pen rangePen,
        StringFormat centerAlign, StringFormat nearFarAlign, StringFormat centerFarAlign)
    {
        _sprites = sprites;
        MainFont = mainFont;
        BoldFont = boldFont;
        SmallFont = smallFont;
        TitleFont = titleFont;
        HeaderFont = headerFont;
        SubheaderFont = subheaderFont;
        BtnFont = btnFont;
        HugeFont = hugeFont;
        AccentBrush = accentBrush;
        SecondaryBrush = secondaryBrush;
        DangerBrush = dangerBrush;
        SuccessBrush = successBrush;
        WarningBrush = warningBrush;
        PanelBrush = panelBrush;
        PathBrush = pathBrush;
        HudBrush = hudBrush;
        ShadowBrush = shadowBrush;
        OverlayBrush = overlayBrush;
        WhiteGhostBrush = whiteGhostBrush;
        GreenGhostBrush = greenGhostBrush;
        YellowGhostBrush = yellowGhostBrush;
        RedGhostBrush = redGhostBrush;
        BossColorBrush = bossColorBrush;
        EnemyColorBrush = enemyColorBrush;
        ProjBrush = projBrush;
        BlueBrush = blueBrush;
        BlackBrush = blackBrush;
        TomatoBrush = tomatoBrush;
        LightGreenBrush = lightGreenBrush;
        GridPen = gridPen;
        PathPen = pathPen;
        RedPen = redPen;
        WhitePen = whitePen;
        RangePen = rangePen;
        CenterAlign = centerAlign;
        NearFarAlign = nearFarAlign;
        CenterFarAlign = centerFarAlign;

        for (int i = 0; i < 40; i++)
            _stars.Add((_starRng.Next(0, 1024), _starRng.Next(0, 768),
                        _starRng.Next(5, 20) / 10f, _starRng.Next(1, 3)));
    }

    public void DrawMainMenu(Graphics g, Size clientSize)
    {
        // Звёздный фон
        g.Clear(Color.FromArgb(15, 15, 25));
        foreach (var star in _stars)
        {
            int alpha = (int)(120 + Math.Sin(Environment.TickCount * 0.001 * star.Speed + star.X) * 80);
            using var b = new SolidBrush(Color.FromArgb(alpha, 200, 200, 255));
            g.FillEllipse(b, star.X, star.Y, star.Size, star.Size);
        }

        // Центрируем блок по вертикали
        int btnH = 65;
        int btnSpacing = 80;
        int totalH = 60 + 20 + btnH * 4 + btnSpacing * 3;
        int startY = (clientSize.Height - totalH) / 2;

        // Заголовок по центру горизонтали
        var titleRect = new Rectangle(0, startY, clientSize.Width, 60);
        g.DrawString("TOWER DEFENSE", TitleFont, Brushes.Black, new Rectangle(3, startY + 3, clientSize.Width, 60), CenterAlign);
        using var titleBrush = new SolidBrush(Color.Gold);
        g.DrawString("TOWER DEFENSE", TitleFont, titleBrush, titleRect, CenterAlign);

        int btnStartY = startY + 80;
        DrawMenuSpriteButton(g, clientSize, "ui_sword", "СТАРТ", btnStartY,
            Color.FromArgb(34, 197, 94), Color.FromArgb(22, 130, 60));
        DrawMenuSpriteButton(g, clientSize, "ui_gear", "ТЕХНОЛОГИИ", btnStartY + btnSpacing,
            Color.FromArgb(79, 70, 229), Color.FromArgb(50, 45, 150));
        DrawMenuSpriteButton(g, clientSize, "ui_trophy", "РЕКОРДЫ", btnStartY + btnSpacing * 2,
            Color.FromArgb(168, 85, 247), Color.FromArgb(110, 40, 180));
        DrawMenuSpriteButton(g, clientSize, null, "ВЫХОД", btnStartY + btnSpacing * 3,
            Color.FromArgb(180, 50, 50), Color.FromArgb(120, 30, 30));

        var hintText = "ESC = назад в меню | Пробел = пауза";
        var hintSz = g.MeasureString(hintText, MainFont).ToSize();
        g.DrawString(hintText, MainFont, Brushes.DimGray,
            clientSize.Width / 2 - hintSz.Width / 2, clientSize.Height - 50);
    }

    private void DrawMenuSpriteButton(Graphics g, Size clientSize, string? spriteKey, string text, int y,
        Color topColor, Color bottomColor)
    {
        const int btnW = 320;
        const int btnH = 65;
        int x = clientSize.Width / 2 - btnW / 2;
        var rect = new Rectangle(x, y, btnW, btnH);

        using var brush = new LinearGradientBrush(rect, topColor, bottomColor, LinearGradientMode.Vertical);
        FillRoundedRect(g, brush, rect, 10);
        g.DrawPath(new Pen(Color.FromArgb(255, 255, 255, 255), 2), GetRoundedRect(rect, 10));

        if (spriteKey != null && _sprites.TryGetValue(spriteKey, out var sprite))
            g.DrawImage(sprite, x + 14, y + (btnH - 30) / 2, 30, 30);

        int textX = x + (spriteKey != null ? 52 : 0);
        g.DrawString(text, BoldFont, Brushes.White,
            new Rectangle(textX, y + 2, btnW - 14 - (spriteKey != null ? 52 : 0), btnH - 4), CenterAlign);
    }

    public void DrawLevelSelect(Graphics g, Size clientSize)
    {
        DrawMenuBackground(g, clientSize);
        var lvlTitle = "ВЫБОР УРОВНЯ";
        var lvlSz = g.MeasureString(lvlTitle, HeaderFont).ToSize();
        DrawTextWithShadow(g, lvlTitle, HeaderFont, Color.White, clientSize.Width / 2 - lvlSz.Width / 2, 150);

        for (int i = 1; i <= 3; i++)
        {
            DrawMenuSpriteButton(g, clientSize, null, $"УРОВЕНЬ {i}", 220 + (i - 1) * 85,
                Color.FromArgb(60, 100, 180), Color.FromArgb(30, 60, 120));
        }

        DrawMenuSpriteButton(g, clientSize, null, "НАЗАД", 480,
            Color.FromArgb(82, 82, 91), Color.FromArgb(50, 50, 58));
    }

    public void DrawTechTree(Graphics g, Size clientSize)
    {
        DrawMenuBackground(g, clientSize);
        var tTitle = "ТЕХНОЛОГИИ";
        var tSz = g.MeasureString(tTitle, HeaderFont).ToSize();
        DrawTextWithShadow(g, tTitle, HeaderFont, Color.White, clientSize.Width / 2 - tSz.Width / 2, 140);
        var gemText = $"Гемы: {GlobalProgress.Gems}";
        var gemSz = g.MeasureString(gemText, SubheaderFont).ToSize();
        int gemTotalW = 28 + 8 + gemSz.Width;
        int gemStartX = clientSize.Width / 2 - gemTotalW / 2;
        if (_sprites.TryGetValue("ui_gem", out var gemSpr))
            g.DrawImage(gemSpr, gemStartX, 180, 28, 28);
        DrawTextWithShadow(g, gemText, SubheaderFont, Color.Cyan, gemStartX + 36, 185);

        DrawTechButton(g, clientSize, 240, "+5% к урону всех башен",
            GlobalProgress.DamageUpgradeLevel,
            GlobalProgress.GetUpgradeCost(GlobalProgress.DamageUpgradeLevel));
        DrawTechButton(g, clientSize, 355, "+10% к выработке генераторов",
            GlobalProgress.GeneratorUpgradeLevel,
            GlobalProgress.GetUpgradeCost(GlobalProgress.GeneratorUpgradeLevel));
        DrawTechButton(g, clientSize, 470, "+2 к стартовому HP базы",
            GlobalProgress.BaseHPUpgradeLevel,
            GlobalProgress.GetUpgradeCost(GlobalProgress.BaseHPUpgradeLevel));

        DrawMenuSpriteButton(g, clientSize, null, "НАЗАД", 590,
            Color.FromArgb(82, 82, 91), Color.FromArgb(50, 50, 58));
    }

    private void DrawTechButton(Graphics g, Size clientSize, int y, string label, int level, int cost)
    {
        int btnH = 100;
        var rect = new Rectangle(clientSize.Width / 2 - 220, y, 440, btnH);
        using var brush = new LinearGradientBrush(rect,
            Color.FromArgb(60, 60, 120), Color.FromArgb(40, 40, 80), LinearGradientMode.Vertical);
        FillRoundedRect(g, brush, rect, 10);

        var affordable = GlobalProgress.Gems >= cost;
        var tc = affordable ? Brushes.White : Brushes.Gray;
        int tx = rect.X + 15;

        // Измеряем общую высоту трёх строк и центрируем по вертикали
        float lineH = BoldFont.GetHeight(g);
        float lineGap = lineH + 10; // увеличенный межстрочный интервал
        float totalH = lineH + lineGap * 2 + 4;
        float startY = rect.Y + (rect.Height - totalH) / 2;

        g.DrawString(label, BoldFont, tc, new PointF(tx, startY));
        g.DrawString($"Уровень: {level} / 5", BoldFont, tc, new PointF(tx, startY + lineGap));

        // Цена + гем + число на одной строке
        float priceY = startY + lineGap * 2;
        var priceLabelSz = g.MeasureString("Цена: ", BoldFont).ToSize();
        g.DrawString("Цена: ", BoldFont, tc, new PointF(tx, priceY));
        int iconX = tx + priceLabelSz.Width + 2;
        if (_sprites.TryGetValue("ui_gem", out var gemIcon))
            g.DrawImage(gemIcon, iconX, (int)priceY - 2, 18, 18);
        g.DrawString($"{cost}", BoldFont, affordable ? Brushes.Cyan : Brushes.Gray, new PointF(iconX + 22, priceY));

        int borderAlpha = affordable ? 200 : 80;
        using var borderPen = new Pen(Color.FromArgb(borderAlpha, affordable ? 0 : 128, affordable ? 255 : 128, affordable ? 80 : 128), 2);
        g.DrawPath(borderPen, GetRoundedRect(rect, 10));
    }

    private void DrawMenuBackground(Graphics g, Size clientSize)
    {
        g.Clear(Color.FromArgb(15, 15, 25));
        var r = new Rectangle(0, 0, clientSize.Width, clientSize.Height);
        using var bg = new LinearGradientBrush(r, Color.FromArgb(20, 20, 40), Color.FromArgb(10, 10, 20),
            LinearGradientMode.Vertical);
        g.FillRectangle(bg, r);

        foreach (var star in _stars)
        {
            int alpha = (int)(80 + Math.Sin(Environment.TickCount * 0.0008f * star.Speed + star.X) * 40);
            using var b = new SolidBrush(Color.FromArgb(alpha, 200, 200, 255));
            g.FillEllipse(b, star.X % clientSize.Width, star.Y % clientSize.Height, star.Size, star.Size);
        }
    }

    private static void DrawTextWithShadow(Graphics g, string text, Font font, Color color, int x, int y)
    {
        using var b = new SolidBrush(color);
        g.DrawString(text, font, Brushes.Black, new PointF(x + 3, y + 3), StringFormat.GenericDefault);
        g.DrawString(text, font, b, new PointF(x, y), StringFormat.GenericDefault);
    }

    public void DrawRecordsScreen(Graphics g, Size clientSize)
    {
        DrawMenuBackground(g, clientSize);
        var rTitle = "РЕКОРДЫ";
        var rSz = g.MeasureString(rTitle, HeaderFont).ToSize();
        DrawTextWithShadow(g, rTitle, HeaderFont, Color.Gold, clientSize.Width / 2 - rSz.Width / 2, 140);

        var records = HighScores.GetTop5();
        if (records.Count == 0)
        {
            g.DrawString("Нет рекордов. Сыграйте первую игру!", SubheaderFont, Brushes.DimGray,
                new Rectangle(0, 260, clientSize.Width, 40), CenterAlign);
        }
        else
        {
            Color[] rc = { Color.Gold, Color.Silver, Color.FromArgb(205, 127, 50) };
            for (int i = 0; i < records.Count; i++)
            {
                var r = records[i];
                string line = $"{i + 1}.  Волна {r.Wave}   |   Счёт {r.Score}   |   {r.Date:dd.MM.yyyy HH:mm}";
                var lineSz = g.MeasureString(line, BoldFont).ToSize();
                using var brush = new SolidBrush(i < 3 ? rc[i] : Color.White);
                g.DrawString(line, BoldFont, brush,
                    new PointF(clientSize.Width / 2 - lineSz.Width / 2, 250 + i * 50));
            }
        }

        DrawMenuSpriteButton(g, clientSize, null, "НАЗАД", 500,
            Color.FromArgb(82, 82, 91), Color.FromArgb(50, 50, 58));
    }

    public void DrawGameField(Graphics g, GameController game, int cellSize, Point mousePos,
        TowerType selectedTower, int offsetX, int offsetY, int topHudHeight, Rectangle bottomPanel)
    {
        int fieldW = GameConstants.TargetCols * cellSize;
        int fieldH = GameConstants.TargetRows * cellSize;
        g.FillRectangle(new SolidBrush(Color.FromArgb(20, 40, 15)), offsetX, offsetY, fieldW, fieldH);

        g.TranslateTransform(offsetX, offsetY);

        DrawGrid(g, cellSize);
        DrawPath(g, game, cellSize);
        DrawShadows(g, game, cellSize);
        DrawGeneratorRange(g, game, cellSize, selectedTower, bottomPanel, mousePos, offsetX, offsetY, topHudHeight);
        DrawTowers(g, game, cellSize);
        DrawEnemies(g, game, cellSize);
        DrawProjectiles(g, game);
        DrawHitEffects(g, game);
        DrawPlacementGhost(g, game, cellSize, mousePos, selectedTower, offsetX, offsetY, topHudHeight, bottomPanel);

        g.ResetTransform();

        // Спавн-зона и база
        DrawSpawnAndBase(g, game, cellSize, offsetX, offsetY);
    }

    private void DrawSpawnAndBase(Graphics g, GameController game, int cellSize, int offsetX, int offsetY)
    {
        if (game.Path.Count < 2) return;
        var start = game.Path[0];
        var end = game.Path[^1];
        int sx = offsetX + start.X * cellSize;
        int sy = offsetY + start.Y * cellSize;
        int ex = offsetX + end.X * cellSize;
        int ey = offsetY + end.Y * cellSize;

        using var spawnPen = new Pen(Color.FromArgb(100, 255, 80, 80), 3);
        g.DrawRectangle(spawnPen, sx - 2, sy - 2, cellSize + 4, cellSize + 4);

        using var basePen = new Pen(Color.FromArgb(100, 80, 200, 255), 3);
        g.DrawRectangle(basePen, ex - 2, ey - 2, cellSize + 4, cellSize + 4);
    }

    private void DrawGrid(Graphics g, int cellSize)
    {
        using var p = new Pen(Color.FromArgb(30, 30, 50), 1);
        for (int x = 0; x <= GameConstants.TargetCols * cellSize; x += cellSize)
            g.DrawLine(p, x, 0, x, GameConstants.TargetRows * cellSize);
        for (int y = 0; y <= GameConstants.TargetRows * cellSize; y += cellSize)
            g.DrawLine(p, 0, y, GameConstants.TargetCols * cellSize, y);
    }

    private void DrawPath(Graphics g, GameController game, int cellSize)
    {
        if (game.Path.Count < 2) return;

        // Текстурированный путь
        var pathPoints = new Point[game.Path.Count];
        for (int i = 0; i < game.Path.Count; i++)
            pathPoints[i] = new Point(game.Path[i].X * cellSize + cellSize / 2,
                                       game.Path[i].Y * cellSize + cellSize / 2);

        using var pathBrush = new SolidBrush(Color.FromArgb(55, 50, 40));
        using var pathOutline = new Pen(Color.FromArgb(100, 80, 70, 50), cellSize + 4) { LineJoin = LineJoin.Round };
        g.DrawLines(pathOutline, pathPoints);

        using var pathFill = new Pen(Color.FromArgb(70, 60, 45), cellSize) { LineJoin = LineJoin.Round };
        g.DrawLines(pathFill, pathPoints);

        // Точечная текстура пути
        using var dotPen = new Pen(Color.FromArgb(30, 90, 80, 60), 1);
        for (int i = 0; i < game.Path.Count - 1; i++)
        {
            var p1 = pathPoints[i];
            var p2 = pathPoints[i + 1];
            float len = (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            for (float t = 0; t < len; t += 12)
            {
                float ratio = t / len;
                float px = p1.X + (p2.X - p1.X) * ratio + (_starRng.Next(3) - 1);
                float py = p1.Y + (p2.Y - p1.Y) * ratio + (_starRng.Next(3) - 1);
                g.DrawLine(dotPen, px, py, px + 1, py + 1);
            }
        }
    }

    private void DrawShadows(Graphics g, GameController game, int cellSize)
    {
        using var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
        foreach (var tower in game.Towers)
        {
            int sx = tower.GridX * cellSize + 2;
            int sy = tower.GridY * cellSize + (int)(cellSize * 0.7);
            g.FillEllipse(shadow, sx, sy, cellSize - 2, 5);
        }
        foreach (var enemy in game.Enemies)
        {
            if (enemy.IsDying) continue;
            g.FillEllipse(shadow, (float)enemy.X + 2, (float)enemy.Y + cellSize * 0.7f, cellSize - 4, 4);
        }
    }

    private void DrawGeneratorRange(Graphics g, GameController game, int cellSize, TowerType selectedTower,
        Rectangle bottomPanel, Point mousePos, int offsetX, int offsetY, int topHudHeight)
    {
        if (selectedTower != TowerType.Generator) return;
        if (bottomPanel.Contains(mousePos) || mousePos.Y <= topHudHeight) return;

        using var fill = new SolidBrush(Color.FromArgb(20, 234, 179, 8));
        using var pen = new Pen(Color.FromArgb(60, 234, 179, 8), 1) { DashStyle = DashStyle.Dash };

        foreach (var gen in game.Towers)
        {
            if (gen.Type != TowerType.Generator) continue;
            float rng = (float)TowerStats.GetRange(TowerType.Generator, gen.Level) * cellSize;
            float cx = gen.GridX * cellSize + cellSize / 2f;
            float cy = gen.GridY * cellSize + cellSize / 2f;
            g.FillEllipse(fill, cx - rng, cy - rng, rng * 2, rng * 2);
            g.DrawEllipse(pen, cx - rng, cy - rng, rng * 2, rng * 2);
        }
    }

    private void DrawTowers(Graphics g, GameController game, int cellSize)
    {
        foreach (var tower in game.Towers)
        {
            int keyIdx = (int)tower.Type;
            if (keyIdx < 0 || keyIdx >= TowerSpriteKeys.Length) continue;

            var spriteKey = TowerSpriteKeys[keyIdx];
            int spriteSize = cellSize - 4;

            if (_sprites.TryGetValue(spriteKey, out var src))
            {
                int tX = tower.GridX * cellSize + 2;
                int tY = tower.GridY * cellSize + 2;

                if (!tower.IsPowered && tower.Type != TowerType.Generator)
                {
                    // Затемнённая версия для выключенной башни
                    using var dimBmp = new Bitmap(spriteSize, spriteSize);
                    using var dimG = Graphics.FromImage(dimBmp);
                    dimG.DrawImage(src, 0, 0, spriteSize, spriteSize);
                    using var dimBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
                    dimG.FillRectangle(dimBrush, 0, 0, spriteSize, spriteSize);
                    g.DrawImage(dimBmp, tX, tY);

                    // Красный крест
                    g.DrawLine(RedPen, tX + 4, tY + 4, tX + spriteSize - 4, tY + spriteSize - 4);
                    g.DrawLine(RedPen, tX + spriteSize - 4, tY + 4, tX + 4, tY + spriteSize - 4);
                }
                else
                {
                    g.DrawImage(src, tX, tY, spriteSize, spriteSize);
                }

                // Звёзды уровня
                for (int s = 0; s < tower.Level; s++)
                {
                    g.DrawString("★", SmallFont, Brushes.Gold, tX + s * 9, tY - 8);
                }
            }
            else
            {
                // Fallback: цветной прямоугольник
                var tBase = tower.Type == TowerType.Generator ? WarningBrush : AccentBrush;
                if (!tower.IsPowered) tBase = SecondaryBrush;
                var tRect = new Rectangle(tower.GridX * cellSize + 2, tower.GridY * cellSize + 2,
                                           cellSize - 4, cellSize - 4);
                FillRoundedRect(g, tBase, tRect, 6);
                g.DrawString(tower.Level.ToString(), SmallFont, Brushes.Black, tRect, CenterAlign);
            }
        }
    }

    private void DrawEnemies(Graphics g, GameController game, int cellSize)
    {
        var enemyKeys = new[] { "enemy_normal", "enemy_fast", "enemy_armored", "enemy_boss" };

        foreach (var enemy in game.Enemies)
        {
            float alpha = 1f;
            float scale = 1f;
            if (enemy.IsDying)
            {
                float progress = 1f - (float)(enemy.DeathTimer / GameConstants.DeathAnimDuration);
                alpha = 1f - progress;
                scale = 1f - progress * 0.5f;
            }

            int keyIdx = (int)enemy.Type;
            if (keyIdx < 0 || keyIdx >= enemyKeys.Length) continue;

            int size = (int)((cellSize - 4) * scale);
            int offset = (cellSize - size) / 2;
            float ex = (float)enemy.X + offset;
            float ey = (float)enemy.Y + offset;

            bool isSlowed = enemy.SlowTimer > 0;

            if (_sprites.TryGetValue(enemyKeys[keyIdx], out var src))
            {
                if (enemy.IsDying || isSlowed)
                {
                    using var tmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                    using var tg = Graphics.FromImage(tmp);
                    tg.InterpolationMode = InterpolationMode.NearestNeighbor;
                    tg.DrawImage(src, 0, 0, size, size);

                    if (enemy.IsDying)
                    {
                        using var fb = new SolidBrush(Color.FromArgb((int)((1f - alpha) * 255), 0, 0, 0));
                        tg.FillRectangle(fb, 0, 0, size, size);
                    }
                    if (isSlowed)
                    {
                        using var sb = new SolidBrush(Color.FromArgb(80, 100, 180, 255));
                        tg.FillRectangle(sb, 0, 0, size, size);
                    }

                    g.DrawImage(tmp, ex, ey);
                }
                else
                {
                    g.DrawImage(src, ex, ey, size, size);
                }
            }

            // Полоска здоровья
            if (!enemy.IsDying)
            {
                float hpPercent = Math.Max(0, (float)(enemy.HP / enemy.MaxHP));
                int barW = size - 2;
                g.FillRectangle(BlackBrush, (float)enemy.X + 1, (float)enemy.Y - 7, barW, 4);
                var hpColor = hpPercent > 0.5f ? Brushes.LimeGreen :
                               hpPercent > 0.25f ? Brushes.Orange : Brushes.Red;
                g.FillRectangle(hpColor, (float)enemy.X + 1, (float)enemy.Y - 7, barW * hpPercent, 4);

                if (enemy.Type == EnemyType.Boss)
                    g.DrawRectangle(new Pen(Color.Gold, 1), (float)enemy.X + 1, (float)enemy.Y - 7, barW, 4);
            }
        }
    }

    private void DrawProjectiles(Graphics g, GameController game)
    {
        foreach (var proj in game.Projectiles)
        {
            // Трейл
            using var trailPen = new Pen(Color.FromArgb(80, 255, 180, 60), 5);
            g.DrawLine(trailPen, (float)proj.PrevX, (float)proj.PrevY, (float)proj.X, (float)proj.Y);
            using var trailPen2 = new Pen(Color.FromArgb(40, 255, 200, 80), 3);
            g.DrawLine(trailPen2, (float)proj.PrevX, (float)proj.PrevY, (float)proj.X, (float)proj.Y);

            using var glowBrush = new SolidBrush(Color.FromArgb(180, 255, 200, 80));
            g.FillEllipse(glowBrush, (float)proj.X - 6, (float)proj.Y - 6, 12, 12);
            g.FillEllipse(ProjBrush, (float)proj.X - 4, (float)proj.Y - 4, 8, 8);
            g.FillEllipse(Brushes.White, (float)proj.X - 1.5f, (float)proj.Y - 1.5f, 3, 3);
        }
    }

    private void DrawHitEffects(Graphics g, GameController game)
    {
        foreach (var effect in game.ActiveEffects)
        {
            float progress = 1f - (float)(effect.Timer / effect.MaxTimer);
            float radius = (float)(effect.MaxRadius * progress);
            int alpha = (int)(200 * (1f - progress));
            if (alpha < 0) alpha = 0;

            using var flash = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255));
            using var ringPen = new Pen(Color.FromArgb(alpha / 2, 255, 200, 100), 2);
            g.FillEllipse(flash, (float)effect.X - radius, (float)effect.Y - radius, radius * 2, radius * 2);
            g.DrawEllipse(ringPen, (float)effect.X - radius, (float)effect.Y - radius, radius * 2, radius * 2);
        }
    }

    // Три состояния: зелёный (есть энергия), жёлтый (нужен генератор), красный (занято/вне пути)
    private void DrawPlacementGhost(Graphics g, GameController game, int cellSize, Point mousePos,
        TowerType selectedTower, int offsetX, int offsetY, int topHudHeight, Rectangle bottomPanel)
    {
        int hX = (mousePos.X - offsetX) / cellSize;
        int hY = (mousePos.Y - offsetY) / cellSize;

        if (bottomPanel.Contains(mousePos) || mousePos.Y <= topHudHeight ||
            hX < 0 || hX >= GameConstants.TargetCols || hY < 0 || hY >= GameConstants.TargetRows)
            return;

        bool canBuild = !game.Towers.Any(t => t.GridX == hX && t.GridY == hY)
                         && !IsOnPath(game, hX, hY)
                         && IsAdjacentToPath(game, hX, hY);

        int tX = hX * cellSize;
        int tY = hY * cellSize;
        var ghostRect = new Rectangle(tX, tY, cellSize, cellSize);

        if (!canBuild)
        {
            FillRoundedRect(g, RedGhostBrush, ghostRect, 4);
        }
        else if (selectedTower == TowerType.Generator || game.IsCellPowered(hX, hY))
        {
            // Зелёный призрак + спрайт
            FillRoundedRect(g, GreenGhostBrush, ghostRect, 4);
            int keyIdx = (int)selectedTower;
            if (keyIdx >= 0 && keyIdx < TowerSpriteKeys.Length &&
                _sprites.TryGetValue(TowerSpriteKeys[keyIdx], out var src))
            {
                float alpha = 0.6f;
                using var ia = new ImageAttributes();
                var cm = new ColorMatrix { Matrix33 = alpha };
                ia.SetColorMatrix(cm);
                g.DrawImage(src, new Rectangle(tX + 2, tY + 2, cellSize - 4, cellSize - 4),
                    0, 0, 32, 32, GraphicsUnit.Pixel, ia);
            }
        }
        else
        {
            FillRoundedRect(g, YellowGhostBrush, ghostRect, 4);
        }

        g.DrawRectangle(new Pen(Color.White, 1), ghostRect);
    }

    public void DrawHUD(Graphics g, GameController game, Size clientSize,
        int mouseDownX, int mouseDownY, bool isPaused, int timeScale, int topHudHeight, int cellSize)
    {
        int iconSize = cellSize - 4;
        int btnH = cellSize;
        int margin = cellSize / 2;

        // Считаем, сколько места нужно для одной строки
        int statsW = 6 * (iconSize + 4 + cellSize * 2) + iconSize * 5; // 6 статов + энерго-бар
        int btnsW = cellSize * 2 + cellSize * 3 + cellSize * 3 + cellSize * 4 + cellSize * 4 + 4 * margin;
        bool twoRows = clientSize.Width < margin * 2 + statsW + btnsW + margin * 2;

        int hudH = twoRows ? btnH * 2 + margin * 3 : btnH + margin * 2;
        var topHud = new Rectangle(0, 0, clientSize.Width, hudH);

        using var hudGrad = new LinearGradientBrush(topHud,
            Color.FromArgb(30, 30, 50), Color.FromArgb(20, 20, 35), LinearGradientMode.Vertical);
        g.FillRectangle(hudGrad, topHud);
        using var shadowPen = new Pen(Color.FromArgb(80, 0, 0, 0), 3);
        g.DrawLine(shadowPen, 0, hudH, clientSize.Width, hudH);

        if (twoRows)
        {
            // Две строки: статы сверху, кнопки снизу
            int statY = margin;
            int statSlotW = Math.Max(40, (clientSize.Width - margin * 2) / 6);
            DrawStatInSlot(g, "icon_heart", $"{game.BaseHP}", game.BaseHP <= 5 ? Brushes.Red : Brushes.White,
                margin + statSlotW * 0, statY, statSlotW, iconSize, true);
            DrawStatInSlot(g, "icon_coin", $"{game.Gold}", Brushes.Gold,
                margin + statSlotW * 1, statY, statSlotW, iconSize, true);
            DrawStatInSlot(g, "icon_wave", $"{game.Wave}/50", Brushes.White,
                margin + statSlotW * 2, statY, statSlotW, iconSize, true);
            DrawEnergyBarInSlot(g, game, margin + statSlotW * 3, statY, statSlotW, iconSize, true);
            int totalEnemies = game.Enemies.Count + game.EnemiesRemainingToSpawn;
            DrawStatInSlot(g, "icon_skull", $"{game.Enemies.Count}/{totalEnemies}", Brushes.Cyan,
                margin + statSlotW * 4, statY, statSlotW, iconSize, true);
            DrawStatInSlot(g, "icon_star", $"{game.Score}", Brushes.LightGreen,
                margin + statSlotW * 5, statY, statSlotW, iconSize, true);

            int btnY = hudH - btnH - margin;
            int btnSlotW = (clientSize.Width - margin * 2) / 5;
            string[] labels2 = { $"{timeScale}x", "Меню", "Заново", "", "ВОЛНА" };
            Color[] colors2 = {
                Color.FromArgb(40, 160, 60), Color.FromArgb(50, 50, 65), Color.FromArgb(180, 50, 50),
                isPaused ? Color.FromArgb(200, 160, 30) : Color.FromArgb(50, 50, 65), Color.FromArgb(60, 100, 200)
            };
            for (int i = 0; i < 5; i++)
            {
                int bX = margin + btnSlotW * i + 4;
                int bW = btnSlotW - 8;
                if (i == 3)
                    DrawPauseButton(g, bX, btnY, bW, btnH, colors2[i], labels2[i], isPaused, mouseDownX, mouseDownY);
                else
                    DrawHudButton(g, bX, btnY, bW, btnH, colors2[i], labels2[i], mouseDownX, mouseDownY);
            }
        }
        else
        {
            // Одна строка: 6 статов слева, 5 кнопок справа
            int statY = (hudH - iconSize) / 2;
            int rightBtnsStart = clientSize.Width - margin - btnsW;
            int leftStatsEnd = rightBtnsStart - margin;
            int statSlotW = Math.Max(40, (leftStatsEnd - margin) / 6);

            DrawStatInSlot(g, "icon_heart", $"{game.BaseHP}", game.BaseHP <= 5 ? Brushes.Red : Brushes.White,
                margin, statY, statSlotW, iconSize, false);
            DrawStatInSlot(g, "icon_coin", $"{game.Gold}", Brushes.Gold,
                margin + statSlotW, statY, statSlotW, iconSize, false);
            DrawStatInSlot(g, "icon_wave", $"{game.Wave}/50", Brushes.White,
                margin + statSlotW * 2, statY, statSlotW, iconSize, false);
            DrawEnergyBarInSlot(g, game, margin + statSlotW * 3, statY, statSlotW, iconSize, false);
            int totalEnemies = game.Enemies.Count + game.EnemiesRemainingToSpawn;
            DrawStatInSlot(g, "icon_skull", $"{game.Enemies.Count}/{totalEnemies}", Brushes.Cyan,
                margin + statSlotW * 4, statY, statSlotW, iconSize, false);
            DrawStatInSlot(g, "icon_star", $"{game.Score}", Brushes.LightGreen,
                margin + statSlotW * 5, statY, statSlotW, iconSize, false);

            int btnY2 = (hudH - btnH) / 2;
            int btnSlotW2 = btnsW / 5;
            string[] labels3 = { $"{timeScale}x", "Меню", "Заново", "", "ВОЛНА" };
            Color[] colors3 = {
                Color.FromArgb(40, 160, 60), Color.FromArgb(50, 50, 65), Color.FromArgb(180, 50, 50),
                isPaused ? Color.FromArgb(200, 160, 30) : Color.FromArgb(50, 50, 65), Color.FromArgb(60, 100, 200)
            };
            for (int i = 0; i < 5; i++)
            {
                int bX = rightBtnsStart + btnSlotW2 * i + 3;
                int bW = btnSlotW2 - 6;
                if (i == 3)
                    DrawPauseButton(g, bX, btnY2, bW, btnH, colors3[i], labels3[i], isPaused, mouseDownX, mouseDownY);
                else
                    DrawHudButton(g, bX, btnY2, bW, btnH, colors3[i], labels3[i], mouseDownX, mouseDownY);
            }
        }
    }

    private void DrawStatInSlot(Graphics g, string iconKey, string text, Brush textBrush,
        int slotX, int slotY, int slotW, int iconSize, bool twoRows)
    {
        // Измеряем общую ширину (иконка + отступ + текст)
        var textSz = g.MeasureString(text, BoldFont).ToSize();
        int totalW = iconSize + 4 + textSz.Width;
        int startX = slotX + (slotW - totalW) / 2;
        int y = twoRows ? slotY : slotY;

        if (_sprites.TryGetValue(iconKey, out var icon))
            g.DrawImage(icon, startX, y, iconSize, iconSize);
        var textRect = new Rectangle(startX + iconSize + 4, y, textSz.Width + 10, iconSize);
        using var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        g.DrawString(text, BoldFont, textBrush, textRect, sf);
    }

    private void DrawEnergyBarInSlot(Graphics g, GameController game, int slotX, int slotY, int slotW, int iconSize, bool twoRows)
    {
        int barW = Math.Max(1, slotW - iconSize - 4 - 8);
        int barH = iconSize;
        int startX = slotX + (slotW - iconSize - 4 - barW) / 2;
        int y = slotY;

        if (_sprites.TryGetValue("icon_energy", out var icon))
            g.DrawImage(icon, startX, y, iconSize, iconSize);

        int barX = startX + iconSize + 4;
        var bgRect = new Rectangle(barX, y, barW, barH);
        g.FillRectangle(BlackBrush, bgRect);

        if (game.EnergyProduction > 0 && barW > 1)
        {
            float ratio = Math.Min(1f, (float)game.EnergyConsumption / game.EnergyProduction);
            int fillW = Math.Max(1, (int)(barW * ratio));
            var fillRect = new Rectangle(barX, y, fillW, barH);
            using var barBrush = game.EnergyConsumption <= game.EnergyProduction
                ? new LinearGradientBrush(fillRect, Color.FromArgb(40, 200, 80), Color.FromArgb(20, 140, 50), LinearGradientMode.Vertical)
                : (Brush)new LinearGradientBrush(fillRect, Color.FromArgb(240, 60, 60), Color.FromArgb(160, 30, 30), LinearGradientMode.Vertical);
            g.FillRectangle(barBrush, fillRect);
        }

        g.DrawRectangle(new Pen(Color.FromArgb(100, 255, 255, 255), 1), bgRect);
        using var esf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString($"{game.EnergyConsumption}/{game.EnergyProduction}", SmallFont, Brushes.White, bgRect, esf);
    }

    private void DrawStatWithIcon(Graphics g, string iconKey, string text, Brush textBrush, int x, int y, int iconSize)
    {
        if (_sprites.TryGetValue(iconKey, out var icon))
            g.DrawImage(icon, x, y, iconSize, iconSize);

        // Текст рядом с иконкой, вертикально по центру
        var textRect = new Rectangle(x + iconSize + 4, y, iconSize * 3, iconSize);
        using var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        g.DrawString(text, BoldFont, textBrush, textRect, sf);
    }

    private void DrawEnergyBar(Graphics g, GameController game, int x, int y, int iconSize)
    {
        if (_sprites.TryGetValue("icon_energy", out var icon))
            g.DrawImage(icon, x, y, iconSize, iconSize);

        int barX = x + iconSize + 6;
        int barW = iconSize * 5;
        int barH = iconSize;
        var bgRect = new Rectangle(barX, y, barW, barH);
        g.FillRectangle(BlackBrush, bgRect);

        if (game.EnergyProduction > 0)
        {
            float ratio = Math.Min(1f, (float)game.EnergyConsumption / game.EnergyProduction);
            int fillW = (int)(barW * ratio);
            var fillRect = new Rectangle(barX, y, fillW, barH);

            using var barBrush = game.EnergyConsumption <= game.EnergyProduction
                ? new LinearGradientBrush(fillRect, Color.FromArgb(40, 200, 80), Color.FromArgb(20, 140, 50),
                    LinearGradientMode.Vertical)
                : (Brush)new LinearGradientBrush(fillRect, Color.FromArgb(240, 60, 60), Color.FromArgb(160, 30, 30),
                    LinearGradientMode.Vertical);
            g.FillRectangle(barBrush, fillRect);
        }

        g.DrawRectangle(new Pen(Color.FromArgb(100, 255, 255, 255), 1), bgRect);
        using var esf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString($"{game.EnergyConsumption}/{game.EnergyProduction}", SmallFont, Brushes.White, bgRect, esf);
    }

    public void DrawBottomPanel(Graphics g, Size clientSize, TowerType selectedTower, int bottomPanelY, int cellSize)
    {
        int panelH = cellSize * 3;
        int gap = Math.Max(6, cellSize / 4);
        int cardW = clientSize.Width - cellSize;
        int btnW = (cardW - gap * 4) / 5;
        int btnH = panelH - 10;
        int totalW = 5 * btnW + 4 * gap;

        var bottomCard = new Rectangle(cellSize / 2, bottomPanelY + 5, cardW, panelH);

        using var panelGrad = new LinearGradientBrush(bottomCard,
            Color.FromArgb(35, 35, 55), Color.FromArgb(25, 25, 40), LinearGradientMode.Vertical);
        FillRoundedRect(g, panelGrad, bottomCard, 12);
        g.DrawPath(new Pen(Color.FromArgb(100, 60, 60, 100), 2), GetRoundedRect(bottomCard, 12));

        TowerType[] types = (TowerType[])Enum.GetValues(typeof(TowerType));
        int startX = bottomCard.X + (bottomCard.Width - totalW) / 2;

        int spriteSz = Math.Min(Math.Min(cellSize * 2 - 4, btnH - 4), btnW / 3);

        for (int i = 0; i < types.Length; i++)
        {
            int btnX = startX + i * (btnW + gap);
            int btnY = bottomCard.Y + 5;
            var btnRect = new Rectangle(btnX, btnY, btnW, btnH);

            bool isSelected = selectedTower == types[i];
            using var btnBrush = new LinearGradientBrush(btnRect,
                isSelected ? Color.FromArgb(90, 70, 230) : Color.FromArgb(55, 55, 70),
                isSelected ? Color.FromArgb(60, 40, 180) : Color.FromArgb(40, 40, 55),
                LinearGradientMode.Vertical);
            FillRoundedRect(g, btnBrush, btnRect, 8);

            // Спрайт башни
            if (i < TowerSpriteKeys.Length && _sprites.TryGetValue(TowerSpriteKeys[i], out var sprite))
                g.DrawImage(sprite, btnX + 5, btnY + 5, spriteSz, spriteSz);

            // Текст справа от иконки
            int textX = btnX + spriteSz + 8;
            int textW = btnW - spriteSz - 10;
            using var nameSf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(TowerRuNames[i], SmallFont, Brushes.White,
                new Rectangle(textX, btnY, textW, btnH / 2), nameSf);
            // Монетка + жёлтая цена
            int coinSz = btnH / 3;
            if (_sprites.TryGetValue("icon_coin", out var coinSpr))
                g.DrawImage(coinSpr, textX, btnY + btnH / 2 + (btnH / 2 - coinSz) / 2, coinSz, coinSz);
            g.DrawString($"{TowerStats.GetCost(types[i], 1)}", SmallFont, Brushes.Gold,
                new Rectangle(textX + coinSz + 3, btnY + btnH / 2, textW - coinSz - 3, btnH / 2), nameSf);

            if (isSelected)
            {
                g.DrawPath(new Pen(Color.Gold, 2), GetRoundedRect(btnRect, 8));
                using var glow = new SolidBrush(Color.FromArgb(30, 255, 215, 0));
                FillRoundedRect(g, glow, new Rectangle(btnX - 3, btnY - 3, btnW + 6, btnH + 6), 11);
            }
        }
    }

    public void DrawTowerInfoPanel(Graphics g, Tower tower, Size clientSize,
        int cellSize, int offsetX, int offsetY, int mouseDownX, int mouseDownY, int topHudHeight)
    {
        int panelW = 190;
        int panelH = 280;
        // Центрируем по вертикали
        int panelY = (clientSize.Height - topHudHeight - 100) / 2 + topHudHeight - panelH / 2;
        var rightPanel = new Rectangle(clientSize.Width - panelW - 10, panelY, panelW, panelH);

        g.FillRectangle(new SolidBrush(Color.FromArgb(80, 0, 0, 0)),
            rightPanel.X + 5, rightPanel.Y + 5, rightPanel.Width, rightPanel.Height);

        using var panelGrad = new LinearGradientBrush(rightPanel,
            Color.FromArgb(40, 40, 65), Color.FromArgb(25, 25, 45), LinearGradientMode.Vertical);
        FillRoundedRect(g, panelGrad, rightPanel, 10);
        g.DrawPath(new Pen(Color.FromArgb(150, 70, 70, 130), 2), GetRoundedRect(rightPanel, 10));

        int px = rightPanel.X + 10;
        int py = rightPanel.Y + 8;

        int spriteKeyIdx = (int)tower.Type;
        if (spriteKeyIdx >= 0 && spriteKeyIdx < TowerSpriteKeys.Length && _sprites.TryGetValue(TowerSpriteKeys[spriteKeyIdx], out var sprite))
            g.DrawImage(sprite, px + (panelW - 60) / 2, py, 48, 48);

        py += 52;
        g.DrawString(TowerRuNames[spriteKeyIdx], BoldFont, Brushes.White,
            new Rectangle(px, py, panelW - 20, 18), CenterAlign);
        py += 20;
        g.DrawString($"Ур.{tower.Level}  Урон:{TowerStats.GetDamage(tower.Type, tower.Level):F0}",
            SmallFont, Brushes.LightGreen, new Rectangle(px, py, panelW - 20, 15), CenterAlign);
        py += 17;
        g.DrawString($"Радиус:{TowerStats.GetRange(tower.Type, tower.Level):F1}",
            SmallFont, Brushes.Cyan, new Rectangle(px, py, panelW - 20, 15), CenterAlign);
        py += 17;
        int energyVal = -TowerStats.GetEnergy(tower.Type, tower.Level);
        string energyText = tower.Type == TowerType.Generator ? $"+{TowerStats.GetEnergy(tower.Type, tower.Level)}" : $"{energyVal}";
        g.DrawString($"Энергия: {energyText}",
            SmallFont, Brushes.Yellow, new Rectangle(px, py, panelW - 20, 15), CenterAlign);
        py += 22;

        int btnW = panelW - 20;
        int btnH = 32;
        var btnUpg = new Rectangle(px, py, btnW, btnH);
        DrawInfoButton(g, btnUpg, "УЛУЧШИТЬ", tower.Level < 3, mouseDownX, mouseDownY);
        py += btnH + 6;

        // Продажа
        int sellPrice = (int)(TowerStats.GetCost(tower.Type, tower.Level) * 0.7);
        int sellH = 48;
        var btnSell = new Rectangle(px, py, btnW, sellH);
        DrawInfoButton(g, btnSell, "", true, mouseDownX, mouseDownY);
        g.DrawString("ПРОДАТЬ", SmallFont, Brushes.White,
            new Rectangle(btnSell.X, btnSell.Y + 4, btnSell.Width, 16), CenterAlign);
        // Монетка + цена
        var priceText = $"+{sellPrice}";
        var priceSz = g.MeasureString(priceText, BoldFont).ToSize();
        int totalW2 = 16 + 4 + priceSz.Width;
        int pStartX = btnSell.X + (btnSell.Width - totalW2) / 2;
        if (_sprites.TryGetValue("icon_coin", out var coinSpr2))
            g.DrawImage(coinSpr2, pStartX, btnSell.Y + 24, 16, 16);
        g.DrawString(priceText, BoldFont, Brushes.Gold,
            new PointF(pStartX + 20, btnSell.Y + 24));

        py += sellH + 6;

        if (tower.Type != TowerType.Generator)
        {
            string prioText = tower.Priority switch
            {
                TargetPriority.First => "ЦЕЛЬ: ПЕРВЫЙ",
                TargetPriority.Strongest => "ЦЕЛЬ: СИЛЬНЫЙ",
                TargetPriority.Closest => "ЦЕЛЬ: БЛИЖНИЙ",
                _ => "ЦЕЛЬ: ПЕРВЫЙ"
            };
            var btnPrio = new Rectangle(px, py, btnW, btnH);
            DrawInfoButton(g, btnPrio, prioText, true, mouseDownX, mouseDownY);
        }

        // Круг радиуса
        float rng = (float)TowerStats.GetRange(tower.Type, tower.Level) * cellSize;
        float cX = tower.GridX * cellSize + cellSize / 2f + offsetX;
        float cY = tower.GridY * cellSize + cellSize / 2f + offsetY;
        using var rangePen2 = new Pen(Color.FromArgb(120, 56, 189, 248), 2) { DashStyle = DashStyle.Dash };
        g.DrawEllipse(rangePen2, cX - rng, cY - rng, rng * 2, rng * 2);
    }

    private void DrawInfoButton(Graphics g, Rectangle rect, string text, bool enabled, int mouseX, int mouseY)
    {
        var drawRect = rect.Contains(mouseX, mouseY)
            ? new Rectangle(rect.X, rect.Y + 2, rect.Width, rect.Height) : rect;

        using var bg = new SolidBrush(enabled
            ? Color.FromArgb(60, 120, 200)
            : Color.FromArgb(50, 50, 60));
        FillRoundedRect(g, bg, drawRect, 5);
        g.DrawPath(new Pen(Color.FromArgb(150, 100, 100, 200)), GetRoundedRect(drawRect, 5));
        g.DrawString(text, SmallFont, enabled ? Brushes.White : Brushes.Gray, drawRect, CenterAlign);
    }

    public void DrawGameOver(Graphics g, GameController game, Size clientSize, out Rectangle restartBtn)
    {
        bool won = game.BaseHP > 0;
        g.FillRectangle(new SolidBrush(Color.FromArgb(200, won ? 10 : 40, won ? 40 : 10, won ? 10 : 40)),
            0, 0, clientSize.Width, clientSize.Height);

        string msg = won ? "ПОБЕДА!" : "БАЗА РАЗРУШЕНА";
        // Строго по центру
        var ts = g.MeasureString(msg, HugeFont).ToSize();
        DrawTextWithShadow(g, msg, HugeFont, won ? Color.Gold : Color.Red,
            clientSize.Width / 2 - ts.Width / 2, clientSize.Height / 2 - 120);

        if (won)
        {
            var cc = new[] { Color.Red, Color.Gold, Color.Cyan, Color.Lime, Color.Magenta, Color.Orange };
            for (int i = 0; i < 50; i++)
            {
                using var cb = new SolidBrush(cc[_starRng.Next(cc.Length)]);
                g.FillRectangle(cb, _starRng.Next(clientSize.Width), _starRng.Next(clientSize.Height),
                    _starRng.Next(3, 8), _starRng.Next(3, 8));
            }
        }

        var subRect = new Rectangle(0, clientSize.Height / 2 - 50, clientSize.Width, 80);
        g.DrawString($"Заработано гемов: {game.GemsEarnedThisRun}\nВсего гемов: {GlobalProgress.Gems}",
            HeaderFont, Brushes.Cyan, subRect, CenterAlign);

        int btnW = 220;
        int btnH = 55;
        restartBtn = new Rectangle(clientSize.Width / 2 - btnW / 2, clientSize.Height / 2 + 50, btnW, btnH);
        using var btnBrush2 = new LinearGradientBrush(restartBtn,
            Color.FromArgb(70, 70, 200), Color.FromArgb(40, 40, 140), LinearGradientMode.Vertical);
        FillRoundedRect(g, btnBrush2, restartBtn, 10);
        g.DrawPath(new Pen(Color.White, 2), GetRoundedRect(restartBtn, 10));
        g.DrawString("В МЕНЮ", BtnFont, Brushes.White, restartBtn, CenterAlign);
    }

    public void DrawPauseOverlay(Graphics g, Size clientSize)
    {
        g.FillRectangle(new SolidBrush(Color.FromArgb(180, 10, 10, 20)), 0, 0, clientSize.Width, clientSize.Height);

        int panelW = clientSize.Width / 3;
        int panelH = clientSize.Height / 5;
        int px = clientSize.Width / 2 - panelW / 2;
        int py = clientSize.Height / 2 - panelH / 2;
        var panel = new Rectangle(px, py, panelW, panelH);

        using var splashBrush = new LinearGradientBrush(panel,
            Color.FromArgb(50, 50, 80), Color.FromArgb(30, 30, 55), LinearGradientMode.Vertical);
        FillRoundedRect(g, splashBrush, panel, 15);
        g.DrawPath(new Pen(Color.FromArgb(200, 100, 100, 200), 2), GetRoundedRect(panel, 15));

        var sz = g.MeasureString("ПАУЗА", HeaderFont).ToSize();
        DrawTextWithShadow(g, "ПАУЗА", HeaderFont, Color.White,
            clientSize.Width / 2 - sz.Width / 2, py + panelH / 3);

        var sz2 = g.MeasureString("Пробел — продолжить", SmallFont).ToSize();
        g.DrawString("Пробел — продолжить", SmallFont, Brushes.Gray,
            clientSize.Width / 2 - sz2.Width / 2, py + panelH * 2 / 3);
    }

    private void DrawHudButton(Graphics g, int x, int y, int w, int h, Color bgColor,
        string text, int mouseX, int mouseY)
    {
        var rect = new Rectangle(x, y, w, h);
        var drawRect = rect.Contains(mouseX, mouseY)
            ? new Rectangle(rect.X, rect.Y + 2, rect.Width, rect.Height) : rect;

        using var brush = new LinearGradientBrush(drawRect, bgColor,
            Color.FromArgb(bgColor.R / 2, bgColor.G / 2, bgColor.B / 2), LinearGradientMode.Vertical);
        FillRoundedRect(g, brush, drawRect, 6);
        g.DrawPath(new Pen(Color.FromArgb(150, 255, 255, 255), 1), GetRoundedRect(drawRect, 6));
        g.DrawString(text, BoldFont, Brushes.White, drawRect, CenterAlign);
    }

    private void DrawPauseButton(Graphics g, int x, int y, int w, int h, Color bgColor,
        string text, bool isPaused, int mouseX, int mouseY)
    {
        var rect = new Rectangle(x, y, w, h);
        var drawRect = rect.Contains(mouseX, mouseY)
            ? new Rectangle(rect.X, rect.Y + 2, rect.Width, rect.Height) : rect;

        using var brush = new LinearGradientBrush(drawRect, bgColor,
            Color.FromArgb(bgColor.R / 2, bgColor.G / 2, bgColor.B / 2), LinearGradientMode.Vertical);
        FillRoundedRect(g, brush, drawRect, 6);
        g.DrawPath(new Pen(Color.FromArgb(150, 255, 255, 255), 1), GetRoundedRect(drawRect, 6));

        string spriteKey = isPaused ? "ui_play" : "ui_pause";
        int iconSz = h * 2 / 3;
        int sx = drawRect.X + (drawRect.Width - iconSz) / 2;
        int sy = drawRect.Y + (drawRect.Height - iconSz) / 2;
        if (_sprites.TryGetValue(spriteKey, out var icon))
            g.DrawImage(icon, sx, sy, iconSz, iconSz);
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

    private static GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
    {
        int minDim = Math.Min(bounds.Width, bounds.Height);
        if (radius * 2 > minDim) radius = minDim / 2;
        var path = new GraphicsPath();
        if (radius <= 0) { if (bounds.Width > 0 && bounds.Height > 0) path.AddRectangle(bounds); return path; }
        int d = radius * 2;
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static void FillRoundedRect(Graphics g, Brush brush, Rectangle bounds, int radius)
    {
        using var path = GetRoundedRect(bounds, radius);
        g.FillPath(brush, path);
    }

    // Главное меню: totalH = 60+20+65*4+80*3 = 580
    private static int MainMenuStartY(Size sz) => (sz.Height - 580) / 2 + 80;
    public static Rectangle GetMainMenuStartBtn(Size clientSize) => new(clientSize.Width / 2 - 160, MainMenuStartY(clientSize), 320, 65);
    public static Rectangle GetMainMenuTechBtn(Size clientSize) => new(clientSize.Width / 2 - 160, MainMenuStartY(clientSize) + 80, 320, 65);
    public static Rectangle GetMainMenuRecordsBtn(Size clientSize) => new(clientSize.Width / 2 - 160, MainMenuStartY(clientSize) + 160, 320, 65);
    public static Rectangle GetMainMenuExitBtn(Size clientSize) => new(clientSize.Width / 2 - 160, MainMenuStartY(clientSize) + 240, 320, 65);
    public static Rectangle GetLevelBtn(Size clientSize, int levelIndex)
        => new(clientSize.Width / 2 - 160, 220 + levelIndex * 85, 320, 65);
    public static Rectangle GetLevelBackBtn(Size clientSize) => new(clientSize.Width / 2 - 160, 480, 320, 65);
    public static Rectangle GetTechDmgBtn(Size clientSize) => new(clientSize.Width / 2 - 220, 240, 440, 100);
    public static Rectangle GetTechGenBtn(Size clientSize) => new(clientSize.Width / 2 - 220, 355, 440, 100);
    public static Rectangle GetTechHpBtn(Size clientSize) => new(clientSize.Width / 2 - 220, 470, 440, 100);
    public static Rectangle GetTechBackBtn(Size clientSize) => new(clientSize.Width / 2 - 160, 590, 320, 65);
    public static Rectangle GetRecordsBackBtn(Size clientSize) => new(clientSize.Width / 2 - 160, 500, 320, 65);
    // Позиции кнопок HUD для хит-тестов. Дублирует раскладку из DrawHUD.
    private static (int hudH, bool twoRows, int margin, int btnH, int btnsW, int rightBtnsStart, int btnSlotW)
        CalcHudLayout(Size clientSize, int cellSize)
    {
        int iconSize = cellSize - 4;
        int btnH = cellSize;
        int margin = cellSize / 2;
        int statsW = 6 * (iconSize + 4 + cellSize * 2) + iconSize * 5;
        int btnsW = cellSize * 2 + cellSize * 3 + cellSize * 3 + cellSize * 4 + cellSize * 4 + 4 * margin;
        bool twoRows = clientSize.Width < margin * 2 + statsW + btnsW + margin * 2;
        int hudH = twoRows ? btnH * 2 + margin * 3 : btnH + margin * 2;
        int rightBtnsStart = clientSize.Width - margin - btnsW;
        int btnSlotW = btnsW / 5;
        return (hudH, twoRows, margin, btnH, btnsW, rightBtnsStart, btnSlotW);
    }

    public static Rectangle GetSpeedBtn(Size clientSize, int topHudHeight, int cellSize)
    { var L = CalcHudLayout(clientSize, cellSize); int y = L.twoRows ? L.hudH - L.btnH - L.margin : (L.hudH - L.btnH) / 2; int x = L.twoRows ? L.margin + 4 : L.rightBtnsStart + 3; return new(x, y, L.btnSlotW - 8, L.btnH); }
    public static Rectangle GetExitToMenuBtn(Size clientSize, int topHudHeight, int cellSize)
    { var L = CalcHudLayout(clientSize, cellSize); int y = L.twoRows ? L.hudH - L.btnH - L.margin : (L.hudH - L.btnH) / 2; int x = L.twoRows ? L.margin + L.btnSlotW + 4 : L.rightBtnsStart + L.btnSlotW + 3; return new(x, y, L.btnSlotW - 8, L.btnH); }
    public static Rectangle GetRestartBtn(Size clientSize, int topHudHeight, int cellSize)
    { var L = CalcHudLayout(clientSize, cellSize); int y = L.twoRows ? L.hudH - L.btnH - L.margin : (L.hudH - L.btnH) / 2; int x = L.twoRows ? L.margin + L.btnSlotW * 2 + 4 : L.rightBtnsStart + L.btnSlotW * 2 + 3; return new(x, y, L.btnSlotW - 8, L.btnH); }
    public static Rectangle GetPauseBtn(Size clientSize, int topHudHeight, int cellSize)
    { var L = CalcHudLayout(clientSize, cellSize); int y = L.twoRows ? L.hudH - L.btnH - L.margin : (L.hudH - L.btnH) / 2; int x = L.twoRows ? L.margin + L.btnSlotW * 3 + 4 : L.rightBtnsStart + L.btnSlotW * 3 + 3; return new(x, y, L.btnSlotW - 8, L.btnH); }
    public static Rectangle GetStartWaveBtn(Size clientSize, int topHudHeight, int cellSize)
    { var L = CalcHudLayout(clientSize, cellSize); int y = L.twoRows ? L.hudH - L.btnH - L.margin : (L.hudH - L.btnH) / 2; int x = L.twoRows ? L.margin + L.btnSlotW * 4 + 4 : L.rightBtnsStart + L.btnSlotW * 4 + 3; return new(x, y, L.btnSlotW - 8, L.btnH); }
    public static Rectangle GetTowerBtn(Size clientSize, int index, int bottomPanelY, int cellSize)
    {
        int panelH = cellSize * 3;
        int gap = Math.Max(6, cellSize / 4);
        int cardW = clientSize.Width - cellSize;
        int btnW = (cardW - gap * 4) / 5;
        int totalW = 5 * btnW + 4 * gap;
        int sx = cellSize / 2 + (cardW - totalW) / 2;
        return new Rectangle(sx + index * (btnW + gap), bottomPanelY + 5 + 5, btnW, panelH - 10);
    }
    private static int TowerPanelY(Size clientSize, int topHudHeight)
        => (clientSize.Height - topHudHeight - 100) / 2 + topHudHeight - 140;
    private static int TowerBtnY(Size clientSize, int topHudHeight, int btnIndex)
    {
        int baseY = TowerPanelY(clientSize, topHudHeight) + 136;
        // Upgrade = +0, Sell = +38 (32+6), Priority = +92 (38+48+6)
        return btnIndex switch { 0 => baseY, 1 => baseY + 38, 2 => baseY + 92, _ => baseY };
    }
    public static Rectangle GetUpgradeBtn(Size clientSize, int topHudHeight)
        => new(clientSize.Width - 190, TowerBtnY(clientSize, topHudHeight, 0), 170, 32);
    public static Rectangle GetSellBtn(Size clientSize, int topHudHeight)
        => new(clientSize.Width - 190, TowerBtnY(clientSize, topHudHeight, 1), 170, 48);
    public static Rectangle GetPriorityBtn(Size clientSize, int topHudHeight)
        => new(clientSize.Width - 190, TowerBtnY(clientSize, topHudHeight, 2), 170, 32);
    public static Rectangle GetGameOverRestartBtn(Size clientSize)
        => new(clientSize.Width / 2 - 110, clientSize.Height / 2 + 50, 220, 55);
}
