import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { FornecedoresRoutingModule } from './fornecedores-routing.module';
import { FornecedorListComponent } from './fornecedor-list.component';

@NgModule({
  declarations: [
    FornecedorListComponent
  ],
  imports: [
    SharedModule,
    FornecedoresRoutingModule
  ],
  exports: [
    FornecedorListComponent
  ]
})
export class FornecedoresModule { }
