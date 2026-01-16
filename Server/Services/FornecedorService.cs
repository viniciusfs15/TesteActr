using Intf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
	public class FornecedorService : CrudBaseService<FornecedorDto, FornecedorModel, IFornecedorRepository>, IFornecedorService
	{
		public FornecedorService(IFornecedorRepository fornecedorRepository) : base(fornecedorRepository)
		{
		}

		protected override FornecedorDto MapToDto(FornecedorModel model)
		{
			return FornecedorDto.FromModel(model);
		}

		protected override FornecedorModel MapToModel(FornecedorDto dto)
		{
			return FornecedorDto.ToModel(dto);
		}

		public async Task<FornecedorDto?> GetByCnpjOrCpfAsync(string cnpjOrCpf)
		{
			var fornecedor = await _repository.GetByCnpjOrCpfAsync(cnpjOrCpf);
			return fornecedor != null ? MapToDto(fornecedor) : null;
		}

		public async Task<FornecedorDto?> GetByEmailAsync(string email)
		{
			var fornecedor = await _repository.GetByEmailAsync(email);
			return fornecedor != null ? MapToDto(fornecedor) : null;
		}

		public async Task<IEnumerable<FornecedorDto>> GetByCepAsync(int cep)
		{
			var fornecedores = await _repository.GetByCepAsync(cep);
			return fornecedores.Select(MapToDto);
		}

		public async Task<IEnumerable<FornecedorDto>> GetByNameAsync(string name)
		{
			var fornecedores = await _repository.GetByNameAsync(name);
			return fornecedores.Select(MapToDto);
		}

		public async Task<IEnumerable<FornecedorDto>> GetByDocumentoAsync(string documento)
		{
			var fornecedores = await _repository.GetByDocumentoAsync(documento);
			return fornecedores.Select(MapToDto);
		}

		public async Task<IEnumerable<FornecedorDto>> GetByTypeAsync(FornecedorTypeEnum type)
		{
			var fornecedores = await _repository.GetByTypeAsync(type);
			return fornecedores.Select(MapToDto);
		}

		protected override async Task DoCreate(FornecedorDto dto, string createdBy)
		{
			// Validar duplicidade de CNPJ/CPF
			if (!string.IsNullOrEmpty(dto.Cnpj) || !string.IsNullOrEmpty(dto.Cpf))
			{
				var cnpjOrCpf = !string.IsNullOrEmpty(dto.Cnpj) ? dto.Cnpj : dto.Cpf;
				var existingFornecedor = await _repository.GetByCnpjOrCpfAsync(cnpjOrCpf!);
				if (existingFornecedor != null)
				{
					throw new InvalidOperationException($"Fornecedor com CNPJ/CPF {cnpjOrCpf} já existe.");
				}
			}

			// Validar duplicidade de Email
			if (!string.IsNullOrEmpty(dto.Email))
			{
				var existingFornecedor = await _repository.GetByEmailAsync(dto.Email);
				if (existingFornecedor != null)
				{
					throw new InvalidOperationException($"Fornecedor com Email {dto.Email} já existe.");
				}
			}

			if(dto.Type == FornecedorTypeEnum.PessoaFisica && (string.IsNullOrWhiteSpace(dto.Cpf) || string.IsNullOrWhiteSpace(dto.Rg)))
			{
				throw new InvalidOperationException("Fornecedor do tipo Pessoa Física deve ter CPF e RG preenchidos.");
			}
		}

		protected override async Task DoUpdate(Guid id, FornecedorDto dto, string updatedBy)
		{
			// Validar duplicidade de CNPJ/CPF
			if (!string.IsNullOrEmpty(dto.Cnpj) || !string.IsNullOrEmpty(dto.Cpf))
			{
				var cnpjOrCpf = !string.IsNullOrEmpty(dto.Cnpj) ? dto.Cnpj : dto.Cpf;
				var existingFornecedor = await _repository.GetByCnpjOrCpfAsync(cnpjOrCpf!);
				if (existingFornecedor != null && existingFornecedor.Id != id)
				{
					throw new InvalidOperationException($"Já existe outro fornecedor com CNPJ/CPF {cnpjOrCpf}.");
				}
			}

			// Validar duplicidade de Email
			if (!string.IsNullOrEmpty(dto.Email))
			{
				var existingFornecedor = await _repository.GetByEmailAsync(dto.Email);
				if (existingFornecedor != null && existingFornecedor.Id != id)
				{
					throw new InvalidOperationException($"Já existe outro fornecedor com Email {dto.Email}.");
				}
			}
		}
	}
}
