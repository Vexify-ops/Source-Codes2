using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class Program
{
    // PInvoke declarations for necessary Win32 functions
    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    // POINT structure for Win32 API calls
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    // Main method where the code will execute
    public static void Main()
    {
        Random random = new Random();
        int screenW = Screen.PrimaryScreen.Bounds.Width;
        int screenH = Screen.PrimaryScreen.Bounds.Height;

        IntPtr hwnd = GetDesktopWindow();
        IntPtr hdc = GetDC(hwnd); // Get device context for the screen

        // Continuously draw triangles at random positions without a delay
        while (true)
        {
            // Create random points for the triangle
            POINT[] points = new POINT[3];
            points[0].X = random.Next(0, screenW);
            points[0].Y = random.Next(0, screenH);
            points[1].X = random.Next(0, screenW);
            points[1].Y = random.Next(0, screenH);
            points[2].X = random.Next(0, screenW);
            points[2].Y = random.Next(0, screenH);

            // Draw the triangle at the random position
            using (Graphics g = Graphics.FromHdc(hdc))
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(128, random.Next(256), random.Next(256), random.Next(256))))
                {
                    g.FillPolygon(brush, new Point[]
                    {
                        new Point(points[0].X, points[0].Y),
                        new Point(points[1].X, points[1].Y),
                        new Point(points[2].X, points[2].Y)
                    });
                }
            }
        }
    }
}
