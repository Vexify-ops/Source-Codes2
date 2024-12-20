using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // P/Invoke declarations for working with GDI
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

    private static int cc = 0;  // Color change counter
    private static int redrawCounter = 0;  // Redraw counter

    static void Main(string[] args)
    {
        IntPtr hwnd = IntPtr.Zero;  // Use null pointer for the whole screen
        IntPtr hdc = GetDC(hwnd);   // Get the device context of the screen
        IntPtr memDC = CreateCompatibleDC(hdc); // Create a compatible memory DC

        IntPtr hBitmap = CreateCompatibleBitmap(hdc, screenW, screenH);  // Create compatible bitmap for double-buffering
        SelectObject(memDC, hBitmap);  // Select the bitmap into the memory DC

        Random random = new Random();

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
                    // Calculate hue and RGB color
                    HSL data = new HSL(cc, 1f, 0.5f);
                    RGB value = HSLToRGB(data);

                    // Create pen with color from HSL
                    Pen pen = new Pen(Color.FromArgb(255, value.R, value.G, value.B), 50);
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;

                    // Get cursor position
                    int curx = System.Windows.Forms.Cursor.Position.X;
                    int cury = System.Windows.Forms.Cursor.Position.Y;

                    // Draw lines from the cursor to the edges
                    g.DrawLine(pen, screenW, cury, curx, cury);
                    g.DrawLine(pen, 0, cury, curx, cury);
                    g.DrawLine(pen, curx, screenH, curx, cury);
                    g.DrawLine(pen, curx, 0, curx, cury);
                    g.DrawLine(pen, screenW, screenH, curx, cury);
                    g.DrawLine(pen, 0, 0, curx, cury);
                    g.DrawLine(pen, screenW, 0, curx, cury);
                    g.DrawLine(pen, 0, screenH, curx, cury);

                    // Increment color change counter and reset if necessary
                    cc++;
                    redrawCounter++;
                    if (redrawCounter >= 5)
                    {
                        // Optional: Call a custom redraw function if needed
                        // Redraw(); 
                        redrawCounter = 0;
                    }

                    if (cc >= 360) { cc = 0; }
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

    // HSL to RGB conversion function
    private static RGB HSLToRGB(HSL hsl)
    {
        float h = hsl.Hue;
        float s = hsl.Saturation;
        float l = hsl.Lightness;

        float c = (1 - Math.Abs(2 * l - 1)) * s;
        float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        float m = l - c / 2;

        float r = 0, g = 0, b = 0;

        if (h >= 0 && h < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (h >= 60 && h < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (h >= 120 && h < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (h >= 180 && h < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (h >= 240 && h < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else
        {
            r = c;
            g = 0;
            b = x;
        }

        r = (r + m) * 255;
        g = (g + m) * 255;
        b = (b + m) * 255;

        return new RGB { R = (int)r, G = (int)g, B = (int)b };
    }

    // Helper function for RGB struct
    public struct RGB
    {
        public int R;
        public int G;
        public int B;
    }

    // Helper function for HSL struct
    public struct HSL
    {
        public float Hue;
        public float Saturation;
        public float Lightness;

        public HSL(int hue, float saturation, float lightness)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
        }
    }
}
