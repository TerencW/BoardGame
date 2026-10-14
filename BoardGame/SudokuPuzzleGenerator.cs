namespace BoardGame
{
    public class SudokuPuzzleGenerator
    {
        private const int Size = 9;
        private const int ClueCount = 30;

        public record GeneratedSudoku(int?[,] Puzzle,int[,] Solution);

        public GeneratedSudoku Generate()
        {
            // Retry if removal cannot reach 30 clues
            // while maintaining a unique solution.
            for (int attempt = 0; attempt < 100; attempt++)
            {
                var board = new int[Size, Size];
                FillBoard(board);
                int[,] solution = (int[,])board.Clone();
                int remaining = Size * Size;
                int[] positions = Enumerable.Range(0, 81).ToArray();
                Random.Shared.Shuffle(positions);

                foreach (int position in positions)
                {
                    if (remaining == ClueCount)
                        break;

                    int row = position / Size;
                    int col = position % Size;

                    int original = board[row, col];
                    board[row, col] = 0;

                    if (CountSolutions(board, 2) == 1)
                        remaining--;
                    else
                        board[row, col] = original;
                }

                if (remaining == ClueCount)
                {
                    var puzzle = new int?[Size, Size];

                    for (int row = 0; row < Size; row++)
                    {
                        for (int col = 0; col < Size; col++)
                        {
                            puzzle[row, col] =
                                board[row, col] == 0
                                    ? null
                                    : board[row, col];
                        }
                    }

                    return new GeneratedSudoku(puzzle, solution);
                }
            }

            throw new InvalidOperationException(
                "Unable to generate a unique Sudoku puzzle.");
        }

        private static bool FillBoard(int[,] board)
        {
            var empty = FindEmpty(board);

            if (empty == null)
                return true;

            var (row, col) = empty.Value;

            int[] numbers = Enumerable.Range(1, 9).ToArray();
            Random.Shared.Shuffle(numbers);

            foreach (int number in numbers)
            {
                if (!CanPlace(board, row, col, number))
                    continue;

                board[row, col] = number;

                if (FillBoard(board))
                    return true;

                board[row, col] = 0;
            }

            return false;
        }

        private static int CountSolutions(int[,] board, int limit)
        {
            var empty = FindEmpty(board);

            if (empty == null)
                return 1;

            var (row, col) = empty.Value;
            int count = 0;

            for (int number = 1; number <= 9; number++)
            {
                if (!CanPlace(board, row, col, number))
                    continue;

                board[row, col] = number;
                count += CountSolutions(board, limit - count);
                board[row, col] = 0;

                if (count >= limit)
                    break;
            }

            return count;
        }

        private static (int Row, int Col)? FindEmpty(int[,] board)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (board[row, col] == 0)
                        return (row, col);
                }
            }

            return null;
        }

        private static bool CanPlace(
            int[,] board, int row, int col, int number)
        {
            for (int i = 0; i < Size; i++)
            {
                if (board[row, i] == number ||
                    board[i, col] == number)
                    return false;
            }

            int startRow = row / 3 * 3;
            int startCol = col / 3 * 3;

            for (int r = startRow; r < startRow + 3; r++)
            {
                for (int c = startCol; c < startCol + 3; c++)
                {
                    if (board[r, c] == number)
                        return false;
                }
            }

            return true;
        }
    }
}
