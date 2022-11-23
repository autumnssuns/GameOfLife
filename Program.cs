using System.Linq;
namespace GameOfLife
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Matrix m = new RandomMatrix(10, 10, 0.25);

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
            do
            {
                Console.Clear();
                Console.WriteLine(m);
                Console.WriteLine(m.Sum());
                m = m.Convolve(k, transformation);
                Console.Read();
            } while (m.Sum() > 0);
        }
    }
}