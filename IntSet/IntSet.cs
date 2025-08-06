using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IntSet;

public class IntSet
{
    private const int PageBits = 6;
    private const int PageSize = 1 << PageBits;
    private const int PageMask = PageSize - 1;
    private const int MaxStackBytes = 128 * 1024; // How much stack space we can use for masks.
    private const int BytesPerPage = sizeof(ulong);
    private const int MaxPageCount = MaxStackBytes / BytesPerPage;

    private ulong[] _pages;
    private int _pageCount;
    private int _count;
    private uint _initialKey;
    private bool _isInitialized;

    public int Count => _count;

    public IntSet()
    {
        _pages = new ulong[16];
        _pageCount = 0;
    }

    public IntSet(Span<int> values)
    {
        _pages = new ulong[16];
        _pageCount = 0;
        UnionWith(values);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsurePage(int pageIndex)
    {
        if (pageIndex >= _pages.Length)
        {
            // double until big enough
            int newSize = Math.Max(_pages.Length * 2, pageIndex + 1);
            Array.Resize(ref _pages, newSize);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(int value)
    {
        uint zigzagValue = ZigZagEncode(value);
        _initialKey = _isInitialized ? _initialKey : zigzagValue;
        _isInitialized = true;
        uint key = zigzagValue - _initialKey;
        // Which page does this key belong to? Each page has 64 bits, so we can store 64 keys per page.
        int p = (int) (key >> PageBits);
        // which bit in the page do we set for this key?
        int bit = (int) (key & PageMask);
        // create the mask that we |= to set the bit
        ulong mask = 1UL << bit;
        // make sure the page exists in our _pages array
        EnsurePage(p);
        ulong page = _pages[p];
        _pages[p] |= mask;
        // if we have created a new page, increment pageCount
        _pageCount = p >= _pageCount ? p + 1 : _pageCount;
        // return true if we set the bit, false if it was already set
        if ((page & mask) != 0)
            return false;
        _count++;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int value)
    {
        if (!_isInitialized) return false;
        uint key = ZigZagEncode(value) - _initialKey;
        // get page
        int p = (int) (key >> PageBits);
        if (p >= _pageCount) return false;
        // get bit position
        int bit = (int) (key & PageMask);
        ulong mask = (1UL << bit);
        return p < _pages.Length && (_pages[p] & mask) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(int value)
    {
        if (!_isInitialized) return false;
        uint key = ZigZagEncode(value) - _initialKey;
        // get page
        int p = (int) (key >> PageBits);
        if (p >= _pageCount) return false;
        // get bit position
        int bit = (int) (key & PageMask);
        ulong mask = 1UL << bit;
        ulong page = _pages[p];
        if ((page & mask) == 0)
            return false;

        ulong newPage = page & ~mask;
        _pages[p] = newPage;

        _count--;
        return true;
    }

    public void IntersectWith(Span<int> span)
    {
        if (!_isInitialized)
        {
            return; // Empty set intersected with anything is empty
        }

        int pageCount = _pageCount;

        Span<ulong> masks = pageCount > MaxPageCount ? new ulong[pageCount] : stackalloc ulong[pageCount];
        if (pageCount > MaxPageCount)
            masks.Clear();

        foreach (int value in span)
        {
            uint key = ZigZagEncode(value) - _initialKey;
            // get page
            int p = (int) (key >> PageBits);
            if (p >= pageCount) continue;
            // get bit position
            int bit = (int) (key & PageMask);
            ulong mask = 1UL << bit;
            masks[p] |= mask;
        }

        for (int i = 0; i < _pageCount; i++)
            _pages[i] &= masks[i];

        int newCount = 0;

        for (int i = 0; i < _pageCount; i++)
        {
            // intersect the page
            ulong bits = _pages[i] & masks[i];
            _pages[i] = bits;

            if (bits != 0)
            {
                newCount += PopCount(bits);
            }
        }

        _count = newCount;
    }

    public void UnionWith(Span<int> span)
    {
        foreach (int v in span)
        {
            Add(v);
        }
    }

    public void ExceptWith(Span<int> span)
    {
        foreach (int v in span)
        {
            Remove(v);
        }
    }

    public void SymmetricExceptWith(Span<int> span)
    {
        if (!_isInitialized)
        {
            // If set is empty, symmetric except is just union
            UnionWith(span);
            return;
        }

        int maxPage = _pageCount;
        foreach (int value in span)
        {
            uint key = ZigZagEncode(value) - _initialKey;
            int p = (int) (key >> PageBits);
            if (p + 1 > maxPage) maxPage = p + 1;
        }

        if (maxPage > _pages.Length)
            Array.Resize(ref _pages, maxPage);
        _pageCount = maxPage;

        Span<ulong> masks = maxPage > MaxPageCount ? new ulong[maxPage] : stackalloc ulong[maxPage];
        if (maxPage > MaxPageCount)
            masks.Clear();

        foreach (int value in span)
        {
            uint key = ZigZagEncode(value) - _initialKey;
            int p = (int) (key >> PageBits);
            int bit = (int) (key & PageMask);
            masks[p] |= 1UL << bit;
        }

        int newCount = 0;

        for (int p = 0; p < maxPage; p++)
        {
            ulong oldBits = _pages[p];
            ulong flip = masks[p];
            if (flip != 0UL)
                _pages[p] = oldBits ^ flip;

            ulong bits = _pages[p];
            if (bits != 0UL)
            {
                newCount += PopCount(bits);
            }
        }

        _count = newCount;
    }

    public Enumerator GetEnumerator() => new Enumerator(_pages, _pageCount, _initialKey);

    public struct Enumerator
    {
        private readonly ulong[] _pages;
        private readonly int _pageCount;
        private int _currentPageIndex;
        private ulong _currentBits;
        private int _currentPageBase;
        private readonly uint _initialKey;

        public int Current { get; private set; }

        internal Enumerator(ulong[] pages, int pageCount, uint initialKey)
        {
            _pages = pages;
            _pageCount = pageCount;
            _currentPageIndex = -1;
            _currentBits = 0;
            _currentPageBase = 0;
            _initialKey = initialKey;
            Current = 0;
        }

        public bool MoveNext()
        {
            // Extract remaining bits from current page
            if (_currentBits != 0)
            {
                int tz = TrailingZeroCount(_currentBits);
                _currentBits &= _currentBits - 1; // Clear lowest bit
                Current = ZigZagDecode((uint) (_currentPageBase | tz) + _initialKey);
                return true;
            }

            // Move to next page
            int pageCount = _pageCount;
            while (++_currentPageIndex < pageCount)
            {
                _currentBits = _pages[_currentPageIndex];
                if (_currentBits == 0) continue;
                _currentPageBase = _currentPageIndex << PageBits;
                int tz = TrailingZeroCount(_currentBits);
                _currentBits &= _currentBits - 1; // Clear lowest bit
                Current = ZigZagDecode((uint) (_currentPageBase | tz) + _initialKey);
                return true;
            }

            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ZigZagEncode(int v)
    {
        return ((uint) (v << 1)) ^ ((uint) (v >> 31));
    }

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
        List<int> list = new List<int>();
        foreach (int i in this)
            list.Add(i);
        return list.ToArray();
    }

    public List<int> ToList()
    {
        List<int> list = new List<int>();
        foreach (int i in this)
            list.Add(i);
        return list;
    }

    public void Clear()
    {
        for (int i = 0; i < _pageCount; i++)
            _pages[i] = 0ul;
        _pageCount = 0;
        _count = 0;
        _initialKey = 0;
        _isInitialized = false;
    }
}