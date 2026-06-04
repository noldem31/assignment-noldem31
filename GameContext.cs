using System.Runtime.InteropServices;
using Silk.NET.SDL;

namespace Minesweeper;

public enum GameState {
    MainMenu,
    Playing
}

public sealed unsafe partial class GameContext : IDisposable {
    private readonly Sdl _sdl;
    private bool _disposed;
    
    public Window* Window { get; private set; }
    public Renderer* Renderer { get; private set; }
    public Cursor* ArrowCursor { get; }
    public Cursor* HandCursor { get; }
    public IntPtr FlagTexture { get; private set; }
    public IntPtr TileHiddenTexture { get; private set; }
    public Dictionary<int, IntPtr> TileTextureMap { get; } = new();
    
    // AI-generated
    [LibraryImport("SDL2", EntryPoint = "SDL_RWFromMem")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static partial void* RWFromMem(void* mem, int size);
    [LibraryImport("SDL2", EntryPoint = "SDL_LoadBMP_RW")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    // end AI-generated
    
    private static partial Surface* LoadBMPRW(void* rwops, int freesrc);

    public GameContext(Sdl sdl) {
        _sdl = sdl ?? throw new ArgumentNullException(nameof(sdl));
        
        ArrowCursor = _sdl.CreateSystemCursor(SystemCursor.SystemCursorArrow);
        HandCursor = _sdl.CreateSystemCursor(SystemCursor.SystemCursorHand);
        
        CreateWindow(500, 500);
    }

    public void CreateWindow(int width, int height) {
        if (Renderer != null) _sdl.DestroyRenderer(Renderer);
        if (Window != null) _sdl.DestroyWindow(Window);

        Window = _sdl.CreateWindow(
            "Minesweeper", Sdl.WindowposUndefined, Sdl.WindowposUndefined, width, height,
            (uint)WindowFlags.AllowHighdpi
        );

        if (Window == null) {
            throw new Exception("Failed to initialize context.");
        }
        
        Renderer = _sdl.CreateRenderer(Window, -1, (uint)RendererFlags.Accelerated);
        if (Renderer == null) {
            throw new Exception("Failed to initialize context.");
        }

        _sdl.RenderSetVSync(Renderer, 1);
        InitializeTextures();
    }

    private void InitializeTextures() {
        var assetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");
        TileTextureMap.Clear();

        FlagTexture = LoadTexture(Path.Combine(assetsPath, "flag.bmp"));
        TileHiddenTexture = LoadTexture(Path.Combine(assetsPath, "tile_hidden.bmp"));
        TileTextureMap[0] = LoadTexture(Path.Combine(assetsPath, "tile_empty.bmp"));
        TileTextureMap[10] = LoadTexture(Path.Combine(assetsPath, "mine.bmp"));

        for (var i = 1; i <= 8; i++) {
            TileTextureMap[i] = LoadTexture(Path.Combine(assetsPath, $"{i}.bmp"));
        }
    }

    private IntPtr LoadTexture(string filename) {
        if (!File.Exists(filename)) return IntPtr.Zero;

        try {
            var fileBytes = File.ReadAllBytes(filename);
        
            fixed (byte* pBytes = fileBytes) {
                var rwops = RWFromMem(pBytes, fileBytes.Length);
                if (rwops == null) return IntPtr.Zero;
                
                var surfacePtr = LoadBMPRW(rwops, 1);
                if (surfacePtr == null) return IntPtr.Zero;
                var texture = _sdl.CreateTextureFromSurface(Renderer, surfacePtr);
                
                _sdl.FreeSurface(surfacePtr); 

                return (IntPtr)texture;
            } 
        }
        catch {
            return IntPtr.Zero;
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) return;
        
        if (disposing) {
            foreach (var tex in TileTextureMap.Values.Where(tex => tex != IntPtr.Zero)) {
                _sdl.DestroyTexture((Texture*)tex);
            }
        }
        
        if (FlagTexture != IntPtr.Zero) _sdl.DestroyTexture((Texture*)FlagTexture);
        if (TileHiddenTexture != IntPtr.Zero) _sdl.DestroyTexture((Texture*)TileHiddenTexture);

        if (Renderer != null) _sdl.DestroyRenderer(Renderer);
        if (Window != null) _sdl.DestroyWindow(Window);
        
        if (ArrowCursor != null) _sdl.FreeCursor(ArrowCursor);
        if (HandCursor != null) _sdl.FreeCursor(HandCursor);

        _sdl.Quit();
        _disposed = true;
    }

    ~GameContext() {
        Dispose(false);
    }
}