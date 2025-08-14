using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IntSet.Tests;

[TestFixture]
public class IntSetClusteredVsHashSet
{
    private IntSetClustered _intSetClustered;
    private HashSet<int> _hashSet;

    [SetUp]
    public void Setup()
    {
        _intSetClustered = new IntSetClustered();
        _hashSet = new HashSet<int>();
    }

    IEnumerable<(int[], int[])> TestArrays()
    {
        int[] A;
        int[] B;
        A = [12, 98, 123, 118281, -2131, 329999, 32, 1, 2, 0];
        B = [12, 1, 2, 3, -82, 11, 54, 27, 901, 324];
        yield return (A, B);
        A = [98, 12, 98, 123, 118281, -2131, 329999, 32, 1, 2, 0];
        B = [12, 1, 2, 3, -82, 11, 54, 27, 901, 324];
        yield return (A, B);
        A = [-13, 12, 98, 123, 118281, -2131, 329999, 32, 1, 2, 0];
        B = [1002, 1, 2, 3, -82, 11, 54, 27, 901, 324];
        yield return (A, B);
        A = [0, 64, 128, 192, 256, 320];
        B = [0, 64];                     
        yield return (A, B);
        A = [1000, 2000, 3000, 4000, 5000];  
        B = [1000];                          
        yield return (A, B);
        A = [-1000, -500, 0, 500, 1000];     
        B = [0, 1];                         
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
        var intSet = new IntSetClustered();
        var hashSet = new HashSet<int>();
        foreach (var (a, b) in TestArrays())
        {
            foreach (var i in a)
            {
                intSet.Add(i);
                hashSet.Add(i);
            }
            foreach (var i in a)
            {
                Assert.That(intSet.Contains(i), Is.EqualTo(hashSet.Contains(i)));
            }
            foreach (var i in b)
            {
                Assert.That(intSet.Contains(i), Is.EqualTo(hashSet.Contains(i)));
            }
            var intSetList = intSet.ToList();
            Assert.That(intSetList, Is.EquivalentTo(hashSet));
        }
    }

    [Test]
    public void Check_IntersectionWith_Span()
    {
        foreach (var (a, b) in TestArrays())
        {
            var hashSet = new HashSet<int>(a);
            var intSet = new IntSetClustered(a);
            Assert.That(intSet.ToList(), Is.EquivalentTo(hashSet));
            intSet.IntersectWith(b);
            hashSet.IntersectWith(b);
            Assert.That(intSet.ToList(), Is.EquivalentTo(hashSet));
        }
    }
    
    [Test]
    public void Check_IntersectionWith_IntSet()
    {
        foreach (var (a, b) in TestArrays())
        {
            var hashSet = new HashSet<int>(a);
            var intSet = new IntSetClustered(a);
            Assert.That(intSet.ToList(), Is.EquivalentTo(hashSet));
            var intSetB = new IntSetClustered(b);
            intSet.IntersectWith(intSetB);
            hashSet.IntersectWith(b);
            Assert.That(intSet.ToList(), Is.EquivalentTo(hashSet));
        }
    }

