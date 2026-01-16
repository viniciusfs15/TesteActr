import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Empresa } from '../models/empresa.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({
  providedIn: 'root'
})
export class EmpresaService {
  private apiUrl = `${environment.apiBaseUrl}/empresa`;

  constructor(private http: HttpClient) { }

  // Obter lista paginada de empresas
  getPaged(page: number = 1, pageSize: number = 20): Observable<PagedResult<Empresa>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Empresa>>(this.apiUrl, { params });
  }

  // Obter empresa por ID
  getById(id: string): Observable<Empresa> {
    return this.http.get<Empresa>(`${this.apiUrl}/${id}`);
  }

  // Obter empresa por CNPJ
  getByCnpj(cnpj: string): Observable<Empresa> {
    return this.http.get<Empresa>(`${this.apiUrl}/cnpj/${cnpj}`);
  }

  // Obter empresas por CNPJ contendo caracteres
  getByCnpjContains(chars: string): Observable<Empresa[]> {
    return this.http.get<Empresa[]>(`${this.apiUrl}/cnpj/contains/${chars}`);
  }

  // Obter empresas por nome
  getByName(name: string): Observable<Empresa[]> {
    return this.http.get<Empresa[]>(`${this.apiUrl}/name/${name}`);
  }

  // Obter empresas por CEP
  getByCep(cep: string): Observable<Empresa[]> {
    return this.http.get<Empresa[]>(`${this.apiUrl}/cep/${cep}`);
  }

  // Criar nova empresa
  create(empresa: Empresa): Observable<Empresa> {
    return this.http.post<Empresa>(this.apiUrl, empresa);
  }

  // Atualizar empresa existente
  update(id: string, empresa: Empresa): Observable<Empresa> {
    return this.http.put<Empresa>(`${this.apiUrl}/${id}`, empresa);
  }

  // Deletar empresa
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
