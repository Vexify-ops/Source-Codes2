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

    // Structure to get the cursor position
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    // POINT structure for GetCursorPos
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    // Function to create the PolyDraw effect
    public static void DrawPolyDrawEffect(IntPtr hdc)
    {
        // Get screen width and height
        int screenW = Screen.PrimaryScreen.Bounds.Width;
        int screenH = Screen.PrimaryScreen.Bounds.Height;

        // Create a Graphics object from the device context
        using (Graphics g = Graphics.FromHdc(hdc))
        {
            Random r = new Random();

            // Loop to draw random polyline
            while (true)
            {

                // Generate a random number of points for the polyline (between 5 and 15)
                int numPoints = r.Next(5, 16);
                Point[] polylinePoints = new Point[numPoints];

                // Generate random points for the polyline
                for (int i = 0; i < numPoints; i++)
                {
                    polylinePoints[i] = new Point(r.Next(0, screenW), r.Next(0, screenH));
                }

                // Create a random color for the polyline
                Color randomColor = Color.FromArgb(r.Next(255), r.Next(0), r.Next(0));
                using (Pen pen = new Pen(randomColor, 2))
                {
                    // Draw the polyline using the random points and color
                    g.DrawPolygon(pen, polylinePoints);
                }

                // Sleep for a short period to control the speed of the effect
                Thread.Sleep(10);
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
            // Start the PolyDraw effect
            DrawPolyDrawEffect(hdc);
        }
        finally
        {
            // Release the device context when done
            ReleaseDC(hwnd, hdc);
        }
    }
}
