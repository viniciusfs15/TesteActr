using Intf;
using Intf.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	public abstract class CrudBaseController<TDto, TService> : ControllerBase 
		where TDto : IDto
		where TService : ICrudService<TDto>
	{
		protected readonly TService _service;

		protected CrudBaseController(TService service)
		{
			_service = service;
		}

		[HttpGet]
		public virtual async Task<ActionResult<PagedResult<TDto>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			// TODO: Buscar valor máximo de pageSize a partir de configuração
			var maxSize = 100;
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize > maxSize ? maxSize : pageSize;

			var result = await _service.GetPagedAsync(page, pageSize);
			return Ok(result);
		}

		[HttpGet("{id}")]
		public virtual async Task<ActionResult<TDto>> GetById(Guid id)
		{
			var item = await _service.GetByIdAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return Ok(item);
		}

		[HttpPost]
		public virtual async Task<ActionResult<TDto>> Create([FromBody] TDto dto)
		{
			var created = await _service.CreateAsync(dto, "Sistema");
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}

		[HttpPut("{id}")]
		public virtual async Task<ActionResult<TDto>> Update(Guid id, [FromBody] TDto dto)
		{
			try
			{
				var updated = await _service.UpdateAsync(id, dto, "Sistema");
				return Ok(updated);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpDelete("{id}")]
		public virtual async Task<ActionResult> Delete(Guid id)
		{
			var deleted = await _service.DeleteAsync(id);
			if (!deleted)
			{
				return NotFound();
			}
			return NoContent();
		}
	}
}
