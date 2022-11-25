using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Display
{
    /// <summary>
    /// A cell on the (Row, Column) coordinate system.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Read-only. The row index of the cell, with 0 being the top most row. 
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// Read-only. The column index of the cell, with 0 being the left most column. 
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Read-only. The width of the cell in characters.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Read-only. The height of the cell in characters.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Defines a cell with a row and a column index.
        /// </summary>
        /// <param name="row">The row index of the cell</param>
        /// <param name="column">The column index of the cell</param>
        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Creates a cell based on the current point, shifted by certain values.
        /// </summary>
        /// <param name="row">The row shift.</param>
        /// <param name="column">The column shift.</param>
        /// <param name="inPlace">Whether the current cell is shifted.</param>
        /// <returns>The shifted cell.</returns>
        public Cell Shift(int row, int column, bool inPlace = false)
        {
            if (!inPlace) return new Cell(Row + row, Column + column);
            Row += row;
            Column += column;
            return this;
        }

        /// <summary>
        /// Creates a cell based on the current cell, shifted by certain displacement vector.
        /// </summary>
        /// <param name="displacement">The displacement vector to shift by</param>
        /// <param name="inPlace">Whether the current cell is shifted.</param>
        /// <returns>The shifted cell.</returns>
        public Cell Shift(Cell displacement, bool inPlace = false)
        {
            return Shift(displacement.Row, displacement.Column, inPlace);
        }
    }
}