namespace IntSet.Tests;

[TestFixture]
public class TestCenteredArray
{
    [Test]
    public void TestCenteredArrayIndexer()
    {
        var centeredArray = new CenteredArray<int>(2);
        centeredArray[0] = 1;
        centeredArray[1] = 2;
        centeredArray[-1] = 3;
        centeredArray[-2] = 4;
        Assert.That(centeredArray[1], Is.EqualTo(2));
        Assert.That(centeredArray[0], Is.EqualTo(1));
        Assert.That(centeredArray[-1], Is.EqualTo(3));
        Assert.That(centeredArray[-2], Is.EqualTo(4));
        int[] expectedArray = [4,3,1,2];
        Assert.That(centeredArray.Array, Is.EqualTo(expectedArray));
    }
    
    [Test]
    public void TestCenteredArrayIndexerGrowthOnNegative()
    {
        var centeredArray = new CenteredArray<int>(2);
        centeredArray[0] = 1;
        centeredArray[1] = 2;
        centeredArray[-1] = 3;
        centeredArray[-2] = 4;
        centeredArray[-3] = 5; // This should trigger a resize
        int[] expectedArray = [0,5,4,3,1,2,0,0];
        Assert.That(centeredArray.Array, Is.EqualTo(expectedArray));
    }
    
    [Test]
    public void TestCenteredArrayIndexerGrowthOnPositive()
    {
        var centeredArray = new CenteredArray<int>(2);
        centeredArray[0] = 1;
        centeredArray[1] = 2;
        centeredArray[-1] = 3;
        centeredArray[-2] = 4;
        centeredArray[2] = 5; // This should trigger a resize
        int[] expectedArray = [0,0,4,3,1,2,5,0];
        Assert.That(centeredArray.Array, Is.EqualTo(expectedArray));
    }
}