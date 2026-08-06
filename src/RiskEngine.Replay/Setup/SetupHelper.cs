namespace RiskEngine.Replay.Setup;

public static class SetupHelper
{
    public static bool AskYesNo(string question)
    {
        while (true)
        {
            Console.Write($"{question} [Y/N]: ");

            string? input = Console.ReadLine()?.Trim().ToUpperInvariant();

            switch (input)
            {
                case "Y":
                case "YES":
                    return true;

                case "N":
                case "NO":
                    return false;

                default:
                    Console.WriteLine("Please enter Y or N.");
                    break;
            }
        }
    }

    public static int AskInt(string question, int min, int max)
    {
        while (true)
        {
            Console.Write($"{question} ({min}-{max}): ");

            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) &&
                value >= min &&
                value <= max)
            {
                return value;
            }

            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    public static T AskEnum<T>(string question) where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();

        while (true)
        {
            Console.WriteLine(question);

            for (int i = 0; i < values.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {values[i]}");
            }

            Console.Write("Selection: ");

            if (int.TryParse(Console.ReadLine(), out int selection) &&
                selection >= 1 &&
                selection <= values.Length)
            {
                return values[selection - 1];
            }

            Console.WriteLine("Invalid selection.");
            Console.WriteLine();
        }
    }
}