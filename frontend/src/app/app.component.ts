import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  activeTab: 'empresas' | 'fornecedores' = 'empresas';

  setActiveTab(tab: 'empresas' | 'fornecedores'): void {
    this.activeTab = tab;
  }
}
