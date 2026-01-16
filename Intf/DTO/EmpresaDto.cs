using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intf
{
	public class EmpresaDto: IDto
	{
		public Guid Id { get; set; }
		public string Cnpj { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public int Cep { get; set; }

		public static EmpresaModel ToModel(EmpresaDto dto)
		{
			return new EmpresaModel
			{
				Id = dto.Id,
				Cnpj = dto.Cnpj,
				Name = dto.Name,
				Cep = dto.Cep
			};
		}

		public static EmpresaDto FromModel(EmpresaModel model)
		{
			return new EmpresaDto
			{
				Id = model.Id,
				Cnpj = model.Cnpj,
				Name = model.Name,
				Cep = model.Cep
			};
		}
	}
}
