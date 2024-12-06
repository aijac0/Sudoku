namespace Sudoku_Console;

using Sudoku_Library;

class Program
{
    private static Game ParseArgs(string[] args)
    {
        // Invalid number of arguments
        if (args.Length < 2)
        {
            throw new FormatException("Invalid number of arguments");
        }
        
        // Try to parse size
        int size;
        if (!int.TryParse(args[0], out size))
        {
            throw new FormatException("Invalid size");
        }

        // Try to parse difficulty
        float fillPercent;
        switch (args[1].Trim().ToLower())
        {
            case "easy":
                fillPercent = 0.50f;
                break;
            case "m":
            case "medium":
                fillPercent = 0.35f;
                break;
            case "h":
            case "hard":
                fillPercent = 0.20f;
                break;
            default:
                throw new FormatException("Invalid difficulty");
        }

        // Create game
        return new Game(size, fillPercent);
    }

    static void Main(string[] args)
    {
        Game game;
        try
        {
            game = ParseArgs(args);
        }
        catch (FormatException)
        {
            string size = "9";
            string difficulty = "hard";
            Console.WriteLine("Using default arguments");
            Console.WriteLine($"Size: {size}");
            Console.WriteLine($"Difficulty: {difficulty}");
            game = ParseArgs([size, difficulty]);
        }
        
        game.Display();
        
    }
}