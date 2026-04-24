namespace IntSet.Tests;

[TestFixture]
public class NativeUIntMapTests
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
        for (var i = 0; i < N; i++)
        {
            keys[i] = rng.Next(0, N);
            values[i] = rng.Next(0, N);
            lookup[i] = rng.Next(0, N);
        }
    }

    [Test]
    public void TestNativeUIntMapAdd()
    {
        using var map = new NativeUIntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        for (var i = 0; i < keys.Length; i++)
            Assert.That(map[keys[i]], Is.EqualTo(dict[keys[i]]));
    }

    [Test]
    public void TestNativeUIntMapContains()
    {
        using var map = new NativeUIntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach (var i in lookup)
            Assert.That(map.ContainsKey(i), Is.EqualTo(dict.ContainsKey(i)));
    }

    [Test]
    public void TestNativeUIntMapTryGetValue()
    {
        using var map = new NativeUIntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach (var i in lookup)
        {
            int mapValue = 0, dictValue = 0;
            Assert.Multiple(() =>
            {
                Assert.That(map.TryGetValue(i, out mapValue), Is.EqualTo(dict.TryGetValue(i, out dictValue)));
                Assert.That(mapValue, Is.EqualTo(dictValue));
            });
        }
    }

    [Test]
    public void TestNativeUIntMapRemove()
    {
        using var map = new NativeUIntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach (var i in lookup)
            Assert.That(map.Remove(i), Is.EqualTo(dict.Remove(i)));
    }

    [Test]
    public void TestNativeUIntMapGet()
    {
        using var map = new NativeUIntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach (var i in lookup)
        {
            if (dict.ContainsKey(i))
                Assert.That(() => { var _ = map[i]; }, Throws.Nothing);
            else
                Assert.That(() => { var _ = map[i]; }, Throws.InstanceOf<KeyNotFoundException>());
        }
    }

    [Test]
    public void TestIteration()
    {
        using var map = new NativeUIntMap<int>();
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < keys.Length; i++)
        {
            map[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
        foreach (var i in map.Keys)
            Assert.That(dict.ContainsKey(i), Is.True);
    }

    [Test]
    public void TestNativeUIntMapTryGetValuePresentAndAbsent()
    {
        using var map = new NativeUIntMap<int>();
        map[1] = 42;
        map[2] = 99;
        int v1 = 0, v2 = 0;
        Assert.Multiple(() =>
        {
            Assert.That(map.TryGetValue(1, out v1), Is.True);
            Assert.That(v1, Is.EqualTo(42));
            Assert.That(map.TryGetValue(2, out v2), Is.True);
            Assert.That(v2, Is.EqualTo(99));
            Assert.That(map.TryGetValue(3, out _), Is.False);
        });
    }

    [Test]
    public void TestNativeUIntMapValuesIterator()
    {
        using var map = new NativeUIntMap<int>();
        var expected = new HashSet<int>();
        for (int i = 0; i < 10; i++)
        {
            map[i] = i * 10;
            expected.Add(i * 10);
        }
        var actual = new HashSet<int>();
        foreach (var v in map.Values)
            actual.Add(v);
        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void TestNativeUIntMapTryAdd()
    {
        using var map = new NativeUIntMap<int>();
        map.TryAdd(5, 100);
        Assert.That(map[5], Is.EqualTo(100));
        map.TryAdd(5, 200);
        Assert.That(map[5], Is.EqualTo(100));
    }

    [Test]
    public void TestNativeUIntMapAddThrowsOnDuplicate()
    {
        using var map = new NativeUIntMap<int>();
        map.Add(7, 77);
        Assert.That(map[7], Is.EqualTo(77));
        Assert.That(() => map.Add(7, 88), Throws.InstanceOf<InvalidOperationException>());
    }

    [Test]
    public void TestNativeUIntMapLargeKeys()
    {
        using var map = new NativeUIntMap<int>();
        var largeKeys = new[] { int.MaxValue, int.MaxValue - 1, 1000000000, 2000000000 };
        var vals = new[] { 1, 2, 3, 4 };
        for (int i = 0; i < largeKeys.Length; i++)
            map[largeKeys[i]] = vals[i];
        for (int i = 0; i < largeKeys.Length; i++)
            Assert.Multiple(() =>
            {
                Assert.That(map[largeKeys[i]], Is.EqualTo(vals[i]));
                Assert.That(map.ContainsKey(largeKeys[i]), Is.True);
            });
    }

    [Test]
    public void TestNativeUIntMapTryGetValueWithLargeKeys()
    {
        using var map = new NativeUIntMap<long>();
        map[88812381] = 12345L;
        map[999999999] = 54321L;
        long v1 = 0, v2 = 0;
        Assert.Multiple(() =>
        {
            Assert.That(map.TryGetValue(88812381, out v1), Is.True);
            Assert.That(v1, Is.EqualTo(12345L));
            Assert.That(map.TryGetValue(999999999, out v2), Is.True);
            Assert.That(v2, Is.EqualTo(54321L));
            Assert.That(map.TryGetValue(11111111, out _), Is.False);
        });
    }

    [Test]
    public void TestNativeUIntMapValuesIteratorWithLargeKeys()
    {
        using var map = new NativeUIntMap<int>();
        var ks = new[] { 0, 1000000, 500000, 2000000, int.MaxValue };
        var vs = new[] { 1, 2, 3, 4, 5 };
        for (int i = 0; i < ks.Length; i++)
            map[ks[i]] = vs[i];
        var actualValues = new List<int>();
        foreach (var value in map.Values)
            actualValues.Add(value);
        Assert.Multiple(() =>
        {
            Assert.That(actualValues, Has.Count.EqualTo(vs.Length));
            Assert.That(actualValues, Is.EquivalentTo(vs));
        });
    }

    [Test]
    public void TestNativeUIntMapClear()
    {
        using var map = new NativeUIntMap<int>();
        map[0] = 1;
        map[1000000] = 2;
        map[int.MaxValue] = 3;
        Assert.That(map.Count, Is.EqualTo(3));
        map.Clear();
        Assert.Multiple(() =>
        {
            Assert.That(map.Count, Is.EqualTo(0));
            Assert.That(map.ContainsKey(0), Is.False);
            Assert.That(map.ContainsKey(1000000), Is.False);
            Assert.That(map.ContainsKey(int.MaxValue), Is.False);
        });
    }
}
