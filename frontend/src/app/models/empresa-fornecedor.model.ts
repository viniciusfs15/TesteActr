import { Empresa } from './empresa.model';
import { Fornecedor } from './fornecedor.model';

export interface EmpresaComFornecedores {
  empresa: Empresa;
  fornecedores: Fornecedor[];
}

export interface FornecedorComEmpresas {
  fornecedor: Fornecedor;
  empresas: Empresa[];
}

export interface EmpresaFornecedor {
  id: string;
  empresaId: string;
  fornecedorId: string;
}
