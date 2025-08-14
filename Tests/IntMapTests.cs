namespace IntSet.Tests;

[TestFixture]
public class IntMapTests
{
    private const int N = 10000;
    private int[] keys, values, lookup;
    [SetUp]
    public void Setup()
    {
        var rng = new Random(123);
        keys = new int[N];
        values = new int[N];
        lookup = new int[N];
        int RandomValue() => rng.Next(-N / 2, N / 2);
        for (var i = 0; i < N; i++)
        {
            keys[i] = RandomValue();
            values[i] = RandomValue();
            lookup[i] = RandomValue();
        }
    }
    
    
    [Test]
    public void TestIntMapAdd()
    {
        var map = new IntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        for (var i = 0; i < keys.Length; i++)
        {
            Assert.That(map[keys[i]], Is.EqualTo(dict[keys[i]]));
        }
    }
    
    [Test]
    public void TestIntMapContains()
    {
        var map = new IntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach(var i in lookup)
        {
            Assert.That(map.ContainsKey(i), Is.EqualTo(dict.ContainsKey(i)));
        }
    }
    
    [Test]
    public void TestIntMapTryGetValue()
    {
        var map = new IntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach(var i in lookup)
        {
            var mapValue = 0;
            var dictValue = 0;
            Assert.That(map.TryGetValue(i, out mapValue), Is.EqualTo(dict.TryGetValue(i,out dictValue)));
            Assert.That(mapValue, Is.EqualTo(dictValue));
        }
    }
    
    [Test]
    public void TestIntMapRemove()
    {
        var map = new IntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach (var i in lookup)
        {
            Assert.That(map.Remove(i), Is.EqualTo(dict.Remove(i)));
        }
    }
    
    [Test]
    public void TestIntMapGet()
    {
        var map = new IntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach (var i in lookup)
        {
            if (dict.ContainsKey(i))
            {
                Assert.DoesNotThrow(() => { var _ = map[i]; });
            }
            else
            {
                Assert.Throws<KeyNotFoundException>(() => { var _ = map[i]; });
            }
        }
    }

    [Test]
    public void TestIteration()
    {
        var map = new IntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }

        foreach (var i in map.Keys)
        {
            Assert.That(dict.ContainsKey(i));
        }
    }
    
    [Test]
    public void TestIntMapTryGetValuePresentAndAbsent()
    {
        var map = new IntMap<int>();
        map[1] = 42;
        map[2] = 99;
        int value;
        Assert.IsTrue(map.TryGetValue(1, out value));
        Assert.AreEqual(42, value);
        Assert.IsTrue(map.TryGetValue(2, out value));
        Assert.AreEqual(99, value);
        Assert.IsFalse(map.TryGetValue(3, out value));
    }

    [Test]
    public void TestIntMapValuesIterator()
    {
        var map = new IntMap<int>();
        var expected = new HashSet<int>();
        for (int i = 0; i < 10; i++)
        {
            map[i] = i * 10;
            expected.Add(i * 10);
        }
        var actual = new HashSet<int>();
        foreach (var v in map.Values)
            actual.Add(v);
        CollectionAssert.AreEquivalent(expected, actual);
    }

    [Test]
    public void TestIntMapTryAdd()
    {
        var map = new IntMap<int>();
        map.TryAdd(5, 100);
        Assert.AreEqual(100, map[5]);
        map.TryAdd(5, 200); // Should not overwrite
        Assert.AreEqual(100, map[5]);
    }

    [Test]
    public void TestIntMapAddThrowsOnDuplicate()
    {
        var map = new IntMap<int>();
        map.Add(7, 77);
        Assert.AreEqual(77, map[7]);
        Assert.Throws<InvalidOperationException>(() => map.Add(7, 88));
    }

    [Test]
    public void TestIntMapLargePositiveKeys()
    {
        var map = new IntMap<string>();
        var largeKeys = new[] { int.MaxValue, int.MaxValue - 1, 1000000000, 2000000000 };
        var values = new[] { "max", "max-1", "billion", "2billion" };

        for (int i = 0; i < largeKeys.Length; i++)
        {
            map[largeKeys[i]] = values[i];
        }

        for (int i = 0; i < largeKeys.Length; i++)
        {
            Assert.AreEqual(values[i], map[largeKeys[i]]);
            Assert.IsTrue(map.ContainsKey(largeKeys[i]));
        }
    }

    [Test]
    public void TestIntMapLargeNegativeKeys()
    {
        var map = new IntMap<string>();
        var largeKeys = new[] { int.MinValue, int.MinValue + 1, -1000000000, -2000000000 };
        var values = new[] { "min", "min+1", "-billion", "-2billion" };

        for (int i = 0; i < largeKeys.Length; i++)
        {
            map[largeKeys[i]] = values[i];
        }

        for (int i = 0; i < largeKeys.Length; i++)
        {
            Assert.AreEqual(values[i], map[largeKeys[i]]);
            Assert.IsTrue(map.ContainsKey(largeKeys[i]));
        }
    }

    [Test]
    public void TestIntMapExtremeValueGaps()
    {
        var map = new IntMap<int>();
        
        // Test the pathological case with extreme gaps
        map[int.MinValue] = 1;
        map[int.MaxValue] = 2;
        map[0] = 3;

        Assert.AreEqual(1, map[int.MinValue]);
        Assert.AreEqual(2, map[int.MaxValue]);
        Assert.AreEqual(3, map[0]);
        Assert.AreEqual(3, map.Count);

        // Test removal
        Assert.IsTrue(map.Remove(int.MinValue));
        Assert.IsFalse(map.ContainsKey(int.MinValue));
        Assert.AreEqual(2, map.Count);
    }

    [Test]
    public void TestIntMapTryGetValueWithLargeKeys()
    {
        var map = new IntMap<long>();
        map[-88812381] = 12345L; // This was the problematic value mentioned
        map[88812381] = 54321L;

        long value;
        Assert.IsTrue(map.TryGetValue(-88812381, out value));
        Assert.AreEqual(12345L, value);

        Assert.IsTrue(map.TryGetValue(88812381, out value));
        Assert.AreEqual(54321L, value);

        Assert.IsFalse(map.TryGetValue(99999999, out value));
        Assert.AreEqual(0L, value);
    }

    [Test]
    public void TestIntMapValuesIteratorWithLargeKeys()
    {
        var map = new IntMap<int>();
        var keys = new[] { int.MinValue, -1000000, 0, 1000000, int.MaxValue };
        var values = new[] { 1, 2, 3, 4, 5 };

        for (int i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
        }

        var actualValues = new List<int>();
        foreach (var value in map.Values)
        {
            actualValues.Add(value);
        }

        Assert.AreEqual(values.Length, actualValues.Count);
        CollectionAssert.AreEquivalent(values, actualValues);
    }

    [Test]
    public void TestIntMapClearWithLargeKeys()
    {
        var map = new IntMap<string>();
        map[int.MinValue] = "min";
        map[int.MaxValue] = "max";
        map[0] = "zero";

        Assert.AreEqual(3, map.Count);
        
        map.Clear();
        
        Assert.AreEqual(0, map.Count);
        Assert.IsFalse(map.ContainsKey(int.MinValue));
        Assert.IsFalse(map.ContainsKey(int.MaxValue));
        Assert.IsFalse(map.ContainsKey(0));
    }
}