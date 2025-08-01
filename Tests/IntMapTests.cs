
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

        foreach (var i in map)
        {
            Assert.That(dict.ContainsKey(i));
        }
    }
}