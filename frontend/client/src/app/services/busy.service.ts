import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  private spinnerSvc = inject(NgxSpinnerService);

  private busyRequestCount = 0;

  constructor() { }

  busy(): void {
    console.log('busy hit');
    this.busyRequestCount++;

    console.log(this.busyRequestCount);
    this.spinnerSvc.show();
  }

  idle(): void {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.spinnerSvc.hide();
    }
  }
}
