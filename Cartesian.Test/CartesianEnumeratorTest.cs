using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cartesian.Test
{
	[TestClass]
	public class CartesianEnumeratorTest
	{
		private static List<int> _10BaseNumbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		private static void TestCase<T>(IEnumerable<T> dimensions, int expectedLength, bool useCache)
			where T : IEnumerable<int>
		{
			IEnumerator<int[]> instance = new CartesianEnumerator<int, T>(dimensions, useCache);

			var i = 0;
			var sb = new List<string>();
			while (instance.MoveNext())
			{
				Assert.IsTrue(i < expectedLength);
				foreach (var item in instance.Current)
					sb.Add(item.ToString());
				sb.Reverse();
				var actual = int.Parse(string.Join("", sb));
				Assert.AreEqual(i, actual);
				i++;
				sb.Clear();
			}

			if (useCache)
			{
				instance.Reset();
				i = 0;
				sb = new List<string>();
				while (instance.MoveNext())
				{
					Assert.IsTrue(i < expectedLength);
					foreach (var item in instance.Current)
						sb.Add(item.ToString());
					sb.Reverse();
					var actual = int.Parse(string.Join("", sb));
					Assert.AreEqual(i, actual);
					i++;
					sb.Clear();
				}
			}
		}

		[TestMethod]
		public void DirectCase()
		{
			var dimensions = new List<List<int>> 
			{ 
				_10BaseNumbers, 
				_10BaseNumbers, 
				_10BaseNumbers 
			};
			TestCase(dimensions, 1000, false);
		}

		[TestMethod]
		public void DeferredCase()
		{
			var dimensions = new List<IEnumerable<int>> 
			{ 
				GetDeferred10BaseNumbers(), 
				_10BaseNumbers, 
				GetDeferred10BaseNumbers() 
			};
			TestCase(dimensions, 1000, true);
		}

		private IEnumerable<int> GetDeferred10BaseNumbers()
		{
			for (int i = 0; i < _10BaseNumbers.Count; i++)
				yield return _10BaseNumbers[i];
		}
	}
}
