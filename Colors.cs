namespace Minesweeper;

// https://rgbcolorpicker.com/
public static class Colors {
    public static readonly (byte R, byte G, byte B) White = (255, 255, 255);
    public static readonly (byte R, byte G, byte B) Gray = (180, 180, 180);
    public static readonly (byte R, byte G, byte B) LightGray = (210, 210, 210);
    
    public static readonly (byte R, byte G, byte B) Background = (45, 45, 45);
    public static readonly (byte R, byte G, byte B) Header = (30, 30, 30);
    public static readonly (byte R, byte G, byte B) WinOverlay = (50, 200, 100);
    public static readonly (byte R, byte G, byte B) LoseOverlay = (230, 75, 60);
    
    public static readonly (byte R, byte G, byte B) ButtonIdleBackground = (55, 55, 55);
    public static readonly (byte R, byte G, byte B) ButtonHoverBackground = (65, 65, 65);
    public static readonly (byte R, byte G, byte B) ButtonIdleBorder = (100, 100, 100);
    public static readonly (byte R, byte G, byte B) ButtonHoverBorder = (50, 150, 200);
    
    public static readonly (byte R, byte G, byte B) OverlayDimBackground = (15, 15, 20);
    public static readonly (byte R, byte G, byte B) OverlayPanelBackground = (33, 33, 33);
    
    public static readonly (byte R, byte G, byte B) RestartIdleBackground = (40, 130, 185);
    public static readonly (byte R, byte G, byte B) RestartHoverBackground = (50, 150, 220);
    
    public static readonly (byte R, byte G, byte B) MenuIdleBackground = (180, 100, 30);
    public static readonly (byte R, byte G, byte B) MenuHoverBackground = (230, 125, 35);
}