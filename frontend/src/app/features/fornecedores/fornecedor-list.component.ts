import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FornecedorService } from '../../services/fornecedor.service';
import { EmpresaService } from '../../services/empresa.service';
import { EmpresaFornecedorService } from '../../services/empresa-fornecedor.service';
import { CepValidatorService } from '../../services/cep-validator.service';
import { Fornecedor, FornecedorType } from '../../models/fornecedor.model';
import { Empresa } from '../../models/empresa.model';
import { NotificationService } from '../../core/services/notification.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-fornecedor-list',
  templateUrl: './fornecedor-list.component.html',
  styleUrls: ['./fornecedor-list.component.css']
})
export class FornecedorListComponent implements OnInit {
  fornecedores: Fornecedor[] = [];
  selectedFornecedores: Set<string> = new Set();
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  hasNext = false;
  isLoading = false;

  FornecedorType = FornecedorType;

  // Filtros por coluna
  filters = {
    name: '',
    email: '',
    type: ''
  };

  // Modal
  showModal = false;
  modalTitle = '';
  fornecedorForm: FormGroup;
  isEditMode = false;
  currentFornecedorId: string | null = null;

  // Associações de empresas
  empresasDisponiveis: Empresa[] = [];
  empresasAssociadas: Empresa[] = [];
  empresasAssociadasOriginais: Empresa[] = [];

  // Diálogo de confirmação
  showConfirmDialog = false;
  confirmMessage = '';
  fornecedoresToDelete: string[] = [];

  constructor(
    private fornecedorService: FornecedorService,
    private empresaService: EmpresaService,
    private empresaFornecedorService: EmpresaFornecedorService,
    private cepValidatorService: CepValidatorService,
    private notificationService: NotificationService,
    private fb: FormBuilder
  ) {
    this.fornecedorForm = this.fb.group({
      type: [FornecedorType.PessoaJuridica, Validators.required],
      cnpj: [''],
      cpf: [''],
      rg: [''],
      birthDate: [''],
      name: ['', Validators.required],
      cep: ['', [Validators.required, Validators.pattern(/^\d{8}$/)]],
      email: ['', [Validators.required, Validators.email]]
    });

    // Atualizar validações quando o tipo muda
    this.fornecedorForm.get('type')?.valueChanges.subscribe(type => {
      this.updateValidations(type);
    });
  }
  ngOnInit(): void {
    this.loadFornecedores();
  }

  updateValidations(type: FornecedorType): void {
    const cnpjControl = this.fornecedorForm.get('cnpj');
    const cpfControl = this.fornecedorForm.get('cpf');
    const rgControl = this.fornecedorForm.get('rg');
    const birthDateControl = this.fornecedorForm.get('birthDate');

    // Limpar validações
    cnpjControl?.clearValidators();
    cpfControl?.clearValidators();
    rgControl?.clearValidators();
    birthDateControl?.clearValidators();

    if (type === FornecedorType.PessoaJuridica) {
      cnpjControl?.setValidators([Validators.required, Validators.pattern(/^\d{14}$/)]);
    } else {
      cpfControl?.setValidators([Validators.required, Validators.pattern(/^\d{11}$/)]);
      rgControl?.setValidators([Validators.required]);
      birthDateControl?.setValidators([Validators.required]);
    }

    cnpjControl?.updateValueAndValidity();
    cpfControl?.updateValueAndValidity();
    rgControl?.updateValueAndValidity();
    birthDateControl?.updateValueAndValidity();
  }

  loadFornecedores(): void {
    this.isLoading = true;

    if (this.filters.name || this.filters.email || this.filters.type) {
      this.applyFilters();
    } else {
      this.fornecedorService.getPaged(this.currentPage, this.pageSize).subscribe({
        next: (result) => {
          this.fornecedores = result.items;
          this.totalCount = result.totalCount;
          this.hasNext = result.hasNext;
          this.isLoading = false;
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage = this.extractErrorMessage(error, 'Erro ao carregar fornecedores');
          this.notificationService.error(errorMessage);
        }
      });
    }
  }

  applyFilters(): void {
    this.isLoading = true;
    const observables = [];

    if (this.filters.name) {
      observables.push(this.fornecedorService.getByName(this.filters.name));
    }
    if (this.filters.email) {
      observables.push(this.fornecedorService.getByEmail(this.filters.email));
    }

    if (observables.length === 0 && this.filters.type) {
      this.fornecedorService.getPaged(this.currentPage, this.pageSize).subscribe({
        next: (result) => {
          this.fornecedores = result.items.filter(f => f.type.toString() === this.filters.type);
          this.totalCount = this.fornecedores.length;
          this.hasNext = false;
          this.isLoading = false;
        },
        error: () => {
          // Não exibe mensagem de erro, apenas limpa os resultados
          this.fornecedores = [];
          this.totalCount = 0;
          this.isLoading = false;
        }
      });
      return;
    }

    if (observables.length === 0) {
      this.loadFornecedores();
      return;
    }

    forkJoin(observables).subscribe({
      next: (results: any[]) => {
        const fornecedoresMap = new Map<string, Fornecedor>();
        results.forEach(result => {
          if (Array.isArray(result)) {
            result.forEach(forn => fornecedoresMap.set(forn.id, forn));
          } else if (result && result.id) {
            fornecedoresMap.set(result.id, result);
          }
        });
        this.fornecedores = Array.from(fornecedoresMap.values());

        if (this.filters.type) {
          this.fornecedores = this.fornecedores.filter(f => f.type.toString() === this.filters.type);
        }

        this.totalCount = this.fornecedores.length;
        this.hasNext = false;
        this.isLoading = false;
      },
      error: () => {
        // Não exibe mensagem de erro, apenas limpa os resultados
        this.fornecedores = [];
        this.totalCount = 0;
        this.isLoading = false;
      }
    });
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadFornecedores();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadFornecedores();
  }

