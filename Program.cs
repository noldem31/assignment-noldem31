using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Maths; 
using Silk.NET.SDL;

namespace Minesweeper;

public static class Program {
    private const int GridRows = 10;
    private const int GridCols = 10;
    private const int TotalMines = 15;
    private const int TileSize = 50; 
    private const int GridOffset = 150; 
    
    private static readonly Dictionary<int, IntPtr> TileTextureMap = new();
    private static IntPtr _flagTexture;
    private static IntPtr _tileHiddenTexture;
    
    private struct Cell {
        public bool IsMine;
        public bool IsRevealed;
        public bool IsFlagged;
        public int AdjacentMines;
    }

    // AI-generated
    [DllImport("SDL2", EntryPoint = "SDL_RWFromMem", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void* RWFromMem(void* mem, int size);

    [DllImport("SDL2", EntryPoint = "SDL_LoadBMP_RW", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Surface* LoadBMPRW(void* rwops, int freesrc);
    // end AI-generated

    public static void Main() {
        var sdl = new Sdl(new SdlContext());
        var timer = new Stopwatch();
        
        ReadOnlySpan<byte> keyboardState;
        unsafe
        {
            keyboardState = new(sdl.GetKeyboardState(null), (int)KeyCode.Count);
        }

        Span<byte> mouseButtonStates = stackalloc byte[(int)MouseButton.Count];

        var ev = new Event();

        var sdlInitResult = sdl.Init(Sdl.InitVideo | Sdl.InitAudio | Sdl.InitEvents | Sdl.InitTimer | Sdl.InitGamecontroller |
                                     Sdl.InitJoystick);
        if (sdlInitResult < 0)
        {
            throw new InvalidOperationException("Failed to initialize SDL.");
        }

        IntPtr window;
        unsafe
        {
            window = (IntPtr)sdl.CreateWindow(
                "Minesweeper", Sdl.WindowposUndefined, Sdl.WindowposUndefined, 800, 800,
                (uint)WindowFlags.Resizable | (uint)WindowFlags.AllowHighdpi
            );

            if (window == IntPtr.Zero)
            {
                var ex = sdl.GetErrorAsException();
                if (ex != null)
                {
                    throw ex;
                }

                throw new Exception("Failed to create window.");
            }
        }

        IntPtr renderer;
        unsafe
        {
            renderer = (IntPtr)sdl.CreateRenderer((Window*)window, -1, (uint)RendererFlags.Accelerated);
            sdl.RenderSetVSync((Renderer*)renderer, 1);
            
            InitializeTextures((Renderer*)renderer, sdl);
        }

        if (renderer == IntPtr.Zero)
        {
            var ex = sdl.GetErrorAsException();
            if (ex != null) {
                throw ex;
            }

            throw new Exception("Failed to create renderer.");
        }
        
        var board = new Cell[GridRows, GridCols];
        var isFirstMove = true; 
        var gameOver = false;
        var gameWon = false;

        var quit = false;
        while (!quit) {
            while (sdl.PollEvent(ref ev) != 0) {
                if (ev.Type == (uint)EventType.Quit) {
                    quit = true;
                    break;
                }

                switch (ev.Type) {
                    case (uint)EventType.Mousebuttondown: {
                        if (gameOver || gameWon) break;

                        var mouseX = ev.Button.X;
                        var mouseY = ev.Button.Y;

                        if (mouseX >= GridOffset && mouseX < GridOffset + (GridCols * TileSize) &&
                            mouseY >= GridOffset && mouseY < GridOffset + (GridRows * TileSize))
                        {
                            var col = (mouseX - GridOffset) / TileSize;
                            var row = (mouseY - GridOffset) / TileSize;

                            if (ev.Button.Button == (byte)MouseButton.Primary) {
                                if (!board[row, col].IsFlagged && !board[row, col].IsRevealed) {
                                    if (isFirstMove) {
                                        GenerateSafeZone(board, GridRows, GridCols, TotalMines, row, col);
                                        isFirstMove = false;
                                    }

                                    RevealCell(board, GridRows, GridCols, row, col);
                                    
                                    if (board[row, col].IsMine) {
                                        gameOver = true;
                                        RevealAllMines(board, GridRows, GridCols);
                                        Console.WriteLine("Game Over");
                                    }
                                    else if (CheckWinCondition(board, GridRows, GridCols)) {
                                        gameWon = true;
                                        Console.WriteLine("You won");
                                    }
                                }
                            }
                            else if (ev.Button.Button == (byte)MouseButton.Secondary) {
                                if (!board[row, col].IsRevealed) {
                                    board[row, col].IsFlagged = !board[row, col].IsFlagged;
                                }
                            }
                        }
                        break;
                    }
                }
            }

            timer.Restart();
            
            unsafe {
                var r = (Renderer*)renderer;

                sdl.SetRenderDrawColor(r, 45, 45, 45, 255);
                sdl.RenderClear(r);

                for (var row = 0; row < GridRows; row++) {
                    for (var col = 0; col < GridCols; col++) {
                        var cell = board[row, col];
                        
                        var sdlRect = new Rectangle<int>(
                            GridOffset + (col * TileSize),
                            GridOffset + (row * TileSize),
                            TileSize,
                            TileSize
                        );

                        var activeTexture = _tileHiddenTexture;

                        if (cell.IsRevealed) {
                            if (cell.IsMine) {
                                TileTextureMap.TryGetValue(10, out activeTexture); 
                            }
                            else if (cell.AdjacentMines > 0) {
                                TileTextureMap.TryGetValue(cell.AdjacentMines, out activeTexture); 
                            }
                            else {
                                TileTextureMap.TryGetValue(0, out activeTexture); 
                            }
                        }
                        else if (cell.IsFlagged) {
                            activeTexture = _flagTexture;
                        }
                        
                        if (activeTexture != IntPtr.Zero) {
                            sdl.RenderCopy(r, (Texture*)activeTexture, null, &sdlRect);
                        }

                        sdl.SetRenderDrawColor(r, 30, 30, 30, 255);
                        sdl.RenderDrawRect(r, &sdlRect);
                    }
                }

                sdl.RenderPresent(r);
            }
        }

        unsafe {
            foreach (var tex in TileTextureMap.Values) sdl.DestroyTexture((Texture*)tex);
            sdl.DestroyTexture((Texture*)_flagTexture);
            sdl.DestroyTexture((Texture*)_tileHiddenTexture);
            sdl.DestroyWindow((Window*)window);
        }
        sdl.Quit();
    }
    
    private static void GenerateSafeZone(Cell[,] board, int rows, int cols, int mineCount, int safeRow, int safeCol) {
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
    
    private static unsafe void InitializeTextures(Renderer* renderer, Sdl sdl) {
        var assetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");

        _flagTexture = LoadTexture(sdl, renderer, Path.Combine(assetsPath, "flag.bmp"));
        _tileHiddenTexture = LoadTexture(sdl, renderer, Path.Combine(assetsPath, "tile_hidden.bmp"));

        TileTextureMap[0] = LoadTexture(sdl, renderer, Path.Combine(assetsPath, "tile_empty.bmp"));
        TileTextureMap[10] = LoadTexture(sdl, renderer, Path.Combine(assetsPath, "mine.bmp"));

        for (var i = 1; i <= 8; i++) {
            TileTextureMap[i] = LoadTexture(sdl, renderer, Path.Combine(assetsPath, $"{i}.bmp"));
        }
    }

    private static unsafe IntPtr LoadTexture(Sdl sdl, Renderer* renderer, string filename) {
        if (!File.Exists(filename)) return IntPtr.Zero;

        try {
            var fileBytes = File.ReadAllBytes(filename);
        
            fixed (byte* pBytes = fileBytes) {
                var rwops = RWFromMem(pBytes, fileBytes.Length);
                if (rwops == null) return IntPtr.Zero;
                
                var surfacePtr = LoadBMPRW(rwops, 1);
                if (surfacePtr == null) return IntPtr.Zero;
                var texture = sdl.CreateTextureFromSurface(renderer, surfacePtr);
                
                sdl.FreeSurface(surfacePtr); 

                return (IntPtr)texture;
            } 
        }
        catch {
            return IntPtr.Zero;
        }
    }
    
    private static void RevealCell(Cell[,] board, int rows, int cols, int row, int col) {
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

    private static bool CheckWinCondition(Cell[,] board, int rows, int cols) {
        for (var r = 0; r < rows; r++) {
            for (var c = 0; c < cols; c++) {
                if (!board[r, c].IsMine && !board[r, c].IsRevealed) return false;
            }
        }
        return true;
    }

    private static void RevealAllMines(Cell[,] board, int rows, int cols) {
        for (var r = 0; r < rows; r++) {
            for (var c = 0; c < cols; c++) {
                if (board[r, c].IsMine) board[r, c].IsRevealed = true;
            }
        }
    }
}