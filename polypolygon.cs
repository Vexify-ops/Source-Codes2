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

    // Function to create the PolyPolygon effect
    public static void DrawPolyPolygonEffect(IntPtr hdc)
    {
        // Get screen width and height
        int screenW = Screen.PrimaryScreen.Bounds.Width;
        int screenH = Screen.PrimaryScreen.Bounds.Height;

        // Create a Graphics object from the device context
        using (Graphics g = Graphics.FromHdc(hdc))
        {
            Random r = new Random();
            int size = 100; // Size of each polygon

            // Loop to draw multiple polygons with random vertices and colors
            while (true)
            {
                // Clear the screen by filling it with black

                // Generate random polygons
                for (int i = 0; i < 5; i++) // Draw 5 polygons
                {
                    // Randomly choose the number of vertices for this polygon
                    int numVertices = r.Next(3, 6); // Triangles (3) to pentagons (5)
                    Point[] polygonPoints = new Point[numVertices];

                    // Randomly generate points for the polygon
                    for (int j = 0; j < numVertices; j++)
                    {
                        polygonPoints[j] = new Point(
                            r.Next(0, screenW),
                            r.Next(0, screenH)
                        );
                    }

                    // Create a random color for the polygon
                    Color randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                    using (Brush brush = new SolidBrush(randomColor))
                    {
                        // Draw the polygon using the random points and color
                        g.FillPolygon(brush, polygonPoints);
                    }
                }

                // Sleep for a short period to control the speed of the effect
                Thread.Sleep(100);
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
            // Start the PolyPolygon effect
            DrawPolyPolygonEffect(hdc);
        }
        finally
        {
            // Release the device context when done
            ReleaseDC(hwnd, hdc);
        }
    }
}
