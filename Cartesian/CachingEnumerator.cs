using System.Collections;
using System.Collections.Generic;

namespace Cartesian
{
	public class CachingEnumerator<T> : IEnumerator<T>
	{
		private readonly IEnumerator<T> _itemsEnumerator;
		private readonly HashSet<T> _cachedItems;
		private bool _fullCache;
		private IEnumerator<T> _cachedItemsEnumerator;
		private IEnumerator arrayEnumerator;

		public CachingEnumerator(IEnumerator<T> itemsEnumerator)
		{
			_itemsEnumerator = itemsEnumerator;
			_cachedItems = new HashSet<T>();
		}

		public T Current
		{
			get
			{
				if (_fullCache)
					return _cachedItemsEnumerator.Current;

				var item = _itemsEnumerator.Current;
				_cachedItems.Add(item);
				return item;
			}
		}

		public void Dispose()
		{
			_cachedItemsEnumerator.Dispose();
			_itemsEnumerator.Dispose();
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public bool MoveNext()
		{
			if (_fullCache)
				return _cachedItemsEnumerator.MoveNext();

			var moved = _itemsEnumerator.MoveNext();
			if (!moved)
			{
				_fullCache = true;
				_cachedItemsEnumerator = _cachedItems.GetEnumerator();
			}
			return moved;
		}

		public void Reset()
		{
			if (_fullCache)
				_cachedItemsEnumerator.Reset();
			else
				_itemsEnumerator.Reset();
		}
	}
}
