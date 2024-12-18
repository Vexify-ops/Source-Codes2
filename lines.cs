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

    static void Main(string[] args)
    {
        // Create a random number generator
        Random random = new Random();

        IntPtr hwnd = IntPtr.Zero;  // Use null pointer for the whole screen
        IntPtr hdc = GetDC(hwnd);   // Get the device context of the screen
        IntPtr memDC = CreateCompatibleDC(hdc); // Create a compatible memory DC

        IntPtr hBitmap = CreateCompatibleBitmap(hdc, screenW, screenH);  // Create compatible bitmap for double-buffering
        SelectObject(memDC, hBitmap);  // Select the bitmap into the memory DC

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
                    // Create a pen with random color and random thickness
                    Pen barako = new Pen(Color.FromArgb(random.Next(255), random.Next(255), random.Next(255), random.Next(255)), random.Next(50));

                    // Set the line join and cap styles
                    barako.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                    barako.StartCap = barako.EndCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;

                    // Create random points for the line
                    Point[] ptsArray =
                    {
                        new Point(random.Next(-screenW, screenW + screenW), random.Next(-screenH, screenH + screenH)),
                        new Point(random.Next(-screenW, screenW + screenW), random.Next(-screenH, screenH + screenH))
                    };

                    // Draw the random line with the pen
                    g.DrawLines(barako, ptsArray);
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
}
