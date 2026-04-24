using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IntSet;

public unsafe class NativeUIntSet : IEnumerable<int>, IDisposable
{
    private const int PageBits = 6;
    private const int PageSize = 1 << PageBits;
    private const int PageMask = PageSize - 1;
    private const int MaxStackBytes = 128 * 1024;
    private const int BytesPerPage = sizeof(ulong);
    private const int MaxPageCount = MaxStackBytes / BytesPerPage;

    private IntPtr _pagesPtr;
    private int _capacity;
    private int _pageCount;
    private int _count;
    private bool _disposed;

    public int Count => _count;
    public int PageCount => _pageCount;

    public NativeUIntSet()
    {
        _capacity = 16;
        _pagesPtr = Marshal.AllocHGlobal(_capacity * BytesPerPage);
        new Span<byte>((void*)_pagesPtr, _capacity * BytesPerPage).Clear();
        _pageCount = 0;
    }

    public NativeUIntSet(Span<int> values) : this()
    {
        UnionWith(values);
    }

    ~NativeUIntSet() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (_pagesPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_pagesPtr);
            _pagesPtr = IntPtr.Zero;
        }
        _disposed = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsurePage(int pageIndex)
    {
        if (pageIndex < _capacity) return;
        var newCapacity = Math.Max(_capacity * 2, pageIndex + 1);
        var newPtr = Marshal.AllocHGlobal(newCapacity * BytesPerPage);
        new Span<byte>((void*)newPtr, newCapacity * BytesPerPage).Clear();
        if (_capacity > 0)
        {
            Buffer.MemoryCopy((void*)_pagesPtr, (void*)newPtr, newCapacity * BytesPerPage, _capacity * BytesPerPage);
            Marshal.FreeHGlobal(_pagesPtr);
        }
        _pagesPtr = newPtr;
        _capacity = newCapacity;
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
        var pages = (ulong*)_pagesPtr;
        if ((pages[pageIndex] & mask) != 0) return false;
        pages[pageIndex] |= mask;
        _pageCount = pageIndex >= _pageCount ? pageIndex + 1 : _pageCount;
        _count++;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var mask);
        if (pageIndex >= _pageCount) return false;
        return (((ulong*)_pagesPtr)[pageIndex] & mask) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var mask);
        if (pageIndex >= _pageCount) return false;
        var pages = (ulong*)_pagesPtr;
        if ((pages[pageIndex] & mask) == 0) return false;
        pages[pageIndex] &= ~mask;
        _count--;
        return true;
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
        if (pageCount > MaxPageCount) masks.Clear();

        foreach (var value in span)
        {
            GetPageAndBit(value, out var pageIndex, out var mask);
            if (pageIndex >= pageCount) continue;
            masks[pageIndex] |= mask;
        }

        var pages = (ulong*)_pagesPtr;
        for (var i = 0; i < _pageCount; i++)
            pages[i] |= masks[i];

        Recount();
    }

    public void IntersectWith(Span<int> span)
    {
        var pageCount = _pageCount;
        var masks = pageCount > MaxPageCount ? new ulong[pageCount] : stackalloc ulong[pageCount];
        if (pageCount > MaxPageCount) masks.Clear();

        foreach (var value in span)
        {
            GetPageAndBit(value, out var pageIndex, out var mask);
            if (pageIndex >= pageCount) continue;
            masks[pageIndex] |= mask;
        }

        var pages = (ulong*)_pagesPtr;
        for (var i = 0; i < _pageCount; i++)
            pages[i] &= masks[i];

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
        if (pageCount > MaxPageCount) masks.Clear();

        foreach (var value in span)
        {
            GetPageAndBit(value, out var pageIndex, out var mask);
            if (pageIndex >= pageCount)
                Add(value);
            else
                masks[pageIndex] |= mask;
        }

        var pages = (ulong*)_pagesPtr;
        var minCount = Math.Min(_pageCount, masks.Length);
        for (var i = 0; i < minCount; i++)
            pages[i] ^= masks[i];

        Recount();
    }

    public void UnionWith(NativeUIntSet other)
    {
        if (other._count == 0) return;
        if (_count == 0)
        {
            CopyFrom(other);
            return;
        }

        if (_pageCount < other._pageCount)
        {
            EnsurePage(other._pageCount - 1);
            _pageCount = other._pageCount;
        }

        var pages = (ulong*)_pagesPtr;
        var otherPages = (ulong*)other._pagesPtr;
        for (var i = 0; i < other._pageCount; i++)
            pages[i] |= otherPages[i];
        Recount();
    }

    public void IntersectWith(NativeUIntSet other)
    {
        if (_count == 0 || other._count == 0) { Clear(); return; }

        var pages = (ulong*)_pagesPtr;
        var otherPages = (ulong*)other._pagesPtr;
        var minCount = Math.Min(_pageCount, other._pageCount);
        for (var i = 0; i < minCount; i++)
            pages[i] &= otherPages[i];
        for (var i = minCount; i < _pageCount; i++)
            pages[i] = 0ul;

        _pageCount = minCount;
        Recount();
    }

    public void ExceptWith(NativeUIntSet other)
    {
        if (_count == 0 || other._count == 0) return;

        var pages = (ulong*)_pagesPtr;
        var otherPages = (ulong*)other._pagesPtr;
        for (var i = 0; i < _pageCount && i < other._pageCount; i++)
            pages[i] &= ~otherPages[i];

        Recount();
    }

    public void SymmetricExceptWith(NativeUIntSet other)
    {
        if (other._count == 0) return;
        if (_count == 0) { UnionWith(other); return; }

        var maxPageCount = Math.Max(_pageCount, other._pageCount);
        EnsurePage(maxPageCount - 1);

        var pages = (ulong*)_pagesPtr;
        var otherPages = (ulong*)other._pagesPtr;
        var minPageCount = Math.Min(_pageCount, other._pageCount);
        for (var i = 0; i < minPageCount; i++)
            pages[i] ^= otherPages[i];
        for (var i = _pageCount; i < other._pageCount; i++)
            pages[i] = otherPages[i];

        _pageCount = maxPageCount;
        Recount();
    }

    public void Clear()
    {
        if (_pageCount > 0 && _pagesPtr != IntPtr.Zero)
            new Span<byte>((void*)_pagesPtr, _pageCount * BytesPerPage).Clear();
        _pageCount = 0;
        _count = 0;
    }

    private void CopyFrom(NativeUIntSet other)
    {
        _count = other._count;
        _pageCount = other._pageCount;
        EnsurePage(other._pageCount - 1);
        Buffer.MemoryCopy((void*)other._pagesPtr, (void*)_pagesPtr, _capacity * BytesPerPage, other._pageCount * BytesPerPage);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Recount()
    {
        var pages = (ulong*)_pagesPtr;
        var count = 0;
        for (int i = 0, end = _pageCount; i < end; i++)
        {
            var v = pages[i];
            if (v != 0UL) count += PopCount(v);
        }
        _count = count;
    }

    public Enumerator GetEnumerator() => new Enumerator((ulong*)_pagesPtr, _pageCount);
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<int>
    {
        private readonly ulong* _pages;
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

        object? IEnumerator.Current => Current;
        public int Current { get; private set; }

        internal Enumerator(ulong* pages, int pageCount)
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

            while (++_currentPageIndex < _pageCount)
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
        => MultiplyDeBruijnBitPosition[((ulong)((long)b & -(long)b) * DeBruijnSequence) >> 58];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PopCount(ulong x)
    {
        x -= (x >> 1) & 0x5555_5555_5555_5555UL;
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
        var list = new List<int>(_count);
        foreach (var i in this)
            list.Add(i);
        return list;
    }
}
