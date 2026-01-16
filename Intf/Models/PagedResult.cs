using System.Collections.Generic;

namespace Intf.Models
{
	public class PagedResult<T>
	{
		public IEnumerable<T> Items { get; set; } = new List<T>();
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public bool HasNext { get; set; }
	}
}
