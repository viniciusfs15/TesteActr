using Intf;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;

namespace Data.Repositories
{
	public class EmpresaRepository : RepositoryBase<EmpresaModel>, IEmpresaRepository
	{
		public EmpresaRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<EmpresaModel?> GetByCnpjAsync(string cnpj)
		{
			return await _dbSet.FirstOrDefaultAsync(e => e.Cnpj == cnpj);
		}

		public async Task<IEnumerable<EmpresaModel>> GetByCepAsync(int cep)
		{
			return await _dbSet.Where(e => e.Cep == cep).ToListAsync();
		}

		public async Task<IEnumerable<EmpresaModel>> GetByNameAsync(string name)
		{
			return await _dbSet.Where(e => e.Name.Contains(name)).ToListAsync();
		}

		public async Task<IEnumerable<EmpresaModel>> GetByCnpjContainsAnyAsync(string chars)
		{
			if (string.IsNullOrEmpty(chars))
			{
				return new List<EmpresaModel>();
			}
			return await _dbSet.Where(e => e.Cnpj.Contains(chars)).ToListAsync();
		}
	}
}
