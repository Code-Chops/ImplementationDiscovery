using ToBeImplemented;

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