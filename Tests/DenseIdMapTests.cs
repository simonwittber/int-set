namespace IntSet.Tests;

[TestFixture]
public class DenseIdMapTests
{
    [Test]
    public void GetOrAdd_ReturnsSequentialIds()
    {
        var vm = new DenseIdMap();
        Assert.Multiple(() =>
        {
            Assert.That(vm.GetOrAdd(100), Is.EqualTo(0));
            Assert.That(vm.GetOrAdd(200), Is.EqualTo(1));
            Assert.That(vm.GetOrAdd(300), Is.EqualTo(2));
            Assert.That(vm.GetOrAdd(100), Is.EqualTo(0));
            Assert.That(vm.Count, Is.EqualTo(3));
        });
    }

    [Test]
    public void GetOrAdd_HandlesNegativeValues()
    {
        var vm = new DenseIdMap();
        Assert.Multiple(() =>
        {
            Assert.That(vm.GetOrAdd(-1000), Is.EqualTo(0));
            Assert.That(vm.GetOrAdd(-500), Is.EqualTo(1));
            Assert.That(vm.GetOrAdd(int.MinValue), Is.EqualTo(2));
            Assert.That(vm.GetOrAdd(int.MaxValue), Is.EqualTo(3));
            Assert.That(vm.Count, Is.EqualTo(4));
        });
    }

    [Test]
    public void GetOrAdd_HandlesZeroValue()
    {
        var vm = new DenseIdMap();
        Assert.Multiple(() =>
        {
            Assert.That(vm.GetOrAdd(0), Is.EqualTo(0));
            Assert.That(vm.GetOrAdd(1), Is.EqualTo(1));
            Assert.That(vm.GetOrAdd(0), Is.EqualTo(0));
            Assert.That(vm.Count, Is.EqualTo(2));
        });
    }

    [Test]
    public void TryGetId_ExistingValue_ReturnsTrue()
    {
        var vm = new DenseIdMap();
        vm.GetOrAdd(42);
        vm.GetOrAdd(100);
        int id = 0;
        Assert.Multiple(() =>
        {
            Assert.That(vm.TryGetId(42, out id), Is.True);
            Assert.That(id, Is.EqualTo(0));
            Assert.That(vm.TryGetId(100, out id), Is.True);
            Assert.That(id, Is.EqualTo(1));
        });
    }

    [Test]
    public void TryGetId_NonExistingValue_ReturnsFalse()
    {
        var vm = new DenseIdMap();
        vm.GetOrAdd(42);
        int id = 0;
        Assert.Multiple(() =>
        {
            Assert.That(vm.TryGetId(999, out id), Is.False);
            Assert.That(id, Is.EqualTo(-1));
        });
    }

    [Test]
    public void GetValue_ValidId_ReturnsOriginalValue()
    {
        var vm = new DenseIdMap();
        var values = new[] { 42, -100, 0, int.MaxValue, int.MinValue };
        for (int i = 0; i < values.Length; i++)
            vm.GetOrAdd(values[i]);
        for (int i = 0; i < values.Length; i++)
            Assert.That(vm.GetValue(i), Is.EqualTo(values[i]));
    }

    [Test]
    public void GetValue_InvalidId_ThrowsArgumentOutOfRangeException()
    {
        var vm = new DenseIdMap();
        vm.GetOrAdd(42);
        Assert.Multiple(() =>
        {
            Assert.That(() => vm.GetValue(-1), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => vm.GetValue(1), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => vm.GetValue(100), Throws.InstanceOf<ArgumentOutOfRangeException>());
        });
    }

    [Test]
    public void Count_InitiallyZero()
    {
        var vm = new DenseIdMap();
        Assert.That(vm.Count, Is.EqualTo(0));
    }

    [Test]
    public void Count_IncreasesWithUniqueValues()
    {
        var vm = new DenseIdMap();
        Assert.That(vm.Count, Is.EqualTo(0));
        vm.GetOrAdd(1);
        Assert.That(vm.Count, Is.EqualTo(1));
        vm.GetOrAdd(2);
        Assert.That(vm.Count, Is.EqualTo(2));
        vm.GetOrAdd(1);
        Assert.That(vm.Count, Is.EqualTo(2));
    }

