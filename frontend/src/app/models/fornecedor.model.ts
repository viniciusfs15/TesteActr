export enum FornecedorType {
  PessoaFisica = 0,
  PessoaJuridica = 1
}

export interface Fornecedor {
  id: string;
  cnpj?: string | null;
  cpf?: string | null;
  rg?: string | null;
  type: FornecedorType;
  birthDate?: string | null;
  name: string;
  cep: number;
  email: string;
}
