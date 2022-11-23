using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    internal partial class Matrix
    {
        /// <summary>
        /// Perform a convolution on the matrix against a kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="transformation">The transformation on the resultant convolution, as a function of the current value and the 
        /// base convolution.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">When either the matrix or the kernel is null.</exception>
        /// <exception cref="ArgumentException">When the kernel is not odd, or the kernel is not smaller than the matrix</exception>
        public Matrix Convolve(Matrix kernel, Func<double, double, double>? transformation = null)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException(nameof(kernel));
            }
            if (kernel.Rows % 2 == 0 || kernel.Columns % 2 == 0)
            {
                throw new ArgumentException("Kernel must have odd dimensions.", nameof(kernel));
            }
            if (Rows < kernel.Rows || Columns < kernel.Columns)
            {
                throw new ArgumentException("Kernel must be smaller than the matrix.", nameof(kernel));
            }
            Matrix result = new Matrix(Rows, Columns);
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    double current = this[row, column];
                    double convolution = Convolve(kernel, row, column);
                    if (transformation != null) 
                        convolution = transformation(current, convolution);
                    result[row, column] = convolution;
                }
            }
            return result;
        }
        
        private double Convolve(Matrix kernel, int row, int column)
        {
            int kernelRow = kernel.Rows / 2;
            int kernelColumn = kernel.Columns / 2;
            double result = 0;
            for (int i = 0; i < kernel.Rows; i++)
            {
                for (int j = 0; j < kernel.Columns; j++)
                {
                    int matrixRow = row + i - kernelRow;
                    int matrixColumn = column + j - kernelColumn;
                    if (matrixRow >= 0 && matrixRow < Rows && matrixColumn >= 0 && matrixColumn < Columns)
                    {
                        result += this[matrixRow, matrixColumn] * kernel[i, j];
                    }
                }
            }
            return result;
        }
    }
}
