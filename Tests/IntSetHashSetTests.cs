using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IntSet.Tests;

[TestFixture]
public class IdSetHashSetComparisonTests
{
    private IntSet _intSet;
    private HashSet<int> _hashSet;

    [SetUp]
    public void Setup()
    {
        _intSet = new IntSet();
        _hashSet = new HashSet<int>();
    }
    
    [Test]
    public void CheckAddAndContains()
    {
        var values = new[] { 0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000 };
        
        foreach (var value in values)
        {
            var idSetResult = _intSet.Add(value);
            var hashSetResult = _hashSet.Add(value);
            Assert.That(idSetResult, Is.EqualTo(hashSetResult));
        }

        for (var i = 0; i < 100000; i++)
        {
            Assert.That(_intSet.Contains(i), Is.EqualTo(values.Contains(i)));
        }
    }

    [Test]
    public void CheckIntersection()
    {
        var a = new[] { 0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000 };
        var b = new[] {0, 1, 5, 10, 12312312};
        
        _intSet.UnionWith(a);
        Assert.That(_intSet.Count, Is.EqualTo(a.Length));
        _intSet.IntersectWith(b);
        Assert.That(_intSet.Count, Is.EqualTo(4));
        Assert.That(_intSet.Contains(b[0]), Is.True);
        Assert.That(_intSet.Contains(b[1]), Is.True);
        Assert.That(_intSet.Contains(b[2]), Is.True);
        Assert.That(_intSet.Contains(b[3]), Is.True);
        Assert.That(_intSet.Contains(b[4]), Is.False);
        
        Assert.That(_intSet.Contains(a[4]), Is.False);
        Assert.That(_intSet.Contains(a[5]), Is.False);
        Assert.That(_intSet.Contains(a[6]), Is.False);
        Assert.That(_intSet.Contains(a[7]), Is.False);
        Assert.That(_intSet.Contains(a[8]), Is.False);
        Assert.That(_intSet.Contains(a[9]), Is.False);
        Assert.That(_intSet.Contains(a[10]), Is.False);
        
        _hashSet.UnionWith(a);
        _hashSet.IntersectWith(b);
        
        Assert.That(_hashSet, Is.EquivalentTo(_intSet.ToList()));
    }
    
    [Test]
    public void CheckIntersection_IntSet_IntSet()
    {
        var a = new[] { 0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000 };
        var b = new[] {0, 1, 5, 10, 12312312};
        
        _intSet.UnionWith(new IntSet(a));
        _intSet.IntersectWith(new IntSet(b));
        
        _hashSet.UnionWith(a);
        _hashSet.IntersectWith(b);
        
        Assert.That(_hashSet, Is.EquivalentTo(_intSet.ToList()));
    }
    
    [Test]
    public void CheckExceptWith_IntSet_IntSet()
    {
        var a = new[] { 0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000 };
        var b = new[] {0, 1, 5, 10, 12312312};
        
        _intSet.UnionWith(new IntSet(a));
        _intSet.ExceptWith(new IntSet(b));
        
        _hashSet.UnionWith(a);
        _hashSet.ExceptWith(b);
        
        Assert.That(_hashSet, Is.EquivalentTo(_intSet.ToList()));
    }
    
    [Test]
    public void CheckSymmetricExceptWith_IntSet_IntSet()
    {
        var a = new[] { 0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000 };
        var b = new[] {0, 1, 5, 10, 12312312};
        
        _intSet.UnionWith(new IntSet(a));
        _intSet.SymmetricExceptWith(new IntSet(b));
        
        _hashSet.UnionWith(a);
        _hashSet.SymmetricExceptWith(b);
        
        Assert.That(_hashSet, Is.EquivalentTo(_intSet.ToList()));
    }
    
    [Test]
    public void CheckIterator()
    {
        var values = new[] { 0, 1, 5, 10, 63, 64, 65, 127, 128, 1000, 10000 };
        
        foreach (var value in values)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }

        foreach (var value in _intSet)
        {
            Assert.That(_intSet.Contains(value), Is.True, $"Got an incorrect value from IdSet iterator: {value}");
        }
    }

    [Test]
    public void ExceptWith_EmptySpan_MatchesHashSet()
    {
        var initialValues = new[] { 1, 2, 3, 4, 5 };
        var exceptValues = Array.Empty<int>();
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        
        _intSet.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);
        
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

    }

    [Test]
    public void ExceptWith_NoOverlap_MatchesHashSet()
    {
        var initialValues = new[] { 1, 2, 3 };
        var exceptValues = new[] { 4, 5, 6 }; 
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        
        _intSet.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);
        
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

    }

    [Test]
    public void ExceptWith_PartialOverlap_MatchesHashSet()
    {
        var initialValues = new[] { 1, 2, 3, 4, 5 };
        var exceptValues = new[] { 3, 4, 5, 6, 7 }; 
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        
        _intSet.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);
        
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

    }

    [Test]
    public void ExceptWith_CompleteOverlap_MatchesHashSet()
    {
        var initialValues = new[] { 1, 2, 3, 4, 5 };
        var exceptValues = new[] { 1, 2, 3, 4, 5, 6, 7 }; 
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        
        _intSet.ExceptWith(exceptValues);
        _hashSet.ExceptWith(exceptValues);
        
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

    }

    [Test]
    public void SymmetricExceptWith_EmptySpan_MatchesHashSet()
    {
        var initialValues = new[] { 1, 2, 3, 4, 5, 0, -1 };
        var symmetricExceptValues = Array.Empty<int>(); 
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        
        _intSet.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);
        
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

    }

    [Test]
    public void SymmetricExceptWith_NoOverlap_MatchesHashSet()
    {
        var initialValues = new[] { 1, 2, 3 };
        var symmetricExceptValues = new[] { 4, 5, 6, -43 }; 
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        
        _intSet.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);
        
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

    }

    [Test]
    public void SymmetricExceptWith_PartialOverlap_MatchesHashSet()
    {
        var initialValues = new[] { 1, 2, 3, 4, 5, -92342 };
        var symmetricExceptValues = new[] { -1, 3, 4, 5, 6, 7 };
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        
        _intSet.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);
        
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

    }

    [Test]
    public void SymmetricExceptWith_CompleteOverlap_MatchesHashSet()
    {
        var initialValues = new[] { 2, 1, 3, 4, 5 };
        var symmetricExceptValues = new[] { 1, -3, 2, 3, 4, 5 };
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        
        _intSet.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);
        
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

    }

    [Test]
    public void SymmetricExceptWith_WithDuplicates_MatchesHashSet()
    {
        var initialValues = new[] { 1, 2, 3, -7 };
        var symmetricExceptValues = new[] { 2, 3, 3, 4, 4, 5 };
        
        foreach (var value in initialValues)
        {
            _intSet.Add(value);
            _hashSet.Add(value);
        }
        Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));

        _intSet.SymmetricExceptWith(symmetricExceptValues);
        _hashSet.SymmetricExceptWith(symmetricExceptValues);
        
       Assert.That(_intSet.ToList(), Is.EquivalentTo(_hashSet));
    }

   
}
