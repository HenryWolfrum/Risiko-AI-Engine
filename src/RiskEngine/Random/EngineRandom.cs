using System;
using System.Runtime.CompilerServices;

namespace RiskEngine.State;

// Deterministic, 100% Zero-Allocation, Fully Unbiased Xoroshiro128++ Generator
public sealed class EngineRandom
{
    private ulong _s0;
    private ulong _s1;

    public EngineRandom(ulong seed)
    {
        _s0 = SplitMix64(ref seed);
        _s1 = SplitMix64(ref seed);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong NextUInt64()
    {
        var s0 = _s0;
        var s1 = _s1;
        var result = RotateLeft(s0 + s1, 17) + s0;

        s1 ^= s0;
        _s0 = RotateLeft(s0, 49) ^ s1 ^ (s1 << 21);
        _s1 = RotateLeft(s1, 28);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Next(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive)
            throw new ArgumentOutOfRangeException(nameof(minInclusive), "minInclusive must be smaller than maxExclusive.");

        var range = (uint)(maxExclusive - minInclusive);
        return minInclusive + (int)NextBoundedUInt32(range);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte RollDice()
    {
        return (byte)(1 + NextBoundedUInt32(6u));
    }

    // Unbiased Lemire's Fast Range Reduction mit Rejection Sampling
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint NextBoundedUInt32(uint range)
    {
        var x = (uint)NextUInt64();
        var m = (ulong)x * range;
        var l = (uint)m;

        if (l < range)
        {
            var t = (uint)-(int)range % range; // Rejection Threshold
            while (l < t)
            {
                x = (uint)NextUInt64();
                m = (ulong)x * range;
                l = (uint)m;
            }
        }

        return (uint)(m >> 32);
    }

    public void Shuffle<T>(Span<T> span)
    {
        for (var i = span.Length - 1; i > 0; i--)
        {
            var j = Next(0, i + 1);
            (span[i], span[j]) = (span[j], span[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RotateLeft(ulong x, int k) => (x << k) | (x >> (64 - k));

    private static ulong SplitMix64(ref ulong state)
    {
        var z = state += 0x9E3779B97F4A7C15ul;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ul;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBul;
        return z ^ (z >> 31);
    }
}