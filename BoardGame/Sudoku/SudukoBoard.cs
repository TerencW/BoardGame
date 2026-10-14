namespace BoardGame.Sudoku
{
    public class SudokuBoard
    {
        public const int Size = 9;
        private readonly SudokuCell[,] _cells = new SudokuCell[Size, Size];

        public SudokuBoard(int?[,] puzzle)
        {
            ArgumentNullException.ThrowIfNull(puzzle);

            if (puzzle.GetLength(0) != Size ||
                puzzle.GetLength(1) != Size)
            {
                throw new ArgumentException("Puzzle must be 9x9.");
            }

            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    _cells[row, col] =
                        new SudokuCell(row, col, puzzle[row, col]);
                }
            }
        }

        public SudokuCell GetCell(int row, int col)
        {
            if (row < 0 || row >= Size)
                throw new ArgumentOutOfRangeException(nameof(row));

            if (col < 0 || col >= Size)
                throw new ArgumentOutOfRangeException(nameof(col));

            return _cells[row, col];
        }

        public void PlaceNumber(int row, int col, int value)
        {
            SudokuCell cell = GetCell(row, col);
            cell.SetValue(value);
        }

        public bool IsFull()
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (GetCell(row, col).Value == null)
                        return false;
                }
            }

            return true;
        }

        public void ClearCell(int row, int col)
        {
            SudokuCell cell = GetCell(row, col);
            cell.Clear();
        }
    }

}
