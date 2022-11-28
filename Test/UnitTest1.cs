using ToBeImplemented;
using Xunit.Abstractions;
using ITest = ToBeImplemented.ITest;

namespace Test;

public partial record class UnitTest1 : TestBase<int>
{
	[Fact]
	public void Test1()
	{
		var _ = typeof(TestProxyEnum);
	}
}


public partial record class UnitTest2 : ITest
{
	[Fact]
	public void Test2()
	{
		var _ = typeof(TestProxyEnum);
	}
}

public partial record class UnitTest3 : TestBase
{
	[Fact]
	public void Test3()
	{
		var _ = typeof(TestProxyEnum);
	}
}

public partial record class UnitTest4 : TestUltimateBase
{
	[Fact]
	public void Test4()
	{
		var _ = typeof(TestUltimateProxyEnum);
	}
}

public partial record class UnitTest5
{
	public UnitTest5(ITestOutputHelper testOutputHelper)
	{
		this.TestOutputHelper = testOutputHelper;
	}

	public ITestOutputHelper TestOutputHelper { get; }
	
	
	[Fact]
	public void Test5()
	{
		this.TestOutputHelper.WriteLine(TestUltimateProxyEnum.IsInitialized().ToString());
		var a = TestUltimateProxyEnum.GetDiscoveredObjects();
		this.TestOutputHelper.WriteLine(a.Count().ToString());

	}
}