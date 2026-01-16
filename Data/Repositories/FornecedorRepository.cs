using Intf;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
	public class FornecedorRepository : RepositoryBase<FornecedorModel>, IFornecedorRepository
	{
		public FornecedorRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<FornecedorModel?> GetByCnpjOrCpfAsync(string cnpjOrCpf)
		{
			return await _dbSet.FirstOrDefaultAsync(f => f.Cnpj == cnpjOrCpf || f.Cpf == cnpjOrCpf);
		}

		public async Task<FornecedorModel?> GetByEmailAsync(string email)
		{
			return await _dbSet.FirstOrDefaultAsync(f => f.Email == email);
		}

		public async Task<IEnumerable<FornecedorModel>> GetByCepAsync(int cep)
		{
			return await _dbSet.Where(f => f.Cep == cep).ToListAsync();
		}

		public async Task<IEnumerable<FornecedorModel>> GetByNameAsync(string name)
		{
			return await _dbSet.Where(f => f.Name.Contains(name)).ToListAsync();
		}

		public async Task<IEnumerable<FornecedorModel>> GetByDocumentoAsync(string documento)
		{
			return await _dbSet.Where(f => f.Cnpj == documento || f.Cpf == documento).ToListAsync();
		}

		public async Task<IEnumerable<FornecedorModel>> GetByTypeAsync(FornecedorTypeEnum type)
		{
			return await _dbSet.Where(f => f.Type == type).ToListAsync();
		}
	}
}
