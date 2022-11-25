using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Display
{
    /// <summary>
    /// Statically sized console window with a programmable grid. Updates to 
    /// the grid can be specified and rendered separately when requested.
    /// </summary>
    /// <author>Benjamin Lewis (2020) & Dan Tran (2022)</author>
    /// <created>August 2020</created>
    /// <updated>June 2022</updated>
    public class Grid
    {
        private const int TOP_MARGIN = 1;
        private const int BOTTOM_MARGIN = 0;
        private const int LEFT_MARGIN = 1;
        private const int RIGHT_MARGIN = 0;
        private const int BORDER = 1;
        private const int MIN_ROW = 8;
        private const int MAX_ROW = 32;
        private const int MIN_COL = 16;
        private const int MAX_COL = 64;
        private const ConsoleColor DefaultConsoleColor = ConsoleColor.White;

        private int rows;
        private int cols;
        private int bufferHeight;
        private int bufferWidth;
        private int cellWidth;
        private int cellHeight;
        private string footnote;
        private char[][] buffer;
        private ConsoleColor[][] colorBuffer;
        private readonly Queue<Cell> renderQueue;

        public bool IsComplete { get; set; }

#if WINDOWS
                [DllImport("kernel32.dll", ExactSpelling = true)]
                private static extern IntPtr GetConsoleWindow();
#else
        [DllImport("libc")]
        private static extern int system(string exec);
#endif

        /// <summary>
        /// Instantiate a grid with a fixed number of rows and columns.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the grid size exceed pre-defined limits.</exception>
        public Grid(int rows, int cols, int cellWidth = 1, int cellHeight = 1)
        {
            if (rows < MIN_ROW || rows > MAX_ROW)
            {
                throw new ArgumentOutOfRangeException($"The number of grid rows is not within the acceptable range " +
                                                      $"of values ({MIN_ROW} to {MAX_ROW}).");
            }

            if (cols < MIN_COL || cols > MAX_COL)
            {
                throw new ArgumentOutOfRangeException($"The number of grid columns is not within the acceptable range " +
                                                      $"of values ({MIN_COL} to {MAX_COL}).");
            }

            this.rows = rows;
            this.cols = cols;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.renderQueue = new Queue<Cell>();
            this.IsComplete = false;

            CalculateBufferSize();
            InitializeBuffer();
            DrawBorder();
        }

        /// <summary>
        /// Resizes the window to the appropriate size and clears the console.
        /// </summary>
        public void InitializeWindow()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.Clear();
        }

        /// <summary>
        /// Renders the current state of the grid (all updates applied after the last render will be rendered).
        /// </summary>
        public void Render()
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            while (renderQueue.Count > 0)
            {
                Cell pixel = renderQueue.Dequeue();
                int row = pixel.Row, col = pixel.Column;
                Console.SetCursorPosition(col * cellWidth, row * cellHeight);
                char c = buffer[row][col];
                Console.ForegroundColor = colorBuffer[row][col];
                string str = new string(c, cellWidth);
                for (int i = 0; i < cellHeight - 1; i++)
                {
                    str += '\n' + new string(c, cellWidth);
                }
                Console.Write(str);
            }
            Console.ForegroundColor = currentColor;
        }

        /// <summary>
        /// Fill a pixel on the grid with a character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <param name="color">The color to set to.</param>
        /// <param name="row">The row index of the pixel.</param>
        /// <param name="col">The column index of the pixel.</param>
        public void FillPixel(char c, ConsoleColor color, int row, int col)
        {
            if (buffer[row + CellRowOffset(row)][col + CellColOffset(col)] == c &&
                colorBuffer[row + CellRowOffset(row)][col + CellColOffset(col)] == color) return;
            buffer[row + CellRowOffset(row)][col + CellColOffset(col)] = c;
            colorBuffer[row + CellRowOffset(row)][col + CellColOffset(col)] = color;
            renderQueue.Enqueue(new Cell(row + CellRowOffset(row), col + CellColOffset(col)));
        }

        /// <summary>
        /// Clear the grid.
        /// </summary>
        public void Clear()
        {
            InitializeBuffer();
            DrawBorder();
            Render();
        }

        /// ------------------------------------------------------------
        /// Private Methods. These CANNOT be called from your program.
        /// ------------------------------------------------------------

        /// <summary>
        /// Initializes the buffer array to be filled with whitespace.
        /// </summary>
        private void InitializeBuffer()
        {
            buffer = new char[bufferHeight][];
            for (int i = 0; i < bufferHeight; i++)
            {
                buffer[i] = new char[bufferWidth];
                for (int j = 0; j < bufferWidth; j++)
                {
                    buffer[i][j] = ' ';
                    renderQueue.Enqueue(new Cell(i, j));
                }
            }
            colorBuffer = new ConsoleColor[bufferHeight][];
            for (int i = 0; i < bufferHeight; i++)
            {
                colorBuffer[i] = new ConsoleColor[bufferWidth];
                for (int j = 0; j < bufferWidth; j++)
                {
                    colorBuffer[i][j] = DefaultConsoleColor;
                }
            }
        }

        /// <summary>
        /// Draws border characters at the appropriate buffer locations.
        /// </summary>
        private void DrawBorder()
        {
            if (BORDER == 1)
            {
                buffer[TOP_MARGIN][LEFT_MARGIN] = '╔';
                buffer[TOP_MARGIN][LEFT_MARGIN + BORDER + cols] = '╗';
                buffer[TOP_MARGIN + BORDER + rows][LEFT_MARGIN] = '╚';
                buffer[TOP_MARGIN + BORDER + rows][LEFT_MARGIN + BORDER + cols] = '╝';
                for (int i = TOP_MARGIN + BORDER; i <= (TOP_MARGIN + BORDER * rows); i++)
                {
                    buffer[i][LEFT_MARGIN] = '║';
                    buffer[i][LEFT_MARGIN + BORDER + cols] = '║';
                }
                for (int j = LEFT_MARGIN + BORDER; j <= (LEFT_MARGIN + BORDER * cols); j++)
                {
                    buffer[TOP_MARGIN][j] = '═';
                    buffer[TOP_MARGIN + BORDER + rows][j] = '═';
                }

                colorBuffer[TOP_MARGIN][LEFT_MARGIN] = DefaultConsoleColor;
                colorBuffer[TOP_MARGIN][LEFT_MARGIN + BORDER + cols] = DefaultConsoleColor;
                colorBuffer[TOP_MARGIN + BORDER + rows][LEFT_MARGIN] = DefaultConsoleColor;
                colorBuffer[TOP_MARGIN + BORDER + rows][LEFT_MARGIN + BORDER + cols] = DefaultConsoleColor;
                for (int i = TOP_MARGIN + BORDER; i <= (TOP_MARGIN + BORDER * rows); i++)
                {
                    colorBuffer[i][LEFT_MARGIN] = DefaultConsoleColor;
                    colorBuffer[i][LEFT_MARGIN + BORDER + cols] = DefaultConsoleColor;
                }
                for (int j = LEFT_MARGIN + BORDER; j <= (LEFT_MARGIN + BORDER * cols); j++)
                {
                    colorBuffer[TOP_MARGIN][j] = DefaultConsoleColor;
                    colorBuffer[TOP_MARGIN + BORDER + rows][j] = DefaultConsoleColor;
                }
            }
        }

        /// <summary>
        /// Offsets a grid column index to a buffer column index with respect
        /// to the left margin, border and cell width.
        /// </summary>
        /// <param name="col">The grid column index</param>
        /// <returns>The offset buffer column index</returns>
        private int CellColOffset(int col)
        {
            return LEFT_MARGIN + BORDER;
        }

        /// <summary>
        /// Offsets a grid row index to a buffer row index with respect
        /// to the top margin, border and cell height.
        /// </summary>
        /// <param name="row">The grid row index</param>
        /// <returns>The offset buffer row index</returns>
        private int CellRowOffset(int row)
        {
            return TOP_MARGIN + BORDER;
        }

        /// <summary>
        /// Calculates the buffer size based on margins borders and cell counts/sizes.
        /// </summary>
        private void CalculateBufferSize()
        {
            bufferHeight = TOP_MARGIN + BOTTOM_MARGIN + 2 * BORDER + rows;
            bufferWidth = LEFT_MARGIN + RIGHT_MARGIN + 2 * BORDER + cols;
        }
    }
}