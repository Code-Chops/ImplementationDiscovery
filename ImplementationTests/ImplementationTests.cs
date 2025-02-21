using ToBeImplementedMocks;
using Xunit;
using Xunit.Abstractions;
using ITest = ToBeImplementedMocks.ITest;

namespace ImplementationTests;

public record Mock1A : TestBase<int>;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Assert.True(TestUltimateProxyEnum<int>.TryGetSingleMember(nameof(Mock1A), out _));
    }
}

public partial record UnitTest2 : ITest
{
    [Fact]
    public void Test2()
    {
        Assert.True(TestProxyEnum.TryGetSingleMember(nameof(UnitTest2), out _));
    }
}

public partial record UnitTest3 : TestBase
{
    [Fact]
    public void Test3()
    {
        Assert.True(TestProxyEnum.TryGetSingleMember(nameof(UnitTest3), out _));
    }
}

public record Mock4A<T> : TestUltimateBase<T>;

public record UnitTest4
{
    [Fact]
    public void Test4()
    {
        Assert.True(TestUltimateProxyEnum<int>.TryGetSingleMember(nameof(Mock4A<int>), out _));
    }
}

public record UnitTest5
{
    public UnitTest5(ITestOutputHelper testOutputHelper)
    {
        this.TestOutputHelper = testOutputHelper;
    }

    public ITestOutputHelper TestOutputHelper { get; }

    [Fact]
    public void Test5()
    {
        var a = TestUltimateProxyEnum<int>.GetInstances();
        this.TestOutputHelper.WriteLine(a.Count().ToString());
    }
}

public record UnitTest6
{
    [Fact]
    public void RetrieveExternalMemberFromSourceAssembly()
    {
        AllDiscoveredImplementations.Initialize();

        ITest.GetMemberOnSourceAssembly(nameof(UnitTest3));
    }
}