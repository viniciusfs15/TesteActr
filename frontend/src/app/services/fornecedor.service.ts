import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Fornecedor } from '../models/fornecedor.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({
  providedIn: 'root'
})
export class FornecedorService {
  private apiUrl = `${environment.apiBaseUrl}/fornecedor`;

  constructor(private http: HttpClient) { }

  // Obter lista paginada de fornecedores
  getPaged(page: number = 1, pageSize: number = 20): Observable<PagedResult<Fornecedor>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Fornecedor>>(this.apiUrl, { params });
  }

  // Obter fornecedor por ID
  getById(id: string): Observable<Fornecedor> {
    return this.http.get<Fornecedor>(`${this.apiUrl}/${id}`);
  }

  // Obter fornecedor por CNPJ ou CPF
  getByCnpjOrCpf(cnpjOrCpf: string): Observable<Fornecedor> {
    return this.http.get<Fornecedor>(`${this.apiUrl}/cnpjcpf/${cnpjOrCpf}`);
  }

  // Obter fornecedores por nome
  getByName(name: string): Observable<Fornecedor[]> {
    return this.http.get<Fornecedor[]>(`${this.apiUrl}/name/${name}`);
  }

  // Obter fornecedores por email
  getByEmail(email: string): Observable<Fornecedor> {
    return this.http.get<Fornecedor>(`${this.apiUrl}/email/${email}`);
  }

  // Obter fornecedores por CEP
  getByCep(cep: string): Observable<Fornecedor[]> {
    return this.http.get<Fornecedor[]>(`${this.apiUrl}/cep/${cep}`);
  }

  // Criar novo fornecedor
  create(fornecedor: Fornecedor): Observable<Fornecedor> {
    return this.http.post<Fornecedor>(this.apiUrl, fornecedor);
  }

  // Atualizar fornecedor existente
  update(id: string, fornecedor: Fornecedor): Observable<Fornecedor> {
    return this.http.put<Fornecedor>(`${this.apiUrl}/${id}`, fornecedor);
  }

  // Deletar fornecedor
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
