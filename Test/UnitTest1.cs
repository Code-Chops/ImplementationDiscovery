using TestProject2;

namespace Test;

public partial class UnitTest1 : UnitTest2
{
	[Fact]
	public void Test1()
	{
		var _ = typeof(UnitTest1Enum);
	}
}