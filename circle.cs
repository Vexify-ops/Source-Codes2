using System;
using System.Collections.Generic;
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

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern bool PatBlt(IntPtr hdc, int x, int y, int nWidth, int nHeight, int dwRop);

    // Circle class to represent each moving circle
    public class Circle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public Color Color { get; set; }
        private float speedX, speedY;

        public Circle(float x, float y, float radius, Color color)
        {
            X = x;
            Y = y;
            Radius = radius;
            Color = color;
            speedX = (float)(new Random().NextDouble() * 2 - 1); // Random speed between -1 and 1
            speedY = (float)(new Random().NextDouble() * 2 - 1);
        }

        // Method to move the circle
        public void Move()
        {
            X += speedX;
            Y += speedY;

            // Bounce off the edges of the screen
            if (X - Radius < 0 || X + Radius > Screen.PrimaryScreen.Bounds.Width)
                speedX = -speedX;

            if (Y - Radius < 0 || Y + Radius > Screen.PrimaryScreen.Bounds.Height)
                speedY = -speedY;
        }
    }

    // Main method where the code will execute
    public static void Main()
    {
        int screenW = Screen.PrimaryScreen.Bounds.Width;
        int screenH = Screen.PrimaryScreen.Bounds.Height;

        // Initialize a list of circles
        List<Circle> circles = new List<Circle>();
        Random random = new Random();
        for (int i = 0; i < 10; i++)
        {
            circles.Add(new Circle(
                random.Next(50, screenW - 50),
                random.Next(50, screenH - 50),
                random.Next(20, 50),
                Color.FromArgb(random.Next(256), random.Next(256), random.Next(256))
            ));
        }

        IntPtr hwnd = GetDesktopWindow();
        IntPtr hdc = GetDC(hwnd); // Get device context for the screen

        // Define the background color
        Color backgroundColor = Color.Black;

        using (Graphics g = Graphics.FromHdc(hdc))
        {
            using (BufferedGraphicsContext context = BufferedGraphicsManager.Current)
            {
                using (BufferedGraphics bufferedGraphics = context.Allocate(g, new Rectangle(0, 0, screenW, screenH)))
                {
                    Graphics bg = bufferedGraphics.Graphics;

                    while (true)
                    {
                        bg.Clear(backgroundColor);

                        foreach (var circle in circles)
                        {
                            circle.Move();
                            using (SolidBrush brush = new SolidBrush(circle.Color))
                            {
                                bg.FillEllipse(brush, circle.X - circle.Radius, circle.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2);
                            }
                        }

                        // Render the buffered graphics to the screen
                        bufferedGraphics.Render(g);

                        // Optional: Perform a small delay to control the speed of updates
                        Thread.Sleep(16); // About 60 FPS
                    }
                }
            }
        }
    }
}
