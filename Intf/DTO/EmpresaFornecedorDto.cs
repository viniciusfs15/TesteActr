using System;

namespace Intf
{
	public class EmpresaFornecedorDto : IDto
	{
		public Guid Id { get; set; }
		public Guid EmpresaId { get; set; }
		public Guid FornecedorId { get; set; }

		public static EmpresaFornecedorModel ToModel(EmpresaFornecedorDto dto)
		{
			return new EmpresaFornecedorModel
			{
				Id = dto.Id,
				EmpresaId = dto.EmpresaId,
				FornecedorId = dto.FornecedorId
			};
		}

		public static EmpresaFornecedorDto FromModel(EmpresaFornecedorModel model)
		{
			return new EmpresaFornecedorDto
			{
				Id = model.Id,
				EmpresaId = model.EmpresaId,
				FornecedorId = model.FornecedorId
			};
		}
	}
}
