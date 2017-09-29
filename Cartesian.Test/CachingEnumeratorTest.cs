using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cartesian.Test
{
	[TestClass]
	public class CachingEnumeratorTest
	{
		private static void TestCase(IEnumerator<int> naturalNumberEnumerator, int expectedLength)
		{
			IEnumerator<int> instance = new CachingEnumerator<int>(naturalNumberEnumerator);

			var i = 0;
			while (instance.MoveNext())
			{
				Assert.IsTrue(i < expectedLength);
				Assert.AreEqual(i, instance.Current);
				i++;
			}
			instance.Reset();

			i = 0;
			while (instance.MoveNext())
			{
				Assert.IsTrue(i < expectedLength);
				Assert.AreEqual(i, instance.Current);
				i++;
			}
		}

		[TestMethod]
		public void DirectCase()
		{
			var list = new List<int> { 0, 1, 2, 3 };
			TestCase(list.GetEnumerator(), list.Count);
		}

		[TestMethod]
		public void DeferredCase()
		{
			TestCase(GetDeferredNumbers().GetEnumerator(), _deferredNumbersLength);
		}

		private const int _deferredNumbersLength = 10;

		private static IEnumerable<int> GetDeferredNumbers()
		{
			for (int i = 0; i < _deferredNumbersLength; i++)
				yield return i;
		}
	}
}
