﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IntSet;

/// <summary>
/// NativeClusteredBitmap is the same as ClusteredBitmap, except it uses unmanaged memory.
/// </summary>
public unsafe class NativeClusteredBitmap : IDisposable
{
    private const int PageBits = 6;
    private const int PageSize = 1 << PageBits;
    private const int PageMask = PageSize - 1;
    private const int MaxStackBytes = 128 * 1024; // How much stack space we can use for masks.
    private const int BytesPerPage = sizeof(ulong);
    private const int MaxPageCount = MaxStackBytes / BytesPerPage;

    private IntPtr _pagesPtr; // unmanaged memory block containing pages (each page is one ulong of 64 bits)
    private int _capacity;    // number of allocated ulong slots (>= _pageCount)
    private int _pageCount;   // highest page index in use + 1
    private int _count;       // number of set bits total

    private int _centerValue;
    private bool _disposed;

    public int Count => _count;
    public int PageCount => _pageCount;

    public NativeClusteredBitmap()
    {
        _capacity = 16;
        _pagesPtr = Marshal.AllocHGlobal(_capacity * BytesPerPage);
        new Span<byte>((void*)_pagesPtr, _capacity * BytesPerPage).Clear();
        _pageCount = 0;
    }

    public NativeClusteredBitmap(Span<int> values) : this()
    {
        Or(values);
    }

    ~NativeClusteredBitmap()
    {
        Dispose(false);
    }

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
        // zero new block then copy existing
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
    public bool Set(int value)
    {
        if (_count == 0)
            _centerValue = value;
        GetPageAndBit(value, out var pageIndex, out var mask);
        EnsurePage(pageIndex);
        var pages = (ulong*)_pagesPtr;
        if ((pages[pageIndex] & mask) != 0UL) return false;
        pages[pageIndex] |= mask;
        _pageCount = pageIndex >= _pageCount ? pageIndex + 1 : _pageCount;
        _count++;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UnSetBit(int p, ulong mask)
    {
        var pages = (ulong*)_pagesPtr;
        pages[p] &= ~mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSet(int p, ulong mask)
    {
        var pages = (ulong*)_pagesPtr;
        return (pages[p] & mask) != 0UL;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GetPageAndBit(int value, out int pageIndex, out ulong mask)
    {
        var key = ZigZagEncode(value - _centerValue);
        pageIndex = (int)(key >> PageBits);
        var bitIndex = (int)(key & PageMask);
        mask = 1ul << bitIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSet(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var mask);
        return pageIndex < _pageCount && IsSet(pageIndex, mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool UnSet(int value)
    {
        GetPageAndBit(value, out var pageIndex, out var mask);
        if (pageIndex >= _pageCount) return false;
        if (IsSet(pageIndex, mask))
        {
            UnSetBit(pageIndex, mask);
            _count--;
            return true;
        }
        return false;
    }

    public void And(Span<int> span)
    {
        var pageCount = _pageCount;
        if (pageCount == 0) return;
        var masks = pageCount > MaxPageCount ? new ulong[pageCount] : stackalloc ulong[pageCount];
        if (pageCount > MaxPageCount)
            masks.Clear();
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

    public void Or(Span<int> span)
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

        // Collapse duplicate values in the span into masks for each page.
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

    public void Not(Span<int> span)
    {
        foreach (var v in span)
            UnSet(v);
    }

    public void Xor(Span<int> span)
    {
        var pageCount = _pageCount;
        var masks = pageCount > MaxPageCount ? new ulong[pageCount] : stackalloc ulong[pageCount];
        if (pageCount > MaxPageCount)
            masks.Clear();
        foreach (var value in span)
        {
            GetPageAndBit(value, out var pageIndex, out var mask);
            if (pageIndex >= pageCount)
                Set(value);
            else
                masks[pageIndex] |= mask;
        }
        var pages = (ulong*)_pagesPtr;
        var minCount = Math.Min(_pageCount, masks.Length);
        for (var i = 0; i < minCount; i++)
            pages[i] ^= masks[i];
        Recount();
    }

    public void Clear()
    {
        if (_pageCount > 0 && _pagesPtr != IntPtr.Zero)
            new Span<byte>((void*)_pagesPtr, _pageCount * BytesPerPage).Clear();
        _pageCount = 0;
        _count = 0;
        _centerValue = 0;
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

    public Enumerator GetEnumerator() => new Enumerator((ulong*)_pagesPtr, _pageCount, _centerValue);

    public struct Enumerator
    {
        private readonly ulong* _pages;
        private readonly int _pageCount;
        private int _currentPageIndex;
        private ulong _currentPage;
        private int _currentPageBase;
        private readonly int _centerValue;
        public int Current { get; private set; }

        internal Enumerator(ulong* pages, int pageCount, int centerValue)
        {
            _pages = pages;
            _pageCount = pageCount;
            _currentPageIndex = -1;
            _currentPage = 0;
            _currentPageBase = 0;
            _centerValue = centerValue;
            Current = 0;
        }

        public Enumerator GetEnumerator() => this;

        public void Reset()
        {
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
                Current = ZigZagDecode((uint)(_currentPageBase | bitPosition)) + _centerValue;
                return true;
            }
            while (++_currentPageIndex < _pageCount)
            {
                _currentPage = _pages[_currentPageIndex];
                if (_currentPage == 0) continue;
                _currentPageBase = _currentPageIndex << PageBits;
                var bitPosition = TrailingZeroCount(_currentPage);
                _currentPage &= _currentPage - 1;
                Current = ZigZagDecode((uint)(_currentPageBase | bitPosition)) + _centerValue;
                return true;
            }
            return false;
        }

        public void Dispose() { }
    }

    private const ulong DeBruijnSequence = 0x37E84A99DAE458F;
    private static readonly int[] MultiplyDeBruijnBitPosition = {0,1,17,2,18,50,3,57,47,19,22,51,29,4,33,58,15,48,20,27,25,23,52,41,54,30,38,5,43,34,59,8,63,16,49,56,46,21,28,32,14,26,24,40,53,37,42,7,62,55,45,31,13,39,36,6,61,44,12,35,60,11,10,9};

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int TrailingZeroCount(ulong b)
    {
        return MultiplyDeBruijnBitPosition[((ulong)((long)b & -(long)b) * DeBruijnSequence) >> 58];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ZigZagEncode(int v) => ((uint)(v << 1)) ^ (uint)(v >> 31);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ZigZagDecode(uint u) => (int)((u >> 1) ^ -(u & 1));

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