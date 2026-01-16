using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intf
{
	public interface IEmpresaService : ICrudService<EmpresaDto>
	{
		Task<EmpresaDto?> GetByCnpjAsync(string cnpj);
		Task<IEnumerable<EmpresaDto>> GetByCepAsync(int cep);
		Task<IEnumerable<EmpresaDto>> GetByNameAsync(string name);
		Task<IEnumerable<EmpresaDto>> GetByCnpjContainsAnyAsync(string chars);
	}
}
