using BoardGame;
using BoardGame.Sudoku;


var generator = new SudokuPuzzleGenerator();
var renderer = new SudokuRenderer();
var validator = new SudokuValidator();

var session = new SudokuSession(() =>
{
    var generated = generator.Generate();
    return new SudokuGame(
        new SudokuBoard(generated.Puzzle),
        renderer,
        validator,
        generated.Solution);
});

session.Run();



