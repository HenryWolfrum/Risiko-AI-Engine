namespace RiskEngine;

//Deterministic RNG Sequence
public class EngineRandom
{
    private Random _rng;

    public int Seed { get; }

    public EngineRandom(int seed)
    {
        Seed = seed;
        _rng = new Random(seed);
    }

    //Next Random Range
    public int Next(int minInclusive, int maxExclusive)
    {
        return _rng.Next(minInclusive, maxExclusive);
    }

    //Roll Dice
    public byte RollDice()
    {
        return (byte)_rng.Next(1, 7);
    }

    //Fisher-Yates Shuffle Algorithm for zero Allocation
    public void Shuffle<T>(Span<T> span)
    {
        for (int i = span.Length - 1; i > 0; i--)
        {
            int j = _rng.Next(0, i + 1);
            (span[i], span[j]) = (span[j], span[i]);
        }
    }
}