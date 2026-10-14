using BoardGame.Sudoku;

namespace BoardGame
{

    public class SudokuValidator
    {
        public HashSet<(int Row, int Col)> FindConflictingCells(SudokuBoard board)
        {
            var conflicts = new HashSet<(int Row, int Col)>();
            for (int row = 0; row < SudokuBoard.Size; row++)
                for (int col = 0; col < SudokuBoard.Size; col++)
                {
                    var cell = board.GetCell(row, col);
                    if (cell.IsFixed || !cell.Value.HasValue)
                        continue;

                    for (int otherRow = 0; otherRow < SudokuBoard.Size; otherRow++)
                        for (int otherCol = 0; otherCol < SudokuBoard.Size; otherCol++)
                        {
                            if (row == otherRow && col == otherCol)
                                continue;
                            bool sharesUnit = row == otherRow || col == otherCol ||
                                (row / 3 == otherRow / 3 && col / 3 == otherCol / 3);
                            if (sharesUnit && board.GetCell(otherRow, otherCol).Value == cell.Value)
                                conflicts.Add((row, col));
                        }
                }
            return conflicts;
        }

        public List<string> Check(SudokuBoard board)
        {
            var violations = new List<string>();

            // Check rows
            for (int row = 0; row < SudokuBoard.Size; row++)
            {
                var numbers = new HashSet<int>();

                for (int col = 0; col < SudokuBoard.Size; col++)
                {
                    int? value = board.GetCell(row, col).Value;

                    if (value.HasValue && !numbers.Add(value.Value))
                    {
                        violations.Add(
                            $"Number {value} already exists in Row {(char)('A' + row)}.");
                    }
                }
            }

            // Check columns
            for (int col = 0; col < SudokuBoard.Size; col++)
            {
                var numbers = new HashSet<int>();

                for (int row = 0; row < SudokuBoard.Size; row++)
                {
                    int? value = board.GetCell(row, col).Value;

                    if (value.HasValue && !numbers.Add(value.Value))
                    {
                        violations.Add(
                            $"Number {value} already exists in Column {col + 1}.");
                    }
                }
            }

            // Check 3x3 subgrids
            for (int boxRow = 0; boxRow < 9; boxRow += 3)
            {
                for (int boxCol = 0; boxCol < 9; boxCol += 3)
                {
                    var numbers = new HashSet<int>();

                    for (int row = boxRow; row < boxRow + 3; row++)
                    {
                        for (int col = boxCol; col < boxCol + 3; col++)
                        {
                            int? value = board.GetCell(row, col).Value;

                            if (value.HasValue && !numbers.Add(value.Value))
                            {
                                violations.Add(
                                    $"Number {value} already exists in the same 3x3 subgrid.");
                            }
                        }
                    }
                }
            }

            return violations;
        }
    }

}
