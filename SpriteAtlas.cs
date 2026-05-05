using System.Drawing.Imaging;

namespace tower_defense;

// Процедурный пиксель-арт атлас. Генерирует спрайты 32x32

public static class SpriteAtlas
{
    public static Dictionary<string, Bitmap> Sprites { get; } = new();

    // Палитра
    private static readonly Color Transparent = Color.Transparent;
    private static readonly Color DarkBg = Color.FromArgb(24, 24, 27);

    public static void Generate()
    {
        Sprites.Clear();
        GenerateTowers();
        GenerateEnemies();
        GenerateIcons();
        GenerateUI();
    }

    private static void GenerateTowers()
    {
        // Тесла 
        var tesla = NewSprite();
        // Основание
        FillRect(tesla, 12, 24, 8, 8, Color.FromArgb(60, 60, 80));
        // Катушка
        FillRect(tesla, 14, 8, 4, 16, Color.FromArgb(100, 120, 200));
        // Кольца катушки
        FillRect(tesla, 12, 12, 8, 2, Color.FromArgb(150, 170, 255));
        FillRect(tesla, 12, 17, 8, 2, Color.FromArgb(150, 170, 255));
        // Молния
        FillRect(tesla, 15, 2, 2, 6, Color.FromArgb(255, 220, 50));
        FillRect(tesla, 13, 5, 2, 3, Color.FromArgb(255, 220, 50));
        FillRect(tesla, 17, 4, 2, 2, Color.FromArgb(255, 200, 30));
        Sprites["tesla"] = tesla;

        // Арбалет
        var xbow = NewSprite();
        FillRect(xbow, 12, 22, 8, 10, Color.FromArgb(80, 60, 40));
        // Лук
        FillRect(xbow, 8, 10, 2, 12, Color.FromArgb(139, 90, 43));
        FillRect(xbow, 22, 10, 2, 12, Color.FromArgb(139, 90, 43));
        FillRect(xbow, 10, 8, 12, 2, Color.FromArgb(160, 110, 50));
        FillRect(xbow, 10, 20, 12, 2, Color.FromArgb(160, 110, 50));
        // Тетива
        FillRect(xbow, 10, 14, 12, 1, Color.FromArgb(200, 180, 150));
        // Стрела
        FillRect(xbow, 14, 10, 2, 10, Color.FromArgb(180, 160, 120));
        FillRect(xbow, 13, 6, 4, 4, Color.FromArgb(220, 60, 50));
        Sprites["crossbow"] = xbow;

        // Пушка 
        var cannon = NewSprite();
        FillRect(cannon, 12, 20, 8, 12, Color.FromArgb(70, 70, 75));
        // Ствол
        FillRect(cannon, 14, 6, 4, 14, Color.FromArgb(100, 100, 105));
        FillRect(cannon, 16, 4, 4, 4, Color.FromArgb(130, 130, 135));
        // Колёса
        FillRect(cannon, 10, 26, 4, 4, Color.FromArgb(50, 40, 30));
        FillRect(cannon, 18, 26, 4, 4, Color.FromArgb(50, 40, 30));
        // Оранжевое дуло
        FillRect(cannon, 18, 2, 2, 4, Color.FromArgb(255, 140, 30));
        Sprites["cannon"] = cannon;

        // Замедление 
        var slow = NewSprite();
        FillRect(slow, 12, 20, 8, 12, Color.FromArgb(40, 80, 120));
        FillRect(slow, 10, 14, 12, 6, Color.FromArgb(60, 130, 200));
        // Снежинка (крест)
        FillRect(slow, 15, 4, 2, 10, Color.FromArgb(180, 230, 255));
        FillRect(slow, 10, 8, 12, 2, Color.FromArgb(180, 230, 255));
        FillRect(slow, 12, 5, 2, 2, Color.FromArgb(200, 240, 255));
        FillRect(slow, 18, 5, 2, 2, Color.FromArgb(200, 240, 255));
        FillRect(slow, 12, 11, 2, 2, Color.FromArgb(200, 240, 255));
        FillRect(slow, 18, 11, 2, 2, Color.FromArgb(200, 240, 255));
        Sprites["slow"] = slow;

        // Генератор 
        var gen = NewSprite();
        FillRect(gen, 12, 18, 8, 14, Color.FromArgb(80, 70, 20));
        // Турбина
        FillRect(gen, 10, 8, 12, 10, Color.FromArgb(200, 170, 40));
        // Лопасти
        FillRect(gen, 6, 10, 20, 3, Color.FromArgb(220, 190, 50));
        FillRect(gen, 6, 15, 20, 3, Color.FromArgb(220, 190, 50));
        // Центр
        FillRect(gen, 14, 10, 4, 6, Color.FromArgb(240, 210, 60));
        // Искры
        SetPixel(gen, 10, 4, Color.FromArgb(255, 240, 100));
        SetPixel(gen, 20, 6, Color.FromArgb(255, 240, 100));
        SetPixel(gen, 8, 9, Color.FromArgb(255, 220, 60));
        SetPixel(gen, 22, 12, Color.FromArgb(255, 220, 60));
        Sprites["generator"] = gen;
    }

