using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intf
{
	public interface IFornecedorRepository : IRepository<FornecedorModel>
	{
		Task<FornecedorModel?> GetByCnpjOrCpfAsync(string cnpjOrCpf);
		Task<FornecedorModel?> GetByEmailAsync(string email);
		Task<IEnumerable<FornecedorModel>> GetByCepAsync(int cep);
		Task<IEnumerable<FornecedorModel>> GetByNameAsync(string name);
		Task<IEnumerable<FornecedorModel>> GetByDocumentoAsync(string documento);
		Task<IEnumerable<FornecedorModel>> GetByTypeAsync(FornecedorTypeEnum type);
	}
}
