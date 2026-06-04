namespace Minesweeper;

public static class GameLogic {
    public struct Cell {
        public bool IsMine;
        public bool IsRevealed;
        public bool IsFlagged;
        public int AdjacentMines;
    }

    public static void GenerateSafeZone(Cell[,] board, int rows, int cols, int mineCount, int safeRow, int safeCol) {
        var rand = new Random();
        var placedMines = 0;

        while (placedMines < mineCount) {
            var randomRow = rand.Next(rows);
            var randomCol = rand.Next(cols);
            var isInsideSafeZone = Math.Abs(randomRow - safeRow) <= 1 && Math.Abs(randomCol - safeCol) <= 1;

            if (board[randomRow, randomCol].IsMine || isInsideSafeZone) continue;
            board[randomRow, randomCol].IsMine = true;
            placedMines++;
        }

        for (var r = 0; r < rows; r++) {
            for (var c = 0; c < cols; c++) {
                if (board[r, c].IsMine) continue;
                var count = 0;
                for (var i = -1; i <= 1; i++) {
                    for (var j = -1; j <= 1; j++) {
                        var nr = r + i; var nc = c + j;
                        if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && board[nr, nc].IsMine) count++;
                    }
                }
                board[r, c].AdjacentMines = count;
            }
        }
    }

    public static void RevealCell(Cell[,] board, int rows, int cols, int row, int col) {
        if (row < 0 || row >= rows || col < 0 || col >= cols) return;
        if (board[row, col].IsRevealed || board[row, col].IsFlagged) return;

        board[row, col].IsRevealed = true;

        if (board[row, col].AdjacentMines != 0 || board[row, col].IsMine) return;
        for (var i = -1; i <= 1; i++) {
            for (var j = -1; j <= 1; j++) {
                RevealCell(board, rows, cols, row + i, col + j);
            }
        }
    }

    public static bool CheckWinCondition(Cell[,] board, int rows, int cols) {
        for (var r = 0; r < rows; r++) {
            for (var c = 0; c < cols; c++) {
                if (!board[r, c].IsMine && !board[r, c].IsRevealed) return false;
            }
        }
        return true;
    }

    public static void RevealAllMines(Cell[,] board, int rows, int cols) {
        for (var r = 0; r < rows; r++) {
            for (var c = 0; c < cols; c++) {
                if (board[r, c].IsMine) board[r, c].IsRevealed = true;
            }
        }
    }
}