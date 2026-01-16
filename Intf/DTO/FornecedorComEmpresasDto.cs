using System;
using System.Collections.Generic;

namespace Intf
{
	public class FornecedorComEmpresasDto
	{
		public FornecedorDto Fornecedor { get; set; } = null!;
		public List<EmpresaDto> Empresas { get; set; } = new List<EmpresaDto>();
	}
}
