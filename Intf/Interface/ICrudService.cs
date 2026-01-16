using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intf.Models;

namespace Intf
{
	public interface ICrudService<TDto> where TDto : IDto
	{
		Task<TDto?> GetByIdAsync(Guid id);
		Task<PagedResult<TDto>> GetPagedAsync(int page, int pageSize);
		Task<TDto> CreateAsync(TDto dto, string createdBy);
		Task<TDto> UpdateAsync(Guid id, TDto dto, string updatedBy);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
	}
}
