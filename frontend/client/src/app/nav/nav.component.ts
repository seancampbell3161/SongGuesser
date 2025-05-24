import { NgClass } from '@angular/common';
import { Component, effect, inject, OnInit, output, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { AuthService } from '../services/auth.service';
import { FormsModule } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

@Component({
  selector: 'app-nav',
  imports: [NgClass, ButtonModule, Dialog, InputTextModule, FormsModule],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent implements OnInit {
  authSvc = inject(AuthService);

  darkModeOutput = output<boolean>();

  isDarkMode = signal(false);
  showLogin = signal(false);
  showRegister = signal(false);

  invalidAttempt = toSignal(this.authSvc.invalidLoginAttempt$);

  email = '';
  password = '';

  constructor() {
    effect(() => {
      if (this.authSvc.isLoggedIn())
        this.showLogin.set(false);
    });
  }

  ngOnInit(): void {
    const isDarkMode = localStorage.getItem('darkmode');

    if (isDarkMode === 'true') {
      this.toggleDarkMode();
    }
  }

  toggleDarkMode() {
    const element = document.querySelector('html');
    element?.classList.toggle('darkmode');

    const isDarkMode = element?.classList.contains('darkmode') ?? false;

    localStorage.setItem('darkmode', isDarkMode.toString())

    this.isDarkMode.set(isDarkMode);
    this.darkModeOutput.emit(isDarkMode);
  }

  login() {
    this.authSvc.login$.next({ email: this.email, password: this.password, loggedIn: false });
  }

  register() {

  }
}
