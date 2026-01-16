using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intf
{
	public interface IFornecedorService : ICrudService<FornecedorDto>
	{
		Task<FornecedorDto?> GetByCnpjOrCpfAsync(string cnpjOrCpf);
		Task<FornecedorDto?> GetByEmailAsync(string email);
		Task<IEnumerable<FornecedorDto>> GetByCepAsync(int cep);
		Task<IEnumerable<FornecedorDto>> GetByNameAsync(string name);
		Task<IEnumerable<FornecedorDto>> GetByDocumentoAsync(string documento);
		Task<IEnumerable<FornecedorDto>> GetByTypeAsync(FornecedorTypeEnum type);
	}
}
