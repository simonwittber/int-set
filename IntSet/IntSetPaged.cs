using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace IntSet;

/// <summary>
/// A memory-efficient set for storing integers, optimized for dense ranges of integer keys.
/// Note, a sparse, large range is not stored efficiently.
/// Use this class for fast membership checks and set operations on integers.
/// </summary>
public class IntSetPaged : IEnumerable<int>
{
    private const int PageBits = 10;
    private const int PageSize = 1 << PageBits;
    private const int PageMask = PageSize - 1;

    private List<ulong[]> _pages;
    private int _count;

    public int Count => _count;

    public IntSetPaged()
    {
        _pages = new();
    }

    public IntSetPaged(Span<int> values)
    {
        _pages = new();
        UnionWith(values);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsurePage(int pageIndex)
    {
        while (_pages.Count <= pageIndex)
            _pages.Add(null);
        _pages[pageIndex] ??= new ulong[PageSize];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(int value)
    {
        // convert the value to a key, which is a uint integer
        GetPageAndBit(value, out var pageIndex, out var slot, out var mask);
        // make sure the page exists in our _pages array
        EnsurePage(pageIndex);
        if (IsSet(pageIndex, slot, mask)) return false;
        SetBit(pageIndex, slot, mask);
        // if we have created a new page, increment pageCount
        _count++;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetBit(int p, int i, ulong mask) => _pages[p][i] |= mask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UnSetBit(int p, int i, ulong mask) => _pages[p][i] &= ~mask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSet(int p, int i, ulong mask) => (_pages[p][i] & mask) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GetPageAndBit(int value, out int pageIndex, out int slotIndex, out ulong mask)
    {
        var key = ZigZagEncode(value);
        pageIndex = (int) (key >> PageBits);
        var localKey = (int) (key & PageMask);
        slotIndex = localKey >> 6; // Divide by 64 to get ulong slot
        var bitIndex = localKey & 63; // Mod 64 to get bit position within ulong
        mask = 1ul << bitIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var slot, out var mask);
        return pageIndex < _pages.Count && _pages[pageIndex] != null && IsSet(pageIndex, slot, mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var slot, out var mask);
        if (pageIndex >= _pages.Count || _pages[pageIndex] == null) return false;
        if (IsSet(pageIndex, slot, mask))
        {
            UnSetBit(pageIndex, slot, mask);
            _count--;
            return true;
        }

        return false;
    }

    public void IntersectWith(Span<int> span)
    {
        var set = new IntSetPaged();
        foreach (var value in span)
        {
            set.Add(value);
        }

        IntersectWith(set);
    }

    public void UnionWith(Span<int> span)
    {
        foreach (var v in span)
        {
            Add(v);
        }
    }

    public void ExceptWith(Span<int> span)
    {
        foreach (var v in span)
        {
            Remove(v);
        }
    }

    public void SymmetricExceptWith(Span<int> span)
    {
        var set = new IntSetPaged();
        foreach (var value in span)
        {
            set.Add(value);
        }

        SymmetricExceptWith(set);
    }

    public void Clear()
    {
        for (var i = 0; i < _pages.Count; i++)
            _pages[i] = null;
        _count = 0;
    }

    public void UnionWith(IntSetPaged other)
    {
        EnsurePage(other._pages.Count);
        for (var i = 0; i < other._pages.Count; i++)
        {
            if (other._pages[i] == null) continue;
            if (_pages[i] == null)
            {
                _pages[i] = new ulong[PageSize];
                Array.Copy(other._pages[i], _pages[i], PageSize);
            }
            else
            {
                for (var k = 0; k < PageSize; k++)
                {
                    _pages[i][k] |= other._pages[i][k];
                }
            }
        }

        Recount();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Recount()
    {
        _count = 0;
        foreach (var page in _pages)
        {
            if (page == null) continue;
            foreach (var slot in page)
                _count += PopCount(slot);
        }
    }


    public void IntersectWith(IntSetPaged other)
    {
        if (_count == 0 || other._count == 0)
        {
            Clear();
            return;
        }

        for (var i = 0; i < _pages.Count; i++)
        {
            if (_pages[i] == null) continue;

            if (i >= other._pages.Count || other._pages[i] == null)
            {
                _pages[i] = null;
                continue;
            }

            for (var k = 0; k < PageSize; k++)
            {
                var localMap = _pages[i][k];
                var otherMap = other._pages[i][k];
                if (localMap == 0 || otherMap == 0)
                {
                    _pages[i][k] = 0;
                    continue;
                }

                var newMap = localMap & otherMap;
                _pages[i][k] = newMap;
            }
        }

        Recount();
    }

    public void ExceptWith(IntSetPaged other)
    {
        for (var i = 0; i < other._pages.Count; i++)
        {
            if (i > _pages.Count - 1) break; // Avoid out of bounds
            if (other._pages[i] == null) continue;
            if (_pages[i] == null)
                continue;
            for (var k = 0; k < PageSize; k++)
            {
                _pages[i][k] &= ~other._pages[i][k];
            }
        }

        Recount();
    }

    public void SymmetricExceptWith(IntSetPaged other)
    {
        if (other._count == 0) return;
        if (_count == 0)
        {
            UnionWith(other);
            return;
        }
        while (_pages.Count < other._pages.Count)
            _pages.Add(null);
        
        for (var i = 0; i < other._pages.Count; i++)
        {
            if (other._pages[i] == null) continue;
            if (_pages[i] == null)
            {
                _pages[i] = new ulong[PageSize];
                Array.Copy(other._pages[i], _pages[i], PageSize);
                continue;
            }
            for (var k = 0; k < PageSize; k++)
            {
                _pages[i][k] ^= other._pages[i][k];
            }
        }

        Recount();
    }

    public struct Enumerator : IEnumerator<int>
{
    private readonly List<ulong[]> _pages;
    private int _pageIndex;    // which page we’re on
    private int _slotIndex;    // index of the next slot to load within the current page
    private ulong _slot;       // current 64-bit slot being consumed
    private int _baseValue;    // base for the current slot (pageOffset + slotOffset)
    private int _current;      // decoded current value

    internal Enumerator(List<ulong[]> pages)
    {
        _pages = pages;
        _pageIndex = 0;
        _slotIndex = 0;
        _slot = 0;
        _baseValue = 0;
        _current = default;
    }

    public int Current => _current;
    object IEnumerator.Current => _current;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        // If we still have bits in the current slot, return the next one.
        if (_slot != 0)
            return NextFromCurrentSlot();

        // Otherwise, advance until we find a non-null page with a non-zero slot.
        while (_pageIndex < _pages.Count)
        {
            var page = _pages[_pageIndex];
            if (page == null)
            {
                _pageIndex++;
                _slotIndex = 0;
                continue;
            }

            // Walk slots in this page
            while (_slotIndex < page.Length)
            {
                _slot = page[_slotIndex];
                if (_slot != 0)
                {
                    // Prepare base for this slot and consume from it.
                    _baseValue = (_pageIndex << PageBits) + (_slotIndex << 6);
                    // Do NOT advance _slotIndex yet; we only move to the next slot
                    // once we've exhausted this one. We’ll increment below when done.
                    return NextFromCurrentSlot();
                }

                _slotIndex++;
            }

            // Page exhausted; move to next
            _pageIndex++;
            _slotIndex = 0;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool NextFromCurrentSlot()
    {
        // Extract next set bit
        int bitIndex = TrailingZeroCount(_slot);
        _current = ZigZagDecode((uint)(_baseValue + bitIndex));
        _slot &= ~(1ul << bitIndex); // clear that bit

        // If slot is now empty, advance slotIndex for next MoveNext()
        if (_slot == 0)
            _slotIndex++;

        return true;
    }

    public void Reset()
    {
        _pageIndex = 0;
        _slotIndex = 0;
        _slot = 0;
        _baseValue = 0;
        _current = default;
    }

    public void Dispose() { /* no resources */ }
}

// Pattern-based enumerator to avoid boxing on foreach(IntSet ...)
public Enumerator GetEnumerator() => new Enumerator(_pages);

// Interface implementations (these will box the struct if used via the interface)
IEnumerator<int> IEnumerable<int>.GetEnumerator() => new Enumerator(_pages);
IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_pages);


    /// <summary>
    /// Converts negative and positive integers to a non-negative integer using ZigZag encoding.
    /// -1 => 1, -2 => 3, 0 => 0, 1 => 2, 2 => 4, etc.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ZigZagEncode(int v)
    {
        return ((uint) (v << 1)) ^ ((uint) (v >> 31));
    }

    /// <summary>
    /// Converts a non-negative integer back to a signed integer using ZigZag decoding.
    /// 1 => -1, 3 => -2, 0 => 0, 2 => 1, 4 => 2, etc.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ZigZagDecode(uint u)
    {
        return (int) ((u >> 1) ^ -(u & 1));
    }

    private const ulong DeBruijnSequence = 0x37E84A99DAE458F;

    private static readonly int[] MultiplyDeBruijnBitPosition = {0, 1, 17, 2, 18, 50, 3, 57, 47, 19, 22, 51, 29, 4, 33, 58, 15, 48, 20, 27, 25, 23, 52, 41, 54, 30, 38, 5, 43, 34, 59, 8, 63, 16, 49, 56, 46, 21, 28, 32, 14, 26, 24, 40, 53, 37, 42, 7, 62, 55, 45, 31, 13, 39, 36, 6, 61, 44, 12, 35, 60, 11, 10, 9,};

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int TrailingZeroCount(ulong b)
    {
        return MultiplyDeBruijnBitPosition[((ulong) ((long) b & -(long) b) * DeBruijnSequence) >> 58];
    }

    /// <summary>
    /// Returns the number of bits set to 1 in the given ulong value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PopCount(ulong x)
    {
        x = x - ((x >> 1) & 0x5555_5555_5555_5555UL);
        x = (x & 0x3333_3333_3333_3333UL) + ((x >> 2) & 0x3333_3333_3333_3333UL);
        x = (x + (x >> 4)) & 0x0F0F_0F0F_0F0F_0F0FUL;
        return (int) ((x * 0x0101_0101_0101_0101UL) >> 56);
    }

    public int[] ToArray()
    {
        var array = new int[_count];
        var index = 0;
        foreach (var i in this)
        {
            array[index++] = i;
        }

        return array;
    }

    public List<int> ToList()
    {
        return new List<int>(ToArray());
    }

}