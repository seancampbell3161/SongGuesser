import { Injectable, inject } from '@angular/core';
import { EMPTY, Subject, catchError, switchMap } from 'rxjs';
import { UploadRequest } from '../interfaces/upload-request';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  private http = inject(HttpClient);
  private readonly url = 'http://localhost:5244/api/music'

  sendUploadRequest$ = new Subject<UploadRequest>();

  constructor() {
    this.sendUploadRequest$.pipe(
      takeUntilDestroyed(),
      switchMap((req) => this.http.post(`${this.url}/convert-and-separate`, req)),
      catchError(() => {
        console.error('error occurred with upload');
        return EMPTY;
      }),
    ).subscribe(res => 
      console.log(JSON.stringify(res))
    );
   }
}
