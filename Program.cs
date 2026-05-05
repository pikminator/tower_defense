namespace tower_defense
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            SaveManager.Load();
            SpriteAtlas.Generate();
            ApplicationConfiguration.Initialize();
            Application.Run(new GameForm());
        }
    }
}
