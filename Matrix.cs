using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    /// <summary>
    /// A 2D array of cells.
    /// </summary>
    internal partial class Matrix: ICollection<double>
    {
        private double[,] _matrix;
        private int _rows;
        private int _count;
        private bool[,] _initialised;
        public Matrix(int rows, int columns)
        {
            _matrix = new double[rows, columns];
            _initialised = new bool[rows, columns];
            _rows = 0;
            _count = 0;
        }

        public int Rows => _matrix.GetLength(0);
        public int Columns => _matrix.GetLength(1);

        public double this[int row, int column]
        {
            get
            {
                return _matrix[row, column];
            }
            set
            {
                // Check if the current row has already been initialised
                bool newRow = false;
                bool[] rowInitialisedElements = new bool[Columns];
                for (int c = 0; c < Columns; c++)
                {
                    rowInitialisedElements[c] = _initialised[row, c];
                }
                // If the row has not been initialised, try to initialise it
                if (rowInitialisedElements.Any(x => x == false))
                {
                    newRow = true;
                }
                // Initialise the element, if not already
                if (!_initialised[row, column])
                {
                    _initialised[row, column] = true;
                    rowInitialisedElements[column] = true;
                    _count++;
                }
                // If the row is full after this addition, increment the row count
                if (rowInitialisedElements.All(x => x) && newRow)
                {
                    _rows++;
                }
                _matrix[row, column] = value;
            }
        }

        /// <summary>
        /// Traverse the matrix in row-major order.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The element at the given index.</returns>
        public double this[int index]
        {
            get
            {
                int row = index / Columns;
                int column = index % Columns;
                return _matrix[row, column];
            }
            set
            {
                int row = index / Columns;
                int column = index % Columns;
                _matrix[row, column] = value;
            }
        }

        /// <summary>
        /// Add a row to the matrix.
        /// </summary>
        /// <param name="row"></param>
        /// <exception cref="ArgumentException">When the row is not the same length as the matrix (in terms of the number of columns),
        /// or when the matrix is full.</exception>
        public void Add(params double[] row)
        {
            if (row.Length != Columns)
            {
                throw new ArgumentException("Row length must match the number of columns.");
            }
            if (_rows == Rows)
            {
                throw new ArgumentException("Matrix is full.");
            }
            for (int i = 0; i < Columns; i++)
            {
                this[_rows, i] = row[i];
            }
        }

        /// <summary>
        /// Add a single item to the matrix.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArgumentException">When the matrix is full.</exception>
        public void Add(double item)
        {
            if (_count == Rows * Columns)
            {
                throw new ArgumentException("Matrix is full.");
            }
            this[_count] = item;
            _count++;
        }

        /// <summary>
        /// Clear the matrix.
        /// </summary>
        public void Clear()
        {
            _matrix = new double[Rows, Columns];
            _rows = 0;
            _count = 0;
        }

        /// <summary>
        /// Check if the matrix contains an item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item exists, false otherwise.</returns>
        public bool Contains(double item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (this[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// The number of initialised elements in the matrix.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Check if the matrix is read-only. Always false.
        /// </summary>
        public bool IsReadOnly => false;

        public bool Remove(double item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (this[i].Equals(item))
                {
                    for (int j = i; j < _count - 1; j++)
                    {
                        this[j] = default;
                    }
                    _count--;
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < Rows * Columns; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    sb.Append(_matrix[row, column]);
                    sb.Append('\t');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Flatten the internal 2D matrix to a 1D array
        /// </summary>
        /// <returns></returns>
        private double[] Flatten()
        {
            int size = Rows * Columns;
            double[] flat = new double[size];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    flat[i * Columns + j] = _matrix[i, j];
                }
            }
            return flat;
        }

        public void CopyTo(double[] array, int arrayIndex)
        {
            Flatten().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Get the sum of all elements in the matrix.
        /// </summary>
        /// <returns>The sum of all elements in the matrix.</returns>
        public double Sum()
        {
            double sum = 0;
            this.AsEnumerable().ToList().ForEach(item => sum += item);
            return sum;
        }
    }
}