using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompactJson;
using System;

namespace UnitTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestSerializeScalar()
		{
			var d = DateTime.Now;
			var a = Converter.Serialize(d);
			var b = Converter.Deserialize<DateTime>(a);
			Assert.AreEqual(5, Converter.Deserialize<int>(Converter.Serialize(5)));
			Assert.AreEqual(5.5, Converter.Deserialize<float>(Converter.Serialize(5.5)));
			Assert.AreEqual("asd", Converter.Deserialize<string>(Converter.Serialize("asd")));
			Assert.AreEqual(true, Converter.Deserialize<bool>(Converter.Serialize(true)));
			Assert.IsInstanceOfType(Converter.Deserialize<DateTime>(Converter.Serialize(d)), typeof(DateTime));
		}
	}
}
