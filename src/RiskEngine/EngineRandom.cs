namespace RiskEngine;

// Deterministic, 100% Zero-Allocation RNG
public struct EngineRandom
{
    //Internal state must not be 0
    private uint _state;

    public int Seed { get; }

    public EngineRandom(int seed)
    {
        Seed = seed;
        // XorShift needs start value not 0
        _state = seed == 0 ? 1u : (uint)seed;
    }

    //XorShift 32 Algorithm
    private uint NextUInt()
    {
        uint x = _state;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        _state = x;
        return x;
    }

    // Next Random Range
    public int Next(int minInclusive, int maxExclusive)
    {
        uint range = (uint)(maxExclusive - minInclusive);
        return (int)(minInclusive + (NextUInt() % range));
    }

    // Roll Dice (1 to 6)
    public byte RollDice()
    {
        return (byte)(1 + (NextUInt() % 6));
    }

    // Fisher-Yates Shuffle Algorithm for zero Allocation
    public void Shuffle<T>(Span<T> span)
    {
        for (int i = span.Length - 1; i > 0; i--)
        {
            int j = (int)(NextUInt() % (uint)(i + 1));
            (span[i], span[j]) = (span[j], span[i]);
        }
    }
}