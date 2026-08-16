using System.Runtime.CompilerServices;

namespace RiskEngine.State;

// Deterministic, 100% Zero-Allocation inside Game Loop RNG
public sealed class EngineRandom
{
    private uint _state;

    public int Seed { get; }

    public EngineRandom(int seed)
    {
        Seed = seed;
        _state = seed == 0 ? 1u : (uint)seed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextUInt()
    {
        var x = _state;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        _state = x;
        return x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Next(int minInclusive, int maxExclusive)
    {
        var range = (uint)(maxExclusive - minInclusive);
        return (int)(minInclusive + NextUInt() % range);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte RollDice()
    {
        return (byte)(1 + NextUInt() % 6);
    }

    public void Shuffle<T>(Span<T> span)
    {
        for (var i = span.Length - 1; i > 0; i--)
        {
            var j = (int)(NextUInt() % (uint)(i + 1));
            (span[i], span[j]) = (span[j], span[i]);
        }
    }
}