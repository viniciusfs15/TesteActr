using System;
using System.Collections.Generic;

namespace Intf
{
	public class EmpresaComFornecedoresDto
	{
		public EmpresaDto Empresa { get; set; } = null!;
		public List<FornecedorDto> Fornecedores { get; set; } = new List<FornecedorDto>();
	}
}
