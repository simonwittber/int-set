using System;

namespace IntSet;

public class CenteredArray<T> {
    T[] _array;
    private int _loIndex, _hiIndex;
    private int _centerIndex;
    public T[] Array => _array;
    
    public CenteredArray(int capacity = 16)
    {
        _array = new T[capacity*2];
        _loIndex = 0;
        _hiIndex = 0;
        _centerIndex = capacity;
    }

    public T GetRawIndex(int index)
    {
        return _array[index];
    }
    
    public T this[int index]
    {
        get
        {
            var actualIndex = index + _centerIndex;
            if (actualIndex < 0 || actualIndex >= _array.Length)
                throw new IndexOutOfRangeException("Index out of range for CenteredArray.");
            return _array[actualIndex];
        }
        set
        {
            var actualIndex = index + _centerIndex;
            if (actualIndex < 0 || actualIndex >= _array.Length)
            {
                Resize(actualIndex);
                actualIndex = index + _centerIndex;
            }
            _array[actualIndex] = value;
            if (actualIndex < _loIndex) _loIndex = actualIndex;
            if (actualIndex > _hiIndex) _hiIndex = actualIndex;
        }
    }

    private void Resize(int index)
    {
        var neededSlots = 0;
        if (index < 0) neededSlots = -index;
        if( index >= _array.Length) neededSlots = index - _array.Length + 1;
        if (neededSlots > 0)
        {
            var newSize = Math.Max(_array.Length * 2, _array.Length + neededSlots);
            var newArray = new T[newSize];
            // data current exists from _loIndex to _hiIndex
            var currentWidth = _hiIndex - _loIndex + 1;
            var newCenterIndex = newSize - currentWidth;
            System.Array.Copy(_array, 0, newArray, newCenterIndex - _centerIndex, _array.Length);
            _array = newArray;
            _loIndex = newCenterIndex - (_centerIndex - _loIndex);
            _hiIndex = newCenterIndex + (_hiIndex - _centerIndex);
            _centerIndex = newCenterIndex;
        }
    }
}