using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

class PlgBltEffect
{
    // Import necessary GDI functions
    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    public static extern bool SelectObject(IntPtr hdc, IntPtr hObject);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern bool PlgBlt(IntPtr hdcDest, POINT[] lpPoint, IntPtr hdcSrc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hbmMask, int nXMask, int nYMask);

    [DllImport("gdi32.dll")]
    public static extern bool PatBlt(IntPtr hdc, int nXStart, int nYStart, int nWidth, int nHeight, uint dwRop);

    // Define the necessary constants for PatBlt
    public const uint PATINVERT = 0x5A0049;

    // Define structure for points
    public struct POINT
    {
        public int X;
        public int Y;
    }

    // Main entry point of the application
    public static void Main()
    {
        Random random = new Random();
        int x = Screen.PrimaryScreen.Bounds.X;
        int y = Screen.PrimaryScreen.Bounds.Y;
        int left = Screen.PrimaryScreen.Bounds.Left;
        int top = Screen.PrimaryScreen.Bounds.Top;
        int right = Screen.PrimaryScreen.Bounds.Right;
        int bottom = Screen.PrimaryScreen.Bounds.Bottom;
        int screenW = 1920; // Screen width (can be retrieved dynamically)
        int screenH = 1080; // Screen height (can be retrieved dynamically)

        // Get the DC of the screen (desktop window)
        IntPtr hwnd = IntPtr.Zero; // Desktop window
        IntPtr hdc = GetDC(hwnd); // Get DC for screen

        // Create a memory device context
        IntPtr hdcMem = CreateCompatibleDC(hdc);

        // Create a compatible bitmap for drawing (We are not using it for the drawing anymore)
        IntPtr hBitmap = CreateCompatibleBitmap(hdc, screenW, screenH);
        SelectObject(hdcMem, hBitmap);

        // Create random polygon points
        POINT[] lppoint = new POINT[3];
        lppoint[0].X = random.Next(screenW);
        lppoint[0].Y = random.Next(screenH);
        lppoint[1].X = random.Next(screenW);
        lppoint[1].Y = random.Next(screenH);
        lppoint[2].X = random.Next(screenW);
        lppoint[2].Y = random.Next(screenH);

        // Keep the effect running in a loop
        while (true)
        {
            // Use PlgBlt to fill the polygon (this will apply the effect directly to the screen)
            // PlgBlt here is drawing the shape directly to the screen by mapping a polygon
            PlgBlt(hdc, lppoint, hdc, 0, 0, screenW, screenH, IntPtr.Zero, 0, 0);

            // Optionally, invert pixels in the selected region (PATINVERT effect)
            PatBlt(hdc, 0, 0, random.Next(screenW), random.Next(screenH), PATINVERT);

            // Re-create random points for a new effect each time
            lppoint[0].X = left + (random.Next(screenW));
            lppoint[0].Y = top + (random.Next(screenH));
            lppoint[1].X = right + (random.Next(screenW));
            lppoint[1].Y = top - (random.Next(screenH));
            lppoint[2].X = left + (random.Next(screenW));
            lppoint[2].Y = bottom + (random.Next(screenH));

            // Delay to control how often the effect is updated
            Thread.Sleep(10); // 10 milliseconds between updates (adjust for faster/slower effects)
        }

        // Cleanup is not reached unless the loop breaks
        DeleteObject(hBitmap);
        DeleteDC(hdcMem);
        ReleaseDC(hwnd, hdc);
    }
}