    private static void GenerateEnemies()
    {
        var normal = NewSprite();
        FillOval(normal, 3, 6, 26, 22, Color.FromArgb(40, 180, 60));
        FillOval(normal, 5, 8, 22, 18, Color.FromArgb(60, 210, 80));
        // Тень снизу
        FillOval(normal, 5, 20, 22, 6, Color.FromArgb(30, 150, 40));
        // Глаза
        FillRect(normal, 9, 12, 5, 6, Color.White);
        FillRect(normal, 19, 12, 5, 6, Color.White);
        FillRect(normal, 11, 14, 3, 4, Color.Black);
        FillRect(normal, 21, 14, 3, 4, Color.Black);
        Sprites["enemy_normal"] = normal;

        // Fast 
        var fast = NewSprite();
        // Вытянутое тело
        FillRect(fast, 4, 10, 24, 12, Color.FromArgb(240, 190, 30));
        FillRect(fast, 6, 11, 22, 10, Color.FromArgb(255, 220, 50));
        // Заострённый нос
        FillRect(fast, 26, 13, 5, 6, Color.FromArgb(240, 190, 30));
        FillRect(fast, 28, 14, 3, 4, Color.FromArgb(255, 220, 50));
        // Глаз
        FillRect(fast, 20, 12, 3, 3, Color.White);
        FillRect(fast, 21, 13, 2, 2, Color.Black);
        // Полосы скорости
        FillRect(fast, 2, 12, 3, 2, Color.FromArgb(200, 240, 200, 60));
        FillRect(fast, 0, 14, 5, 2, Color.FromArgb(150, 240, 200, 60));
        FillRect(fast, 2, 18, 3, 2, Color.FromArgb(200, 240, 200, 60));
        Sprites["enemy_fast"] = fast;

        // Armored 
        var armored = NewSprite();
        // Корпус
        FillRect(armored, 3, 6, 26, 22, Color.FromArgb(90, 90, 100));
        FillRect(armored, 5, 8, 22, 18, Color.FromArgb(130, 130, 140));
        // Гусеницы
        FillRect(armored, 2, 24, 28, 6, Color.FromArgb(50, 50, 55));
        FillRect(armored, 3, 23, 26, 2, Color.FromArgb(70, 70, 75));
        // Башня
        FillRect(armored, 10, 2, 12, 8, Color.FromArgb(100, 100, 110));
        // Визор 
        FillRect(armored, 20, 10, 7, 3, Color.FromArgb(255, 60, 30));
        FillRect(armored, 22, 8, 3, 2, Color.FromArgb(255, 100, 60));
        // Ствол
        FillRect(armored, 20, 2, 3, 6, Color.FromArgb(70, 70, 75));
        Sprites["enemy_armored"] = armored;

        // Boss 
        var boss = NewSprite();
        // Тело
        FillRect(boss, 5, 10, 22, 20, Color.FromArgb(180, 25, 25));
        FillRect(boss, 7, 12, 18, 16, Color.FromArgb(220, 35, 35));
        // Рога
        FillRect(boss, 4, 2, 4, 8, Color.FromArgb(50, 8, 8));
        FillRect(boss, 24, 2, 4, 8, Color.FromArgb(50, 8, 8));
        FillRect(boss, 2, 5, 4, 3, Color.FromArgb(70, 10, 10));
        FillRect(boss, 26, 5, 4, 3, Color.FromArgb(70, 10, 10));
        // Глаза 
        FillRect(boss, 9, 14, 5, 6, Color.FromArgb(255, 200, 30));
        FillRect(boss, 18, 14, 5, 6, Color.FromArgb(255, 200, 30));
        FillRect(boss, 10, 16, 3, 3, Color.Black);
        FillRect(boss, 19, 16, 3, 3, Color.Black);
        // Рот с зубами
        FillRect(boss, 11, 24, 10, 4, Color.FromArgb(80, 10, 10));
        FillRect(boss, 12, 23, 2, 2, Color.White);
        FillRect(boss, 15, 23, 2, 2, Color.White);
        FillRect(boss, 18, 23, 2, 2, Color.White);
        // Крылья
        FillRect(boss, 0, 8, 5, 14, Color.FromArgb(140, 15, 15));
        FillRect(boss, 27, 8, 5, 14, Color.FromArgb(140, 15, 15));
        Sprites["enemy_boss"] = boss;
    }

