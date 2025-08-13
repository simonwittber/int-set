using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IntSet.Tests;

[TestFixture]
public class IntSetPagedVsHashSet
{
    private IntSetPaged _intSetPaged;
    private HashSet<int> _hashSet;

    [SetUp]
    public void Setup()
    {
        _intSetPaged = new IntSetPaged();
        _hashSet = new HashSet<int>();
    }

    IEnumerable<(int[], int[])> TestArrays()
    {
        int[] A;
        int[] B;
        // two arrays with same starting value
        A = new[] {12, 98, 123, 118281, -2131, 329999, 32, 1, 2, 0};
        B = new[] {12, 1, 2, 3, -82, 11, 54, 27, 901, 324};
        yield return (A, B);
        A = new[] {98, 12, 98, 123, 118281, -2131, 329999, 32, 1, 2, 0};
        B = new[] {12, 1, 2, 3, -82, 11, 54, 27, 901, 324};
        yield return (A, B);
        A = new[] {-13, 12, 98, 123, 118281, -2131, 329999, 32, 1, 2, 0};
        B = new[] {1002, 1, 2, 3, -82, 11, 54, 27, 901, 324};
        yield return (A, B);
    }

    void Shuffle<T>(IList<T> list)
    {
        var rng = new Random(123);
        int n = list.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    [Test]
    public void CheckAddAndContains()
    {
        var values = new[] {0, 1, 5, -10, 63, 64, 65, 127, 128, 1000, 10000};

        foreach (var value in values)
        {
            var idSetResult = _intSetPaged.Add(value);
            var hashSetResult = _hashSet.Add(value);
            Assert.That(idSetResult, Is.EqualTo(hashSetResult));
        }

        for (var i = -10000; i < 100000; i++)
        {
            Assert.That(_intSetPaged.Contains(i), Is.EqualTo(values.Contains(i)));
        }
    }

    [Test]
    public void CheckIntersection()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};

        _intSetPaged.UnionWith(a);
        Assert.That(_intSetPaged.Count, Is.EqualTo(a.Length));
        _intSetPaged.IntersectWith(b);
        Assert.That(_intSetPaged.Count, Is.EqualTo(4));
        Assert.That(_intSetPaged.Contains(b[0]), Is.True);
        Assert.That(_intSetPaged.Contains(b[1]), Is.True);
        Assert.That(_intSetPaged.Contains(b[2]), Is.True);
        Assert.That(_intSetPaged.Contains(b[3]), Is.True);
        Assert.That(_intSetPaged.Contains(b[4]), Is.False);

        Assert.That(_intSetPaged.Contains(a[4]), Is.False);
        Assert.That(_intSetPaged.Contains(a[5]), Is.False);
        Assert.That(_intSetPaged.Contains(a[6]), Is.False);
        Assert.That(_intSetPaged.Contains(a[7]), Is.False);
        Assert.That(_intSetPaged.Contains(a[8]), Is.False);
        Assert.That(_intSetPaged.Contains(a[9]), Is.False);
        Assert.That(_intSetPaged.Contains(a[10]), Is.False);

        _hashSet.UnionWith(a);
        _hashSet.IntersectWith(b);

