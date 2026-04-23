using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IntSet;

/// <summary>
/// A set of non-negative integer values. Keys must be >= 0.
/// Eliminates ZigZag encoding used by IntSet, halving the pages array
/// for equivalent value ranges and making all operations faster.
/// </summary>
public class UIntSet : IEnumerable<int>
{
    private const int PageBits = 6;
    private const int PageSize = 1 << PageBits;
    private const int PageMask = PageSize - 1;
    private const int MaxStackBytes = 128 * 1024;
    private const int BytesPerPage = sizeof(ulong);
    private const int MaxPageCount = MaxStackBytes / BytesPerPage;

    private ulong[] _pages;
    private int _pageCount;
    private int _count;

    public int Count => _count;
    public int PageCount => _pageCount;

    public UIntSet()
    {
        _pages = new ulong[16];
        _pageCount = 0;
    }

    public UIntSet(Span<int> values)
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
            var newSize = Math.Max(_pages.Length * 2, pageIndex + 1);
            Array.Resize(ref _pages, newSize);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GetPageAndBit(int value, out int pageIndex, out ulong mask)
    {
        var key = (uint)value;
        pageIndex = (int)(key >> PageBits);
        mask = 1ul << (int)(key & PageMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var mask);
        EnsurePage(pageIndex);
        if (Contains(pageIndex, mask)) return false;
        SetBit(pageIndex, mask);
        _pageCount = pageIndex >= _pageCount ? pageIndex + 1 : _pageCount;
        _count++;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetBit(int p, ulong mask) => _pages[p] |= mask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UnSetBit(int p, ulong mask) => _pages[p] &= ~mask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool Contains(int p, ulong mask) => (_pages[p] & mask) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var mask);
        return pageIndex < _pageCount && Contains(pageIndex, mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var mask);
        if (pageIndex >= _pageCount) return false;
        if (Contains(pageIndex, mask))
        {
            UnSetBit(pageIndex, mask);
            _count--;
            return true;
        }
        return false;
    }

    public void IntersectWith(Span<int> span)
    {
        var pageCount = _pageCount;
        var masks = pageCount > MaxPageCount ? new ulong[pageCount] : stackalloc ulong[pageCount];
        if (pageCount > MaxPageCount)
            masks.Clear();

        foreach (var value in span)
        {
            GetPageAndBit(value, out var pageIndex, out var mask);
            if (pageIndex >= pageCount) continue;
            masks[pageIndex] |= mask;
        }

        var pages = _pages;
        for (var i = 0; i < _pageCount; i++)
            pages[i] &= masks[i];

        Recount();
    }

    public void UnionWith(Span<int> span)
    {
        var maxPageIndex = _pageCount - 1;
        foreach (var value in span)
        {
            GetPageAndBit(value, out var pageIndex, out _);
            maxPageIndex = Math.Max(maxPageIndex, pageIndex);
        }

        if (maxPageIndex >= 0)
        {
            EnsurePage(maxPageIndex);
            _pageCount = Math.Max(_pageCount, maxPageIndex + 1);
        }
        var pageCount = _pageCount;

        var masks = pageCount > MaxPageCount ? new ulong[pageCount] : stackalloc ulong[pageCount];
        if (pageCount > MaxPageCount)
            masks.Clear();

        foreach (var value in span)
        {
            GetPageAndBit(value, out var pageIndex, out var mask);
            if (pageIndex >= pageCount) continue;
            masks[pageIndex] |= mask;
        }

        var pages = _pages;
        for (var i = 0; i < _pageCount; i++)
            pages[i] |= masks[i];

        Recount();
    }

    public void ExceptWith(Span<int> span)
    {
        foreach (var v in span)
            Remove(v);
    }

    public void SymmetricExceptWith(Span<int> span)
    {
        var pageCount = _pageCount;
        var masks = pageCount > MaxPageCount ? new ulong[pageCount] : stackalloc ulong[pageCount];
        if (pageCount > MaxPageCount)
            masks.Clear();

        foreach (var value in span)
        {
            GetPageAndBit(value, out var pageIndex, out var mask);
            if (pageIndex >= pageCount)
                Add(value);
            else
                masks[pageIndex] |= mask;
        }

        var pages = _pages;
        var minCount = Math.Min(_pageCount, masks.Length);
        for (var i = 0; i < minCount; i++)
            pages[i] ^= masks[i];

        Recount();
    }

    public void Clear()
    {
        for (var i = 0; i < _pageCount; i++)
            _pages[i] = 0ul;
        _pageCount = 0;
        _count = 0;
    }

    public void UnionWith(UIntSet other)
    {
        if (other._count == 0) return;
        if (_count == 0)
        {
            CopyOtherSet(other);
            return;
        }

        if (_pageCount < other._pageCount)
        {
            EnsurePage(other._pageCount);
            _pageCount = other._pageCount;
        }

        for (var i = 0; i < other._pageCount; i++)
            _pages[i] |= other._pages[i];
        Recount();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Recount()
    {
        var pages = _pages;
        var count = 0;
        for (int i = 0, end = _pageCount; i < end; i++)
            if (pages[i] != 0UL)
                count += PopCount(pages[i]);
        _count = count;
    }

    private void CopyOtherSet(UIntSet other)
    {
        _count = other._count;
        _pageCount = other._pageCount;
        if (other._pageCount > _pages.Length)
            Array.Resize(ref _pages, other._pageCount);
        Array.Copy(other._pages, _pages, other._pageCount);
    }

    public void IntersectWith(UIntSet other)
    {
        if (_count == 0 || other._count == 0)
        {
            Clear();
            return;
        }

        var minCount = Math.Min(_pageCount, other._pageCount);
        for (var i = 0; i < minCount; i++)
            _pages[i] &= other._pages[i];

        for (var i = minCount; i < _pageCount; i++)
            _pages[i] = 0ul;

        _pageCount = minCount;
        Recount();
    }

    public void ExceptWith(UIntSet other)
    {
        if (_count == 0 || other._count == 0) return;

        for (var i = 0; i < _pageCount && i < other._pageCount; i++)
            _pages[i] &= ~other._pages[i];

        Recount();
    }

    public void SymmetricExceptWith(UIntSet other)
    {
        if (other._count == 0) return;
        if (_count == 0)
        {
            UnionWith(other);
            return;
        }

        var maxPageCount = Math.Max(_pageCount, other._pageCount);
        EnsurePage(maxPageCount - 1);

        var minPageCount = Math.Min(_pageCount, other._pageCount);
        for (var i = 0; i < minPageCount; i++)
            _pages[i] ^= other._pages[i];

        for (var i = _pageCount; i < other._pageCount; i++)
            _pages[i] = other._pages[i];

        _pageCount = maxPageCount;
        Recount();
    }

    public struct PageEnumerator
    {
        private readonly ulong[] _pages;
        private readonly int _pageCount;
        private int _index;
        public BitEnumerator Current { get; private set; }

        internal PageEnumerator(ulong[] pages, int pageCount)
        {
            _pages = pages;
            _pageCount = pageCount;
            _index = -1;
            Current = default;
        }

        public PageEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            while (++_index < _pageCount)
            {
                var page = _pages[_index];
                if (page == 0UL) continue;
                Current = new BitEnumerator(page, _index << PageBits);
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _index = -1;
            Current = default;
        }

        public void Dispose() { }
    }

    public PageEnumerator Pages() => new PageEnumerator(_pages, _pageCount);

    public struct BitEnumerator : IEnumerator<int>
    {
        private ulong _currentPage;
        private int _currentPageBase;

        public BitEnumerator GetEnumerator() => this;

        public void Reset()
        {
            _currentPage = 0;
            _currentPageBase = 0;
            Current = 0;
        }

        object? IEnumerator.Current { get; }
        public int Current { get; private set; }

        internal BitEnumerator(ulong page, int pageBase)
        {
            _currentPage = page;
            _currentPageBase = pageBase;
            Current = 0;
        }

        public bool MoveNext()
        {
            if (_currentPage != 0)
            {
                var bitPosition = TrailingZeroCount(_currentPage);
                _currentPage &= _currentPage - 1;
                Current = _currentPageBase | bitPosition;
                return true;
            }
            return false;
        }

        public void Dispose() { }
    }

    public Enumerator GetEnumerator() => new Enumerator(_pages, _pageCount);
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<int>
    {
        private readonly ulong[] _pages;
        private readonly int _pageCount;
        private int _currentPageIndex;
        private ulong _currentPage;
        private int _currentPageBase;

        public Enumerator GetEnumerator() => this;

        public void Reset()
        {
            _currentPageIndex = -1;
            _currentPage = 0;
            _currentPageBase = 0;
            Current = 0;
        }

        object? IEnumerator.Current { get; }
        public int Current { get; private set; }

        internal Enumerator(ulong[] pages, int pageCount)
        {
            _pages = pages;
            _pageCount = pageCount;
            _currentPageIndex = -1;
            _currentPage = 0;
            _currentPageBase = 0;
            Current = 0;
        }

        public bool MoveNext()
        {
            if (_currentPage != 0)
            {
                var bitPosition = TrailingZeroCount(_currentPage);
                _currentPage &= _currentPage - 1;
                Current = _currentPageBase | bitPosition;
                return true;
            }

            var pageCount = _pageCount;
            while (++_currentPageIndex < pageCount)
            {
                _currentPage = _pages[_currentPageIndex];
                if (_currentPage == 0) continue;
                _currentPageBase = _currentPageIndex << PageBits;
                var bitPosition = TrailingZeroCount(_currentPage);
                _currentPage &= _currentPage - 1;
                Current = _currentPageBase | bitPosition;
                return true;
            }

            return false;
        }

        public void Dispose() { }
    }

    private const ulong DeBruijnSequence = 0x37E84A99DAE458F;

    private static readonly int[] MultiplyDeBruijnBitPosition =
    {
        0, 1, 17, 2, 18, 50, 3, 57, 47, 19, 22, 51, 29, 4, 33, 58, 15, 48, 20, 27, 25, 23, 52, 41, 54, 30, 38, 5,
        43, 34, 59, 8, 63, 16, 49, 56, 46, 21, 28, 32, 14, 26, 24, 40, 53, 37, 42, 7, 62, 55, 45, 31, 13, 39, 36,
        6, 61, 44, 12, 35, 60, 11, 10, 9,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int TrailingZeroCount(ulong b)
    {
        return MultiplyDeBruijnBitPosition[((ulong)((long)b & -(long)b) * DeBruijnSequence) >> 58];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PopCount(ulong x)
    {
        x = x - ((x >> 1) & 0x5555_5555_5555_5555UL);
        x = (x & 0x3333_3333_3333_3333UL) + ((x >> 2) & 0x3333_3333_3333_3333UL);
        x = (x + (x >> 4)) & 0x0F0F_0F0F_0F0F_0F0FUL;
        return (int)((x * 0x0101_0101_0101_0101UL) >> 56);
    }

    public int[] ToArray()
    {
        var array = new int[_count];
        var index = 0;
        foreach (var i in this)
            array[index++] = i;
        return array;
    }

    public List<int> ToList()
    {
        var array = new List<int>(_count);
        foreach (var i in this)
            array.Add(i);
        return array;
    }
}
