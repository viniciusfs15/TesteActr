import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ToastComponent } from './toast/toast.component';
import { ModalComponent } from './modal/modal.component';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { PaginationComponent } from './pagination/pagination.component';
import { SpinnerComponent } from './spinner/spinner.component';
import { TopbarComponent } from './topbar/topbar.component';

// Pipes
import { CnpjPipe } from './pipes/cnpj.pipe';
import { CpfPipe } from './pipes/cpf.pipe';
import { CepPipe } from './pipes/cep.pipe';
import { RgPipe } from './pipes/rg.pipe';

@NgModule({
  declarations: [
    ToastComponent,
    ModalComponent,
    ConfirmDialogComponent,
    PaginationComponent,
    SpinnerComponent,
    TopbarComponent,
    CnpjPipe,
    CpfPipe,
    CepPipe,
    RgPipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  exports: [
    ToastComponent,
    ModalComponent,
    ConfirmDialogComponent,
    PaginationComponent,
    SpinnerComponent,
    TopbarComponent,
    CnpjPipe,
    CpfPipe,
    CepPipe,
    RgPipe,
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class SharedModule { }
