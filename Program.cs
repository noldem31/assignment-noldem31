using System.Diagnostics;
using Silk.NET.Maths; 
using Silk.NET.SDL;

namespace Minesweeper;

public static class Program {
    private static GameState _currentState = GameState.MainMenu;

    private static int _gridRows;
    private static int _gridCols;
    private static int _totalMines;
    private static int _windowWidth;
    private static int _windowHeight;

    private const int TileSize = 50; 
    private const int GridOffset = 150;

    public static void Main() {
        var sdl = new Sdl(new SdlContext());
        
        Span<byte> mouseButtonStates = stackalloc byte[(int)MouseButton.Count];
        
        var sdlInitResult = sdl.Init(Sdl.InitVideo | Sdl.InitAudio | Sdl.InitEvents | Sdl.InitTimer | Sdl.InitGamecontroller | Sdl.InitJoystick);
        if (sdlInitResult < 0) {
            throw new InvalidOperationException("Failed to initialize SDL.");
        }
        
        using var context = new GameContext(sdl);
        var gameplayTimer = new Stopwatch();
        var ev = new Event();

        GameLogic.Cell[,] board = null!; 
        var isFirstMove = true; 
        var gameOver = false;
        var gameWon = false;
        long finalTimeSeconds = 0;

        var quit = false;
        while (!quit) {
            var wasMouseClickedThisFrame = false;
            byte clickedButton = 0;
            var clickX = 0;
            var clickY = 0;
            
            while (sdl.PollEvent(ref ev) != 0) {
                if (ev.Type == (uint)EventType.Quit) {
                    quit = true;
                    break;
                }

                switch (ev.Type) {
                    case (uint)EventType.Windowevent: {
                        if (ev.Window.Event == (byte)WindowEventID.TakeFocus) {
                            unsafe {
                                sdl.SetWindowInputFocus(sdl.GetWindowFromID(ev.Window.WindowID));
                            }
                        }

                        break;
                    }
                    case (uint)EventType.Fingerdown: {
                        mouseButtonStates[(byte)MouseButton.Primary] = 1;
                        break;
                    }
                    case (uint)EventType.Fingerup: {
                        mouseButtonStates[(byte)MouseButton.Primary] = 0;
                        break;
                    }
                    case (uint)EventType.Mousebuttondown: {
                        mouseButtonStates[ev.Button.Button] = 1;

                        wasMouseClickedThisFrame = true;
                        clickedButton = ev.Button.Button;
                        clickX = ev.Button.X;
                        clickY = ev.Button.Y;

                        break;
                    }
                    case (uint)EventType.Mousebuttonup: {
                        mouseButtonStates[ev.Button.Button] = 0;

                        break;
                    }
                }
            }

            int mouseX;
            int mouseY;
            unsafe {sdl.GetMouseState(&mouseX, &mouseY);}
            
            if (wasMouseClickedThisFrame) {
                if (_currentState == GameState.MainMenu) {
                    if (clickedButton == (byte)MouseButton.Primary) {
                        var selectedDifficulty = MainMenuScene.HandleClick(clickX, clickY);
                        if (selectedDifficulty.HasValue) {
                            ApplyDifficulty(selectedDifficulty.Value);
                            context.CreateWindow(_windowWidth, _windowHeight);
                            
                            unsafe {
                                sdl.RaiseWindow(context.Window);
                                sdl.SetWindowInputFocus(context.Window);
                            }
                            
                            board = new GameLogic.Cell[_gridRows, _gridCols];
                            isFirstMove = true; 
                            gameOver = false; 
                            gameWon = false; 
                            finalTimeSeconds = 0;
                            gameplayTimer.Reset();
                            _currentState = GameState.Playing;
                        }
                    }
                }
                else if (_currentState == GameState.Playing) {
                    if (gameOver || gameWon) {
                        if (clickedButton == (byte)MouseButton.Primary) {
                            var result = GameScene.HandleOverlayClick(clickX, clickY, _windowWidth, _windowHeight);
                            
                            if (result == GameScene.OverlayClickResult.Restart) {
                                board = new GameLogic.Cell[_gridRows, _gridCols];
                                isFirstMove = true; 
                                gameOver = false; 
                                gameWon = false; 
                                finalTimeSeconds = 0;
                                gameplayTimer.Reset();
                                unsafe { sdl.SetCursor(context.ArrowCursor); }
                            }
                            else if (result == GameScene.OverlayClickResult.BackToMainMenu) {
                                _currentState = GameState.MainMenu;
                                context.CreateWindow(500, 500);
                                
                                unsafe {
                                    sdl.RaiseWindow(context.Window);
                                    sdl.SetWindowInputFocus(context.Window);
                                    sdl.SetCursor(context.ArrowCursor);
                                }
                            }
                        }
                    }
                    
                    else if (clickX >= GridOffset && clickX < GridOffset + (_gridCols * TileSize) && 
                             clickY >= GridOffset && clickY < GridOffset + (_gridRows * TileSize)) 
                    {
                        var col = (clickX - GridOffset) / TileSize;
                        var row = (clickY - GridOffset) / TileSize;
                        
                        if (clickedButton == (byte)MouseButton.Primary && !board[row, col].IsFlagged && !board[row, col].IsRevealed) {
                            if (isFirstMove) { 
                                GameLogic.GenerateSafeZone(board, _gridRows, _gridCols, _totalMines, row, col); 
                                isFirstMove = false; 
                                gameplayTimer.Start(); 
                            }
                            
                            GameLogic.RevealCell(board, _gridRows, _gridCols, row, col);
                            
                            if (board[row, col].IsMine) { 
                                gameOver = true; 
                                gameplayTimer.Stop(); 
                                finalTimeSeconds = gameplayTimer.ElapsedMilliseconds / 1000; 
                                GameLogic.RevealAllMines(board, _gridRows, _gridCols); 
                            }
                            else if (GameLogic.CheckWinCondition(board, _gridRows, _gridCols)) { 
                                gameWon = true; 
                                gameplayTimer.Stop(); 
                                finalTimeSeconds = gameplayTimer.ElapsedMilliseconds / 1000; 
                            }
                        }
                        else if (clickedButton == (byte)MouseButton.Secondary && !board[row, col].IsRevealed) {
                            board[row, col].IsFlagged = !board[row, col].IsFlagged;
                        }
                    }
                }
            }
            
            unsafe {
                var handCursor = _currentState == GameState.MainMenu 
                    ? MainMenuScene.IsHoveringButtons(mouseX, mouseY) 
                    : (gameOver || gameWon) && GameScene.IsHoveringOverlayButtons(mouseX, mouseY, _windowWidth, _windowHeight);
                
                sdl.SetCursor(handCursor ? context.HandCursor : context.ArrowCursor);
            }
            
            unsafe {
                var r = context.Renderer;
                sdl.SetRenderDrawColor(r, Colors.Background.R, Colors.Background.G, Colors.Background.B, 255);
                sdl.RenderClear(r);

                if (_currentState == GameState.MainMenu) {
                    MainMenuScene.Render(sdl, r, mouseX, mouseY);
                }
                else if (_currentState == GameState.Playing) {
                    var headerWidth = _gridCols * TileSize;
                    var headerBar = new Rectangle<int>(GridOffset, 40, headerWidth, 60);
    
                    sdl.SetRenderDrawColor(r, Colors.Header.R, Colors.Header.G, Colors.Header.B, 255);
                    sdl.RenderFillRect(r, &headerBar);
                    
                    var currentDisplayTime = gameOver || gameWon ? finalTimeSeconds : (gameplayTimer.ElapsedMilliseconds / 1000);
                    var timeString = $"TIME {currentDisplayTime:D3}";
                    
                    var textWidth = timeString.Length * 12;
                    var centeredX = GridOffset + (headerWidth - textWidth) / 2;
                    
                    TextRenderer.DrawVectorText(sdl, r, timeString, centeredX, 60, 2, Colors.White);
                    
                    for (var row = 0; row < _gridRows; row++) {
                        for (var col = 0; col < _gridCols; col++) {
                            var cell = board[row, col];
                            var sdlRect = new Rectangle<int>(GridOffset + (col * TileSize), GridOffset + (row * TileSize), TileSize, TileSize);
                            var activeTexture = cell.IsRevealed ? (cell.IsMine ? 10 : cell.AdjacentMines) : -1;

                            IntPtr texturePtr;
                            
                            if (activeTexture >= 0) context.TileTextureMap.TryGetValue(activeTexture, out texturePtr);
                            else if (cell.IsFlagged) texturePtr = context.FlagTexture;
                            else texturePtr = context.TileHiddenTexture;
                            
                            if (texturePtr != IntPtr.Zero) sdl.RenderCopy(r, (Texture*)texturePtr, null, &sdlRect);
                            
                            sdl.SetRenderDrawColor(r, Colors.Header.R, Colors.Header.G, Colors.Header.B, 255);
                            sdl.RenderDrawRect(r, &sdlRect);
                        }
                    }

                    if (gameOver || gameWon) {
                        GameScene.DrawEndScreenOverlay(sdl, r, gameWon, finalTimeSeconds, _windowWidth, _windowHeight);
                    }
                }
                sdl.RenderPresent(r);
            }
        }
    }

    private static void ApplyDifficulty(Difficulty difficulty) {
        (_gridRows, _gridCols, _totalMines) = difficulty.GetSettings();
        _windowWidth = (_gridCols * TileSize) + (GridOffset * 2);
        _windowHeight = (_gridRows * TileSize) + GridOffset + 60;
    }
}