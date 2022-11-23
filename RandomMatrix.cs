using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    /// <summary>
    /// A randomised matrix.
    /// </summary>
    internal class RandomMatrix : Matrix
    {
        
        public RandomMatrix(int rows, int columns, double probability) : base(rows, columns) 
        {
            Random random = new Random();
            for (int i = 0; i < rows * columns; i++)
            {
                double nextRandom = random.NextDouble();
                Add(nextRandom < probability ? 1 : 0);
            }
        }
    }
}
