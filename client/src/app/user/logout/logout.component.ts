import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-logout',
  imports: [RouterLink],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export default class LogoutComponent {

  constructor() {
    const token = localStorage.getItem('token');

    if (token) {
      localStorage.removeItem('token');
    }
  }
}
