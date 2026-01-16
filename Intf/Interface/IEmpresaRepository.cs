using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intf
{
	public interface IEmpresaRepository : IRepository<EmpresaModel>
	{
		Task<EmpresaModel?> GetByCnpjAsync(string cnpj);
		Task<IEnumerable<EmpresaModel>> GetByCepAsync(int cep);
		Task<IEnumerable<EmpresaModel>> GetByNameAsync(string name);
		Task<IEnumerable<EmpresaModel>> GetByCnpjContainsAnyAsync(string chars);
	}
}
