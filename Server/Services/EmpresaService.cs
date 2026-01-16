using Intf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Server.Services
{
	public class EmpresaService : CrudBaseService<EmpresaDto, EmpresaModel, IEmpresaRepository>, IEmpresaService
	{
		public EmpresaService(IEmpresaRepository empresaRepository) : base(empresaRepository)
		{
		}

		protected override EmpresaDto MapToDto(EmpresaModel model)
		{
			return EmpresaDto.FromModel(model);
		}

		protected override EmpresaModel MapToModel(EmpresaDto dto)
		{
			return EmpresaDto.ToModel(dto);
		}

		public async Task<EmpresaDto?> GetByCnpjAsync(string cnpj)
		{
			var empresa = await _repository.GetByCnpjAsync(cnpj);
			return empresa != null ? MapToDto(empresa) : null;
		}

		public async Task<IEnumerable<EmpresaDto>> GetByCepAsync(int cep)
		{
			var empresas = await _repository.GetByCepAsync(cep);
			return empresas.Select(MapToDto);
		}

		public async Task<IEnumerable<EmpresaDto>> GetByNameAsync(string name)
		{
			var empresas = await _repository.GetByNameAsync(name);
			return empresas.Select(MapToDto);
		}

		public async Task<IEnumerable<EmpresaDto>> GetByCnpjContainsAnyAsync(string chars)
		{
			var empresas = await _repository.GetByCnpjContainsAnyAsync(chars);
			return empresas.Select(MapToDto);
		}

		protected override async Task DoCreate(EmpresaDto dto, string createdBy)
		{
			var existingEmpresa = await _repository.GetByCnpjAsync(dto.Cnpj);
			if (existingEmpresa != null)
			{
				throw new InvalidOperationException($"Empresa com CNPJ {dto.Cnpj} já existe.");
			}
		}

		protected override async Task DoUpdate(Guid id, EmpresaDto dto, string updatedBy)
		{
			var existingEmpresa = await _repository.GetByCnpjAsync(dto.Cnpj);
			if (existingEmpresa != null && existingEmpresa.Id != id)
			{
				throw new InvalidOperationException($"Já existe outra empresa com CNPJ {dto.Cnpj}.");
			}
		}
	}
}
