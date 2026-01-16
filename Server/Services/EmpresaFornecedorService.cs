using Intf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
	public class EmpresaFornecedorService : CrudBaseService<EmpresaFornecedorDto, EmpresaFornecedorModel, IEmpresaFornecedorRepository>, IEmpresaFornecedorService
	{
		private readonly IEmpresaRepository _empresaRepository;
		private readonly IFornecedorRepository _fornecedorRepository;

		public EmpresaFornecedorService(
			IEmpresaFornecedorRepository repository,
			IEmpresaRepository empresaRepository,
			IFornecedorRepository fornecedorRepository) : base(repository)
		{
			_empresaRepository = empresaRepository;
			_fornecedorRepository = fornecedorRepository;
		}

		protected override EmpresaFornecedorDto MapToDto(EmpresaFornecedorModel model)
		{
			return EmpresaFornecedorDto.FromModel(model);
		}

		protected override EmpresaFornecedorModel MapToModel(EmpresaFornecedorDto dto)
		{
			return EmpresaFornecedorDto.ToModel(dto);
		}

		public async Task<IEnumerable<EmpresaFornecedorDto>> GetByEmpresaIdAsync(Guid empresaId)
		{
			var items = await _repository.GetByEmpresaIdAsync(empresaId);
			return items.Select(MapToDto);
		}

		public async Task<IEnumerable<EmpresaFornecedorDto>> GetByFornecedorIdAsync(Guid fornecedorId)
		{
			var items = await _repository.GetByFornecedorIdAsync(fornecedorId);
			return items.Select(MapToDto);
		}

		public async Task<EmpresaFornecedorDto?> GetByEmpresaAndFornecedorAsync(Guid empresaId, Guid fornecedorId)
		{
			var item = await _repository.GetByEmpresaAndFornecedorAsync(empresaId, fornecedorId);
			return item != null ? MapToDto(item) : null;
		}

		public async Task<EmpresaComFornecedoresDto?> GetEmpresaComFornecedoresAsync(Guid empresaId)
		{
			var empresa = await _empresaRepository.GetByIdAsync(empresaId);
			if (empresa == null)
			{
				return null;
			}

			var relacionamentos = await _repository.GetByEmpresaIdAsync(empresaId);
			var fornecedorIds = relacionamentos.Select(r => r.FornecedorId).ToList();

			var fornecedores = new List<FornecedorDto>();
			foreach (var fornecedorId in fornecedorIds)
			{
				var fornecedor = await _fornecedorRepository.GetByIdAsync(fornecedorId);
				if (fornecedor != null)
				{
					fornecedores.Add(FornecedorDto.FromModel(fornecedor));
				}
			}

			return new EmpresaComFornecedoresDto
			{
				Empresa = EmpresaDto.FromModel(empresa),
				Fornecedores = fornecedores
			};
		}

		public async Task<FornecedorComEmpresasDto?> GetFornecedorComEmpresasAsync(Guid fornecedorId)
		{
			var fornecedor = await _fornecedorRepository.GetByIdAsync(fornecedorId);
			if (fornecedor == null)
			{
				return null;
			}

			var relacionamentos = await _repository.GetByFornecedorIdAsync(fornecedorId);
			var empresaIds = relacionamentos.Select(r => r.EmpresaId).ToList();

			var empresas = new List<EmpresaDto>();
			foreach (var empresaId in empresaIds)
			{
				var empresa = await _empresaRepository.GetByIdAsync(empresaId);
				if (empresa != null)
				{
					empresas.Add(EmpresaDto.FromModel(empresa));
				}
			}

			return new FornecedorComEmpresasDto
			{
				Fornecedor = FornecedorDto.FromModel(fornecedor),
				Empresas = empresas
			};
		}

		protected override async Task DoCreate(EmpresaFornecedorDto dto, string createdBy)
		{
			var fornecedor = new FornecedorModel();
			var empresa = new EmpresaModel();

			(empresa, fornecedor) = await ExistingRelationCheck(dto);

			// Empresas com CEP do Paranara não podem ter fornecedor pessoa fisica ou menor de idade
			if (empresa.Cep >= 80000000 && empresa.Cep <= 87999999)
			{
				if (fornecedor.Type == FornecedorTypeEnum.PessoaFisica)
				{
					throw new ValidationException("Empresas do Paraná não podem ter fornecedores pessoa física.");
				}
				if (fornecedor.BirthDate != null)
				{
					var idade = DateTime.Today.Year - fornecedor.BirthDate.Value.Year;
					if (fornecedor.BirthDate.Value.Date > DateTime.Today.AddYears(-idade)) idade--;
					if (idade < 18)
					{
						throw new ValidationException("Empresas do Paraná não podem ter fornecedores menores de idade.");
					}
				}
			}
		}

		private async Task<(EmpresaModel empresa, FornecedorModel fornecedor)> ExistingRelationCheck(EmpresaFornecedorDto dto)
		{
			var empresa = await _empresaRepository.GetByIdAsync(dto.EmpresaId);
			// Validar se a empresa existe
			if (empresa == null)
			{
				throw new KeyNotFoundException($"Empresa com ID {dto.EmpresaId} não encontrada.");
			}
			
			var fornecedor = await _fornecedorRepository.GetByIdAsync(dto.FornecedorId);
			// Validar se o fornecedor existe
			if (fornecedor == null)
			{
				throw new KeyNotFoundException($"Fornecedor com ID {dto.FornecedorId} não encontrado.");
			}

			// Validar se o relacionamento já existe
			var existingRelation = await _repository.GetByEmpresaAndFornecedorAsync(dto.EmpresaId, dto.FornecedorId);
			if (existingRelation != null)
			{
				throw new InvalidOperationException($"Relacionamento entre Empresa {dto.EmpresaId} e Fornecedor {dto.FornecedorId} já existe.");
			}
			return (empresa, fornecedor);
		}

		protected override async Task DoUpdate(Guid id, EmpresaFornecedorDto dto, string updatedBy)
		{
			ExistingRelationCheck(dto).Wait();
		}
	}
}
