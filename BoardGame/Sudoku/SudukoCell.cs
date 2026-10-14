using BoardGame.Model;

namespace BoardGame.Sudoku
{
    public class SudokuCell : Cell
    {
        public int? Value { get; private set; }
        public bool IsFixed { get; }

        public SudokuCell(int row, int col, int? value)
        : base(row, col)
        {
            if (value is < 1 or > 9)
                throw new ArgumentOutOfRangeException(nameof(value));

            Value = value;
            IsFixed = value.HasValue;
        }

        public void SetValue(int value)
        {
            if (IsFixed)
                throw new InvalidOperationException(
                    "Cannot modify a pre-filled cell.");

            if (value is < 1 or > 9)
                throw new ArgumentOutOfRangeException(nameof(value));

            Value = value;
        }

        public void Clear()
        {
            if (IsFixed)
                throw new InvalidOperationException(
                    "Cannot clear a pre-filled cell.");

            Value = null;
        }


    }
}
