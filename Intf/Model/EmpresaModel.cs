using System;
using System.Collections.Generic;

namespace Intf
{
	public class EmpresaModel : IModel
	{
		public Guid Id { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string? CreatedBy { get; set; }
		public string? UpdatedBy { get; set; }

		public string Cnpj { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public int Cep { get; set; }

		public ICollection<EmpresaFornecedorModel> EmpresaFornecedores { get; set; } = new List<EmpresaFornecedorModel>();
	}
}
