using Intf;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories
{
	public abstract class RepositoryBase<TModel> : IRepository<TModel> where TModel : class, IModel
	{
		protected readonly AppDbContext _context;
		protected readonly DbSet<TModel> _dbSet;

		protected RepositoryBase(AppDbContext context)
		{
			_context = context;
			_dbSet = context.Set<TModel>();
		}

		public virtual async Task<TModel?> GetByIdAsync(Guid id)
		{
			return await _dbSet.FindAsync(id);
		}

		public virtual async Task<(IEnumerable<TModel> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
		{
			var totalCount = await _dbSet.CountAsync();
			var items = await _dbSet
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
			
			return (items, totalCount);
		}

		public virtual async Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public virtual async Task<TModel> AddAsync(TModel entity)
		{
			entity.Id = Guid.NewGuid();
			entity.CreatedAt = DateTime.UtcNow;
			entity.UpdatedAt = DateTime.UtcNow;
			
			await _dbSet.AddAsync(entity);
			return entity;
		}

		public virtual async Task<TModel> UpdateAsync(TModel entity)
		{
			entity.UpdatedAt = DateTime.UtcNow;
			
			_dbSet.Update(entity);
			return await Task.FromResult(entity);
		}

		public virtual async Task<bool> DeleteAsync(Guid id)
		{
			var entity = await GetByIdAsync(id);
			if (entity == null)
			{
				return false;
			}

			_dbSet.Remove(entity);
			return true;
		}

		public virtual async Task<bool> ExistsAsync(Guid id)
		{
			return await _dbSet.AnyAsync(e => e.Id == id);
		}

		public virtual async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
