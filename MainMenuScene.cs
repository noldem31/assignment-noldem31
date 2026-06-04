using Silk.NET.Maths;
using Silk.NET.SDL;

namespace Minesweeper;

public static class MainMenuScene {
    private const int MenuWidth = 500;
    private const int MenuHeight = 500;
    
    private const int MenuButtonW = 250;
    private const int MenuButtonH = 50;
    private const int MenuButtonX = (MenuWidth - MenuButtonW) / 2;

    private const int TotalButtonHeight = (3 * MenuButtonH) + (2 * ButtonGap);
    private const int FirstButtonY = (MenuHeight - TotalButtonHeight) / 2;
    private const int ButtonGap = 25;
    
    private const int EasyButtonY = FirstButtonY;
    private const int MediumButtonY = FirstButtonY + 1 * (MenuButtonH + ButtonGap);
    private const int HardButtonY = FirstButtonY + 2 * (MenuButtonH + ButtonGap);
    
    public static Difficulty? HandleClick(int mouseX, int mouseY) {
        if (IsMouseOverButton(mouseX, mouseY, MenuButtonX, EasyButtonY, MenuButtonW, MenuButtonH)) return Difficulty.Easy;
        if (IsMouseOverButton(mouseX, mouseY, MenuButtonX, MediumButtonY, MenuButtonW, MenuButtonH)) return Difficulty.Medium;
        if (IsMouseOverButton(mouseX, mouseY, MenuButtonX, HardButtonY, MenuButtonW, MenuButtonH)) return Difficulty.Hard;
        
        return null;
    }

    public static bool IsHoveringButtons(int mx, int my) {
        return IsMouseOverButton(mx, my, MenuButtonX, EasyButtonY, MenuButtonW, MenuButtonH) ||
               IsMouseOverButton(mx, my, MenuButtonX, MediumButtonY, MenuButtonW, MenuButtonH) ||
               IsMouseOverButton(mx, my, MenuButtonX, HardButtonY, MenuButtonW, MenuButtonH);
    }

    public static unsafe void Render(Sdl sdl, Renderer* r, int mouseX, int mouseY) {
        const string mainTitle = "MINESWEEPER";
        var mainTitleWidth = mainTitle.Length * 24;
        var mainTitleX = (MenuWidth - mainTitleWidth) / 2;
        
        TextRenderer.DrawVectorText(sdl, r, mainTitle, mainTitleX+5, 50, 4, Colors.White);
        
        const string subTitle = "SELECT DIFFICULTY";
        var subTitleWidth = subTitle.Length * 12;
        var subTitleX = (MenuWidth - subTitleWidth) / 2;
        TextRenderer.DrawVectorText(sdl, r, subTitle, subTitleX, 100, 2, Colors.Gray);
        
        RenderMenuButton(sdl, r, "EASY", EasyButtonY, mouseX, mouseY);
        RenderMenuButton(sdl, r, "MEDIUM", MediumButtonY, mouseX, mouseY);
        RenderMenuButton(sdl, r, "HARD", HardButtonY, mouseX, mouseY);
    }

    private static unsafe void RenderMenuButton(Sdl sdl, Renderer* r, string text, int y, int mx, int my) {
        var isHovered = IsMouseOverButton(mx, my, MenuButtonX, y, MenuButtonW, MenuButtonH);
        var rect = new Rectangle<int>(MenuButtonX, y, MenuButtonW, MenuButtonH);
        
        var background = isHovered ? Colors.ButtonHoverBackground : Colors.ButtonIdleBackground;
        var border = isHovered ? Colors.ButtonHoverBorder : Colors.ButtonIdleBorder;
        
        sdl.SetRenderDrawColor(r, background.R, background.G, background.B, 255);
        sdl.RenderFillRect(r, &rect);
        
        sdl.SetRenderDrawColor(r, border.R, border.G, border.B, 255);
        sdl.RenderDrawRect(r, &rect);
        
        var textX = MenuButtonX + (MenuButtonW - (text.Length * 12)) / 2;
        TextRenderer.DrawVectorText(sdl, r, text, textX, y + 15, 2, Colors.White);
    }

    private static bool IsMouseOverButton(int mouseX, int mouseY, int buttonX, int buttonY, int buttonWidth, int buttonHeight) {
        return mouseX >= buttonX && 
               mouseX <  buttonX + buttonWidth && 
               mouseY >= buttonY && 
               mouseY <  buttonY + buttonHeight;
    }
}