using Intf;
using Intf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
	public abstract class CrudBaseService<TDto, TModel, TRepository> : ICrudService<TDto>
		where TDto : IDto
		where TModel : class, IModel
		where TRepository : IRepository<TModel>
	{
		protected readonly TRepository _repository;

		protected CrudBaseService(TRepository repository)
		{
			_repository = repository;
		}

		protected abstract TDto MapToDto(TModel model);
		protected abstract TModel MapToModel(TDto dto);

		public virtual async Task<TDto?> GetByIdAsync(Guid id)
		{
			await DoGetById(id);
			
			var model = await _repository.GetByIdAsync(id);
			return model != null ? MapToDto(model) : default;
		}

		public virtual async Task<PagedResult<TDto>> GetPagedAsync(int page, int pageSize)
		{
			await DoGetPaged(page, pageSize);
			
			var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize);
			var dtos = items.Select(MapToDto).ToList();
			
			var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
			var hasNext = page < totalPages;
			
			return new PagedResult<TDto>
			{
				Items = dtos,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount,
				HasNext = hasNext
			};
		}

		public virtual async Task<TDto> CreateAsync(TDto dto, string createdBy)
		{
			await DoCreate(dto, createdBy);
			
			var model = MapToModel(dto);
			model.CreatedBy = createdBy;
			model.UpdatedBy = createdBy;
			model.CreatedAt = DateTime.UtcNow;
			model.UpdatedAt = DateTime.UtcNow;

			var created = await _repository.AddAsync(model);
			await _repository.SaveChangesAsync();

			return MapToDto(created);
		}

		public virtual async Task<TDto> UpdateAsync(Guid id, TDto dto, string updatedBy)
		{
			await DoUpdate(id, dto, updatedBy);
			
			var existingModel = await _repository.GetByIdAsync(id);
			if (existingModel == null)
			{
				throw new KeyNotFoundException($"Registro com ID {id} não encontrado.");
			}

			var model = MapToModel(dto);
			model.CreatedAt = existingModel.CreatedAt;
			model.CreatedBy = existingModel.CreatedBy;
			model.UpdatedBy = updatedBy;
			model.UpdatedAt = DateTime.UtcNow;

			var updated = await _repository.UpdateAsync(model);
			await _repository.SaveChangesAsync();

			return MapToDto(updated);
		}

		public virtual async Task<bool> DeleteAsync(Guid id)
		{
			await DoDelete(id);
			
			var deleted = await _repository.DeleteAsync(id);
			if (deleted)
			{
				await _repository.SaveChangesAsync();
			}
			return deleted;
		}

		public virtual async Task<bool> ExistsAsync(Guid id)
		{
			await DoExists(id);
			
			return await _repository.ExistsAsync(id);
		}

		// Métodos virtuais para validação e customização
		protected virtual Task DoGetById(Guid id)
		{
			return Task.CompletedTask;
		}

		protected virtual Task DoGetPaged(int page, int pageSize)
		{
			return Task.CompletedTask;
		}

		protected virtual Task DoCreate(TDto dto, string createdBy)
		{
			return Task.CompletedTask;
		}

		protected virtual Task DoUpdate(Guid id, TDto dto, string updatedBy)
		{
			return Task.CompletedTask;
		}

		protected virtual Task DoDelete(Guid id)
		{
			return Task.CompletedTask;
		}

		protected virtual Task DoExists(Guid id)
		{
			return Task.CompletedTask;
		}
	}
}
