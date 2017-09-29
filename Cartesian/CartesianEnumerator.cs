using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cartesian
{
	public class CartesianEnumerator<TElement, TDimension> : IEnumerator<TElement[]>
		where TDimension : IEnumerable<TElement>
	{
		private readonly IEnumerator<TElement>[] _dimensionsEnumerators;

		public CartesianEnumerator(IEnumerable<TDimension> dimensions, bool useCache = true)
		{
			Func<IEnumerable<TElement>, IEnumerator<TElement>> getEnumerator = useCache ?
				new Func<IEnumerable<TElement>, IEnumerator<TElement>>((e) => new CachingEnumerator<TElement>(e.GetEnumerator())) :
				(e) => e.GetEnumerator();
			_dimensionsEnumerators = dimensions.Select(d => getEnumerator(d)).ToArray();
			foreach (var enumerator in _dimensionsEnumerators.Skip(1))
				if (!enumerator.MoveNext())
					throw new InvalidOperationException();
		}

		public TElement[] Current
		{
			get
			{
				return _dimensionsEnumerators.Select(e => e.Current).ToArray();
			}
		}

		public void Dispose()
		{
			foreach (var enumerator in _dimensionsEnumerators)
				enumerator.Dispose();
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public bool MoveNext()
		{
			for (int i = 0; i < _dimensionsEnumerators.Length; i++)
			{
				var enumerator = _dimensionsEnumerators[i];
				if (enumerator.MoveNext())
					return true;
				else
				{
					enumerator.Reset();
					if (!enumerator.MoveNext())
						throw new InvalidOperationException();
				}
			}
			return false;
		}

		public void Reset()
		{
			_dimensionsEnumerators.First().Reset();
			foreach (var enumerator in _dimensionsEnumerators.Skip(1))
			{
				enumerator.Reset();
				if (!enumerator.MoveNext())
					throw new InvalidOperationException();
			}
		}
	}
}
