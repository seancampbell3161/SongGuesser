import { Component } from '@angular/core';
import { SongComponent } from '../song/song.component';

@Component({
  selector: 'app-home',
  imports: [SongComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export default class HomeComponent {

}
