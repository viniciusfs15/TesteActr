import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ViaCepResponse {
  cep: string;
  logradouro: string;
  complemento: string;
  bairro: string;
  localidade: string;
  uf: string;
  ibge: string;
  gia: string;
  ddd: string;
  siafi: string;
  erro?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CepValidatorService {
  private viaCepUrl = 'https://viacep.com.br/ws';

  constructor(private http: HttpClient) { }

  validateCep(cep: string): Observable<ViaCepResponse> {
    // Remove caracteres especiais
    const cleanCep = cep.replace(/\D/g, '');

    if (cleanCep.length !== 8) {
      throw new Error('CEP deve conter 8 dígitos');
    }

    return this.http.get<ViaCepResponse>(`${this.viaCepUrl}/${cleanCep}/json/`);
  }
}