    [Test]
    public void CheckIntersection_IntSet_IntSet()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};


        _intSetClustered.UnionWith(new IntSetClustered(a));
        _intSetClustered.IntersectWith(new IntSetClustered(b));

        _hashSet.UnionWith(a);
        _hashSet.IntersectWith(b);

        Assert.That(_hashSet, Is.EquivalentTo(_intSetClustered.ToList()));
    }

    [Test]
    public void CheckExceptWith_IntSet_IntSet()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};

        _intSetClustered.UnionWith(new IntSetClustered(a));
        _intSetClustered.ExceptWith(new IntSetClustered(b));

        _hashSet.UnionWith(a);
        _hashSet.ExceptWith(b);

        Assert.That(_hashSet, Is.EquivalentTo(_intSetClustered.ToList()));
    }

    [Test]
    public void CheckSymmetricExceptWith_IntSet_IntSet()
    {
        var a = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};
        var b = new[] {0, 1, 5, 10, 12312312};

        _intSetClustered.UnionWith(new IntSetClustered(a));
        _intSetClustered.SymmetricExceptWith(new IntSetClustered(b));

        _hashSet.UnionWith(a);
        _hashSet.SymmetricExceptWith(b);

        Assert.That(_hashSet, Is.EquivalentTo(_intSetClustered.ToList()));
    }

    [Test]
    public void CheckIterator()
    {
        var values = new[] {0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000};

        foreach (var value in values)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        foreach (var value in _intSetClustered)
        {
            Assert.That(_intSetClustered.Contains(value), Is.True, $"Got an incorrect value from IdSet iterator: {value}");
        }
    }

    [Test]
    public void ExceptWith_EmptySpan_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5};
        var exceptValues = Array.Empty<int>();

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        _intSetClustered.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void ExceptWith_NoOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3};
        var exceptValues = new[] {4, 5, 6};

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        _intSetClustered.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void ExceptWith_PartialOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5};
        var exceptValues = new[] {3, 4, 5, 6, 7};

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        _intSetClustered.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void ExceptWith_CompleteOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5};
        var exceptValues = new[] {1, 2, 3, 4, 5, 6, 7};

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        _intSetClustered.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_EmptySpan_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5, 0, -1};
        var symmetricExceptValues = Array.Empty<int>();

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        _intSetClustered.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_NoOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3};
        var symmetricExceptValues = new[] {4, 5, 6, -43};

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        _intSetClustered.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_PartialOverlap_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, 4, 5, -92342};
        var symmetricExceptValues = new[] {-1, 3, 4, 5, 6, 7};

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        _intSetClustered.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_CompleteOverlap_MatchesHashSet()
    {
        var initialValues = new[] {2, 1, 3, 4, 5};
        var symmetricExceptValues = new[] {1, -3, 2, 3, 4, 5};

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        _intSetClustered.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void SymmetricExceptWith_WithDuplicates_MatchesHashSet()
    {
        var initialValues = new[] {1, 2, 3, -7};
        var symmetricExceptValues = new[] {2, 3, 3, 4, 4, 5};

        foreach (var value in initialValues)
        {
            _intSetClustered.Add(value);
            _hashSet.Add(value);
        }

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));

        _intSetClustered.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);

        Assert.That(_intSetClustered.ToList(), Is.EquivalentTo(_hashSet));
    }

    [Test]
    public void CheckUnionWith_IntSet_IntSet_Shuffled()
    {
        foreach (var (a, b) in TestArrays())
        {
            Console.WriteLine($"a => {string.Join(", ", a)}");
            Console.WriteLine($"b => {string.Join(", ", b)}");
            var truth = new HashSet<int>(a);

            var intSetA = new IntSetClustered(a);
            Assert.That(intSetA.ToList(), Is.EquivalentTo(truth));
            intSetA.UnionWith(b);

            Shuffle(a);
            Shuffle(b);

            var intSetB = new IntSetClustered(a);
            Assert.That(intSetB.ToList(), Is.EquivalentTo(truth));
            intSetB.UnionWith(b);

            var intSetC = new IntSetClustered(a);
            var otherSet = new IntSetClustered(b);
            foreach(var i in b) Assert.That(otherSet.Contains(i), Is.True);
            Console.WriteLine($"intSetC => {string.Join(", ", intSetC)}");
            Console.WriteLine($"otherSet => {string.Join(", ", otherSet)}");
            intSetC.UnionWith(otherSet);
            Console.WriteLine($"union => {string.Join(", ", intSetC)}");

            Shuffle(a);
            Shuffle(b);

            var intSetD = new IntSetClustered(a);
            Assert.That(intSetD.ToList(), Is.EquivalentTo(truth));

            intSetD.UnionWith(new IntSetClustered(b));

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

        var intSetA = new IntSetClustered(A);
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

            var intSetA = new IntSetClustered(a);
            Assert.That(intSetA.ToList(), Is.EquivalentTo(truth));
            intSetA.IntersectWith(b);

            Shuffle(a);
            Shuffle(b);

            var intSetB = new IntSetClustered(a);
            Assert.That(intSetB.ToList(), Is.EquivalentTo(truth));
            intSetB.IntersectWith(b);

            var intSetC = new IntSetClustered(a);
            intSetC.IntersectWith(new IntSetClustered(b));

            Shuffle(a);
            Shuffle(b);

            var intSetD = new IntSetClustered(a);
            Assert.That(intSetD.ToList(), Is.EquivalentTo(truth));

            intSetD.IntersectWith(new IntSetClustered(b));

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
        foreach (var (a, b) in TestArrays())
        {
            var insetA = new IntSetClustered(a);
            insetA.ExceptWith(new IntSetClustered(b));
            Shuffle(a);
            Shuffle(b);
            var intSetB = new IntSetClustered(a);
            intSetB.ExceptWith(new IntSetClustered(b));

            Assert.That(intSetB.ToList(), Is.EquivalentTo(insetA.ToList()));
        }
    }

    [Test]
    public void CheckSymmetricExceptWith_IntSet_IntSet_Shuffled()
    {
        foreach (var (a, b) in TestArrays())
        {
            var intSetA = new IntSetClustered(a);
            intSetA.SymmetricExceptWith(new IntSetClustered(b));

            Shuffle(a);
            Shuffle(b);
            var intSetB = new IntSetClustered(a);
            intSetB.SymmetricExceptWith(new IntSetClustered(b));

            Assert.That(intSetB.ToList(), Is.EquivalentTo(intSetA.ToList()));
        }
    }
}