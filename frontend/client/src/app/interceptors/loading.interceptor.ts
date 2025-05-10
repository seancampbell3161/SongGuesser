import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy.service';
import { finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busySvc = inject(BusyService);

  busySvc.busy();

  return next(req).pipe(finalize(() => busySvc.idle()));
};
