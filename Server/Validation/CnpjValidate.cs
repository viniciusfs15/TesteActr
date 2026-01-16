using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class CnpjValidate
	{
		public static bool IsValidCnpj(string cnpj)
		{
			cnpj = new string(cnpj.Where(char.IsDigit).ToArray());
			if (cnpj.Length != 14)
			{
				return false;
			}
			// Verifica se todos os dígitos são iguais
			if (new string(cnpj[0], cnpj.Length) == cnpj)
			{
				return false;
			}
			int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
			int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
			// Calcula o primeiro dígito verificador
			int sum = 0;
			for (int i = 0; i < 12; i++)
			{
				sum += (cnpj[i] - '0') * multiplicadores1[i];
			}
			int firstCheckDigit = sum % 11;
			firstCheckDigit = firstCheckDigit < 2 ? 0 : 11 - firstCheckDigit;
			// Calcula o segundo dígito verificador
			sum = 0;
			for (int i = 0; i < 13; i++)
			{
				sum += (cnpj[i] - '0') * multiplicadores2[i];
			}
			int secondCheckDigit = sum % 11;
			secondCheckDigit = secondCheckDigit < 2 ? 0 : 11 - secondCheckDigit;
			// Verifica se os dígitos calculados são iguais aos dígitos fornecidos
			return firstCheckDigit == (cnpj[12] - '0') && secondCheckDigit == (cnpj[13] - '0');
		}
	}
}
