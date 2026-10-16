namespace RiskEngine.Replay.Setup;

public static class SetupHelper
{
    public static bool AskYesNo(string question)
    {
        while (true)
        {
            Console.Write($"{question} [Y/N]: ");

            var input = Console.ReadLine()?.Trim().ToUpperInvariant();

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

    public static ulong AskRange(string question, ulong min, ulong max)
    {
        while (true)
        {
            Console.Write($"{question} ({min}-{max}): ");

            var input = Console.ReadLine();

            if (ulong.TryParse(input, out var value) &&
                value >= min &&
                value <= max)
                return value;

            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    public static T AskEnum<T>(string question) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();

        while (true)
        {
            Console.WriteLine(question);

            for (var i = 0; i < values.Length; i++) Console.WriteLine($"{i + 1}. {values[i]}");

            Console.Write("Selection: ");

            if (int.TryParse(Console.ReadLine(), out var selection) &&
                selection >= 1 &&
                selection <= values.Length)
                return values[selection - 1];

            Console.WriteLine("Invalid selection.");
            Console.WriteLine();
        }
    }
}