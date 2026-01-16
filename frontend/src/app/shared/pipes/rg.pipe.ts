import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'rg'
})
export class RgPipe implements PipeTransform {
  transform(value: string | number): string {
    if (!value) return '';

    const rg = value.toString().replace(/\D/g, '');

    if (rg.length < 7) {
      return value.toString();
    }

    // Formato: XX.XXX.XXX-X (padrão mais comum)
    if (rg.length === 9) {
      return rg.replace(/(\d{2})(\d{3})(\d{3})(\d{1})/, '$1.$2.$3-$4');
    }

    return value.toString();
  }
}