    private static void GenerateIcons()
    {
        // Сердце
        var heart = NewSprite();
        FillRect(heart, 10, 6, 4, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 18, 6, 4, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 6, 10, 4, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 14, 10, 4, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 22, 10, 4, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 4, 14, 24, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 6, 18, 20, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 8, 22, 16, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 10, 26, 12, 4, Color.FromArgb(240, 50, 50));
        FillRect(heart, 14, 30, 4, 4, Color.FromArgb(240, 50, 50));
        Sprites["icon_heart"] = heart;

        // Монета 
        var coin = NewSprite();
        FillOval(coin, 6, 4, 20, 20, Color.FromArgb(240, 180, 30));
        FillOval(coin, 8, 6, 16, 16, Color.FromArgb(255, 210, 50));
        FillRect(coin, 14, 8, 4, 12, Color.FromArgb(220, 160, 20));
        Sprites["icon_coin"] = coin;

        // Молния
        var bolt = NewSprite();
        Color b1 = Color.FromArgb(255, 240, 50); 
        Color b2 = Color.FromArgb(255, 210, 20); 
        // Вершина
        FillRect(bolt, 14, 0, 4, 4, b1);
        // Сегмент 1: вправо-вниз
        FillRect(bolt, 12, 3, 4, 5, b2);
        FillRect(bolt, 14, 5, 6, 5, b1);
        FillRect(bolt, 18, 8, 5, 5, b2);
        FillRect(bolt, 22, 11, 4, 4, b1);
        // Поворот: влево-вниз
        FillRect(bolt, 24, 13, 3, 5, b2);
        FillRect(bolt, 18, 16, 8, 5, b1);
        FillRect(bolt, 14, 19, 5, 5, b2);
        FillRect(bolt, 10, 22, 6, 5, b1);
        // Поворот: вправо-вниз
        FillRect(bolt, 8, 25, 4, 5, b2);
        FillRect(bolt, 5, 28, 10, 4, b1);
        Sprites["icon_energy"] = bolt;

        // Волна (Wave)
        var wave = NewSprite();
        FillRect(wave, 4, 14, 6, 14, Color.FromArgb(60, 140, 220));
        FillRect(wave, 12, 8, 4, 20, Color.FromArgb(60, 140, 220));
        FillRect(wave, 18, 10, 4, 18, Color.FromArgb(60, 140, 220));
        FillRect(wave, 24, 6, 4, 14, Color.FromArgb(60, 140, 220));
        // Волны сверху
        FillRect(wave, 6, 12, 6, 2, Color.FromArgb(120, 190, 250));
        FillRect(wave, 14, 8, 2, 3, Color.FromArgb(120, 190, 250));
        FillRect(wave, 22, 5, 2, 2, Color.FromArgb(120, 190, 250));
        Sprites["icon_wave"] = wave;

        // Враг 
        var skull = NewSprite();
        FillOval(skull, 3, 3, 26, 26, Color.FromArgb(220, 50, 50));
        FillOval(skull, 5, 5, 22, 22, Color.FromArgb(240, 70, 70));
        // Глаза
        FillRect(skull, 8, 10, 6, 7, Color.White);
        FillRect(skull, 18, 10, 6, 7, Color.White);
        FillRect(skull, 9, 11, 4, 5, Color.Black);
        FillRect(skull, 19, 11, 4, 5, Color.Black);
        // Брови 
        FillRect(skull, 7, 7, 7, 3, Color.Black);
        FillRect(skull, 18, 7, 7, 3, Color.Black);
        // Рот 
        FillRect(skull, 10, 20, 12, 5, Color.Black);
        FillRect(skull, 11, 21, 3, 3, Color.White);
        FillRect(skull, 18, 21, 3, 3, Color.White);
        Sprites["icon_skull"] = skull;

        // Звезда 
        var star = NewSprite();
        FillRect(star, 14, 2, 4, 4, Color.FromArgb(255, 200, 40));
        FillRect(star, 10, 6, 12, 4, Color.FromArgb(255, 200, 40));
        FillRect(star, 6, 10, 20, 4, Color.FromArgb(255, 200, 40));
        FillRect(star, 8, 14, 16, 6, Color.FromArgb(255, 200, 40));
        FillRect(star, 10, 20, 12, 4, Color.FromArgb(255, 200, 40));
        FillRect(star, 12, 10, 8, 4, Color.FromArgb(255, 240, 80));
        Sprites["icon_star"] = star;
    }

