using Silk.NET.Maths;
using Silk.NET.SDL;

namespace Minesweeper;

public static class GameScene {
    private const int PanelW = 500;
    private const int PanelH = 300;
    private const int ButtonH = 50;

    private const int FirstButtonY = 150;
    private const int ButtonGap = 25;
    private const int ButtonPadding = 30;

    public enum OverlayClickResult {
        None,
        Restart,
        BackToMainMenu
    }

    private static int GetPanelX(int windowWidth) => (windowWidth - PanelW) / 2;  
    private static int GetPanelY(int windowHeight) => (windowHeight - PanelH) / 2;
    private static int GetButtonWidth() => PanelW - 50;
    private static int GetButtonStride() => ButtonH + ButtonGap;

    public static OverlayClickResult HandleOverlayClick(int mouseX, int mouseY, int windowWidth, int windowHeight) {
        if (IsInsideOverlayRestartButton(mouseX, mouseY, windowWidth, windowHeight)) return OverlayClickResult.Restart;
        if (IsInsideOverlayMenuButton(mouseX, mouseY, windowWidth, windowHeight)) return OverlayClickResult.BackToMainMenu;
        return OverlayClickResult.None;
    }

    public static bool IsHoveringOverlayButtons(int mouseX, int mouseY, int windowWidth, int windowHeight) {
        return IsInsideOverlayRestartButton(mouseX, mouseY, windowWidth, windowHeight) || 
               IsInsideOverlayMenuButton(mouseX, mouseY, windowWidth, windowHeight);
    }

    public static unsafe void DrawEndScreenOverlay(Sdl sdl, Renderer* r, bool win, long finalTime, int windowWidth, int windowHeight) {
        int mouseX;
        int mouseY;
        sdl.GetMouseState(&mouseX, &mouseY);
        
        var dimLayer = new Rectangle<int>(0, 0, windowWidth, windowHeight);
        sdl.SetRenderDrawBlendMode(r, BlendMode.Blend);
        sdl.SetRenderDrawColor(r, Colors.OverlayDimBackground.R, Colors.OverlayDimBackground.G, Colors.OverlayDimBackground.B, 200); 
        sdl.RenderFillRect(r, &dimLayer);

        var panelX = GetPanelX(windowWidth);
        var panelY = GetPanelY(windowHeight);
        
        var panel = new Rectangle<int>(panelX, panelY, PanelW, PanelH);
        sdl.SetRenderDrawColor(r, Colors.OverlayPanelBackground.R, Colors.OverlayPanelBackground.G, Colors.OverlayPanelBackground.B, 255);
        sdl.RenderFillRect(r, &panel);
        
        var titleBar = new Rectangle<int>(panelX, panelY, PanelW, 60);
        var overlayHeaderColor = win ? Colors.WinOverlay : Colors.LoseOverlay;
        sdl.SetRenderDrawColor(r, overlayHeaderColor.R, overlayHeaderColor.G, overlayHeaderColor.B, 255);
        sdl.RenderFillRect(r, &titleBar);
        
        var statusText = win ? "GAME WON" : "GAME OVER";
        var titleTextX = panelX + (PanelW - (statusText.Length * 18)) / 2;
        TextRenderer.DrawVectorText(sdl, r, statusText, titleTextX, panelY + 18, 3, Colors.White);
        
        var scoreText = $"TIME SPENT {finalTime} SECONDS";
        var scoreX = panelX + (PanelW - (scoreText.Length * 12)) / 2;
        TextRenderer.DrawVectorText(sdl, r, scoreText, scoreX, panelY + 100, 2, Colors.LightGray);
        
        var buttonW = GetButtonWidth();
        var buttonX = panelX + ButtonPadding;
        
        var restartBtnY = panelY + FirstButtonY; 
        var restartHovered = IsInsideOverlayRestartButton(mouseX, mouseY, windowWidth, windowHeight);
        var restartBar = new Rectangle<int>(buttonX, restartBtnY, buttonW, ButtonH);
        
        var restartColor = restartHovered ? Colors.RestartHoverBackground : Colors.RestartIdleBackground;
        sdl.SetRenderDrawColor(r, restartColor.R, restartColor.G, restartColor.B, 255);
        sdl.RenderFillRect(r, &restartBar);
        
        const string restartText = "RESTART GAME";
        var restartX = restartBar.Origin.X + (restartBar.Size.X - (restartText.Length * 12)) / 2;
        TextRenderer.DrawVectorText(sdl, r, restartText, restartX, restartBar.Origin.Y + 12, 2, Colors.White);
        
        var menuBtnY = restartBtnY + (1 * GetButtonStride());
        var menuHovered = IsInsideOverlayMenuButton(mouseX, mouseY, windowWidth, windowHeight);
        var menuBar = new Rectangle<int>(buttonX, menuBtnY, buttonW, ButtonH);
        
        var menuColor = menuHovered ? Colors.MenuHoverBackground : Colors.MenuIdleBackground;
        sdl.SetRenderDrawColor(r, menuColor.R, menuColor.G, menuColor.B, 255);
        sdl.RenderFillRect(r, &menuBar);
        
        const string menuText = "BACK TO MAIN MENU";
        var menuX = menuBar.Origin.X + (menuBar.Size.X - (menuText.Length * 12)) / 2;
        TextRenderer.DrawVectorText(sdl, r, menuText, menuX, menuBar.Origin.Y + 12, 2, Colors.White);
    }
    
    private static bool IsMouseOverButton(int mouseX, int mouseY, int buttonX, int buttonY, int buttonWidth, int buttonHeight) {
        return mouseX >= buttonX && 
               mouseX <  buttonX + buttonWidth && 
               mouseY >= buttonY && 
               mouseY <  buttonY + buttonHeight;
    }
    
    private static bool IsInsideOverlayRestartButton(int x, int y, int windowWidth, int windowHeight) {
        var buttonX = GetPanelX(windowWidth) + ButtonPadding;
        var buttonY = GetPanelY(windowHeight) + FirstButtonY;
        return IsMouseOverButton(x, y, buttonX, buttonY, GetButtonWidth(), ButtonH);
    }

    private static bool IsInsideOverlayMenuButton(int x, int y, int windowWidth, int windowHeight) {
        var buttonX = GetPanelX(windowWidth) + ButtonPadding;
        var buttonY = GetPanelY(windowHeight) + FirstButtonY + (1 * GetButtonStride());
        return IsMouseOverButton(x, y, buttonX, buttonY, GetButtonWidth(), ButtonH);
    }
}