namespace Minesweeper;

public enum Difficulty {
    Easy,
    Medium,
    Hard
}

public static class DifficultySettings {
    public static (int Rows, int Cols, int Mines) GetSettings(this Difficulty mode) {
        return mode switch {
            Difficulty.Easy => (8, 8, 10),
            Difficulty.Medium => (10, 10, 15),
            Difficulty.Hard => (14, 14, 35),
            _ => (8, 8, 10)
        };
    }
}