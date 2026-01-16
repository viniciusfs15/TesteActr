using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intf
{
	public interface IEmpresaFornecedorRepository : IRepository<EmpresaFornecedorModel>
	{
		Task<IEnumerable<EmpresaFornecedorModel>> GetByEmpresaIdAsync(Guid empresaId);
		Task<IEnumerable<EmpresaFornecedorModel>> GetByFornecedorIdAsync(Guid fornecedorId);
		Task<EmpresaFornecedorModel?> GetByEmpresaAndFornecedorAsync(Guid empresaId, Guid fornecedorId);
	}
}
