using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intf
{
	public interface IEmpresaFornecedorService : ICrudService<EmpresaFornecedorDto>
	{
		Task<IEnumerable<EmpresaFornecedorDto>> GetByEmpresaIdAsync(Guid empresaId);
		Task<IEnumerable<EmpresaFornecedorDto>> GetByFornecedorIdAsync(Guid fornecedorId);
		Task<EmpresaFornecedorDto?> GetByEmpresaAndFornecedorAsync(Guid empresaId, Guid fornecedorId);
		Task<EmpresaComFornecedoresDto?> GetEmpresaComFornecedoresAsync(Guid empresaId);
		Task<FornecedorComEmpresasDto?> GetFornecedorComEmpresasAsync(Guid fornecedorId);
	}
}
