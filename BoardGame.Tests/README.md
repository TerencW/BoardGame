# Sudoku automated tests

Requires the .NET 10 SDK. From the repository root, run:

```sh
dotnet test BoardGame.Tests/BoardGame.Tests.csproj
```

The first run restores xUnit and the .NET test runner from NuGet.

The tests use the puzzle and solution from SudokuProblemStatement.md and
exercise the real command loop with captured console input and output.
They cover accepted moves, fixed-cell protection, clearing, all three sample
duplicate violations, invalid numbers and positions, hints, completion,
quitting, end-of-input, and replay after correct completion. Replay tests check
that the next game uses a fresh board and that quitting or an invalid full grid
does not start another game. Generator coverage checks exactly 30 clues,
agreement with the stored solution, and valid puzzle/solution grids.

Hints are random, so the tests verify that exactly one empty cell receives
its correct solution value rather than expecting a particular cell.
Console tests run sequentially and restore the original streams after each run.
The generator test does not independently prove solution uniqueness.
