using Display;
using System.Linq;
namespace GameOfLife
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Matrix m = new RandomMatrix(20, 20, 0.25);

            Matrix k = new Matrix(3, 3)
            {
                {1,1,1},
                {1,0,1},
                {1,1,1},
            };
            Func<double, double, double> transformation = (current, conv) =>
            {
                switch (current, conv)
                {
                    case (1, 2 or 3):
                    case (0, 3):
                        return 1;
                    default:
                        return 0;
                }
            };
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Grid display = new Grid(m.Rows, m.Columns, 2, 2);
            Console.CursorVisible = false;
            do
            {
                display.Clear();
                for (int r = 0; r < m.Rows; r++)
                {
                    for (int c = 0; c < m.Columns; c++)
                    {
                        display.FillPixel(m[r,c] == 1 ? '\u2588' : ' ', ConsoleColor.White, r, c);
                    }
                }
                display.Render();
                m = m.Convolve(k, transformation);
                //Console.ReadLine();
            } while (m.Sum() > 0);
        }
    }
}