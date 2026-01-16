using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class CpfValidate
	{
		public static bool IsValidCpf(string cpf)
		{
			cpf = new string(cpf.Where(char.IsDigit).ToArray());
			if (cpf.Length != 11)
			{
				return false;
			}
			// Verifica se todos os dígitos são iguais
			if (new string(cpf[0], cpf.Length) == cpf)
			{
				return false;
			}
			// Calcula o primeiro dígito verificador
			int sum = 0;
			for (int i = 0; i < 9; i++)
			{
				sum += (cpf[i] - '0') * (10 - i);
			}
			int firstCheckDigit = sum % 11;
			firstCheckDigit = firstCheckDigit < 2 ? 0 : 11 - firstCheckDigit;
			// Calcula o segundo dígito verificador
			sum = 0;
			for (int i = 0; i < 10; i++)
			{
				sum += (cpf[i] - '0') * (11 - i);
			}
			int secondCheckDigit = sum % 11;
			secondCheckDigit = secondCheckDigit < 2 ? 0 : 11 - secondCheckDigit;
			// Verifica se os dígitos calculados são iguais aos dígitos fornecidos
			return firstCheckDigit == (cpf[9] - '0') && secondCheckDigit == (cpf[10] - '0');
		}
	}
}
