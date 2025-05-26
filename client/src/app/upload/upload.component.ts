import { Component, inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UploadService } from './data-access/upload.service';
import { UploadRequest } from './interfaces/upload-request';

@Component({
  selector: 'app-upload',
  imports: [ReactiveFormsModule],
  templateUrl: './upload.component.html',
  styleUrl: './upload.component.css'
})
export default class UploadComponent {
  private fb = inject(FormBuilder);
  private uploadSvc = inject(UploadService);

  uploadForm: FormGroup;

  constructor() {
    this.uploadForm = this.fb.group({
      url: new FormControl(''),
      songTitle: new FormControl(''),
      artist: new FormControl('')
    });
  }

  onSubmit() {
    console.log(JSON.stringify(this.uploadForm.value));
    if (!this.uploadForm.get('url')?.value) {
      console.error('url is required');
      return;
    }
    if (!this.uploadForm.get('songTitle')?.value) {
      console.error('song title is required');
      return;
    }
    if (!this.uploadForm.get('artist')?.value) {
      console.error('artist name is required');
      return;
    }
    this.uploadSvc.sendUploadRequest$.next(this.uploadForm.value as UploadRequest);
  }
}
