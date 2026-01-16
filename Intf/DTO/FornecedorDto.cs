using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intf
{
	public class FornecedorDto : IDto
	{
		public Guid Id { get; set; }
		public string? Cnpj { get; set; }
		public string? Cpf { get; set; }
		public string? Rg { get; set; }
		public FornecedorTypeEnum Type { get; set; }
		public DateTime? BirthDate { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Cep { get; set; }
		public string Email { get; set; } = string.Empty;

		public static FornecedorModel ToModel(FornecedorDto dto)
		{
			return new FornecedorModel
			{
				Id = dto.Id,
				Cnpj = dto.Cnpj,
				Cpf = dto.Cpf,
				Rg = dto.Rg,
				Type = dto.Type,
				BirthDate = dto.BirthDate,
				Name = dto.Name,
				Cep = dto.Cep,
				Email = dto.Email
			};
		}

		public static FornecedorDto FromModel(FornecedorModel model)
		{
			return new FornecedorDto
			{
				Id = model.Id,
				Cnpj = model.Cnpj,
				Cpf = model.Cpf,
				Rg = model.Rg,
				Type = model.Type,
				BirthDate = model.BirthDate,
				Name = model.Name,
				Cep = model.Cep,
				Email = model.Email
			};
		}
	}
}
