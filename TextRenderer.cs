using Silk.NET.SDL;

namespace Minesweeper;

public static class TextRenderer {
    // AI-generated
    public static unsafe void DrawVectorText(Sdl sdl, Renderer* r, string text, int startX, int startY, int size, (byte R, byte G, byte B) color) {
        sdl.SetRenderDrawColor(r, color.R, color.G, color.B, 255);
        var currentX = startX;

        foreach (var c in text.ToUpper()) {
            if (c == ' ') {
                currentX += 6 * size;
                continue;
            }
            
            List<(int x1, int y1, int x2, int y2)> segments = [];

            switch (c) {
                case 'A':
                    segments.Add((0,5, 0,0)); segments.Add((0,0, 4,0)); segments.Add((4,0, 4,5)); segments.Add((0,2, 4,2));
                    break;
                case 'B':
                    segments.Add((0,0, 0,5)); segments.Add((0,0, 3,0)); segments.Add((3,0, 4,1)); segments.Add((4,1, 4,2)); segments.Add((4,2, 3,2)); segments.Add((3,2, 0,2)); segments.Add((3,2, 4,3)); segments.Add((4,3, 4,4)); segments.Add((4,4, 3,5)); segments.Add((3,5, 0,5));
                    break;
                case 'C':
                    segments.Add((4,0, 0,0)); segments.Add((0,0, 0,5)); segments.Add((0,5, 4,5));
                    break;
                case 'D':
                    segments.Add((0,0, 0,5)); segments.Add((0,0, 3,0)); segments.Add((3,0, 4,1)); segments.Add((4,1, 4,4)); segments.Add((4,4, 3,5)); segments.Add((3,5, 0,5));
                    break;
                case 'E':
                    segments.Add((4,0, 0,0)); segments.Add((0,0, 0,5)); segments.Add((0,5, 4,5)); segments.Add((0,2, 3,2));
                    break;
                case 'F':
                    segments.Add((4,0, 0,0)); segments.Add((0,0, 0,5)); segments.Add((0,2, 3,2));
                    break;
                case 'G':
                    segments.Add((4,0, 0,0)); segments.Add((0,0, 0,5)); segments.Add((0,5, 4,5)); segments.Add((4,5, 4,3)); segments.Add((4,3, 2,3));
                    break;
                case 'H':
                    segments.Add((0,0, 0,5)); segments.Add((4,0, 4,5)); segments.Add((0,2, 4,2));
                    break;
                case 'I':
                    segments.Add((0,0, 4,0)); segments.Add((2,0, 2,5)); segments.Add((0,5, 4,5));
                    break;
                case 'J':
                    segments.Add((4,0, 4,4)); segments.Add((4,4, 2,5)); segments.Add((2,5, 0,4)); segments.Add((0,4, 0,3));
                    break;
                case 'K':
                    segments.Add((0,0, 0,5)); segments.Add((4,0, 0,2)); segments.Add((0,2, 4,5));
                    break;
                case 'L':
                    segments.Add((0,0, 0,5)); segments.Add((0,5, 4,5));
                    break;
                case 'M':
                    segments.Add((0,5, 0,0)); segments.Add((0,0, 2,2)); segments.Add((2,2, 4,0)); segments.Add((4,0, 4,5));
                    break;
                case 'N':
                    segments.Add((0,5, 0,0)); segments.Add((0,0, 4,5)); segments.Add((4,5, 4,0));
                    break;
                case 'O':
                    segments.Add((0,0, 4,0)); segments.Add((4,0, 4,5)); segments.Add((4,5, 0,5)); segments.Add((0,5, 0,0));
                    break;
                case 'P':
                    segments.Add((0,5, 0,0)); segments.Add((0,0, 4,0)); segments.Add((4,0, 4,2)); segments.Add((4,2, 0,2));
                    break;
                case 'Q':
                    segments.Add((0,0, 4,0)); segments.Add((4,0, 4,4)); segments.Add((4,4, 0,4)); segments.Add((0,4, 0,0)); segments.Add((2,3, 4,5));
                    break;
                case 'R':
                    segments.Add((0,5, 0,0)); segments.Add((0,0, 4,0)); segments.Add((4,0, 4,2)); segments.Add((4,2, 0,2)); segments.Add((0,2, 4,5));
                    break;
                case 'S':
                    segments.Add((4,0, 0,0)); segments.Add((0,0, 0,2)); segments.Add((0,2, 4,2)); segments.Add((4,2, 4,5)); segments.Add((4,5, 0,5));
                    break;
                case 'T':
                    segments.Add((0,0, 4,0)); segments.Add((2,0, 2,5));
                    break;
                case 'U':
                    segments.Add((0,0, 0,5)); segments.Add((0,5, 4,5)); segments.Add((4,5, 4,0));
                    break;
                case 'V':
                    segments.Add((0,0, 2,5)); segments.Add((2,5, 4,0));
                    break;
                case 'W':
                    segments.Add((0,0, 0,5)); segments.Add((0,5, 2,3)); segments.Add((2,3, 4,5)); segments.Add((4,5, 4,0));
                    break;
                case 'X':
                    segments.Add((0,0, 4,5)); segments.Add((4,0, 0,5));
                    break;
                case 'Y':
                    segments.Add((0,0, 2,2)); segments.Add((4,0, 2,2)); segments.Add((2,2, 2,5));
                    break;
                case 'Z':
                    segments.Add((0,0, 4,0)); segments.Add((4,0, 0,5)); segments.Add((0,5, 4,5));
                    break;
                case '1':
                    segments.Add((1,1, 2,0)); segments.Add((2,0, 2,5)); segments.Add((1,5, 3,5));
                    break;
                case '2':
                    segments.Add((0,0, 4,0)); segments.Add((4,0, 4,2)); segments.Add((4,2, 0,5)); segments.Add((0,5, 4,5));
                    break;
                case '3':
                    segments.Add((0,0, 4,0)); segments.Add((4,0, 4,5)); segments.Add((4,5, 0,5)); segments.Add((0,2, 4,2));
                    break;
                case '4':
                    segments.Add((0,0, 0,3)); segments.Add((0,3, 4,3)); segments.Add((4,0, 4,5));
                    break;
                case '5':
                    segments.Add((4,0, 0,0)); segments.Add((0,0, 0,2)); segments.Add((0,2, 4,2)); segments.Add((4,2, 4,5)); segments.Add((4,5, 0,5));
                    break;
                case '6':
                    segments.Add((4,0, 0,0)); segments.Add((0,0, 0,5)); segments.Add((0,5, 4,5)); segments.Add((4,5, 4,2)); segments.Add((4,2, 0,2));
                    break;
                case '7':
                    segments.Add((0,0, 4,0)); segments.Add((4,0, 4,5));
                    break;
                case '8':
                    segments.Add((0,0, 4,0)); segments.Add((4,0, 4,5)); segments.Add((4,5, 0,5)); segments.Add((0,5, 0,0)); segments.Add((0,2, 4,2));
                    break;
                case '9':
                    segments.Add((4,5, 4,0)); segments.Add((4,0, 0,0)); segments.Add((0,0, 0,2)); segments.Add((0,2, 4,2));
                    break;
                case '0':
                    segments.Add((0,0, 4,0)); segments.Add((4,0, 4,5)); segments.Add((4,5, 0,5)); segments.Add((0,5, 0,0)); segments.Add((4,0, 0,5));
                    break;
            }

            foreach (var seg in segments) {
                var x1 = currentX + (seg.x1 * size);
                var y1 = startY + (seg.y1 * size);
                var x2 = currentX + (seg.x2 * size);
                var y2 = startY + (seg.y2 * size);

                sdl.RenderDrawLine(r, x1, y1, x2, y2);
                sdl.RenderDrawLine(r, x1 + 1, y1, x2 + 1, y2);
            }

            currentX += 6 * size;
        }
    }
    // end AI-generated
}