using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

class GDI
{
    private static Random r = new Random();
    private static int screenW = 1920;  // Modify to your screen width
    private static int screenH = 1080;  // Modify to your screen height

    // P/Invoke declarations
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

    [DllImport("gdi32.dll")]
    public static extern bool SelectObject(IntPtr hdc, IntPtr obj);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr obj);

    [DllImport("gdi32.dll")]
    public static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int widthDest, int heightDest,
        IntPtr hdcSource, int xSource, int ySource, int widthSource, int heightSource, int rop);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    // Constants for StretchBlt operation
    private const int SRCCOPY = 0x00CC0020;
    private const int NOTSRCINVERT = 0x999999;

    public static void Main()
    {
        IntPtr hwnd = IntPtr.Zero; // Desktop window handle
        IntPtr hdc = GetDC(hwnd); // Get device context for the screen

        int sx = screenW;  // Screen width
        int sy = screenH;  // Screen height
        int s = 0; // Start Y position

        while (true)
        {
            try
            {
                // Loop through the screen width with a step of 200 pixels
                for (int i = 0; i < sx; i += 200)
                {
                    // Apply StretchBlt effect, drawing part of the screen into another part with some offset
                    StretchBlt(hdc, i, s, 200, 200, hdc, i - 5, s - 5, 210, 210, SRCCOPY);
                }

                // Update the y-position incrementally
                s += 200;

                // If the y-position exceeds screen height, reset it to 0
                if (s >= sy)
                {
                    s = 0;
                }

            }
            catch
            {
                // No output on error
            }

            // Sleep for a short period to control the drawing speed
            Thread.Sleep(10);
        }

        // Cleanup (this won't be reached in an infinite loop)
        ReleaseDC(hwnd, hdc);
    }
}
