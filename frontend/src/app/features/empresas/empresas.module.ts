import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { EmpresasRoutingModule } from './empresas-routing.module';
import { EmpresaListComponent } from './empresa-list.component';

@NgModule({
  declarations: [
    EmpresaListComponent
  ],
  imports: [
    SharedModule,
    EmpresasRoutingModule
  ],
  exports: [
    EmpresaListComponent
  ]
})
export class EmpresasModule { }
