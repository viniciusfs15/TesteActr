import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { EmpresaComFornecedores, FornecedorComEmpresas, EmpresaFornecedor } from '../models/empresa-fornecedor.model';

@Injectable({
  providedIn: 'root'
})
export class EmpresaFornecedorService {
  private apiUrl = `${environment.apiBaseUrl}/empresafornecedor`;

  constructor(private http: HttpClient) { }

  getEmpresaComFornecedores(empresaId: string): Observable<EmpresaComFornecedores> {
    return this.http.get<EmpresaComFornecedores>(`${this.apiUrl}/empresa/${empresaId}`);
  }

  getFornecedorComEmpresas(fornecedorId: string): Observable<FornecedorComEmpresas> {
    return this.http.get<FornecedorComEmpresas>(`${this.apiUrl}/fornecedor/${fornecedorId}`);
  }

  getByEmpresaAndFornecedor(empresaId: string, fornecedorId: string): Observable<EmpresaFornecedor> {
    return this.http.get<EmpresaFornecedor>(`${this.apiUrl}/empresa/${empresaId}/fornecedor/${fornecedorId}`);
  }

  create(empresaFornecedor: EmpresaFornecedor): Observable<EmpresaFornecedor> {
    return this.http.post<EmpresaFornecedor>(this.apiUrl, empresaFornecedor);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
