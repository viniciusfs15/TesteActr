import { Component } from '@angular/core';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.css']
})
export class TopbarComponent {
  showUserMenu = false;

  constructor(private notificationService: NotificationService) { }

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
  }

  closeUserMenu(): void {
    this.showUserMenu = false;
  }

  onProfile(): void {
    this.showUserMenu = false;
    this.notificationService.info('Funcionalidade de Perfil em construção');
  }

  onLogout(): void {
    this.showUserMenu = false;
    this.notificationService.info('Funcionalidade de Sair em construção');
  }
}
