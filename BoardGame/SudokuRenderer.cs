using BoardGame.Sudoku;

namespace BoardGame
{
    public class SudokuRenderer
    {
        public void Display(SudokuBoard board, IReadOnlySet<(int Row, int Col)>? highlightedCells = null)
        {
            Console.WriteLine("    1 2 3   4 5 6   7 8 9");

            for (int row = 0; row < SudokuBoard.Size; row++)
            {
                if (row > 0 && row % 3 == 0)
                    Console.WriteLine("   -----------------------");

                Console.Write($" {(char)('A' + row)}  ");

                for (int col = 0; col < SudokuBoard.Size; col++)
                {
                    if (col > 0 && col % 3 == 0)
                        Console.Write("| ");

                    var cell = board.GetCell(row, col);
                    string value = cell.Value?.ToString() ?? "_";

                    // Bold for pre-filled cells
                    if (cell.IsFixed)
                        value = $"\x1b[1;94m{value}\x1b[0m";
                    else if (cell.Value.HasValue && highlightedCells?.Contains((row, col)) == true)
                        value = $"\x1b[1;31m{value}\x1b[0m";

                    Console.Write($"{value} ");
                }

                Console.WriteLine();
            }
        }
    }
}
