namespace tower_defense;

public static class HighScores
{
    public static List<Record> Records { get; set; } = new();

    public static void Add(int wave, int score)
    {
        Records.Add(new Record
        {
            Wave = wave,
            Score = score,
            Date = DateTime.Now
        });
        Records = Records.OrderByDescending(r => r.Wave).ThenByDescending(r => r.Score).Take(5).ToList();
    }

    public static List<Record> GetTop5() => Records;
}

public class Record
{
    public int Wave { get; set; }
    public int Score { get; set; }
    public DateTime Date { get; set; }
}