  toggleSelection(id: string): void {
    if (this.selectedFornecedores.has(id)) {
      this.selectedFornecedores.delete(id);
    } else {
      this.selectedFornecedores.add(id);
    }
  }

  toggleSelectAll(event: any): void {
    if (event.target.checked) {
      this.fornecedores.forEach(f => this.selectedFornecedores.add(f.id));
    } else {
      this.selectedFornecedores.clear();
    }
  }

  isSelected(id: string): boolean {
    return this.selectedFornecedores.has(id);
  }

  isAllSelected(): boolean {
    return this.fornecedores.length > 0 && this.fornecedores.every(f => this.selectedFornecedores.has(f.id));
  }

  onNew(): void {
    this.isEditMode = false;
    this.currentFornecedorId = null;
    this.modalTitle = 'Novo Fornecedor';
    this.fornecedorForm.reset({ type: FornecedorType.PessoaJuridica });
    this.updateValidations(FornecedorType.PessoaJuridica);
    this.empresasAssociadas = [];
    this.empresasAssociadasOriginais = [];
    this.loadEmpresasDisponiveis();
    this.showModal = true;
  }

  onEdit(): void {
    if (this.selectedFornecedores.size === 0) {
      this.notificationService.error('Selecione ao menos um registro para editar');
      return;
    }

    if (this.selectedFornecedores.size > 1) {
      this.notificationService.warning('Selecione apenas um registro para editar');
      return;
    }

    const id = Array.from(this.selectedFornecedores)[0];
    const fornecedor = this.fornecedores.find(f => f.id === id);

    if (fornecedor) {
      this.isEditMode = true;
      this.currentFornecedorId = id;
      this.modalTitle = 'Editar Fornecedor';
      this.fornecedorForm.patchValue({
        type: fornecedor.type,
        cnpj: fornecedor.cnpj || '',
        cpf: fornecedor.cpf || '',
        rg: fornecedor.rg || '',
        birthDate: fornecedor.birthDate ? fornecedor.birthDate.split('T')[0] : '',
        name: fornecedor.name,
        cep: fornecedor.cep,
        email: fornecedor.email
      });
      this.updateValidations(fornecedor.type);
      this.loadEmpresasAssociadas(id);
      this.loadEmpresasDisponiveis();
      this.showModal = true;
    }
  }

  onDelete(): void {
    if (this.selectedFornecedores.size === 0) {
      this.notificationService.error('Selecione ao menos um registro para deletar');
      return;
    }

    this.fornecedoresToDelete = Array.from(this.selectedFornecedores);
    this.confirmMessage = `Tem certeza que deseja deletar ${this.fornecedoresToDelete.length} fornecedor(es)?`;
    this.showConfirmDialog = true;
  }

  confirmDelete(): void {
    this.showConfirmDialog = false;
    this.isLoading = true;

    const deleteRequests = this.fornecedoresToDelete.map(id =>
      this.fornecedorService.delete(id).toPromise()
    );

    Promise.all(deleteRequests)
      .then(() => {
        this.notificationService.success('Fornecedor(es) deletado(s) com sucesso');
        this.selectedFornecedores.clear();
        this.loadFornecedores();
      })
      .catch(() => {
        this.isLoading = false;
      });
  }

  cancelDelete(): void {
    this.showConfirmDialog = false;
    this.fornecedoresToDelete = [];
  }

