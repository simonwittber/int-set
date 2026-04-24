using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IntSet;

public unsafe class NativeIntMap<T> : IDisposable where T : unmanaged
{
    private const int PageBits = 10;
    private const int PageSize = 1 << PageBits;
    private const int PageMask = PageSize - 1;

    private IntPtr[] _valuePages;
    private readonly NativeClusteredBitmap _keys;
    private bool _disposed;

    public int Count { get; private set; }

    public NativeIntMap()
    {
        _valuePages = new IntPtr[16];
        _keys = new NativeClusteredBitmap();
    }

    ~NativeIntMap() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        foreach (var p in _valuePages)
            if (p != IntPtr.Zero) Marshal.FreeHGlobal(p);
        if (disposing)
            _keys.Dispose();
        _disposed = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GetIndexAndSlot(int value, out int pageIndex, out int slot)
    {
        var key = ZigZagEncode(value);
        pageIndex = (int)(key >> PageBits);
        slot = (int)(key & PageMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ZigZagEncode(int v) => ((uint)(v << 1)) ^ (uint)(v >> 31);

    private void EnsurePage(int pageIndex)
    {
        if (_valuePages.Length <= pageIndex)
            Array.Resize(ref _valuePages, Math.Max(_valuePages.Length * 2, pageIndex + 1));
        if (_valuePages[pageIndex] == IntPtr.Zero)
        {
            var ptr = Marshal.AllocHGlobal(sizeof(T) * PageSize);
            new Span<byte>((void*)ptr, sizeof(T) * PageSize).Clear();
            _valuePages[pageIndex] = ptr;
        }
    }

    public void EnsureCapacity(int items) { }

    public bool Remove(int v)
    {
        if (!_keys.UnSet(v)) return false;
        GetIndexAndSlot(v, out var pageIndex, out var slot);
        ((T*)_valuePages[pageIndex])[slot] = default;
        Count--;
        return true;
    }

    public bool ContainsKey(int id) => _keys.IsSet(id);

    public bool TryGetValue(int v, out T value)
    {
        if (_keys.IsSet(v))
        {
            GetIndexAndSlot(v, out var pageIndex, out var slot);
            value = ((T*)_valuePages[pageIndex])[slot];
            return true;
        }
        value = default;
        return false;
    }

    public void Add(int key, T item)
    {
        if (_keys.IsSet(key))
            throw new InvalidOperationException("Key already exists");
        this[key] = item;
    }

    public void TryAdd(int key, T item)
    {
        if (!ContainsKey(key))
            this[key] = item;
    }

    public T this[int v]
    {
        get
        {
            if (!_keys.IsSet(v))
                throw new KeyNotFoundException();
            GetIndexAndSlot(v, out var pageIndex, out var slot);
            return ((T*)_valuePages[pageIndex])[slot];
        }
        set
        {
            GetIndexAndSlot(v, out var pageIndex, out var slot);
            EnsurePage(pageIndex);
            ((T*)_valuePages[pageIndex])[slot] = value;
            if (_keys.Set(v))
                Count++;
        }
    }

    public void Clear()
    {
        foreach (var p in _valuePages)
            if (p != IntPtr.Zero)
                new Span<byte>((void*)p, sizeof(T) * PageSize).Clear();
        Count = 0;
        _keys.Clear();
    }

    public ValueEnumerator Values => new ValueEnumerator(this);
    public NativeClusteredBitmap.Enumerator Keys => _keys.GetEnumerator();

    public struct ValueEnumerator
    {
        private readonly NativeIntMap<T> _map;
        private NativeClusteredBitmap.Enumerator _keyEnumerator;

        public ValueEnumerator GetEnumerator() => this;
        public T Current { get; private set; }

        public ValueEnumerator(NativeIntMap<T> map)
        {
            _map = map;
            _keyEnumerator = map._keys.GetEnumerator();
            Current = default;
        }

        public bool MoveNext()
        {
            if (_keyEnumerator.MoveNext())
            {
                Current = _map[_keyEnumerator.Current];
                return true;
            }
            return false;
        }
    }
}
