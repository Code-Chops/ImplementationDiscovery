using TestProject2;

namespace Test;

public partial class UnitTest1 : UnitTest2<int>
{
	[Fact]
	public void Test1()
	{
		var a = typeof(UnitTest1Enum);
	}
}