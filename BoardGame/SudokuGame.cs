using BoardGame.Sudoku;

namespace BoardGame
{
    public class SudokuGame
    {
        private readonly SudokuBoard _board;
        private readonly SudokuRenderer _renderer;
        private readonly SudokuValidator _validator;
        private readonly int[,] _solution;
        private readonly HashSet<(int Row, int Col)> _highlightedCells = new();

        public SudokuGame(
           SudokuBoard board,
           SudokuRenderer renderer,
           SudokuValidator validator,
           int[,] solution
           )
        {
            _board = board;
            _renderer = renderer;
            _validator = validator;
            _solution = solution;
        }

        public bool Run()
        {
            Console.WriteLine("Welcome to Sudoku!");
            _renderer.Display(_board);

            while (true)
            {
                Console.Write("\nEnter command (e.g., A3 4, C5 clear, hint, check, violation clear, quit): ");
                string? input = Console.ReadLine();

                if (input == null ||
                    input.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (input.Trim().Equals("check", StringComparison.OrdinalIgnoreCase))
                {
                    var violations = _validator.Check(_board);
                    HighlightConflicts();

                    if (violations.Count == 0)
                    {
                        Console.WriteLine("No rule violations detected.");
                    }
                    else
                    {
                        foreach (var violation in violations)
                            Console.WriteLine(violation);
                    }

                    DisplayBoard();
                    continue;
                }

                if (input.Trim().Equals("violation clear", StringComparison.OrdinalIgnoreCase))
                {
                    int cleared = _highlightedCells.Count;
                    foreach (var (highlightedRow, highlightedCol) in _highlightedCells)
                        _board.ClearCell(highlightedRow, highlightedCol);
                    _highlightedCells.Clear();
                    Console.WriteLine($"Cleared {cleared} highlighted violation cell(s).");
                    DisplayBoard();
                    continue;
                }

                if (input.Trim().Equals("hint", StringComparison.OrdinalIgnoreCase))
                {
                    ShowHint();
                    if (CheckCompletion())
                        return true;

                    continue;
                }
                string[] parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2 &&
                    parts[1].Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    if (!TryParsePosition(parts[0], out int rowClear, out int colClear))
                    {
                        Console.WriteLine("Invalid cell position.");
                        continue;
                    }

                    try
                    {
                        _board.ClearCell(rowClear, colClear);
                        _highlightedCells.Remove((rowClear, colClear));

                        Console.WriteLine($"Cell {parts[0].ToUpperInvariant()} cleared.");
                        DisplayBoard();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine(
                            $"Invalid move. {(char)('A' + rowClear)}{colClear + 1} is pre-filled.");
                    }

                    continue;
                }

                if (!TryParseMove(input, out int row, out int col, out int value))
                {
                    Console.WriteLine("Invalid move. command. Example: A3 4");
                    continue;
                }

                try
                {
                    _board.PlaceNumber(row, col, value);
                    _highlightedCells.Remove((row, col));
                    Console.WriteLine("Move accepted.");
                    DisplayBoard();
                    if (CheckCompletion())
                        return true;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Invalid move. {(char)('A' + row)}{col + 1} is pre-filled.");
                }
            }
            return false;
        }

        private bool CheckCompletion()
        {
            if (!_board.IsFull())
                return false;

            var violations = _validator.Check(_board);

            if (violations.Count > 0)
            {
                foreach (var violation in violations)
                    Console.WriteLine(violation);

                HighlightConflicts();
                DisplayBoard();
                return false;
            }

            Console.WriteLine(
                "You have successfully completed the Sudoku puzzle!");

            return true;
        }

        private void HighlightConflicts()
        {
            _highlightedCells.Clear();
            _highlightedCells.UnionWith(_validator.FindConflictingCells(_board));
        }

        private void DisplayBoard()
        {
            // Remove resolved highlights without marking newly entered conflicts.
            _highlightedCells.IntersectWith(_validator.FindConflictingCells(_board));
            _renderer.Display(_board, _highlightedCells);
        }

        private void ShowHint()
        {
            var emptyCells = new List<(int Row, int Col)>();

            for (int row = 0; row < SudokuBoard.Size; row++)
            {
                for (int col = 0; col < SudokuBoard.Size; col++)
                {
                    var cell = _board.GetCell(row, col);

                    // Skip both pre-filled and player-entered cells
                    if (cell.Value == null)
                        emptyCells.Add((row, col));
                }
            }

            if (emptyCells.Count == 0)
            {
                Console.WriteLine("No empty cells available for a hint.");
                return;
            }

            var (selectedRow, selectedCol) =
                emptyCells[Random.Shared.Next(emptyCells.Count)];

            int value = _solution[selectedRow, selectedCol];

            _board.PlaceNumber(selectedRow, selectedCol, value);

            Console.WriteLine(
                $"Hint: Cell {(char)('A' + selectedRow)}{selectedCol + 1} = {value}");

            DisplayBoard();
        }


        private static bool TryParseMove(
            string input, out int row, out int col, out int value)
        {
            row = col = value = 0;

            string[] parts = input.Split(
                ' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2 || parts[0].Length != 2)
                return false;

            char rowChar = char.ToUpperInvariant(parts[0][0]);
            char colChar = parts[0][1];

            if (rowChar < 'A' || rowChar > 'I' ||
                colChar < '1' || colChar > '9')
                return false;

            if (!int.TryParse(parts[1], out value) ||
                value < 1 || value > 9)
                return false;

            row = rowChar - 'A';
            col = colChar - '1';

            return true;
        }


        private static bool TryParsePosition( string position, out int row, out int col)
        {
            row = col = 0;

            if (position.Length != 2)
                return false;

            char rowChar = char.ToUpperInvariant(position[0]);
            char colChar = position[1];

            if (rowChar < 'A' || rowChar > 'I' ||
                colChar < '1' || colChar > '9')
                return false;

            row = rowChar - 'A';
            col = colChar - '1';

            return true;
        }
    }

}
