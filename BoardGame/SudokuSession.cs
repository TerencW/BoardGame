namespace BoardGame;

public class SudokuSession(Func<SudokuGame> createGame)
{
    public void Run()
    {
        while (createGame().Run())
        {
            Console.WriteLine("Press Enter to play again, or type quit to exit.");
            string? input = Console.ReadLine();
            if (input == null || input.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
                return;
        }
    }
}
