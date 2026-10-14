# Sudoku command-line app

A C# console application that generates Sudoku puzzles with exactly 30
pre-filled numbers. Players can enter numbers, clear cells, request hints,
check rule violations, and play again after completing a puzzle.

## Environment

- .NET 10 SDK (a runtime alone cannot build the source).
- Windows, Linux, or macOS with a terminal and the .NET 10 SDK installed.
- Internet access on the first test run to restore the testing packages from NuGet.

The grid uses ANSI escape sequences to highlight pre-filled numbers. Use a
terminal that supports ANSI colors for the intended display.

## Build and run

Open a terminal in the folder containing `BoardGame.slnx` and run:

```sh
dotnet build BoardGame.slnx
dotnet run --project BoardGame/BoardGame.csproj
```

The application displays a new 9-by-9 puzzle. Rows are labeled A through I,
columns 1 through 9, and empty cells are displayed as `_`.

## Commands

| Command | Behavior |
| --- | --- |
| `A3 4` | Place 4 in row A, column 3. |
| `C5 clear` | Clear a player-editable cell. |
| `hint` | Fill one randomly selected empty cell with its correct solution value. |
| `check` | Report duplicate numbers in rows, columns, and 3-by-3 subgrids. |
| `violation clear` | Clear all player entries currently highlighted in red. |
| `quit` | Exit the game. |

Row letters and command words are case-insensitive. Numbers must be between
1 and 9. Pre-filled cells cannot be changed or cleared.

`check` highlights player entries involved in duplicate conflicts in red.
Pre-filled clues retain their blue highlight. Both editable entries are marked
when they conflict with each other; a conflict does not establish which entry
is the correct solution value. New or replaced entries are not highlighted
until the next `check`, or the automatic completion check when the grid is full.
An invalid full grid highlights conflicts using the same rules as `check`.
Resolved highlights disappear when the grid is displayed.
`violation clear` clears only the currently highlighted entries, preserving
fixed clues and unchecked entries.

When the grid is full and has no rule violations, the application announces
completion. Press Enter to generate a fresh puzzle, or type `quit` to exit.
End-of-input also exits cleanly.

## Design

- `SudokuCell` stores a cell's value and protects its pre-filled status.
- `SudokuBoard` owns the cells and provides placement, clearing, and fullness checks.
- `SudokuValidator` detects duplicate values in rows, columns, and subgrids.
- `SudokuRenderer` displays the grid.
- `SudokuPuzzleGenerator` generates a complete solution through randomized backtracking, then removes
  numbers while checking that exactly one solution remains.
- `SudokuGame` processes commands, supplies hints from the stored solution,
  and reports whether a game ended successfully.
- `SudokuSession` manages replay and requests a fresh game after completion.
- `Program.cs` creates and connects the application components.

The application uses the .NET standard library for gameplay and generation.
External packages are used only for automated testing.

## Assumptions

- Conflicting numbers are accepted as player moves. The `check` command reports
  violations, matching the problem statement's examples.
- Completion requires every cell to contain a number and no row, column, or
  subgrid to contain duplicates. An invalid full grid remains editable.
- Hints fill an empty cell and preserve existing player entries, even if those
  entries are incorrect. Hints do not become fixed clues and can be cleared.
- Generated puzzles have exactly 30 fixed clues and a unique solution. The
  generator retries up to 100 times, then throws if generation fails.
- Replay uses Enter rather than the sample's any-key prompt.

## Automated tests

From the repository root, run:

```sh
dotnet test BoardGame.Tests/BoardGame.Tests.csproj
```

The xUnit tests cover the problem statement's sample moves and violations,
fixed-cell protection, clearing, invalid input, hints, completion, quitting,
end-of-input, replay, and generated puzzles with 30 clues and a matching valid
solution. Console tests run sequentially because console streams are global.
The generator test does not independently prove solution uniqueness.

See `BoardGame.Tests/README.md` for further testing details.

## Source submission

Create a ZIP containing `BoardGame.slnx`, this README, and the `BoardGame` and
`BoardGame.Tests` source folders, including their project files and the problem
statement. Exclude all `bin` and `obj` folders, compiled binaries, and installer
files. The recipient can build and test the application using the commands above.
