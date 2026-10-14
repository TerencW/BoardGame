using BoardGame;
using BoardGame.Sudoku;
using Xunit;

// The game uses global Console streams; keep these tests sequential.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BoardGame.Tests;

public class SudokuTests
{
    private static readonly string[] Puzzle =
    ["53__7____", "6__195___", "_98____6_", "8___6___3", "4__8_3__1",
     "7___2___6", "_6____28_", "___419__5", "____8__79"];

    private static readonly string[] Solution =
    ["534678912", "672195348", "198342567", "859761423", "426853791",
     "713924856", "961537284", "287419635", "345286179"];

    private static int?[,] Parse(string[] rows)
    {
        var values = new int?[9, 9];
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                values[r, c] = rows[r][c] == '_' ? null : rows[r][c] - '0';
        return values;
    }

    private static int[,] CorrectSolution()
    {
        var values = new int[9, 9];
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                values[r, c] = Solution[r][c] - '0';
        return values;
    }

    private static string Play(SudokuBoard board, params string[] commands)
        => Capture(() => new SudokuGame(board, new SudokuRenderer(), new SudokuValidator(), CorrectSolution()).Run(), commands);

    private static string Capture(Action run, params string[] commands)
    {
        var originalInput = Console.In;
        var originalOutput = Console.Out;
        using var input = new StringReader(string.Join(Environment.NewLine, commands));
        using var output = new StringWriter();
        try
        {
            Console.SetIn(input);
            Console.SetOut(output);
            run();
            return output.ToString();
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }

    [Fact]
    public void InitialDisplayShowsSamplePuzzleWithThirtyFixedClues()
    {
        var board = new SudokuBoard(Parse(Puzzle));
        var output = Play(board, "quit");
        Assert.Contains("Welcome to Sudoku!", output);
        Assert.Contains("1 2 3", output);
        Assert.Contains("_", output);
        int fixedCount = 0;
        for (int r = 0; r < 9; r++)
        {
            Assert.Contains($" {(char)('A' + r)}  ", output);
            for (int c = 0; c < 9; c++)
                if (board.GetCell(r, c).IsFixed) fixedCount++;
        }
        Assert.Equal(30, fixedCount);
    }

    [Fact]
    public void SampleAcceptedMoveAndCheck()
    {
        var board = new SudokuBoard(Parse(Puzzle));
        var output = Play(board, "A3 4", "check", "quit");
        Assert.Equal(4, board.GetCell(0, 2).Value);
        Assert.Contains("Move accepted.", output);
        Assert.Contains("No rule violations detected.", output);
        Assert.DoesNotContain("successfully completed", output);
    }

    [Theory]
    [InlineData("A1 6")]
    [InlineData("A1 clear")]
    public void FixedClueCannotBeChangedOrCleared(string command)
    {
        var board = new SudokuBoard(Parse(Puzzle));
        Assert.Contains("Invalid move. A1 is pre-filled.", Play(board, command, "quit"));
        Assert.Equal(5, board.GetCell(0, 0).Value);
    }

    [Fact]
    public void PlayerCanClearC5()
    {
        var board = new SudokuBoard(Parse(Puzzle));
        var output = Play(board, "C5 4", "C5 clear", "quit");
        Assert.Null(board.GetCell(2, 4).Value);
        Assert.Contains("Cell C5 cleared.", output);
    }

    [Theory]
    [InlineData("A3 3", "Number 3 already exists in Row A.")]
    [InlineData("C1 5", "Number 5 already exists in Column 1.")]
    [InlineData("B3 8", "Number 8 already exists in the same 3x3 subgrid.")]
    public void SampleViolationsAreAcceptedThenReportedByCheck(string command, string violation)
    {
        var output = Play(new SudokuBoard(Parse(Puzzle)), command, "check", "quit");
        Assert.Contains("Move accepted.", output);
        Assert.Contains(violation, output);
        Assert.True(output.IndexOf("Move accepted.") < output.IndexOf(violation));
    }

    [Theory]
    [InlineData("A3 0")]
    [InlineData("A3 10")]
    [InlineData("A3 -1")]
    [InlineData("A3 abc")]
    [InlineData("J3 4")]
    [InlineData("A0 4")]
    [InlineData("A10 4")]
    [InlineData("A3")]
    [InlineData("A3 4 extra")]
    public void InvalidMoveLeavesCellEmpty(string command)
    {
        var board = new SudokuBoard(Parse(Puzzle));
        Assert.Contains("Invalid move.", Play(board, command, "quit"));
        Assert.Null(board.GetCell(0, 2).Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void BoardAlsoRejectsOutOfRangeNumbers(int value)
    {
        var board = new SudokuBoard(Parse(Puzzle));
        Assert.Throws<ArgumentOutOfRangeException>(() => board.PlaceNumber(0, 2, value));
        Assert.Null(board.GetCell(0, 2).Value);
    }

    [Fact]
    public void HintRevealsExactlyOneCorrectEmptyCellAndPreservesOtherCells()
    {
        var board = new SudokuBoard(Parse(Puzzle));
        board.PlaceNumber(0, 2, 4);
        var before = new int?[9, 9];
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++) before[r, c] = board.GetCell(r, c).Value;
        Assert.Contains("Hint: Cell ", Play(board, "hint", "quit"));
        int changed = 0;
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (before[r, c] != board.GetCell(r, c).Value)
                {
                    changed++;
                    Assert.Null(before[r, c]);
                    Assert.Equal(Solution[r][c] - '0', board.GetCell(r, c).Value);
                }
        Assert.Equal(1, changed);
    }

    [Theory]
    [InlineData("I4 2")]
    [InlineData("hint")]
    public void LastCorrectMoveOrHintCompletesGame(string command)
    {
        var puzzle = Parse(Solution);
        puzzle[8, 3] = null;
        var board = new SudokuBoard(puzzle);
        var output = Play(board, command, "A1 6");
        Assert.True(board.IsFull());
        Assert.Contains("You have successfully completed the Sudoku puzzle!", output);
        Assert.DoesNotContain("A1 is pre-filled", output);
    }

    [Fact]
    public void FullGridWithViolationDoesNotWinAndCanBeCorrected()
    {
        var puzzle = Parse(Solution);
        puzzle[8, 3] = null;
        var board = new SudokuBoard(puzzle);
        var invalidOutput = Play(board, "I4 3", "hint", "quit");
        Assert.True(board.IsFull());
        Assert.Contains("already exists", invalidOutput);
        Assert.Contains("No empty cells available for a hint.", invalidOutput);
        Assert.DoesNotContain("successfully completed", invalidOutput);
        Assert.Contains("successfully completed", Play(board, "I4 2"));
    }

    [Theory]
    [InlineData("quit")]
    [InlineData(" QUIT ")]
    public void QuitStopsBeforeFurtherMoves(string quit)
    {
        var board = new SudokuBoard(Parse(Puzzle));
        Play(board, quit, "A3 4");
        Assert.Null(board.GetCell(0, 2).Value);
    }

    [Fact]
    public void EndOfInputExitsCleanly()
    {
        Assert.Contains("Welcome to Sudoku!", Play(new SudokuBoard(Parse(Puzzle))));
    }

    [Fact]
    public void CompletedPuzzleCanReplayWithFreshBoardAndThenQuit()
    {
        var puzzle = Parse(Solution);
        puzzle[8, 3] = null;
        var first = new SudokuBoard(puzzle);
        var second = new SudokuBoard(Parse(Puzzle));
        int gamesCreated = 0;
        var session = new SudokuSession(() => new SudokuGame(
            ++gamesCreated == 1 ? first : second,
            new SudokuRenderer(), new SudokuValidator(), CorrectSolution()));

        var output = Capture(session.Run, "I4 2", "", "A3 4", "quit");

        Assert.Equal(2, gamesCreated);
        Assert.Contains("Press Enter to play again", output);
        Assert.True(first.IsFull());
        Assert.False(second.IsFull());
        Assert.Equal(4, second.GetCell(0, 2).Value);
    }

    [Theory]
    [InlineData("I4 2", "quit", true)]
    [InlineData("I4 2", null, true)]
    [InlineData("I4 3", "quit", false)]
    [InlineData("quit", null, false)]
    public void ReplayIsOfferedOnlyAfterCorrectCompletion(string move, string? next, bool offered)
    {
        var puzzle = Parse(Solution);
        puzzle[8, 3] = null;
        int gamesCreated = 0;
        var session = new SudokuSession(() =>
        {
            gamesCreated++;
            return new SudokuGame(new SudokuBoard(puzzle), new SudokuRenderer(),
                new SudokuValidator(), CorrectSolution());
        });
        var output = Capture(session.Run, next == null ? [move] : [move, next]);
        Assert.Equal(offered, output.Contains("Press Enter to play again"));
        Assert.Equal(1, gamesCreated);
    }

    [Fact]
    public void InvalidFinalMoveHighlightsConflictsAndAllowsViolationClearWithoutCheckCommand()
    {
        var puzzle = Parse(Solution);
        puzzle[8, 3] = null;
        var board = new SudokuBoard(puzzle);
        var output = Play(board, "I4 3", "violation clear", "quit");
        Assert.Contains("\x1b[1;31m3\x1b[0m", output);
        Assert.Contains("Cleared 1 highlighted violation cell(s).", output);
        Assert.Null(board.GetCell(8, 3).Value);
        Assert.Equal(3, board.GetCell(8, 0).Value);
        Assert.DoesNotContain("successfully completed", output);
    }

    [Fact]
    public void ConflictsOnlyTurnRedAfterCheckAndClearProtectsClues()
    {
        var board = new SudokuBoard(Parse(Puzzle));
        var output = Play(board, "A3 3", "check", "violation clear", "quit");
        int checkResult = output.IndexOf("Number 3 already exists");
        Assert.DoesNotContain("\x1b[1;31m", output[..checkResult]);
        Assert.Contains("\x1b[1;31m3\x1b[0m", output[checkResult..]);
        Assert.Null(board.GetCell(0, 2).Value);
        Assert.Equal(3, board.GetCell(0, 1).Value);
        Assert.Contains("Cleared 1 highlighted violation cell(s).", output);
    }

    [Fact]
    public void ViolationClearLeavesNewUncheckedConflictsAndValidEntries()
    {
        var board = new SudokuBoard(Parse(Puzzle));
        Play(board, "A3 3", "check", "C1 5", "B2 7", "violation clear", "quit");
        Assert.Null(board.GetCell(0, 2).Value);
        Assert.Equal(5, board.GetCell(2, 0).Value);
        Assert.Equal(7, board.GetCell(1, 1).Value);
    }

    [Fact]
    public void CheckHighlightsBothEditableEntriesInDuplicatePair()
    {
        var board = new SudokuBoard(new int?[9, 9]);
        var output = Play(board, "A1 4", "A2 4", "check", "violation clear", "quit");
        Assert.Contains("\x1b[1;31m4\x1b[0m", output);
        Assert.Contains("Cleared 2 highlighted violation cell(s).", output);
        Assert.Null(board.GetCell(0, 0).Value);
        Assert.Null(board.GetCell(0, 1).Value);
    }

    [Fact]
    public void CorrectingHighlightedEntryPreservesReplacementDuringViolationClear()
    {
        var board = new SudokuBoard(Parse(Puzzle));
        var output = Play(board, "A3 3", "check", "A3 4", "violation clear", "quit");
        Assert.Equal(4, board.GetCell(0, 2).Value);
        Assert.Contains("Cleared 0 highlighted violation cell(s).", output);
    }

    [Fact]
    public void GeneratedPuzzleHasThirtyCluesAndMatchingValidSolution()
    {
        var generated = new SudokuPuzzleGenerator().Generate();
        var solved = new int?[9, 9];
        int clues = 0;
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
            {
                Assert.InRange(generated.Solution[r, c], 1, 9);
                solved[r, c] = generated.Solution[r, c];
                if (generated.Puzzle[r, c].HasValue)
                {
                    clues++;
                    Assert.Equal(generated.Solution[r, c], generated.Puzzle[r, c]);
                }
            }
        Assert.Equal(30, clues);
        Assert.Empty(new SudokuValidator().Check(new SudokuBoard(generated.Puzzle)));
        Assert.Empty(new SudokuValidator().Check(new SudokuBoard(solved)));
    }
}
