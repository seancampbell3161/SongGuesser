import { NgClass } from '@angular/common';
import { Component, inject, output, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { AuthService } from '../services/auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-nav',
  imports: [NgClass, ButtonModule, Dialog, InputTextModule, FormsModule],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
  authSvc = inject(AuthService);

  darkModeOutput = output<boolean>();

  isDarkMode = signal(false);
  showLogin = signal(false);
  showRegister = signal(false);

  email = '';
  password = '';

  toggleDarkMode() {
    const element = document.querySelector('html');
    element?.classList.toggle('darkmode');

    this.isDarkMode.set(element?.classList.contains('darkmode') ?? false);
    this.darkModeOutput.emit(element?.classList.contains('darkmode') ?? false)
  }

  login() {
    this.authSvc.login$.next({ email: this.email, password: this.password });
  }

  register() {

  }
}
