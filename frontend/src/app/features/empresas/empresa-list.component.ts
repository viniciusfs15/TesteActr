import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EmpresaService } from '../../services/empresa.service';
import { FornecedorService } from '../../services/fornecedor.service';
import { EmpresaFornecedorService } from '../../services/empresa-fornecedor.service';
import { CepValidatorService } from '../../services/cep-validator.service';
import { Empresa } from '../../models/empresa.model';
import { Fornecedor } from '../../models/fornecedor.model';
import { NotificationService } from '../../core/services/notification.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-empresa-list',
  templateUrl: './empresa-list.component.html',
  styleUrls: ['./empresa-list.component.css']
})
export class EmpresaListComponent implements OnInit {
  empresas: Empresa[] = [];
  selectedEmpresas: Set<string> = new Set();
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  hasNext = false;
  isLoading = false;

  // Filtros por coluna
  filters = {
    cnpj: '',
    name: '',
    cep: ''
  };

  // Modal
  showModal = false;
  modalTitle = '';
  empresaForm: FormGroup;
  isEditMode = false;
  currentEmpresaId: string | null = null;

  // Associações de fornecedores
  fornecedoresDisponiveis: Fornecedor[] = [];
  fornecedoresAssociados: Fornecedor[] = [];
  fornecedoresAssociadosOriginais: Fornecedor[] = [];

  // Diálogo de confirmação
  showConfirmDialog = false;
  confirmMessage = '';
  empresasToDelete: string[] = [];

