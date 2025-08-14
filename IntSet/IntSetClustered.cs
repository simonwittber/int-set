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
public class IntSetClustered
{
    private const int PageBits = 6;
    private const int PageSize = 1 << PageBits;
    private const int PageMask = PageSize - 1;
    private const int MaxStackBytes = 128 * 1024; // How much stack space we can use for masks.
    private const int BytesPerPage = sizeof(ulong);
    private const int MaxPageCount = MaxStackBytes / BytesPerPage;

    private Bitmap _loBitmap = new Bitmap();
    private Bitmap _hiBitmap = new Bitmap();

    private int _centerValue;

    private bool _isInitialized;

    public int Count => _loBitmap.Count + _hiBitmap.Count;

    public IntSetClustered()
    {
        _centerValue = 0;
        _isInitialized = false;
    }

    public IntSetClustered(Span<int> values)
    {
        _centerValue = 0;
        _isInitialized = false;
        UnionWith(values);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(int value)
    {
        // is this the first value being added?
        if (!_isInitialized)
        {
            // The goal here is to center the key space around the smallest multiple of 64,
            // to reduce number pages needed.
            var smallestMultipleOf64 = value & ~PageMask;
            _centerValue = smallestMultipleOf64;
            _isInitialized = true;
        }
        var index = value - _centerValue;
        return index < 0 ? _loBitmap.Set(-index) : _hiBitmap.Set(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int value)
    {
        var index = value - _centerValue;
        return index < 0 ? _loBitmap.IsSet(-index) : _hiBitmap.IsSet(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(int value)
    {
        var index = value - _centerValue;
        return index < 0 ? _loBitmap.UnSet(-index) : _hiBitmap.UnSet(index);
    }

    public void IntersectWith(Span<int> span)
    {
        var count = span.Length;
        var loSpan = count > MaxPageCount ? new int[count] : stackalloc int[count];
        var hiSpan = count > MaxPageCount ? new int[count] : stackalloc int[count];
        var hiCount = 0;
        var loCount = 0;
        if (count > MaxPageCount)
        {
            loSpan.Clear();
            hiSpan.Clear();
        }

        foreach (var i in span)
        {
            var index = i - _centerValue;
            if (index < 0)
                loSpan[loCount++] = -index;
            else
                hiSpan[hiCount++] = index;
        }
        _loBitmap.And(loSpan.Slice(0, loCount));
        _hiBitmap.And(hiSpan.Slice(0, hiCount));
    }

    public void UnionWith(Span<int> span)
    {
        foreach (var value in span)
        {
            var index = value - _centerValue;
            if (index < 0)
                _loBitmap.Set(-index);
            else
                _hiBitmap.Set(index);
        }
    }

    public void ExceptWith(Span<int> span)
    {
        foreach (var value in span)
        {
            var index = value - _centerValue;
            if (index < 0)
                _loBitmap.UnSet(-index);
            else
                _hiBitmap.UnSet(index);
        }
    }

    public void SymmetricExceptWith(Span<int> span)
    {
        foreach (var v in span)
        {

        }
    }

    public void Clear()
    {
        _loBitmap.Clear();
        _hiBitmap.Clear();
        _centerValue = 0;
        _isInitialized = false;
    }

    public void UnionWith(IntSetClustered other)
    {
        
    }

    public void IntersectWith(IntSetClustered other)
    {
        // we need to & the lo and hi bitmaps with the other set's bitmaps
        // the center value might be different, so we need to adjust the indices accordingly
        if (_centerValue == other._centerValue)
        {
            _loBitmap.And(other._loBitmap);
            _hiBitmap.And(other._hiBitmap);
            return;
        }
        // our loBitmap is for values < _centerValue
        // other.loBitmap is for values < other._centerValue
        // we need to to line up the bitmaps so that we can & them together
        var offset = other._centerValue - _centerValue;

        


    }

    public void ExceptWith(IntSetClustered other)
    {
    }

    public void SymmetricExceptWith(IntSetClustered other)
    {
    }

    public Enumerator GetEnumerator() => new Enumerator(_loBitmap, _hiBitmap, _centerValue);

    public struct Enumerator
    {
        private Bitmap.Enumerator _loEnumerator;
        private Bitmap.Enumerator _hiEnumerator;
        private bool _isLoSet;
        int _centerValue;

        public Enumerator GetEnumerator() => this;

        public void Reset()
        {
        }

        public int Current { get; private set; }

        public Enumerator(Bitmap loSet, Bitmap hiSet, int centerValue)
        {
            _loEnumerator = loSet.GetEnumerator();
            _hiEnumerator = hiSet.GetEnumerator();
            _centerValue = centerValue;
            _isLoSet = true;
        }

        public bool MoveNext()
        {
            if (_isLoSet)
            {
                if (_loEnumerator.MoveNext())
                {
                    Current = -(_loEnumerator.Current - _centerValue);
                    return true;
                }
                _isLoSet = false; // Switch to positive set
            }

            if (_hiEnumerator.MoveNext())
            {
                Current = _hiEnumerator.Current + _centerValue;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
        }
    }

    public int[] ToArray()
    {
        var array = new int[Count];
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