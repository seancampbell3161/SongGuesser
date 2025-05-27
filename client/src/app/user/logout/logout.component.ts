import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-logout',
  imports: [RouterLink],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export default class LogoutComponent {
  private authSvc = inject(AuthService)

  constructor() {
    const token = localStorage.getItem('token');

    if (token) {
      localStorage.removeItem('token');
      this.authSvc.clearToken();
    }
  }
}
