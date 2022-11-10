using ToBeImplemented;

namespace Test;

public partial class UnitTest1 : TestBase<int>
{
	[Fact]
	public void Test1()
	{
		var _ = typeof(TestProxyEnum);
	}
}


public partial class UnitTest2 : ITest
{
	[Fact]
	public void Test1()
	{
		var _ = typeof(TestProxyEnum);
	}
}