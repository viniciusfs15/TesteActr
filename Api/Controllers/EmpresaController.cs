using Intf;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
	[Route("api/[controller]")]
	public class EmpresaController : CrudBaseController<EmpresaDto, IEmpresaService>
	{
		public EmpresaController(IEmpresaService empresaService) : base(empresaService)
		{
		}

		[HttpGet("cnpj/{cnpj}")]
		public async Task<ActionResult<EmpresaDto>> GetByCnpj(string cnpj)
		{
			var empresa = await _service.GetByCnpjAsync(cnpj);
			if (empresa == null)
			{
				return NotFound();
			}
			return Ok(empresa);
		}

		[HttpGet("cnpj/contains/{chars}")]
		public async Task<ActionResult<IEnumerable<EmpresaDto>>> GetByCnpjContainsAny(string chars)
		{
			var empresas = await _service.GetByCnpjContainsAnyAsync(chars);
			return Ok(empresas);
		}

		[HttpGet("name/{name}")]
		public async Task<ActionResult<IEnumerable<EmpresaDto>>> GetByName(string name)
		{
			var empresas = await _service.GetByNameAsync(name);
			return Ok(empresas);
		}

		[HttpGet("cep/{cep}")]
		public async Task<ActionResult<IEnumerable<EmpresaDto>>> GetByCep(int cep)
		{
			var empresas = await _service.GetByCepAsync(cep);
			return Ok(empresas);
		}
	}
}