    [Test]
    public void ResizingBehavior_ManyValues()
    {
        var vm = new DenseIdMap(4);
        var values = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            values.Add(i * 1000);
            vm.GetOrAdd(i * 1000);
        }
        Assert.That(vm.Count, Is.EqualTo(100));
        for (int i = 0; i < values.Count; i++)
        {
            int id = 0;
            Assert.Multiple(() =>
            {
                Assert.That(vm.TryGetId(values[i], out id), Is.True);
                Assert.That(id, Is.EqualTo(i));
                Assert.That(vm.GetValue(id), Is.EqualTo(values[i]));
            });
        }
    }

    [Test]
    public void HashCollisions_HandledCorrectly()
    {
        var vm = new DenseIdMap(8);
        var values = new[] { 1, 9, 17, 25, 33 };
        var ids = new int[values.Length];
        for (int i = 0; i < values.Length; i++)
            ids[i] = vm.GetOrAdd(values[i]);
        for (int i = 0; i < values.Length; i++)
        {
            int retrievedId = 0;
            Assert.Multiple(() =>
            {
                Assert.That(vm.TryGetId(values[i], out retrievedId), Is.True);
                Assert.That(retrievedId, Is.EqualTo(ids[i]));
                Assert.That(vm.GetValue(ids[i]), Is.EqualTo(values[i]));
            });
        }
    }

    [Test]
    public void LargeValues_HandledCorrectly()
    {
        var vm = new DenseIdMap();
        var largeValues = new[] {
            int.MaxValue,
            int.MinValue,
            int.MaxValue - 1,
            int.MinValue + 1,
            1000000000,
            -1000000000
        };
        for (int i = 0; i < largeValues.Length; i++)
            Assert.That(vm.GetOrAdd(largeValues[i]), Is.EqualTo(i));
        Assert.That(vm.Count, Is.EqualTo(largeValues.Length));
        for (int i = 0; i < largeValues.Length; i++)
            Assert.That(vm.GetValue(i), Is.EqualTo(largeValues[i]));
    }

    [Test]
    public void StressTest_ManyRandomValues()
    {
        var vm = new DenseIdMap();
        var rng = new Random(12345);
        var values = new HashSet<int>();
        var valueToIdMapping = new Dictionary<int, int>();
        for (int i = 0; i < 1000; i++)
        {
            int value = rng.Next(int.MinValue, int.MaxValue);
            int id = vm.GetOrAdd(value);
            if (!values.Contains(value))
            {
                values.Add(value);
                valueToIdMapping[value] = id;
            }
            Assert.That(id, Is.EqualTo(valueToIdMapping[value]));
        }
        Assert.That(vm.Count, Is.EqualTo(values.Count));
        foreach (var kvp in valueToIdMapping)
        {
            int retrievedId = 0;
            Assert.Multiple(() =>
            {
                Assert.That(vm.TryGetId(kvp.Key, out retrievedId), Is.True);
                Assert.That(retrievedId, Is.EqualTo(kvp.Value));
                Assert.That(vm.GetValue(kvp.Value), Is.EqualTo(kvp.Key));
            });
        }
    }

    [Test]
    public void ForeachIteration_ReturnsValuesInIdOrder()
    {
        var vm = new DenseIdMap();
        var values = new[] { 100, 200, 50, -25, 1000 };
        foreach (var value in values)
            vm.GetOrAdd(value);
        var iteratedValues = new List<int>();
        foreach (int value in vm)
            iteratedValues.Add(value);
        Assert.That(iteratedValues, Is.EqualTo(values));
    }

    [Test]
    public void ForeachIteration_EmptyMap_ReturnsNothing()
    {
        var vm = new DenseIdMap();
        var count = 0;
        foreach (int value in vm)
            count++;
        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public void ForeachIteration_SingleValue()
    {
        var vm = new DenseIdMap();
        vm.GetOrAdd(42);
        var iteratedValues = new List<int>();
        foreach (int value in vm)
            iteratedValues.Add(value);
        Assert.Multiple(() =>
        {
            Assert.That(iteratedValues, Has.Count.EqualTo(1));
            Assert.That(iteratedValues[0], Is.EqualTo(42));
        });
    }

    [Test]
    public void ForeachIteration_AfterResize_CorrectOrder()
    {
        var vm = new DenseIdMap(4);
        var values = new List<int>();
        for (int i = 0; i < 50; i++)
        {
            int value = i * 100;
            values.Add(value);
            vm.GetOrAdd(value);
        }
        var iteratedValues = new List<int>();
        foreach (int value in vm)
            iteratedValues.Add(value);
        Assert.That(iteratedValues, Is.EqualTo(values));
    }

    [Test]
    public void ForeachIteration_WithDuplicateGetOrAdd_CorrectBehavior()
    {
        var vm = new DenseIdMap();
        vm.GetOrAdd(100);
        vm.GetOrAdd(200);
        vm.GetOrAdd(100);
        vm.GetOrAdd(300);
        var iteratedValues = new List<int>();
        foreach (int value in vm)
            iteratedValues.Add(value);
        Assert.That(iteratedValues, Is.EqualTo(new[] { 100, 200, 300 }));
    }

    [Test]
    public void GetEnumerator_MultipleCalls_IndependentIterators()
    {
        var vm = new DenseIdMap();
        vm.GetOrAdd(10);
        vm.GetOrAdd(20);
        vm.GetOrAdd(30);
        var enumerator1 = vm.GetEnumerator();
        var enumerator2 = vm.GetEnumerator();
        Assert.That(enumerator1.MoveNext(), Is.True);
        Assert.That(enumerator1.Current, Is.EqualTo(10));
        Assert.That(enumerator2.MoveNext(), Is.True);
        Assert.That(enumerator2.Current, Is.EqualTo(10));
        Assert.That(enumerator1.MoveNext(), Is.True);
        Assert.That(enumerator1.Current, Is.EqualTo(20));
    }
}
