using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intf
{
	public class FornecedorModel : IModel
	{
		public Guid Id { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string? CreatedBy { get; set; }
		public string? UpdatedBy { get; set; }

		public string? Cnpj { get; set; }
		public string? Cpf { get; set; }
		public string? Rg { get; set; }
		public FornecedorTypeEnum Type { get; set; }
		public DateTime? BirthDate { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Cep { get; set; }
		public string Email { get; set; } = string.Empty;

		public ICollection<EmpresaFornecedorModel> EmpresaFornecedores { get; set; } = new List<EmpresaFornecedorModel>();
	}
}
