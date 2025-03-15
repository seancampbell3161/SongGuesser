import { Component } from '@angular/core';
import { SongComponent } from '../song/song.component';
import UploadComponent from '../upload/upload.component';

@Component({
  selector: 'app-home',
  imports: [SongComponent, UploadComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export default class HomeComponent {

}
