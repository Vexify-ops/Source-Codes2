using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

class Program
{
    // PInvoke declarations for necessary Win32 functions
    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

    // Function to create the Pixel effect
    public static void DrawPixelEffect(IntPtr hdc)
    {
        // Get screen width and height
        int screenW = Screen.PrimaryScreen.Bounds.Width;
        int screenH = Screen.PrimaryScreen.Bounds.Height;

        // Create a Graphics object from the device context
        using (Graphics g = Graphics.FromHdc(hdc))
        {
            Random r = new Random();

            // Loop to create a dynamic SetPixel effect
            while (true)
            {
                // Create random pixels on the screen
                for (int i = 0; i < 100; i++) // Draw 100 random pixels each frame
                {
                    int x = r.Next(0, screenW);
                    int y = r.Next(0, screenH);
                    Color randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));

                    // Set the pixel at (x, y) to the random color
                    g.FillRectangle(new SolidBrush(randomColor), x, y, 1, 1);
                }

                // Sleep for a short period to control the speed of the effect
                Thread.Sleep(50); // Update every 50ms
            }
        }
    }

    public static void Main()
    {
        // Get the device context for the screen
        IntPtr hwnd = GetDesktopWindow();
        IntPtr hdc = GetDC(hwnd);

        try
        {
            // Start the Pixel effect
            DrawPixelEffect(hdc);
        }
        finally
        {
            // Release the device context when done
            ReleaseDC(hwnd, hdc);
        }
    }
}
