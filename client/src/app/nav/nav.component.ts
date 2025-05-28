import { NgClass } from '@angular/common';
import { Component, effect, inject, OnInit, output, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { AuthService } from '../services/auth.service';
import { FormsModule } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { MenuItem } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  imports: [NgClass, ButtonModule, Dialog, InputTextModule, FormsModule, TieredMenuModule],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent implements OnInit {
  authSvc = inject(AuthService);
  router = inject(Router);
  route = inject(ActivatedRoute);

  darkModeOutput = output<boolean>();

  isDarkMode = signal(false);
  showLogin = signal(false);
  showRegister = signal(false);

  invalidAttempt = toSignal(this.authSvc.invalidLoginAttempt$);
  invalidRegisterErrors = toSignal(this.authSvc.invalidRegisterAttempt$);

  items: MenuItem[]= [
    {
      label: 'Home',
      icon: 'fa-solid fa-house',
      command: () => this.goToHome()
    },
    {
      label: 'Hiscores',
      icon: 'fa-solid fa-list',
      command: () => this.goToHiscores()
    },
    {
      label: 'Logout',
      icon: 'fa-solid fa-arrow-right-from-bracket',
      command: () => {
        this.logout()
      }
    }
  ]
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
    const element = document.querySelector('html');

    if (isDarkMode === 'true' && !element?.classList.contains('darkmode')) {
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
    this.authSvc.register$.next({ email: this.email, password: this.password, loggedIn: false })
  }

  private goToHome() {
    this.router.navigate(['/']);
  }

  private goToHiscores() {
    this.router.navigate(['/leaderboards'], { relativeTo: this.route });
  }

  private logout() {
    this.router.navigate(['/logout'], { relativeTo: this.route });
  }
}