  constructor(
    private empresaService: EmpresaService,
    private fornecedorService: FornecedorService,
    private empresaFornecedorService: EmpresaFornecedorService,
    private cepValidatorService: CepValidatorService,
    private notificationService: NotificationService,
    private fb: FormBuilder
  ) {
    this.empresaForm = this.fb.group({
      cnpj: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]],
      name: ['', Validators.required],
      cep: ['', [Validators.required, Validators.pattern(/^\d{8}$/)]]
    });
  }

  ngOnInit(): void {
    this.loadEmpresas();
  }

  loadEmpresas(): void {
    this.isLoading = true;

    if (this.filters.cnpj || this.filters.name || this.filters.cep) {
      this.applyFilters();
    } else {
      this.empresaService.getPaged(this.currentPage, this.pageSize).subscribe({
        next: (result) => {
          this.empresas = result.items;
          this.totalCount = result.totalCount;
          this.hasNext = result.hasNext;
          this.isLoading = false;
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage = this.extractErrorMessage(error, 'Erro ao carregar empresas');
          this.notificationService.error(errorMessage);
        }
      });
    }
  }

  applyFilters(): void {
    this.isLoading = true;
    const observables = [];

    if (this.filters.cnpj) {
      observables.push(this.empresaService.getByCnpjContains(this.filters.cnpj));
    }
    if (this.filters.name) {
      observables.push(this.empresaService.getByName(this.filters.name));
    }
    if (this.filters.cep) {
      observables.push(this.empresaService.getByCep(this.filters.cep));
    }

    if (observables.length === 0) {
      this.loadEmpresas();
      return;
    }

    forkJoin(observables).subscribe({
      next: (results: any[]) => {
        const empresasMap = new Map<string, Empresa>();
        results.forEach(result => {
          if (Array.isArray(result)) {
            result.forEach(emp => empresasMap.set(emp.id, emp));
          } else if (result && result.id) {
            empresasMap.set(result.id, result);
          }
        });
        this.empresas = Array.from(empresasMap.values());
        this.totalCount = this.empresas.length;
        this.hasNext = false;
        this.isLoading = false;
      },
      error: () => {
        // Não exibe mensagem de erro, apenas limpa os resultados
        this.empresas = [];
        this.totalCount = 0;
        this.isLoading = false;
      }
    });
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadEmpresas();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadEmpresas();
  }

  toggleSelection(id: string): void {
    if (this.selectedEmpresas.has(id)) {
      this.selectedEmpresas.delete(id);
    } else {
      this.selectedEmpresas.add(id);
    }
  }

  toggleSelectAll(event: any): void {
    if (event.target.checked) {
      this.empresas.forEach(e => this.selectedEmpresas.add(e.id));
    } else {
      this.selectedEmpresas.clear();
    }
  }

  isSelected(id: string): boolean {
    return this.selectedEmpresas.has(id);
  }

  isAllSelected(): boolean {
    return this.empresas.length > 0 && this.empresas.every(e => this.selectedEmpresas.has(e.id));
  }

  // Ações CRUD
  onNew(): void {
    this.isEditMode = false;
    this.currentEmpresaId = null;
    this.modalTitle = 'Nova Empresa';
    this.empresaForm.reset();
    this.fornecedoresAssociados = [];
    this.fornecedoresAssociadosOriginais = [];
    this.loadFornecedoresDisponiveis();
    this.showModal = true;
  }

  onEdit(): void {
    if (this.selectedEmpresas.size === 0) {
      this.notificationService.error('Selecione ao menos um registro para editar');
      return;
    }

    if (this.selectedEmpresas.size > 1) {
      this.notificationService.warning('Selecione apenas um registro para editar');
      return;
    }

    const id = Array.from(this.selectedEmpresas)[0];
    const empresa = this.empresas.find(e => e.id === id);

    if (empresa) {
      this.isEditMode = true;
      this.currentEmpresaId = id;
      this.modalTitle = 'Editar Empresa';
      this.empresaForm.patchValue({
        cnpj: empresa.cnpj,
        name: empresa.name,
        cep: empresa.cep
      });
      this.loadFornecedoresAssociados(id);
      this.loadFornecedoresDisponiveis();
      this.showModal = true;
    }
  }

  onDelete(): void {
    if (this.selectedEmpresas.size === 0) {
      this.notificationService.error('Selecione ao menos um registro para deletar');
      return;
    }

    this.empresasToDelete = Array.from(this.selectedEmpresas);
    this.confirmMessage = `Tem certeza que deseja deletar ${this.empresasToDelete.length} empresa(s)?`;
    this.showConfirmDialog = true;
  }

  confirmDelete(): void {
    this.showConfirmDialog = false;
    this.isLoading = true;

    const deleteRequests = this.empresasToDelete.map(id =>
      this.empresaService.delete(id).toPromise()
    );

    Promise.all(deleteRequests)
      .then(() => {
        this.notificationService.success('Empresa(s) deletada(s) com sucesso');
        this.selectedEmpresas.clear();
        this.loadEmpresas();
      })
      .catch(() => {
        this.isLoading = false;
      });
  }

  cancelDelete(): void {
    this.showConfirmDialog = false;
    this.empresasToDelete = [];
  }

  onSubmit(): void {
    if (this.empresaForm.invalid) {
      this.notificationService.error('Preencha todos os campos corretamente');
      return;
    }

    // Validar CEP usando ViaCEP
    const cepValue = this.empresaForm.value.cep;

    this.isLoading = true;

    // Validar o CEP com a API ViaCEP
    this.cepValidatorService.validateCep(cepValue).subscribe({
      next: (viaCepResponse) => {
        // Verificar se o CEP é válido
        if (viaCepResponse.erro) {
          this.isLoading = false;
          this.notificationService.error('CEP inválido. Por favor, verifique o CEP informado.');
          return;
        }

        // CEP válido, continuar com o salvamento
        this.saveEmpresa();
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('Erro ao validar CEP. Verifique se o CEP está correto.');
      }
    });
  }

  private saveEmpresa(): void {
    const formValue = this.empresaForm.value;
    formValue.cep = parseInt(formValue.cep, 10);

    if (this.isEditMode && this.currentEmpresaId) {
      const empresa: Empresa = {
        id: this.currentEmpresaId,
        ...formValue
      };

      this.empresaService.update(this.currentEmpresaId, empresa).subscribe({
        next: () => {
          this.notificationService.success('Empresa atualizada com sucesso');
          this.closeModal();
          this.loadEmpresas();
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage = this.extractErrorMessage(error, 'Erro ao atualizar empresa');
          this.notificationService.error(errorMessage);
        }
      });
    } else {
      const empresa: Empresa = {
        id: '00000000-0000-0000-0000-000000000000',
        ...formValue
      };

      this.empresaService.create(empresa).subscribe({
        next: () => {
          this.notificationService.success('Empresa criada com sucesso');
          this.closeModal();
          this.loadEmpresas();
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage = this.extractErrorMessage(error, 'Erro ao criar empresa');
          this.notificationService.error(errorMessage);
        }
      });
    }
  }

  closeModal(): void {
    this.showModal = false;
    this.empresaForm.reset();
    this.fornecedoresAssociados = [];
    this.fornecedoresDisponiveis = [];
  }

  // Métodos para gerenciar associações
  loadFornecedoresDisponiveis(): void {
    this.fornecedorService.getPaged(1, 1000).subscribe({
      next: (result) => {
        this.fornecedoresDisponiveis = result.items;
      },
      error: () => {
        this.notificationService.error('Erro ao carregar fornecedores');
      }
    });
  }

  loadFornecedoresAssociados(empresaId: string): void {
    this.empresaFornecedorService.getEmpresaComFornecedores(empresaId).subscribe({
      next: (result) => {
        this.fornecedoresAssociados = result.fornecedores;
        this.fornecedoresAssociadosOriginais = [...result.fornecedores];
      },
      error: () => {
        this.fornecedoresAssociados = [];
        this.fornecedoresAssociadosOriginais = [];
      }
    });
  }

  adicionarFornecedor(fornecedorId: string): void {
    if (!this.currentEmpresaId) {
      this.notificationService.warning('Salve a empresa primeiro antes de adicionar fornecedores');
      return;
    }

    const fornecedor = this.fornecedoresDisponiveis.find(f => f.id === fornecedorId);
    if (!fornecedor) return;

    if (this.fornecedoresAssociados.some(f => f.id === fornecedorId)) {
      this.notificationService.warning('Fornecedor já está associado');
      return;
    }

    const associacao = {
      id: '00000000-0000-0000-0000-000000000000',
      empresaId: this.currentEmpresaId,
      fornecedorId: fornecedorId
    };

    this.empresaFornecedorService.create(associacao).subscribe({
      next: () => {
        this.fornecedoresAssociados.push(fornecedor);
        this.notificationService.success('Fornecedor associado com sucesso');
      },
      error: (error) => {
        // Exibe a mensagem de erro específica retornada pelo backend
        const errorMessage = this.extractErrorMessage(error, 'Erro ao associar fornecedor');
        this.notificationService.error(errorMessage);
      }
    });
  }

  removerFornecedor(fornecedorId: string): void {
    if (!this.currentEmpresaId) return;

    this.empresaFornecedorService.getByEmpresaAndFornecedor(this.currentEmpresaId, fornecedorId).subscribe({
      next: (associacao) => {
        this.empresaFornecedorService.delete(associacao.id).subscribe({
          next: () => {
            this.fornecedoresAssociados = this.fornecedoresAssociados.filter(f => f.id !== fornecedorId);
            this.notificationService.success('Fornecedor desassociado com sucesso');
          },
          error: () => {
            this.notificationService.error('Erro ao desassociar fornecedor');
          }
        });
      },
      error: () => {
        this.notificationService.error('Erro ao buscar associação');
      }
    });
  }

  isFornecedorAssociado(fornecedorId: string): boolean {
    return this.fornecedoresAssociados.some(f => f.id === fornecedorId);
  }

  /**
   * Extrai a mensagem de erro limpa do objeto de erro HTTP
   * Remove o stack trace e retorna apenas a mensagem de validação
   */
  private extractErrorMessage(error: any, defaultMessage: string): string {
    let errorMessage = defaultMessage;

    // Tenta extrair a mensagem de diferentes estruturas de erro
    if (error?.error?.message) {
      errorMessage = error.error.message;
    } else if (typeof error?.error === 'string') {
      errorMessage = error.error;
    }

    // Remove o stack trace (tudo após " at ")
    if (errorMessage.includes(' at ')) {
      errorMessage = errorMessage.split(' at ')[0].trim();
    }

    // Remove prefixos de exceção comuns
    const exceptionPrefixes = [
      'System.ComponentModel.DataAnnotations.ValidationException: ',
      'System.Exception: ',
      'System.InvalidOperationException: ',
      'System.ArgumentException: '
    ];

    for (const prefix of exceptionPrefixes) {
      if (errorMessage.startsWith(prefix)) {
        errorMessage = errorMessage.substring(prefix.length).trim();
        break;
      }
    }

    return errorMessage;
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }
}
