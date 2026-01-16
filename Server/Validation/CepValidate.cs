namespace Server
{
	public class CepValidate
	{
		public static bool IsValidCep(string cep)
		{
			if(ValidateCep(cep))
			{
				return ViaCepValidate(cep).Result;
			}
			return false;
		}

		private static bool ValidateCep(string cep)
		{
			cep = new string(cep.Where(char.IsDigit).ToArray());
			if (cep.Length != 8)
			{
				return false;
			}
			foreach (char c in cep)
			{
				if (!char.IsDigit(c))
				{
					return false;
				}
			}
			return true;
		}

		private static async Task<bool> ViaCepValidate(string cep)
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri($"https://viacep.com.br/ws/{cep}/json/")
			};

			try
			{
				using (var response = await client.SendAsync(request))
				{
					if (!response.IsSuccessStatusCode)
					{
						return false;
					}
					var body = await response.Content.ReadAsStringAsync();
					if (body.Contains("\"erro\": true"))
					{
						return false;
					}
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

	}
}
