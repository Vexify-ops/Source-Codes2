using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

class circle
{
    // Import necessary GDI functions
    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

    [DllImport("gdi32.dll")]
    public static extern bool SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, uint rop);

    const uint SRCCOPY = 0x00CC0020;

    // Screen width and height
    private static int screenW = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
    private static int screenH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

    static void Main(string[] args)
    {
        IntPtr hwnd = IntPtr.Zero;  // Use null pointer for the whole screen
        IntPtr hdc = GetDC(hwnd);   // Get the device context of the screen
        IntPtr memDC = CreateCompatibleDC(hdc); // Create a compatible memory DC

        IntPtr hBitmap = CreateCompatibleBitmap(hdc, screenW, screenH);  // Create compatible bitmap for double-buffering
        SelectObject(memDC, hBitmap);  // Select the bitmap into the memory DC

        int circleRadius = 50; // Radius of the circle
        int hue = 0; // Starting hue for color

        // Main loop to make the circle follow the cursor
        while (true)
        {
            try
            {
                IntPtr screenDC = GetDC(IntPtr.Zero);  // Get the screen DC to draw on the screen

                // BitBlt to copy the current screen content to the memory DC
                BitBlt(memDC, 0, 0, screenW, screenH, screenDC, 0, 0, SRCCOPY);

                // Create a Graphics object from the memory DC
                using (Graphics g = Graphics.FromHdc(memDC))
                {
                    // Get the cursor's position
                    Point cursorPos = System.Windows.Forms.Cursor.Position;

                    // Convert HSL to RGB
                    Color color = HSLToRGB(hue, 1f, 0.5f); // Full saturation and lightness for bright colors
                    Brush brush = new SolidBrush(color);

                    // Fill the circle with the color at the cursor position
                    g.FillEllipse(brush, cursorPos.X - circleRadius, cursorPos.Y - circleRadius, circleRadius * 2, circleRadius * 2);

                    // Increase the hue to get a new color for the next frame
                    hue += 2; // Increase hue to change color
                    if (hue >= 360)
                    {
                        hue = 0; // Reset hue after a full rotation
                    }
                }

                // Copy the memory DC to the screen
                BitBlt(screenDC, 0, 0, screenW, screenH, memDC, 0, 0, SRCCOPY);

                ReleaseDC(IntPtr.Zero, screenDC);  // Release the screen DC

                Thread.Sleep(10); // Delay to make it smooth
            }
            catch
            {
                // Handle any errors (if necessary)
            }
        }
    }

    // HSL to RGB conversion
    private static Color HSLToRGB(int hue, float saturation, float lightness)
    {
        float r = 0, g = 0, b = 0;
        float c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        float x = c * (1 - Math.Abs((hue / 60f) % 2 - 1));
        float m = lightness - c / 2;

        if (hue >= 0 && hue < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (hue >= 60 && hue < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (hue >= 120 && hue < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (hue >= 180 && hue < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (hue >= 240 && hue < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else if (hue >= 300 && hue < 360)
        {
            r = c;
            g = 0;
            b = x;
        }

        r = (r + m) * 255;
        g = (g + m) * 255;
        b = (b + m) * 255;

        return Color.FromArgb((int)r, (int)g, (int)b);
    }
}
