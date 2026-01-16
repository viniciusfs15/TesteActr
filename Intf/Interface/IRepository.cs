using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Intf
{
	public interface IRepository<TModel> where TModel : IModel
	{
		Task<TModel?> GetByIdAsync(Guid id);
		Task<(IEnumerable<TModel> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
		Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate);
		Task<TModel> AddAsync(TModel entity);
		Task<TModel> UpdateAsync(TModel entity);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
		Task<int> SaveChangesAsync();
	}
}