        Assert.That(_hashSet, Is.EquivalentTo(_intSetPaged.ToList()));
    }

    [Test]
    public void CheckIntersection_IntSet_IntSet()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};


        _intSetPaged.UnionWith(new IntSetPaged(a));
        _intSetPaged.IntersectWith(new IntSetPaged(b));

        _hashSet.UnionWith(a);
        _hashSet.IntersectWith(b);

        Assert.That(_hashSet, Is.EquivalentTo(_intSetPaged.ToList()));
    }

    [Test]
    public void CheckExceptWith_IntSet_IntSet()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};

        _intSetPaged.UnionWith(new IntSetPaged(a));
        _intSetPaged.ExceptWith(new IntSetPaged(b));

        _hashSet.UnionWith(a);
        _hashSet.ExceptWith(b);

        Assert.That(_hashSet, Is.EquivalentTo(_intSetPaged.ToList()));
    }

    [Test]
    public void CheckSymmetricExceptWith_IntSet_IntSet()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};

        _intSetPaged.UnionWith(new IntSetPaged(a));
        _intSetPaged.SymmetricExceptWith(new IntSetPaged(b));

        _hashSet.UnionWith(a);
        _hashSet.SymmetricExceptWith(b);

        Assert.That(_hashSet, Is.EquivalentTo(_intSetPaged.ToList()));
    }

    [Test]
    public void CheckIterator()
    {
        var values = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};

        foreach (var value in values)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        foreach (var value in _intSetPaged)
        {
            Assert.That(_intSetPaged.Contains(value), Is.True, $"Got an incorrect value from IdSet iterator: {value}");
        }
    }

    [Test]
    public void ExceptWith_EmptySpan_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5};
        var exceptValues = Array.Empty<int>();

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        _intSetPaged.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void ExceptWith_NoOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3};
        var exceptValues = new[] {4, 5, 6};

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        _intSetPaged.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void ExceptWith_PartialOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5};
        var exceptValues = new[] {3, 4, 5, 6, 7};

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        _intSetPaged.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void ExceptWith_CompleteOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5};
        var exceptValues = new[] {1, 2, 3, 4, 5, 6, 7};

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        _intSetPaged.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_EmptySpan_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5, 0, -1};
        var symmetricExceptValues = Array.Empty<int>();

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        _intSetPaged.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_NoOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3};
        var symmetricExceptValues = new[] {4, 5, 6, -43};

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        _intSetPaged.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_PartialOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5, -92342};
        var symmetricExceptValues = new[] {-1, 3, 4, 5, 6, 7};

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        _intSetPaged.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_CompleteOverlap_MatchesHashSet()
    {
        var initialValues = new[] {2, 1, 3, 4, 5};
        var symmetricExceptValues = new[] {1, -3, 2, 3, 4, 5};

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        _intSetPaged.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_WithDuplicates_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, -7};
        var symmetricExceptValues = new[] {2, 3, 3, 4, 4, 5};

        foreach (var value in initialValues)
        {
            _intSetPaged.Add(value);
            _hashSet.Add(value);
        }

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));

        _intSetPaged.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetPaged.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void CheckUnionWith_IntSet_IntSet_Shuffled()
    {
        foreach (var (a, b) in TestArrays())
        {
            var truth = new HashSet<int>(a);

            var intSetA = new IntSetPaged(a);
            Assert.That(intSetA.ToList(), Is.EquivalentTo(truth));
            intSetA.UnionWith(b);

            Shuffle(a);
            Shuffle(b);

            var intSetB = new IntSetPaged(a);
            Assert.That(intSetB.ToList(), Is.EquivalentTo(truth));
            intSetB.UnionWith(b);

            var intSetC = new IntSetPaged(a);
            intSetC.UnionWith(new IntSetPaged(b));

            Shuffle(a);
            Shuffle(b);

            var intSetD = new IntSetPaged(a);
            Assert.That(intSetD.ToList(), Is.EquivalentTo(truth));

            intSetD.UnionWith(new IntSetPaged(b));

            truth.UnionWith(b);
            var arrayFirstValues = $"{a[0] & ~63}, {b[0] & ~63}";
            Assert.That(intSetA.Count, Is.EqualTo(truth.Count), arrayFirstValues);
            Assert.That(intSetA.ToList(), Is.EquivalentTo(truth), arrayFirstValues);
            Assert.That(intSetB.Count, Is.EqualTo(truth.Count), arrayFirstValues);
            Assert.That(intSetB.ToList(), Is.EquivalentTo(truth), arrayFirstValues);
            // Assert.That(intSetC.Count, Is.EqualTo(truth.Count), arrayFirstValues);
            Assert.That(intSetC.ToList(), Is.EquivalentTo(truth), arrayFirstValues);
            Assert.That(intSetD.Count, Is.EqualTo(truth.Count), arrayFirstValues);
            Assert.That(intSetD.ToList(), Is.EquivalentTo(truth), arrayFirstValues);
        }
    }


    [Test]
    public void CheckIntersectWith_IntSet_IntSet_Bug()
    {
        var A = new[] {12, 98, 123, 118281, -2131, 329999, 32, 1, 2, 0};
        var B = new[] {12, 1, 2, 3, -82, 11, 54, 27, 901, 324};

        var truth = new HashSet<int>(A);

        var intSetA = new IntSetPaged(A);
        Assert.That(intSetA.ToList(), Is.EquivalentTo(truth));
        
        intSetA.IntersectWith(B);

        truth.IntersectWith(B);

        //Assert.That(intSetA.Count, Is.EqualTo(truth.Count));
        Assert.That(intSetA.ToList(), Is.EquivalentTo(truth));
    }

    [Test]
    public void CheckIntersectWith_IntSet_IntSet_Shuffled()
    {
        foreach (var (a, b) in TestArrays())
        {
            var truth = new HashSet<int>(a);

            var intSetA = new IntSetPaged(a);
            Assert.That(intSetA.ToList(), Is.EquivalentTo(truth));
            intSetA.IntersectWith(b);

            Shuffle(a);
            Shuffle(b);

            var intSetB = new IntSetPaged(a);
            Assert.That(intSetB.ToList(), Is.EquivalentTo(truth));
            intSetB.IntersectWith(b);

            var intSetC = new IntSetPaged(a);
            intSetC.IntersectWith(new IntSetPaged(b));

            Shuffle(a);
            Shuffle(b);

            var intSetD = new IntSetPaged(a);
            Assert.That(intSetD.ToList(), Is.EquivalentTo(truth));

            intSetD.IntersectWith(new IntSetPaged(b));

            truth.IntersectWith(b);

            Assert.That(intSetA.Count, Is.EqualTo(truth.Count));
            Assert.That(intSetA.ToList(), Is.EquivalentTo(truth));
            Assert.That(intSetB.Count, Is.EqualTo(truth.Count));
            Assert.That(intSetB.ToList(), Is.EquivalentTo(truth));
            Assert.That(intSetC.Count, Is.EqualTo(truth.Count));
            Assert.That(intSetC.ToList(), Is.EquivalentTo(truth));
            Assert.That(intSetD.Count, Is.EqualTo(truth.Count));
            Assert.That(intSetD.ToList(), Is.EquivalentTo(truth));
        }
    }

    [Test]
    public void CheckExceptWith_IntSet_IntSet_Shuffled()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};

        _intSetPaged.UnionWith(new IntSetPaged(a));
        _intSetPaged.ExceptWith(new IntSetPaged(b));

        Shuffle(a);
        Shuffle(b);
        var intSetB = new IntSetPaged(a);
        intSetB.ExceptWith(new IntSetPaged(b));

        Assert.That(intSetB.ToList(), Is.EquivalentTo(_intSetPaged.ToList()));
    }

    [Test]
    public void CheckSymmetricExceptWith_IntSet_IntSet_Shuffled()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};

        _intSetPaged.UnionWith(new IntSetPaged(a));
        _intSetPaged.SymmetricExceptWith(new IntSetPaged(b));

        Shuffle(a);
        Shuffle(b);
        var intSetB = new IntSetPaged(a);
        intSetB.SymmetricExceptWith(new IntSetPaged(b));

        Assert.That(intSetB.ToList(), Is.EquivalentTo(_intSetPaged.ToList()));
    }
}