    private static void GenerateUI()
    {
        // Меч 
        var sword = NewSprite();
        FillRect(sword, 14, 2, 4, 18, Color.FromArgb(200, 200, 210));
        FillRect(sword, 12, 4, 8, 2, Color.FromArgb(240, 240, 250));
        FillRect(sword, 8, 20, 16, 3, Color.FromArgb(160, 120, 60));
        FillRect(sword, 10, 23, 12, 4, Color.FromArgb(140, 100, 40));
        FillRect(sword, 8, 27, 16, 4, Color.FromArgb(120, 80, 30));
        Sprites["ui_sword"] = sword;

        // Шестерня 
        var gear = NewSprite();
        FillOval(gear, 8, 8, 16, 16, Color.FromArgb(120, 120, 180));
        FillOval(gear, 12, 12, 8, 8, Color.FromArgb(60, 60, 100));
        // Зубцы
        for (int i = 0; i < 8; i++)
        {
            double angle = i * Math.PI / 4;
            int cx = 16 + (int)(Math.Cos(angle) * 10);
            int cy = 16 + (int)(Math.Sin(angle) * 10);
            FillRect(gear, cx - 2, cy - 2, 5, 5, Color.FromArgb(150, 150, 210));
        }
        Sprites["ui_gear"] = gear;

        // Трофей 
        var trophy = NewSprite();
        FillRect(trophy, 6, 14, 20, 4, Color.FromArgb(200, 160, 40));
        FillRect(trophy, 8, 10, 4, 8, Color.FromArgb(200, 160, 40));
        FillRect(trophy, 20, 10, 4, 8, Color.FromArgb(200, 160, 40));
        FillRect(trophy, 9, 4, 14, 8, Color.FromArgb(220, 180, 50));
        FillRect(trophy, 6, 22, 4, 10, Color.FromArgb(160, 120, 20));
        FillRect(trophy, 22, 22, 4, 10, Color.FromArgb(160, 120, 20));
        FillRect(trophy, 10, 18, 12, 6, Color.FromArgb(240, 200, 60));
        Sprites["ui_trophy"] = trophy;

        // Гем 
        var gem = NewSprite();
        FillRect(gem, 13, 2, 6, 4, Color.FromArgb(180, 100, 255)); 
        FillRect(gem, 10, 6, 12, 6, Color.FromArgb(200, 140, 255)); 
        FillRect(gem, 8, 12, 16, 10, Color.FromArgb(160, 80, 240)); 
        FillRect(gem, 10, 14, 12, 6, Color.FromArgb(200, 140, 255)); 
        FillRect(gem, 8, 22, 16, 6, Color.FromArgb(140, 60, 220)); 
        FillRect(gem, 13, 24, 6, 4, Color.FromArgb(120, 40, 200)); 
        FillRect(gem, 10, 9, 3, 3, Color.FromArgb(230, 200, 255));
        FillRect(gem, 20, 16, 3, 3, Color.FromArgb(230, 200, 255));
        Sprites["ui_gem"] = gem;

        var pause = NewSprite();
        FillRect(pause, 6, 4, 8, 24, Color.FromArgb(255, 220, 100));
        FillRect(pause, 18, 4, 8, 24, Color.FromArgb(255, 220, 100));
        Sprites["ui_pause"] = pause;

        var play = NewSprite();
        FillRect(play, 8, 4, 4, 24, Color.FromArgb(100, 220, 100));
        FillRect(play, 12, 6, 4, 20, Color.FromArgb(100, 220, 100));
        FillRect(play, 16, 8, 4, 16, Color.FromArgb(100, 220, 100));
        FillRect(play, 20, 10, 4, 12, Color.FromArgb(100, 220, 100));
        FillRect(play, 24, 12, 4, 8, Color.FromArgb(100, 220, 100));
        Sprites["ui_play"] = play;
    }

    private static Bitmap NewSprite()
    {
        var bmp = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Transparent);
        return bmp;
    }

    private static void SetPixel(Bitmap bmp, int x, int y, Color c)
    {
        if (x >= 0 && x < 32 && y >= 0 && y < 32)
            bmp.SetPixel(x, y, c);
    }

    private static void FillRect(Bitmap bmp, int x, int y, int w, int h, Color c)
    {
        for (int py = y; py < y + h && py < 32; py++)
            for (int px = x; px < x + w && px < 32; px++)
                if (px >= 0 && py >= 0)
                    bmp.SetPixel(px, py, c);
    }

    private static void FillOval(Bitmap bmp, int x, int y, int w, int h, Color c)
    {
        int cx = x + w / 2;
        int cy = y + h / 2;
        double rx = w / 2.0;
        double ry = h / 2.0;
        for (int py = y; py < y + h && py < 32; py++)
        {
            for (int px = x; px < x + w && px < 32; px++)
            {
                if (px < 0 || py < 0) continue;
                double dx = (px - cx) / rx;
                double dy = (py - cy) / ry;
                if (dx * dx + dy * dy <= 1.0)
                    bmp.SetPixel(px, py, c);
            }
        }
    }

    public static Bitmap GetScaled(string key, int size)
    {
        if (!Sprites.TryGetValue(key, out var src)) return new Bitmap(size, size);
        var scaled = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(scaled);
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        g.DrawImage(src, 0, 0, size, size);
        return scaled;
    }
}
