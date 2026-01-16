using Intf;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Route("api/[controller]")]
	public class FornecedorController : CrudBaseController<FornecedorDto, IFornecedorService>
	{
		public FornecedorController(IFornecedorService fornecedorService) : base(fornecedorService)
		{
		}

		[HttpGet("cnpjcpf/{cnpjOrCpf}")]
		public async Task<ActionResult<FornecedorDto>> GetByCnpjOrCpf(string cnpjOrCpf)
		{
			var fornecedor = await _service.GetByCnpjOrCpfAsync(cnpjOrCpf);
			if (fornecedor == null)
			{
				return NotFound();
			}
			return Ok(fornecedor);
		}
	}
}
