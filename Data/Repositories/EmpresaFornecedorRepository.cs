using Intf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
	public class EmpresaFornecedorRepository : RepositoryBase<EmpresaFornecedorModel>, IEmpresaFornecedorRepository
	{
		public EmpresaFornecedorRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<EmpresaFornecedorModel>> GetByEmpresaIdAsync(Guid empresaId)
		{
			return await _dbSet
				.Include(ef => ef.Fornecedor)
				.Include(ef => ef.Empresa)
				.Where(ef => ef.EmpresaId == empresaId)
				.ToListAsync();
		}

		public async Task<IEnumerable<EmpresaFornecedorModel>> GetByFornecedorIdAsync(Guid fornecedorId)
		{
			return await _dbSet
				.Include(ef => ef.Empresa)
				.Include(ef => ef.Fornecedor)
				.Where(ef => ef.FornecedorId == fornecedorId)
				.ToListAsync();
		}

		public async Task<EmpresaFornecedorModel?> GetByEmpresaAndFornecedorAsync(Guid empresaId, Guid fornecedorId)
		{
			return await _dbSet
				.Include(ef => ef.Empresa)
				.Include(ef => ef.Fornecedor)
				.FirstOrDefaultAsync(ef => ef.EmpresaId == empresaId && ef.FornecedorId == fornecedorId);
		}
	}
}
