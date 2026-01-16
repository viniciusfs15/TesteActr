using System;

namespace Intf
{
	public class EmpresaFornecedorModel : IModel
	{
		public Guid Id { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string? CreatedBy { get; set; }
		public string? UpdatedBy { get; set; }

		public Guid EmpresaId { get; set; }
		public Guid FornecedorId { get; set; }

		public EmpresaModel Empresa { get; set; } = null!;
		public FornecedorModel Fornecedor { get; set; } = null!;
	}
}
