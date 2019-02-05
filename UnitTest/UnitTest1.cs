using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompactJson;

namespace UnitTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestSerializeScalar()
		{
			var a = Converter.Serialize(5);

			var b = Converter.Deserialize<int>(a);
		}
	}
}
