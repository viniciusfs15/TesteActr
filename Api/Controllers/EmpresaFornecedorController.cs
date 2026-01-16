using Intf;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Route("api/[controller]")]
	public class EmpresaFornecedorController : CrudBaseController<EmpresaFornecedorDto, IEmpresaFornecedorService>
	{
		public EmpresaFornecedorController(IEmpresaFornecedorService service) : base(service)
		{
		}

		[HttpGet("empresa/{empresaId}")]
		public async Task<ActionResult<EmpresaComFornecedoresDto>> GetByEmpresaId(Guid empresaId)
		{
			var result = await _service.GetEmpresaComFornecedoresAsync(empresaId);
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}

		[HttpGet("fornecedor/{fornecedorId}")]
		public async Task<ActionResult<FornecedorComEmpresasDto>> GetByFornecedorId(Guid fornecedorId)
		{
			var result = await _service.GetFornecedorComEmpresasAsync(fornecedorId);
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}

		[HttpGet("empresa/{empresaId}/fornecedor/{fornecedorId}")]
		public async Task<ActionResult<EmpresaFornecedorDto>> GetByEmpresaAndFornecedor(Guid empresaId, Guid fornecedorId)
		{
			var item = await _service.GetByEmpresaAndFornecedorAsync(empresaId, fornecedorId);
			if (item == null)
			{
				return NotFound();
			}
			return Ok(item);
		}
	}
}