  onSubmit(): void {
    if (this.fornecedorForm.invalid) {
      this.notificationService.error('Preencha todos os campos corretamente');
      return;
    }

    // Validar CEP usando ViaCEP apenas para novos registros ou se o CEP foi alterado
    const cepValue = this.fornecedorForm.value.cep;

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
        this.saveFornecedor();
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('Erro ao validar CEP. Verifique se o CEP está correto.');
      }
    });
  }

  private saveFornecedor(): void {
    const formValue = this.fornecedorForm.value;
    formValue.cep = parseInt(formValue.cep, 10);
    formValue.type = parseInt(formValue.type, 10);

    // Limpar campos não usados baseado no tipo
    if (formValue.type === FornecedorType.PessoaJuridica) {
      formValue.cpf = null;
      formValue.rg = null;
      formValue.birthDate = null;
    } else {
      formValue.cnpj = null;
    }

    if (this.isEditMode && this.currentFornecedorId) {
      const fornecedor: Fornecedor = {
        id: this.currentFornecedorId,
        ...formValue
      };

      this.fornecedorService.update(this.currentFornecedorId, fornecedor).subscribe({
        next: () => {
          this.notificationService.success('Fornecedor atualizado com sucesso');
          this.closeModal();
          this.loadFornecedores();
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage = this.extractErrorMessage(error, 'Erro ao atualizar fornecedor');
          this.notificationService.error(errorMessage);
        }
      });
    } else {
      const fornecedor: Fornecedor = {
        id: '00000000-0000-0000-0000-000000000000',
        ...formValue
      };

      this.fornecedorService.create(fornecedor).subscribe({
        next: () => {
          this.notificationService.success('Fornecedor criado com sucesso');
          this.closeModal();
          this.loadFornecedores();
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage = this.extractErrorMessage(error, 'Erro ao criar fornecedor');
          this.notificationService.error(errorMessage);
        }
      });
    }
  }

  closeModal(): void {
    this.showModal = false;
    this.fornecedorForm.reset();
    this.empresasAssociadas = [];
    this.empresasDisponiveis = [];
  }

  // Métodos para gerenciar associações
  loadEmpresasDisponiveis(): void {
    this.empresaService.getPaged(1, 1000).subscribe({
      next: (result) => {
        this.empresasDisponiveis = result.items;
      },
      error: () => {
        this.notificationService.error('Erro ao carregar empresas');
      }
    });
  }

  loadEmpresasAssociadas(fornecedorId: string): void {
    this.empresaFornecedorService.getFornecedorComEmpresas(fornecedorId).subscribe({
      next: (result) => {
        this.empresasAssociadas = result.empresas;
        this.empresasAssociadasOriginais = [...result.empresas];
      },
      error: () => {
        this.empresasAssociadas = [];
        this.empresasAssociadasOriginais = [];
      }
    });
  }

  adicionarEmpresa(empresaId: string): void {
    if (!this.currentFornecedorId) {
      this.notificationService.warning('Salve o fornecedor primeiro antes de adicionar empresas');
      return;
    }

    const empresa = this.empresasDisponiveis.find(e => e.id === empresaId);
    if (!empresa) return;

    if (this.empresasAssociadas.some(e => e.id === empresaId)) {
      this.notificationService.warning('Empresa já está associada');
      return;
    }

    const associacao = {
      id: '00000000-0000-0000-0000-000000000000',
      empresaId: empresaId,
      fornecedorId: this.currentFornecedorId
    };

    this.empresaFornecedorService.create(associacao).subscribe({
      next: () => {
        this.empresasAssociadas.push(empresa);
        this.notificationService.success('Empresa associada com sucesso');
      },
      error: (error) => {
        // Exibe a mensagem de erro específica retornada pelo backend
        const errorMessage = this.extractErrorMessage(error, 'Erro ao associar empresa');
        this.notificationService.error(errorMessage);
      }
    });
  }

  removerEmpresa(empresaId: string): void {
    if (!this.currentFornecedorId) return;

    this.empresaFornecedorService.getByEmpresaAndFornecedor(empresaId, this.currentFornecedorId).subscribe({
      next: (associacao) => {
        this.empresaFornecedorService.delete(associacao.id).subscribe({
          next: () => {
            this.empresasAssociadas = this.empresasAssociadas.filter(e => e.id !== empresaId);
            this.notificationService.success('Empresa desassociada com sucesso');
          },
          error: () => {
            this.notificationService.error('Erro ao desassociar empresa');
          }
        });
      },
      error: () => {
        this.notificationService.error('Erro ao buscar associação');
      }
    });
  }

  isEmpresaAssociada(empresaId: string): boolean {
    return this.empresasAssociadas.some(e => e.id === empresaId);
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  getTipoText(type: FornecedorType): string {
    return type === FornecedorType.PessoaFisica ? 'Pessoa Física' : 'Pessoa Jurídica';
  }

  getDocumento(fornecedor: Fornecedor): string {
    if (fornecedor.type === FornecedorType.PessoaFisica) {
      const cpf = fornecedor.cpf || '';
      return cpf ? this.formatCpf(cpf) : '-';
    } else {
      const cnpj = fornecedor.cnpj || '';
      return cnpj ? this.formatCnpj(cnpj) : '-';
    }
  }

  private formatCpf(cpf: string): string {
    const cleaned = cpf.replace(/\D/g, '');
    if (cleaned.length === 11) {
      return cleaned.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
    }
    return cpf;
  }

  private formatCnpj(cnpj: string): string {
    const cleaned = cnpj.replace(/\D/g, '');
    if (cleaned.length === 14) {
      return cleaned.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
    }
    return cnpj;
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
}
